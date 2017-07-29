# PDV-SAT

### Descrição ###

Ponto de Venda de Produtos com integração SAT.
Transmite as informações das operações comerciais dos contribuintes varejistas do estado de São Paulo,
utilizando Internet, através de equipamento SAT.

=============

### Funcionalidades ###

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


### Configuração para desenvolvimento ###

1 - Configurar projeto para compilar em x86 (por causa das dlls do SAT e Impressora) <br>
2 - Alterar arquivo de configuração: <br>
```
...
<add key="NomeDoCaixa" value="pdv1" /> <!-- Nome de identificação do Caixa -->
<add key="TituloInicial" value="PDV - SAT - v2.0" /> <!-- Titulo das Janelas -->
<add key="Cultura" value="pt-BR" /> <!-- Idioma Padrão -->
<add key="CaminhoFisicoDaPlanilhaDeProdutos" value="C:\Projects\luissanches\pdv-sat\misc\Produtos.xls" /> <!-- Caminho da planilha de Produtos -->
<add key="CaminhoFisicoDoBatSincronizador" value="C:\Projects\Bolaria\Pdv\Node\sync\syncronize.bat" /> <!-- Caminho do bat com script para sincronização dos dados para relatórios Web -->
<add key="MascaraCPF" value="999,999,999-99" /> <!-- Máscara CPF -->
<add key="MascaraCNPJ" value="99,99,999/9999-99" /> <!-- Máscara CNPJ -->
...
<connectionStrings>
	<add name="Repositorio" connectionString="Data Source=C:\Projects\luissanches\pdv-sat\misc\db.sl3;Version=3;" /> <!-- Caminho do banco SQLite -->
</connectionStrings>
```

3 - Alterar tabela de Parametros<br>
4 - Alterar tabela de Produtos<br>
5 - Alterar planilha de produtos importar através do PDV-SAT<br>
6 - Configuração concluída<br>
-------------

### Prints ###

![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/login.png)
=============
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/main.png)
=============
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/sell.png)
=============
![](https://github.com/luissanches/pdv-sat/blob/master/misc/printscreen/payment.png)
