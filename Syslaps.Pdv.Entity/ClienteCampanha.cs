namespace Syslaps.Pdv.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ClienteCampanha")]
    public partial class ClienteCampanha
    {
        [Key]
        [StringLength(32)]
        public string CodigoClienteCampanha { get; set; }

        [Required]
        [StringLength(60)]
        public string NomeCampanha { get; set; }

        [Required]
        [StringLength(20)]
        public string CpfCnpj { get; set; }

        [Required]
        [StringLength(120)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefone { get; set; }

        [Required]
        [StringLength(120)]
        public string NomeCliente { get; set; }

        public DateTime DataCadastro { get; set; }

        [StringLength(500)]
        public string Observacao { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
