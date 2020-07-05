using System.Collections.Generic;

namespace GraphTraversal
{
    public class Graph<TName, TLabel>
    {
        private List<Vertex<TName,TLabel>> _Vertices = new List<Vertex<TName, TLabel>>();

        public List<Vertex<TName,TLabel>> Vertices { get => _Vertices; }

        public Graph() 
        {
        }

        public Vertex<TName,TLabel> Reference(TName name)
        {
            return _Vertices.Find(x => EqualityComparer<TName>.Default.Equals(name, x.Name));
        }

        public bool Exists(TName name)
        {
            return _Vertices.Exists(x => EqualityComparer<TName>.Default.Equals(name, x.Name));
        }

        public void AddSoleVertex(TName name)
        {
            _Vertices.Add(new Vertex<TName, TLabel>(name));
        }

        public void Link(TName from, TName to, TLabel label)
        {
            Vertex<TName,TLabel> start, end;
            start = Reference(from);
            if (start == null)
            {
                start = new Vertex<TName, TLabel>(from);
                _Vertices.Add(start);
            }
            end = Reference(to);
            if (end == null)
            {
                end = new Vertex<TName, TLabel>(to);
                _Vertices.Add(end);
            }
            start.addEdge(end, label);
        }
    }
}
