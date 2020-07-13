using System;

namespace LL1Parse
{
    sealed class Symbol
    {
        public enum Types
        {
            Terminal,
            NonTerminal
        }

        public readonly string Name;
        public readonly Types Type;

        public Symbol(string name, Types group)
        {
            Name = name;
            Type = group;
        }

        public override bool Equals(object? obj)
        {
            return obj is Symbol symbol &&
                   Name == symbol.Name &&
                   Type == symbol.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
