namespace Syslaps.Pdv.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CupomFiscalSat")]
    public partial class CupomFiscalSat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CupomFiscalSat()
        {
        }

        [Key]
        [StringLength(32)]
        public string CodigoVenda { get; set; }

        public string CpfCnpj { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorCode2 { get; set; }

        public string ErrorMessage { get; set; }

        public string InvoiceKey { get; set; }

        public string QrCodeSignature { get; set; }

        public string SessionCode { get; set; }

        public string TimeStamp { get; set; }

        public string Total { get; set; }

        public string Xml { get; set; }

        public string XmlEnvio { get; set; }

        public DateTime DataOperacao { get; set; }
        public string CodigoSat { get; set; }

    }
}
