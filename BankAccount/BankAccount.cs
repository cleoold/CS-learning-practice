using System;
using System.Collections.Generic;
using System.Globalization;

namespace BankAccount
{
    public class BankAccount
    {
        private static int AccountNumberSeed = 66709220;

        private List<Transaction> AllTransactions = new List<Transaction>();

        public string Password { get; set; }
        public string Number { get; set; }
        public string Owner { get; set; }
        public decimal Balance
        {
            get
            {
                decimal balance = 0M;
                foreach (var item in AllTransactions)
                    balance += item.Amount;
                return balance;
            }
        }

        public BankAccount(string name, string password, decimal initialBalance = 0M)
        {
            Password = password;
            Owner = name;
            if (!TranscationChecker.IsInitialDepositValid(initialBalance))
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Cannot open account");
            Number = (AccountNumberSeed++).ToString();
            var firstTransaction = new Transaction(Number, Owner, initialBalance, DateTime.Now, "Opens account");
            AllTransactions.Add(firstTransaction);
            Serialize.WriteBankAccount(this);
            Serialize.WriteTransaction(this, firstTransaction);
        }

        public BankAccount()
        {
            AccountNumberSeed += 1;
        }

        public static implicit operator string(BankAccount ba) => $"[Bank account of {ba.Owner} ID {ba.Number}]";

        public override string ToString() => (string)this;

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => HashCode.Combine(AllTransactions, Number, Owner, Balance, GetAccountHistory());

        public void _PushTransaction(Transaction transaction)
        {
            AllTransactions.Add(transaction);
        }

        private TransactionStatus _MakeDeposit(decimal amount, DateTime date, string note)
        {
            var transaction = new Transaction(Number, Owner, amount, date, note);
            AllTransactions.Add(transaction);
            Serialize.WriteTransaction(this, transaction);
            return new TransactionStatus(Number, Owner, TransactionStatusCode.SUCCESS, DateTime.Now, "Deposit successful");
        }

        public TransactionStatus MakeDeposit(decimal amount, DateTime date, string note)
        {
            if (!TranscationChecker.IsAmountValid(amount))
                return new TransactionStatus(Number, Owner, TransactionStatusCode.FAILED, DateTime.Now, "Amount of deposit is not valid");
            return _MakeDeposit(amount, date, note);
        }

        private TransactionStatus _MakeWithdrawal(decimal amount, DateTime date, string note)
        {
            var transaction = new Transaction(Number, Owner, -amount, date, note);
            AllTransactions.Add(transaction);
            Serialize.WriteTransaction(this, transaction);
            return new TransactionStatus(Number, Owner, TransactionStatusCode.SUCCESS, DateTime.Now, "Withdrawal successful");
        }

        public TransactionStatus MakeWithdrawal(decimal amount, DateTime date, string note)
        {
            if (!TranscationChecker.IsAmountValid(amount))
                return new TransactionStatus(Number, Owner, TransactionStatusCode.FAILED, DateTime.Now, "Amount of withdrawal is not valid");
            if (!TranscationChecker.IsFundSufficientForWithdrawal(amount, this))
                return new TransactionStatus(Number, Owner, TransactionStatusCode.FAILED, DateTime.Now, "Not sufficient fund for this withdrawal");
            return _MakeWithdrawal(amount, date, note);
        }

        public TransactionStatus MakeTransfer(decimal amount, DateTime date, BankAccount recipient, string note)
        {
            string note2 = $"Transfer from {Owner} to {recipient.Owner} note: {note}";
            var isWithdrawnStatus = MakeWithdrawal(amount, date, note2);
            if (isWithdrawnStatus.Status != TransactionStatusCode.SUCCESS)
                return isWithdrawnStatus;
            return recipient._MakeDeposit(amount, date, note2);
        }

        public string GetAccountHistory()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine($"ACCOUNT SUMMARY FOR {Owner.ToUpper()} {Number}:");
            report.AppendLine($"{"Transaction number",-25}{"Date",-25}{"Amount",-13}Note");
            foreach (var item in AllTransactions)
                report.AppendLine($"{item.TransactionNumber,-25}{item.Date,-25:yyyy/MM/dd HH:mm:ss}{item.Amount.ToString("C", CultureInfo.CreateSpecificCulture("fr-CA")),-13}{item.Notes}");
            report.Append($"{"Current balance:",-25}{Balance.ToString("C", CultureInfo.CreateSpecificCulture("fr-CA"))}");
            return report.ToString();
        }
    }
}

