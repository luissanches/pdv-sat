using System.Runtime.InteropServices;

namespace Syslaps.Pdv.Infra.Impressora.T20
{
	public class InterfaceEpsonT20
	{
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ConfiguraTaxaSerial(int dwTaxa);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int IniciaPorta(string pszPorta);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int FechaPorta();
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeTexto(string pszTexto);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeTextoTag(string pszTexto);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int FormataTX(string pszTexto, int dwTipoLetra, int dwItalico, int dwSublinhado, int dwExpandido, int dwEnfatizado);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int AcionaGuilhotina(int dwTipoCorte);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ComandoTX(string pszComando, int dwTamanho);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int Le_Status();
		[DllImport("InterfaceEpsonNF.dll")]public static extern int Le_Status_Gaveta();
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ConfiguraCodigoBarras(int dwAltura, int dwLargura, int dwHRI, int dwFonte, int dwMargem);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasCODABAR(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasCODE128(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasCODE39(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasCODE93(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasEAN13(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasEAN8(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasITF(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasUPCA(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasUPCE(string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoBarrasPDF417(int dwCorrecao, int dwAltura, int dwLargura, int dwColunas, string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCodigoQRCODE(int dwRestauracao, int dwModulo, int dwTipo, int dwVersao, int dwModo, string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int GerarQRCodeArquivo(string pszFileName, string pszDados);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeBmpEspecial(string pszFileName, int dwX, int dwY, int dwAngulo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int Habilita_Log(int dwEstado, string pszCaminho);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeCheque(string szIndice, string szValor, string szData, string szPara, string szCidade, string szAdicional);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int ImprimeAutenticacao(string pszPosX, string pszPosY, string pszLinhaTexto);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int LeMICR([MarshalAs(UnmanagedType.VBByRefStr)] ref string pszCodigo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int LeModelo([MarshalAs(UnmanagedType.VBByRefStr)] ref string pszModelo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int Le_Status_Slip([MarshalAs(UnmanagedType.VBByRefStr)] ref string pszFlags);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int AcionaGaveta();
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Imprimir(string szXML, string szTipo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_SAT_Imprimir(string szXML, string szTipo);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_SAT_Imprimir_Cancelamento(string szXML, string szQRCodeVenda, string szTeste);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Abrir(string szCPF, string szNome, string szEndereco, string szNumero, string szBairro, string szCodMunicipio, string szMunicipio, string szUF, string szCep);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_AbrirEX(string szCPF, string szNome, string szEndereco, string szNumero, string szBairro, string szCodMunicipio, string szMunicipio, string szUF, string szCep, string szNum, string szSerie);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Vender_Item(string szCodigo, string szDescricao, string szQuantidade, string szCasasDecimaisQuantidade, string szUnidadeDeMedida, string szPrecoUnidade, string szCasasDecimaisPreco, string szAliquotas);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Vender_ItemEX(string szCodigo, string szDescricao, string szQuantidade, string szCasasDecimaisQuantidade, string szUnidadeDeMedida, string szPrecoUnidade, string szCasasDecimaisPreco, string szAliquotas, string szNCM, string szCST, string szPIS, string szCOFINS, string szCFOP);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Desconto_Acrescimo_Item(string szNumeroItem, string szOperacao, string szTipo, string szValor, string szCasasDecimaisValor);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Cancelar_Item(string szNumeroItem);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Cancelar_Item_Parcial(string szNumeroItem, string szQuantidade, string szCasasDecimaisQuantidade);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Cancelar_Desconto_Acrescimo_Item(string szNumeroItem, string szOperacao);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Pagamento(string szFormaPagamento, string szValor, string szCasasDecimaisValor);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Estorno_Pagamento(string szEstornada, string szEfetivada, string szValor, string szCasasDecimaisValor);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Dados_Consumidor(string szCPF, string szNome, string szEndereco, string szNumero, string szBairro, string szCodMunicipio, string szMunicipio, string szUF, string szCep, string szEmail);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Dados_Lei_Imposto(string szValorMunicipal, string szPercMun, string szValorEstadual, string szPercEst, string szValorFederal, string szPercFed, string szUF, string szTabela);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Dados_Lei_ImpostoEX(string szMensagem);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Fechar(string szImprime);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Reimprimir();
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Cancelar(string szNumero, string szSerie, string szChave, string szProtocolo, string szJustificativa);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Inutilizar_Numeros(string szInicial, string szFinal, string szSerie, string szJustificativa);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Obter_Erro([MarshalAs(UnmanagedType.VBByRefStr)] ref string szCodigo, [MarshalAs(UnmanagedType.VBByRefStr)] ref string szDescricao);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Obter_Estado([MarshalAs(UnmanagedType.VBByRefStr)] ref string szEstado);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Verifica_Servidor([MarshalAs(UnmanagedType.VBByRefStr)] ref string szEstado);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Imprimir_Mensagem(string szMensagem);
		[DllImport("InterfaceEpsonNF.dll")]public static extern int EPSON_NFCe_Obter_Informacao(string szIndice, [MarshalAs(UnmanagedType.VBByRefStr)] ref string szInformacao);
	}
}
