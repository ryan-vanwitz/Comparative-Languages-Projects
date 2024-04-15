using System;

// Define the Zipcode class
public class Zipcode
{
    // Properties to represent different attributes of a zipcode
    public int RecordNumber { get; set; }
    public string Code { get; set; }
    public string ZipCodeType { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string LocationType { get; set; }
    public double Lat { get; set; }
    public double Long { get; set; }
    public double Xaxis { get; set; }
    public double Yaxis { get; set; }
    public double Zaxis { get; set; }
    public string WorldRegion { get; set; }
    public string Country { get; set; }
    public string LocationText { get; set; }
    public string Location { get; set; }
    public bool Decommissioned { get; set; }
    public int TaxReturnsFiled { get; set; }
    public int EstimatedPopulation { get; set; }
    public int TotalWages { get; set; }
    public string Notes { get; set; }

    // Constructor to initialize zipcode object
    public Zipcode(int recordNumber, string code, string zipCodeType, string city, string state, string locationType, double lat, double longitude, double xaxis, double yaxis, double zaxis, string worldRegion, string country, string locationText, string location, bool decommissioned, int taxReturnsFiled, int estimatedPopulation, int totalWages, string notes)
    {
        RecordNumber = recordNumber;
        Code = code;
        ZipCodeType = zipCodeType;
        City = city;
        State = state;
        LocationType = locationType;
        Lat = lat;
        Long = longitude;
        Xaxis = xaxis;
        Yaxis = yaxis;
        Zaxis = zaxis;
        WorldRegion = worldRegion;
        Country = country;
        LocationText = locationText;
        Location = location;
        Decommissioned = decommissioned;
        TaxReturnsFiled = taxReturnsFiled;
        EstimatedPopulation = estimatedPopulation;
        TotalWages = totalWages;
        Notes = notes;
    }

    // Override ToString method to provide a string representation of the zipcode object
    public override string ToString()
    {
        return $"{RecordNumber}\t{Code}\t{ZipCodeType}\t{City}\t{State}\t{LocationType}\t{Lat}\t{Long}\t{Xaxis}\t{Yaxis}\t{Zaxis}\t{WorldRegion}\t{Country}\t{LocationText}\t{Location}\t{Decommissioned}\t{TaxReturnsFiled}\t{EstimatedPopulation}\t{TotalWages}\t{Notes}";
    }

    // Override the ">" operator to compare two Zipcode objects based on their zip codes
    public static Zipcode operator >(Zipcode zip1, Zipcode zip2)
    {
        // Check if either zip1 or zip2 is null
        if (zip1 == null)
        {
            return zip2;
        }
        if (zip2 == null)
        {
            return zip1;
        }

        // Compare zip codes
        int result = string.Compare(zip1.Code, zip2.Code);

        // Return the Zipcode object with the larger zip code number
        return result > 0 ? zip1 : zip2;
    }

    // Override the "<" operator to compare two Zipcode objects based on their zip codes
    public static Zipcode operator <(Zipcode zip1, Zipcode zip2)
    {
        // Check if either zip1 or zip2 is null
        if (zip1 == null)
        {
            return zip2;
        }
        if (zip2 == null)
        {
            return zip1;
        }

        // Compare zip codes
        int result = string.Compare(zip1.Code, zip2.Code);

        // Return the Zipcode object with the smaller zip code number
        return result < 0 ? zip1 : zip2;
    }
}
