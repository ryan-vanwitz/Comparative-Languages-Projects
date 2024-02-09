/*
 * Zpm.java
 * This class contains the main method to execute the Zpm program.
 * Zpm is a command-line tool for running scripts written in a custom scripting language.
 * 
 * Copyright (c) 2024 Ryan Van Witzenburg and ChatGPT. All rights reserved.
 */

/**
 * The Zpm class contains the main method to execute the Zpm program.
 * Zpm is a command-line tool for running scripts written in a custom scripting language.
 */
public class Zpm {

    /**
     * The main method of the Zpm program.
     * It takes a single command-line argument representing the filename of the script to be executed.
     * If the correct number of arguments is not provided, it prints an error message and exits.
     * Otherwise, it creates an instance of the Parser class, runs the script, and executes it.
     *
     * @param args the command-line arguments
     */
    public static void main(String[] args) {
        if (args.length != 1) {
            System.out.println("Need to add java Zpm file to run statement");
            return;
        }

        String fileName = args[0]; // Get the filename from command-line arguments
        Parser parser = new Parser(); // Create an instance of the Parser class
        parser.run(fileName); // Run the script specified by the filename
    }

}