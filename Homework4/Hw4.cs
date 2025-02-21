//Copyright @2024 Ryan Van Witzenburg and ChatGPT

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using words = System.String;

public class Hw4
{
  public static void Main(words[] args)
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

  // ChatGPT aided in the making of this method
  private unsafe static void ParseZipcodesFromFile(string filePath, List<Zipcode> zipcodesList, out int insufficientFieldCount)
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

        // Use unsafe context for pointer manipulation
        fixed (char* linePtr = line)
        {
          char* ptr = linePtr;

          // Initialize variables to store field values
          int recordNumber = 0;
          string code = null;
          string zipCodeType = null;
          string city = null;
          string state = null;
          string locationType = null;
          double lat = 0.0;
          double longitude = 0.0;
          double xaxis = 0.0;
          double yaxis = 0.0;
          double zaxis = 0.0;
          string worldRegion = null;
          string country = null;
          string locationText = null;
          string location = null;
          bool decommissioned = false;
          int taxReturnsFiled = 0;
          int estimatedPopulation = 0;
          int totalWages = 0;
          string notes = null;

          // Find the tab delimiter and extract fields
          for (int i = 0; i < 20; i++)
          {
            // Find the next tab delimiter
            while (*ptr != '\t')
            {
              if (*ptr == '\0') // Reached end of line before expected number of fields
              {
                insufficientFieldCount++;
                goto EndOfLine;
              }
              ptr++;
            }

            // Replace the tab delimiter with null terminator
            *ptr = '\0';

            // Store the field's start pointer
            char* fieldStart = linePtr;

            // Move to the next character after the null terminator
            ptr++;

            // Process the field based on its position
            switch (i)
            {
              case 0:
                recordNumber = int.Parse(new string(fieldStart));
                break;
              case 1:
                code = new string(fieldStart);
                break;
              case 2:
                zipCodeType = new string(fieldStart);
                break;
              case 3:
                city = new string(fieldStart);
                break;
              case 4:
                state = new string(fieldStart);
                break;
              case 5:
                locationType = new string(fieldStart);
                break;
              case 6:
                double.TryParse(new string(fieldStart), out lat);
                break;
              case 7:
                double.TryParse(new string(fieldStart), out longitude);
                break;
              case 8:
                double.TryParse(new string(fieldStart), out xaxis);
                break;
              case 9:
                double.TryParse(new string(fieldStart), out yaxis);
                break;
              case 10:
                double.TryParse(new string(fieldStart), out zaxis);
                break;
              case 11:
                worldRegion = new string(fieldStart);
                break;
              case 12:
                country = new string(fieldStart);
                break;
              case 13:
                locationText = new string(fieldStart);
                break;
              case 14:
                location = new string(fieldStart);
                break;
              case 15:
                bool.TryParse(new string(fieldStart), out decommissioned);
                break;
              case 16:
                int.TryParse(new string(fieldStart), out taxReturnsFiled);
                break;
              case 17:
                int.TryParse(new string(fieldStart), out estimatedPopulation);
                break;
              case 18:
                int.TryParse(new string(fieldStart), out totalWages);
                break;
              case 19:
                notes = new string(fieldStart);
                break;
            }
          }

          // Create Zipcode object and add it to the list
          Zipcode zipcode = new Zipcode(
              recordNumber,
              code,
              zipCodeType,
              city,
              state,
              locationType,
              lat,
              longitude,
              xaxis,
              yaxis,
              zaxis,
              worldRegion,
              country,
              locationText,
              location,
              decommissioned,
              taxReturnsFiled,
              estimatedPopulation,
              totalWages,
              notes
          );
          zipcodesList.Add(zipcode);
        }

      EndOfLine:;
      }
    }
  }

  // ChatGPT aided in the making of this method
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

      Console.WriteLine("CommonCityNames.txt generated successfully.");

    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }


  // ChatGPT aided in the making of this method
  public static void GenerateLatLonFile(bool? successfulParsing = null)
  {
    // Read the list of zip codes from zips.txt
    List<string> zipCodes = File.ReadAllLines("zips.txt").ToList();

    // Create a dictionary to store latitude and longitude for each zip code
    Dictionary<string, Tuple<double?, double?>> latLonMap = new Dictionary<string, Tuple<double?, double?>>();

    // Read zip code data from zipcodes.txt and populate the dictionary
    using (StreamReader sr = new StreamReader("zipcodes.txt"))
    {
      string line;
      while ((line = sr.ReadLine()) != null)
      {
        string[] fields = line.Split(new char[] { '\t' }, StringSplitOptions.None);

        try
        {
          string zipCode = fields[1]; // Assuming zip code is at index 1
          double? latitude = null;
          double? longitude = null;

          // Check if latitude and longitude fields are present and parse them
          if (fields.Length > 6)
          {
            double tempLatitude;
            if (double.TryParse(fields[6], out tempLatitude))
            {
              latitude = tempLatitude;
            }
          }
          if (fields.Length > 7)
          {
            double tempLongitude;
            if (double.TryParse(fields[7], out tempLongitude))
            {
              longitude = tempLongitude;
            }
          }

          // Add zip code and its latitude/longitude to the dictionary if it doesn't already exist
          if (!latLonMap.ContainsKey(zipCode))
          {
            latLonMap.Add(zipCode, new Tuple<double?, double?>(latitude, longitude));
          }
          else
          {
            // Handle duplicate zip code if needed
            // You can choose to ignore it, update the existing entry, or handle it in any other appropriate way
            // Console.WriteLine($"Duplicate zip code found: {zipCode}");
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
          Tuple<double?, double?> latLon = latLonMap[zipCode];
          double? latitude = latLon.Item1;
          double? longitude = latLon.Item2;

          // Write the zip code, latitude, and longitude to the file
          sw.WriteLine($"{latitude ?? 0.0} {longitude ?? 0.0}");
        }
        else
        {
          // If the zip code does not have corresponding latitude and longitude, write an error message
          sw.WriteLine($"Error: Latitude and longitude not found for {zipCode}");
        }
      }
    }

    Console.WriteLine("LatLon.txt generated successfully.");

    // Check if successfulParsing parameter is provided and print a message accordingly
    if (successfulParsing.HasValue)
    {
      if (successfulParsing.Value)
      {
        Console.WriteLine("Latitude and longitude parsing was successful for all zip codes.");
      }
      else
      {
        Console.WriteLine("Latitude and longitude parsing failed for one or more zip codes.");
      }
    }
  }

  // ChatGPT aided in the making of this method
  public static void GenerateCityStatesFile()
  {
    try
    {
      // Read the list of cities from cities.txt
      List<string> cities = File.ReadAllLines("cities.txt").ToList();
      // Console.WriteLine($"Cities from cities.txt: {string.Join(", ", cities)}");

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
          sw.WriteLine($"{string.Join(" ", states)}");
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
