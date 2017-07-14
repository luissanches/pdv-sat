namespace Syslaps.Pdv.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Caixa")]
    public partial class Caixa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Caixa()
        {
            OperacaoCaixas = new HashSet<OperacaoCaixa>();
        }

        [Key]
        [StringLength(32)]
        public string CodigoCaixa { get; set; }

        [Required]
        [StringLength(80)]
        public string Nome { get; set; }

        [Required]
        [StringLength(60)]
        public string Machine { get; set; }

        [Required]
        [StringLength(20)]
        public string IP { get; set; }

        [Required]
        [StringLength(15)]
        public string Situacao { get; set; }

        [StringLength(32)]
        public string CodigoOperacaoDeAbertura { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OperacaoCaixa> OperacaoCaixas { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
