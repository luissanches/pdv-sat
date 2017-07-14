namespace Syslaps.Pdv.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Comanda")]
    public partial class Comanda
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comanda()
        {
            ComandaProdutoes = new HashSet<ComandaProduto>();
        }

        [Key]
        public string CodigoComanda { get; set; }

        [Required]
        [StringLength(20)]
        public string Situacao { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComandaProduto> ComandaProdutoes { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
