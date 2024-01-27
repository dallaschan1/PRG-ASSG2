using Assignment2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assignment2
{
    internal class Order
    {
        public int Id;
        public DateTime TimeReceived { get; set; }
        public DateTime? TimeFulfilled { get; set; }
        public List<IceCream> IceCreamList { get; set; }

        public Order()
        {
            IceCreamList = new List<IceCream>();
            Id = 0;
            TimeReceived = DateTime.MinValue;
            TimeFulfilled = null;
        }

        public Order(int id, DateTime timeRecieved)
        {
            Id = id;
            TimeReceived = timeRecieved;
            TimeFulfilled = null;
            IceCreamList = new List<IceCream>();
        }

        public void AddIceCream(IceCream iceCream)
        {

            IceCreamList.Add(iceCream);
        }

        public void DeleteIceCream(int id)
        {
            
            if (id >= 0 && id < IceCreamList.Count)
            {
                IceCreamList.RemoveAt(id - 1);
            }
            else
            {
                Console.WriteLine($"No ice cream found at index {id}.");
            }
        }

        public double CalculateTotal()
        {
            double total = 0;
            foreach (IceCream iceCream in IceCreamList)
            {
                total += iceCream.CalculatePrice();
            }
            return total;
        }


        public void ModifyIceCream(int iceCream)
        {
            List<string> normalFlavor = new List<string>() { "vanilla", "chocolate", "strawberry" };
            List<string> specialFlavor = new List<string>() { "durian", "ube", "sea salt" };
            List<string> flavorsWanted = new List<string>();
            List<string> toppingsWanted = new List<string>();
            List<Flavour> flavours = new List<Flavour>();
            List<Topping> toppings = new List<Topping>();



            string GetValidInput(string prompt, List<string> validResponses)
            {
                string response;
                do
                {
                    Console.Write(prompt);
                    response = Console.ReadLine().ToLower();

                    if (string.IsNullOrWhiteSpace(response))
                    {
                        Console.WriteLine("Input cannot be blank. Please try again.");
                        continue;
                    }

                    if (!validResponses.Contains(response))
                    {
                        Console.WriteLine("Invalid input. Please try again.");
                    }
                }
                while (string.IsNullOrWhiteSpace(response) || !validResponses.Contains(response));
                return response;
            }


            int GetNumberOfScoops()
            {
                return GetValidInput("How many scoops do you want? (Single, Double or Triple): ", new List<string> { "single", "double", "triple" }) switch
                {
                    "single" => 1,
                    "double" => 2,
                    "triple" => 3
                };
            }

            void AddFlavors(int numberOfScoops)
            {
                for (int i = 0; i < numberOfScoops; i++)
                {
                    string flavorName;
                    do
                    {
                        Console.Write($"Scoop {i + 1}, what flavor would you like? Regular Flavors: (Vanilla, Chocolate, Strawberry), Premium Flavors: (Durian, Sea salt, Ube): ");
                        flavorName = Console.ReadLine().ToLower();

                        if (!normalFlavor.Contains(flavorName) && !specialFlavor.Contains(flavorName))
                        {
                            Console.WriteLine("Error: Invalid flavor. Please choose a valid flavor.");
                        }
                    }
                    while (!normalFlavor.Contains(flavorName) && !specialFlavor.Contains(flavorName));

                    bool isPremium = specialFlavor.Contains(flavorName);


                    Flavour existingFlavour = flavours.FirstOrDefault(f => f.Type == flavorName);
                    if (existingFlavour != null)
                    {
                        existingFlavour.Quantity++;
                    }
                    else
                    {
                        flavours.Add(new Flavour(flavorName, isPremium, 1));
                    }
                }
            }

            void AddToppings()
            {
                string response = GetValidInput("Do you wish to add toppings? (yes / no): ", new List<string> { "yes", "no" });
                if (response == "yes")
                {
                    string topping;
                    do
                    {
                        topping = GetValidInput("Which topping do you wish to add? (Sprinkles, Mochi, Sago, Oreos, exit to finish): ", new List<string> { "sprinkles", "mochi", "sago", "oreos", "exit" });
                        if (topping != "exit")
                        {
                            toppings.Add(new Topping(topping));
                            toppingsWanted.Add(topping);

                            Console.WriteLine("Added!");
                        }
                    }
                    while (topping != "exit");
                }
            }


            string type = GetValidInput("Type of Ice Cream [waffle, cone, cup]: ", new List<string> { "waffle", "cone", "cup" });
            int scoops = GetNumberOfScoops();
            AddFlavors(scoops);
            AddToppings();


            if (type == "waffle")
            {
                string waffleFlavor = "original";


                string waffleResponse = GetValidInput("Do you wish to change the flavor of your waffle? (yes / no) ", new List<string> { "yes", "no" });
                if (waffleResponse == "yes")
                {
                    waffleFlavor = GetValidInput("Which flavor do u want? (Red velvet, charcoal, or pandan waffle): ", new List<string> { "red velvet", "charcoal", "pandan waffle" });

                }

                Waffle newOne = new Waffle(type, scoops, flavours, toppings, waffleFlavor);
                IceCreamList[iceCream] = newOne;
            }
            else if (type == "cone")
            {
                bool dipped = false;
                string coneResponse = GetValidInput("Do you wish to dip your cone in chocolate? (yes / no) ", new List<string> { "yes", "no" });
                if (coneResponse == "yes")
                {
                    dipped = true;

                }

                Cone newOne = new Cone(type, scoops, flavours, toppings, dipped);
                IceCreamList[iceCream] = newOne;
            }
            else if (type == "cup")
            {
                Cup newOne = new Cup(type, scoops, flavours, toppings);
                IceCreamList[iceCream] = newOne;
            }
        }

        public override string ToString()
        {
            string timeFulfilledStr = TimeFulfilled.HasValue ? TimeFulfilled.Value.ToString("g") : "Not Fulfilled";
            string timeReceivedStr = TimeReceived.ToString("g");

            string iceCreamDetails = "";
            int iceCreamCount = 1;
            foreach (IceCream iceCream in IceCreamList)
            {
                iceCreamDetails += $"  Ice Cream #{iceCreamCount}:\n    {iceCream.ToString().Replace("\n", "\n    ")}\n";
                iceCreamCount++;
            }

            return $"Order ID: {Id}\n" +
                   $"Time Received: {timeReceivedStr}\n" +
                   $"Time Fulfilled: {timeFulfilledStr}\n" +
                   $"Ice Creams:\n{iceCreamDetails}";
        }





    }
}
