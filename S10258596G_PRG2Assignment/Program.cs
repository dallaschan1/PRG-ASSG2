using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Globalization;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.VisualBasic.FileIO;
using static System.Formats.Asn1.AsnWriter;

using System.Text;
using System.Collections.Specialized;
using System.Security.Cryptography;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue<Order> NormalQueue = new Queue<Order>();
            Queue<Order> GoldQueue = new Queue<Order>();
            Dictionary<string, Customer> customerDic = new Dictionary<string, Customer>();
            Dictionary<int, Order> orderDic = new Dictionary<int, Order>();
            Dictionary<string, double> monthYearData = new Dictionary<string, double>();
            Dictionary<string, List<Review>> reviewDic = new Dictionary<string, List<Review>>(); /* (NOTE: THE KEY FOR THIS DICTIONARY IS THE MemberID) This is for identifying the reviews by their MemberID & OrderID */ 
            Dictionary<string, List<Review>> reviewCat = new Dictionary<string, List<Review>> /* (NOTE: The key for this dictionary is the flavour) This is for displaying the reviews by their flavours */
            {
                { "Vanilla", new List<Review>() },
                { "Chocolate", new List<Review>() },
                { "Strawberry", new List<Review>() },
                { "Durian", new List<Review>() },
                { "Ube", new List<Review>() },
                { "Sea Salt", new List<Review>() }
            };




            // Create customers and orders from the given data files at the onset. 
            void CustomerCreation()
            {
                using (StreamReader sr = new StreamReader("customers.csv"))
                {
                    string? s = sr.ReadLine(); // added the heading so it doenst cause error
                    string[] heading = s.Split(',');

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] details = s.Split(',').Select(detail => detail.Trim()).ToArray();
                        if (details[1].Length < 6)
                        {
                            details[1] = details[1].PadLeft(6, '0');
                        }

                        DateTime dob = DateTime.ParseExact(details[2], "d/M/yyyy", CultureInfo.InvariantCulture);
                        PointCard pointCard = new PointCard(Int32.Parse(details[4]), Int32.Parse(details[5]));
                        Customer customerNew = new Customer(details[0], Convert.ToInt32(details[1]), dob);
                        customerNew.rewards = pointCard;

                        customerDic.Add(details[1], customerNew);
                    }
                }
            }

            CustomerCreation();


            bool CheckPremiumFlavour(string flavour)
            {
                List<string> specialFlavour = new List<string>() { "Durian", "Ube", "Sea Salt" };
                if (specialFlavour.Contains(flavour))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void OrderCreation()
            {
                using (StreamReader sr = new StreamReader("orders.csv"))
                {


                    string? s = sr.ReadLine(); // read the heading
                    string[] heading = s.Split(',');

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] details = s.Split(',');
                        int id = Convert.ToInt32(details[0]);
                        string[] formats = { "d/M/yyyy HH:mm", "dd/MM/yyyy HH:mm", "d/MM/yyyy HH:mm", "dd/M/yyyy HH:mm" };
                        bool success = DateTime.TryParseExact(details[2], formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timeReceived);
                        DateTime? timeFulfilled;


                        timeFulfilled = DateTime.ParseExact(details[3], formats, CultureInfo.InvariantCulture);

                        // Creating individual IceCreams
                        string option = details[4];
                        int scoops = Convert.ToInt32(details[5]);

                        // Adding all the flavours into flavourList
                        List<Flavour> flavourList = new List<Flavour>();
                        Dictionary<string, int> flavourCount = new Dictionary<string, int>();
                        for (int i = 8; i < 8 + scoops; i++)
                        {
                            if (!string.IsNullOrEmpty(details[i]) && !flavourCount.ContainsKey(details[i]))
                            {
                                flavourCount.Add(details[i], 1);
                            }
                            else if (!string.IsNullOrEmpty(details[i]) && flavourCount.ContainsKey(details[i]))
                            {
                                flavourCount[details[i]]++;
                            }
                        }
                        foreach (KeyValuePair<string, int> kvp in flavourCount)
                        {
                            Flavour addFlavour = new Flavour(kvp.Key, CheckPremiumFlavour(kvp.Key), kvp.Value);
                            flavourList.Add(addFlavour);
                        }

                        // Adding all the toppings into toppingList
                        List<Topping> toppingList = new List<Topping>();
                        List<string> tops = new List<string>() { "Sprinkles", "Mochi", "Sago", "Oreos" };
                        for (int i = 11; i < 4 + 11; i++)
                        {
                            if (tops.Contains(details[i]))
                            {
                                toppingList.Add(new Topping(details[i]));
                            }
                            else { break; }
                        }

                        IceCream newIceCream = null;

                        if (option == "Waffle")
                        {
                            string waffleFlavour = details[7];
                            newIceCream = new Waffle(option, scoops, flavourList, toppingList, waffleFlavour);
                        }
                        else if (option == "Cone")
                        {
                            bool dipped = bool.Parse(details[6]);
                            newIceCream = new Cone(option, scoops, flavourList, toppingList, dipped);
                        }
                        else if (option == "Cup")
                        {
                            newIceCream = new Cup(option, scoops, flavourList, toppingList);
                        }
                        else
                        {
                            Console.WriteLine("Error Occurred! IceCream has not been created!");
                        }
                        // Finish creating IceCream and adding it to iceCreamList

                        List<IceCream> iceCreamList = new List<IceCream> { newIceCream };
                        Order newOrder = new Order(id, timeReceived);
                        newOrder.TimeFulfilled = timeFulfilled;




                        // Creating order for customer
                        int memberID = Int32.Parse(details[1]);
                        Customer customer = customerDic[details[1]];
                        List<Order> orderHistory = customer.orderHistory;
                        bool isNewOrder = false;
                        newOrder.IceCreamList = iceCreamList;
                        foreach (Order order in orderHistory)
                        {
                            if (order.Id == id)
                            {

                                order.IceCreamList.Add(newIceCream);
                                isNewOrder = true;
                                break;
                            }
                        }
                        if (!isNewOrder)
                        {
                            orderHistory.Add(newOrder);
                            orderDic.Add(id, newOrder);
                        }
                        double price = newIceCream.CalculatePrice();

                        // Create a key for the dictionary using the month and year of timeReceived
                        string monthYearKey = timeReceived.ToString("MMM yyyy");

                        // Check if the monthYearKey already exists in the dictionary
                        if (monthYearData.ContainsKey(monthYearKey))
                        {
                            // If key exists, add the price to the existing value
                            monthYearData[monthYearKey] += price;
                        }
                        else
                        {
                            // If key does not exist, add a new entry with the key and price
                            monthYearData.Add(monthYearKey, price);
                        }
                    }
                }
            }
            OrderCreation();


            void FeedbackCreation()
            {
                using (StreamReader sr = new StreamReader("reviews.csv"))
                {
                    string? s = sr.ReadLine(); // read the heading
                    string[] heading = s.Split(',');

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] details = s.Split(',');
                        string memberId = details[0].PadLeft(6, '0');

                        string flavour = details[1];
                        int rating = int.Parse(details[2]);
                        string comment = details[3];
                        string dateString = details[4];
                        DateTime dateTime;
                        string[] formats = new string[] { "d/M/yyyy HH:mm", "d-M-yyyy HH:mm" };

                        DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);

                        


                        Review review = new Review(flavour, rating, comment, dateTime);

                        Customer customer = customerDic[memberId];
                        customer.Reviews.Add(review);
                        string dicKey = memberId;
                        if (reviewDic.ContainsKey(dicKey))
                        {
                            reviewDic[dicKey].Add(review);
                        }
                        else
                        {
                            reviewDic.Add(dicKey, new List<Review> { review });
                        }
                        reviewCat[flavour].Add(review);
                    }
                }
            }

            FeedbackCreation();

            // Displays a menu for user to choose to perform each of the feature describe below repeatedly until user chooses to exit from the menu
            void DisplayMenu()
            {
                Console.WriteLine("---------------- M E N U -----------------");
                Console.WriteLine("[1] List all customers");
                Console.WriteLine("[2] List all current orders");
                Console.WriteLine("[3] Register a new customer");
                Console.WriteLine("[4] Create a customer’s order");
                Console.WriteLine("[5] Display order details of a customer");
                Console.WriteLine("[6] Modify order details");
                Console.WriteLine("[7] Process an order and checkout");
                Console.WriteLine("[8] Display monthly charged amounts breakdown & total charged amounts for the year");
                Console.WriteLine("[9] Display All Existing Reviews for our IceCreams");
                Console.WriteLine("[10] Modify Existing Reviews");
                Console.WriteLine("[0] Exit Program");
                Console.WriteLine("------------------------------------------");
            }

            int OptionValidation()
            {
                do
                {
                    try
                    {
                        Console.Write("Enter your option: ");
                        int option = int.Parse(Console.ReadLine());

                        if (option >= 0 && option <= 10) // Included 0 as a valid option
                        {
                            return option;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a number between 0 and 10.");
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 0 and 10.");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 0 and 10.");
                    }
                } while (true);
            }


            bool ProcessOption(int option)
            {
                switch (option)
                {
                    case 0:
                        Console.WriteLine("Thank you & Have a nice day! Bye bye");
                        return true;

                    case 1:
                        Option1();
                        break;

                    case 2:
                        Option2();
                        break;

                    case 3:
                        Option3();
                        break;
                    case 4:
                        Option4();
                        break;
                    case 5:
                        Option5();
                        break;
                    case 6:
                        Option6();
                        break;
                    case 7:
                        Option7();
                        break;
                    case 8:
                        Option8();
                        break;
                    case 9:
                        Option9();
                        break;
                    case 10:
                        Option10();
                        break;
                }
                return false;
            }

            while (true)
            {
                DisplayMenu();
                int option = OptionValidation();
                bool exit = ProcessOption(option);
                if (exit)
                {
                    break;
                }
            }

            void Option1()
            {
                // 1) List all customers - display the information of all the customers
                Console.WriteLine($"{"Name",-10} \t{"Member ID",-6} \t{"Date Of Birth",-10} \t{"Tier",-8} \t{"Points",-6} \t{"PunchCard",-9}");
                foreach (KeyValuePair<string, Customer> kvp in customerDic)
                {
                    Customer customer = kvp.Value;
                    Console.WriteLine($"{customer.name,-10} \t{kvp.Key,-6} \t\t{customer.dob.ToString("d/M/yyyy"),-10} \t{customer.rewards.Tier,-8} \t{customer.rewards.Points,-6} \t{customer.rewards.PunchCard,-9}");
                }
                Console.WriteLine();
            }

            void Option2()
            {


                if (NormalQueue.Count == 0)
                {
                    Console.WriteLine("\nNo Orders in the Normal Queue");
                }
                else
                {
                    Console.WriteLine("\nOrders in the Normal Queue: \n");
                    foreach (Order order in NormalQueue)
                    {
                        Console.WriteLine(order.ToString());
                    }
                }



                if (GoldQueue.Count == 0)
                {
                    Console.WriteLine("\nNo Orders in the Gold Queue");
                }
                else
                {
                    Console.WriteLine("\nOrders in the Gold Queue: \n");
                    foreach (Order order in GoldQueue)
                    {
                        Console.WriteLine(order.ToString());
                    }
                }

                Console.WriteLine();
            }


            void Option3()
            {
                string name = "";
                string id = "";
                DateTime dob;

                // Error Handling for customer name
                while (true)
                {
                    try
                    {
                        Console.Write("Enter Customer Name: ");
                        name = Console.ReadLine();

                        if (name == "")
                        {
                            Console.WriteLine("Name cannot be blank. Please enter a valid name.\n");
                        }
                        else if (!Regex.IsMatch(name, "^[a-zA-Z]+$"))
                        {
                            Console.WriteLine("Name cannot include numbers or special characters. Pleae enter a valid name.\n");
                        }
                        else { break; }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

                // Error handling for customer ID
                while (true)
                {
                    try
                    {
                        Console.Write("Enter Customer ID: ");
                        id = Console.ReadLine();
                        if (id == "")
                        {
                            Console.WriteLine("CustomerID cannot be empty. Please try again.\n");
                        }
                        else
                        {
                            if (!Regex.IsMatch(id, @"^\d{6}$"))
                            {
                                Console.WriteLine("MemberID should be 6 numbers long.\n");
                            }
                            else
                            {
                                if (customerDic.ContainsKey(id) == false) { break; }
                                else
                                {
                                    Console.WriteLine("CustomerID has already been taken! Please try again.\n");
                                }
                            }
                        }
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("CustomerID cannot be include special characters or letters. Please try again.\n");
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

                // Error handling for customer date of birth
                while (true)
                {
                    try
                    {
                        Console.Write("Enter Customer Date Of Birth (dd/mm/yyyy): ");
                        dob = DateTime.ParseExact(Console.ReadLine(), "d/M/yyyy", null);
                        break;
                    }
                    catch (FormatException)
                    {
                        // Input is not in the correct format
                        Console.WriteLine("Invalid date format. Please enter the date in dd/mm/yyyy format.\n");
                    }
                    catch (Exception ex)
                    {
                        // Other unexpected errors
                        Console.WriteLine($"An error occurred: {ex.Message}\n");
                    }
                }

                // Creating new customer and appending it to the customers.csv file
                Customer newCustomer = new Customer(name, Int32.Parse(id), dob);
                customerDic.Add(id, newCustomer);

                string data = $"{name},{id},{dob.ToString("d/M/yyyy")},{newCustomer.rewards.Tier},{newCustomer.rewards.Points},{newCustomer.rewards.PunchCard}";

                using (StreamWriter sw = new StreamWriter("customers.csv", true))
                {
                    sw.WriteLine(data);
                }
                Console.WriteLine("Registration Status: Successful!\n"); 
            }

            void Option4()
            {
                // list the customers from the customers.csv
                Console.WriteLine($"{"Name",-10} \t{"Member ID",-6} \t{"Date Of Birth",-10} \t{"Tier",-8} \t{"Points",-6} \t{"PunchCard",-9}");
                foreach (KeyValuePair<string, Customer> kvp in customerDic)
                {
                    Customer customer = kvp.Value;
                    Console.WriteLine($"{customer.name,-10} \t{kvp.Key,-6} \t\t{customer.dob.ToShortDateString(),-10} \t{customer.rewards.Tier,-8} \t{customer.rewards.Points,-6} \t{customer.rewards.PunchCard,-9}");
                }
                Console.WriteLine();


                // prompt user to select a customer and retrieve the selected customer
                string memberID;
                while (true)
                {
                    try
                    {
                        Console.Write("Select Customer (Enter MemberID): ");
                        memberID = Console.ReadLine();
                        if (memberID == "")
                        {
                            Console.WriteLine("CustomerID cannot be empty. Please try again.\n");
                        }
                        else
                        {
                            if (!Regex.IsMatch(memberID, @"^\d{6}$"))
                            {
                                Console.WriteLine("MemberID should be 6 numbers long.\n");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("CustomerID cannot be include special characters or letters. Please try again.\n");
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

                Customer selectedCustomer = customerDic[memberID];

                if (selectedCustomer.currentOrder.Id != 0)
                {
                    Console.WriteLine("Customer already has a current order. Proceed to Option 6 if customer wants to modify order\n");
                    return;
                }

                // Creating Order for customer
                Order newOrder = new Order();

                int orderID = orderDic.Count + 1;
                DateTime timeRecieved = DateTime.Now;
                DateTime? timefulfilled = null;

                int i = 1;
                Console.WriteLine();
                Console.WriteLine($"IceCream #{i}");

                // Asking them to addIceCream the first time
                AddIceCream(selectedCustomer);

                // The code below is to ask them whether they would like to add anymore icecreams
                while (true)
                {
                    Console.Write("Would you like to add another ice cream to the order? (yes / no): ");
                    string response = Console.ReadLine().ToLower();
                    if (response == "yes")
                    {
                        i++;
                        Console.WriteLine();
                        Console.WriteLine($"IceCream #{i}");
                        AddIceCream(selectedCustomer);
                    }
                    else if (response == "no")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input. Please enter (yes / no).");
                    }
                }

                // Adding newly created order to the orderDic & queueing them in a queue
                orderDic.Add(selectedCustomer.currentOrder.Id, selectedCustomer.currentOrder);

                if (selectedCustomer.rewards.Tier == "Gold")
                {
                    GoldQueue.Enqueue(selectedCustomer.currentOrder);
                }
                else
                {
                    NormalQueue.Enqueue(selectedCustomer.currentOrder);
                }

                Console.WriteLine("Your order has been made successfully!\n");
            }

            void Option5()
            {
                Console.WriteLine();
                if (customerDic.Count > 0)
                {
                    Console.WriteLine($"{"Name",-15}{"Member ID",-15}{"Date of Birth",-15}{"Tier",-10}{"Points",-10}{"Punch Card",-15}{"Order History Count",-20}{"Currently Ordering",-25}");
                    foreach (var entry in customerDic)
                    {

                        Console.WriteLine(entry.Value);

                    }
                    Console.WriteLine() ;
                    Console.Write("Which Customer do you wish to select (ID): ");

                    string customerId;
                    while (true)
                    {
                        customerId = Console.ReadLine();
                        if ((customerId != "") && customerDic.TryGetValue(customerId, out Customer selectedCustomer))
                        {
                            if (selectedCustomer.orderHistory.Count != 0 || (selectedCustomer.currentOrder != null && selectedCustomer.currentOrder.Id != 0))
                            {
                                

                                if (selectedCustomer.orderHistory.Count == 0)
                                {
                                    Console.WriteLine("No Order History.\n");
                                }
                                else
                                {
                                    Console.WriteLine("\nOrder History: ");
                                    Console.WriteLine("------------------------------------------");
                                    foreach (Order order in selectedCustomer.orderHistory)
                                    {
                                        Console.WriteLine($"{order}\n");
                                    }
                                    Console.WriteLine("------------------------------------------\n");
                                }

                                

                                if (selectedCustomer.currentOrder == null || selectedCustomer.currentOrder.Id == 0)
                                {
                                    Console.WriteLine("No current order.");
                                }
                                else
                                {
                                    Console.WriteLine("Current Order: ");
                                    Console.WriteLine("------------------------------------------");
                                    Console.WriteLine($"{selectedCustomer.currentOrder}");
                                    Console.WriteLine("------------------------------------------\n");
                                }

                            }
                            else
                            {
                                Console.WriteLine("This customer has no order history and current orders.\n");
                            }
                            break;
                        }
                        Console.Write("Invalid input. Please enter a valid customer ID:");
                    }
                }
                else
                {
                    Console.WriteLine("No customers available.\n");
                }

            }

            void Option6()
            {
                Console.WriteLine();
                if (customerDic.Count == 0)
                {
                    Console.WriteLine("There are no customers.");
                    return;
                }

                Customer selectedCustomer = null;

                Console.WriteLine($"{"Name",-15}{"Member ID",-15}{"Date of Birth",-15}{"Tier",-10}{"Points",-10}{"Punch Card",-15}{"Order History Count",-20}{"Currently Ordering",-25}");
                foreach (var entry in customerDic)
                {

                    Console.WriteLine(entry.Value);

                }
                do
                {

                    Console.Write("\nWhich Customer do you wish to select (ID or 0 to exit): ");

                    string customerId;
                    while (true)
                    {
                        string inputs = Console.ReadLine();
                        customerId = inputs;
                        if ((customerId != "") &&
                            customerDic.TryGetValue(customerId, out selectedCustomer))
                        {
                            break;
                        }
                        else if (customerId == "0")
                        {
                            Console.WriteLine();
                            return;
                        }
                        Console.Write("\nInvalid input. Please enter a valid customer ID (0 to exit): ");
                    }

                    if (selectedCustomer.currentOrder.Id == 0)
                    {
                        Console.WriteLine("The selected customer does not have a current order. Please select another customer.");
                    }

                } while (selectedCustomer.currentOrder.Id == 0);

                int iceCreamNumber = 1;
                Console.WriteLine("\nIce Creams in the Order:\n");
                foreach (IceCream icecream in selectedCustomer.currentOrder.IceCreamList)
                {
                    Console.WriteLine($"Ice Cream {iceCreamNumber}:\n{icecream.ToString()}");
                    iceCreamNumber++;
                }

                Console.WriteLine("Choose an action for the ice cream order:");
                Console.WriteLine("[0] Exit");
                Console.WriteLine("[1] Modify an existing ice cream");
                Console.WriteLine("[2] Add a new ice cream");
                Console.WriteLine("[3] Delete an existing ice cream");
                Console.Write("Enter your choice: ");

                string input = Console.ReadLine();
                List<string> output = new List<string>() {"1", "2", "3", "0" };
                while (!output.Contains(input))
                {
                    Console.WriteLine("\nInvalid Response.");
                    Console.Write("Enter your choice: ");
                    input = Console.ReadLine();
                }
                switch (input)
                {
                    case "1":
                        
                        Console.Write("Which Ice Cream do you wish to modify (0 to exit): ");

                        int selectedIceCreamIndex = -1;
                        while (selectedIceCreamIndex != 0)
                        {
                            string inputs = Console.ReadLine();
                            if (int.TryParse(inputs, out selectedIceCreamIndex) &&
                                selectedIceCreamIndex >= 0 &&
                                selectedIceCreamIndex <= selectedCustomer.currentOrder.IceCreamList.Count)
                            {
                                break;
                            }
                            Console.Write($"Invalid input. Please enter a number between 0 and {selectedCustomer.currentOrder.IceCreamList.Count}: ");
                            selectedIceCreamIndex = -1;
                        }

                        if (selectedIceCreamIndex == 0)
                        {
                            break;
                        }

                        selectedIceCreamIndex -= 1;
                        selectedCustomer.currentOrder.ModifyIceCream(selectedIceCreamIndex);
                        Console.WriteLine("Successfully Modified!\n");
                        break;

                    case "2":
                        AddIceCream(selectedCustomer);
                        selectedCustomer.currentOrder.Id -= 1;
                        Console.WriteLine("Successfully Added!\n");
                        break;

                    case "3":
                        DeleteIceCream(selectedCustomer);
                        
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 3.");
                        break;
                }
            }

            void Option7()
            {
                Order processOrder;

                // Check if gold queue has any queues followed by normal queue
                if (GoldQueue.Count() != 0)
                {
                    processOrder = GoldQueue.Peek();
                    GoldQueue.Dequeue();
                }
                else if (NormalQueue.Count() == 0)
                {
                    Console.WriteLine("No orders to process & checkout.\n");
                    return;
                }
                else
                {
                    processOrder = NormalQueue.Peek();
                    NormalQueue.Dequeue();
                }

                Console.WriteLine("\nOrder Detail");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine(processOrder.ToString());

                double totalBill = 0.00;
                double mostExPriceOfIceCream = 0.00;
                IceCream mostExIceCream = null;
                IceCream punchCardIceCream = null;
                List<IceCream> iceCreamList = processOrder.IceCreamList;

                // Finding most expensive icecream in the order
                foreach (IceCream icecream in processOrder.IceCreamList)
                {
                    double priceOfIceCream = icecream.CalculatePrice();
                    totalBill += priceOfIceCream;

                    if (mostExPriceOfIceCream < priceOfIceCream)
                    {
                        mostExPriceOfIceCream = priceOfIceCream;
                        mostExIceCream = icecream;
                    }
                }

                Console.WriteLine("------------------------------------------");
                Console.WriteLine($"Total Bill Amount: ${totalBill:0.00}");

                Customer processCustomer = null;

                foreach (KeyValuePair<string, Customer> kvp in customerDic)
                {
                    if (kvp.Value.currentOrder == processOrder)
                    {
                        processCustomer = kvp.Value;
                        break;
                    }
                }

                if (processCustomer != null)
                {
                    // Birthday Condition
                    if (processCustomer.IsBirthday())
                    {
                        Console.WriteLine("Happy Birthday!!!");
                        totalBill -= mostExPriceOfIceCream;
                    }

                    // Every 10 punchcard condition
                    if (processCustomer.rewards.PunchCard == 10)
                    {
                        totalBill -= processOrder.IceCreamList[0].CalculatePrice();
                        processCustomer.rewards.PunchCard = 0;
                    }

                    // Checking if they are eligible to redeem their points
                    if (processCustomer.rewards.Points != 0 && processCustomer.rewards.Tier != "Ordinary")
                    {
                        Console.WriteLine("------------------------------------------");
                        Console.WriteLine($"Membership Tier: {processCustomer.rewards.Tier}");
                        Console.WriteLine($"Point Balance: {processCustomer.rewards.Points}");

                        // Error handling for how many points they want to redeem and redeeming this points once selected
                        while (true)
                        {
                            try
                            {
                                Console.Write("How many points do you want to apply to your bill (1 point = $0.02): ");
                                int redeemedPoints = Int32.Parse(Console.ReadLine());
                                if (redeemedPoints > processCustomer.rewards.Points)
                                {
                                    Console.WriteLine("Insufficient Points. Please try again within your point balance.\n");
                                }
                                else
                                {
                                    if (redeemedPoints < 0)
                                    {
                                        Console.WriteLine("Invalid Input. Please enter a positive number.\n");
                                    }
                                    else if (redeemedPoints > processCustomer.rewards.Points)
                                    {
                                        Console.WriteLine("Insufficient Points. Please try again within your point balance.\n");
                                    }
                                    else
                                    {
                                        processCustomer.rewards.RedeemPoints(redeemedPoints);
                                        totalBill -= (redeemedPoints * 0.02);
                                        break;
                                    }
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Input. Please enter number within your point balance.\n");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("ERROR OCCURED!!!");
                                break;
                            }
                        }
                    }

                    Console.WriteLine($"Total Bill Amount (Applied Discount): ${totalBill:0.00}");
                    Console.Write("(Press any key to make payment)");
                    Console.ReadKey();
                    Console.WriteLine();
                    processCustomer.rewards.Punch();
                    processCustomer.rewards.AddPoints((int)Math.Round(totalBill));
                    processCustomer.currentOrder.TimeFulfilled = DateTime.Now;
                    AppendOrder(processCustomer, processOrder); // Appending order to the csv data file


                    Console.WriteLine("Payment Successful!\n");

                    string monthYearKey = processCustomer.currentOrder.TimeFulfilled.Value.ToString("MMM yyyy");
                    if (monthYearData.ContainsKey(monthYearKey))
                    {
                        monthYearData[monthYearKey] += totalBill;
                    }
                    else
                    {
                        monthYearData.Add(monthYearKey, totalBill);
                    }
                    processCustomer.orderHistory.Add(processOrder);
                    processCustomer.currentOrder = new Order();
                    Console.WriteLine("\nPlease provide a review for each unique flavor of ice cream in your order:");
                    List<Flavour> flavourHistory = new List<Flavour>();
                    // Getting review from customers
                    foreach (IceCream iceCream in processOrder.IceCreamList)
                    {
                        foreach (Flavour flavour in iceCream.Flavours)
                        {
                            if (flavourHistory.Contains(flavour))
                            {
                                continue;
                            }
                            else
                            {
                                Review review = GetReview(flavour);
                                processCustomer.Reviews.Add(review);
                                string dicKey = processCustomer.memberId.ToString().PadLeft(6, '0');

                                if (reviewDic.ContainsKey(dicKey))
                                {
                                    reviewDic[dicKey].Add(review);
                                }
                                else
                                {
                                    reviewDic.Add(dicKey, new List<Review> { review });
                                }
                                reviewCat[flavour.Type].Add(review);
                                flavourHistory.Add(flavour);

                                string memberId = processCustomer.memberId.ToString().PadLeft(6, '0');


                                string data = $"{memberId},{review.FlavourType},{review.Rating},{(review.Comment.Contains(",") || review.Comment.Contains("\"") ? $"\"{review.Comment.Replace("\"", "\"\"")}\"" : review.Comment)},{review.DateTime.ToString("dd-MM-yyyy HH:mm")}";
                                using (StreamWriter sw = new StreamWriter("reviews.csv", true))
                                {
                                    sw.WriteLine(data);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: No matching order found."); // Handle the case when no match is found
                }
            }

            // Appending order to the csv data file function
            void AppendOrder(Customer processCustomer, Order processOrder)
            {
                foreach (IceCream icecream in processOrder.IceCreamList)
                {
                    string memberId = processCustomer.memberId.ToString().PadLeft(6, '0');
                    string orderId = processOrder.Id.ToString();
                    string timeReceived = processOrder.TimeReceived.ToString("M/d/yyyy HH:mm");
                    string timeFulfilled = processOrder.TimeFulfilled?.ToString("M/d/yyyy HH:mm") ?? "ERROR PLEASE CHECK PROGRAM";
                    string option = icecream.Option;
                    string scoops = icecream.Scoops.ToString();
                    string dipped = "";
                    string waffleFlavour = "";
                    string flavours = "";
                    int flavoursCount = icecream.Flavours.Count();
                    int emptyFlavourCount = 3 - flavoursCount;
                    for (int i = 0; i < flavoursCount; i++)
                    {
                        flavours += $"{icecream.Flavours[i].Type},";
                    }
                    for (int i = 0; i < emptyFlavourCount; i++)
                    {
                        flavours += $",";
                    }
                    // NOTE: flavours would automatically have a comma at the last character so there is no need to add a comma after calling it in the datacreation

                    string toppings = "";
                    int toppingsCount = icecream.Toppings.Count();
                    int emptyToppingCount = 4 - toppingsCount;
                    for (int i = 0; i < toppingsCount; i++)
                    {
                        toppings += $"{icecream.Toppings[i].Type},";
                    }
                    for (int i = 0; i < emptyToppingCount; i++)
                    {
                        toppings += $",";
                    }
                    toppings.Substring(0, toppings.Length - 1);

                    string data = "";
                    if (icecream.Option == "Waffle")
                    {
                        Waffle waffle = (Waffle)icecream;
                        waffleFlavour = waffle.WaffleFlavour;
                        data = $"{orderId},{memberId},{timeReceived},{timeFulfilled},{option},{scoops},{dipped},{waffleFlavour},{flavours}{toppings}";
                    }
                    else if (icecream.Option == "Cone")
                    {
                        Cone cone = (Cone)icecream;
                        dipped = cone.Dipped.ToString().ToUpper();
                        data = $"{orderId},{memberId},{timeReceived},{timeFulfilled},{option},{scoops},{dipped},{waffleFlavour},{flavours}{toppings}";
                    }
                    else if(icecream.Option == "Cup")
                    {
                        data = $"{orderId},{memberId},{timeReceived},{timeFulfilled},{option},{scoops},{dipped},{waffleFlavour},{flavours}{toppings}";
                    }
                    else
                    {
                        Console.WriteLine("ERROR PLEASE CHECK PROGRAM!!!");
                    }
                    using (StreamWriter sw = new StreamWriter("orders.csv", true))
                    {
                        sw.WriteLine(data);
                    }
                }
            }

            void Option8()
            {
                Console.Write("Enter the year: ");
                int year;
                while (true)
                {
                    if (int.TryParse(Console.ReadLine(), out year))
                    {
                        
                       if (year >= 1900 && year <= 2100) { break; }
                        else
                        {
                            Console.Write("Invalid input. Please enter a valid year: ");
                        }
                        
                    }
                    else
                    {
                        Console.Write("Invalid input. Please enter a valid year: ");
                    }
                }
                double yearlyTotal = 0.00;
                Console.WriteLine();
                // Iterate over each month of the year
                for (int month = 1; month <= 12; month++)
                {
                    // Create a key for the month and year
                    string monthYearKey = new DateTime(year, month, 1).ToString("MMM yyyy");

                    // Check if the key exists in the dictionary
                    if (monthYearData.TryGetValue(monthYearKey, out double monthlyTotal))
                    {
                        Console.WriteLine($"{monthYearKey}: ${monthlyTotal:0.00}");
                        yearlyTotal += monthlyTotal;
                    }
                    else
                    {
                        // If the month does not have data, I print it as $0.00 
                        Console.WriteLine($"{monthYearKey}: $0.00");
                    }
                }

                // Print the total for the year
                Console.WriteLine($"\nTotal for {year}: ${yearlyTotal:0.00}\n");
            }


            Review GetReview(Flavour flavour)
            {
                int rating;
                string comment;
                while (true)
                {
                    Console.Write($"{char.ToUpper(flavour.Type[0]) + flavour.Type.Substring(1)} Flavour rating (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out rating) && rating >= 1 && rating <= 5)
                    {
                        break;
                    }
                    Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
                }
                while (true)
                {
                    Console.Write("Please enter a comment: ");
                    comment = Console.ReadLine();
                    if (comment == "")
                    {
                        Console.WriteLine("Blank Input. Please enter a comment");
                    }
                    else
                    {
                        break;
                    }
                }
                Console.WriteLine();

                return new Review(flavour.Type, rating, comment, DateTime.Now);
            }

            void Option9()
            {
                if (reviewDic.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("---------------------------------------------------");
                    foreach (KeyValuePair<string, List<Review>> kvp in reviewCat)
                    {
                        if (kvp.Value.Count > 0)
                        {
                            double totalRating = 0;
                            double avgRating;
                            int count = 0;
                            Console.WriteLine($"{char.ToUpper(kvp.Key[0]) + kvp.Key.Substring(1)} Reviews: ");
                            foreach (Review review in kvp.Value)
                            {
                                count += 1;
                                totalRating += review.Rating;
                                Console.WriteLine($"Review {count} ({review.DateTime}):");
                                Console.WriteLine($"{review}");
                                Console.WriteLine();
                            }
                            avgRating = Math.Round(totalRating / count, 2);
                            Console.WriteLine($"{char.ToUpper(kvp.Key[0]) + kvp.Key.Substring(1)} Avg Rating: {avgRating:0.00}");
                            Console.WriteLine();
                            Console.WriteLine("---------------------------------------------------");
                        }
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No reviews available");
                }
            }

            void Option10()
            {
                // Display Customers with Reviews once
                foreach (var entry in reviewDic)
                {
                    Console.WriteLine($"Member ID: {entry.Key.PadLeft(6, '0')}, Number of Reviews: {entry.Value.Count}");
                }

                Console.WriteLine() ;
                while (true)
                {
                    // User Selects a Customer or Quits
                    Console.Write("Enter Member ID or 0 to quit: ");
                    string input = Console.ReadLine();
                    if (input == "0") return;

                    string selectedMemberId = input.PadLeft(6, '0');
                    if (reviewDic.ContainsKey(selectedMemberId))
                    {
                        var reviews = reviewDic[selectedMemberId];

                        Console.WriteLine($"\nCustomer {selectedMemberId} Reviews:");
                        Console.WriteLine("-------------------------------------");
                        for (int i = 0; i < reviews.Count; i++)
                        {
                            Console.WriteLine($"Review {i + 1}:");
                            Console.WriteLine($"- Flavour: {reviews[i].FlavourType}");
                            Console.WriteLine($"- Rating: {reviews[i].Rating}");
                            Console.WriteLine($"- Comment: {reviews[i].Comment}");
                            Console.WriteLine("----------------------");
                        }
                        Console.WriteLine("------------------------------------------------------------(End)\n");

                        // User Selects a Review to Modify
                        int reviewIndex = -1;
                        while (reviewIndex == -1)
                        {
                            Console.Write("Enter Review Number to modify or 0 to quit: ");
                            input = Console.ReadLine();
                            if (input == "0") return;

                            if (int.TryParse(input, out int result) && result >= 1 && result <= reviews.Count)
                            {
                                reviewIndex = result - 1;
                            }
                            else
                            {
                                Console.WriteLine("Invalid review number. Please try again.\n");
                            }
                        }

                        Review reviewToModify = reviews[reviewIndex];

                        // User Enters New Rating
                        int newRating = -1;
                        while (newRating == -1)
                        {
                            Console.Write("Enter new rating (1-5): ");
                            input = Console.ReadLine();
                            if (int.TryParse(input, out newRating) && newRating >= 1 && newRating <= 5)
                            {
                                reviewToModify.Rating = newRating;
                            }
                            else
                            {
                                Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.\n");
                                newRating = -1;
                            }
                        }

                        // User Enters New Comment
                        string newComment = null;
                        while (string.IsNullOrEmpty(newComment))
                        {
                            Console.Write("Enter new comment: ");
                            newComment = Console.ReadLine();
                            if (string.IsNullOrEmpty(newComment))
                            {
                                Console.WriteLine("Comment cannot be empty. Please try again.\n");
                            }
                            else
                            {
                                reviewToModify.Comment = newComment;
                            }
                        }

                        reviewToModify.DateTime = DateTime.Now; // Update the review date to the current time

                        // Update the reviews.csv File
                        UpdateReviewFile();
                        Console.WriteLine("Review updated successfully.\n");
                        break; // Break out of the loop after successful update
                    }
                    else
                    {
                        Console.WriteLine("Member ID not found. Please try again.\n");
                    }
                }
            }


            void UpdateReviewFile()
            {
                using (StreamWriter sw = new StreamWriter("reviews.csv"))
                {
                    sw.WriteLine("MemberID,Flavour,Rating,Comment,DateTime"); // Update the header
                    foreach (var entry in reviewDic)
                    {
                        foreach (var review in entry.Value)
                        {
                            string data = $"{entry.Key.PadLeft(6, '0')},{review.FlavourType},{review.Rating},{(review.Comment.Contains(",") || review.Comment.Contains("\"") ? $"\"{review.Comment.Replace("\"", "\"\"")}\"" : review.Comment)},{review.DateTime.ToString("dd-MM-yyyy HH:mm")}";
                            sw.WriteLine(data);
                        }
                    }
                }
            }



            void DeleteIceCream(Customer selectedCustomer)
            {
                if (selectedCustomer.currentOrder.IceCreamList.Count == 1)
                {
                    Console.WriteLine("There cannot be 0 Ice Creams in the order.\n");
                    return;
                }
                string GetValidInput(string prompt, int maxValidResponse)
                {
                    string response;
                    int responseNumber;
                    do
                    {
                        Console.Write(prompt);
                        response = Console.ReadLine().ToLower();

                        if (string.IsNullOrWhiteSpace(response))
                        {
                            Console.WriteLine("Input cannot be blank. Please try again.\n");
                            continue;
                        }

                        bool isNumeric = int.TryParse(response, out responseNumber);
                        if (!isNumeric || responseNumber < 0 || responseNumber > maxValidResponse)
                        {
                            Console.WriteLine($"Invalid input. Please enter a number between 0 and {maxValidResponse}.\n");
                        }
                    }
                    while (string.IsNullOrWhiteSpace(response) ||
                           !int.TryParse(response, out responseNumber) ||
                           responseNumber < 0 || responseNumber > maxValidResponse);
                    return response;
                }

                Console.WriteLine("\nIce Creams Within the Selected Customer's Current Order: ");
                string iceCreamDetails = "";
                int iceCreamCount = 1;
                foreach (IceCream iceCream in selectedCustomer.currentOrder.IceCreamList)
                {
                    iceCreamDetails += $"\nIce Cream #{iceCreamCount}:\n{iceCream}\n";
                    iceCreamCount++;
                }
                Console.WriteLine(iceCreamDetails);

                int maxIceCreamIndex = selectedCustomer.currentOrder.IceCreamList.Count;
                string userInput = GetValidInput("Select an ice cream to delete (0 to exit): ", maxIceCreamIndex);

                if (userInput != "0")
                {
                    int iceCreamIndex = int.Parse(userInput);
                    selectedCustomer.currentOrder.DeleteIceCream(iceCreamIndex);
                    Console.WriteLine("Successfully Deleted.\n");
                }
            }





            void AddIceCream(Customer selectedCustomer)
            {
                Dictionary<string, string> normalFlavor = new Dictionary<string, string>() { { "1", "Vanilla" }, { "2", "Chocolate" }, { "3", "Strawberry" } };
                Dictionary<string, string> specialFlavor = new Dictionary<string, string>() { { "4", "Durian" }, { "5", "Ube" }, { "6", "Sea Salt" } };
                Dictionary<string, string> iceCreamType = new Dictionary<string, string>() { { "1", "Cup" }, { "2", "Cone" }, { "3", "Waffle" } };
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
                            Dictionary<string, string> toppingDic = new Dictionary<string, string> { { "1", "Sprinkles" }, { "2", "Mochi" }, { "3", "Sago" }, { "4", "Oreos" }, { "0", "exit" } };
                            string input = GetValidInput($"Topping {i}/4 (0 to finish): ", new List<string> { "1", "2", "3", "4", "0" });
                            topping = toppingDic[input];

                            if (topping != "exit")
                            {
                                toppings.Add(new Topping(topping));
                                Console.WriteLine($"{topping} ADDED!");
                                i++;
                            }
                        }
                        while (topping != "exit" && i <= 4);
                    }
                }

                Console.WriteLine("\nTypes of Ice Cream:");
                Console.WriteLine("1. Cup\n2. Cone\n3. Waffle\n");

                string typeInput = GetValidInput("Select Type: ", new List<string> { "1", "2", "3" });
                string type = iceCreamType[typeInput];

                int scoops = GetNumberOfScoops();
                AddFlavors(scoops);
                AddToppings();

                if (type == "Waffle")
                {
                    string waffleFlavor = "Original";
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
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                    Console.WriteLine();
                }
                else if (type == "Cone")
                {
                    bool dipped = false;
                    string coneResponse = GetValidInput("Do you wish to dip your cone in chocolate? (yes / no): ", new List<string> { "yes", "no" });
                    if (coneResponse == "yes")
                    {
                        dipped = true;
                    }
                    IceCream newOne = new Cone(type, scoops, flavours, toppings, dipped);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                    Console.WriteLine();
                }
                else if (type == "Cup")
                {
                    IceCream newOne = new Cup(type, scoops, flavours, toppings);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                    Console.WriteLine();
                }

                selectedCustomer.currentOrder.Id = orderDic.Count + 1;
                selectedCustomer.currentOrder.TimeReceived = DateTime.Now;
                selectedCustomer.currentOrder.TimeFulfilled = null;
                Console.WriteLine("Order successfully added! Thank you for choosing our ice cream.");
                Console.WriteLine("--------------------------------------------------");
            }
        }
    }
}