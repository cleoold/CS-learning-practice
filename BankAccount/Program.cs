using System;
using System.Collections.Generic;

namespace BankAccount
{
    class Program
    {
        private Dictionary<string, BankAccount> Accounts = new Dictionary<string, BankAccount>();

        private TransactionStatus CreateAccount(string name, decimal initialBalance)
        {
            BankAccount newAcc;
            try
            {
                newAcc = new BankAccount(name, initialBalance);
            }
            catch (ArgumentOutOfRangeException err)
            {
                return new TransactionStatus("N/A", "N/A", TransactionStatusCode.FAILED, DateTime.Now, err.Message);
            }
            Accounts.Add(newAcc.Number, newAcc);
            return new TransactionStatus(newAcc.Number, newAcc.Owner, TransactionStatusCode.SUCCESS, DateTime.Now, "Creates new account");
        }

        private void PrintAccounts()
        {
            Console.WriteLine("All registered bank accounts:");
            foreach (var item in Accounts)
                Console.WriteLine(item.Value.ToString());
        }

        private void UserAccountCreationSession(string[] arguments)
        {
            if (arguments.Length < 3)
            {
                Console.WriteLine("Account creation expects a name and an initial deposit.");
                return;
            }
            TransactionStatus newAccountLog;
            try
            {
                newAccountLog = CreateAccount(arguments[1], Convert.ToDecimal(arguments[arguments.Length-1]));
                if (newAccountLog.Status == TransactionStatusCode.SUCCESS)
                    Console.WriteLine($"Successfully created account.\n    Your name is {newAccountLog.AccountOwner}\n    account number: {newAccountLog.AccountNumber}");
                Console.WriteLine(newAccountLog.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Initial balance is not a number.");
                return;
            }
        }

        private bool UserLoginSession(string[] arguments)
        {
            if (arguments.Length < 2)
            {
                Console.WriteLine("Login expects one account number.");
                return false;
            }
            if (!Accounts.ContainsKey(arguments[1]))
            {
                Console.WriteLine($"This account ({arguments[1]}) does not exist.");
                return false;
            }
            return true;
        }

        private void MainMenu()
        {
            Console.WriteLine(ATMDialogue.MainMenuLog);
            while (true)
            {
                Console.Write("Your action: ");
                var input = Console.ReadLine();
                var arguments = input.Split(' ');
                switch (arguments[0])
                {
                case "create":
                    UserAccountCreationSession(arguments);
                    break;
                case "login":
                    if (!UserLoginSession(arguments))
                        break;
                    UserAction(Accounts[arguments[1]]);
                    Console.WriteLine(ATMDialogue.MainMenuLog);
                    break;
                case "list":
                    PrintAccounts();
                    break;
                case "help":
                    Console.WriteLine(ATMDialogue.MainMenuLog);
                    break;
                case "back":
                    return;
                }
            }
        }

        private void UserDepositSession(BankAccount bankaccount, string[] arguments)
        {
            if (arguments.Length < 2)
            {
                Console.WriteLine("Deposit expects an amount.");
                return;
            }
            try
            {
                var notes = new List<string>();
                for (int j = 2; j++ < arguments.Length-1;) notes.Add(arguments[j]);
                Console.WriteLine(bankaccount.MakeDeposit(Convert.ToDecimal(arguments[1]), DateTime.Now, String.Join(" ", notes)).ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Amount of deposit is not a number.");
            }
        }

        private void UserWithdrawalSession(BankAccount bankaccount, string[] arguments)
        {
            if (arguments.Length < 2)
            {
                Console.WriteLine("Withdrawal expects an amount.");
                return;
            }
            try
            {
                var notes = new List<string>();
                for (int j = 2; j++ < arguments.Length-1;) notes.Add(arguments[j]);
                Console.WriteLine(bankaccount.MakeWithdrawal(Convert.ToDecimal(arguments[1]), DateTime.Now, String.Join(" ", notes)).ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Amount of deposit is not a number.");
            }
        }

        private void UserTransferSession(BankAccount bankaccount, string[] arguments)
        {
            if (arguments.Length < 3)
            {
                Console.WriteLine("Transfer expects an account number and an amount.");
                return;
            }
            if (!Accounts.ContainsKey(arguments[1]))
            {
                Console.WriteLine($"The account ({arguments[1]}) does not exist.");
                return;
            }
            try
            {
                var notes = new List<string>();
                for (int j = 3; j++ < arguments.Length-1;) notes.Add(arguments[j]);
                Console.WriteLine(bankaccount.MakeTransfer(Convert.ToDecimal(arguments[2]), DateTime.Now, Accounts[arguments[1]], String.Join(" ", notes)).ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Amount of deposit is not a number.");
            }
        }

        private void UserAction(BankAccount bankaccount)
        {
            Console.WriteLine(String.Format(ATMDialogue.UserActionLog, bankaccount.Owner.ToUpper()));
            while (true)
            {
                Console.Write("Your action: ");
                var input = Console.ReadLine();
                var arguments = input.Split(' ');
                switch (arguments[0])
                {
                case "deposit":
                    UserDepositSession(bankaccount, arguments);
                    break;
                case "withdraw":
                    UserWithdrawalSession(bankaccount, arguments);
                    break;
                case "transfer":
                    UserTransferSession(bankaccount, arguments);
                    break;
                case "history":
                    Console.WriteLine(bankaccount.AccountHistory);
                    break;
                case "help":
                    Console.WriteLine(String.Format(ATMDialogue.UserActionLog, bankaccount.Owner));
                    break;
                case "back":
                    return;
                }
            }
        }

        static void Main(string[] args)
        {
            new Program().MainMenu();
            Console.WriteLine("Bye bye!");
        }
    }
}
