using System;
using System.IO;
using System.Collections.Generic;

public class Hw4
{
  public static void Main(string[] args)
  {
    // Capture the start time
    DateTime startTime = DateTime.Now;

    // ============================
    // Do not add or change anything above, inside the 
    // Main method
    // ============================

    List<Zipcode> zipcodes = new List<Zipcode>();
    string filePath = "zipcodes.txt"; // Update with the correct file path
    int insufficientFieldCount = 0; // Track the number of times insufficient fields occur

    try
    {
      using (StreamReader sr = new StreamReader(filePath))
      {
        string line;
        int lineNumber = 0;
        while ((line = sr.ReadLine()) != null)
        {
          lineNumber++;
          // Split the line by tab delimiter
          string[] fields = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

          // Check if the line has enough fields
          if (fields.Length < 20)
          {
            insufficientFieldCount++;
            // Print error message for the first few occurrences only
            if (insufficientFieldCount <= 10)
            {
              Console.WriteLine($"Error: Insufficient fields in line {lineNumber}: '{line}'");
            }
            else if (insufficientFieldCount == 11)
            {
              Console.WriteLine("Too many lines with insufficient fields. Further occurrences will not be displayed.");
            }
            continue; // Skip processing this line further
          }

          try
          {
            // Parse fields and create Zipcode object
            Zipcode zipcode = new Zipcode(
                int.Parse(fields[0]),
                fields[1],
                fields[2],
                fields[3],
                fields[4],
                fields[5],
                double.Parse(fields[6]),
                double.Parse(fields[7]),
                double.Parse(fields[8]),
                double.Parse(fields[9]),
                double.Parse(fields[10]),
                fields[11],
                fields[12],
                fields[13],
                fields[14],
                bool.Parse(fields[15]),
                int.Parse(fields[16]),
                int.Parse(fields[17]),
                int.Parse(fields[18]),
                fields[19]
            );
            // Add zipcode to list
            zipcodes.Add(zipcode);
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Error parsing line {lineNumber}: '{line}'. {ex.Message}");
          }
        }
      }
    }
    catch (Exception e)
    {
      Console.WriteLine("Error reading the file: " + e.Message);
    }

    // Display the first 10 zipcodes for demonstration
    for (int i = 0; i < 10 && i < zipcodes.Count; i++)
    {
      Console.WriteLine(zipcodes[i]);
    }

    // Display the total number of lines with insufficient fields
    if (insufficientFieldCount > 0)
    {
      Console.WriteLine($"Total lines with insufficient fields: {insufficientFieldCount}");
    }

    // ============================
    // Do not add or change anything below, inside the 
    // Main method
    // ============================

    // Capture the end time
    DateTime endTime = DateTime.Now;

    // Calculate the elapsed time
    TimeSpan elapsedTime = endTime - startTime;

    // Display the elapsed time in milliseconds
    Console.WriteLine($"Elapsed Time: {elapsedTime.TotalMilliseconds} ms");
  }
}
