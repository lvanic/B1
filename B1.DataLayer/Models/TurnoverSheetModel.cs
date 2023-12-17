using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B1.DataLayer.Models
{
    public class TurnoverSheetModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string TurnoverSheetName { get; set; }
        public string PeriodName { get; set; }
        public string TargetName { get; set; }
        public string EntityName { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<TurnoverLineModel> TurnoverLines { get; set; }
    }
}
