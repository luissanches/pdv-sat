namespace Syslaps.Pdv.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Parametro")]
    public partial class Parametro
    {
        [Key]
        [StringLength(120)]
        public string Nome { get; set; }

        [Required]
        [StringLength(1000)]
        public string Valor { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
