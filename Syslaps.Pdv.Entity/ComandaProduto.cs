namespace Syslaps.Pdv.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ComandaProduto")]
    public partial class ComandaProduto
    {
        [Key]
        [StringLength(32)]
        public string CodigoComandaProduto { get; set; }

        public string Comanda_CodigoComanda { get; set; }

        [Required]
        [StringLength(32)]
        public string Produto_CodigoDeBarra { get; set; }

        [Required]
        public decimal Quantidade { get; set; }

        public virtual Comanda Comanda { get; set; }

        public virtual Produto Produto { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
