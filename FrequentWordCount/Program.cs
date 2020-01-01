using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace FrequentWordCount
{
    using WordCountMap = Dictionary<string, int>;
    class Program
    {
        readonly static string Usage = 
@"Given a text, it scans all words appeared in the text and lists the top 50 words that appear most frequently.
usage: FrequenctWordCount.exe [filename]";
        
        static WordCountMap Add(WordCountMap dic, string k)
        {
            int count;
            dic[k] = dic.TryGetValue(k, out count) ? count + 1 : 1;
            return dic;
        }

        static string CountFromStrings(IEnumerable<string> strs)
        {
            var map = strs.SelectMany(l => l.Split
                    (new char[] {' ', '\n', ',', '.', '\'', '"', ':', ';', '?', '!', '(', ')', '，', '。', '、', '“', '”', '：', '；', '？', '！', '（', '）'}
                    , StringSplitOptions.RemoveEmptyEntries)) //.Where(w => w!= "")
                .Select(w => w.ToLower())
                .Aggregate(new WordCountMap(), Add);
            var res = new StringBuilder($"All words: {map.Count}\n");
            foreach (var p in map.OrderByDescending(p => p.Value).Take(50))
                res.AppendLine($"{p.Key}: {p.Value}");
            return res.ToString();
        }

        static void MainFromFile(string fn)
        {
            Console.WriteLine(CountFromStrings(File.ReadAllLines(fn)));
        }

        static void MainFromConsole()
        {
            var arry = new List<string>();
            string line;
            while ((line = Console.ReadLine()) != null && line != "")
                arry.Add(line);
            Console.WriteLine(CountFromStrings(arry));
        }

        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-help")
            {
                Console.WriteLine(Usage);
                return;
            }
            if (args.Length == 0) // open for input, then show result based on input
                MainFromConsole();
            else
                MainFromFile(args[0]);
        }
    }
}
