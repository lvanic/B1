using B1.DataLayer.Data;
using B1.DataLayer.Models;
using ExcelLibrary.SpreadSheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace B1_2task.Controllers
{
    public class ExcelController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public ExcelController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var sheets = await _appDbContext.TurnoverSheets
                .Include(x => x.TurnoverLines)
                .ThenInclude(x => x.LineClass)
                .Include(x => x.TurnoverLines)
                .ThenInclude(x => x.InputBalance)
                .Include(x => x.TurnoverLines)
                .ThenInclude(x => x.Turnover)
                .Include(x => x.TurnoverLines)
                .ThenInclude(x => x.OutputBalance)
                .Include(x => x.TurnoverLines)
                .ToListAsync();
            return View(sheets);
        }
        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a file";
                return View("Index");
            }

            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                Workbook workbook = await LoadWorkbook(memoryStream);

                if (workbook != null && workbook.Worksheets.Count > 0)
                {
                    TurnoverSheetModel turnoverSheet = await ProcessWorkbook(workbook);
                    await _appDbContext.TurnoverSheets.AddAsync(turnoverSheet);
                    await _appDbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    Console.WriteLine("Workbook or worksheets not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return View("Index");
        }

        private async Task<Workbook> LoadWorkbook(MemoryStream memoryStream)
        {
            Workbook workbook;
            memoryStream.Position = 0;

            workbook = Workbook.Load(memoryStream);

            return workbook;
        }

        private async Task<TurnoverSheetModel> ProcessWorkbook(Workbook workbook)
        {
            TurnoverSheetModel sheet = new TurnoverSheetModel();
            Worksheet worksheet = workbook.Worksheets[0];

            PopulateSheetProperties(ref sheet, worksheet);
            ProcessTurnoverLines(ref sheet, worksheet);

            return sheet;
        }

        private void PopulateSheetProperties(ref TurnoverSheetModel sheet, Worksheet worksheet)
        {
            sheet.CompanyName = worksheet.Cells[0, 0].StringValue;
            sheet.TurnoverSheetName = worksheet.Cells[1, 0].StringValue;
            sheet.PeriodName = worksheet.Cells[2, 0].StringValue;
            sheet.TargetName = worksheet.Cells[3, 0].StringValue;
            sheet.CreationDate = worksheet.Cells[5, 0].DateTimeValue;
            sheet.EntityName = worksheet.Cells[5, 6].StringValue;
            sheet.TurnoverLines = new List<TurnoverLineModel>();
        }

        private void ProcessTurnoverLines(ref TurnoverSheetModel sheet, Worksheet worksheet)
        {
            int rowCount = worksheet.Cells.LastRowIndex + 1;
            TurnoverClassModel lineClass = new TurnoverClassModel();
            for (int row = 8; row < rowCount; row++)
            {
                if (worksheet.Cells[row, 0].StringValue.Contains("КЛАСС") && worksheet.Cells[row, 1].IsEmpty)
                {
                    lineClass = new TurnoverClassModel()
                    {
                        Name = worksheet.Cells[row, 0].StringValue
                    };
                    continue;
                }
                else if (worksheet.Cells[row, 0].StringValue == "БАЛАНС")
                {
                    lineClass = new TurnoverClassModel()
                    {
                        Name = "БАЛАНС"
                    };
                }

                BalanceModel inputBalance = new BalanceModel()
                {
                    Asset = Convert.ToDecimal(worksheet.Cells[row, 1].Value),
                    Liability = Convert.ToDecimal(worksheet.Cells[row, 2].Value)
                };

                TurnoverModel turnover = new TurnoverModel()
                {
                    Debit = Convert.ToDecimal(worksheet.Cells[row, 3].Value),
                    Credit = Convert.ToDecimal(worksheet.Cells[row, 4].Value)
                };

                BalanceModel outputBalance = new BalanceModel()
                {
                    Asset = Convert.ToDecimal(worksheet.Cells[row, 5].Value),
                    Liability = Convert.ToDecimal(worksheet.Cells[row, 6].Value)
                };

                TurnoverLineModel line = new TurnoverLineModel()
                {
                    AccountingId = worksheet.Cells[row, 0].StringValue,
                    LineClass = lineClass,
                    InputBalance = inputBalance,
                    Turnover = turnover,
                    OutputBalance = outputBalance,
                };

                sheet.TurnoverLines.Add(line);
            }
        }
    }
}
