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

void Parser::handleOperator(const vector<string> &tokens)
{
    // Handle operator
    // You need to implement this function according to your new approach
}

void Parser::interpretLine(const string &line)
{
    // Interpret line
    // You need to implement this function according to your new approach
}