/*
 
Let's say we're running a small entertainment business as a start-up. This means we're selling tickets to live events on a website. An email campaign service is what we are going to make here. We're building a marketing engine that will send notifications (emails, text messages) directly to the client and we'll add more features as we go.
 
Please, instead of debuging with breakpoints, debug with "Console.Writeline();" for each task because the Interview will be in Coderpad and in that platform you cant do Breakpoints.
 

 
            1. You can see here a list of events, a customer object. Try to understand the code, make it compile. 
 
            2.  The goal is to create a MarketingEngine class sending all events through the constructor as parameter and make it print the events that are happening in the same city as the customer. To do that, inside this class, create a SendCustomerNotifications method which will receive a customer as parameter and will mock the Notification Service. Add this ConsoleWriteLine inside the Method to mock the service. Inside this method you can add the code you need to run this task correctly but you cant modify the console writeline: Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date}");
 
            3. As part of a new campaign, we need to be able to let customers know about events that are coming up close to their next birthday. 
            You can make a guess and add it to the MarketingEngine class if you want to. So we still want to keep how things work now, which is 
            that we email customers about events in their city or the event closest to next customer's birthday, and then we email them again at
            some point during the year. The current customer, his birthday is on may. So it's already in the past. So we want to find the next one,
            which is 23. How would you like the code to be built? We don't just want functionality; we want more than that. We want to know how you
            plan to make that work. Please code it.
 
            4. The next requirement is to extend the solution to be able to send notifications for the five closest events to the customer. The interviewer here can paste a method to help you, or ask you to search it. We will attach 2 different ways to calculate the distance. 
 
            // ATTENTION this row they don't tell you, you can google for it. In some cases, they pasted it so you can use it
 
            Option 1:
            var distance = Math.Abs(customerCityInfo.X - eventCityInfo.X) + Math.Abs(customerCityInfo.Y - eventCityInfo.Y);
 
            Option 2:
            private static int AlphebiticalDistance(string s, string t)
            {
                var result = 0;
                var i = 0;
                for(i = 0; i < Math.Min(s.Length, t.Length); i++)
                    {
                        // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                        result += Math.Abs(s[i] - t[i]);
                    }
                    for(; i < Math.Max(s.Length, t.Length); i++)
                    {
                        // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                        result += s.Length > t.Length ? s[i] : t[i];
                    }
                    
                    return result;
            } 
 
            Tips of this Task:
            Try to use Lamba Expressions. Data Structures. Dictionary, ContainsKey method.
 
            5. If the calculation of the distances is an API call which could fail or is too expensive,
            how will you improve the code written in 4? Think in caching the data which could be code
            it as a dictionary. You need to store the distances between two cities. Example:
 
            New York - Boston => 400 
            Boston - Washington => 540
            Boston - New York => Should not exist because "New York - Boston" is already stored and the distance is the same. 
 
            6. If the calculation of the distances is an API call which could fail, what can be done to 
            avoid the failure? Think in HTTPResponse Answers: Timeoute, 404, 403. How can you handle that
            exceptions? Code it.
 
            7.  If we also want to sort the resulting events by other fields like price,
            etc. to determine whichones to send to the customer, how would you implement it? Code it.
            
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
    */
