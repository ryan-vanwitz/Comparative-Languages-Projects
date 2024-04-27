import time

"""
  Homework#5

  Add your name here: Ryan Van Witzenburg

  You are free to create as many classes within the hw5.py file or across 
  multiple files as you need. However, ensure that the hw5.py file is the 
  only one that contains a __main__ method. This specific setup is crucial 
  because your instructor will run the hw5.py file to execute and evaluate 
  your work.
"""

class CityProcessor:
    def __init__(self):
        self.states_file = "states.txt"
        self.zipcodes_file = "zipcodes.txt"
        self.zips_file = "zips.txt"
        self.cities_file = "cities.txt"

    def __add__(self, other):
        if isinstance(other, CityProcessor):
            return CityProcessor(self.name + other.name)
        else:
            raise TypeError("Unsupported operand type for +: 'CityProcessor' and '{type(other).__name__}'")

    def __str__(self):
        return f"CityProcessor: {self.name}"

    def read_states(self):
        with open(self.states_file, "r") as file:
            states = file.read().splitlines()
        return states

    def read_zipcodes(self):
        cities_by_state = {}
        with open(self.zipcodes_file, "r") as file:
            next(file)  # Skip header line
            for line in file:
                _, _, _, city, state, *_ = line.split("\t")
                if state not in cities_by_state:
                    cities_by_state[state] = set()
                cities_by_state[state].add(city)
        return cities_by_state

    def read_zip_lat_lon(self):
        zip_lat_lon = {}
        with open(self.zipcodes_file, "r") as file:
            next(file)  # Skip header line
            for line in file:
                _, zipcode, _, _, _, _, lat, lon, *_ = line.split("\t")
                if zipcode not in zip_lat_lon:
                    zip_lat_lon[zipcode] = (lat, lon)
        return zip_lat_lon

    def read_cities(self):
        with open(self.cities_file, "r") as file:
            cities = file.read().splitlines()
        return cities

    def get_zip_lat_lon(self, zipcodes):
        zip_lat_lon = self.read_zip_lat_lon()
        return [(zip_lat_lon[zipcode][0], zip_lat_lon[zipcode][1]) for zipcode in zipcodes if zipcode in zip_lat_lon]

    def filter_city_by_name(self, city, city_set):
        """
        Helper method to filter city names case-insensitively.
        """
        filtered_cities = filter(lambda x: x.lower() == city.lower(), city_set)
        return any(filtered_cities)

    def get_city_states(self, *args):
        cities_by_state = self.read_zipcodes()
        cities = self.read_cities()
        for city in cities:
            states = [state for state, city_set in cities_by_state.items() if self.filter_city_by_name(city, city_set)]
            states = sorted(set(states))  # Sorting and removing duplicates
            yield city, states

    def find_common_cities(self):
        states = self.read_states()
        cities_by_state = self.read_zipcodes()
        common_cities = set.intersection(*(cities_by_state[state] for state in states))
        return sorted(common_cities)

if __name__ == "__main__": 
    start_time = time.perf_counter()  # Do not remove this line
    '''
    Inisde the __main__, do not add any codes before this line.
    -----------------------------------------------------------
    '''


    # write your code here
    city_processor = CityProcessor()
    
    # Problem 1: Finding common city names
    common_cities = city_processor.find_common_cities()
    with open("CommonCityNames.txt", "w") as file:
        file.write("\n".join(common_cities))
    
    # Problem 2: Generating LatLon.txt
    zipcodes = [line.strip() for line in open(city_processor.zips_file, "r").readlines()]
    zip_lat_lon = city_processor.get_zip_lat_lon(zipcodes)
    with open("LatLon.txt", "w") as file:
        for lat, lon in zip_lat_lon:
            file.write(f"{lat} {lon}\n")

    # Problem 3: Generating CityStates.txt
    with open("CityStates.txt", "w") as file:
      for city_state in city_processor.get_city_states():
        city, states = city_state
        file.write(f"{' '.join(states)}\n")


    '''
    Inside the __main__, do not add any codes after this line.
    ----------------------------------------------------------
    '''
    end_time = time.perf_counter()
    # Calculate the runtime in milliseconds
    runtime_ms = (end_time - start_time) * 1000
    print(f"The runtime of the program is {runtime_ms} milliseconds.")  
    

