using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GraphTraversal
{
    public class MakeSTree<TName, TLabel>
    {
        private readonly Graph<TName,TLabel> _bfst;
        private readonly Vertex<TName,TLabel> _root;

        public Graph<TName,TLabel> bfst { get => _bfst; }
        public Vertex<TName,TLabel> root { get => _root; }

        // makes a bfst rooted at the given vertex. the resultant graph HAS every edge reversed
        // ensures every vertex has exactly one parent (edge) (except root)
        // a vertex's "edge" is its parent, and the distance from it to the root
        // zero: the default weight (distance from a vertex to itself)
        public MakeSTree(TLabel zero, Vertex<TName,TLabel> root)
        {
            var res = new Graph<TName,TLabel>();
            var que = new SortedSet<LEdge<TName,TLabel>>(new LEdgeComparer<TName,TLabel>());
            que.Add(new LEdge<TName,TLabel>(root, null, zero));

            while (que.Count != 0)
            {
                var neighbour = que.Min;
                var neighbourNode = neighbour.To;
                que.Remove(neighbour);
                // if not already reachable, add it
                if (!res.Exists(neighbourNode.Name))
                {
                    TLabel dist = neighbour.Label;
                    res.AddSoleVertex(neighbourNode.Name);
                    if (neighbour.From != null)
                        res.Link(neighbourNode.Name, neighbour.From.Name, dist);
                    foreach (var it in neighbourNode.Edges)
                        que.Add(new LEdge<TName, TLabel>(it.To, neighbourNode, (dynamic)dist + (dynamic)it.Label));
                }
            }

            _root = root;
            _bfst = res;
        }

        public string ShowAllConnectedVertices()
        {
            return "ALL CONNECTED VERTICES FROM\n" + $"{_root.Name}: " + string.Join(", ", _bfst.Vertices.Select(x => x.Name)) + '\n';
        }

        public string ShowPath(TName to)
        {
            if (!_bfst.Exists(to))
                return $"PATH FROM {_root.Name} TO {to} DOES NOT EXIST.\n";
            
            if (EqualityComparer<TName>.Default.Equals(to, _root.Name))
                return $"PATH FROM {to} TO {to} IS IMMEDIATE.\n";
            
            var path = new Stack<TName>();
            var curr = _bfst.Reference(to);
            while (curr.Edges.Count != 0)
            {
                path.Push(curr.Name);
                curr = curr.Edges[0].To;
            }

            var res = new StringBuilder($"PATH FROM {_root.Name} TO {to}:\n");
            res.Append(_root.Name); // root was not pushed
            while (path.Count != 0)
                res.Append($" -> {path.Pop()}");
            res.AppendLine($"\nCOST {_bfst.Reference(to).Edges[0].Label}.");
            return res.ToString();
        }

        public string ShowPaths()
        {
            var res = new StringBuilder();
            foreach (var v in _bfst.Vertices)
            {
                res.Append(ShowPath(v.Name));
            }
            return res.ToString();
        }
    }
}
