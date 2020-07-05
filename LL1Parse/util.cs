using System.Collections.Generic;

namespace LL1Parse
{
    static class DictUtil
    {
        /// <summary>
        /// this make copies of the inner sets to make sure new dict does not refer to
        /// sets in old dict. however values in each nested set do not matter
        /// </summary>
        public static Dictionary<Symbol, HashSet<Symbol>> Clone(Dictionary<Symbol, HashSet<Symbol>> s)
        {
            var result = new Dictionary<Symbol, HashSet<Symbol>>();
            foreach (var pair in s)
                result.Add(pair.Key, new HashSet<Symbol>(pair.Value));
            return result;
        }

        public static bool Equal(Dictionary<Symbol, HashSet<Symbol>> x, Dictionary<Symbol, HashSet<Symbol>> y)
        {
            if (x.Count != y.Count) return false;
            foreach (var pair in x)
            {
                if (!y.ContainsKey(pair.Key)) return false;
                if (!pair.Value.SetEquals(y[pair.Key])) return false;
            }
            return true;
        }
    }
}
