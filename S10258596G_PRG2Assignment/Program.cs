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

                        DateTime dob = DateTime.ParseExact(details[2], "d/M/yyyy", CultureInfo.InvariantCulture);
                        PointCard pointCard = new PointCard(Int32.Parse(details[4]),Int32.Parse(details[5]));
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
                        for (int i = 11; i < 4+11; i++)
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




                        // Finish creating order for customer
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

            /*
            void displayOrder()
            {
                foreach (KeyValuePair<int, Order> kvp in orderDic)
                {
                    Console.WriteLine(kvp.Value.ToString());
                }
            }

            displayOrder();
            */

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

                        if (option >= 0 && option <= 8) // Included 0 as a valid option
                        {
                            return option;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a number between 0 and 7.");
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 0 and 7.");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 0 and 7.");
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
                        // 1) List all customers - display the information of all the customers
                        Option1();
                        break;

                    case 2:
                        // 2) List all current orders - display the information of all current orders in both the gold members and regular queue
                        Option2();
                        break;

                    case 3:
                        // 3) Register a new customer
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
                    Console.WriteLine($"{customer.name,-10} \t{kvp.Key,-6} \t\t{customer.dob.ToString("d/M/yyyy"),-10} \t{customer.rewards.Tier, -8} \t{customer.rewards.Points,-6} \t{customer.rewards.PunchCard,-9}");
                }
                Console.WriteLine();
            }

            void Option2()
            {
                Console.WriteLine("Orders in the Normal Queue: ");

                foreach (Order order in NormalQueue)
                {
                    Console.WriteLine(order.ToString());
                }
                Console.WriteLine("Orders in the Gold Queue: ");
                foreach (Order order in GoldQueue)
                {
                    Console.WriteLine(order.ToString());
                }
                Console.WriteLine();
            }

            void Option3()
            {
                string name = "";
                string id = "";
                DateTime dob;

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

                Customer newCustomer = new Customer(name, Int32.Parse(id), dob);
                customerDic.Add(id, newCustomer);

                string data = $"{name},{id},{dob.ToString("d/M/yyyy")},{newCustomer.rewards.Tier},{newCustomer.rewards.Points},{newCustomer.rewards.PunchCard}";

                using (StreamWriter sw = new StreamWriter("customers.csv", true))
                {
                    sw.WriteLine(data);
                }
                Console.WriteLine("Registration Status: Successful!\n"); // Might need to shift it in the error handling part instead
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

                Order newOrder = new Order();

                int orderID = orderDic.Count + 1;
                DateTime timeRecieved = DateTime.Now;
                DateTime? timefulfilled = null;

                int i = 1;
                Console.WriteLine();
                Console.WriteLine($"IceCream #{i}");
                AddIceCream(selectedCustomer);

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
                    Console.Write("Which Customer do you wish to select (ID): ");

                    string customerId;
                    while (true)
                    {
                       customerId = Console.ReadLine();
                        if ((customerId != "") &&
                            customerDic.TryGetValue(customerId, out Customer selectedCustomer))
                        {
                            if (selectedCustomer.orderHistory.Count != 0)
                            {
                                Console.WriteLine();
                                foreach (Order order in selectedCustomer.orderHistory)
                                {
                                    Console.WriteLine($"{order}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("This customer has no order history.");
                            }
                            break;
                        }
                        Console.Write("Invalid input. Please enter a valid customer ID:");
                    }
                }
                else
                {
                    Console.WriteLine("No customers available.");
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
                   
                    Console.Write("Which Customer do you wish to select (ID or 0 to exit): ");

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
                            return;
                        }
                        Console.Write("Invalid input. Please enter a valid customer ID (0 to exit):");
                    }

                    if (selectedCustomer.currentOrder.Id == 0)
                    {
                        Console.WriteLine("The selected customer does not have a current order. Please select another customer.");
                    }

                } while (selectedCustomer.currentOrder.Id == 0);

                Console.WriteLine(selectedCustomer.currentOrder);

                Console.WriteLine("\nChoose an action for the ice cream order:");
                Console.WriteLine("[1] Modify an existing ice cream");
                Console.WriteLine("[2] Add a new ice cream");
                Console.WriteLine("[3] Delete an existing ice cream");
                Console.Write("Enter your choice: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        int iceCreamNumber = 1;
                        foreach (IceCream icecream in selectedCustomer.currentOrder.IceCreamList)
                        {
                            Console.WriteLine($"Ice Cream {iceCreamNumber}: {icecream.ToString()}");
                            iceCreamNumber++;
                        }
                        Console.Write("Which Ice Cream do you wish to modify: ");

                        int selectedIceCreamIndex = 0;
                        while (true)
                        {
                            string inputs = Console.ReadLine();
                            if (int.TryParse(inputs, out selectedIceCreamIndex) &&
                                selectedIceCreamIndex >= 1 &&
                                selectedIceCreamIndex <= selectedCustomer.currentOrder.IceCreamList.Count)
                            {
                                break;
                            }
                            Console.WriteLine($"Invalid input. Please enter a number between 1 and {selectedCustomer.currentOrder.IceCreamList.Count}:");
                        }

                        selectedIceCreamIndex -= 1;
                        selectedCustomer.currentOrder.ModifyIceCream(selectedIceCreamIndex);
                        Console.WriteLine("Successfully Modified!");
                        break;

                    case "2":
                        AddIceCream(selectedCustomer);
                        Console.WriteLine("Successfully Added!");
                        break;

                    case "3":
                        DeleteIceCream(selectedCustomer);
                        Console.WriteLine("Successfully Deleted.");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 3.");
                        break;
                }
            }

            void Option7()
            {
                Order processOrder;

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
                    if (processCustomer.IsBirthday())
                    {
                        totalBill -= mostExPriceOfIceCream;
                    }

                    if (processCustomer.rewards.PunchCard == 10)
                    {
                        totalBill -= processOrder.IceCreamList[0].CalculatePrice();
                        processCustomer.rewards.PunchCard = 0;
                    }

                    if (processCustomer.rewards.Points != 0 && processCustomer.rewards.Tier != "Ordinary")
                    {
                        Console.WriteLine("------------------------------------------");
                        Console.WriteLine($"Membership Tier: {processCustomer.rewards.Tier}");
                        Console.WriteLine($"Point Balance: {processCustomer.rewards.Points}");
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
                            catch (FormatException e) {
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
                }
                else
                {
                    Console.WriteLine("ERROR: No matching order found."); // Handle the case when no match is found
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
                        break;
                    }
                    else
                    {
                        Console.Write("Invalid input. Please enter a valid year: ");
                    }
                }
                Console.WriteLine("\n\n") ;
                double yearlyTotal = 0.00;
                

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
                Console.WriteLine($"\nTotal for {year}: ${yearlyTotal:0.00}");
            }


            void DeleteIceCream(Customer selectedCustomer)
            {
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
                            Console.WriteLine("Input cannot be blank. Please try again.");
                            continue;
                        }

                        bool isNumeric = int.TryParse(response, out responseNumber);
                        if (!isNumeric || responseNumber < 0 || responseNumber > maxValidResponse)
                        {
                            Console.WriteLine($"Invalid input. Please enter a number between 0 and {maxValidResponse}.");
                        }
                    }
                    while (string.IsNullOrWhiteSpace(response) ||
                           !int.TryParse(response, out responseNumber) ||
                           responseNumber < 0 || responseNumber > maxValidResponse);
                    return response;
                }

                Console.WriteLine("Ice Creams Within the Selected Customer's Current Order: ");
                Console.WriteLine(selectedCustomer.currentOrder);

                int maxIceCreamIndex = selectedCustomer.currentOrder.IceCreamList.Count; 
                string userInput = GetValidInput("Select an ice cream to delete (0 to exit): ", maxIceCreamIndex);

                if (userInput != "0")
                {
                    int iceCreamIndex = int.Parse(userInput) - 1;
                    selectedCustomer.currentOrder.DeleteIceCream(iceCreamIndex);
                                                               
                }
            }





            void AddIceCream(Customer selectedCustomer)
            {
                Dictionary<string, string> normalFlavor = new Dictionary<string, string>() { { "1", "vanilla" }, { "2", "chocolate" }, { "3", "strawberry" } };
                Dictionary<string, string> specialFlavor = new Dictionary<string, string>() { { "4", "durian" }, { "5", "ube" }, { "6", "sea salt" } };
                Dictionary<string, string> iceCreamType = new Dictionary<string, string>() { { "1", "cup" }, { "2", "cone" }, { "3", "waffle" } };
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
                    Console.WriteLine("Premium Flavors: \n4. Durian\n5. Sea Salt\n6. Ube\n");
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

                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Types of Ice Cream:");
                Console.WriteLine("1. Cup\n2. Cone\n3. Waffle\n");
                string typeInput = GetValidInput("Select Type: ", new List<string> { "1", "2", "3" });
                string type = iceCreamType[typeInput];
                int scoops = GetNumberOfScoops();
                AddFlavors(scoops);
                AddToppings();


                if (type == "waffle")
                {
                    string waffleFlavor = "original";


                    string waffleResponse = GetValidInput("Do you wish to change the flavor of your waffle? (yes / no): ", new List<string> { "yes", "no" });
                    if (waffleResponse == "yes")
                    {
                        Console.WriteLine("\n------------------------------------------");
                        Console.WriteLine("Waffle Flavours:");
                        Console.WriteLine("- Red Velvet\n- Charcoal\n- Pandan Waffle\n");
                        waffleFlavor = GetValidInput("Select Flavour From Above: ", new List<string> { "red velvet", "charcoal", "pandan waffle" });
                    }
                    IceCream newOne = new Waffle(type, scoops, flavours, toppings, waffleFlavor);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                    Console.WriteLine();

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
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                    Console.WriteLine();
                }
                else if (type == "cup")
                {
                    IceCream newOne = new Cup(type, scoops, flavours, toppings);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                    Console.WriteLine();
                }

                selectedCustomer.currentOrder.Id = orderDic.Count + 1;
                selectedCustomer.currentOrder.TimeReceived = DateTime.Now;
                selectedCustomer.currentOrder.TimeFulfilled = null;

            }
        }
    }
}