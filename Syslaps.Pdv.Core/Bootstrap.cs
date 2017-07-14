using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Syslaps.Pdv.Core.Dominio.Base;
using System.Linq;
using Syslaps.Pdv.Core.Dominio;
using Syslaps.Pdv.Core.Dominio.Produto;
using Syslaps.Pdv.Core.Dominio.Usuario;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;
using Produto = Syslaps.Pdv.Entity.Produto;

namespace Syslaps.Pdv.Core
{
    public class Bootstrap : ModeloBase
    {
        public Action<int, int, string> ProdutoImportadoHandler;
        public Action<string> EtapaHandler;
        public Action<Exception> ImportacaoConcluidaHandler;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly IRepositorioBase _repositorio;
        private readonly IRepositorioCriarBaseDeDados _repositorioCriarBaseDeDados;
        private readonly IInfraLogger _logger;
        private List<ConfiguracaoCategoriaProduto> _listaCategoriaConfiguracao;
        private int _codigoDoCumpom = 1;

        private string CodigoDoCupom => (_codigoDoCumpom++).ToString().PadLeft(4, '0');

        public Bootstrap(IUsuarioRepositorio usuarioRepositorio, IProdutoRepositorio produtoRepositorio, IRepositorioBase repositorio, IRepositorioCriarBaseDeDados repositorioCriarBaseDeDados, IInfraLogger logger)
        {
            this._usuarioRepositorio = usuarioRepositorio;
            this._produtoRepositorio = produtoRepositorio;
            this._repositorio = repositorio;
            _repositorioCriarBaseDeDados = repositorioCriarBaseDeDados;
            _logger = logger;
        }

        public void IniciarProcessoCompleto(bool excluirTabelas = true)
        {
            EtapaHandler?.Invoke("#### Processo Completo Iniciado ####");
            if (excluirTabelas)
            {
                ExcluirTabelas();
                EtapaHandler?.Invoke("Tabelas existentes Excluídas...");
            }
            CriarBancoDeDados();
            EtapaHandler?.Invoke("Criação do Banco de dados Concluída...");
            EtapaHandler?.Invoke("Inciar criação dos dados base...");
            CriarDadosIniciais();
            ImportarProdutos();
            _repositorioCriarBaseDeDados.Vacuum();
            EtapaHandler?.Invoke("Base limpa e pronta para operação...");
            EtapaHandler?.Invoke("#### Processo Completo Finalizado ####");
        }

        public void ExcluirTabelas()
        {
            _repositorioCriarBaseDeDados.ExcluirTabelas();
        }

        public void CriarBancoDeDados()
        {
            _repositorioCriarBaseDeDados.CriarDataBase();
        }

        public void CriarDadosIniciais()
        {
            // criar usuarios
            var usuario = new Dominio.Usuario.Usuario(_usuarioRepositorio);
            usuario.RegistrarNovoUsuario("admin", "123", EnumTipoUsuario.Administrador);
            usuario.RegistrarNovoUsuario("admin2", "123", EnumTipoUsuario.Administrador);
            EtapaHandler?.Invoke("Usuários criados...");
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "receiver.email", Valor = "usuario@gmail.com" });
#if !DEBUG
            _repositorio.Inserir<Parametro>(new Parametro() {Nome = "receiver.email", Valor = "debug@gmail.com"});
#endif

            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "smtp", Valor = "smtp-mail.outlook.com" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "smtp.port", Valor = "587" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "smtp.user", Valor = "loja@outlook.com" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "smtp.password", Valor = "senha123" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "smtp.sender.email", Valor = "loja@outlook.com" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "smtp.sender.name", Valor = "Bolaria" });

            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv.message.title", Valor = "Pdv - SAT" });

            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.nome", Valor = "Loja" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.cnpj", Valor = "11.111.111/1111-11" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.ie", Valor = "111.111.111.111" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.endereco", Valor = "Av. Avenida, 628" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.numero", Valor = "22775" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.bairro", Valor = "Bairro" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.cidade", Valor = "City - SP" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.telefone", Valor = "(11) 1111-11111 - Whats (11) 11111-1111" });

            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.nomefatansia", Valor = "Nome Fantasia" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.empresa.im", Valor = "123456789" });

            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "sat.signac", Valor = "sdafsdfasdf6876asdfsadifsa87df68asd787das68f76asd8f" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "sat.sh.cnpj", Valor = "1111111111111111111" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "sat.numero", Valor = "111111" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "sat.modelo", Valor = "OffLine" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "sat.habilitado", Valor = "nao" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "sat.codigo", Valor = "12345678" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.numcaixa", Valor = "001" });
            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "pdv1.gaveta.automatica", Valor = "sim" });

            _repositorio.Inserir<Parametro>(new Parametro() { Nome = "cfop.tributos", Valor = "16,20" });


            EtapaHandler?.Invoke("Parametros criados...");
            _repositorio.Inserir<TipoPagamento>(new TipoPagamento { CodigoTipoPagamento = GerarCodigoUnico(), Nome = "Dinheiro", DiasParaPagamento = 0, PercentualDesconto = 0 });
            _repositorio.Inserir<TipoPagamento>(new TipoPagamento { CodigoTipoPagamento = GerarCodigoUnico(), Nome = "Débito Rede", DiasParaPagamento = 1, PercentualDesconto = 2.48m });
            _repositorio.Inserir<TipoPagamento>(new TipoPagamento { CodigoTipoPagamento = GerarCodigoUnico(), Nome = "Débito Moderninha", DiasParaPagamento = 1, PercentualDesconto = 2.39m });
            _repositorio.Inserir<TipoPagamento>(new TipoPagamento { CodigoTipoPagamento = GerarCodigoUnico(), Nome = "Crédito Rede", DiasParaPagamento = 30, PercentualDesconto = 3.78m });
            _repositorio.Inserir<TipoPagamento>(new TipoPagamento { CodigoTipoPagamento = GerarCodigoUnico(), Nome = "Crédito Moderninha", DiasParaPagamento = 30, PercentualDesconto = 3.20m });
            _repositorio.Inserir<TipoPagamento>(new TipoPagamento { CodigoTipoPagamento = GerarCodigoUnico(), Nome = "Tiket", DiasParaPagamento = 30, PercentualDesconto = 30m });

            EtapaHandler?.Invoke("Tipo de pagamentos criados...");
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Bolos", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Bolos Fatia", TemProducao = false, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Maquina de Café", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Sobremesas", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Coberturas", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Café de Coador", TemProducao = false, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Cha", TemProducao = false, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Bolos Salgado", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Bolos Naked", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Escondidinho", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Tapioca", TemProducao = false, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Bolo Inteiro Pequeno", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Bolos Pote", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Brownie", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Cookies", TemProducao = true, DescontaInsumo = true });
            _repositorio.Inserir<ConfiguracaoCategoriaProduto>(new ConfiguracaoCategoriaProduto { Categoria = "Geladinho", TemProducao = true, DescontaInsumo = true });
            EtapaHandler?.Invoke("Categorias dos produtos criados...");
        }

        public void ImportarProdutos()
        {
            try
            {
                _logger.Log().Debug("importação iniciada...");
                _listaCategoriaConfiguracao = _repositorio.RecuperarTodos<ConfiguracaoCategoriaProduto>();
                _logger.Log().Debug("lista de categosrias carregada...");
                EtapaHandler?.Invoke("Lista de categorias de carregada...");
                var caminhoFisicoDaPlanilha = "CaminhoFisicoDaPlanilhaDeProdutos".GetConfigValue();
                var connectionString =
                    $"Provider=Microsoft.ACE.OLEDB.12.0; data source={caminhoFisicoDaPlanilha}; Extended Properties=Excel 8.0;";
                var adapter = new OleDbDataAdapter("SELECT * FROM [temp$]", connectionString);
                var ds = new DataSet();
                adapter.Fill(ds, "produtos");
                var data = ds.Tables["produtos"].AsEnumerable();
                _logger.Log().Debug("planilha dos produtos carregada...");
                EtapaHandler?.Invoke("Planilha dos produtos carregada...");
                EtapaHandler?.Invoke("Importando produtos...");
                var produtosDaPlanilha = data.Select(x =>
                {
                    var produto = new Produto();
                    produto.CodigoParaCupom = CodigoDoCupom;
                    produto.Ativo = x.Field<string>("ATIVO").Trim().SimNaoToBool();
                    produto.CodigoDeBarra = x.IsNull("CÓDIGO DE BARRAS")
                        ? null
                        : x.Field<string>("CÓDIGO DE BARRAS").Trim();
                    produto.CodigoNCM = x.IsNull("NCM") ? null : x.Field<string>("NCM").Trim();
                    produto.Descricao = x.Field<string>("DESCRIÇÃO").Trim();
                    produto.DigitoCST = x.Field<string>("DÍGITO CST").Trim();
                    produto.EstoqueMinimo = x.IsNull("ESTOQUE MÍNIMO")
                        ? null
                        : x.Field<double>("ESTOQUE MÍNIMO").ToLong();
                    produto.ExibirNoPdv = x.Field<string>("PDV").Trim().SimNaoToBool();
                    produto.Marca = x.Field<string>("MARCA").Trim();
                    produto.Modelo = x.Field<string>("MODELO").Trim();
                    produto.PrecoCusto = x.Field<double>("PREÇO DE CUSTO").ToDecimal();
                    produto.PrecoVenda = x.Field<double>("PREÇO DE VENDA").ToDecimal();
                    produto.PrecoVenda2 = x.Field<double>("PREÇO DE VENDA 2").ToDecimal();
                    produto.TipoFiscal = x.Field<string>("TIPO1").Trim();
                    produto.TipoProduto = x.Field<string>("TIPO").Trim();
                    produto.TipoUnidade = x.Field<string>("UNIDADE").Trim();
                    produto.ControlarEstoque = produto.EstoqueMinimo != null;
                    produto.Categoria = x.Field<string>("CATEGORIA").Trim();
                    produto.SubCategoria = x.Field<string>("SUBCATEGORIA").Trim();
                    produto.DescricaoBusca =
                        string.Concat(produto.CodigoDeBarra, produto.Descricao).ToComparableString();

                    AtribuirDescontarInsumoNaVendaETemProducao(produto);
                    return produto;
                }).ToList();

                var count = 1;
                produtosDaPlanilha.ForEach(produto =>
                {
                    if (string.IsNullOrEmpty(produto.CodigoDeBarra))
                    {
                        AdicionarMensagem(
                            $"O produto {produto.Descricao} não foi importado, pois não possui código de barras.",
                            EnumStatusDoResultado.RegraDeNegocioInvalida);
                    }
                    else
                    {
                        try
                        {
                            var prodDb =
                                _produtoRepositorio.Recuperar(new Produto() {CodigoDeBarra = produto.CodigoDeBarra});
                            if (prodDb == null)
                                _produtoRepositorio.Inserir(produto);
                            else
                            {
                                _produtoRepositorio.Atualizar(produto);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Log().Error(ex);
                        }
                    }
                    ProdutoImportadoHandler?.Invoke(count, produtosDaPlanilha.Count, produto.Descricao);
                    _logger.Log().Debug($"{count}/{produtosDaPlanilha.Count} - {produto.Descricao}");
                    count++;
                });


                AdicionarMensagem("Produtos Importados com Sucesso");
                _logger.Log().Debug("####### Produtos Importados com Sucesso #######");
                ImportacaoConcluidaHandler?.Invoke(null);
            }
            catch (Exception ex)
            {
                _logger.Log().Error(ex);
                ImportacaoConcluidaHandler?.Invoke(ex);
            }
        }

        private void AtribuirDescontarInsumoNaVendaETemProducao(Produto produto)
        {
            var categoriaConfiguracao = _listaCategoriaConfiguracao.FirstOrDefault(x => x.Categoria == produto.Categoria);

            if (categoriaConfiguracao != null)
            {
                produto.TemProducao = categoriaConfiguracao.TemProducao;
                produto.DescontarInsumoNaVenda = categoriaConfiguracao.TemProducao;
            }
        }
    }
}
