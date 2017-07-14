using System;
using System.Collections.Generic;
using System.Linq;
using Syslaps.Pdv.Core.Dominio.Base;
using System.Text;

namespace Syslaps.Pdv.Core.Dominio.Impressora
{
    public class Cupom : ModeloBase
    {
        private readonly IImpressora _impressora;
        private readonly Parametros _parametros;

        public Cupom(IImpressora impressora, Parametros parametros)
        {
            _impressora = impressora;
            _parametros = parametros;
        }

        private bool ImprimirTextoTag(string textoTag)
        {
            return _impressora.ImprimirTextoTag(textoTag);
        }

        private bool ImprimirTexto(string texto)
        {
            return _impressora.ImprimirTexto(texto);
        }

        private string RecuperarProdutosVendidosParaCupom(Entity.Venda vendaCorrente)
        {
            var texto = string.Empty;

            vendaCorrente.VendaProdutoes.ToList().ForEach(vendaProduto =>
            {
                var valorTotal = (vendaProduto.ValorDoProdutoComDesconto * vendaProduto.Quantidade).ToString("0.00").PadLeft(10, ' ');
                var qtde = vendaProduto.Quantidade.ToString("##.#").PadLeft(5, ' ');
                var valorUn = vendaProduto.ValorDoProdutoComDesconto.ToString("0.00");

                var temp = string.Concat(valorUn, qtde, valorTotal);
                var resto = 59 - temp.Length;
                var desc = vendaProduto.DescricaoProduto;

                if (desc.Length > resto)
                    desc = string.Concat(vendaProduto.DescricaoProduto.Substring(0, resto - 1), " ", valorUn, qtde, valorTotal);
                else
                    desc = string.Concat(vendaProduto.DescricaoProduto.PadRight(resto, ' '), valorUn, qtde, valorTotal);

                texto += $"<c>{vendaProduto.CodigoParaCupom} {desc}</c>\n";
            });

            return texto;
        }

        private string RecuperarProdutosPagamentosParaCupom(Entity.Venda vendaCorrente)
        {
            var texto = string.Empty;

            vendaCorrente.VendaPagamentoes.ToList().ForEach(vendaPagamento =>
            {
                var tipoPagamento = string.Concat(vendaPagamento.TipoPagamento.Nome, ": ");
                var valor = vendaPagamento.ValorPagamento.ToString("c");


                var resto = 64 - valor.Length;
                var desc = string.Empty;

                if (tipoPagamento.Length > resto)
                    desc = string.Concat(tipoPagamento.Substring(0, resto - 1), " ", valor);
                else
                    desc = string.Concat(tipoPagamento.PadRight(resto, ' '), valor);

                texto += $"<c>{desc}\n";
            });

            return texto;
        }

        private string RecuperarValorSubTotalParaCupom(Entity.Venda vendaCorrente)
        {
            var texto = "";

            var valorTotal = vendaCorrente.VendaProdutoes.Sum(x => x.ValorDoProduto * x.Quantidade).ToString("c");
            texto += "Subtotal:".PadRight((64 - valorTotal.Length), ' ');
            texto += valorTotal + "\n";
            return texto;
        }
        private string RecuperarValorTotalParaCupom(Entity.Venda vendaCorrente)
        {
            var texto = "<b>";

            var valorTotal = vendaCorrente.VendaProdutoes.Sum(x => x.ValorTotalVendaProduto).ToString("c");
            texto += "TOTAL:".PadRight((64 - valorTotal.Length), ' ');
            texto += valorTotal + "</b>\n";
            return texto;
        }

        private string RecuperarValorDescontoTotalParaCupom(Entity.Venda vendaCorrente)
        {
            var texto = "";

            var valorTotal = (vendaCorrente.VendaProdutoes.Sum(x => x.ValorTotalDoDesconto) * -1).ToString("c");
            texto += "Desconto:".PadRight((64 - valorTotal.Length), ' ');
            texto += valorTotal + "\n";
            return texto;
        }

        private string RecuperarTrocoParaCupom(Entity.Venda vendaCorrente)
        {
            var texto = string.Empty;

            var troco = vendaCorrente.ValorTroco.ToString("c");
            texto += "Troco:".PadRight((64 - troco.Length), ' ');
            texto += troco + "\n";
            return texto;
        }

        private string RecuperarTotalDeTributos(Entity.Venda vendaCorrente)
        {
            var tributos = (_parametros.CfopTributo / 100) * vendaCorrente.ValorTotalVenda;
            var texto = $"<c><ce>Valor aproximado dos tributos deste cupom: {tributos.ToString("c")}\nConforme Lei Federal 12.741/2012</c></ce>\n";


            return texto;
        }

        private string RecuperarClienteParaCupom(Entity.Venda vendaCorrente)
        {
            var texto = "<c><ce>CPF/CNPJ do Cosumidor: ";

            if (string.IsNullOrEmpty(vendaCorrente.CpfCnpjCliente))
            {
                texto += "Cliente Não Identificado";
            }
            else
            {
                texto += string.Concat(vendaCorrente.CpfCnpjCliente, string.IsNullOrEmpty(vendaCorrente.NomeCliente) ? "" : "\nNome: " + vendaCorrente.NomeCliente);
            }

            return string.Concat(texto, "</ce></c>\n");
        }

        public bool AbrirGaveta()
        {
            return _impressora.AbrirGaveta();
        }

        public bool AcionarGuilhotina()
        {
            return _impressora.AcionarGuilhotina();
        }

        public void ImprimirVenda(Entity.Venda vendaCorrente)
        {
            if (vendaCorrente.SatResponse == null)
                ImprimirVendaNaoSat(vendaCorrente);
            else
                ImprimirVendaSat(vendaCorrente);
        }

        private void ImprimirVendaNaoSat(Entity.Venda vendaCorrente)
        {
            vendaCorrente.CupomFiscalImpresso = true;
            string texto = new String(' ', 4096);

            texto = $"<cespl>50</cespl><ce><b>{_parametros.NomeDaEmpresa}</b></ce>\n";
            texto += $"<ce><c>CNPJ: {_parametros.CnpjDaEmpresa} - IE: {_parametros.IeDaEmpresa}\n";
            texto += $"{_parametros.Endereco}, Bairro: {_parametros.BairroDaEmpresa}\n";
            texto += $"Cidade: {_parametros.CidadeDaEmpresa} \n";
            texto += $"Telefone: {_parametros.TelefoneDaEmpresa} \n";
            texto += "----------------------------------------------------------------\n";
            texto += $"<ce>Cupom não Fiscal\n";
            texto += $"Data: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}\n";
            texto += $"Nr.: {vendaCorrente.CodigoVenda}</ce>\n";
            texto += "</c></ce><c>----------------------------------------------------------------\n";
            texto += "Código Descrição do Item                Vlr.Unit. Qtde Vlr.Total\n";
            texto += "----------------------------------------------------------------</c>\n";
            texto += RecuperarProdutosVendidosParaCupom(vendaCorrente);
            texto += "<c>----------------------------------------------------------------\n";
            texto += $"QTD. TOTAL DE ITENS:                                         {vendaCorrente.VendaProdutoes.Sum(x => x.Quantidade).ToString("000")}\n";
            texto += RecuperarValorDescontoTotalParaCupom(vendaCorrente);
            texto += RecuperarValorDescontoTotalParaCupom(vendaCorrente);
            texto += RecuperarValorTotalParaCupom(vendaCorrente);
            texto += "\n";
            texto += "FORMA DE PAGAMENTO                                    Valor Pago\n";
            texto += RecuperarProdutosPagamentosParaCupom(vendaCorrente);
            texto += RecuperarTrocoParaCupom(vendaCorrente);
            texto += RecuperarTotalDeTributos(vendaCorrente);
            texto += "</c><c>----------------------------------------------------------------</c>\n";
            texto += RecuperarClienteParaCupom(vendaCorrente);
            texto += "<c>----------------------------------------------------------------</c>\n";
            texto += "\n";
            texto += "<c><ce>Obrigado. Volte Sempre.</ce></c>";
            texto += "\n\n\n\n\n";

            ImprimirTextoTag(texto);
            AcionarGuilhotina();
        }

        private void ImprimirVendaSat(Entity.Venda vendaCorrente)
        {
            vendaCorrente.CupomFiscalImpresso = true;
            string texto = new String(' ', 4096);

            texto = $"<cespl>50</cespl><ce><b>{_parametros.NomeDaEmpresa}</b></ce>\n";
            texto += $"<ce><c>CNPJ: {_parametros.CnpjDaEmpresa} - IE: {_parametros.IeDaEmpresa}\n";
            texto += $"{_parametros.Endereco}, Bairro: {_parametros.BairroDaEmpresa}\n";
            texto += $"Cidade: {_parametros.CidadeDaEmpresa} \n";
            texto += $"Telefone: {_parametros.TelefoneDaEmpresa} \n";
            texto += "----------------------------------------------------------------\n";
            texto += RecuperarTituloCupomFiscalSAT(vendaCorrente);
            texto += "----------------------------------------------------------------\n";
            texto += RecuperarClienteParaCupom(vendaCorrente);
            texto += "</c></ce><c>----------------------------------------------------------------\n";
            texto += "Código Descrição do Item                Vlr.Unit. Qtde Vlr.Total\n";
            texto += "----------------------------------------------------------------</c>\n";
            texto += RecuperarProdutosVendidosParaCupom(vendaCorrente);
            texto += "<c>----------------------------------------------------------------\n";
            texto += $"QTD. TOTAL DE ITENS:                                         {vendaCorrente.VendaProdutoes.Sum(x => x.Quantidade).ToString("000")}\n";
            texto += RecuperarValorSubTotalParaCupom(vendaCorrente);
            texto += RecuperarValorDescontoTotalParaCupom(vendaCorrente);
            texto += RecuperarValorTotalParaCupom(vendaCorrente);
            texto += "\n";
            texto += "FORMA DE PAGAMENTO                                    Valor Pago\n";
            texto += RecuperarProdutosPagamentosParaCupom(vendaCorrente);
            texto += RecuperarTrocoParaCupom(vendaCorrente);
            texto += RecuperarTotalDeTributos(vendaCorrente);
            texto += "<c>----------------------------------------------------------------</c>";
            texto += $"\n<ce><c>SAT No. {_parametros.NumeroSat}";
            texto += $"\n{vendaCorrente.DataVenda.ToString("dd/MM/yyyy HH:mm:ss")}</c></ce>";
            texto += $"\n<c><ce>{RecuperarCodigoDeInvoice(vendaCorrente)}</ce></c>";


            ImprimirTextoTag(texto);
            ImprimirCodigoBarra(vendaCorrente);
            ImprimirQRCode(vendaCorrente);
            texto = "<c><ce>Consulte o QR Code pelo aplicativo \"De olho na nota\",\ndisponível na AppStore (Apple) e PlayStore (Android)</ce></c>\n";
            ImprimirTextoTag(texto);
            AcionarGuilhotina();
        }

        private void ImprimirQRCode(Entity.Venda vendaCorrente)
        {
            var strBuild = string.Concat(vendaCorrente.SatResponse.InvoiceKey, "|", vendaCorrente.SatResponse.TimeStamp, "|", vendaCorrente.SatResponse.Total, "|", vendaCorrente.SatResponse.CpfCnpj, "|", vendaCorrente.SatResponse.QrCodeSignature);

            _impressora.ImprimirQRCode(1, 3, 0, 10, 1, strBuild);
        }

        private void ImprimirCodigoBarra(Entity.Venda vendaCorrente)
        {
            _impressora.ImprimirCodigoBarra(vendaCorrente.SatResponse.InvoiceKey);
        }

        private string RecuperarCodigoDeInvoice(Entity.Venda vendaCorrente)
        {
            var returnVal = vendaCorrente.SatResponse.InvoiceKey;
            var skip = 0;
            for (int i = 1; i <= 10; i++)
            {
                var position = 4 * i;

                returnVal = returnVal.Insert(position + skip, " ");
                skip++;

            }

            return returnVal;
        }

        private string RecuperarTituloCupomFiscalSAT(Entity.Venda vendaCorrente)
        {
            var returnVal = $"Extrato No. {vendaCorrente.CodigoVenda} \nCUPOM FISCAL ELETRONICO - SAT \n";
            return returnVal;
        }

        private string RecuperarProdutosDoPedido(Entity.Pedido pedido)
        {
            var texto = "<c>";
            pedido.PedidoProduto.ToList().ForEach(vendaProduto =>
            {
                var qtde = vendaProduto.Quantidade.ToString();
                //texto += vendaProduto.Produto.Descricao.PadRight((64 - qtde.Length), '.');
                texto += vendaProduto.Produto.Descricao.PadRight((45 - qtde.Length), '.');
                texto += string.Concat(qtde, "\n");
            });

            return string.Concat(texto, "</c>");
        }

        public void ImprimirVendaCurta(Entity.Venda vendaCorrente)
        {
            string texto = new String(' ', 4096);

            texto = $"<ce><b>Venda Realizada: {vendaCorrente.DataVenda.ToString("dd/MM/yyyy HH:mm:ss")}</b></ce>\n";
            texto += "<c>----------------------------------------------------------------\n";
            texto += "Código Descrição do Item                Vlr.Unit. Qtde Vlr.Total\n";
            texto += "----------------------------------------------------------------</c>\n";
            texto += RecuperarProdutosVendidosParaCupom(vendaCorrente);
            texto += "<c>----------------------------------------------------------------\n";
            texto += $"QTD. TOTAL DE ITENS:                                         {vendaCorrente.VendaProdutoes.Count.ToString("000")}\n";
            texto += RecuperarValorTotalParaCupom(vendaCorrente);
            texto += "\n";
            texto += "FORMA DE PAGAMENTO                                    Valor Pago\n";
            texto += RecuperarProdutosPagamentosParaCupom(vendaCorrente);
            texto += RecuperarTrocoParaCupom(vendaCorrente);
            texto += "----------------------------------------------------------------</c>\n";
            texto += RecuperarClienteParaCupom(vendaCorrente);
            texto += "<c>----------------------------------------------------------------</c>\n";
            texto += "\n\n\n\n\n";

            ImprimirTextoTag(texto);
        }

        public void ImprimirComanda(Entity.Venda vendaCorrente)
        {
            string texto = new String(' ', 4096);

            texto = $"<ce><b>Consumo Realizado: {vendaCorrente.DataVenda.ToString("dd/MM/yyyy HH:mm:ss")}</b></ce>\n";
            texto += "<c>----------------------------------------------------------------\n";
            texto += "Código Descrição do Item                Vlr.Unit. Qtde Vlr.Total\n";
            texto += "----------------------------------------------------------------</c>\n";
            texto += RecuperarProdutosVendidosParaCupom(vendaCorrente);
            texto += "<c>----------------------------------------------------------------\n";
            texto += $"QTD. TOTAL DE ITENS:                                         {vendaCorrente.VendaProdutoes.Sum(x => x.Quantidade).ToString("000")}\n";
            texto += RecuperarValorSubTotalParaCupom(vendaCorrente);
            texto += RecuperarValorDescontoTotalParaCupom(vendaCorrente);
            texto += RecuperarValorTotalParaCupom(vendaCorrente);
            texto += "\n";
            texto += "----------------------------------------------------------------</c>\n";
            texto += "\n\n\n\n\n";

            ImprimirTextoTag(texto);
            AcionarGuilhotina();
        }

        public void ImprimirListaVendaCurta(List<Entity.Venda> vendas)
        {
            vendas.ForEach(ImprimirVendaCurta);

            AcionarGuilhotina();
        }

        public void ImprimirPedido(Entity.Pedido pedido)
        {
            string texto = new String(' ', 4096);

            texto = $"<ce><b>Data da Entrega: {pedido.DataEntrega.ToString("dd/MM/yyyy HH:mm")}</b></ce>\n";
            texto += "<c>----------------------------------------------------------------\n";
            texto += $"<b>Cliente: {pedido.NomeCliente}</b>\n";
            texto += $"<b>Telefone: {pedido.Telefone}</b>\n";
            texto += "----------------------------------------------------------------</c>\n";
            texto += "Produto                          Quantidade\n";
            texto += "<c>----------------------------------------------------------------</c>\n";
            texto += RecuperarProdutosDoPedido(pedido);
            texto += "<c>----------------------------------------------------------------</c>\n";
            texto += "<ce>Observações</ce>\n";
            texto += "<c>----------------------------------------------------------------</c>\n";
            texto += $"<ce><c>{pedido.Observacao}</c></ce>\n";
            texto += "<c>----------------------------------------------------------------</c>\n";
            texto += "\n\n";

            ImprimirTextoTag(texto);
        }
    }
}
