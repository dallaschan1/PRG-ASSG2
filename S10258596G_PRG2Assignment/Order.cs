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
            
            if (id >= 0 && id <= IceCreamList.Count)
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
            Dictionary<string, string> normalFlavor = new Dictionary<string, string>() { { "1", "vanilla" }, { "2", "chocolate" }, { "3", "strawberry" } };
            Dictionary<string, string> specialFlavor = new Dictionary<string, string>() { { "4", "durian" }, { "5", "ube" }, { "6", "sea salt" } };
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
                return GetValidInput("How many scoops do you want (1-3)?: ", new List<string> { "1", "2", "3" }) switch
                {
                    "1" => 1,
                    "2" => 2,
                    "3" => 3
                };
            }

            void AddFlavors(int numberOfScoops)
            {
                string chosenFlavour;
                Console.WriteLine("\n------------------------------------------");
                Console.WriteLine("Regular Flavours: \n1. Vanilla\n2. Chocolate\n3. Strawberry\n");
                Console.WriteLine("Premium Flavors: \n4. Durian\n5. Ube\n6. Sea Salt\n");
                for (int i = 0; i < numberOfScoops; i++)
                {
                    do
                    {
                        Console.Write($"Scoop {i + 1} flavour: ");
                        chosenFlavour = Console.ReadLine();

                        if (!normalFlavor.ContainsKey(chosenFlavour) && !specialFlavor.ContainsKey(chosenFlavour))
                        {
                            Console.WriteLine("Error: Invalid flavor. Please choose a valid flavor.");
                        }
                    }
                    while (!normalFlavor.ContainsKey(chosenFlavour) && !specialFlavor.ContainsKey(chosenFlavour));

                    bool isPremium = specialFlavor.ContainsKey(chosenFlavour);
                    string flavorName = "";

                    if (normalFlavor.ContainsKey(chosenFlavour))
                    {
                        flavorName = normalFlavor[chosenFlavour];
                    }
                    else
                    {
                        flavorName = specialFlavor[chosenFlavour];
                    }

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
                    Console.WriteLine("\n------------------------------------------");
                    Console.WriteLine("Toppings Available (+$1)");
                    Console.WriteLine("1. Sprinkles\n2. Mochi\n3. Sago\n4. Oreos\n");

                    int i = 1;
                    do
                    {
                        Dictionary<string, string> toppingDic = new Dictionary<string, string> { { "1", "sprinkles" }, { "2", "mochi" }, { "3", "sago" }, { "4", "oreos" }, { "0", "exit" } };
                        string input = GetValidInput($"Topping {i}/4 (0 to finish): ", new List<string> { "1", "2", "3", "4", "0" });
                        topping = toppingDic[input];

                        if (topping != "exit")
                        {
                            toppings.Add(new Topping(topping));
                            Console.WriteLine($"{topping.ToUpper()} ADDED!");
                            i++;
                        }
                    }
                    while (topping != "exit" && i <= 4);
                }
            }

            Console.WriteLine("\nTypes of Ice Cream:");
            Console.WriteLine("1. Cup\n2. Cone\n3. Waffle\n");

            string typeInput = GetValidInput("Select Type: ", new List<string> { "1", "2", "3" });
            string type = "";

            switch (typeInput)
            {
                case "1":
                    type = "cup";
                    break;
                case "2":
                    type = "cone";
                    break;
                case "3":
                    type = "waffle";
                    break;
            }

            int scoops = GetNumberOfScoops();
            AddFlavors(scoops);
            AddToppings();

            if (type == "waffle")
            {
                string waffleFlavor = "original";
                string waffleResponse = GetValidInput("Do you wish to change the flavor of your waffle? (yes / no): ", new List<string> { "yes", "no" });
                if (waffleResponse == "yes")
                {
                    Console.WriteLine("\n--------------------------------------------------");
                    Console.WriteLine("Waffle Flavours:");
                    Console.WriteLine("1. Red Velvet\n2. Charcoal\n3. Pandan\n");
                    Dictionary<string, string> waffleDic = new Dictionary<string, string>() { { "1", "Red Velvet" }, { "2", "Charcoal" }, { "3", "Pandan" } };
                    waffleFlavor = waffleDic[GetValidInput("Select Flavour From Above: ", new List<string> { "1", "2", "3" })];
                }
                IceCream newOne = new Waffle(type, scoops, flavours, toppings, waffleFlavor);
                IceCreamList[iceCream] = newOne;
            }
            else if (type == "cone")
            {
                bool dipped = false;
                string coneResponse = GetValidInput("Do you wish to dip your cone in chocolate? (yes / no): ", new List<string> { "yes", "no" });
                if (coneResponse == "yes")
                {
                    dipped = true;
                }
                IceCream newOne = new Cone(type, scoops, flavours, toppings, dipped);
                IceCreamList[iceCream] = newOne;
            }
            else if (type == "cup")
            {
                IceCream newOne = new Cup(type, scoops, flavours, toppings);
                IceCreamList[iceCream] = newOne;
            }
            Console.WriteLine("Ice cream modified successfully!");
            Console.WriteLine("--------------------------------------------------");
        }
    

        public override string ToString()
        {
            string timeFulfilledStr = TimeFulfilled.HasValue ? TimeFulfilled.Value.ToString("g") : "Not Fulfilled";
            string timeReceivedStr = TimeReceived.ToString("g");

            string iceCreamDetails = "";
            int iceCreamCount = 1;
            foreach (IceCream iceCream in IceCreamList)
            {
                iceCreamDetails += $"\nIce Cream #{iceCreamCount}:\n{iceCream}\n";
                iceCreamCount++;
            }

            return $"Order ID: {Id}\n" +
                   $"Time Received: {timeReceivedStr}\n" +
                   $"Time Fulfilled: {timeFulfilledStr}\n" +
                   $"Ice Creams:\n{iceCreamDetails}";
        }





    }
}
