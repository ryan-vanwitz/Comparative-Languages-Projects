using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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

    List<Zipcode> zipcodesList = new List<Zipcode>();
    string filePath = "zipcodes.txt"; // Update with the correct file path
    int insufficientFieldCount = 0; // Track the number of times insufficient fields occur

    try
    {
      // Read and parse the file
      ParseZipcodesFromFile(filePath, zipcodesList, out insufficientFieldCount);
    }
    catch (Exception e)
    {
      Console.WriteLine("Error reading the file: " + e.Message);
    }

    // Display the first 10 zipcodes for demonstration
    for (int i = 0; i < 10 && i < zipcodesList.Count; i++)
    {
      Console.WriteLine(zipcodesList[i]);
    }

    // Display the total number of lines with insufficient fields
    if (insufficientFieldCount > 0)
    {
      Console.WriteLine($"Total lines with insufficient fields: {insufficientFieldCount}");
    }

    GenerateCommonCityNamesFile();


    // Capture the end time
    DateTime endTime = DateTime.Now;

    // Calculate the elapsed time
    TimeSpan elapsedTime = endTime - startTime;

    // Display the elapsed time in milliseconds
    Console.WriteLine($"Elapsed Time: {elapsedTime.TotalMilliseconds} ms");
  }

  private static void ParseZipcodesFromFile(string filePath, List<Zipcode> zipcodesList, out int insufficientFieldCount)
  {
    insufficientFieldCount = 0;

    using (StreamReader sr = new StreamReader(filePath))
    {
      string line;
      int lineNumber = 0;

      // Skip the header line
      sr.ReadLine();
      lineNumber++;

      while ((line = sr.ReadLine()) != null)
      {
        lineNumber++;

        // Split the line by tab delimiter
        string[] parsedFields = line.Split(new char[] { '\t' }, StringSplitOptions.None);

        // Initialize fields array with correct size and set all elements to null
        string[] fields = new string[20];
        for (int i = 0; i < fields.Length; i++)
        {
          fields[i] = null;
        }

        // Copy parsed fields into the fields array
        for (int i = 0; i < parsedFields.Length && i < fields.Length; i++)
        {
          fields[i] = parsedFields[i];
        }

        // Check if the line has insufficient fields
        if (parsedFields.Length < 10000000)
        {
          insufficientFieldCount++;
          continue; // Skip this line and proceed to the next one
        }

        try
        {
          double latitude, longitude;

          // Try parsing latitude and longitude
          if (!double.TryParse(fields[6], out latitude) || !double.TryParse(fields[7], out longitude))
          {
            // Skip this line if latitude or longitude parsing fails
            Console.WriteLine($"Skipping line {lineNumber}: '{line}' due to invalid latitude or longitude.");
            continue;
          }

          // Parse other fields and create Zipcode object
          Zipcode zipcode = new Zipcode(
              int.Parse(fields[0]),
              fields[1],
              fields[2],
              fields[3],
              fields[4],
              fields[5],
              latitude,
              longitude,
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
          zipcodesList.Add(zipcode);

          // Print the created Zipcode object
          Console.WriteLine($"Created Zipcode: {zipcode}");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error parsing line {lineNumber}: '{line}'. {ex.Message}");
        }
      }
    }
  }

  public static void GenerateCommonCityNamesFile()
  {
    // Read the list of states from states.txt
    List<string> states = File.ReadAllLines("states.txt").ToList();

    // Create a HashSet to store common city names
    HashSet<string> commonCityNames = new HashSet<string>();

    // Iterate through each state to find common city names
    foreach (string state in states)
    {
      Console.WriteLine($"Processing state: {state}");

      // Read the file containing zip codes data for the current state
      string filePath = "zipcodes.txt";
      if (File.Exists(filePath))
      {
        // Read city names from the current state's file
        List<string> cities = File.ReadAllLines(filePath)
                                    .Select(line => line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[3].Trim())
                                    //.Where(city => string.Equals(city, state, StringComparison.OrdinalIgnoreCase)) // Filter city names matching the current state
                                    .ToList();

        Console.WriteLine($"Cities for state {state}: {string.Join(", ", cities)}");

        if (commonCityNames.Count == 0)
        {
          // First state, add all cities to the common set
          commonCityNames.UnionWith(cities);
        }
        else
        {
          // Remove cities that are not common in this state
          commonCityNames.IntersectWith(cities);
        }
      }
    }

    // Sort the common city names alphabetically
    List<string> sortedCommonCityNames = commonCityNames.OrderBy(city => city).ToList();

    // Output the common city names to CommonCityNames.txt
    File.WriteAllLines("CommonCityNames.txt", sortedCommonCityNames);

    Console.WriteLine("Common city names written to CommonCityNames.txt:");
    foreach (var city in sortedCommonCityNames)
    {
      Console.WriteLine(city);
    }
  }

}
