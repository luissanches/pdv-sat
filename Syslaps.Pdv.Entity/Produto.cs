namespace Syslaps.Pdv.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Produto")]
    public partial class Produto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Produto()
        {
            ComandaProdutoes = new HashSet<ComandaProduto>();
            ProdutoProducaos = new HashSet<ProdutoProducao>();
            VendaProdutoes = new HashSet<VendaProduto>();
        }

        [Key]
        [StringLength(32)]
        public string CodigoDeBarra { get; set; }

        [Required]
        [StringLength(15)]
        public string TipoProduto { get; set; }

        [Required]
        [StringLength(60)]
        public string Modelo { get; set; }

        [Required]
        [StringLength(250)]
        public string Descricao { get; set; }

        [Editable(false)]
        public string DescricaoDisplay { get; set; }

        [Required]
        [StringLength(200)]
        public string DescricaoBusca { get; set; }

        public decimal PrecoCusto { get; set; }

        public decimal PrecoVenda { get; set; }

        public decimal PrecoVenda2 { get; set; }

        [Required]
        [StringLength(15)]
        public string TipoUnidade { get; set; }

        public long? EstoqueMinimo { get; set; }

        [Required]
        [StringLength(60)]
        public string Marca { get; set; }

        [StringLength(80)]
        public string TipoFiscal { get; set; }

        [StringLength(10)]
        public string CodigoNCM { get; set; }

        [StringLength(15)]
        public string DigitoCST { get; set; }

        [StringLength(15)]
        public string CEST { get; set; }

        [Required]
        [StringLength(60)]
        public string Categoria { get; set; }

        [Required]
        [StringLength(60)]
        public string SubCategoria { get; set; }

        public bool Ativo { get; set; }

        public bool ExibirNoPdv { get; set; }

        public bool ControlarEstoque { get; set; }

        public bool DescontarInsumoNaVenda { get; set; }

        public bool TemProducao { get; set; }

        [Required]
        [StringLength(4)]
        public string CodigoParaCupom { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComandaProduto> ComandaProdutoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProdutoProducao> ProdutoProducaos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VendaProduto> VendaProdutoes { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }

        [Required]
        public virtual decimal QtdeEstoque { get; set; }

    }
}
