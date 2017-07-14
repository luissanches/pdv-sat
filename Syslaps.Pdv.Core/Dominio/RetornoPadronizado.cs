namespace Syslaps.Pdv.Core.Dominio
{
    public class RetornoPadronizado<TConteudoRetornado>
    {
        public TConteudoRetornado ConteudoRetornado { get; set; }

        public string Mensagem { get; set; }

        public StatusRetornoPadronizado StatusRetornoPadronizado { get; set; }

        public ResultadoDoModelo ResultadoDoModelo { get; set; }

        public RetornoPadronizado()
        {
            ConteudoRetornado = default(TConteudoRetornado);
            StatusRetornoPadronizado = StatusRetornoPadronizado.Ok;
        }

        public static RetornoPadronizado<T> Criar<T>()
        {
            return new RetornoPadronizado<T>();
        }

        public static RetornoPadronizado<TConteudoRetornado> Criar (ResultadoDoModelo resultadoDoModelo)
        {
            return new RetornoPadronizado<TConteudoRetornado>() { ResultadoDoModelo = resultadoDoModelo };
        }
    }
}
