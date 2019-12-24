using System.Collections.Generic;

namespace GraphTraversal
{
    public class Vertex<TName, TLabel>
    {
        private List<LEdge<TName,TLabel>> _Edges = new List<LEdge<TName,TLabel>>();

        public TName Name { get; }
        public List<LEdge<TName,TLabel>> Edges { get => _Edges; }

        public Vertex(TName init)
        {
            Name = init;
        }

        public void addEdge(Vertex<TName,TLabel> o, TLabel label)
        {
            _Edges.Add(new LEdge<TName,TLabel>(o, this, label));
        }
    }

    public class LEdge<TName, TLabel>
    {
        public Vertex<TName,TLabel> To { get; }
        public Vertex<TName,TLabel> From { get; }
        public TLabel Label { get; }

        public LEdge(Vertex<TName,TLabel> to, Vertex<TName,TLabel> from, TLabel label)
        {
            this.To = to;
            this.From = from;
            this.Label = label;
        }
    }
    // used in priority queue, determines what edge is popped
    public class LEdgeComparer<TName, TLabel> : IComparer<LEdge<TName, TLabel>>
    {
        public int Compare(LEdge<TName,TLabel> a, LEdge<TName,TLabel> b)
        {
            if (a.To == b.To && a.From == b.From && EqualityComparer<TLabel>.Default.Equals(a.Label, b.Label))
                return 0;
            else if (Comparer<TLabel>.Default.Compare(a.Label, b.Label) <= 0) // important to bfst
                return -1;
            return 1;
        }
    }
}
