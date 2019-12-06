
namespace BankAccount
{
    public static class TranscationChecker
    {
        public static bool IsInitialDepositValid(decimal amount)
        {
            return amount >= 0;
        }
        
        public static bool IsAmountValid(decimal amount)
        {
            return amount > 0;
        }

        public static bool IsFundSufficientForWithdrawal(decimal amount, BankAccount ba)
        {
            return ba.Balance >= amount;
        }
    }
}
