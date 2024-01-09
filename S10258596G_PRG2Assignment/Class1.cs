using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace classes
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
            
    }
}
