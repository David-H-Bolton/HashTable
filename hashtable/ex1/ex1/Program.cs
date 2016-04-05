using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Console;

namespace ex1
{
    internal class Program
    {
        private class ListedWord
        {
            public string Word { get; }
            public int Count { get; set; } = 1;

            public ListedWord(string newword)
            {
                Word = newword;
            }
        }

        private class WordList
        {
            private readonly Dictionary<string, ListedWord> _words = new Dictionary<string, ListedWord>();
            private static int MaxWords { get; set; }
            public static List<int> WordFrequencies { get; set; }
            private readonly char _separator;

            public WordList(char separator = ',')
            {
                _separator = separator;
                WordFrequencies = new List<int>();
                for (var i = 0; i < 30; i++)
                {
                    WordFrequencies.Add(0);
                }
            }

            public int Count => _words.Count;

            private void AddWord(string word)
            {
                if (!_words.ContainsKey(word))
                {
                    var lword = new ListedWord(word); // add new word to list
                    _words.Add(word, lword);
                    return;
                }
                _words[word].Count++;
            }

            internal void DumpWords(StreamWriter sw)
            {
                var index = 0;
                foreach (var word in _words.Values.OrderByDescending(x => x.Count).ToList())
                {
                    {
                        sw.Write(index);
                        sw.Write(',');
                        sw.Write(word.Count);
                        sw.Write(',');
                    }
                    sw.WriteLine(word.Word);
                    index++;
                }
            }

            // reads through string looking for alphanumerics
            internal void ScanLine(string line)
            {
                var wordCount = 0;

                line = line.ToLower();
                var parts = line.Split(_separator).ToList();

                foreach (var part in parts)
                {
                    if (part.Length == 0) continue;

                    var splitwords = part.Split(' ');
                    foreach (var sw in splitwords)
                    {
                        var sw2 = sw.Trim();
                        if (sw2 != "")
                        {
                            AddWord(sw2);
                            WordFrequencies[sw2.Length]++;
                            wordCount++;
                        }
                    }
                }
                if (wordCount > MaxWords)
                {
                    MaxWords = wordCount;
                }
            }
        }

        private static void Main(string[] args)
        {
            var lines = File.ReadAllLines(@"input.txt");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var words = new WordList(',');
            var total = 0;
            var lineCount = 0;
            foreach (var line in lines)
            {
                {
                    total += line.Length;
                    lineCount++;
                    words.ScanLine(line);
                }
            }
            stopWatch.Stop();
            WriteLine($"Execution time: {stopWatch.ElapsedMilliseconds/1000.0} secs");
            WriteLine($"Average length ={total/lineCount}");
            using (var sw = new StreamWriter(@"output.txt"))
            {
                words.DumpWords(sw);
                sw.Close();
            }
            if (words.Count > 0)
            {
                WriteLine($"{words.Count} Words copied from input.txt to output.txt");
            }
            var totalWords = 0;
            for (var i = 0; i < WordList.WordFrequencies.Count; i++)
            {
                if (WordList.WordFrequencies[i] > 0)
                {
                    WriteLine($"Count of {i} word = {WordList.WordFrequencies[i]}");
                    totalWords += WordList.WordFrequencies[i];
                }
            }
            WriteLine($"Total #Words ={totalWords}");

        }
    }
}
