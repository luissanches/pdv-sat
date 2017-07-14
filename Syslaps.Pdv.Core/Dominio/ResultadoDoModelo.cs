namespace Syslaps.Pdv.Core.Dominio
{
    public class ResultadoDoModelo
    {
        public ResultadoDoModelo(EnumStatusDoResultado status = EnumStatusDoResultado.MensagemDeSucesso)
        {
            this.StatusDoResultado = status;
        }

        public string Messagem { get; set; }

        public EnumStatusDoResultado StatusDoResultado { get; set; }
    }
}