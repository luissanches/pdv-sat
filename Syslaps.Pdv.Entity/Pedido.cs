namespace Syslaps.Pdv.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Pedido")]
    public partial class Pedido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pedido()
        {
            PedidoProduto = new HashSet<PedidoProduto>();
        }

        [Key]
        [StringLength(32)]
        public string CodigoPedido { get; set; }

        public DateTime DataPedido { get; set; }

        public DateTime DataEntrega { get; set; }

        [Required]
        [StringLength(80)]
        public string NomeCliente { get; set; }

        [Required]
        [StringLength(15)]
        public string Situacao { get; set; }

        [StringLength(20)]
        public string Telefone { get; set; }

        [StringLength(500)]
        public string Observacao { get; set; }

        [Required]
        public decimal Valor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PedidoProduto> PedidoProduto { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
