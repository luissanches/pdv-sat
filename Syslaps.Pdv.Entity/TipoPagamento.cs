namespace Syslaps.Pdv.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TipoPagamento")]
    public partial class TipoPagamento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TipoPagamento()
        {
            VendaPagamentoes = new HashSet<VendaPagamento>();
        }

        [Key]
        [StringLength(32)]
        public string CodigoTipoPagamento { get; set; }

        [Required]
        [StringLength(30)]
        public string Nome { get; set; }

        public decimal PercentualDesconto { get; set; }

        public long DiasParaPagamento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VendaPagamento> VendaPagamentoes { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
