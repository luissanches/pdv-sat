namespace Syslaps.Pdv.Entity
{
    using Syslaps.Pdv.Entity.SAT;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Venda")]
    public partial class Venda
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Venda()
        {
            VendaPagamentoes = new HashSet<VendaPagamento>();
            VendaProdutoes = new HashSet<VendaProduto>();
        }

        [Key]
        [StringLength(40)]
        public string CodigoVenda { get; set; }

        [Required]
        [StringLength(32)]
        public string OperacaoCaixa_CodigoOperacaoCaixa { get; set; }

        [Required]
        [StringLength(32)]
        public string Usuario_CodigoUsuario { get; set; }

        public decimal ValorTotalVenda { get; set; }

        public decimal ValorTotalDescontoVenda { get; set; }

        public decimal ValorTotalRecebimento { get; set; }

        public DateTime DataRecebimento { get; set; }

        public DateTime DataVenda { get; set; }

        [StringLength(20)]
        public string CpfCnpjCliente { get; set; }

        [Editable(false)]
        public string TipoDocumento { get; set; }

        [StringLength(60)]
        public string NomeCliente { get; set; }

        public decimal ValorTroco { get; set; }

        public virtual OperacaoCaixa OperacaoCaixa { get; set; }

        public virtual Usuario Usuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VendaPagamento> VendaPagamentoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VendaProduto> VendaProdutoes { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }

        [Required]
        public int CFOP { get; set; }

        [Editable(false)]
        public SendResponse SatResponse { get; set; }

        [Editable(false)]
        public bool CupomFiscalImpresso { get; set; }
    }
}
