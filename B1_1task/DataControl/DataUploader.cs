using B1.DataLayer.Models;
using B1.DataLayer.Repository;
using B1_1task.Utils;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace B1_1task.DataControl
{
    internal class DataUploader
    {
        private readonly GenericRepository<DataModel> _repository;
        private readonly object _locker = new object();
        public DataUploader()
        {
            var connectionString = Consts.connectionString;
            _repository = new GenericRepository<DataModel>(connectionString);
        }
        public void UploadData()
        {
            string filePath = Consts.outputFilePath;
            const int LINE_PORTION = 100000;
            try
            {
                var lines = File.ReadAllLines(filePath);
                int totalLines = lines.Length;

                int importedCount = 0;
                BlockingCollection<DataModel> blocks = new BlockingCollection<DataModel>();

                Parallel.ForEach(lines, line =>
                {
                    string[] fields = line.Split("||");

                    if (fields.Length == 6)
                    {
                        var dataModel = new DataModel
                        {
                            Date = DateTime.Parse(fields[0]),
                            Latin = fields[1],
                            Cyrillic = fields[2],
                            Integer = Convert.ToUInt32(fields[3]),
                            Double = Convert.ToDouble(fields[4])
                        };

                        if (ValidateDataModel(dataModel))
                        {
                            blocks.Add(dataModel);
                            Interlocked.Increment(ref importedCount);

                            lock (_locker)
                            {
                                if (importedCount % LINE_PORTION == 0)
                                {
                                    _repository.AddRange(blocks.ToHashSet());
                                    blocks = new BlockingCollection<DataModel>();
                                    Console.WriteLine($"Imported: {importedCount} / Left: {totalLines - importedCount}");
                                }
                            }
                        }
                    }
                });

                Console.WriteLine("Import process complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during import: {ex.Message}");
            }
        }
        private bool ValidateDataModel(DataModel dataModel)
        {
            return Regex.IsMatch(dataModel.Latin, @"^[a-zA-Z]{10}$") &&
                   Regex.IsMatch(dataModel.Cyrillic, @"^[а-яА-ЯёЁ]{10}$") &&
                   dataModel.Integer % 2 == 0 &&
                   dataModel.Integer >= 2 && dataModel.Integer <= 100000000 &&
                   dataModel.Double >= 1 && dataModel.Double <= 20 &&
                   dataModel.Date >= new DateTime(2018, 1, 1) && dataModel.Date <= new DateTime(2022, 12, 31);
        }
    }
}
