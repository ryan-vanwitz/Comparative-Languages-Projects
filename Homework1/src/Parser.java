/*
 * Parser.java
 * This class is responsible for parsing and interpreting a custom scripting language.
 * It reads from a file and executes commands accordingly.
 * 
 * Copyright (c) 2024 Ryan Van Witzenburg and ChatGPT. All rights reserved.
 */

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * The Parser class parses and interprets commands from a file written in a custom scripting language.
 */
public class Parser {
    private Map<String, Object> variables; // Map to store variables and their values
    boolean key = true; // Flag to control parsing process
    int numberOfLine = 1;

    /**
     * Constructor for Parser class. Initializes the variables map.
     */
    public Parser() {
        variables = new HashMap<>();
    }

    /**
     * Reads and interprets commands from the specified file.
     *
     * @param fileName the name of the file to be parsed
     */
    public void run(String fileName) {
        try (BufferedReader br = new BufferedReader(new FileReader(fileName))) { // Open the file for reading
            String line;
            while ((line = br.readLine()) != null) { // Read each line from the file
                interpretLine(line); // Interpret and execute the command from the line
            }
        } catch (IOException e) { // Catch and handle IOException
            System.err.println("Error reading file: " + e.getMessage()); // Print error message
        }
    }

    /**
     * Interprets a single line of code from the script.
     *
     * @param line the line to be interpreted
     */
    public void interpretLine(String line) {
        if(line.isEmpty()) { // Check if the line is empty
            return; // If so, return without further processing
        }

        String[] tokens = line.split("\\s+"); // Split the line into tokens
        if ((!tokens[tokens.length - 1].equals("ENDFOR") && !tokens[tokens.length - 1].equals(";"))) {
            // Check for syntax errors in the line
            System.err.println("Syntax error: " + line); // Print syntax error message
            return; // Return without further processing
        }

        if (key) { // Check if parsing is enabled
            switch (tokens[0]) { // Determine the type of command based on the first token
                case "FOR":
                    handleForLoop(tokens); // Handle a for loop command
                    numberOfLine++;
                    break;
                case "PRINT":
                    handlePrint(tokens[1]); // Handle a print command
                    numberOfLine++;
                    break;
                default:
                    handleOperator(tokens); // Handle other operator commands
                    numberOfLine++;
            }
        }
    }

    /**
     * Interprets and executes an operator command.
     *
     * @param tokens the tokens representing the operator command
     */
    private void handleOperator(String[] tokens) {
        String variable = tokens[0]; // Get the variable name
        String operator = tokens[1]; // Get the operator
        String value = tokens[2]; // Get the value

        switch (operator) { // Determine the type of operator
            case "=":
                handleAssignment(variable, value, tokens); // Handle assignment
                break;
            case "+=":
                handleAdditionAssignment(variable, value, tokens); // Handle addition assignment
                break;
            case "-=":
                handleSubtractionAssignment(variable, value, tokens); // Handle subtraction assignment
                break;
            case "*=":
                handleMultiplicationAssignment(variable, value, tokens); // Handle multiplication assignment
                break;
            default:
            	System.err.println("Unknown operator: " + operator + " at line " + numberOfLine); // Handle unknown operator
                key = false; // Disable further parsing
        }
    }

    /**
     * Handles the assignment operator command.
     *
     * @param variable the variable to be assigned a value
     * @param value    the value to assign to the variable
     * @param tokens   the tokens representing the assignment command
     */
    private void handleAssignment(String variable, String value, String[] tokens) {
    	 // System.out.println("Variable: " + variable + ", Value: " + value);
    	 try {
    	        // Attempt to parse the value as an integer
    	        int intValue;
    	        if (value.startsWith("-")) {
    	            intValue = Integer.parseInt(value.substring(1)); // Parse without the negative sign
    	            intValue *= -1; // Adjust the parsed value to be negative
    	        } else {
    	            intValue = Integer.parseInt(value);
    	        }
    	        // Store the parsed value
    	        variables.put(variable, intValue);
    	    } catch (NumberFormatException e) {
    	        // If parsing as integer fails, store as string
    	        variables.put(variable, value);
    	    }
    }

    /**
     * Handles addition assignment operation.
     *
     * @param variable the variable to be modified
     * @param value    the value to be added to the variable
     * @param tokens   the tokens representing the addition assignment command
     */
    private void handleAdditionAssignment(String variable, String value, String[] tokens) {
        Object varValue = variables.get(variable); // Get the current value of the variable
        Object parsedValue = parseVariable(value); // Parse the value to be added

        if (varValue instanceof Integer && parsedValue instanceof Integer) { // Check if both values are integers
            variables.put(variable, (Integer) varValue + (Integer) parsedValue); // Perform addition and update variable
        } else if (varValue instanceof String && parsedValue instanceof String) { // Check if both values are strings
            variables.put(variable, (String) varValue + (String) parsedValue); // Concatenate strings and update variable
        } else { // Handle incompatible types
            System.err.println("RUNTIME ERROR: line " + numberOfLine); // Print error message
            key = false; // Disable further parsing
        }
    }

    /**
     * Handles subtraction assignment operation.
     *
     * @param variable the variable to be modified
     * @param value    the value to be subtracted from the variable
     * @param tokens   the tokens representing the subtraction assignment command
     */
    private void handleSubtractionAssignment(String variable, String value, String[] tokens) {
        Object varValue = variables.get(variable); // Get the current value of the variable
        Object parsedValue = parseVariable(value); // Parse the value to be subtracted

        if (varValue instanceof Integer && parsedValue instanceof Integer) { // Check if both values are integers
            variables.put(variable, (Integer) varValue - (Integer) parsedValue); // Perform subtraction and update variable
        } else { // Handle incompatible types
            System.err.println("RUNTIME ERROR: line " + numberOfLine); // Print error message
            key = false; // Disable further parsing
        }
    }

    /**
     * Handles multiplication assignment operation.
     *
     * @param variable the variable to be modified
     * @param value    the value to multiply the variable by
     * @param tokens   the tokens representing the multiplication assignment command
     */
    private void handleMultiplicationAssignment(String variable, String value, String[] tokens) {
        Object varValue = variables.get(variable); // Get the current value of the variable
        Object parsedValue = parseVariable(value); // Parse the value to multiply with

        if (varValue instanceof Integer && parsedValue instanceof Integer) { // Check if both values are integers
            variables.put(variable, (Integer) varValue * (Integer) parsedValue); // Perform multiplication and update variable
        } else { // Handle incompatible types
            System.err.println("RUNTIME ERROR: line " + numberOfLine); // Print error message
            key = false; // Disable further parsing
        }
    }

    /**
     * Handles the execution of a for loop command.
     * Iterates over the specified number of iterations and executes the loop body.
     *
     * @param tokens the tokens representing the for loop command
     */
    private void handleForLoop(String[] tokens) {
        if (!tokens[tokens.length - 1].equals("ENDFOR")) {
            // Check for syntax errors in the for loop command
            System.err.println("Syntax error: " + String.join(" ", tokens)); // Print syntax error message
            key = false; // Disable further parsing
            return; // Exit method
        } 

        int iterations;
        try {
            // Parse the number of iterations from the second token
            iterations = Integer.parseInt(tokens[1]);
        } catch (NumberFormatException e) {
            System.err.println("RUNTIME ERROR: Invalid loop iteration count at line " + numberOfLine); // Print error message
            key = false; // Disable further parsing
            return; // Exit method
        }
        
        // Iterate over parameters for the specified number of iterations
        int j = 2; // Start index for parameters
        for (int i = 0; i < iterations; i++) {
            while (j < tokens.length - 1) { // Iterate until the end of the tokens
                String parameter = tokens[j] + " " + tokens[j+1] + " " + tokens[j+2] + " " + tokens[j+3]; // Construct parameter string
                interpretLine(parameter.trim()); // Interpret and execute the parameter
                j += 4; // Move to the next parameter
            }
            j = 2; // Reset index for parameters
        }
    }

    /**
     * Prints the value of a variable to the console.
     * If the variable is not assigned, prints a runtime error message.
     *
     * @param variable the name of the variable to be printed
     */
    private void handlePrint(String variable) {
        Object value = variables.get(variable); // Get the value of the variable
        if (value != null) { // Check if the variable is assigned
            System.out.println((variable + "=" + value).replace("\"", "")); // Print variable name and value
        } else {
            System.err.println("RUNTIME ERROR: line " + numberOfLine); // Print error message
            key = false; // Disable further parsing
        }
    }
    
    /**
     * Checks if a given string represents an integer.
     *
     * @param value the string to be checked
     * @return true if the string represents an integer, false otherwise
     */
    private boolean isInteger(String value) {
        if (value == null || value.isEmpty()) {
            return false; // Empty string or null is not an integer
        }
        for (char c : value.toCharArray()) {
            if (!(Character.isDigit(c))) {
                return false; // If any character is not a digit, it's not an integer
            }
        }
        return true; // All characters are digits, it's an integer
    }
    
    /**
     * Parses the value of a variable.
     * If the value is a string enclosed in double quotes, removes the quotes.
     * If the value is a variable name, retrieves its value from the variables map.
     *
     * @param value the value to be parsed
     * @return the parsed value
     */
    private Object parseVariable(String value) {
    	try {
            if (value.startsWith("-")) {
                // If the value starts with a negative sign, parse the substring excluding the negative sign
                return Integer.parseInt(value.substring(1)) * -1;
            } else if(isInteger(value)) { // Check if the value is an integer
                return Integer.parseInt(value); // Parse and return integer value
            } else {
                if (value.startsWith("\"") && value.endsWith("\"") && value.length() >= 2) {
                    return value.substring(1, value.length()-1); // Remove surrounding quotes from a string
                } else {
                    Object retrievedValue = variables.get(value); // Retrieve value from variables map
                    if (retrievedValue instanceof String) { // Check if the value is a string
                        String stringValue = (String) retrievedValue;
                        if (stringValue.startsWith("\"") && stringValue.endsWith("\"") && stringValue.length() >= 2) {
                            return stringValue.substring(1, stringValue.length()-1); // Remove surrounding quotes from retrieved string value
                        }
                    }
                    return retrievedValue; // Return retrieved value
                }
            }
        } catch (NumberFormatException e) { // Catch NumberFormatException
            return value; // Return value as is
        }
    }
}