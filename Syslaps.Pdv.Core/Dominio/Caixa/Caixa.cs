using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Core.Dominio.Venda;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;
using Syslaps.Pdv.Entity.Especializadas;

namespace Syslaps.Pdv.Core.Dominio.Caixa
{
    public class Caixa : ModeloBase
    {
        public delegate void OperacaoExecutadaHandler(EnumCaixaTipoOperacao tipoOperacao);

        public event OperacaoExecutadaHandler OnOperacaoExecutada;

        public Entity.Caixa CaixaCorrente { get; private set; }
        public Entity.Usuario UsuarioCorrente { get; private set; }
        public Venda.TipoPagamento TipoDoPagamento { get; private set; }

        private readonly ICaixaRepositorio _repositorio;
        private readonly IVendaRepositorio _vendaRepositorio;
        private readonly Parametros _parametros;
        private readonly IEmail _email;

        public Caixa(ICaixaRepositorio repositorio, IVendaRepositorio vendaRepositorio, IEmail email, Parametros parametros, Venda.TipoPagamento tipoDoPagamento, Entity.Usuario usuario, string nomeDoCaixa = "")
        {
            _repositorio = repositorio;
            _vendaRepositorio = vendaRepositorio;
            _parametros = parametros;
            _email = email;
            CaixaCorrente = repositorio.RecuperarCaixaPorNome(nomeDoCaixa);
            TipoDoPagamento = tipoDoPagamento;
            UsuarioCorrente = usuario;

            if (CaixaCorrente == null)
                RegistrarNovoCaixa(nomeDoCaixa, RecuperarIp());
        }

        public void AbrirCaixa(decimal valor)
        {
            if (CaixaCorrente.Situacao == EnumCaixaSituacao.Aberto.ToString())
            {
                AdicionarMensagem("Não é possível abrir o caixa. Feche-o primeiro e tente novamente.", EnumStatusDoResultado.RegraDeNegocioInvalida);
            }
            else
            {
                var operacaoCorrente = RegistrarMovimentacaoCaixa(EnumCaixaTipoOperacao.Abertura, valor, string.Empty);
                CaixaCorrente.Situacao = EnumCaixaSituacao.Aberto.ToString();
                CaixaCorrente.CodigoOperacaoDeAbertura = operacaoCorrente.CodigoOperacaoCaixa;
                CaixaCorrente.OperacaoCaixas.Add(operacaoCorrente);
                _repositorio.Atualizar(CaixaCorrente);
                AdicionarMensagem("Caixa aberto com sucesso.");
                OnOperacaoExecutada?.Invoke(EnumCaixaTipoOperacao.Abertura);
            }
        }

        public void FecharCaixa(decimal valorContabilizadoNoFechamento)
        {
            if (CaixaCorrente.Situacao == EnumCaixaSituacao.Fechado.ToString())
            {
                AdicionarMensagem("Não é possível fechar o caixa. Abra-o primeiro e tente novamente.", EnumStatusDoResultado.RegraDeNegocioInvalida);
            }
            else
            {
                var operacaoFechamento = RegistrarMovimentacaoCaixa(EnumCaixaTipoOperacao.Fechamento, valorContabilizadoNoFechamento, CaixaCorrente.CodigoOperacaoDeAbertura);
                CaixaCorrente.Situacao = EnumCaixaSituacao.Fechado.ToString();
                _repositorio.Atualizar(CaixaCorrente);
                RegistrarResultadoFechamento(operacaoFechamento, valorContabilizadoNoFechamento);
                AdicionarMensagem("Caixa fechado com sucesso.");
                OnOperacaoExecutada?.Invoke(EnumCaixaTipoOperacao.Fechamento);
            }
        }

        private void RegistrarResultadoFechamento(OperacaoCaixa operacaoCaixaFechamento, decimal valorContabilizadoNoFechamento)
        {
            var vendas = _vendaRepositorio.RecuperarListaDasVendasDaOperacaoDeAbertura(CaixaCorrente.CodigoOperacaoDeAbertura);
            if (vendas != null && vendas.Count > 0)
            {
                var operacoesCaixa = _repositorio.RecuperarOperacoesCaixaPorCodigoDeAbertura(CaixaCorrente.CodigoOperacaoDeAbertura);
                var operacaoCaixaAbertura = operacoesCaixa.SingleOrDefault(x => x.TipoOperacao == EnumCaixaTipoOperacao.Abertura.ToString());

                if (operacaoCaixaAbertura != null)
                {
                    var resultadoFechamento = new ResultadoOperacaoFechamento();

                    resultadoFechamento.CodigoResultado = GerarCodigoUnico();
                    resultadoFechamento.OperacaoCaixa_CodigoOperacaoCaixa = operacaoCaixaFechamento.CodigoOperacaoCaixa;
                    resultadoFechamento.ValorAbertura = operacaoCaixaAbertura.ValorOperacao;
                    resultadoFechamento.ValorContabilizadoNoFechamento = valorContabilizadoNoFechamento;
                    resultadoFechamento.ValorTotalReforco = operacoesCaixa.Where(x => x.TipoOperacao == EnumCaixaTipoOperacao.Reforco.ToString()).Sum(x => x.ValorOperacao);
                    resultadoFechamento.ValorTotalSangria = operacoesCaixa.Where(x => x.TipoOperacao == EnumCaixaTipoOperacao.Sangria.ToString()).Sum(x => x.ValorOperacao);

                    vendas.ForEach(venda =>
                    {
                        var vendaPagamentos = _vendaRepositorio.RecuperarListaDosPagamentosDaVenda(venda.CodigoVenda);
                        resultadoFechamento.ValorTotalDescontoVenda += venda.ValorTotalDescontoVenda;

                        TipoDoPagamento.ListaDeTiposDePagamento.Where(x => x.Nome.Contains("Dinheiro")).ToList().ForEach(item =>
                        {
                            resultadoFechamento.ValorTotalPagamentoDinheiro += vendaPagamentos.Where(
                                    x => x.TipoPagamento_CodigoTipoPagamento == item.CodigoTipoPagamento).Sum(x => x.ValorPagamento) - venda.ValorTroco;
                        });

                        TipoDoPagamento.ListaDeTiposDePagamento.Where(x => x.Nome.Contains("Crédito")).ToList().ForEach(item =>
                        {
                            resultadoFechamento.ValorTotalPagamentoCredito +=
                            vendaPagamentos.Where(x => x.TipoPagamento_CodigoTipoPagamento == item.CodigoTipoPagamento).Sum(x => x.ValorPagamento);

                            resultadoFechamento.ValorRecebimentoCretito = resultadoFechamento.ValorTotalPagamentoCredito - (item.PercentualDesconto / 100 * resultadoFechamento.ValorTotalPagamentoCredito);
                        });

                        TipoDoPagamento.ListaDeTiposDePagamento.Where(x => x.Nome.Contains("Débito")).ToList().ForEach(item =>
                        {
                            resultadoFechamento.ValorTotalPagamentoDebito +=
                            vendaPagamentos.Where(x => x.TipoPagamento_CodigoTipoPagamento == item.CodigoTipoPagamento).Sum(x => x.ValorPagamento);

                            resultadoFechamento.ValorRecebimentoDebito = resultadoFechamento.ValorTotalPagamentoDebito - (item.PercentualDesconto / 100 * resultadoFechamento.ValorTotalPagamentoDebito);
                        });


                        TipoDoPagamento.ListaDeTiposDePagamento.Where(x => x.Nome.Contains("Tiket")).ToList().ForEach(item =>
                        {
                            resultadoFechamento.ValorTotalPagamentoTicket +=
                            vendaPagamentos.Where(x => x.TipoPagamento_CodigoTipoPagamento == item.CodigoTipoPagamento).Sum(x => x.ValorPagamento);

                            resultadoFechamento.ValorRecebimentoTicket += resultadoFechamento.ValorTotalPagamentoTicket - (item.PercentualDesconto / 100 * resultadoFechamento.ValorTotalPagamentoTicket);
                        });
                        

                    });

                    resultadoFechamento.ValorTotalPagamento = resultadoFechamento.ValorTotalPagamentoDinheiro +
                                                              resultadoFechamento.ValorTotalPagamentoCredito +
                                                              resultadoFechamento.ValorTotalPagamentoDebito +
                                                              resultadoFechamento.ValorTotalPagamentoTicket;

                    resultadoFechamento.ValorTotalEstimadoEmEspecie = resultadoFechamento.ValorAbertura +
                        resultadoFechamento.ValorTotalReforco + 
                        resultadoFechamento.ValorTotalPagamentoDinheiro -
                        resultadoFechamento.ValorTotalSangria; 
                    resultadoFechamento.DiferencaNoCaixa = resultadoFechamento.ValorContabilizadoNoFechamento - resultadoFechamento.ValorTotalEstimadoEmEspecie;

                    _repositorio.Inserir<ResultadoOperacaoFechamento>(resultadoFechamento);

                    Task.Factory.StartNew(() =>
                    {
                        EnviarEmail(resultadoFechamento);
                    });

                }
            }
        }

        public void SincronizarCaixa()
        {
            OnOperacaoExecutada?.Invoke(EnumCaixaTipoOperacao.Sincronismo);
        }

        public void AdicionarReforco(decimal valor)
        {
            if (CaixaCorrente != null && CaixaCorrente.Situacao == EnumCaixaSituacao.Fechado.ToString())
            {
                AdicionarMensagem("O caixa não está aberto.");
            }
            else
            {
                if (CaixaCorrente == null)
                {
                    AdicionarMensagem("Nome do caixa não foi encontrado.", EnumStatusDoResultado.RegraDeNegocioInvalida);
                }
                else
                {
                    if (CaixaCorrente.Situacao == EnumCaixaSituacao.Fechado.ToString())
                    {
                        AdicionarMensagem("Não é possível adicionar reforço pois o caixa está fechado.", EnumStatusDoResultado.RegraDeNegocioInvalida);
                    }
                    else
                    {
                        RegistrarMovimentacaoCaixa(EnumCaixaTipoOperacao.Reforco, valor, CaixaCorrente.CodigoOperacaoDeAbertura);
                        AdicionarMensagem("Reforço adicionado com sucesso.");
                        OnOperacaoExecutada?.Invoke(EnumCaixaTipoOperacao.Reforco);
                    }
                }
            }
        }

        public void EfetuarSangria(decimal valor)
        {
            if (CaixaCorrente != null && CaixaCorrente.Situacao == EnumCaixaSituacao.Fechado.ToString())
            {
                AdicionarMensagem("O caixa não está aberto.", EnumStatusDoResultado.RegraDeNegocioInvalida);
            }
            else
            {
                if (CaixaCorrente == null)
                {
                    AdicionarMensagem("Nome do caixa não foi encontrado.", EnumStatusDoResultado.RegraDeNegocioInvalida);
                }

                if (CaixaCorrente == null || CaixaCorrente.Situacao == EnumCaixaSituacao.Fechado.ToString())
                {
                    AdicionarMensagem("Não é possível efetuar sangria pois o caixa está fechado.", EnumStatusDoResultado.RegraDeNegocioInvalida);
                }
                else
                {
                    RegistrarMovimentacaoCaixa(EnumCaixaTipoOperacao.Sangria, valor, CaixaCorrente.CodigoOperacaoDeAbertura);
                    AdicionarMensagem("Sangria efetuada com sucesso.");
                    OnOperacaoExecutada?.Invoke(EnumCaixaTipoOperacao.Sangria);
                }
            }
        }

        public void RegistrarNovoCaixa(string nomeDeRegistroDoCaixa, string ipDoCaixa)
        {
            var novoCaixa = new Entity.Caixa
            {
                CodigoCaixa = GerarCodigoUnico(),
                Nome = nomeDeRegistroDoCaixa,
                Situacao = EnumCaixaSituacao.Fechado.ToString(),
                CodigoOperacaoDeAbertura = "",
                Machine = Dns.GetHostName(),
                IP = ipDoCaixa
            };

            _repositorio.Inserir(novoCaixa);
            CaixaCorrente = novoCaixa;
            AdicionarMensagem("Caixa criado com sucesso.");
        }

        private OperacaoCaixa RegistrarMovimentacaoCaixa(EnumCaixaTipoOperacao tipoOperacao, decimal valor, string codigoUnicoAbertura)
        {
            if (tipoOperacao == EnumCaixaTipoOperacao.Reforco || tipoOperacao == EnumCaixaTipoOperacao.Sangria)
                codigoUnicoAbertura = CaixaCorrente.CodigoOperacaoDeAbertura;
            var operacao = new Entity.OperacaoCaixa
            {
                CodigoOperacaoCaixa = GerarCodigoUnico(),
                Caixa_CodigoCaixa = CaixaCorrente.CodigoCaixa,
                Usuario_CodigoUsuario = UsuarioCorrente.CodigoUsuario,
                DataOperacao = DateTime.Now,
                TipoOperacao = tipoOperacao.ToString(),
                ValorOperacao = valor,
                CodigoOperacaoCaixaAbertura = codigoUnicoAbertura
            };

            _repositorio.Inserir(operacao);
            AdicionarMensagem($"Movimento de {tipoOperacao} registrado com sucesso.");
            return operacao;
        }

        private TotalVendido RecuperarTotalVendido(DateTime dataInicial, DateTime datafinal)
        {
            return _vendaRepositorio.RecuperarTotalDeVendaDoPeriodo(dataInicial, datafinal);
        }

        private void EnviarEmail(ResultadoOperacaoFechamento resultado)
        {
            var dataInicial = DateTime.Now.GetFirstDateTimeOfMonth();
            var datafinal = DateTime.Now.GetLastDateTimeOfMonth();
            var totalVendido = RecuperarTotalVendido(dataInicial, datafinal);
            var senderEmail = _parametros.SmtpSenderEmail;
            var senderName = _parametros.SmtpSenderName;
            var fromEmail = _parametros.SmtpSenderEmail;
            var fromName = _parametros.SmtpSenderName;
            var subject = "Resultado do Fechamento";
            var htmlBody = "";

            var emailsTo = new List<MailAddress>();
            _parametros.EmailsParaEnviar.Split(';').ToList().ForEach(email => emailsTo.Add(new MailAddress(email)));

            htmlBody =
                $@"<h1>Resultado fechamento dia: {DateTime.Now:dd/MM/yyy HH:mm} - <span style='color: red;'>{resultado.ValorTotalPagamento.ToString("c", CultureInfo.GetCultureInfo("pt-BR"))}</span></h1>
                                    <table style='width: 400px; border-collapse: collapse;'>

                                        <tr><td style='width: 250px;  border: 1px solid black;'>Nome Do PDV:</td>
		                                <td align='right' style='width: 150px;  border: 1px solid black;'>{"NomeDoCaixa".GetConfigValue().ToUpper()}</td></tr>

	                                    <tr><td style='width: 250px;  border: 1px solid black;'>Valor de Abertura:</td>
		                                <td  align='right' style='width: 150px;  border: 1px solid black;'>{resultado.ValorAbertura:c}</td></tr>

                                        <tr><td style='width: 250px;  border: 1px solid black;'>Dinheiro no Caixa p/ Operador:</td>
		                                <td align='right' style='width: 150px;  border: 1px solid black;'>{resultado
                        .ValorContabilizadoNoFechamento:c}</td></tr>

                                        <tr><td style='width: 250px;  border: 1px solid black;'>Pagamentos em Dinheiro:</td>
		                                <td align='right' style='width: 150px;  border: 1px solid black;'>{resultado
                            .ValorTotalPagamentoDinheiro:c}</td></tr>

                                        <tr><td style='width: 250px;  border: 1px solid black;'>Pagamentos em Debito:</td>
		                                <td align='right' style='width: 150px;  border: 1px solid black;'>{resultado
                                .ValorTotalPagamentoDebito:c}</td></tr>

                                        <tr><td style='width: 250px;  border: 1px solid black;'>Pagamentos em Credito:</td>
		                                <td align='right' style='width: 150px;  border: 1px solid black;'>{resultado
                                    .ValorTotalPagamentoCredito:c}</td></tr>

                                        <tr><td style='width: 250px;  border: 1px solid black;'>Pagamentos em Ticket:</td>
		                                <td align='right' style='width: 150px;  border: 1px solid black;'>{resultado
                                        .ValorTotalPagamentoTicket:c}</td></tr>

                                        <tr><td style='width: 250px;  border: 1px solid black;'>Dinheiro no Caixa p/ Sistema:</td>
		                                <td align='right' style='width: 150px;  border: 1px solid black;'>{resultado
                                            .ValorTotalEstimadoEmEspecie:c}</td></tr>

                                        <tr><td style='width: 250px;  border: 1px solid black;'>Diferença:</td>
		                                <td align='right' style='width: 150px;  border: 1px solid black;'>{resultado
                                                .DiferencaNoCaixa:c}</td></tr>
                                    </table>
                    <h3>Total Venda em {DateTime.Now.GetMonthName()}: <span style='color: red;'>{totalVendido.ValorTotal.ToString("c", CultureInfo.GetCultureInfo("pt-BR"))}</span></br>
                    Quantidade Vendida: <span style='color: red;'>{totalVendido.Quantidade}</span></h3>";

            
            _email.Enviar(senderEmail, senderName, fromEmail, fromName, subject, htmlBody, emailsTo);
        }
    }
}