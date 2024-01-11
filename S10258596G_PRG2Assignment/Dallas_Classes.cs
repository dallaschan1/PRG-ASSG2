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
                    else if(Console.ReadLine().ToLower() == "double")
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
                                number += 2;
                            }
                            else
                            {
                                Console.WriteLine("Error: Invalid flavor. Please choose a valid flavor.");
                            }
                        }
                        while (!normalFlavor.Contains(flavor) && !specialFlavor.Contains(flavor));



                    }

                    
                       
                   


                }
                if (type == "waffle")
                {
                    
             
                }
                else if (type == "cone")
                {
            
                }
                else if (type == "cup")
                {
                    
                }
                else
                {
                    Console.WriteLine("Please enter either \"Waffle\", \"Cone\" or \"Cup\". ");
                    continue;
                }
            }

        }

    }
}
