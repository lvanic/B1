using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B1.DataLayer.Models
{
    public class TurnoverLineModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string AccountingId { get; set; }
        public TurnoverClassModel LineClass { get; set; }
        public BalanceModel InputBalance { get; set; }
        public BalanceModel OutputBalance { get; set; }
        public TurnoverModel Turnover { get; set; }
    }
}
