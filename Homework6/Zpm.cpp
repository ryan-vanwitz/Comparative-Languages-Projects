// Copyright 2024 Ryan Van Witzenburg and ChatGPT
#include <iostream>
#include "Parser.h"

using namespace std;

int main(int argc, char *argv[])
{
    if (argc != 2)
    {
        cout << "Need to add C++ Zpm file to run statement" << endl;
        return 1;
    }

    string fileName = argv[1];
    Parser parser;
    parser.run(fileName);

    return 0;
}
