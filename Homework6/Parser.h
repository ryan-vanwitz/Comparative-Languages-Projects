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
    int numberOfLine = 1;
    std::map<std::string, std::string> variables; // Map to store variables and their values
    bool key = true;                              // Flag to control parsing process

    void handleForLoop(const std::vector<std::string> &tokens);
    void handlePrint(const std::string &variable);
    void handleAssignment(const std::string &variable, const std::string &value, const std::vector<std::string> &tokens);
    void handleAdditionAssignment(const string &variable, const string &value, const vector<string> &tokens);
    void handleSubtractionAssignment(const string &variable, const string &value, const vector<string> &tokens);
    void handleMultiplicationAssignment(const string &variable, const string &value, const vector<string> &tokens);
    void handleOperator(const std::vector<std::string> &tokens);
    void interpretLine(const std::string &line);
};

#endif // PARSER_H