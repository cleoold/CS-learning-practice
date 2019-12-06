namespace BankAccount
{
    public static class ATMDialogue
    {
        public const string MainMenuLog = @"MAIN MENU:
    create [name] [initial balance]       creates account
    login [acc number]                    navigates to account
    list                                  lists all accounts
    back                                  go back
    help                                  see this message again";

        public const string UserActionLog = @"{0}'s BANK ACCOUNT:
    deposit [amount] [notes?]               deposit money
    withdraw [amount] [notes?]              withdraw money
    transfer [acc number] [amount] [notes?] transfer money to person with given account number
    history                                 see transaction history
    back                                    go back
    help                                    see this message again";
    }
}
