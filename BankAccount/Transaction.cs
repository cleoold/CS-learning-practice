using System;

namespace BankAccount
{
    public class Transaction
    {
        private static Int64 TransactionNumberSeed = 1000000000;

        public string TransactionNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AccountOwner { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }

        public Transaction(string number, string owner, decimal amount, DateTime date, string note)
        {
            TransactionNumber = (TransactionNumberSeed++).ToString();
            AccountNumber = number;
            AccountOwner = owner;
            Amount = amount;
            Date = date;
            Notes = note;
        }

        public Transaction()
        {
            TransactionNumberSeed += 1;
        }

        public static implicit operator string(Transaction tr) => $"[Transaction amount {tr.Amount:0.00} date {tr.Date:yyyy/MM/dd HH:mm:ss} notes {tr.Notes} number {tr.TransactionNumber}]";

        public override string ToString() => (string)this;
    }
}

