using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using B1_1task.Utils;

namespace B1_1task.DataControl
{
    internal class DataGenerator
    {
        private readonly Random _random;
        private readonly DisplayMessageDelegate DisplayMessage;
        internal DataGenerator()
        {
            _random = new Random();
            DisplayMessage = Console.WriteLine;
        }
        internal void GenerateDataToDirectory()
        {
            var sw = Stopwatch.StartNew();
            Parallel.For(0, Consts.numberOfFiles, InsertIntoFiles);
            sw.Stop();

            DisplayMessage($"Time elapsed to generate data - {sw.ElapsedMilliseconds}");
        }
        void InsertIntoFiles(int fileIndex)
        {
            string filePath = Path.Combine(Consts.directoryPath, $"file_{fileIndex + 1}.txt");
            using StreamWriter writer = new(filePath);

            for (int j = 0; j < Consts.numberOfLines; j++)
            {
                string date = GenerateRandomDate();
                string latinChars = GenerateRandomString(10);
                string russianChars = GenerateRandomRussianString(10);
                int randomNumber = GenerateRandomEvenNumber(1, 100000000);
                double randomFloat = GenerateRandomFloat(1, 20);

                string line = $"{date}||{latinChars}||{russianChars}||{randomNumber}||{randomFloat:F8}||";
                writer.WriteLine(line);
            }
        }

        string GenerateRandomDate()
        {
            DateTime start = DateTime.Now.AddYears(-5);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(_random.Next(range)).ToString("dd.MM.yyyy");
        }

        string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        string GenerateRandomRussianString(int length)
        {
            const string chars = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        int GenerateRandomEvenNumber(int min, int max)
        {
            int number;
            do
            {
                number = _random.Next(min, max);
            } while (number % 2 != 0);
            return number;
        }

        double GenerateRandomFloat(double min, double max)
        {
            return min + _random.NextDouble() * (max - min);
        }
    }
}
