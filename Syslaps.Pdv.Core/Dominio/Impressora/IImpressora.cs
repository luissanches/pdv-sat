namespace Syslaps.Pdv.Core.Dominio.Impressora
{
    public interface IImpressora
    {
        void InicializarImpressora();
        
        bool AcionarGuilhotina();
        
        bool ImprimirTextoTag(string textoTag);
        
        bool ImprimirTexto(string textoTag);

        bool ImprimirQRCode(int dwRestauracao, int dwModulo, int dwTipo, int dwVersao, int dwModo, string pszCodigo);

        bool ImprimirCodigoBarra(string pszCodigo);

        bool FecharComunicacao();

        bool AbrirGaveta();
    }
}