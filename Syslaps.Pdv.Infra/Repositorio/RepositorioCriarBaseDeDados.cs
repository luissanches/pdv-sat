using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioCriarBaseDeDados : RepositorioBase, IRepositorioCriarBaseDeDados
    {
        public void ExcluirTabelas()
        {
            var sb = new StringBuilder();
            sb.Append(@"

DROP TABLE VendaPagamento;

DROP TABLE VendaProduto;

DROP TABLE Venda;

DROP TABLE ResultadoOperacaoFechamento;

DROP TABLE ComandaProduto;

DROP TABLE PedidoProduto;

DROP TABLE OperacaoCaixa;

DROP TABLE ProdutoProducao;

DROP TABLE ConfiguracaoCategoriaProduto;

DROP TABLE Caixa;

DROP TABLE ClienteCampanha;

DROP TABLE Comanda;

DROP TABLE TipoPagamento;

DROP TABLE Usuario;

DROP TABLE Produto;

DROP TABLE Parametro;

DROP TABLE Pedido;


");
            Db.Execute(sb.ToString());

        }

        public void CriarDataBase()
        {
            var sb = new StringBuilder();
            sb.Append(@"

CREATE TABLE Pedido (
  CodigoPedido VARCHAR(32)  NOT NULL  ,
  DataPedido DATETIME  NOT NULL  ,
  DataEntrega DATETIME  NOT NULL  ,
  NomeCliente VARCHAR(80)  NOT NULL  ,
  Telefone VARCHAR(20)    ,
  Situacao VARCHAR(15)  NOT NULL  ,
  Observacao VARCHAR(500)    ,
  Valor DECIMAL  NOT NULL DEFAULT 0   ,
PRIMARY KEY(CodigoPedido));




CREATE TABLE Parametro (
  Nome VARCHAR(120)  NOT NULL  ,
  Valor VARCHAR(1000)  NOT NULL    ,
PRIMARY KEY(Nome));




CREATE TABLE Produto (
  CodigoDeBarra VARCHAR(32)  NOT NULL  ,
  TipoProduto VARCHAR(15)  NOT NULL  ,
  Modelo VARCHAR(60)  NOT NULL  ,
  Descricao VARCHAR(250)  NOT NULL  ,
  DescricaoBusca VARCHAR(200)  NOT NULL  ,
  PrecoCusto DECIMAL  NOT NULL  ,
  PrecoVenda DECIMAL  NOT NULL  ,
  TipoUnidade VARCHAR(15)  NOT NULL  ,
  EstoqueMinimo INTEGER    ,
  Marca VARCHAR(60)  NOT NULL  ,
  TipoFiscal VARCHAR(80)    ,
  CodigoNCM VARCHAR(10)    ,
  DigitoCST VARCHAR(15)    ,
  CEST VARCHAR(15)    ,
  Categoria VARCHAR(60)  NOT NULL  ,
  SubCategoria VARCHAR(60)  NOT NULL  ,
  Ativo BIT  NOT NULL  ,
  ExibirNoPdv BIT  NOT NULL  ,
  ControlarEstoque BIT  NOT NULL  ,
  DescontarInsumoNaVenda BIT  NOT NULL  ,
  TemProducao BIT  NOT NULL  ,
  CodigoParaCupom VARCHAR(4)  NOT NULL    ,
PRIMARY KEY(CodigoDeBarra)  );


CREATE INDEX Produto_DescricaoBusca ON Produto (DescricaoBusca);








CREATE TABLE Usuario (
  CodigoUsuario VARCHAR(32)  NOT NULL  ,
  Nome VARCHAR(60)  NOT NULL  ,
  Senha VARCHAR(20)  NOT NULL  ,
  Tipo VARCHAR(15)  NOT NULL    ,
PRIMARY KEY(CodigoUsuario));





CREATE TABLE TipoPagamento (
  CodigoTipoPagamento VARCHAR(32)  NOT NULL  ,
  Nome VARCHAR(30)  NOT NULL  ,
  PercentualDesconto DECIMAL  NOT NULL  ,
  DiasParaPagamento INTEGER  NOT NULL    ,
PRIMARY KEY(CodigoTipoPagamento));




CREATE TABLE Comanda (
  CodigoComanda INTEGER  NOT NULL  ,
  Situacao VARCHAR(20)  NOT NULL    ,
PRIMARY KEY(CodigoComanda));





CREATE TABLE ClienteCampanha (
  CodigoClienteCampanha VARCHAR(32)  NOT NULL  ,
  NomeCampanha VARCHAR(60)  NOT NULL  ,
  CpfCnpj VARCHAR(20)  NOT NULL  ,
  Email VARCHAR(120)  NOT NULL  ,
  Telefone VARCHAR(20)  NOT NULL  ,
  NomeCliente VARCHAR(120)  NOT NULL  ,
  DataCadastro DATETIME  NOT NULL  ,
  Observacao VARCHAR(500)      ,
PRIMARY KEY(CodigoClienteCampanha));




CREATE TABLE Caixa (
  CodigoCaixa VARCHAR(32)  NOT NULL  ,
  Nome VARCHAR(80)  NOT NULL  ,
  Machine VARCHAR(60)  NOT NULL  ,
  IP VARCHAR(20)  NOT NULL  ,
  Situacao VARCHAR(15)  NOT NULL DEFAULT Fechado ,
  CodigoOperacaoDeAbertura VARCHAR(32)      ,
PRIMARY KEY(CodigoCaixa));





CREATE TABLE ConfiguracaoCategoriaProduto (
  Categoria VARCHAR(60)  NOT NULL  ,
  TemProducao BIT  NOT NULL  ,
  DescontaInsumo BIT  NOT NULL    ,
PRIMARY KEY(Categoria));




CREATE TABLE ProdutoProducao (
  CodigoProdutoProducao VARCHAR(32)  NOT NULL  ,
  Produto_CodigoDeBarra VARCHAR(32)  NOT NULL  ,
  DataProducao DATE  NOT NULL  ,
  QuantidadeProduzida INTEGER  NOT NULL  ,
  QuantidadeDescartadaInteira INTEGER  NOT NULL  ,
  QuantidadeDescartadaParcial INTEGER  NOT NULL    ,
PRIMARY KEY(CodigoProdutoProducao)  ,
  FOREIGN KEY(Produto_CodigoDeBarra)
    REFERENCES Produto(CodigoDeBarra)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION);


CREATE INDEX ProdutoProducao_FKIndex1 ON ProdutoProducao (Produto_CodigoDeBarra);


CREATE INDEX IFK_Rel_18 ON ProdutoProducao (Produto_CodigoDeBarra);


CREATE TABLE OperacaoCaixa (
  CodigoOperacaoCaixa VARCHAR(32)  NOT NULL  ,
  Usuario_CodigoUsuario VARCHAR(32)  NOT NULL  ,
  Caixa_CodigoCaixa VARCHAR(32)  NOT NULL  ,
  DataOperacao DATETIME  NOT NULL  ,
  TipoOperacao VARCHAR(20)  NOT NULL  ,
  ValorOperacao DECIMAL  NOT NULL  ,
  CodigoOperacaoCaixaAbertura VARCHAR(32)      ,
PRIMARY KEY(CodigoOperacaoCaixa)    ,
  FOREIGN KEY(Caixa_CodigoCaixa)
    REFERENCES Caixa(CodigoCaixa)
      ON DELETE CASCADE
      ON UPDATE CASCADE,
  FOREIGN KEY(Usuario_CodigoUsuario)
    REFERENCES Usuario(CodigoUsuario)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION);


CREATE INDEX OperacaoCaixa_FKIndex1 ON OperacaoCaixa (Caixa_CodigoCaixa);
CREATE INDEX OperacaoCaixa_FKIndex2 ON OperacaoCaixa (Usuario_CodigoUsuario);



CREATE INDEX IFK_Rel_15 ON OperacaoCaixa (Caixa_CodigoCaixa);
CREATE INDEX IFK_Rel_10 ON OperacaoCaixa (Usuario_CodigoUsuario);


CREATE TABLE PedidoProduto (
  CodigoPedidoProduto VARCHAR(32)  NOT NULL  ,
  Produto_CodigoDeBarra VARCHAR(32)  NOT NULL  ,
  Pedido_CodigoPedido VARCHAR(32)  NOT NULL  ,
  Quantidade INTEGER  NOT NULL    ,
PRIMARY KEY(CodigoPedidoProduto)    ,
  FOREIGN KEY(Pedido_CodigoPedido)
    REFERENCES Pedido(CodigoPedido)
      ON DELETE CASCADE
      ON UPDATE CASCADE,
  FOREIGN KEY(Produto_CodigoDeBarra)
    REFERENCES Produto(CodigoDeBarra)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION);


CREATE INDEX PedidoProduto_FKIndex1 ON PedidoProduto (Pedido_CodigoPedido);
CREATE INDEX PedidoProduto_FKIndex2 ON PedidoProduto (Produto_CodigoDeBarra);


CREATE INDEX IFK_Rel_23 ON PedidoProduto (Pedido_CodigoPedido);
CREATE INDEX IFK_Rel_22 ON PedidoProduto (Produto_CodigoDeBarra);


CREATE TABLE ComandaProduto (
  CodigoComandaProduto VARCHAR(32)  NOT NULL  ,
  Comanda_CodigoComanda INTEGER  NOT NULL  ,
  Usuario_CodigoUsuario VARCHAR(32)  NOT NULL  ,
  Produto_CodigoDeBarra VARCHAR(32)  NOT NULL    ,
PRIMARY KEY(CodigoComandaProduto)      ,
  FOREIGN KEY(Produto_CodigoDeBarra)
    REFERENCES Produto(CodigoDeBarra)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION,
  FOREIGN KEY(Usuario_CodigoUsuario)
    REFERENCES Usuario(CodigoUsuario)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION,
  FOREIGN KEY(Comanda_CodigoComanda)
    REFERENCES Comanda(CodigoComanda)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION);


CREATE INDEX ComandaProduto_FKIndex1 ON ComandaProduto (Produto_CodigoDeBarra);
CREATE INDEX ComandaProduto_FKIndex2 ON ComandaProduto (Usuario_CodigoUsuario);
CREATE INDEX ComandaProduto_FKIndex3 ON ComandaProduto (Comanda_CodigoComanda);


CREATE INDEX IFK_Rel_02 ON ComandaProduto (Produto_CodigoDeBarra);
CREATE INDEX IFK_Rel_13 ON ComandaProduto (Usuario_CodigoUsuario);
CREATE INDEX IFK_Rel_17 ON ComandaProduto (Comanda_CodigoComanda);


CREATE TABLE ResultadoOperacaoFechamento (
  CodigoResultado VARCHAR(32)  NOT NULL  ,
  OperacaoCaixa_CodigoOperacaoCaixa VARCHAR(32)  NOT NULL  ,
  ValorAbertura DECIMAL  NOT NULL  ,
  ValorContabilizadoNoFechamento DECIMAL  NOT NULL  ,
  ValorTotalSangria DECIMAL  NOT NULL  ,
  ValorTotalReforco DECIMAL  NOT NULL  ,
  ValorTotalPagamentoDinheiro DECIMAL  NOT NULL  ,
  ValorTotalPagamentoDebito DECIMAL  NOT NULL  ,
  ValorTotalPagamentoCredito DECIMAL  NOT NULL  ,
  ValorTotalPagamentoTicket DECIMAL  NOT NULL  ,
  ValorTotalPagamento DECIMAL  NOT NULL  ,
  ValorTotalEstimadoEmEspecie DECIMAL  NOT NULL  ,
  DiferencaNoCaixa DECIMAL  NOT NULL  ,
  ValorTotalDescontoVenda DECIMAL  NOT NULL  ,
  ValorRecebimentoDebito DECIMAL  NOT NULL  ,
  ValorRecebimentoCretito DECIMAL  NOT NULL  ,
  ValorRecebimentoTicket DECIMAL  NOT NULL    ,
PRIMARY KEY(CodigoResultado, OperacaoCaixa_CodigoOperacaoCaixa)  ,
  FOREIGN KEY(OperacaoCaixa_CodigoOperacaoCaixa)
    REFERENCES OperacaoCaixa(CodigoOperacaoCaixa)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION);


CREATE INDEX OperacaoCaixaResultado_FKIndex1 ON ResultadoOperacaoFechamento (OperacaoCaixa_CodigoOperacaoCaixa);


CREATE INDEX IFK_Rel_12 ON ResultadoOperacaoFechamento (OperacaoCaixa_CodigoOperacaoCaixa);


CREATE TABLE Venda (
  CodigoVenda VARCHAR(40)  NOT NULL  ,
  OperacaoCaixa_CodigoOperacaoCaixa VARCHAR(32)  NOT NULL  ,
  Usuario_CodigoUsuario VARCHAR(32)  NOT NULL  ,
  ValorTotalVenda DECIMAL  NOT NULL  ,
  ValorTotalDescontoVenda DECIMAL  NOT NULL  ,
  ValorTotalRecebimento DECIMAL  NOT NULL  ,
  DataRecebimento DATETIME  NOT NULL  ,
  DataVenda DATETIME  NOT NULL  ,
  CpfCnpjCliente VARCHAR(20)    ,
  NomeCliente VARCHAR(60)    ,
  ValorTroco DECIMAL  NOT NULL  ,
  VendaTransferida BIT  NOT NULL    ,
PRIMARY KEY(CodigoVenda)    ,
  FOREIGN KEY(Usuario_CodigoUsuario)
    REFERENCES Usuario(CodigoUsuario)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION,
  FOREIGN KEY(OperacaoCaixa_CodigoOperacaoCaixa)
    REFERENCES OperacaoCaixa(CodigoOperacaoCaixa)
      ON DELETE CASCADE
      ON UPDATE CASCADE);


CREATE INDEX Venda_FKIndex1 ON Venda (Usuario_CodigoUsuario);
CREATE INDEX Venda_FKIndex2 ON Venda (OperacaoCaixa_CodigoOperacaoCaixa);


CREATE INDEX IFK_Rel_11 ON Venda (Usuario_CodigoUsuario);
CREATE INDEX IFK_Rel_14 ON Venda (OperacaoCaixa_CodigoOperacaoCaixa);


CREATE TABLE VendaProduto (
  CodigoVendaProduto VARCHAR(32)  NOT NULL  ,
  Produto_CodigoDeBarra VARCHAR(32)  NOT NULL  ,
  Venda_CodigoVenda VARCHAR(40)  NOT NULL  ,
  ValorDoProduto DECIMAL  NOT NULL  ,
  ValorDoProdutoComDesconto DECIMAL  NOT NULL  ,
  ValorDoDesconto DECIMAL  NOT NULL  ,
  Quantidade DECIMAL  NOT NULL  ,
  ValorTotalVendaProduto DECIMAL  NOT NULL  ,
  ValorTotalDoDesconto DECIMAL  NOT NULL  ,
  CodigoParaCupom VARCHAR(4)  NOT NULL  ,
  DescricaoProduto VARCHAR(250)  NOT NULL    ,
PRIMARY KEY(CodigoVendaProduto)    ,
  FOREIGN KEY(Venda_CodigoVenda)
    REFERENCES Venda(CodigoVenda)
      ON DELETE CASCADE
      ON UPDATE CASCADE,
  FOREIGN KEY(Produto_CodigoDeBarra)
    REFERENCES Produto(CodigoDeBarra)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION);


CREATE INDEX VendaProduto_FKIndex1 ON VendaProduto (Venda_CodigoVenda);
CREATE INDEX VendaProduto_FKIndex2 ON VendaProduto (Produto_CodigoDeBarra);


CREATE INDEX IFK_Rel_09 ON VendaProduto (Venda_CodigoVenda);
CREATE INDEX IFK_Rel_01 ON VendaProduto (Produto_CodigoDeBarra);


CREATE TABLE VendaPagamento (
  CodigoVendaPagamento VARCHAR(32)  NOT NULL  ,
  TipoPagamento_CodigoTipoPagamento VARCHAR(32)  NOT NULL  ,
  Venda_CodigoVenda VARCHAR(40)  NOT NULL  ,
  ValorPagamento DECIMAL  NOT NULL    ,
PRIMARY KEY(CodigoVendaPagamento)    ,
  FOREIGN KEY(Venda_CodigoVenda)
    REFERENCES Venda(CodigoVenda)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION,
  FOREIGN KEY(TipoPagamento_CodigoTipoPagamento)
    REFERENCES TipoPagamento(CodigoTipoPagamento)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION);


CREATE INDEX VendaPagamento_FKIndex1 ON VendaPagamento (Venda_CodigoVenda);
CREATE INDEX VendaPagamento_FKIndex2 ON VendaPagamento (TipoPagamento_CodigoTipoPagamento);


CREATE INDEX IFK_Rel_20 ON VendaPagamento (Venda_CodigoVenda);
CREATE INDEX IFK_Rel_19 ON VendaPagamento (TipoPagamento_CodigoTipoPagamento);







");
            Db.Execute(sb.ToString());

        }

        public void Vacuum()
        {
            LimparLogDaBase();
        }

    }
}