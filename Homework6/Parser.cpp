#include "Parser.h"
#include <iostream>
#include <fstream>
#include <sstream>
#include <algorithm>

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
        cout << variable << "=" << it->second << endl;
    }
    else
    {
        cerr << "RUNTIME ERROR: line " << numberOfLine << endl;
        key = false;
    }
}

void Parser::handleAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
    // Handle assignment
    // You need to implement this function according to your new approach
}

void Parser::handleAdditionAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
}

void Parser::handleSubtractionAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
}

void Parser::handleMultiplicationAssignment(const string &variable, const string &value, const vector<string> &tokens)
{
}

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