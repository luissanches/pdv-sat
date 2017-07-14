namespace Syslaps.Pdv.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ConfiguracaoCategoriaProduto")]
    public partial class ConfiguracaoCategoriaProduto
    {
        [Key]
        [StringLength(60)]
        public string Categoria { get; set; }

        public bool TemProducao { get; set; }

        public bool DescontaInsumo { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
