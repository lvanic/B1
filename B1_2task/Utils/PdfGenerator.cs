using B1.DataLayer.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace B1_2task.Utils
{
    public class PdfGenerator
    {
        private readonly Transliteration _transliterator;
        private readonly Font _fontOptions;
        public PdfGenerator()
        {
            _transliterator = new Transliteration();
            _fontOptions = new Font()
            {
                Size = 10,
            };
        }
        public byte[] GeneratePdfBytes(TurnoverSheetModel sheet)
        {
            using MemoryStream memoryStream = new();

            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();
            document.Add(new Paragraph("Created by Yahor"));
            var table = GenerateTable(sheet);
            document.Add(table);
            document.Close();

            return memoryStream.ToArray();
        }

        private IElement GenerateTable(TurnoverSheetModel sheet)
        {
            PdfPTable table = new PdfPTable(7);
            table.WidthPercentage = 100;

            string[] headers = { "Б/сч", "Входящее сальдо (Актив)", "Входящее сальдо (Пассив)",
                             "Обороты (Дебет)", "Обороты (Кредит)", "Исходящее сальдо (Актив)",
                             "Исходящее сальдо (Пассив)" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(_transliterator.Transliterate(header), _fontOptions));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
            }

            string currentClass = null;
            foreach (var line in sheet.TurnoverLines)
            {
                if (currentClass == null || currentClass != line.LineClass.Name)
                {
                    currentClass = line.LineClass.Name;
                    PdfPCell classCell = new PdfPCell(new Phrase(_transliterator.Transliterate(currentClass), _fontOptions));
                    classCell.Colspan = 7;
                    classCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(classCell);
                }

                table.AddCell(new Phrase(_transliterator.Transliterate(line.AccountingId), _fontOptions));
                table.AddCell(new Phrase(line.InputBalance.Asset.ToString(), _fontOptions));
                table.AddCell(new Phrase(line.InputBalance.Liability.ToString(), _fontOptions));
                table.AddCell(new Phrase(line.Turnover.Debit.ToString(), _fontOptions));
                table.AddCell(new Phrase(line.Turnover.Credit.ToString(), _fontOptions));
                table.AddCell(new Phrase(line.OutputBalance.Asset.ToString(), _fontOptions));
                table.AddCell(new Phrase(line.OutputBalance.Liability.ToString(), _fontOptions));
            }
            return table;
        }
    }
}
