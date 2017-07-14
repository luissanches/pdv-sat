using System;
using Syslaps.Pdv.Core.Dominio.Impressora;
using Syslaps.Pdv.UI;

namespace Syslaps.Pdv.TestPrinter
{
    class Program
    {
        static void Main(string[] args) 
        {
            DevoImprimirCupomNaoFiscal();
        }

        public static void DevoImprimirCupomNaoFiscal()
        {
            var texto = new String(' ', 4096);

            texto = "------------------------------------------\n";
            texto += "<ce><c>Empresa Teste\n";
            texto += "CNPJ: xxxxxxxxxxxxxx      Inscrição Estadual: yyyyyyyyyy\n";
            texto += "Rua: aaaaaaaaaaaa, Número: 999   Bairro: bbbbbbb\n";
            texto += "Cidade: zzzzzzzzzzzz nn\n";
            texto += "--------------------------------------------------------\n";
            texto += "DANFE NFC-e - Documento Auxiliar\n";
            texto += "da Nota Fiscal Eletrônica para Consumidor Final\n";
            texto += "Não permite aproveitamento de crédito de ICMS\n";
            texto += "</c></ce><c>--------------------------------------------------------\n";
            texto += "Código Descrição do Item        Vlr.Unit. Qtde Vlr.Total\n";
            texto += "--------------------------------------------------------\n";
            texto += "</c>" +
                     "<c>333333 ITEM 01                     37,14 001Un     37,14</c>\n";
            texto += "<c>444444 ITEM 02                     13,61 001Un     13,61</c>\n";
            texto += "<c>--------------------------------------------------------\n";
            texto += "QTD. TOTAL DE ITENS                                    2\n";
            texto += "VALOR TOTAL R$                                     50,75\n";
            texto += "\n";
            texto += "FORMA DE PAGAMENTO                            Valor Pago\n";
            texto += "</c><c>DINHEIRO                                           50,75\n";
            texto += "</c><c>VALOR PAGO R$                                      50,75\n";
            texto += "TROCO R$                                            0,00\n";
            texto += "</c><c>--------------------------------------------------------</c>\n";
            texto += "<c><ce>Val Aprox Tributos R$ 16.29 (32.10%) Fonte: IBPT</c></ce>\n";
            texto += "<c>--------------------------------------------------------</c>\n";
            texto += "<c><ce><ce>NFC-e nº 000001 Série 001\n";
            texto += "Emissão 03/12/2013 15:50:16</c></ce>\n";
            texto += "<ce><b></c><c><b>Via Consumidor</c></b>\n";
            texto += "</b></ce><c><ce>Consulte pela Chave de Acesso em</c>\n";
            texto += "<c>https://www.sefaz.rs.gov.br/NFCE/NFCE-COM.aspx\n";
            texto += "\n";
            texto += "<c><b>CHAVE DE ACESSO</b></ce></c>\n";
            texto += "<c><ce>8877 2222 4444 1101 7777 6666</ce></c>\n";
            texto += "<c><ce>0000 8888 3333 6666 7788</ce></c>\n";
            texto += "<c>--------------------------------------------------------</c>\n";
            texto += "<c><ce><b>CONSUMIDOR NÃO IDENTIFICADO</b></ce></c>\n";
            texto += "<c>--------------------------------------------------------</c>\n";
            texto += "<c><ce><b>Consulta via leitor de QR Code </b></ce></c>\n";
            texto += "<ce><qrcode>https://www.sefaz.rs.gov.br/NFCE/NFCE-COM.aspx?chNFe=00000000000000000000000000000000000000000000&nVersao=100&tpAmb=2&cDest=11111111111111&dhEmi=77777777777777777777777777777777777777777777777777&vNF=50.75&vICMS=0.00&digVal=88888888888888888888888888888888888888888888888888888888&cIdToken=000001&cHashQRCode=8888888888888887777777777777777777777777<lmodulo>3</lmodulo></qrcode></ce>";
            texto += "<c><ce>Protocolo de Autorização: 000000000000001</c>\n";
            texto += "<c>03/12/2013 15:50:22\n</ce></c><gui></gui>";
            var impressora = ContainerIoc.GetInstance<IImpressora>();
            
            //var texto2 = "<c>333333 ITEM 01                             37,14 001Un     37,14</c>"; //64

            impressora.ImprimirTextoTag(texto);
            impressora.FecharComunicacao();
        }
    }
}
