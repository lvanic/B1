using B1.DataLayer.Data;
using B1.DataLayer.Models;
using B1_2task.Utils;
using ExcelLibrary.SpreadSheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace B1_2task.Controllers
{
    public class ExcelController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly ExcelParser _excelParser;
        public ExcelController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _excelParser = new ExcelParser();
        }
        [HttpGet]
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
        [HttpGet("Excel/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var sheet = await _appDbContext.TurnoverSheets
                .Include(x => x.TurnoverLines)
                .ThenInclude(x => x.LineClass)
                .Include(x => x.TurnoverLines)
                .ThenInclude(x => x.InputBalance)
                .Include(x => x.TurnoverLines)
                .ThenInclude(x => x.Turnover)
                .Include(x => x.TurnoverLines)
                .ThenInclude(x => x.OutputBalance)
                .Include(x => x.TurnoverLines)
                .FirstOrDefaultAsync(x => x.Id == id);

            return View("OneSheet", sheet);
        }
        [HttpGet("Excel/Upload")]
        public async Task<IActionResult> Upload()
        {
            return View("Upload");
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a file";
                return View();
            }

            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                Workbook workbook = await _excelParser.LoadWorkbook(memoryStream);

                if (workbook != null && workbook.Worksheets.Count > 0)
                {
                    TurnoverSheetModel turnoverSheet = await _excelParser.ProcessWorkbook(workbook);
                    await _appDbContext.TurnoverSheets.AddAsync(turnoverSheet);
                    await _appDbContext.SaveChangesAsync();
                    return Redirect("/Excel");
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

            return View("/Excel");
        }
    }
}
