using System;
using System.Collections.Generic;

namespace BankAccount
{
    class Program
    {
        private Dictionary<string, BankAccount> Accounts = new Dictionary<string, BankAccount>();

        private TransactionStatus CreateAccount(string name, string password, decimal initialBalance)
        {
            BankAccount newAcc;
            try
            {
                newAcc = new BankAccount(name, password, initialBalance);
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
            Console.Write("SET UP YOUR PASSWORD: ");
            string pass = Console.ReadLine().Trim();
            TransactionStatus newAccountLog;
            try
            {
                newAccountLog = CreateAccount(arguments[1], pass, Convert.ToDecimal(arguments[^1]));
                if (newAccountLog.Status == TransactionStatusCode.SUCCESS)
                    Console.WriteLine($"Successfully created account.\n    Your name is {newAccountLog.AccountOwner}\n    account number: {newAccountLog.AccountNumber}");
                Console.WriteLine(newAccountLog.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Initial balance is not a number. Account not created.");
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
            Console.Write("ENTER YOUR PASSWORD: ");
            string pass = Console.ReadLine().Trim();
            if (pass != Accounts[arguments[1]].Password)
            {
                Console.WriteLine("Password not correct.");
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
                var notes = String.Join(' ', arguments[2..^0]);
                Console.WriteLine(bankaccount.MakeDeposit(Convert.ToDecimal(arguments[1]), DateTime.Now, notes).ToString());
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
                var notes = String.Join(' ', arguments[2..^0]);
                Console.WriteLine(bankaccount.MakeWithdrawal(Convert.ToDecimal(arguments[1]), DateTime.Now, notes).ToString());
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
                var notes = String.Join(' ', arguments[3..^0]);
                Console.WriteLine(bankaccount.MakeTransfer(Convert.ToDecimal(arguments[2]), DateTime.Now, Accounts[arguments[1]], notes).ToString());
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
                    Console.WriteLine(bankaccount.GetAccountHistory());
                    Serialize.WriteBankAccount(bankaccount);
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
            var program = new Program();
            foreach (var fpath in System.IO.Directory.GetDirectories("accounts\\"))
            {
                var dirName = new System.IO.DirectoryInfo(fpath).Name;
                var bankaccount = Serialize.RestoreBankAccount(dirName);
                program.Accounts.Add(bankaccount.Number, bankaccount);
            }
            program.MainMenu();
            Console.WriteLine("Bye bye!");
        }
    }
}
