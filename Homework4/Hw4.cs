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

    GenerateLatLonFile();

    GenerateCityStatesFile();


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

  public static void GenerateCommonCityNamesFile(Action<string> processCityDelegate = null)
  {
    try
    {
      // Read the list of states from states.txt
      List<string> states = File.ReadAllLines("states.txt").ToList();

      // Create a HashSet to store common city names
      HashSet<string> commonCityNames = new HashSet<string>();

      // Iterate through each state to find common city names
      foreach (string state1 in states)
      {
        foreach (string state2 in states)
        {
          // Skip if both states are the same
          if (state1 == state2)
            continue;

          // Read the file containing zip codes data for the current state
          string filePath = "zipcodes.txt";
          if (File.Exists(filePath))
          {
            // Read city names from the current state's file
            List<string> cities1 = File.ReadAllLines(filePath)
                                        .Where(line => line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[4].Trim().Equals(state1, StringComparison.OrdinalIgnoreCase))
                                        .Select(line => line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[3].Trim())
                                        .ToList();

            List<string> cities2 = File.ReadAllLines(filePath)
                                        .Where(line => line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[4].Trim().Equals(state2, StringComparison.OrdinalIgnoreCase))
                                        .Select(line => line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[3].Trim())
                                        .ToList();

            // Find common cities between the two states
            IEnumerable<string> commonCities = cities1.Intersect(cities2);

            if (processCityDelegate != null)
            {
              foreach (string city in commonCities)
              {
                processCityDelegate(city); // Process each city using the delegate
              }
            }

            if (commonCityNames.Count == 0)
            {
              // First state pair, add all common cities to the common set
              commonCityNames.UnionWith(commonCities);
            }
            else
            {
              // Remove cities that are not common in this state pair
              commonCityNames.IntersectWith(commonCities);
            }
          }
        }
      }

      // Sort the common city names alphabetically
      List<string> sortedCommonCityNames = commonCityNames.OrderBy(city => city).ToList();

      // Output the common city names to CommonCityNames.txt
      File.WriteAllLines("CommonCityNames.txt", sortedCommonCityNames);

      /* Console.WriteLine("Common city names written to CommonCityNames.txt:");
         foreach (var city in sortedCommonCityNames)
         {
           Console.WriteLine(city);
         } */
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }


  public static void GenerateLatLonFile()
  {
    // Read the list of zip codes from zips.txt
    List<string> zipCodes = File.ReadAllLines("zips.txt").ToList();

    // Create a dictionary to store latitude and longitude for each zip code
    Dictionary<string, Tuple<double, double>> latLonMap = new Dictionary<string, Tuple<double, double>>();

    // Read zip code data from zipcodes.txt and populate the dictionary
    using (StreamReader sr = new StreamReader("zipcodes.txt"))
    {
      string line;
      while ((line = sr.ReadLine()) != null)
      {
        string[] fields = line.Split(new char[] { '\t' }, StringSplitOptions.None);

        // Debugging output
        // Console.WriteLine($"Fields array length: {fields.Length}");
        //Console.WriteLine($"Line: {line}");

        try
        {
          string zipCode = fields[1]; // Assuming zip code is at index 1
          double latitude = 0.0;
          double longitude = 0.0;

          // Check if latitude and longitude fields are present and parse them
          if (fields.Length > 6)
          {
            double.TryParse(fields[6], out latitude);
          }
          if (fields.Length > 7)
          {
            double.TryParse(fields[7], out longitude);
          }

          // If the zip code is not already in the dictionary and latitude/longitude are valid, add it
          if (!latLonMap.ContainsKey(zipCode) && latitude != 0.0 && longitude != 0.0)
          {
            latLonMap.Add(zipCode, new Tuple<double, double>(latitude, longitude));
          }
        }
        catch (FormatException ex)
        {
          Console.WriteLine($"Error parsing latitude or longitude: {ex.Message}");
          Console.WriteLine($"Line content: {line}");
        }
      }
    }

    // Create or overwrite LatLon.txt and write the latitude and longitude for each zip code
    using (StreamWriter sw = new StreamWriter("LatLon.txt"))
    {
      foreach (string zipCode in zipCodes)
      {
        // Check if the zip code exists in the dictionary
        if (latLonMap.ContainsKey(zipCode))
        {
          // Get the latitude and longitude for the zip code
          Tuple<double, double> latLon = latLonMap[zipCode];
          double latitude = latLon.Item1;
          double longitude = latLon.Item2;

          // Write the zip code, latitude, and longitude to the file
          sw.WriteLine($"{zipCode} {latitude} {longitude}");
        }
        else
        {
          // If the zip code does not have corresponding latitude and longitude, write an error message
          sw.WriteLine($"Error: Latitude and longitude not found for {zipCode}");
        }
      }
    }

    Console.WriteLine("LatLon.txt generated successfully.");
  }

  public static void GenerateCityStatesFile()
  {
    try
    {
      // Read the list of cities from cities.txt
      List<string> cities = File.ReadAllLines("cities.txt").ToList();
      Console.WriteLine($"Cities from cities.txt: {string.Join(", ", cities)}");

      // Create a dictionary to store states for each city
      Dictionary<string, HashSet<string>> cityStateMap = new Dictionary<string, HashSet<string>>();

      // Iterate through zip codes
      using (StreamReader sr = new StreamReader("zipcodes.txt"))
      {
        string line;
        while ((line = sr.ReadLine()) != null)
        {
          string[] fields = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
          if (fields.Length >= 5) // Ensure there are enough fields
          {
            string city = fields[3].Trim(); // Assuming city name is at index 3
            string state = fields[4].Trim(); // Assuming state name is at index 4

            // Console.WriteLine($"Read City: {city}, State: {state}");

            // Check if the city is in the list of cities we're interested in
            if (cities.Any(c => c.Equals(city, StringComparison.OrdinalIgnoreCase)))
            {
              // Console.WriteLine($"City: {city}, State: {state}");
              // Add the state to the city's set of states
              if (!cityStateMap.ContainsKey(city))
              {
                cityStateMap[city] = new HashSet<string>();
              }
              cityStateMap[city].Add(state);
            }
          }
        }
      }

      // Create or overwrite CityStates.txt and write the states for each city
      using (StreamWriter sw = new StreamWriter("CityStates.txt"))
      {
        foreach (string city in cityStateMap.Keys)
        {
          // Get the states for the city and sort them alphabetically
          List<string> states = cityStateMap[city].OrderBy(s => s).ToList();

          // Write the city and its states to the file
          sw.WriteLine($"{city}\t{string.Join(" ", states)}");
        }
      }

      Console.WriteLine("CityStates.txt generated successfully.");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }

}
