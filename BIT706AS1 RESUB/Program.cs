using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT706AS1_RESUB
{
    class Program
    {

        static List<string> everydaySummary = new List<string>();
        static List<string> omniSummary = new List<string>();
        static List<string> investmentSummary = new List<string>();

        public class Client
        {
            private string _firstName;
            private string _lastName;
            private string _phoneNumber;
            private string _address;

            public Client(string firstName, string lastName, string phoneNumber, string address)
            {
                _firstName = firstName;
                _lastName = lastName;
                _phoneNumber = phoneNumber;
                _address = address;
            }

            public string ClientInfo =>                
                $"Client Info \n\nName: {_firstName} {_lastName}\nPhone Number: {_phoneNumber}\nAddress: {_address}";
        }

        public abstract class Account
        {           
            private Client _client;

            public Account(Client client, int accountNumber, decimal opening)
            {
                _client = client;
                this.AccountNumber = accountNumber;
                this.Opening = opening;
            }

            public Account(Client client, decimal opening) : this(client, Account.GenerateAccountNumber(), opening)
            { }

            public int AccountNumber { get; private set; }

            public string AccountType => $"{this.GetType().Name} Account";

            public abstract decimal Overdraft { get; protected set; }

            public decimal Opening { get; protected set; }
            public decimal Deposits { get; protected set; }
            public decimal Withdrawals { get; protected set; }
            public decimal Interest { get; protected set; }
            public decimal Fees { get; protected set; }            

            public decimal Balance => this.Opening + this.Deposits - this.Withdrawals + this.Interest - this.Fees;            

            private static Random _random = new Random();
            public static int GenerateAccountNumber() => _random.Next(100000000, 1000000000);

            public (decimal Withdrawn, decimal Fee) Withdraw(decimal amount)
            {
                if (amount <= 0m)
                {
                    throw new System.InvalidOperationException("Withdrawal amount must be positive");
                }
                decimal fee = 0;
                decimal availableBalance = this.Balance + this.Overdraft;
                if (amount > this.Balance)
                {                 
                    if (AccountType == "Everyday Account")
                    {
                        fee = 0m;
                        amount = 0m;
                    }
                    else if (AccountType == "Omni Account")
                    {
                        if (amount > availableBalance)
                        {
                            amount = 0m;
                            fee = 10m;
                        }
                    }
                    else if (AccountType == "Investment Account")
                    {
                        fee = 10m;
                        amount = 0m;
                    }

                }
                  
                else if (this.Balance < amount)
                {
                    amount = this.Balance;
                }
                this.Withdrawals += amount;
                this.Fees += fee;
                return (amount, fee);
            }

            public decimal Deposit(decimal amount)
            {
                if (amount <= 0m)
                {
                    throw new System.InvalidOperationException("Deposit amount must be positive");
                }
                this.Deposits += amount;
                return amount;
            }                  

        }

        public class Everyday : Account
        {
            public decimal MinBalance { get; private set; } = 0m;
            public decimal MaxBalance { get; private set; } = 1000000000000m;

            public decimal Fee { get; private set; } = 0m;

            public override decimal Overdraft { get; protected set; } = 0m;

            public decimal InterestRate { get; protected set; } = 0m;

            public Everyday(Client client, decimal opening) : base(client, opening)
            { }

            public Everyday(Client client, int accountNumber, decimal opening) : base(client, accountNumber, opening)
            { }
        }

        public class Investment : Account
        {
            public decimal Fee { get; private set; } = 10m;

            public override decimal Overdraft { get; protected set; } = 0m;

            public decimal InterestRate { get; protected set; } = 4m;

            public Investment(Client client, decimal opening) : base(client, opening)
            { }

            public Investment(Client client, int accountNumber, decimal opening) : base(client, accountNumber, opening)
            { }
        }

        public class Omni : Account
        {
            public decimal Fee { get; private set; } = 10m;

            public override decimal Overdraft { get; protected set; } = 1000m;            

            public decimal InterestRate { get; protected set; } = 4m;

            public Omni(Client client, decimal opening) : base(client, opening)
            { }

            public Omni(Client client, int accountNumber, decimal opening) : base(client, accountNumber, opening)
            { }
        }

        private static void DisplayBalance(Account account)
        {
            Console.WriteLine($"{account.AccountType} Balance: {account.Balance:$#,##0.00}");
        }

        private static void DepositAmount(Account account)
        {
            
            Console.WriteLine("How much would you like to deposit?");
            decimal deposited = account.Deposit(decimal.Parse(Console.ReadLine()));
            Console.WriteLine($"\nYou deposited: {deposited:$#,##0.00} into your {account.AccountType}");

            var time = DateTime.Now;

            if (account.AccountType == "Everyday Account")
            {
                everydaySummary.Add($"\nDeposited: {deposited:$#,##0.00} {time.ToString()}");
            }
            if (account.AccountType == "Omni Account")
            {
                omniSummary.Add($"\nDeposited: {deposited:$#,##0.00} {time.ToString()}");
            }
            if (account.AccountType == "Investment Account")
            {
                investmentSummary.Add($"\nDeposited: {deposited:$#,##0.00} {time.ToString()}");
            }

        }

        private static void WithdrawAmount(Account account)
        {
            Console.WriteLine("How much would you like to withdraw?");
            var result = account.Withdraw(decimal.Parse(Console.ReadLine()));
            Console.WriteLine($"\nYou withdrew: {result.Withdrawn:$#,##0.00}");
            if (result.Fee != 0m)
            {
                Console.WriteLine($"With fee: {result.Fee:$#,##0.00}");
            }

            if (result.Withdrawn == 0m)
            {
                Console.WriteLine("Insufficient Funds");
            }

            var time = DateTime.Now;

            if (account.AccountType == "Everyday Account")
            {
                everydaySummary.Add($"\nWithdrew: {result.Withdrawn:$#,##0.00} {time.ToString()}");
            }
            if (account.AccountType == "Omni Account")
            {
                omniSummary.Add($"\nWithdrew: {result.Withdrawn:$#,##0.00} {time.ToString()} Failed Transaction Fee: {result.Fee:$#,##0.00}");
            }
            if (account.AccountType == "Investment Account")
            {
                investmentSummary.Add($"\nWithdrew: {result.Withdrawn:$#,##0.00} {time.ToString()} Failed Transaction Fee: {result.Fee:$#,##0.00}");
            }
        }

        private static void DisplayDetails(Everyday account)
        {
            Console.WriteLine("Everyday Banking Details\n");
            Console.WriteLine($"Account Number: {account.AccountNumber}");
            Console.WriteLine($"{account.AccountType} Balance: {account.Balance:$#,##0.00}");
            Console.WriteLine("Overdraft: " + account.Overdraft);
            Console.WriteLine("Interest Rate: " + account.InterestRate + "%");
            Console.WriteLine("Fee: " + account.Fee);
        }

        private static void DisplayDetails(Investment account)
        {
            Console.WriteLine("Investment Banking Details\n");
            Console.WriteLine($"Account Number: {account.AccountNumber}");
            Console.WriteLine($"{account.AccountType} Balance: {account.Balance:$#,##0.00}");
            Console.WriteLine("Overdraft: " + account.Overdraft);
            Console.WriteLine("Interest Rate: " + account.InterestRate + "%");
            Console.WriteLine("Fee: " + account.Fee);
        }

        private static void DisplayDetails(Omni account)
        {
            Console.WriteLine("Omni Banking Details\n");
            Console.WriteLine($"Account Number: {account.AccountNumber}");
            Console.WriteLine($"{account.AccountType} Balance: {account.Balance:$#,##0.00}");
            Console.WriteLine("Overdraft: " + account.Overdraft);
            Console.WriteLine("Interest Rate: " + account.InterestRate + "%");
            Console.WriteLine("Fee: " + account.Fee);
        }

        private static void Main(string[] args)
        {
            bool test = false;

            Client client = new Client("John", "Doe", "0211234567", "99 Great South Rd");

            Everyday everyday = new Everyday(client, Account.GenerateAccountNumber(), 2000m);
            Investment investment = new Investment(client, 500m);
            Omni omni = new Omni(client, 1000m);

            do
            {           
                Console.Clear();
                DisplayMenu();
                string userchoice = Console.ReadLine();

                switch (userchoice.ToUpper())
                {
                    case "1":
                        Console.Clear();
                        DisplayHeader();
                        Console.WriteLine(client.ClientInfo);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "2A":
                        Console.Clear();
                        DisplayHeader();
                        DisplayBalance(everyday);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "2B":
                        Console.Clear();
                        DisplayHeader();
                        DisplayBalance(investment);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "2C":
                        Console.Clear();
                        DisplayHeader();
                        DisplayBalance(omni);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "3A":
                        Console.Clear();
                        DisplayHeader();
                        DepositAmount(everyday);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "3B":
                        Console.Clear();
                        DisplayHeader();
                        DepositAmount(investment);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "3C":
                        Console.Clear();
                        DisplayHeader();
                        DepositAmount(omni);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "4A":
                        Console.Clear();
                        DisplayHeader();
                        WithdrawAmount(everyday);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "4B":
                        Console.Clear();
                        DisplayHeader();
                        WithdrawAmount(investment);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "4C":
                        Console.Clear();
                        DisplayHeader();
                        WithdrawAmount(omni);
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "5":
                        Console.Clear();
                        DisplayHeader();
                        DisplayDetails(everyday);
                        foreach (string transaction in everydaySummary)
                        {
                            Console.WriteLine(transaction);
                        }
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "6":
                        Console.Clear();
                        DisplayHeader();
                        DisplayDetails(investment);
                        foreach (string transaction in investmentSummary)
                        {
                            Console.WriteLine(transaction);
                        }
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "7":
                        Console.Clear();
                        DisplayHeader();
                        DisplayDetails(omni);
                        foreach (string transaction in omniSummary)
                        {
                            Console.WriteLine(transaction);
                        }
                        DisplayFooter();
                        MenuPromt();
                        break;
                    case "8":
                        Console.Clear();
                        DisplayHeader();
                        Console.WriteLine("Exiting application");
                        DisplayFooter();
                        MenuPromt();
                        test = true;
                        break;
                    default:
                        Console.Clear();
                        DisplayHeader();
                        Console.WriteLine("Please enter a valid option!");
                        DisplayFooter();
                        MenuPromt();
                        test = false;
                        break;
                }
            } while (!test);
        }
        
        private static void DisplayMenu()
        {
            DisplayHeader();
            Console.WriteLine("1 - View Client Info");
            Console.WriteLine("\n2 - View Account Balance");
            Console.WriteLine("\n       2A - Everyday   2B - Investment   2C - Omni");
            Console.WriteLine("\n\n3 - Deposit Funds");
            Console.WriteLine("\n       3A - Everyday   3B - Investment   3C - Omni");
            Console.WriteLine("\n\n4 - Withdraw Funds");
            Console.WriteLine("\n       4A - Everyday   4B - Investment   4C - Omni");
            Console.WriteLine("\n\n5 - View Everyday Account Details");
            Console.WriteLine("\n6 - View Investment Account Details");
            Console.WriteLine("\n7 - View Omni Account details");
            Console.WriteLine("\n8 - Exit");
            DisplayFooter();

        }

        private static void DisplayHeader()
        {
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("                  Online Banking System");
            Console.WriteLine("-------------------------------------------------------\n");
        }

        private static void DisplayFooter()
        {
            Console.WriteLine("\n-------------------------------------------------------");
            Console.WriteLine("                 Designed by Reuben James");
            Console.WriteLine("-------------------------------------------------------");
        }

        private static void MenuPromt()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
