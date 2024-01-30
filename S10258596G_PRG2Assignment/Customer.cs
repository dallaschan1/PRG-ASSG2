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

        public List<Order> orderHistory { get; set; }

        public PointCard rewards { get; set; }

        public List<Review> Reviews { get; set; }

        public Customer() 
        {
            currentOrder = new Order();
            orderHistory = new List<Order>();
            rewards = new PointCard();
            Reviews = new List<Review>();
        }

        public Customer(string name, int memberId, DateTime dob)
        {
            this.name = name;
            this.memberId = memberId;
            this.dob = dob;
            currentOrder = new Order();
            orderHistory = new List<Order>();
            rewards = new PointCard();
            Reviews = new List<Review>();
        }

        public Order MakeOrder()
        {
            Order order = new Order();
            currentOrder = order;
            orderHistory.Add(order);

            return order;
        }

        public bool IsBirthday()
        {
            DateTime today = DateTime.Today;

           
            bool isBirthday = (today.Day == dob.Day) && (today.Month == dob.Month);

            
            DateTime orderDate = currentOrder.TimeReceived;
            isBirthday = isBirthday && (orderDate.Day == dob.Day) && (orderDate.Month == dob.Month);

            return isBirthday;
        }


        public override string ToString()
        {
            string currentOrderString = currentOrder.Id != 0 ? "Yes" : "No";

           
            string orderHistoryString = orderHistory != null ? $"{orderHistory.Count}" : "Order History: None";
            string memberIdString = memberId.ToString("D6");


            return $"{name,-15}{memberIdString,-15}{dob.ToShortDateString(),-15}{rewards.Tier,-10}{rewards.Points,-10}{rewards.PunchCard, -15}{orderHistoryString,-20}{currentOrderString}";

        }
    }
}
