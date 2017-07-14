using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Core.Dominio.Caixa;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;
using Syslaps.Pdv.Entity.SAT;
using Syslaps.Pdv.Infra.SAT;
using Caixa = Syslaps.Pdv.Core.Dominio.Caixa.Caixa;
using Produto = Syslaps.Pdv.Entity.Produto;
using Usuario = Syslaps.Pdv.Core.Dominio.Usuario.Usuario;

namespace Syslaps.Pdv.UI.Telas.PDV
{
    public class PontoDeVendaMvvm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _filtrarComEsteTexto;
        private List<Produto> _listaDeProdutosDoFiltrada;
        private List<VendaProduto> _listaDeVendaProdutos;
        private List<VendaPagamento> _listaDeVendaPagamentos;
        private bool _podeInicializarVenda;
        private string _numeroComanda;
        private Syslaps.Pdv.Core.Dominio.Comanda.Comanda _comandaDominio;
        private IRepositorioBase _repositorioBase;


        public PontoDeVendaMvvm(Core.Dominio.Comanda.Comanda comandaDominio,IRepositorioBase repositorioBase)
        {
            _comandaDominio = comandaDominio;
            _repositorioBase = repositorioBase;

            UsuarioCorrente = InstanceManager.UsuarioCorrente;
            CaixaCorrente = InstanceManager.CaixaCorrente;
            CaixaCorrente.OnOperacaoExecutada += CaixaCorrente_OnOperacaoExecutada;
            DataHoraCorrente = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Timer timer = new Timer(1000);
            timer.Elapsed += (sender, args) =>
            {
                DataHoraCorrente = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                OnPropertyChanged(nameof(DataHoraCorrente));
            };
            timer.Start();
            _listaDeVendaProdutos = new List<VendaProduto>();
            _listaDeVendaPagamentos = new List<VendaPagamento>();
        }

        private void CaixaCorrente_OnOperacaoExecutada(EnumCaixaTipoOperacao tipoOperacao)
        {

            switch (tipoOperacao)
            {
                case EnumCaixaTipoOperacao.Abertura:
                    {
                        break;
                    }
                case EnumCaixaTipoOperacao.Fechamento:
                    {
                        OnPropertyChanged(nameof(CaixaEstaAberto));
                        OnPropertyChanged(nameof(BtnAbrirFecharCaixaContent));
                        break;
                    }

            }

            if (tipoOperacao == EnumCaixaTipoOperacao.Fechamento)
            {
                PodeInicializarVenda = false;
            }
            else
            {
                PodeInicializarVenda = true;
            }

            OnPropertyChanged(nameof(CaixaCorrente));
            OnPropertyChanged(nameof(CaixaEstaAberto));
            OnPropertyChanged(nameof(BtnAbrirFecharCaixaContent));
        }

        public List<Produto> ListaDeProdutosFiltrada
        {
            get { return _listaDeProdutosDoFiltrada; }
            set
            {
                _listaDeProdutosDoFiltrada = value;
                OnPropertyChanged();
            }
        }

        public List<TipoPagamento> ListaDeTipoPagamentos => InstanceManager.ListaDeTipoPagamentos;

        public List<VendaProduto> ListaDeVendaProdutos
        {
            get { return _listaDeVendaProdutos; }
            set
            {
                _listaDeVendaProdutos = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValorTotalDaVenda));
                OnPropertyChanged(nameof(VendaPossuiProdutos));
            }
        }

        public List<VendaPagamento> ListaDeVendaPagamentos
        {
            get { return _listaDeVendaPagamentos; }
            set
            {
                _listaDeVendaPagamentos = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValorTotalDePagamento));
                OnPropertyChanged(nameof(ValorTotalDeTroco));
                OnPropertyChanged(nameof(PodeFinalizarVenda));
            }
        }

        public decimal ValorTotalDePagamento => _listaDeVendaPagamentos.Sum(x => x.ValorPagamento);

        public decimal ValorTotalDeTroco => VendaCorrente.VendaCorrente.ValorTroco;

        public string FiltrarComEsteTexto
        {
            get { return _filtrarComEsteTexto; }
            set
            {
                _filtrarComEsteTexto = value;
                var count = 1;
                Task.Factory.StartNew(() =>
                {
                    
                    if (value.Length > 0)
                    {
                        ListaDeProdutosFiltrada = InstanceManager.ListaDeProdutosDoPdv.Where(
                                  x => x.DescricaoBusca.Contains(value.ToComparableString())).Select(c => {
                                      c.DescricaoDisplay = string.Empty;
                                      c.DescricaoDisplay = string.Concat(count.ToString().PadLeft(2, ' '), " - ", c.Descricao);
                                      count++;
                                      return c; }).ToList();
                    }
                    else
                    {
                        ListaDeProdutosFiltrada = new List<Produto>();
                    }
                });
            }
        }

        public Usuario UsuarioCorrente { get; private set; }

        public Caixa CaixaCorrente { get; private set; }

        public string DataHoraCorrente { get; private set; }

        public decimal ValorTotalDaVenda
        {
            get
            {
                if (VendaCorrente == null) return 0;
                return VendaCorrente.VendaCorrente.VendaProdutoes.Sum(x => x.ValorTotalVendaProduto);
            }
        }

        public bool CaixaEstaAberto => CaixaCorrente.CaixaCorrente.Situacao == EnumCaixaSituacao.Aberto.ToString();

        public bool PodeInicializarVenda
        {
            get { return _podeInicializarVenda; }
            set { _podeInicializarVenda = value; OnPropertyChanged(); }
        }

        public string NumeroComanda
        {
            get { return _numeroComanda; }
            set { _numeroComanda = value; OnPropertyChanged(); }
        }

        public bool VendaPossuiProdutos => ListaDeVendaProdutos?.Count > 0;

        public bool PodeFinalizarVenda => ValorTotalDePagamento >= ValorTotalDaVenda;

        public string BtnAbrirFecharCaixaContent => $"{(CaixaEstaAberto ? "_Fechar" : "_Abrir")} Caixa";

        public Core.Dominio.Venda.Venda VendaCorrente { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public void AdicionarVendaProduto(Produto produto, decimal quantidade)
        {
            VendaCorrente.AdicionarProdutoNaVenda(produto, quantidade, produto.PrecoVenda);
            ListaDeVendaProdutos = VendaCorrente.VendaCorrente.VendaProdutoes.Reverse().ToList();
        }

        public void AtualizarVendaProduto(VendaProduto vendaProduto, decimal valor, decimal quantidade)
        {
            VendaCorrente.RemoverProdutoDaVenda(vendaProduto);
            VendaCorrente.AdicionarProdutoNaVenda(vendaProduto.Produto, quantidade, valor);
            ListaDeVendaProdutos = VendaCorrente.VendaCorrente.VendaProdutoes.Reverse().ToList();
        }

        public void RemoverVendaProduto(VendaProduto vendaProduto)
        {
            VendaCorrente.RemoverProdutoDaVenda(vendaProduto);
            ListaDeVendaProdutos = VendaCorrente.VendaCorrente.VendaProdutoes.Reverse().ToList();
        }

        public void AdicionarVendaPagamento(TipoPagamento tipoPagamento, decimal valor)
        {
            VendaCorrente.AdicionarPagamento(tipoPagamento, valor);
            ListaDeVendaPagamentos = VendaCorrente.VendaCorrente.VendaPagamentoes.ToList();
        }

        public void RemoverVendaPagamento(VendaPagamento vendaPagamento)
        {
            VendaCorrente.RemoverPagamentoDaVenda(vendaPagamento);
            ListaDeVendaPagamentos = VendaCorrente.VendaCorrente.VendaPagamentoes.ToList();
        }

        public void RegistrarVenda()
        {
            VendaCorrente.RegistrarVenda();

            if (!NumeroComanda.IsNullOrEmpty())
            {
                _comandaDominio.FecharComanda();
            }
        }

        public void RegistrarCupomFiscalSat()
        {
            SatModelEnum modeloSat =
                    (SatModelEnum)Enum.Parse(typeof(SatModelEnum), InstanceManager.Parametros.ModeloSat);
            var sat =
                    new Core.Dominio.SAT.Sat(
                        SatBase.Create(InstanceManager.Parametros.CodigoSat, modeloSat), _repositorioBase, InstanceManager.Parametros,  VendaCorrente.VendaCorrente);
            sat.RegistrarVendaSat();
        }

        public void FinalizarVenda()
        {
            ListaDeVendaProdutos = new List<VendaProduto>();
            ListaDeVendaPagamentos = new List<VendaPagamento>();
            VendaCorrente = null;
        }

        public void CarregarComanda(string codigoComanda)
        {
            VendaCorrente.VendaCorrente.VendaProdutoes.Clear();
            ListaDeVendaProdutos = VendaCorrente.VendaCorrente.VendaProdutoes.ToList();

            var comanda = _comandaDominio.RecuperarComanda(codigoComanda);

            if (comanda!=null)
            comanda.ComandaProdutoes.ToList().ForEach((item) =>
            {
                var produto = InstanceManager.ListaDeProdutosDoPdv.SingleOrDefault(x => x.CodigoDeBarra == item.Produto_CodigoDeBarra);

                if (produto != null)
                    AdicionarVendaProduto(produto, item.Quantidade);
            });

            NumeroComanda = codigoComanda;
        }

        public void SalvarComanda()
        {
            _comandaDominio.ComandaCorrente.CodigoComanda = NumeroComanda;
            _comandaDominio.ExcluirComanda();
            VendaCorrente.VendaCorrente.VendaProdutoes.ToList().ForEach((item) =>
            {
                _comandaDominio.AdicionarAlterarProdutoNaComanda(item.Quantidade, item.Produto);
            });
            _comandaDominio.RegistrarComanda(NumeroComanda);
        }

    }
}
