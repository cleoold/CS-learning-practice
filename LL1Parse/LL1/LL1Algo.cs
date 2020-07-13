namespace LL1Parse
{
    partial class LL1Algo
    {
        public readonly CFG Thecfg;
        public NullableComputer? Nullable { private set; get; }
        public FirstComputer? First { private set; get; }
        public FollowComputer? Follow { private set; get; }
        public Parse? Parser { private set; get; }

        public LL1Algo(CFG thecfg)
        {
            this.Thecfg = thecfg;
        }

        public void Compute()
        {
            Nullable = NullableComputer.Compute(this);
            First = FirstComputer.Compute(this);
            Follow = FollowComputer.Compute(this);
            Parser = Parse.Compute(this);
        }
    }
}
