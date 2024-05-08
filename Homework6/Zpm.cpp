#include <iostream>
#include <fstream>
#include <sstream>
#include <map>
#include <vector>

using namespace std;

int numberOfLine = 1;
map<string, int> variables; // Map to store variables and their values
bool key = true;            // Flag to control parsing process

void handleForLoop(const vector<string> &tokens)
{
    // Handle a for loop command
}

void handlePrint(const string &variable)
{
    auto it = variables.find(variable); // Get the iterator to the variable in the map
    if (it != variables.end())
    {                                                  // Check if the variable is assigned
        cout << variable << "=" << it->second << endl; // Print variable name and value
    }
    else
    {
        cerr << "RUNTIME ERROR: line " << numberOfLine << endl; // Print error message
        key = false;                                            // Disable further parsing
    }
}

void handleAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
    try
    {
        // Attempt to parse the value as an integer
        int intValue;
        if (value[0] == '-')
        {
            intValue = stoi(value.substr(1)); // Parse without the negative sign
            intValue *= -1;                   // Adjust the parsed value to be negative
        }
        else
        {
            intValue = stoi(value);
        }
        // Store the parsed value
        variables[variable] = to_string(intValue);
    }
    catch (const invalid_argument &)
    {
        // If parsing as integer fails, store as string
        variables[variable] = value;
    }
}

void handleOperator(const vector<string> &tokens)
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

void interpretLine(const string &line)
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

int main(int argc, char *argv[])
{
    if (argc != 2)
    {
        cout << "Need to add C++ Zpm file to run statement" << endl;
        return 1;
    }

    string fileName = argv[1]; // Get the filename from command-line arguments
    ifstream file(fileName);   // Open the file for reading
    if (file.is_open())
    {
        string line;
        while (getline(file, line))
        {                        // Read each line from the file
            interpretLine(line); // Interpret and execute the command from the line
        }
        file.close(); // Close the file
    }
    else
    {
        cerr << "Error opening file: " << fileName << endl; // Print error message
        return 1;
    }

    return 0;
}
