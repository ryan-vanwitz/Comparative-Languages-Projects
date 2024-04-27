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
    common_cities = city_processor.find_common_cities()

    with open("CommonCityNames.txt", "w") as file:
        file.write("\n".join(common_cities))


    '''
    Inside the __main__, do not add any codes after this line.
    ----------------------------------------------------------
    '''
    end_time = time.perf_counter()
    # Calculate the runtime in milliseconds
    runtime_ms = (end_time - start_time) * 1000
    print(f"The runtime of the program is {runtime_ms} milliseconds.")  
    

