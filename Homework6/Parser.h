// Copyright 2024 Ryan Van Witzenburg and ChatGPT
#ifndef PARSER_H
#define PARSER_H

#include <string>
#include <map>
#include <vector>

class Parser
{
public:
    void run(const std::string &fileName);

private:
    int numberOfLine = 0;
    std::map<std::string, std::string> variables; // Map to store variables and their values
    bool key = true;                              // Flag to control parsing process

    void handleForLoop(const std::vector<std::string> &tokens);
    void handlePrint(const std::string &variable);
    void handleAssignment(const std::string &variable, const std::string &value, const std::vector<std::string> &tokens);
    void handleAdditionAssignment(const std::string &variable, const std::string &value, const std::vector<std::string> &tokens);
    void handleSubtractionAssignment(const std::string &variable, const std::string &value, const std::vector<std::string> &tokens);
    void handleMultiplicationAssignment(const std::string &variable, const std::string &value, const std::vector<std::string> &tokens);
    void handleOperator(const std::vector<std::string> &tokens);
    void interpretLine(const std::string &line);
    std::string parseVariable(const std::string &value);
    bool isInteger(const std::string &str);
};

#endif // PARSER_H
