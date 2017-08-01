# PDV-SAT

#### Descrição
```
Ponto de Venda de Produtos com integração SAT.
Transmite as informações das operações comerciais dos contribuintes varejistas do estado de São Paulo,
utilizando Internet, através de equipamento SAT.
```


#### Motivação
```
Com a obrigatoriedade da utilização do Sistema Autenticador e Transmissor de Cupons Fiscais Eletrônicos (SAT)
para comerciantes varejistas do Estado de São, eu quis entender e criar uma ferramenta open source
com o propósito de ajudar pequenos contribuintes e desenvolvedores que necessitem usar esse ecossistema
sem ser necessário grandes investimentos. 
```

#### Funcionalidades
```
- Importação / Atualização de Produtos através de Planilha 
- Login para Controle de Acesso
- Venda de Produtos
- Controle de Encomendas
- Consulta de Vendas
- Cadastro de Clientes
- Envio de e-mail com resultado do fechamento diário
- Controle de Produção x Descartados
- Impressão de Cupom SAT
- Geração de arquivo XML de envio (fisco) com movimento do período
```

#### Tecnologias
```
- Plantaforma Windows 
- .Net C#
- Windows Presentation Foundation (WPF)
- Micro ORM Dapper
- Sqlite
```

#### Configuração para desenvolvimento
	1 - Configurar projeto para compilar em x86 - Por causa das dlls do SAT e Impressora 
	2 - Configurar o projeto Syslaps.Pdv.UI como "Startup Project" 
	3 - Alterar tabela de Parametros - Dados da empresa, emails, SAT...
	4 - Alterar tabela de Produtos - Utilizo Sqlite Studio 
	6 - Instalar ˜\misc\AccessDatabaseEngine.exe - Driver para acesso a planila de produtos
	5 - Alterar planilha de produtos importar através do PDV-SAT
	6 - Copiar as dlls de ˜\libs\Copiar Conteudo\*.dll para ˜\Syslaps.Pdv.UI\bin\debug\  
	7 - Alterar arquivo de configuração
```
...
<add key="NomeDoCaixa" value="pdv1" /> <!-- Nome de identificação do Caixa -->
<add key="TituloInicial" value="PDV - SAT - v2.0" /> <!-- Titulo das Janelas -->
<add key="Cultura" value="pt-BR" /> <!-- Idioma Padrão -->
<add key="CaminhoFisicoDaPlanilhaDeProdutos" value="C:\Projects\luissanches\pdv-sat\misc\Produtos.xls" /> <!-- Caminho da planilha de Produtos -->
<add key="CaminhoFisicoDoBatSincronizador" value="C:\Projects\Bolaria\Pdv\Node\sync\syncronize.bat" /> <!-- Caminho do bat com script para sincronização dos dados para relatórios Web -->
<add key="MascaraCPF" value="999,999,999-99" /> <!-- Máscara CPF -->
<add key="MascaraCNPJ" value="99,999,999/9999-99" /> <!-- Máscara CNPJ -->
...
<connectionStrings>
	<add name="Repositorio" connectionString="Data Source=C:\Projects\luissanches\pdv-sat\misc\db.sl3;Version=3;" /> <!-- Caminho do banco SQLite -->
</connectionStrings>
```

### Prints

[Login]
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/login.png)
=============

[Tela Principal]
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/main.png)
=============

[Tela de Venda de Produtos] 
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/sell.png)
=============

[Tela de Pagamentos da Venda]
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/payment.png)
=============

[SAT OffLine]
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/sat_offline.png)
=============

[Tabela de Parametros]
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/parametros.png)
=============

[Arquivo de Configuração]
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/config.png)
=============

[Planilha com Produtos]
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/produtos.png)
=============

