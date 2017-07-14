namespace Syslaps.Pdv.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("VendaProduto")]
    public partial class VendaProduto
    {
        [Key]
        [StringLength(32)]
        public string CodigoVendaProduto { get; set; }

        [Required]
        [StringLength(32)]
        public string Produto_CodigoDeBarra { get; set; }

        [Required]
        [StringLength(40)]
        public string Venda_CodigoVenda { get; set; }

        public decimal ValorDoProduto { get; set; }

        public decimal ValorDoProdutoComDesconto { get; set; }

        public decimal ValorDoDesconto { get; set; }

        public decimal Quantidade { get; set; }

        public decimal ValorTotalVendaProduto { get; set; }

        public decimal ValorTotalDoDesconto { get; set; }

        [Required]
        [StringLength(4)]
        public string CodigoParaCupom { get; set; }

        [Required]
        [StringLength(250)]
        public string DescricaoProduto { get; set; }

        public virtual Produto Produto { get; set; }

        public virtual Venda Venda { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
