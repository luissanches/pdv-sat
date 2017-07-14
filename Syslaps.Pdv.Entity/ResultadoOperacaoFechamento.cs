namespace Syslaps.Pdv.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ResultadoOperacaoFechamento")]
    public partial class ResultadoOperacaoFechamento
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(32)]
        public string CodigoResultado { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(32)]
        public string OperacaoCaixa_CodigoOperacaoCaixa { get; set; }

        public decimal ValorAbertura { get; set; }

        public decimal ValorContabilizadoNoFechamento { get; set; }

        public decimal ValorTotalSangria { get; set; }

        public decimal ValorTotalReforco { get; set; }

        public decimal ValorTotalPagamentoDinheiro { get; set; }

        public decimal ValorTotalPagamentoDebito { get; set; }

        public decimal ValorTotalPagamentoCredito { get; set; }

        public decimal ValorTotalPagamentoTicket { get; set; }

        public decimal ValorTotalPagamento { get; set; }

        public decimal ValorTotalEstimadoEmEspecie { get; set; }

        public decimal DiferencaNoCaixa { get; set; }

        public decimal ValorTotalDescontoVenda { get; set; }

        public decimal ValorRecebimentoDebito { get; set; }

        public decimal ValorRecebimentoCretito { get; set; }

        public decimal ValorRecebimentoTicket { get; set; }

        public virtual OperacaoCaixa OperacaoCaixa { get; set; }

        [Required]
        public virtual bool Sincronizado { get; set; }
    }
}
