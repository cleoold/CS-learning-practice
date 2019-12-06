using System;

namespace BankAccount
{
    public static class Examples
    {
        public static void Garbage()
        {
            var Alice = new BankAccount("Alice");
            var Bob = new BankAccount("Bob", 16);

            var tr = new TransactionStatus[5]
            {
                Alice.MakeDeposit(5450, DateTime.Now, "fix date deposit"),
                Alice.MakeDeposit(3000, DateTime.Now, "Flex date deposit"),
                Alice.MakeWithdrawal(12.34M, DateTime.Now, "Ramen Restaurant"),
                Alice.MakeTransfer(500, DateTime.Now, Bob, "rent"),
                Bob.MakeWithdrawal(1329.5M, DateTime.Now, "buy computer")
            };

            Console.WriteLine(Alice.AccountHistory);
            Console.WriteLine(Bob.AccountHistory);
            Console.WriteLine("All transactions:");
            foreach (var item in tr) Console.WriteLine(item.ToString());
        }
    }
}
