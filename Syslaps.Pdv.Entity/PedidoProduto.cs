
namespace Syslaps.Pdv.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PedidoProduto")]
    public partial class PedidoProduto
    {
        public PedidoProduto()
        {
            IsVisible = true;
        }

        [Key]
        [StringLength(32)]
        public string CodigoPedidoProduto { get; set; }

        [Required]
        [StringLength(32)]
        public string Produto_CodigoDeBarra { get; set; }

        [Required]
        [StringLength(32)]
        public string Pedido_CodigoPedido { get; set; }

        public long Quantidade { get; set; }

        public virtual Pedido Pedido { get; set; }

        public virtual Produto Produto { get; set; }

        [Editable(false)]
        public virtual bool IsVisible { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
