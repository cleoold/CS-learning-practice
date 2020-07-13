using System.Collections.Generic;

namespace LL1Parse
{
    partial class LL1Algo
    {
        public abstract class AbstractComputer<ResultType>
            : IEnumerable<KeyValuePair<Symbol, ResultType>>
        {
            public readonly LL1Algo Algo;

            protected AbstractComputer(LL1Algo algo)
            {
                Algo = algo;
            }

            public abstract Dictionary<Symbol, ResultType> Result { get; }

            public ResultType this[Symbol sym] { get => Result[sym]; }

            public IEnumerator<KeyValuePair<Symbol, ResultType>> GetEnumerator()
                => Result.GetEnumerator();
            
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                => Result.GetEnumerator();
        }
    }
}
