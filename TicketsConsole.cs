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
                new Event(1, "Phantom of the Opera", "New York", new DateTime(2023,12,23),10.0M),
                new Event(2, "Metallica", "Los Angeles", new DateTime(2023,12,02),12.5M),
                new Event(3, "Metallica", "New York", new DateTime(2023,12,06),15.0M),
                new Event(4, "Metallica", "Boston", new DateTime(2023,10,23),7.5M),
                new Event(5, "LadyGaGa", "New York", new DateTime(2023,09,20),6.0M),
                new Event(6, "LadyGaGa", "Boston", new DateTime(2023,08,01),25.0M),
                new Event(7, "LadyGaGa", "Chicago", new DateTime(2023,07,04),9.0M),
                new Event(8, "LadyGaGa", "San Francisco", new DateTime(2023,07,07),17.0M),
                new Event(9, "LadyGaGa", "Washington", new DateTime(2023,05,22),5.0M),
                new Event(10, "Metallica", "Chicago", new DateTime(2023,01,01),11.0M),
                new Event(11, "Phantom of the Opera", "San Francisco", new DateTime(2023,07,04),12.0M),
                new Event(12, "Phantom of the Opera", "Chicago", new DateTime(2024,05,15),15.0M)
            };

            var customer = new Customer()
            {
                Id = 1,
                Name = "John",
                City = "New York",
                BirthDate = new DateTime(1995, 05, 10)
            };

            GetCustomerEvents(customer, events);

        }

        private static void GetCustomerEvents(Customer customer, List<Event> events)
        {

            var customerEvents = GetEventsInCustomerLocation(customer, events);

            Console.WriteLine("-----Events close in customer's location -----");
            MarketingEngine mEngine = new(customerEvents);
            mEngine.SendCustomerNotifications(customer);


            var eventCloseToCustBirthday = GetEventsInSameCityNextToCustomerBirthday(customer, customerEvents);

            Console.WriteLine("\n");
            Console.WriteLine("-----Events close to customer's birthday in same location as customer -----");

            customerEvents.AddRange(eventCloseToCustBirthday);
            mEngine = new(customerEvents);
            mEngine.SendCustomerNotifications(customer);

            Console.WriteLine("\n");
            Console.WriteLine("-----Events close to customer's location -----");
            var closeCityEvents = EventsClosestToCustCity(customer, events);
            mEngine = new(closeCityEvents);
            mEngine.SendCustomerNotifications(customer);

            // since we want to sort resulting events by price field
            customerEvents.AddRange(closeCityEvents);

            Console.WriteLine("\n");
            Console.WriteLine("-----Events sorted customer's price -----");
            var sortEventsByPrice = CustomerEventsBasedOnPrice(customerEvents);
            mEngine = new(sortEventsByPrice);
            mEngine.SendCustomerNotifications(customer);

        }

        private static List<Event> GetEventsInCustomerLocation(Customer customer, List<Event> events)
        {
            // get all events in customer city
            var customerEvents = events.Where(x => x.City.Equals(customer.City)).ToList();
            return customerEvents;
        }

        private static List<Event> GetEventsInSameCityNextToCustomerBirthday(Customer customer, List<Event> customerEvents)
        {
            //set customer birthday for the current year
            var custBirthDay = new DateTime(DateTime.Now.Year, customer.BirthDate.Month, customer.BirthDate.Day);

            if (custBirthDay <= DateTime.UtcNow)
                // customer has already marked birthday for the current year. update custBirthDay to next year
                custBirthDay = custBirthDay.AddYears(1);

            customerEvents = customerEvents.OrderByDescending(x => x.Date).ToList();

            var closestEventDateToBirthday = customerEvents.FirstOrDefault(x => x.Date <= custBirthDay)?.Date;

            var eventsCloseToCustomerBirthday = customerEvents.Where(x => x.Date == closestEventDateToBirthday).ToList(); return eventsCloseToCustomerBirthday;
        }

        private static List<Event> EventsClosestToCustCity(Customer customer, List<Event> events)
        {
            // get all events not in customer city
            var eventsNotInCustCity = events.Where(x => x.City != customer.City).ToList();

            //calculate distant to customer City


            // use to store customer city and event city
            Dictionary<string, string> custCityToEventCityDict = new();

            // use to store distance between customer city and event city
            Dictionary<string, int> custCityToEventCityDistance = new();

            foreach (var eventCity in eventsNotInCustCity.Select(x => x.City).ToList())
            {

                string key = BuildCityKey(customer.City, eventCity);
                if (custCityToEventCityDict.ContainsKey(key) || custCityToEventCityDistance.ContainsKey(key))
                    continue;// it has been calculated

                custCityToEventCityDict.Add(key, eventCity);

                //calculate distance between city
                custCityToEventCityDistance.Add(key, AlphebiticalDistance(customer.City, eventCity));
            }

            //get top 5 closest distances
            var sortedCityDistance = custCityToEventCityDistance.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            List<Event> closeCityEvents = new();
            foreach (var item in sortedCityDistance)
            {
                string city = custCityToEventCityDict[item.Key];
                var eventInCity = eventsNotInCustCity.Where(x => x.City.Equals(city)).ToList();
                closeCityEvents.AddRange(eventInCity);
            }
            // return top 5 closest to customer location
            return closeCityEvents.Take(5).ToList();
        }
        private static List<Event> CustomerEventsBasedOnPrice(List<Event> events)
        {

            var customerEvents = events.OrderBy(x => x.Price).ToList();
            return customerEvents;

        }


        private static string BuildCityKey(string customerCity, string eventCity)
        {
            string cityKey = string.Concat(customerCity, eventCity);
            char[] cityKeyArr = cityKey.ToCharArray();
            Array.Sort(cityKeyArr);
            return new string(cityKeyArr);
        }
        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                //Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                //Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }
    }


    public class MarketingEngine
    {
        private readonly List<Event> events;
        public MarketingEngine(List<Event> events)
        {
            this.events = events;
        }

        public void SendCustomerNotifications(Customer customer)
        {
            foreach (var e in events)
            {
                Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date}");
            }
        }
    }

    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }

        public Event(int id, string name, string city, DateTime date, decimal price)
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

}

