using System;
using System.Collections.Generic;
using System.Linq;
using static TicketsConsole.Program;

namespace TicketsConsole
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var events = new List<Event>{
                new Event(1, "Phantom of the Opera", "New York", new DateTime(2023,12,23),10),
                new Event(2, "Metallica", "Los Angeles", new DateTime(2023,12,02),20),
                new Event(3, "Metallica", "New York", new DateTime(2023,12,06),30),
                new Event(4, "Metallica", "Boston", new DateTime(2023,10,23),5),
                new Event(5, "LadyGaGa", "New York", new DateTime(2023,09,20),15),
                new Event(6, "LadyGaGa", "Boston", new DateTime(2023,08,01),12),
                new Event(7, "LadyGaGa", "Chicago", new DateTime(2023,07,04),22),
                new Event(8, "LadyGaGa", "San Francisco", new DateTime(2023,07,07),10),
                new Event(9, "LadyGaGa", "Washington", new DateTime(2023,05,22),15),
                new Event(10, "Metallica", "Chicago", new DateTime(2023,01,01),20),
                new Event(11, "Phantom of the Opera", "San Francisco", new DateTime(2023,07,04),30),
                new Event(12, "Phantom of the Opera", "Chicago", new DateTime(2024,05,15),25)
            };

            var customer = new Customer()
            {
                Id = 1,
                Name = "John",
                City = "New York",
                BirthDate = new DateTime(1995, 05, 10)
            };

            MarketingEngine mEngine = new MarketingEngine(events);

            Console.WriteLine("---- Events in to customer's city");
            mEngine.EventsInSameCityAsCustomer(customer);
            Console.Write("\n");
            Console.WriteLine("---- Events close to customer's birthday");
            mEngine.EventsClosestToCustomerBirthday(customer, 2);
            Console.Write("\n");

            Console.WriteLine("---- Events close to customer's city");
            mEngine.EventClosestToCustCity(customer, 5);
            Console.Write("\n");

            Console.WriteLine("---- Events based on price");
            mEngine.EventBasedOnPrice(customer, 2);
          


        }
        public class MarketingEngine
        {
            private readonly List<Event> events;
            private Dictionary<string, int> cityDistance = new();
            public MarketingEngine(List<Event> events)
            {
                this.events = events;
            }
            public void SendCustomerNotifications(Customer customer, Event e)
            {

                Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date} " +
                    $",distance to customer is {e.EventDistanceToCustomer} and price is ${e.Price} ");
            }

            public void EventsInSameCityAsCustomer(Customer customer)
            {

                var events = this.events.Where(x => x.City.Equals(customer.City)).ToList();
                foreach (var e in events)
                {
                    SendCustomerNotifications(customer, e);
                }

            }

            public void EventsClosestToCustomerBirthday(Customer customer, int noOFEvents=2)
            {

                //set the customer birthday for the current year
                var custBd = new DateTime(DateTime.Now.Year, customer.BirthDate.Month, customer.BirthDate.Day);

                if (DateTime.Now > custBd)
                    // set birthday to the next
                    custBd = custBd.AddYears(1);

                //Console.WriteLine($"birthday: {custBd.ToShortDateString()}");

                var custEvents = this.events.OrderBy(x => x.Date).ToList();

                var eventClosestToBd = custEvents.Where(x => x.Date > custBd).Take(noOFEvents).ToList();
                foreach (var e in eventClosestToBd)
                {
                    SendCustomerNotifications(customer, e);
                }

            }
            public void EventClosestToCustCity(Customer customer, int topEvents=5)
            {
               
                List<Event> customerEvent = new();
                foreach(var evnt in this.events)
                {
                    int distance = RetrieveDistance(customer.City, evnt.City);
                    if (distance < 0)
                        continue; // we are unable to calculate distance
                    evnt.EventDistanceToCustomer = distance;
                    customerEvent.Add(evnt);
                }

                //send notification for events
                foreach (var item in customerEvent.OrderBy(x=>x.EventDistanceToCustomer).Take(topEvents))
                {
                    SendCustomerNotifications(customer, item);
                }

            }

            private static int CalculateDistance(string custCity, string eventCity)
            {
                var customerCityInfo = Cities.Where(c => c.Key ==custCity).Single().Value;

                var eventCityInfo = Cities.Where(c => c.Key == eventCity).Single().Value;

                return Math.Abs(customerCityInfo.X - eventCityInfo.X) + Math.Abs(customerCityInfo.Y - eventCityInfo.Y);               
            }
            private int RetrieveDistance(string custCity, string eventCity)
            {
                
                int distance;
                string key = BuildCityKey(custCity,eventCity);
                // check if distance has been calculated before
                if (cityDistance.ContainsKey(key))
                    return cityDistance[key];
                try
                {
                    // calculate distance
                    distance = CalculateDistance(custCity, eventCity);
                }
                //check for errors
                catch (TimeoutException) {
                    Console.WriteLine($"Something went wrong while calculating distance between {custCity} and {eventCity}");
                   return  -1;
                }
                catch(Exception)
                {
                    Console.WriteLine($"Something went wrong while calculating distance between {custCity} and {eventCity}");
                   return -1;
                }
                
                  
                // store distance for cities
                
               cityDistance.Add(key, distance);
                     
                return distance;
            }
            public string BuildCityKey(string cityA, string cityB){
                string mergeCity = string.Concat(cityA,cityB);
                var mergeCityArr = mergeCity.ToCharArray();
                Array.Sort(mergeCityArr);
                return new string(mergeCityArr);
            }
            public void EventBasedOnPrice(Customer customer, int topEvent = 5)
            {
                var custEvent = this.events.OrderBy(x=>x.Price).Take(topEvent).ToList();
                foreach(var ev in custEvent)
                {
                    SendCustomerNotifications(customer, ev);
                }
            }
            

            public static readonly IDictionary<string, City> Cities = new Dictionary<string, City>()
        {
            { "New York", new City("New York", 3572, 1455) },
            { "Los Angeles", new City("Los Angeles", 462, 975) },
            { "San Francisco", new City("San Francisco", 183, 1233) },
            { "Boston", new City("Boston", 3778, 1566) },
            { "Chicago", new City("Chicago", 2608, 1525) },
            { "Washington", new City("Washington", 3358, 1320) },
        };



           

        }
        public class Event
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public DateTime Date { get; set; }
            public int EventDistanceToCustomer { get; set; }
            public int Price { get; set; } = 0;

            public Event(int id, string name, string city, DateTime date, int price)
            {
                this.Id = id;
                this.Name = name;
                this.City = city;
                this.Date = date;
                this.Price = price;
            }
        }

        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string City { get; set; } = null!;
            public DateTime BirthDate { get; set; }
        }
        public record City(string Name, int X, int Y);

        /*-------------------------------------
        Coordinates are roughly to scale with miles in the USA
           2000 +----------------------+  
                |                      |  
                |                      |  
             Y  |                      |  
                |                      |  
                |                      |  
                |                      |  
                |                      |  
             0  +----------------------+  
                0          X          4000
        ---------------------------------------*/

    }
}
