using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B1.DataLayer.Models
{
    public class DataModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Range(typeof(DateTime), "1/1/2018", "12/31/2022")]
        public DateTime Date { get; set; }

        [RegularExpression(@"^[a-zA-Z]{10}$")]
        public string Latin { get; set; }

        [RegularExpression(@"^[а-яА-ЯёЁ]{10}$")]
        public string Cyrillic { get; set; }

        [Range(2, 100000000, ErrorMessage = "Число должно быть четным и находиться в диапазоне от 2 до 100,000,000")]
        public uint Integer { get; set; }

        [Range(1, 20)]
        public double Double { get; set; }
    }
}
