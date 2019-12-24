using System;
using System.Linq;
using System.Text;

namespace GraphTraversal
{
    using SGraph = Graph<string, int>;

    class GraphTraverse
    {
        public SGraph theGraph = new SGraph();

        public void ReadSGraphFromFile(string fn)
        {
            string line;
            using (var file = new System.IO.StreamReader(fn))
            {
                while ((line = file.ReadLine()) != null)
                {
                    var link = line.Split(' ').Where(x => x != "").ToArray();
                    // a vertex not going anywhere
                    if (link.Length == 1)
                        theGraph.AddSoleVertex(link[0]);
                    // a vertex follwed by a target and a distance
                    if (link.Length == 3)
                        theGraph.Link(link[0], link[1], Convert.ToInt32(link[2]));
                }
            }
        }

        public string showAllVertices()
        {
            return "ALL VERTICES:\n" + String.Join(", ", theGraph.Vertices.Select(x => x.Name)) + "\n";
        }

        public string showAllEdges(SGraph g)
        {
            var res = new StringBuilder("ALL VERTICES FOLLOWED BY OUTGOING EDGES:\n");
            foreach (var v in g.Vertices)
            {
                res.Append(v.Name);
                res.Append(": ");
                res.AppendLine(String.Join(", ", from e in v.Edges select $"{e.To.Name}({e.Label})"));
            }
            return res.ToString();
        }

        public string showAllEdges()
        {
            return showAllEdges(theGraph);
        }

        static void Main(string[] args)
        {
            if (args.Length != 1 || !System.IO.File.Exists(@args[0]))
            {
                Console.WriteLine("Please provide a valid file path.");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Gray;

            var program = new GraphTraverse();
            MakeSTree<string,int> bfst = null;
            program.ReadSGraphFromFile(@args[0]);
            Console.WriteLine("\nBuilding graph...\n");

            PrintHelp();
            while (true)
            {
                Console.Write("INPUT: ");
                var usrinput = Console.ReadLine();
                string[] ags;
                switch (usrinput[0])
                {
                case 'v':
                    Console.WriteLine(program.showAllVertices());
                    break;
                case 'e':
                    Console.WriteLine(program.showAllEdges());
                    break;
                case 'h':
                    PrintHelp();
                    break;
                case 'q':
                    return;
                case 't': 
                    ags = usrinput.Split(" ");
                    if (ags.Length == 1 || !program.theGraph.Exists(ags[1]))
                    {
                        Console.WriteLine("VERTEX NAME IS NOT VALID.");
                        break;
                    }
                    Console.WriteLine($"WE ARE BUILDING THE SEARCH TREE AT ROOT {ags[1]}...");
                    bfst = new MakeSTree<string, int>(0, program.theGraph.Reference(ags[1]));
                    break;
                case 'r':
                    if (bfst != null) Console.WriteLine(bfst.ShowAllConnectedVertices());
                    break;
                case 'p':
                    if (bfst == null) break;
                    ags = usrinput.Split(" ");
                    if (ags.Length == 1) Console.WriteLine(bfst.ShowPaths());
                    else if (ags.Length == 2) Console.WriteLine(bfst.ShowPath(ags[1]));
                    break;
                }
            }
        }

        private const string Msg = @"OPTIONS:
     v     -  show all vertices of the graph
     e     -  show all edges of the graph
t [vertex] -  create a search tree rooted at this vertex
     r     -  list all reachable vertices from the root set above
p [vertex] -  print the path from the root to this vertex
     p     -  list all paths to those reachable vertices
     h     -  see this message again
     b     -  go back
";

        private static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
