using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B1.DataLayer.Models
{
    public class BalanceModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public decimal Asset { get; set; }
        public decimal Liability { get; set; }

        [InverseProperty("InputBalance")]
        public ICollection<TurnoverLineModel> InputLines { get; set; }

        [InverseProperty("OutputBalance")]
        public ICollection<TurnoverLineModel> OutputLines { get; set; }

    }
}
