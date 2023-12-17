using B1_1task.DataControl;

var dataGenerator = new DataGenerator();
var dataMerger = new DataMerger();
var dataUploader = new DataUploader();

dataGenerator.GenerateDataToDirectory();
dataMerger.MergeFilesAndRemoveText();
dataUploader.UploadData();

Console.ReadKey();

delegate void DisplayMessageDelegate(string message);
