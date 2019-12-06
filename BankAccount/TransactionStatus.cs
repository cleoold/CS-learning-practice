using System;

namespace BankAccount
{

    public class TransactionStatus
    {
        public string AccountNumber { get; }
        public string AccountOwner { get; }
        public TransactionStatusCode Status { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }

        public TransactionStatus(string number, string owner, TransactionStatusCode statusCode, DateTime date, string reason = "")
        {
            AccountNumber = number;
            AccountOwner = owner;
            Status = statusCode;
            Date = date;
            Reason = reason;
        }

        public static implicit operator bool(TransactionStatus tr) => tr.Status == TransactionStatusCode.SUCCESS;

        public static implicit operator string(TransactionStatus tr) => $"[Transaction status {tr.Status} date {tr.Date:yyyy/MM/dd HH:mm:ss} log {tr.Reason}]";

        public override string ToString() => (string)this;
    }
}
