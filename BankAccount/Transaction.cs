using System;

namespace BankAccount
{
    public class Transaction
    {
        public string AccountNumber { get; }
        public string AccountOwner { get; }
        public decimal Amount { get; }
        public DateTime Date { get; }
        public string Notes { get; }

        public Transaction(string number, string owner, decimal amount, DateTime date, string note)
        {
            AccountNumber = number;
            AccountOwner = owner;
            Amount = amount;
            Date = date;
            Notes = note;
        }

        public static implicit operator string(Transaction tr) => $"[Transaction amount {tr.Amount:0.00} date {tr.Date:yyyy/MM/dd HH:mm:ss} notes {tr.Notes}]";

        public override string ToString() => (string)this;
    }
}

