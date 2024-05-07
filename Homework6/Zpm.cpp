#include <iostream>
#include <string>

using namespace std;

int main(int argc, char *argv[])
{
    if (argc != 2)
    {
        cout << "Need to add C++ Zpm file to run statement" << endl;
        return 1;
    }

    string fileName = argv[1]; // Get the filename from command-line arguments
    // Code to run the script specified by the filename
    cout << "Running script: " << fileName << endl;

    return 0;
}
