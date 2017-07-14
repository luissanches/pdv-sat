using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Core.Dominio.Impressora;

namespace Syslaps.Pdv.Infra.Impressora.T20
{
    public class Impressora : IImpressora
    {
        public Impressora()
        {
            InicializarImpressora();
        }

        public EnumSituacaoDaImpressora Status { get; private set; }

        public void InicializarImpressora()
        {
            Status = InterfaceEpsonT20.IniciaPorta("USB") == 0? EnumSituacaoDaImpressora.NoOk : EnumSituacaoDaImpressora.Ok;
        }

        public bool AcionarGuilhotina()
        {
            if(Status== EnumSituacaoDaImpressora.NoOk) InicializarImpressora();
            return InterfaceEpsonT20.AcionaGuilhotina(0) == 1;
        }

        public bool ImprimirTextoTag(string textoTag)
        {
            if (Status == EnumSituacaoDaImpressora.NoOk) InicializarImpressora();
            return InterfaceEpsonT20.ImprimeTextoTag(textoTag) == 1;
        }

        public bool ImprimirTexto(string textoTag)
        {
            if (Status == EnumSituacaoDaImpressora.NoOk) InicializarImpressora();
            return InterfaceEpsonT20.ImprimeTexto(textoTag) == 1;
        }

        public bool ImprimirQRCode(int dwRestauracao, int dwModulo, int dwTipo, int dwVersao, int dwModo, string pszCodigo)
        {
            if (Status == EnumSituacaoDaImpressora.NoOk) InicializarImpressora();
            return InterfaceEpsonT20.ImprimeCodigoQRCODE(dwRestauracao, dwModulo, dwTipo, dwVersao, dwModo, pszCodigo) == 1;
        }

        public bool ImprimirCodigoBarra(string pszCodigo)
        {
            if (Status == EnumSituacaoDaImpressora.NoOk) InicializarImpressora();
            InterfaceEpsonT20.ConfiguraCodigoBarras(80, 0, 0, 0, 5);
            return InterfaceEpsonT20.ImprimeCodigoBarrasCODE128(pszCodigo) == 1;
        }

        public bool FecharComunicacao()
        {
            return InterfaceEpsonT20.FechaPorta() == 1;
        }

        public bool AbrirGaveta()
        {
            if (Status == EnumSituacaoDaImpressora.NoOk) InicializarImpressora();
            return InterfaceEpsonT20.AcionaGaveta() == 1;
        }
    }
}