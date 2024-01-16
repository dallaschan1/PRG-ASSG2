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

        public Customer() 
        {
            currentOrder = new Order();
            orderHistory = new List<Order>();
            rewards = new PointCard();
        }

        public Customer(string name, int memberId, DateTime dob)
        {
            this.name = name;
            this.memberId = memberId;
            this.dob = dob;
            currentOrder = new Order();
            orderHistory = new List<Order>();
            rewards = new PointCard();
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
            string currentOrderString = currentOrder != null ? "Currently Ordering: Yes" : "Currently Ordering: No";

           
            string orderHistoryString = orderHistory != null ? $"Order History Count: {orderHistory.Count}" : "Order History: None";

            string birthdayString = IsBirthday() ? "Today is their birthday!" : "Today is not their birthday.";

            return $"Name: {name}, Member ID: {memberId}, Date of Birth: {dob.ToShortDateString()}, {currentOrderString}, {orderHistoryString}, {birthdayString}";
        }
    }
}
