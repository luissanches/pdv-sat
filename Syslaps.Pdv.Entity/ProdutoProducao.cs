namespace Syslaps.Pdv.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProdutoProducao")]
    public partial class ProdutoProducao
    {
        [Key]
        [StringLength(32)]
        public string CodigoProdutoProducao { get; set; }

        [Required]
        [StringLength(32)]
        public string Produto_CodigoDeBarra { get; set; }

        public DateTime DataProducao { get; set; }

        public long QuantidadeProduzida { get; set; }

        public long QuantidadeDescartadaInteira { get; set; }

        public long QuantidadeDescartadaParcial { get; set; }

        public virtual Produto Produto { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
