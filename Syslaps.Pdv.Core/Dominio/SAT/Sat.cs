using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Cross;
using System.Globalization;
using System.Linq;
using Syslaps.Pdv.Entity;
using Syslaps.Pdv.Entity.SAT;
using Syslaps.Pdv.Core.Dominio.Venda;

namespace Syslaps.Pdv.Core.Dominio.SAT
{
    public class Sat : ModeloBase
    {
        private ISat _sat;
        private IRepositorioBase _repositorio;

        private Parametros _parametros;
        private Entity.Venda _venda;

        public ISat SatIoc
        {
            set { _sat = value; }
        }

        public Sat(ISat iSat, IRepositorioBase repositorio, Parametros parametros, Entity.Venda venda = null)
        {
            _sat = iSat;
            _repositorio = repositorio;
            _parametros = parametros;
            _venda = venda;
        }

        public Sat(IRepositorioBase repositorio)
        {
            _repositorio = repositorio;
        }

        public GetStatusResponse VerificarStatus()
        {
            return _sat.GetStatus();
        }

        public SatResponse VerificarDisponibilidade()
        {
            return _sat.CheckAvailability();
        }

        public SendResponse EnviarVenda(string xml)
        {
            return _sat.Send(xml);
        }

        public void RegistrarVendaSat()
        {
            if (ValidateEnviarSat())
            {
                var xmlVenda = GerarXmlDeVenda();
                _venda.SatResponse = EnviarVenda(xmlVenda);
                RegistrarRespostaSat(xmlVenda);
            }
        }

        private bool ValidateEnviarSat()
        {
            if (!_parametros.SatHabilitado) return false;
            if (!_venda.CpfCnpjCliente.IsNullOrEmpty()) return true;
            if (_venda.CupomFiscalImpresso) return true;
            if (_venda.VendaPagamentoes.Count(x => x.TipoPagamento.Nome.Contains("Débito") ||
             x.TipoPagamento.Nome.Contains("Crédito") ||
             x.TipoPagamento.Nome.Contains("Ticket")) > 0) return true;

            return false;
        }

        public string GerarVinculoCriptAssinado(string cnpjSofwaretHouse, string cnpjEstabelecimentoComercial, string servicoProvedorDeCertificado = "SafeSign Standard Cryptographic Service Provider")
        {
            var csp = new CspParameters(1, servicoProvedorDeCertificado) { Flags = CspProviderFlags.UseDefaultKeyContainer };
            var rsa = new RSACryptoServiceProvider(csp);
            var cnpjs = string.Concat(cnpjSofwaretHouse.LimparCaractersDocumento(), cnpjEstabelecimentoComercial.LimparCaractersDocumento());

            var bytesCnpjs = Encoding.UTF8.GetBytes(cnpjs);
            var signed = rsa.SignData(bytesCnpjs, "SHA256");
            return Convert.ToBase64String(signed);
        }

        public List<string> RecuperarListaDeServicoProvedorDeCertificado()
        {
            var returnVal = new List<string>();

            using (var stores = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                stores.Open(OpenFlags.ReadOnly);
                var certificados = stores.Certificates;
                foreach (var certificado in certificados)
                {
                    var publicprovider = (RSACryptoServiceProvider)certificado.PrivateKey;
                    returnVal.Add(publicprovider.CspKeyContainerInfo.ProviderName);
                }
            }

            return returnVal;
        }

        private string GerarXmlDeVenda()
        {
            var infCFe_versaoDadosEnt = "versaoDadosEnt='0.07'";
            var CNPJ = _parametros.SHCnpj.LimparCaractersDocumento().CreateXmlElement("CNPJ");
            var signAC = _parametros.SignAC.CreateXmlElement("signAC");
            var numeroCaixa = _parametros.NumCaixa.CreateXmlElement("numeroCaixa");
            var CNPJ2 = _parametros.CnpjDaEmpresa.LimparCaractersDocumento().CreateXmlElement("CNPJ");
            var IE = _parametros.IeDaEmpresa.LimparCaractersDocumento().CreateXmlElement("IE");
            var indRatISSQN = "<indRatISSQN>N</indRatISSQN>";

            var CpfCnpj = string.Empty;
            if (!_venda.CpfCnpjCliente.IsNullOrEmpty())
                CpfCnpj = (_venda.TipoDocumento == TipoDocumento.CNPJ.ToString()) ? _venda.CpfCnpjCliente.LimparCaractersDocumento().CreateXmlElement("CNPJ") : _venda.CpfCnpjCliente.LimparCaractersDocumento().CreateXmlElement("CPF");

            var xNome2 = string.Empty;
            if(!_venda.NomeCliente.IsNullOrEmpty())
                xNome2 = _venda.NomeCliente.CreateXmlElement("xNome");

            var xmlDetalheProduto = "";
            var indx = 1;
            _venda.VendaProdutoes.ToList().ForEach(item =>
            {
                xmlDetalheProduto += GerarXmlDetalheProduto(item, indx);
                indx++;
            });

            var xmlDetalhePagamento = "";
            indx = 1;
            _venda.VendaPagamentoes.ToList().ForEach(item =>
            {
                xmlDetalhePagamento += GerarXmlDetalhePagamento(item);
                indx++;
            });


            var xml = $@"<CFe>
                        <infCFe {infCFe_versaoDadosEnt}>
                            <ide>
                                {CNPJ}
                                {signAC}
                                {numeroCaixa}
                            </ide>
                        <emit>
                            {CNPJ2}
                            {IE}
                            {indRatISSQN}
                        </emit>
                        <dest>{CpfCnpj}{xNome2}
                        </dest>
                        {xmlDetalheProduto}
                        <total />
                        <pgto>
                            {xmlDetalhePagamento}
                        </pgto>
                        </infCFe>
                    </CFe>";

            return xml;
        }

        private string GerarXmlDetalheProduto(VendaProduto vendaProd, int indx)
        {
            var xml = $@"<det nItem='{indx}'>
                            <prod>
                                <cProd>{vendaProd.CodigoParaCupom}</cProd>
                                {"".CreateXmlElement("cEAN")/*codigo de barras*/}
                                <xProd>{vendaProd.DescricaoProduto}</xProd>
                                <NCM>{vendaProd.Produto.CodigoNCM}</NCM>
                                {"".CreateXmlElement("CEST") /*codigo cest*/}
                                <CFOP>{vendaProd.Venda.CFOP}</CFOP>
                                <uCom>UN</uCom>
                                <qCom>{vendaProd.Quantidade.ToString("0.0000", CultureInfo.InvariantCulture)}</qCom>
                                <vUnCom>{vendaProd.ValorDoProdutoComDesconto.ToString("0.000", CultureInfo.InvariantCulture)}</vUnCom>
                                {"".CreateXmlElement("vProd") /* valor bruto calculado pelo sat*/ }
                                <indRegra>A</indRegra>
                                {vendaProd.ValorDoDesconto.ToPositive().ToString("0.00", CultureInfo.InvariantCulture).CreateXmlElement("vDesc")}
                                {"".CreateXmlElement("vOutro")}
                            </prod>
                            <imposto>
                                <vItem12741>{((vendaProd.ValorTotalVendaProduto * _parametros.CfopTributo) / 100).ToString("0.00", CultureInfo.InvariantCulture)}</vItem12741>
                                <ICMS>
                                    <ICMSSN102>
                                        <Orig>0</Orig>
                                        <CSOSN>102</CSOSN>
                                    </ICMSSN102>
                                </ICMS>
                                <PIS>
                                    <PISOutr>
                                        <CST>99</CST>
                                        <qBCProd>0.0000</qBCProd>
                                        <vAliqProd>0.0000</vAliqProd>
                                    </PISOutr>
                                </PIS>
                                <COFINS>
                                    <COFINSOutr>
                                        <CST>99</CST>
                                        <qBCProd>0.0000</qBCProd>
                                        <vAliqProd>0.0000</vAliqProd>
                                    </COFINSOutr>
                                </COFINS>
                            </imposto>
                        </det>";
            return xml;
        }

        private string GerarXmlDetalhePagamento(VendaPagamento vendaPagamento)
        {
            var xml = $@"<MP>
                            <cMP>{RecuperarCodigoDoMeioDePagamento(vendaPagamento)}</cMP>
                            <vMP>{vendaPagamento.ValorPagamento.ToString("0.00", CultureInfo.InvariantCulture)}</vMP>
                        </MP>";
            return xml;
        }

        private string RecuperarCodigoDoMeioDePagamento(VendaPagamento vp)
        {
            //01 - Dinheiro 02 - Cheque 03 - Cartão de Crédito 04 - Cartão de Débito 
            //    05 - Crédito Loja 10 - Vale Alimentação 11 - Vale Refeição 
            //    12 - Vale Presente 13 - Vale Combustível
            switch (vp.TipoPagamento.Nome)
            {
                case "Dinheiro":
                    {
                        return "01";
                    }
                case "Cheque":
                    {
                        return "02";
                    }
                case "Débito Rede":
                    {
                        return "04";
                    }

                case "Débito Moderninha":
                    {
                        return "04";
                    }
                case "Crédito Rede":
                    {
                        return "03";
                    }
                case "Crédito Moderninha":
                    {
                        return "03";
                    }
                case "Ticket":
                    {
                        return "11";
                    }
            }

            return "99";
        }

        private string RetornaMensagemSATPorCodigo(string pCodigo)
        {
            switch (pCodigo)
            {
                case "04000": return "Ativado corretamente SAT Ativado com Sucesso.";
                case "04001": return "Erro na criação do certificado processo de ativação foi interrompido.";
                case "04002": return "SEFAZ não reconhece este SAT (CNPJ inválido) Verificar junto a SEFAZ o CNPJ cadastrado.";
                case "04003": return "SAT já ativado SAT disponível para uso.";
                case "04004": return "SAT com uso cessado SAT bloqueado por cessação de uso.";
                case "04005": return "Erro de comunicação com a SEFAZ Tentar novamente.";
                case "04006": return "CSR ICP-BRASIL criado com sucesso Processo de criação do CSR para certificação ICP-BRASIL com sucesso";
                case "04007": return "Erro na criação do CSR ICP-BRASIL Processo de criação do CSR para certificação ICP-BRASIL com erro";
                case "04098": return "SAT em processamento. Tente novamente.";
                case "04099": return "Erro desconhecido na ativação Informar ao administrador.";
                case "05000": return "Certificado transmitido com Sucesso ";
                case "05001": return "Código de ativação inválido.";
                case "05002": return "Erro de comunicação com a SEFAZ. Tentar novamente.";
                case "05003": return "Certificado Inválido ";
                case "05098": return "SAT em processamento.";
                case "05099": return "Erro desconhecido Informar o administrador.";
                case "06000": return "Emitido com sucesso + conteúdo notas. Retorno CF-e-SAT ao AC para contingência.";
                case "06001": return "Código de ativação inválido.";
                case "06002": return "SAT ainda não ativado. Efetuar ativação.";
                case "06003": return "SAT não vinculado ao AC Efetuar vinculação";
                case "06004": return "Vinculação do AC não confere Efetuar vinculação";
                case "06005": return "Tamanho do CF-e-SAT superior a 1.500KB";
                case "06006": return "SAT bloqueado pelo contribuinte";
                case "06007": return "SAT bloqueado pela SEFAZ";
                case "06008": return "SAT bloqueado por falta de comunicação";
                case "06009": return "SAT bloqueado, código de ativação incorreto";
                case "06010": return "Erro de validação do conteúdo.";
                case "06098": return "SAT em processamento.";
                case "06099": return "Erro desconhecido na emissão. Informar o administrador.";
                case "07000": return "Cupom cancelado com sucesso + conteúdo CF-eSAT cancelado.";
                case "07001": return "Código ativação inválido Verificar o código e tentar mais uma vez.";
                case "07002": return "Cupom inválido Informar o administrador.";
                case "07003": return "SAT bloqueado pelo contribuinte";
                case "07004": return "SAT bloqueado pela SEFAZ";
                case "07005": return "SAT bloqueado por falta de comunicação";
                case "07006": return "SAT bloqueado, código de ativação incorreto";
                case "07007": return "Erro de validação do conteúdo";
                case "07098": return "SAT em processamento.";
                case "07099": return "Erro desconhecido no cancelamento.";
                case "08000": return "SAT em operação. Verifica se o SAT está ativo.";
                case "08098": return "SAT em processamento.";
                case "08099": return "Erro desconhecido. Informar o administrador.";
                case "09000": return "Emitido com sucesso Gera e envia um cupom de teste para SEFAZ, para verificar a comunicação.";
                case "09001": return "código ativação inválido Verificar o código e tentar mais uma vez.";
                case "09002": return "SAT ainda não ativado. Efetuar ativação ";
                case "09098": return "SAT em processamento.";
                case "09099": return "Erro desconhecido Informar o ";
                case "10000": return "Resposta com Sucesso. Informações de status do SAT.";
                case "10001": return "Código de ativação inválido";
                case "10098": return "SAT em processamento.";
                case "10099": return "Erro desconhecido Informar o administrador.";
                case "11000": return "Emitido com sucesso Retorna o conteúdo do CF-ao AC.";
                case "11001": return "código ativação inválido Verificar o código e tentar mais uma vez.";
                case "11002": return "SAT ainda não ativado. Efetuar ativação.";
                case "11003": return "Sessão não existe. AC deve executar a sessão novamente.";
                case "11098": return "SAT em processamento.";
                case "11099": return "Erro desconhecido. Informar o administrador.";
                case "12000": return "Rede Configurada com Sucesso";
                case "12001": return "código ativação inválido Verificar o código e tentar mais uma vez.";
                case "12002": return "Dados fora do padrão a ser informado Corrigir dados";
                case "12098": return "SAT em processamento.";
                case "12099": return "Erro desconhecido Informar o administrador.";
                case "13000": return "Assinatura do AC";
                case "13001": return "código ativação inválido Verificar o código e tentar mais uma vez.";
                case "13002": return "Erro de comunicação com a SEFAZ";
                case "13003": return "Assinatura fora do padrão informado Corrigir dados";
                case "13004": return "CNPJ da Software House + CNPJ do emitente assinado no campo “signAC” difere do informado no campo “CNPJvalue” Corrigir dados";
                case "13098": return "SAT em processamento.";
                case "13099": return "Erro desconhecido Informar o administrador.";
                case "14000": return "Software Atualizado com Sucesso ";
                case "14001": return "Código de ativação inválido.";
                case "14002": return "Atualização em Andamento";
                case "14003": return "Erro na atualização Não foi possível Atualizar o SAT.";
                case "14004": return "Arquivo de atualização inválido";
                case "14098": return "SAT em processamento.";
                case "14099": return "Erro desconhecido Informar o administrador.";
                case "15000": return "Transferência completa Arquivos de Logs extraídos";
                case "15001": return "Código de ativação inválido.";
                case "15002": return "Transferência em andamento";
                case "15098": return "SAT em processamento.";
                case "15099": return "Erro desconhecido Informar o administrador.";
                case "16000": return "Equipamento SAT bloqueado com sucesso.";
                case "16001": return "Código de ativação inválido.";
                case "16002": return "Equipamento SAT já está bloqueado.";
                case "16003": return "Erro de comunicação com a SEFAZ";
                case "16004": return "Não existe parametrização de bloqueio disponível.";
                case "16098": return "SAT em processamento.";
                case "16099": return "Erro desconhecido Informar o administrador.";
                case "17000": return "Equipamento SAT desbloqueado com sucesso.";
                case "17001": return "Código de ativação inválido.";
                case "17002": return "SAT bloqueado pelo contribuinte. Verifique configurações na SEFAZ";
                case "17003": return "SAT bloqueado pela SEFAZ";
                case "17004": return "Erro de comunicação com a SEFAZ";
                case "17098": return "SAT em processamento.";
                case "17099": return "Erro desconhecido Informar o administrador.";
                case "18000": return "Código de ativação alterado com sucesso.";
                case "18001": return "Código de ativação inválido.";
                case "18002": return "Código de ativação de emergência Incorreto.";
                case "18098": return "SAT em processamento.";
                case "18099": return "Erro desconhecido Informar o administrador.";
                default: return "";
            }
        }

        private string RetornaMensagemRejeicaoSAT(string pCodigo)
        {
            switch (pCodigo)
            {
                case "100": return "CF-e-SAT processado com sucesso";
                case "101": return "CF-e-SAT de cancelamento processado com sucesso";
                case "102": return "CF-e-SAT processado – verificar inconsistências";
                case "103": return "CF-e-SAT de cancelamento processado – verificar inconsistências";
                case "104": return "Não Existe Atualização do Software";
                case "105": return "Lote recebido com sucesso";
                case "106": return "Lote Processado";
                case "107": return "Lote em Processamento";
                case "108": return "Lote não localizado";
                case "109": return "Serviço em Operação";
                case "110": return "Status SAT recebido com sucesso";
                case "112": return "Assinatura do AC Registrada";
                case "113": return "Consulta cadastro com uma ocorrência";
                case "114": return "Consulta cadastro com mais de uma ocorrência";
                case "115": return "Solicitação de dados efetuada com sucesso";
                case "116": return "Atualização do SB pendente";
                case "117": return "Solicitação de Arquivo de Parametrização efetuada com sucesso";
                case "118": return "Logs extraídos com sucesso";
                case "119": return "Comandos da SEFAZ pendentes";
                case "120": return "Não existem comandos da SEFAZ pendentes";
                case "121": return "Certificado Digital criado com sucesso";
                case "122": return "CRT recebido com sucesso";
                case "123": return "Adiar transmissão do lote";
                case "124": return "Adiar transmissão do CF-e";
                case "125": return "CF-e de teste de produção emitido com sucesso";
                case "126": return "CF-e de teste de ativação emitido com sucesso";
                case "127": return "Erro na emissão de CF-e de teste de produção";
                case "128": return "Erro na emissão de CF-e de teste de ativação";
                case "129": return "Solicitações de emissão de certificados excedidas. (Somente ocorrerá no ambiente de testes)";
                case "200": return "Rejeição: Status do equipamento SAT difere do esperado";
                case "201": return "Rejeição: Falha na Verificação da Assinatura do Número de segurança";
                case "202": return "Rejeição: Falha no reconhecimento da autoria ou integridade do arquivo digital";
                case "203": return "Rejeição: Emissor não Autorizado para emissão da CF-e-SAT";
                case "204": return "Rejeição: Duplicidade de CF-e-SAT";
                case "205": return "Rejeição: Equipamento SAT encontra-se Ativo";
                case "206": return "Rejeição: Hora de Emissão do CF-e-SAT posterior à hora de recebimento.";
                case "207": return "Rejeição: CNPJ do emitente inválido";
                case "208": return "Rejeição: Equipamento SAT encontra-se Desativado";
                case "209": return "Rejeição: IE do emitente inválida";
                case "210": return "Rejeição: Intervalo de tempo entre o CF-e-SAT emitido e a emissão do respectivo CF-e-SAT de cancelamento é maior que 30 (trinta) minutos.";
                case "211": return "Rejeição: CNPJ não corresponde ao informado no processo de transferência.";
                case "212": return "Rejeição: Data de Emissão do CF-e-SAT posterior à data de recebimento.";
                case "213": return "Rejeição: CNPJ-Base do Emitente difere do CNPJ-Base do Certificado Digital";
                case "214": return "Rejeição: Tamanho da mensagem excedeu o limite estabelecido";
                case "215": return "Rejeição: Falha no schema XML";
                case "216": return "Rejeição: Chave de Acesso difere da cadastrada";
                case "217": return "Rejeição: CF-e-SAT não consta na base de dados da SEFAZ";
                case "218": return "Rejeição: CF-e-SAT já esta cancelado na base de dados da SEFAZ";
                case "219": return "Rejeição: CNPJ não corresponde ao informado no processo de declaração de posse.";
                case "220": return "Rejeição: Valor do rateio do desconto sobre subtotal do item (N) inválido.";
                case "221": return "Rejeição: Aplicativo Comercial não vinculado ao SAT";
                case "222": return "Rejeição: Assinatura do Aplicativo Comercial inválida";
                case "223": return "Rejeição: CNPJ do transmissor do lote difere do CNPJ do transmissor da consulta";
                case "224": return "Rejeição: CNPJ da Software House inválido";
                case "225": return "Rejeição: Falha no Schema XML do lote de CFe";
                case "226": return "Rejeição: Código da UF do Emitente diverge da UF receptora";
                case "227": return "Rejeição: Erro na Chave de Acesso - Campo Id – falta a literal CFe";
                case "228": return "Rejeição: Valor do rateio do acréscimo sobre subtotal do item (N) inválido.";
                case "229": return "Rejeição: IE do emitente não informada";
                case "230": return "Rejeição: IE do emitente não autorizada para uso do SAT";
                case "231": return "Rejeição: IE do emitente não vinculada ao CNPJ";
                case "232": return "Rejeição: CNPJ do destinatário do CF-e-SAT de cancelamento diferente daquele do CF-e-SAT a ser cancelado.";
                case "233": return "Rejeição: CPF do destinatário do CF-e-SAT de cancelamento diferente daquele do CF-e-SAT a ser cancelado.";
                case "234": return "Alerta: Razão Social/Nome do destinatário em branco";
                case "235": return "Rejeição: CNPJ do destinatario Invalido";
                case "236": return "Rejeição: Chave de Acesso com dígito verificador inválido";
                case "237": return "Rejeição: CPF do destinatario Invalido";
                case "238": return "Rejeição: CNPJ do emitente do CF-e-SAT de cancelamento diferente do CNPJ do CF-e-SAT a ser cancelado.";
                case "239": return "Rejeição: Versão do arquivo XML não suportada";
                case "240": return "Rejeição: Valor total do CF-e-SAT de cancelamento diferente do Valor total do CF-e-SAT a ser cancelado.";
                case "241": return "Rejeição: diferença de transmissão e recebimento da mensagem superior a 5 minutos.";
                case "242": return "Alerta: CFe dentro do lote estão fora de ordem.";
                case "243": return "Rejeição: XML Mal Formado";
                case "244": return "Rejeição: CNPJ do Certificado Digital difere do CNPJ da Matriz e do CNPJ do Emitente";
                case "245": return "Rejeição: CNPJ Emitente não autorizado para uso do SAT";
                case "246": return "Rejeição: Campo cUF inexistente no elemento cfeCabecMsg do SOAP Header";
                case "247": return "Rejeição: Sigla da UF do Emitente diverge da UF receptora";
                case "248": return "Rejeição: UF do Recibo diverge da UF autorizadora";
                case "249": return "Rejeição: UF da Chave de Acesso diverge da UF receptora";
                case "250": return "Rejeição: UF informada pelo SAT, não é atendida pelo Web Service";
                case "251": return "Rejeição: Certificado enviado não confere com o escolhido na declaração de posse";
                case "252": return "Rejeição: Ambiente informado diverge do Ambiente de recebimento";
                case "253": return "Rejeição: Digito Verificador da chave de acesso composta inválida";
                case "254": return "Rejeição: Elemento cfeCabecMsg inexistente no SOAP Header";
                case "255": return "Rejeição: CSR enviado inválido";
                case "256": return "Rejeição: CRT enviado inválido";
                case "257": return "Rejeição: Número do série do equipamento inválido";
                case "258": return "Rejeição: Data e/ou hora do envio inválida";
                case "259": return "Rejeição: Versão do leiaute inválida";
                case "260": return "Rejeição: UF inexistente";
                case "261": return "Rejeição: Assinatura digital não encontrada";
                case "262": return "Rejeição: CNPJ da software house não está ativo";
                case "263": return "Rejeição: CNPJ do contribuinte não está ativo";
                case "264": return "Rejeição: Base da receita federal está indisponível";
                case "265": return "Rejeição: Número de série inexistente no cadastro do equipamento";
                case "266": return "Falha na comunicação com a AC-SAT";
                case "267": return "Erro desconhecido na geração do certificado pela AC-SAT";
                case "268": return "Rejeição: Certificado está fora da data de validade.";
                case "269": return "Rejeição: Tipo de atividade inválida";
                case "270": return "Rejeição: Chave de acesso do CFe a ser cancelado inválido.";
                case "271": return "Rejeição: Ambiente informado no CF-e difere do Ambiente de recebimento cadastrado.";
                case "272": return "Rejeição: Valor do troco negativo.";
                case "273": return "Rejeição: Serviço Solicitado Inválido";
                case "274": return "Rejeição: Equipamento não possui declaração de posse";
                case "275": return "Rejeição: Status do equipamento diferente de Fabricado";
                case "276": return "Rejeição: Diferença de dias entre a data de emissão e de recepção maior que o prazo legal";
                case "277": return "Rejeição: CNPJ do emitente não está ativo junto à Sefaz na data de emissão";
                case "278": return "Rejeição: IE do emitente não está ativa junto à Sefaz na data de emissão";
                case "280": return "Rejeição: Certificado Transmissor Inválido";
                case "281": return "Rejeição: Certificado Transmissor Data Validade";
                case "282": return "Rejeição: Certificado Transmissor sem CNPJ";
                case "283": return "Rejeição: Certificado Transmissor - erro Cadeia de Certificação";
                case "284": return "Rejeição: Certificado Transmissor revogado";
                case "285": return "Rejeição: Certificado Transmissor difere ICP-Brasil";
                case "286": return "Rejeição: Certificado Transmissor erro no acesso a LCR";
                case "287": return "Rejeição: Código Município do FG - ISSQN: dígito inválido. Exceto os códigos descritos no Anexo 2 que apresentam dígito inválido.";
                case "288": return "Rejeição: Data de emissão do CF-e-SAT a ser cancelado inválida";
                case "289": return "Rejeição: Código da UF informada diverge da UF solicitada";
                case "290": return "Rejeição: Certificado Assinatura inválido";
                case "291": return "Rejeição: Certificado Assinatura Data Validade";
                case "292": return "Rejeição: Certificado Assinatura sem CNPJ";
                case "293": return "Rejeição: Certificado Assinatura - erro Cadeia de Certificação";
                case "294": return "Rejeição: Certificado Assinatura revogado";
                case "295": return "Rejeição: Certificado Raiz difere dos Válidos";
                case "296": return "Rejeição: Certificado Assinatura erro no acesso a LCR";
                case "297": return "Rejeição: Assinatura difere do calculado";
                case "298": return "Rejeição: Assinatura difere do padrão do Projeto";
                case "299": return "Rejeição: Hora de emissão do CF-e-SAT a ser cancelado inválida";
                case "402": return "Rejeição: XML da área de dados com codificação diferente de UTF-8";
                case "403": return "Rejeição: Versão do leiaute do CF-e-SAT não é válida";
                case "404": return "Rejeição: Uso de prefixo de namespace não permitido";
                case "405": return "Alerta: Versão do leiaute do CF-e-SAT não é a mais atual";
                case "406": return "Rejeição: Versão do Software Básico do SAT não é valida.";
                case "407": return "Rejeição: Indicador de CF-e-SAT cancelamento inválido (diferente de „C? e „?)";
                case "408": return "Rejeição: Valor total do CF-e-SAT maior que o somatório dos valores de Meio de Pagamento empregados em seu pagamento.";
                case "409": return "Rejeição: Valor total do CF-e-SAT supera o máximo permitido no arquivo de Parametrização de Uso";
                case "410": return "Rejeição: UF informada no campo cUF não é atendida pelo Web Service";
                case "411": return "Rejeição: Campo versaoDados inexistente no elemento cfeCabecMsg do SOAP Header";
                case "412": return "Rejeição: CFe de cancelamento não corresponde ao CFe anteriormente gerado";
                case "420": return "Rejeição: Cancelamento para CF-e-SAT já cancelado";
                case "450": return "Rejeição: Modelo da CF-e-SAT diferente de 59";
                case "452": return "Rejeição: número de série do SAT inválido ou não autorizado.";
                case "453": return "Rejeição: Ambiente de processamento inválido (diferente de 1 e 2)";
                case "454": return "Rejeição: CNPJ da Software House inválido";
                case "455": return "Rejeição: Assinatura do Aplicativo Comercial não é válida.";
                case "456": return "Rejeição: Código de Regime tributário invalido";
                case "457": return "Rejeição: Código de Natureza da Operação para ISSQN inválido";
                case "458": return "Rejeição: Razão Social/Nome do destinatário em branco";
                case "459": return "Rejeição: Código do produto ou serviço em branco";
                case "460": return "Rejeição: GTIN do item (N) inválido";
                case "461": return "Rejeição: Descrição do produto ou serviço em branco";
                case "462": return "Rejeição: CFOP não é de operação de saída prevista para CF-e-SAT";
                case "463": return "Rejeição: Unidade comercial do produto ou serviço em branco";
                case "464": return "Rejeição: Quantidade Comercial do item (N) inválido";
                case "465": return "Rejeição: Valor unitário do item (N) inválido";
                case "466": return "Rejeição: Valor bruto do item (N) difere de quantidade * Valor Unitário, considerando regra de arred/trunc.";
                case "467": return "Rejeição: Regra de calculo do item (N) inválida";
                case "468": return "Rejeição: Valor do desconto do item (N) inválido";
                case "469": return "Rejeição: Valor de outras despesas acessórias do item (N) inválido.";
                case "470": return "Rejeição: Valor líquido do Item do CF-e difere de Valor Bruto de Produtos e Serviços - desconto + Outras Despesas Acessórias – rateio do desconto sobre subtotal + rateio do acréscimo sobre subtotal ";
                case "471": return "Rejeição: origem da mercadoria do item (N) inválido (difere de 0, 1, 2, 3, 4, 5, 6 e 7)";
                case "472": return "Rejeição: CST do Item (N) inválido (diferente de 00, 20, 90)";
                case "473": return "Rejeição: Alíquota efetiva do ICMS do item (N) inválido.";
                case "474": return "Rejeição: Valor líquido do ICMS do Item (N) difere de Valor do Item * Aliquota Efetiva";
                case "475": return "Rejeição: CST do Item (N) inválido (diferente de 40 e 41 e 50 e 60)";
                case "476": return "Rejeição: Código de situação da operação - Simples Nacional - do Item (N) inválido (diferente de 102, 300 e 500)";
                case "477": return "Rejeição: Código de situação da operação - Simples Nacional - do Item (N) inválido (diferente de 900)";
                case "478": return "Rejeição: Código de Situação Tributária do PIS Inválido (diferente de 01 e 02)";
                case "479": return "Rejeição: Base de cálculo do PIS do item (N) inválido.";
                case "480": return "Rejeição: Alíquota do PIS do item (N) inválido.";
                case "481": return "Rejeição: Valor do PIS do Item (N) difere de Base de Calculo * Aliquota do PIS";
                case "482": return "Rejeição: Código de Situação Tributária do PIS Inválido (diferente de 03)";
                case "483": return "Rejeição: Qtde Vendida do item (N) inválido.";
                case "484": return "Rejeição: Alíquota do PIS em R$ do item (N) inválido.";
                case "485": return "Rejeição: Valor do PIS do Item (N) difere de Qtde Vendida* Aliquota do PIS em R$";
                case "486": return "Rejeição: Código de Situação Tributária do PIS Inválido (diferente de 04, 06, 07, 08 e 09)";
                case "487": return "Rejeição: Código de Situação Tributária do PIS inválido (diferente de 49)";
                case "488": return "Rejeição: Código de Situação Tributária do PIS Inválido (diferente de 99)";
                case "489": return "Rejeição: Valor do PIS do Item (N) difere de Qtde Vendida* Aliquota do PIS em R$ e difere de Base de Calculo * Aliquota do PIS";
                case "490": return "Rejeição: Código de Situação Tributária da COFINS Inválido (diferente de 01 e 02)";
                case "491": return "Rejeição: Base de cálculo do COFINS do item (N) inválido.";
                case "492": return "Rejeição: Alíquota da COFINS do item (N) inválido.";
                case "493": return "Rejeição: Valor da COFINS do Item (N) difere de Base de Calculo * Aliquota da COFINS";
                case "494": return "Rejeição: Código de Situação Tributária da COFINS Inválido (diferente de 03)";
                case "495": return "Rejeição: Valor do COFINS do Item (N) difere de Qtde Vendida* Aliquota do COFINS em R$ e difere de Base de Calculo * Aliquota do COFINS";
                case "496": return "Rejeição: Alíquota da COFINS em R$ do item (N) inválido.";
                case "497": return "Rejeição: Valor da COFINS do Item (N) difere de Qtde Vendida* Aliquota da COFINS em R$";
                case "498": return "Rejeição: Código de Situação Tributária da COFINS Inválido (diferente de 04, 06, 07, 08 e 09)";
                case "499": return "Rejeição: Código de Situação Tributária da COFINS Inválido (diferente de 49)";
                case "500": return "Rejeição: Código de Situação Tributária da COFINS Inválido (diferente de 99)";
                case "501": return "Rejeição: Operação com tributação de ISSQN sem informar a Inscrição Municipal";
                case "502": return "Rejeição: Erro na Chave de Acesso - Campo Id não corresponde à concatenação dos campos correspondentes";
                case "503": return "Rejeição: Valor das deduções para o ISSQN do item (N) inválido.";
                case "504": return "Rejeição: Valor da Base de Calculo do ISSQN do Item (N) difere de Valor do Item - Valor das deduções";
                case "505": return "Rejeição: Alíquota efetiva do ISSQN do item (N) não é maior ou igual a 2,00 (2%) e menor ou igual a 5,00 (5%).";
                case "506": return "Valor do ISSQN do Item (N) difere de Valor da Base de Calculo do ISSQN * Alíquota Efetiva do ISSQN";
                case "507": return "Rejeição: Indicador de rateio para ISSQN inválido";
                case "508": return "Rejeição: Item da lista de Serviços do ISSQN do item (N) inválido.";
                case "509": return "Rejeição: Código municipal de Tributação do ISSQN do Item (N) em branco.";
                case "510": return "Rejeição: Código de Natureza da Operação para ISSQN inválido";
                case "511": return "Rejeição: Indicador de Incentivo Fiscal do ISSQN do item (N) inválido (diferente de 1 e 2)";
                case "512": return "Rejeição: Total do PIS difere do somatório do PIS dos itens";
                case "513": return "Rejeição: Total do COFINS difere do somatório do COFINS dos itens";
                case "514": return "Rejeição: Total do PIS-ST difere do somatório do PIS-ST dos itens";
                case "515": return "Rejeição: Total do COFINS-STdifere do somatório do COFINS-ST dos itens";
                case "516": return "Rejeição: Total de Outras Despesas Acessórias difere do somatório de Outras Despesas Acessórias (acréscimo) dos itens";
                case "517": return "Rejeição: Total dos Itens difere do somatório do valor líquido dos itens";
                case "518": return "Rejeição: Informado grupo de totais do ISSQN sem informar grupo de valores de ISSQN";
                case "519": return "Rejeição: Total da BC do ISSQN difere do somatório da BC do ISSQN dos itens";
                case "520": return "Rejeição: Total do ISSQN difere do somatório do ISSQN dos itens";
                case "521": return "Rejeição: Total do PIS sobre serviços difere do somatório do PIS dos itens de serviços";
                case "522": return "Rejeição: Total do COFINS sobre serviços difere do somatório do COFINS dos itens de serviços";
                case "523": return "Rejeição: Total do PIS-ST sobre serviços difere do somatório do PIS-ST dos itens de serviços";
                case "524": return "Rejeição: Total do COFINS-ST sobre serviços difere do somatório do COFINS-ST dos itens de serviços";
                case "525": return "Rejeição: Valor de Desconto sobre total inválido.";
                case "526": return "Rejeição: Valor de Acréscimo sobre total inválido.";
                case "527": return "Rejeição: Código do Meio de Pagamento inválido";
                case "528": return "Rejeição: Valor do Meio de Pagamento inválido.";
                case "529": return "Rejeição: Valor de desconto sobre subtotal difere do somatório dos seus rateios nos itens.";
                case "530": return "Rejeição: Operação com tributação de ISSQN sem informar a Inscrição Municipal";
                case "531": return "Rejeição: Valor de acréscimo sobre subtotal difere do somatório dos seus rateios nos itens.";
                case "532": return "Rejeição: Total do ICMS difere do somatório dos itens";
                case "533": return "Rejeição: Valor aproximado dos tributos do CF-e-SAT – Lei 12741/12 inválido";
                case "534": return "Rejeição: Valor aproximado dos tributos do Produto ou serviço – Lei 12741/12 inválido.";
                case "535": return "Rejeição: código da credenciadora de cartão de débito ou crédito inválido";
                case "537": return "Rejeição: Total do Desconto difere do somatório dos itens";
                case "539": return "Rejeição: Duplicidade de CF-e-SAT, com diferença na Chave de Acesso [99999999999999999999999999999999999999999]";
                case "540": return "Rejeição: CNPJ da Software House + CNPJ do emitente assinado no campo “signAC” difere do informado no campo “CNPJvalue” ";
                case "555": return "Rejeição: Tipo autorizador do protocolo diverge do Órgão Autorizador";
                case "564": return "Rejeição: Total dos Produtos ou Serviços difere do somatório do valor dos Produtos ou Serviços dos itens";
                case "600": return "Serviço Temporariamente Indisponível";
                case "601": return "CF-e-SAT inidôneo por recepção fora do prazo";
                case "602": return "Rejeição: Status do equipamento não permite ativação";
                case "603": return "Arquivo inválido";
                case "604": return "Erro desconhecido na verificação de comandos";
                case "605": return "Tamanho do arquivo inválido";
                case "999": return "Rejeição: Erro não catalogado";
                default: return "Rejeção não catalogada na nota técnica 2013/001.";
            }
        }

        private void RegistrarRespostaSat(string xmlEnvio)
        {
            var cupomSat = new Entity.CupomFiscalSat
            {
                CodigoVenda = _venda.CodigoVenda,
                CpfCnpj = _venda.SatResponse.CpfCnpj,
                DataOperacao = DateTime.Now,
                ErrorCode = _venda.SatResponse.ErrorCode,
                ErrorCode2 = _venda.SatResponse.ErrorCode2,
                ErrorMessage = _venda.SatResponse.ErrorMessage,
                InvoiceKey = _venda.SatResponse.InvoiceKey,
                QrCodeSignature = _venda.SatResponse.QrCodeSignature,
                SessionCode = _venda.SatResponse.SessionCode.ToString(),
                TimeStamp = _venda.SatResponse.TimeStamp,
                Total = _venda.SatResponse.Total,
                Xml = _venda.SatResponse.Xml,
                CodigoSat = _parametros.NumeroSat,
                XmlEnvio = xmlEnvio
            };

            _repositorio.Inserir(cupomSat);
        }
    }
}
