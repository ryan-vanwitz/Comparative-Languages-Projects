#include "Parser.h"
#include <iostream>
#include <fstream>
#include <sstream>
#include <algorithm>
#include <string>
#include <map>
#include <vector>

using namespace std;

void Parser::run(const string &fileName)
{
    ifstream file(fileName);

    if (file.is_open())
    {
        string line;
        while (getline(file, line))
        {
            numberOfLine++;
            interpretLine(line);
        }
        file.close();
    }
    else
    {
        cerr << "Error opening file: " << fileName << endl;
        exit(1);
    }
}

void Parser::handleForLoop(const vector<string> &tokens)
{
    // Check if the for loop has the correct number of tokens
    if (tokens.size() < 6 || tokens[tokens.size() - 1] != "ENDFOR")
    {
        // Check for syntax errors in the for loop command
        cerr << "Syntax error: ";
        for (const auto &token : tokens)
        {
            cerr << token << " ";
        }
        cerr << endl; // Print syntax error message
        key = false;  // Disable further parsing
        return;       // Exit method
    }

    // Parse the number of iterations
    int iterations;
    if (isInteger(tokens[1]))
    {
        iterations = stoi(tokens[1]); // If it's an integer value
    }
    else
    {
        // Try to find the variable in the map
        auto it = variables.find(tokens[1]);
        if (it != variables.end() && isInteger(it->second))
        {
            iterations = stoi(it->second); // If found and it's an integer, use its value
        }
        else
        {
            // Otherwise, it's an error
            cerr << "RUNTIME ERROR: Invalid loop iteration count at line " << numberOfLine << endl;
            key = false;
            return;
        }
    }

    // Iterate over parameters for the specified number of iterations
    int j = 2; // Start index for parameters
    for (int i = 0; i < iterations; i++)
    {
        while (j < tokens.size() - 1) // Iterate until the end of the tokens
        {
            string parameter = tokens[j] + " " + tokens[j + 1] + " " + tokens[j + 2] + " " + tokens[j + 3]; // Construct parameter string
            interpretLine(parameter);                                                                       // Interpret and execute the parameter
            j += 4;                                                                                         // Move to the next parameter
        }
        j = 2; // Reset index for parameters
    }
}

void Parser::handlePrint(const string &variable)
{
    auto it = variables.find(variable);
    if (it != variables.end())
    {
        if (it->second.front() == '"' && it->second.back() == '"')
        {
            cout << variable << "=" << it->second << endl; // Print string variable with double quotes
        }
        else if (all_of(it->second.begin(), it->second.end(), ::isdigit) || (it->second.front() == '-' && all_of(it->second.begin() + 1, it->second.end(), ::isdigit)))
        {
            cout << variable << "=" << it->second << endl; // Print integer variable without double quotes
        }
        else
        {
            cout << variable << "=\"" << it->second << "\"" << endl; // Print string variable with double quotes
        }
    }
    else
    {
        cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
        key = false;
    }
}

void Parser::handleAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
    // cout << "Variable: " << variable << "     Value: " << value << endl;
    // cout << "Assignment: " << variable << " = " << value << endl; // Debug print statement

    if (value.front() == '"' && value.back() == '"')
    {
        // If the value is enclosed in double quotes, it's a string
        variables[variable] = value.substr(1, value.size() - 2); // Remove the quotes
    }
    else
    {
        // Otherwise, try to parse it as an integer or assign from another variable
        auto it = variables.find(value);
        if (it != variables.end())
        {
            variables[variable] = it->second;
        }
        else
        {
            try
            {
                int intValue = stoi(value);
                variables[variable] = to_string(intValue); // Store as string
            }
            catch (const invalid_argument &)
            {
                cerr << "RUNTIME ERROR: line " << numberOfLine << "It was the saoce " << value << endl;
                key = false;
            }
        }
    }
}

void Parser::handleAdditionAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
    auto it = variables.find(variable);
    // cout << "Addition Assignment: " << variable << " += " << value << endl; // Debug print statement

    if (it != variables.end())
    {
        // Get the current value of the variable
        string currentValue = it->second;

        // Parse the value to be added
        string parsedValue = parseVariable(value);

        // cout << "Current Value: " << currentValue << endl;
        // cout << "Parsed Value: " << parsedValue << endl;

        // Check if both values are integers
        if (isInteger(currentValue) && isInteger(parsedValue))
        {
            // Perform addition for integers
            // cout << "Negative 5" << endl;
            int result = stoi(currentValue) + stoi(parsedValue);
            // cout << "Result: " << result << endl;
            variables[variable] = to_string(result);
        }
        else if (!isInteger(currentValue) && !isInteger(parsedValue))
        {
            // Concatenate strings if both values are strings
            variables[variable] = currentValue + parsedValue;
        }
        else
        {
            // Handle incompatible types
            cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
            key = false;
        }
    }
    else
    {
        cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
        key = false;
    }
}

string Parser::parseVariable(const string &value)
{
    // Check if the value is an integer
    if (isInteger(value))
    {
        return value;
    }
    else
    {
        // Remove surrounding quotes from a string
        if (value.front() == '"' && value.back() == '"' && value.length() >= 2)
        {
            return value.substr(1, value.length() - 2);
        }
        else if ((value.front() == '-' && isInteger(value.substr(1))) || isInteger(value))
        {
            // Handle negative integer or positive integer
            return value;
        }
        else
        {
            // Retrieve value from variables map
            auto it = variables.find(value);
            if (it != variables.end())
            {
                return it->second;
            }
            else
            {
                // Handle invalid value
                cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
                key = false;
                return "";
            }
        }
    }
}

bool Parser::isInteger(const string &str)
{
    // Check if the string represents an integer
    if (str.empty())
    {
        return false;
    }

    // Check if the string starts with a negative sign
    size_t start = (str[0] == '-') ? 1 : 0;

    // Check if all characters after the optional negative sign are digits
    return all_of(str.begin() + start, str.end(), ::isdigit);
}

void Parser::handleSubtractionAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
    auto it = variables.find(variable);
    if (it != variables.end())
    {
        // Get the current value of the variable
        string currentValue = it->second;

        // Parse the value to be multiplied
        string parsedValue = parseVariable(value);

        // Check if the variable is an integer
        if (isInteger(currentValue) && isInteger(parsedValue))
        {
            // Perform subtraction for integers
            try
            {
                int result = stoi(currentValue) - stoi(parsedValue);
                variables[variable] = to_string(result);
            }
            catch (const invalid_argument &)
            {
                cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
                key = false;
                return;
            }
        }
        else
        {
            // Handle incompatible types (e.g., subtraction of string with an integer)
            cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
            key = false;
            return;
        }
    }
    else
    {
        cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
        key = false;
    }
}

void Parser::handleMultiplicationAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
    auto it = variables.find(variable);
    if (it != variables.end())
    {
        // Get the current value of the variable
        string currentValue = it->second;

        // Parse the value to be multiplied
        string parsedValue = parseVariable(value);

        // Check if the variable is an integer
        if (isInteger(currentValue) && isInteger(parsedValue))
        {
            // Perform multiplication for integers
            try
            {
                int result = stoi(currentValue) * stoi(parsedValue);
                variables[variable] = to_string(result);
            }
            catch (const invalid_argument &)
            {
                cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
                key = false;
                return;
            }
        }
        else
        {
            // Handle incompatible types (e.g., multiplication of string with an integer)
            cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
            key = false;
            return;
        }
    }
    else
    {
        cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
        key = false;
    }
}
void Parser::handleOperator(const vector<string> &tokens)
{
    string variable = tokens[0]; // Get the variable name
    string op = tokens[1];       // Get the operator
    string value;
    if (tokens[2] == "\"")
    {
        value = "\" \"";
    }
    else
    {
        value = tokens[2]; // Get the value
    }

    // cout << "Operator: " << op << ", Variable: " << variable << ", Value: " << tokens[2] << endl; // Debug statement

    if (op == "=")
    {
        handleAssignment(variable, value, tokens); // Handle assignment
    }
    else if (op == "+=")
    {
        handleAdditionAssignment(variable, value, tokens); // Handle addition assignment
    }
    else if (op == "-=")
    {
        handleSubtractionAssignment(variable, value, tokens); // Handle subtraction assignment
    }
    else if (op == "*=")
    {
        handleMultiplicationAssignment(variable, value, tokens); // Handle multiplication assignment
    }
    else
    {
        cerr << "Unknown operator: " << op << " at line " << numberOfLine << endl; // Handle unknown operator
        key = false;                                                               // Disable further parsing
    }
}

void Parser::interpretLine(const string &line)
{
    // numberOfLine++;

    if (line.empty())
    {
        // numberOfLine++;
        return;
    }

    // Tokenize the line based on spaces
    istringstream iss(line);
    vector<string> tokens;
    string token;
    while (iss >> token)
    {
        // Check if the token starts with a double quote
        if (token.front() == '"')
        {
            // Concatenate tokens until the end of the string is found
            string concatenatedToken = token;
            while (concatenatedToken.back() != '"')
            {
                if (!(iss >> token))
                {
                    // End of line reached before the end of the string
                    cerr << "Syntax error: " << line << endl;
                    return;
                }
                concatenatedToken += " " + token;
            }
            tokens.push_back(concatenatedToken);
        }
        else
        {
            tokens.push_back(token);
            // numberOfLine++;
        }
    }

    // Debug output to check tokens
    /* cout << "Tokens: ";
    for (const auto &t : tokens)
    {
        cout << t << " ";
    }
    cout << endl; */

    if (tokens.empty())
    {
        return;
    }

    // Check for syntax errors in the line
    if (tokens[tokens.size() - 1] != "ENDFOR" && tokens[tokens.size() - 1] != ";")
    {
        cerr << "Syntax error: " << line << endl;
        return;
    }

    if (key)
    {
        if (tokens[0] == "FOR")
        {
            handleForLoop(tokens);
            // numberOfLine++;
        }
        else if (tokens[0] == "PRINT")
        {
            handlePrint(tokens[1]);
            // numberOfLine++;
        }
        else
        {
            handleOperator(tokens);
            // numberOfLine++;
        }
    }
}