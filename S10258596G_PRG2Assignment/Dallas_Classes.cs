using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class Customer
    {
        public string name { get; set; }
        public int memberId { get; set; }

        public DateTime dob { get; set; }

        public Order currentOrder { get; set; }

        public List<Order> orderHistory { get; set; } = new List<Order>();

        public PointCard rewards { get; set; }

        public Customer() { }

        public Customer(string name, int memberId, DateTime dob)
        {
            this.name = name;
            this.memberId = memberId;
            this.dob = dob;
            currentOrder = null;
            rewards = null;

        }

        public Order MakeOrder()
        {
            Order order = new Order();

            return order;
        }

        public bool IsBirthday()
        {
            DateTime today = DateTime.Today;
            if ((today.Day == dob.Day) && (today.Month == dob.Month))
            {
                return true;
            }
            else
            {
                return false;
            }


          
        }

        public override string ToString()
        {
            return $"{name}, {memberId}, {dob}";
        }

    }
    
    internal class PointCard
    {
        public int points { get; set; }

        public int punchCard {  get; set; }

        public string tier { get; set; }

        public PointCard() { 
            points = 0;
            punchCard = 0;
        
        }

        public PointCard(int points, int punchCard)
        {
            
            this.points = points;
            this.punchCard = punchCard;
            tier = "Ordinary";
        }

        public void AddPoints(int money)
        {

            points += (int)Math.Floor(money * 0.72);
            if (points >= 100 && tier != "Gold")
            {
                tier = "Gold";
            }
            else if (points >= 50 && tier == "Ordinary")
            {
                tier = "Silver";
            }

        }

        public void RedeemPoints(int amount)
        {
            points -= amount;
        } 

        public void Punch()
        {
            if (punchCard == 10)
            {
                punchCard = 0;
            }
            else { punchCard += 1; }
        }
        public override string ToString()
        {
            return $"points: {points}, punchCard: {punchCard}, tier: {tier}";
        }

    }

    internal class Order
    {
        public int id;
        public DateTime timeRecieved {  get; set; }
        public DateTime? timeFulfilled { get; set;  }
        public List<IceCream> iceCreamList {  get; set; } = new List<IceCream> ();

        public Order()
        {

        }

        public Order (int id, DateTime timeRecieved)
        {
            this.id = id;
            this.timeRecieved = timeRecieved;
        }

        public void ModifyIceCream(int iceCream)
        {
            /*
            IceCream modifiedIceCream = iceCreamList[iceCream];
            List<string> normalFlavor = new List<string>() { "vanilla", "chocolate", "strawberry"};
            List<string> specialFlavor = new List<string>() { "durian", "ube", "sea salt" };
            List<string> flavorsWanted = new List<string> ();
            
            int flavorMoney = 0;
            
            

            while (true)
            {
                Console.WriteLine("Type of Ice Cream [waffle, cone, cup]: ");
                string type = Console.ReadLine().ToLower();
                if ((type == "waffle") || (type == "cone") || (type == "cup"))
                {
                    int number = 0;
                    Console.Write("How many scoops do you want? (Single, Double or Triple): ");
                    if (Console.ReadLine().ToLower() == "single")
                    {
                        number = 1;
                    }
                    else if (Console.ReadLine().ToLower() == "double")
                    {
                        number = 2;
                    }
                    else if (Console.ReadLine().ToLower() == "triple")
                    {
                        number = 3;
                    }

                    while (number > 3 || number < 1)
                    {
                        Console.WriteLine("Please enter the values (Single, Double or Triple).");
                        Console.Write("How many scoops do you want? (1 - 3): ");
                        if (Console.ReadLine().ToLower() == "single")
                        {
                            number = 1;
                        }
                        else if (Console.ReadLine().ToLower() == "double")
                        {
                            number = 2;
                        }
                        else if (Console.ReadLine().ToLower() == "triple")
                        {
                            number = 3;
                        }
                    }
                    for (int i = 0; i < number; i++)
                    {
                        string flavor;


                        do
                        {
                            Console.Write($"Scoop {i + 1}, what flavor would you like? Regular Flavors: (Vanilla, Chocolate, Strawberry), Premium Flavors: (Durian, Sea salt, Ube): ");
                            flavor = Console.ReadLine().ToLower();

                            if (normalFlavor.Contains(flavor))
                            {
                                flavorsWanted.Add(flavor);
                            }
                            else if (specialFlavor.Contains(flavor))
                            {
                                flavorsWanted.Add(flavor);
                                flavorMoney += 2;
                            }
                            else
                            {
                                Console.WriteLine("Error: Invalid flavor. Please choose a valid flavor.");
                            }
                        }
                        while (!normalFlavor.Contains(flavor) && !specialFlavor.Contains(flavor));



                    }
                    string topping = "";
                    List<string> toppingsWanted = new List<string>();

                    while (true)
                    {
                        Console.Write("Do you wish to add toppings? (yes / no): ");
                        string response = Console.ReadLine().ToLower();

                        if (response.ToLower() == "yes")
                        {
                            do



                            {
                                List<string> list = new List<string>() { "sprinkles", "mochi", "sago", "oreos" };
                                
                                Console.Write("Which topping do you wish to add? (Sprinkes, Mochi, Sago and Oreos) are available (Exit to exit): ");
                                topping = Console.ReadLine();
                                if (list.Contains(topping.ToLower()))
                                {
                                    toppingsWanted.Add(topping);
                                    flavorMoney += 1;
                                    Console.WriteLine("Added!");
                                }
                                else if (topping != "exit")
                                {
                                    Console.WriteLine("The only Available Toppings is (Sprinkes, Mochi, Sago and Oreos).");
                                }
                                
                            }
                            while (topping.ToLower() != "exit");
                        }
                        else if (response.ToLower() != "no")
                        {
                            Console.WriteLine("Please enter either yes or no.");
                            continue;
                        }
                        break;
                    }


                    if (type == "waffle")
                    {
                        Console.Write("Do you wish to change the flavor of your waffle? (yes / no) ");
                        List<string> list = new List<string>() { "red velvet", "charcoal", "pandan waffle"};
                        
                        if (Console.ReadLine().ToLower() == "yes")
                        {
                            while (true)
                            {
                                Console.Write("Which flavor do u want? (Red velvet, charcoal, or pandan waffle), (exit to Exit): ");

                                string waffleFlavor = "";
                                waffleFlavor = Console.ReadLine().ToLower();
                                if (!list.Contains(waffleFlavor))
                                {
                                    Console.WriteLine("Please Enter either (Red velvet, charcoal, or pandan waffle).");
                                    continue;
                                }
                                else
                                {
                                    flavorMoney += 3;
                                    break;
                                }
                            }
                        }

                    }
                    else if (type == "cone")
                    {
                        Console.Write("Do you wish to dip your cone in chocolate? (yes / no) ");
                        

                        if (Console.ReadLine().ToLower() == "yes")
                        {


                            bool dipped = true;
                            flavorMoney += 2;
                            
                        }
                    





                    }
                    break;
                
                    
                }
                else
                {
                    Console.WriteLine("Please enter (cone, waffle or cup): ");
                }
               
            }*/

            
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
                    if (!validResponses.Contains(response))
                    {
                        Console.WriteLine("Invalid input. Please try again.");
                    }
                }
                while (!validResponses.Contains(response));
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
                iceCreamList[iceCream] = newOne;
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
                iceCreamList[iceCream] = newOne;
            }
            else if (type == "cup")
            {
                Cup newOne = new Cup(type, scoops, flavours, toppings);
                iceCreamList[iceCream] = newOne;
            }

            

        }

    }
}
