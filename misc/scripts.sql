--
-- File generated with SQLiteStudio v3.1.1 on sex jul 14 01:22:11 2017
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Caixa
DROP TABLE IF EXISTS Caixa;

CREATE TABLE Caixa (
    CodigoCaixa              VARCHAR (32) NOT NULL,
    Nome                     VARCHAR (80) NOT NULL,
    Machine                  VARCHAR (60) NOT NULL,
    IP                       VARCHAR (20) NOT NULL,
    Situacao                 VARCHAR (15) NOT NULL
                                          DEFAULT Fechado,
    CodigoOperacaoDeAbertura VARCHAR (32),
    Sincronizado             BIT          NOT NULL
                                          DEFAULT (0),
    PRIMARY KEY (
        CodigoCaixa
    )
);


-- Table: ClienteCampanha
DROP TABLE IF EXISTS ClienteCampanha;

CREATE TABLE ClienteCampanha (
    CodigoClienteCampanha VARCHAR (32)  NOT NULL,
    NomeCampanha          VARCHAR (60)  NOT NULL,
    CpfCnpj               VARCHAR (20)  NOT NULL,
    Email                 VARCHAR (120) NOT NULL,
    Telefone              VARCHAR (20)  NOT NULL,
    NomeCliente           VARCHAR (120) NOT NULL,
    DataCadastro          DATETIME      NOT NULL,
    Observacao            VARCHAR (500),
    Sincronizado          BIT           NOT NULL
                                        DEFAULT (0),
    PRIMARY KEY (
        CodigoClienteCampanha
    )
);


-- Table: Comanda
DROP TABLE IF EXISTS Comanda;

CREATE TABLE Comanda (
    CodigoComanda VARCHAR (15) NOT NULL,
    Situacao      VARCHAR (20) NOT NULL,
    Sincronizado  BIT          NOT NULL
                               DEFAULT 0,
    PRIMARY KEY (
        CodigoComanda
    )
);


-- Table: ComandaProduto
DROP TABLE IF EXISTS ComandaProduto;

CREATE TABLE ComandaProduto (
    CodigoComandaProduto  VARCHAR (32) NOT NULL,
    Comanda_CodigoComanda VARCHAR (15) NOT NULL,
    Produto_CodigoDeBarra VARCHAR (32) NOT NULL,
    Quantidade            DECIMAL      NOT NULL,
    Sincronizado          BIT          NOT NULL
                                       DEFAULT 0,
    PRIMARY KEY (
        CodigoComandaProduto
    ),
    FOREIGN KEY (
        Produto_CodigoDeBarra
    )
    REFERENCES Produto (CodigoDeBarra) ON DELETE NO ACTION
                                       ON UPDATE NO ACTION,
    FOREIGN KEY (
        Comanda_CodigoComanda
    )
    REFERENCES Comanda (CodigoComanda) ON DELETE NO ACTION
                                       ON UPDATE NO ACTION
);


-- Table: ConfiguracaoCategoriaProduto
DROP TABLE IF EXISTS ConfiguracaoCategoriaProduto;

CREATE TABLE ConfiguracaoCategoriaProduto (
    Categoria      VARCHAR (60) NOT NULL,
    TemProducao    BIT          NOT NULL,
    DescontaInsumo BIT          NOT NULL,
    Sincronizado   BIT          NOT NULL
                                DEFAULT (0),
    PRIMARY KEY (
        Categoria
    )
);


-- Table: CupomFiscalSat
DROP TABLE IF EXISTS CupomFiscalSat;

CREATE TABLE CupomFiscalSat (
    CodigoVenda     VARCHAR (32)   NOT NULL,
    CpfCnpj         VARCHAR (20),
    ErrorCode       VARCHAR (200),
    ErrorCode2      VARCHAR (200),
    ErrorMessage    VARCHAR (200),
    InvoiceKey      VARCHAR (100),
    QrCodeSignature VARCHAR (500),
    SessionCode     VARCHAR (15),
    TimeStamp       VARCHAR (15),
    Total           VARCHAR (15),
    Xml             VARCHAR (5000),
    DataOperacao    DATETIME,
    CodigoSat       VARCHAR (20),
    XmlEnvio        VARCHAR (5000),
    PRIMARY KEY (
        CodigoVenda
    ),
    FOREIGN KEY (
        CodigoVenda
    )
    REFERENCES Venda (CodigoVenda) ON DELETE CASCADE
                                   ON UPDATE CASCADE
);


-- Table: OperacaoCaixa
DROP TABLE IF EXISTS OperacaoCaixa;

CREATE TABLE OperacaoCaixa (
    CodigoOperacaoCaixa         VARCHAR (32) NOT NULL,
    Usuario_CodigoUsuario       VARCHAR (32) NOT NULL,
    Caixa_CodigoCaixa           VARCHAR (32) NOT NULL,
    DataOperacao                DATETIME     NOT NULL,
    TipoOperacao                VARCHAR (20) NOT NULL,
    ValorOperacao               DECIMAL      NOT NULL,
    CodigoOperacaoCaixaAbertura VARCHAR (32),
    Sincronizado                BIT          NOT NULL
                                             DEFAULT (0),
    PRIMARY KEY (
        CodigoOperacaoCaixa
    ),
    FOREIGN KEY (
        Caixa_CodigoCaixa
    )
    REFERENCES Caixa (CodigoCaixa) ON DELETE CASCADE
                                   ON UPDATE CASCADE,
    FOREIGN KEY (
        Usuario_CodigoUsuario
    )
    REFERENCES Usuario (CodigoUsuario) ON DELETE NO ACTION
                                       ON UPDATE NO ACTION
);


-- Table: Parametro
DROP TABLE IF EXISTS Parametro;

CREATE TABLE Parametro (
    Nome  VARCHAR (120)  NOT NULL,
    Valor VARCHAR (1000) NOT NULL,
    PRIMARY KEY (
        Nome
    )
);


-- Table: Pedido
DROP TABLE IF EXISTS Pedido;

CREATE TABLE Pedido (
    CodigoPedido VARCHAR (32)  NOT NULL,
    DataPedido   DATETIME      NOT NULL,
    DataEntrega  DATETIME      NOT NULL,
    NomeCliente  VARCHAR (80)  NOT NULL,
    Telefone     VARCHAR (20),
    Situacao     VARCHAR (15)  NOT NULL,
    Observacao   VARCHAR (500),
    Valor        DECIMAL       NOT NULL
                               DEFAULT 0,
    Sincronizado BIT           NOT NULL
                               DEFAULT (0),
    PRIMARY KEY (
        CodigoPedido
    )
);


-- Table: PedidoProduto
DROP TABLE IF EXISTS PedidoProduto;

CREATE TABLE PedidoProduto (
    CodigoPedidoProduto   VARCHAR (32) NOT NULL,
    Produto_CodigoDeBarra VARCHAR (32) NOT NULL,
    Pedido_CodigoPedido   VARCHAR (32) NOT NULL,
    Quantidade            INTEGER      NOT NULL,
    Sincronizado          BIT          NOT NULL
                                       DEFAULT (0),
    PRIMARY KEY (
        CodigoPedidoProduto
    ),
    FOREIGN KEY (
        Pedido_CodigoPedido
    )
    REFERENCES Pedido (CodigoPedido) ON DELETE CASCADE
                                     ON UPDATE CASCADE,
    FOREIGN KEY (
        Produto_CodigoDeBarra
    )
    REFERENCES Produto (CodigoDeBarra) ON DELETE NO ACTION
                                       ON UPDATE NO ACTION
);


-- Table: Produto
DROP TABLE IF EXISTS Produto;

CREATE TABLE Produto (
    CodigoDeBarra          VARCHAR (32)  NOT NULL,
    TipoProduto            VARCHAR (15)  NOT NULL,
    Modelo                 VARCHAR (60)  NOT NULL,
    Descricao              VARCHAR (250) NOT NULL,
    DescricaoBusca         VARCHAR (200) NOT NULL,
    PrecoCusto             DECIMAL       NOT NULL,
    PrecoVenda             DECIMAL       NOT NULL,
    PrecoVenda2            DECIMAL,
    TipoUnidade            VARCHAR (15)  NOT NULL,
    EstoqueMinimo          INTEGER,
    Marca                  VARCHAR (60)  NOT NULL,
    TipoFiscal             VARCHAR (80),
    CodigoNCM              VARCHAR (10),
    DigitoCST              VARCHAR (15),
    CEST                   VARCHAR (15),
    Categoria              VARCHAR (60)  NOT NULL,
    SubCategoria           VARCHAR (60)  NOT NULL,
    Ativo                  BIT           NOT NULL,
    ExibirNoPdv            BIT           NOT NULL,
    ControlarEstoque       BIT           NOT NULL,
    DescontarInsumoNaVenda BIT           NOT NULL,
    TemProducao            BIT           NOT NULL,
    CodigoParaCupom        VARCHAR (4)   NOT NULL,
    Sincronizado           BIT           NOT NULL
                                         DEFAULT (0),
    QtdeEstoque            DECIMAL       NOT NULL,
    PRIMARY KEY (
        CodigoDeBarra
    )
);


-- Table: ProdutoProducao
DROP TABLE IF EXISTS ProdutoProducao;

CREATE TABLE ProdutoProducao (
    CodigoProdutoProducao       VARCHAR (32) NOT NULL,
    Produto_CodigoDeBarra       VARCHAR (32) NOT NULL,
    DataProducao                DATE         NOT NULL,
    QuantidadeProduzida         INTEGER      NOT NULL,
    QuantidadeDescartadaInteira INTEGER      NOT NULL,
    QuantidadeDescartadaParcial INTEGER      NOT NULL,
    Sincronizado                BIT          NOT NULL
                                             DEFAULT (0),
    PRIMARY KEY (
        CodigoProdutoProducao
    ),
    FOREIGN KEY (
        Produto_CodigoDeBarra
    )
    REFERENCES Produto (CodigoDeBarra) ON DELETE NO ACTION
                                       ON UPDATE NO ACTION
);


-- Table: ResultadoOperacaoFechamento
DROP TABLE IF EXISTS ResultadoOperacaoFechamento;

CREATE TABLE ResultadoOperacaoFechamento (
    CodigoResultado                   VARCHAR (32) NOT NULL,
    OperacaoCaixa_CodigoOperacaoCaixa VARCHAR (32) NOT NULL,
    ValorAbertura                     DECIMAL      NOT NULL,
    ValorContabilizadoNoFechamento    DECIMAL      NOT NULL,
    ValorTotalSangria                 DECIMAL      NOT NULL,
    ValorTotalReforco                 DECIMAL      NOT NULL,
    ValorTotalPagamentoDinheiro       DECIMAL      NOT NULL,
    ValorTotalPagamentoDebito         DECIMAL      NOT NULL,
    ValorTotalPagamentoCredito        DECIMAL      NOT NULL,
    ValorTotalPagamentoTicket         DECIMAL      NOT NULL,
    ValorTotalPagamento               DECIMAL      NOT NULL,
    ValorTotalEstimadoEmEspecie       DECIMAL      NOT NULL,
    DiferencaNoCaixa                  DECIMAL      NOT NULL,
    ValorTotalDescontoVenda           DECIMAL      NOT NULL,
    ValorRecebimentoDebito            DECIMAL      NOT NULL,
    ValorRecebimentoCretito           DECIMAL      NOT NULL,
    ValorRecebimentoTicket            DECIMAL      NOT NULL,
    Sincronizado                      BIT          NOT NULL
                                                   DEFAULT (0),
    PRIMARY KEY (
        CodigoResultado,
        OperacaoCaixa_CodigoOperacaoCaixa
    ),
    FOREIGN KEY (
        OperacaoCaixa_CodigoOperacaoCaixa
    )
    REFERENCES OperacaoCaixa (CodigoOperacaoCaixa) ON DELETE NO ACTION
                                                   ON UPDATE NO ACTION
);


-- Table: TipoPagamento
DROP TABLE IF EXISTS TipoPagamento;

CREATE TABLE TipoPagamento (
    CodigoTipoPagamento VARCHAR (32) NOT NULL,
    Nome                VARCHAR (30) NOT NULL,
    PercentualDesconto  DECIMAL      NOT NULL,
    DiasParaPagamento   INTEGER      NOT NULL,
    Sincronizado        BIT          NOT NULL
                                     DEFAULT (0),
    PRIMARY KEY (
        CodigoTipoPagamento
    )
);


-- Table: Usuario
DROP TABLE IF EXISTS Usuario;

CREATE TABLE Usuario (
    CodigoUsuario VARCHAR (32) NOT NULL,
    Nome          VARCHAR (60) NOT NULL,
    Senha         VARCHAR (20) NOT NULL,
    Tipo          VARCHAR (15) NOT NULL,
    Sincronizado  BIT          NOT NULL
                               DEFAULT (0),
    PRIMARY KEY (
        CodigoUsuario
    )
);


-- Table: Venda
DROP TABLE IF EXISTS Venda;

CREATE TABLE Venda (
    CodigoVenda                       VARCHAR (40) NOT NULL,
    OperacaoCaixa_CodigoOperacaoCaixa VARCHAR (32) NOT NULL,
    Usuario_CodigoUsuario             VARCHAR (32) NOT NULL,
    ValorTotalVenda                   DECIMAL      NOT NULL,
    ValorTotalDescontoVenda           DECIMAL      NOT NULL,
    ValorTotalRecebimento             DECIMAL      NOT NULL,
    DataRecebimento                   DATETIME     NOT NULL,
    DataVenda                         DATETIME     NOT NULL,
    CpfCnpjCliente                    VARCHAR (20),
    NomeCliente                       VARCHAR (60),
    ValorTroco                        DECIMAL      NOT NULL,
    Sincronizado                      BIT          NOT NULL
                                                   DEFAULT 0,
    CFOP                              INTEGER      NOT NULL,
    PRIMARY KEY (
        CodigoVenda
    ),
    FOREIGN KEY (
        Usuario_CodigoUsuario
    )
    REFERENCES Usuario (CodigoUsuario) ON DELETE NO ACTION
                                       ON UPDATE NO ACTION,
    FOREIGN KEY (
        OperacaoCaixa_CodigoOperacaoCaixa
    )
    REFERENCES OperacaoCaixa (CodigoOperacaoCaixa) ON DELETE CASCADE
                                                   ON UPDATE CASCADE
);


-- Table: VendaPagamento
DROP TABLE IF EXISTS VendaPagamento;

CREATE TABLE VendaPagamento (
    CodigoVendaPagamento              VARCHAR (32) NOT NULL,
    TipoPagamento_CodigoTipoPagamento VARCHAR (32) NOT NULL,
    Venda_CodigoVenda                 VARCHAR (40) NOT NULL,
    ValorPagamento                    DECIMAL      NOT NULL,
    Sincronizado                      BIT          NOT NULL
                                                   DEFAULT (0),
    PRIMARY KEY (
        CodigoVendaPagamento
    ),
    FOREIGN KEY (
        Venda_CodigoVenda
    )
    REFERENCES Venda (CodigoVenda) ON DELETE NO ACTION
                                   ON UPDATE NO ACTION,
    FOREIGN KEY (
        TipoPagamento_CodigoTipoPagamento
    )
    REFERENCES TipoPagamento (CodigoTipoPagamento) ON DELETE NO ACTION
                                                   ON UPDATE NO ACTION
);


-- Table: VendaProduto
DROP TABLE IF EXISTS VendaProduto;

CREATE TABLE VendaProduto (
    CodigoVendaProduto        VARCHAR (32)  NOT NULL,
    Produto_CodigoDeBarra     VARCHAR (32)  NOT NULL,
    Venda_CodigoVenda         VARCHAR (40)  NOT NULL,
    ValorDoProduto            DECIMAL       NOT NULL,
    ValorDoProdutoComDesconto DECIMAL       NOT NULL,
    ValorDoDesconto           DECIMAL       NOT NULL,
    Quantidade                DECIMAL       NOT NULL,
    ValorTotalVendaProduto    DECIMAL       NOT NULL,
    ValorTotalDoDesconto      DECIMAL       NOT NULL,
    CodigoParaCupom           VARCHAR (4)   NOT NULL,
    DescricaoProduto          VARCHAR (250) NOT NULL,
    Sincronizado              BIT           NOT NULL
                                            DEFAULT (0),
    PRIMARY KEY (
        CodigoVendaProduto
    ),
    FOREIGN KEY (
        Venda_CodigoVenda
    )
    REFERENCES Venda (CodigoVenda) ON DELETE CASCADE
                                   ON UPDATE CASCADE,
    FOREIGN KEY (
        Produto_CodigoDeBarra
    )
    REFERENCES Produto (CodigoDeBarra) ON DELETE NO ACTION
                                       ON UPDATE NO ACTION
);


-- Index: ComandaProduto_FKIndex1
DROP INDEX IF EXISTS ComandaProduto_FKIndex1;

CREATE INDEX ComandaProduto_FKIndex1 ON ComandaProduto (
    Produto_CodigoDeBarra
);


-- Index: ComandaProduto_FKIndex3
DROP INDEX IF EXISTS ComandaProduto_FKIndex3;

CREATE INDEX ComandaProduto_FKIndex3 ON ComandaProduto (
    Comanda_CodigoComanda
);


-- Index: CupomFiscalSat_FKIndex1
DROP INDEX IF EXISTS CupomFiscalSat_FKIndex1;

CREATE INDEX CupomFiscalSat_FKIndex1 ON CupomFiscalSat (
    CodigoVenda
);


-- Index: IFK_Rel_01
DROP INDEX IF EXISTS IFK_Rel_01;

CREATE INDEX IFK_Rel_01 ON VendaProduto (
    Produto_CodigoDeBarra
);


-- Index: IFK_Rel_02
DROP INDEX IF EXISTS IFK_Rel_02;

CREATE INDEX IFK_Rel_02 ON ComandaProduto (
    Produto_CodigoDeBarra
);


-- Index: IFK_Rel_09
DROP INDEX IF EXISTS IFK_Rel_09;

CREATE INDEX IFK_Rel_09 ON VendaProduto (
    Venda_CodigoVenda
);


-- Index: IFK_Rel_10
DROP INDEX IF EXISTS IFK_Rel_10;

CREATE INDEX IFK_Rel_10 ON OperacaoCaixa (
    Usuario_CodigoUsuario
);


-- Index: IFK_Rel_11
DROP INDEX IF EXISTS IFK_Rel_11;

CREATE INDEX IFK_Rel_11 ON Venda (
    Usuario_CodigoUsuario
);


-- Index: IFK_Rel_12
DROP INDEX IF EXISTS IFK_Rel_12;

CREATE INDEX IFK_Rel_12 ON ResultadoOperacaoFechamento (
    OperacaoCaixa_CodigoOperacaoCaixa
);


-- Index: IFK_Rel_14
DROP INDEX IF EXISTS IFK_Rel_14;

CREATE INDEX IFK_Rel_14 ON Venda (
    OperacaoCaixa_CodigoOperacaoCaixa
);


-- Index: IFK_Rel_15
DROP INDEX IF EXISTS IFK_Rel_15;

CREATE INDEX IFK_Rel_15 ON OperacaoCaixa (
    Caixa_CodigoCaixa
);


-- Index: IFK_Rel_1555
DROP INDEX IF EXISTS IFK_Rel_1555;

CREATE INDEX IFK_Rel_1555 ON CupomFiscalSat (
    CodigoVenda
);


-- Index: IFK_Rel_17
DROP INDEX IF EXISTS IFK_Rel_17;

CREATE INDEX IFK_Rel_17 ON ComandaProduto (
    Comanda_CodigoComanda
);


-- Index: IFK_Rel_18
DROP INDEX IF EXISTS IFK_Rel_18;

CREATE INDEX IFK_Rel_18 ON ProdutoProducao (
    Produto_CodigoDeBarra
);


-- Index: IFK_Rel_19
DROP INDEX IF EXISTS IFK_Rel_19;

CREATE INDEX IFK_Rel_19 ON VendaPagamento (
    TipoPagamento_CodigoTipoPagamento
);


-- Index: IFK_Rel_20
DROP INDEX IF EXISTS IFK_Rel_20;

CREATE INDEX IFK_Rel_20 ON VendaPagamento (
    Venda_CodigoVenda
);


-- Index: IFK_Rel_22
DROP INDEX IF EXISTS IFK_Rel_22;

CREATE INDEX IFK_Rel_22 ON PedidoProduto (
    Produto_CodigoDeBarra
);


-- Index: IFK_Rel_23
DROP INDEX IF EXISTS IFK_Rel_23;

CREATE INDEX IFK_Rel_23 ON PedidoProduto (
    Pedido_CodigoPedido
);


-- Index: OperacaoCaixa_FKIndex1
DROP INDEX IF EXISTS OperacaoCaixa_FKIndex1;

CREATE INDEX OperacaoCaixa_FKIndex1 ON OperacaoCaixa (
    Caixa_CodigoCaixa
);


-- Index: OperacaoCaixa_FKIndex2
DROP INDEX IF EXISTS OperacaoCaixa_FKIndex2;

CREATE INDEX OperacaoCaixa_FKIndex2 ON OperacaoCaixa (
    Usuario_CodigoUsuario
);


-- Index: OperacaoCaixaResultado_FKIndex1
DROP INDEX IF EXISTS OperacaoCaixaResultado_FKIndex1;

CREATE INDEX OperacaoCaixaResultado_FKIndex1 ON ResultadoOperacaoFechamento (
    OperacaoCaixa_CodigoOperacaoCaixa
);


-- Index: PedidoProduto_FKIndex1
DROP INDEX IF EXISTS PedidoProduto_FKIndex1;

CREATE INDEX PedidoProduto_FKIndex1 ON PedidoProduto (
    Pedido_CodigoPedido
);


-- Index: PedidoProduto_FKIndex2
DROP INDEX IF EXISTS PedidoProduto_FKIndex2;

CREATE INDEX PedidoProduto_FKIndex2 ON PedidoProduto (
    Produto_CodigoDeBarra
);


-- Index: Produto_DescricaoBusca
DROP INDEX IF EXISTS Produto_DescricaoBusca;

CREATE INDEX Produto_DescricaoBusca ON Produto (
    DescricaoBusca
);


-- Index: ProdutoProducao_FKIndex1
DROP INDEX IF EXISTS ProdutoProducao_FKIndex1;

CREATE INDEX ProdutoProducao_FKIndex1 ON ProdutoProducao (
    Produto_CodigoDeBarra
);


-- Index: Venda_FKIndex1
DROP INDEX IF EXISTS Venda_FKIndex1;

CREATE INDEX Venda_FKIndex1 ON Venda (
    Usuario_CodigoUsuario
);


-- Index: Venda_FKIndex2
DROP INDEX IF EXISTS Venda_FKIndex2;

CREATE INDEX Venda_FKIndex2 ON Venda (
    OperacaoCaixa_CodigoOperacaoCaixa
);


-- Index: VendaPagamento_FKIndex1
DROP INDEX IF EXISTS VendaPagamento_FKIndex1;

CREATE INDEX VendaPagamento_FKIndex1 ON VendaPagamento (
    Venda_CodigoVenda
);


-- Index: VendaPagamento_FKIndex2
DROP INDEX IF EXISTS VendaPagamento_FKIndex2;

CREATE INDEX VendaPagamento_FKIndex2 ON VendaPagamento (
    TipoPagamento_CodigoTipoPagamento
);


-- Index: VendaProduto_FKIndex1
DROP INDEX IF EXISTS VendaProduto_FKIndex1;

CREATE INDEX VendaProduto_FKIndex1 ON VendaProduto (
    Venda_CodigoVenda
);


-- Index: VendaProduto_FKIndex2
DROP INDEX IF EXISTS VendaProduto_FKIndex2;

CREATE INDEX VendaProduto_FKIndex2 ON VendaProduto (
    Produto_CodigoDeBarra
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
