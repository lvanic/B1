using System.Collections.Concurrent;
using System.Diagnostics;
using B1_1task.Utils;

namespace B1_1task.DataControl
{
    internal class DataMerger
    {
        private static int _removedLinesCount;
        private readonly BlockingCollection<string> _mergeLines;
        private readonly DisplayMessageDelegate DisplayMessage;
        internal DataMerger()
        {
            _removedLinesCount = 0;
            _mergeLines = new BlockingCollection<string>();
            DisplayMessage = Console.WriteLine;
        }
        internal void MergeFilesAndRemoveText()
        {
            string[] fileEntries = Directory.GetFiles(Consts.directoryPath, "*.txt");
            using StreamWriter writer = new(Consts.outputFilePath);
            var sw = Stopwatch.StartNew();
            Parallel.ForEach(fileEntries, WriteTextIntoFile);
            sw.Stop();

            DisplayMessage($"Time elapsed to merge and remove data - {sw.ElapsedMilliseconds}");

            _mergeLines.CompleteAdding();
            writer.Write(string.Join("\n", _mergeLines));
        }

        void WriteTextIntoFile(string filePath)
        {
            int removedLinesCountLocal = 0;
            Parallel.ForEach(File.ReadLines(filePath), line =>
            {
                if (!line.Contains(Consts.textToRemove))
                {
                    _mergeLines.Add(line);
                }
                else
                {
                    removedLinesCountLocal++;
                }
            });
            _removedLinesCount += removedLinesCountLocal;
        }
    }
}
