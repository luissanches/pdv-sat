namespace Syslaps.Pdv.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OperacaoCaixa")]
    public partial class OperacaoCaixa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OperacaoCaixa()
        {
            ResultadoOperacaoFechamentoes = new HashSet<ResultadoOperacaoFechamento>();
            Vendas = new HashSet<Venda>();
        }

        [Key]
        [StringLength(32)]
        public string CodigoOperacaoCaixa { get; set; } 

        [Required]
        [StringLength(32)]
        public string Usuario_CodigoUsuario { get; set; }

        [Required]
        [StringLength(32)]
        public string Caixa_CodigoCaixa { get; set; }

        public DateTime DataOperacao { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoOperacao { get; set; }

        public decimal ValorOperacao { get; set; }

        [StringLength(32)]
        public string CodigoOperacaoCaixaAbertura { get; set; }

        public virtual Caixa Caixa { get; set; }

        public virtual Usuario Usuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultadoOperacaoFechamento> ResultadoOperacaoFechamentoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Venda> Vendas { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
