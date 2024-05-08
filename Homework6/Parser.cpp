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
    // Handle a for loop command
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
        else
        {
            cout << variable << "=" << it->second << endl; // Print integer variable without double quotes
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
                cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
                key = false;
            }
        }
    }
}

/* void Parser::handleAdditionAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
}

void Parser::handleSubtractionAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
}

void Parser::handleMultiplicationAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
} */

void Parser::handleOperator(const vector<string> &tokens)
{
    string variable = tokens[0]; // Get the variable name
    string op = tokens[1];       // Get the operator
    string value = tokens[2];    // Get the value

    if (op == "=")
    {
        handleAssignment(variable, value, tokens); // Handle assignment
    }
    else if (op == "+=")
    {
        // handleAdditionAssignment(variable, value, tokens); // Handle addition assignment
    }
    else if (op == "-=")
    {
        // handleSubtractionAssignment(variable, value, tokens); // Handle subtraction assignment
    }
    else if (op == "*=")
    {
        // handleMultiplicationAssignment(variable, value, tokens); // Handle multiplication assignment
    }
    else
    {
        cerr << "Unknown operator: " << op << " at line " << numberOfLine << endl; // Handle unknown operator
        key = false;                                                               // Disable further parsing
    }
}

void Parser::interpretLine(const string &line)
{
    if (line.empty())
    {           // Check if the line is empty
        return; // If so, return without further processing
    }

    istringstream iss(line);
    string token;
    vector<string> tokens;
    while (iss >> token)
    {
        tokens.push_back(token);
    }

    if (tokens.empty())
    {
        return;
    }

    if (tokens[tokens.size() - 1] != "ENDFOR" && tokens[tokens.size() - 1] != ";")
    {
        // Check for syntax errors in the line
        cerr << "Syntax error: " << line << endl; // Print syntax error message
        return;                                   // Return without further processing
    }

    if (key)
    { // Check if parsing is enabled
        if (tokens[0] == "FOR")
        {
            handleForLoop(tokens); // Handle a for loop command
            numberOfLine++;
        }
        else if (tokens[0] == "PRINT")
        {
            handlePrint(tokens[1]); // Handle a print command
            numberOfLine++;
        }
        else
        {
            handleOperator(tokens); // Handle other operator commands
            numberOfLine++;
        }
    }
}