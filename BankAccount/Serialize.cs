using System.IO;
using System.Text.Json;

namespace BankAccount
{
    public static class Serialize
    {
        public static void WriteTransaction(BankAccount bankaccount, Transaction transaction)
        {
            string jsonstring = JsonSerializer.Serialize(transaction);
            string accountNumber = bankaccount.Number;
            string transactionNumber = transaction.TransactionNumber;
            using (var file = new StreamWriter($"accounts\\{accountNumber}\\transac_{transactionNumber}.json"))
            {
                file.WriteLine(jsonstring);
            }
        }
        
        public static void WriteBankAccount(BankAccount bankaccount)
        {
            string jsonstring = JsonSerializer.Serialize(bankaccount);
            string accountNumber = bankaccount.Number;
            Directory.CreateDirectory($"accounts\\{accountNumber}");
            using (var file = new StreamWriter($"accounts\\{accountNumber}\\account_{accountNumber}.json"))
            {
                file.WriteLine(jsonstring);
            }
        }

        public static BankAccount RestoreBankAccount(string number)
        {
            string accountJson = File.ReadAllText($"accounts\\{number}\\account_{number}.json");
            var bankaccount = JsonSerializer.Deserialize<BankAccount>(accountJson);
            foreach (var fpath in Directory.GetFiles($"accounts\\{number}"))
            {
                if (!fpath.Contains("transac")) continue;
                string transactionJson = File.ReadAllText(fpath);
                var transaction = JsonSerializer.Deserialize<Transaction>(transactionJson);
                bankaccount._PushTransaction(transaction);
            }
            return bankaccount;
        }
    }
}
