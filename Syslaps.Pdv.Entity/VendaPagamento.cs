namespace Syslaps.Pdv.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("VendaPagamento")]
    public partial class VendaPagamento
    {
        [Key]
        [StringLength(32)]
        public string CodigoVendaPagamento { get; set; }

        [Required]
        [StringLength(32)]
        public string TipoPagamento_CodigoTipoPagamento { get; set; }

        [Required]
        [StringLength(40)]
        public string Venda_CodigoVenda { get; set; }

        public decimal ValorPagamento { get; set; }

        public virtual TipoPagamento TipoPagamento { get; set; }

        public virtual Venda Venda { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
