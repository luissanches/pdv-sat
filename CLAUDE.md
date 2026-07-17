# CLAUDE.md

Guia para o Claude Code (e contribuidores) trabalharem neste repositório.

## Visão geral

**PDV-SAT** é um Ponto de Venda (PDV) para varejo com integração ao **SAT-CF-e**
(Sistema Autenticador e Transmissor de Cupons Fiscais Eletrônicos) do Estado de
São Paulo. É uma aplicação **desktop Windows (WPF)** que registra vendas, emite
cupom fiscal via equipamento SAT, controla caixa, produção, encomendas/comandas,
clientes e gera o arquivo XML de movimento para o fisco.

> Todo o domínio, nomes de classes, métodos e comentários estão em **português
> (pt-BR)**. Mantenha esse idioma ao escrever ou alterar código.

## Stack

- **.NET Framework 4.6.1** (não é .NET Core/5+) — `TargetFrameworkVersion v4.6.1`
- **C#** + **WPF** (Windows Presentation Foundation)
- **Plataforma x86** obrigatória na compilação (por causa das DLLs nativas do
  SAT e da impressora). Windows apenas.
- **Dapper** + **Dapper.FastCrud** (micro-ORM)
- **SQLite** (`System.Data.SQLite`) — banco em arquivo (`db.sl3`)
- **StructureMap** (injeção de dependência / IoC)
- **log4net** (logging)
- **MSTest** (`Microsoft.VisualStudio.TestTools.UnitTesting`) para testes

## Estrutura da solução (`Syslaps.Pdv.sln`)

Arquitetura em camadas. Dependências fluem **UI → Core → Entity → Cross**, com
**Infra** implementando as interfaces de `Core`.

| Projeto | Tipo | Responsabilidade |
|---|---|---|
| `Syslaps.Pdv.Cross` | Library | Utilitários transversais: `Utils` (`GerarCodigoUnico`, `RecuperarIp`), `Extensions` (helpers de string/data/decimal). |
| `Syslaps.Pdv.Entity` | Library | POCOs/entidades persistidas, mapeadas via DataAnnotations (`[Table]`, `[Key]`, `[StringLength]`). Inclui os modelos de resposta do SAT em `Entity/SAT/`. |
| `Syslaps.Pdv.Core` | Library | **Domínio**. Cada agregado vive em `Dominio/<Contexto>/`: o modelo de domínio (ex.: `Venda`, `Caixa`), a interface do repositório (`I<Coisa>Repositorio`) e enums. Classes-base em `Dominio/Base/`. `Bootstrap` inicializa a aplicação. |
| `Syslaps.Pdv.Infra` | Library | **Implementações**. Repositórios Dapper (`Repositorio/`), drivers do SAT (`SAT/`), impressora (`Impressora/T20/`), `Email`, `Logger`. |
| `Syslaps.Pdv.UI` | WinExe (WPF) | Telas (`Telas/<Contexto>/`), `App.xaml.cs` (startup), `ContainerIoc` (StructureMap), `InstanceManager` (estado global da sessão). |
| `Syslaps.Pdv.Test` | Library | Testes MSTest + seu próprio `ContainerIoc`. |
| `Syslaps.Pdv.TestPrinter` | Exe | Aplicação auxiliar para testar a impressora isoladamente. |

## Padrões e convenções importantes

### Modelos de domínio (`Core/Dominio`)
- Herdam de `ModeloBase` (que herda de `Utils`).
- Acumulam mensagens/erros via `AdicionarMensagem(msg, EnumStatusDoResultado)`;
  o consumidor lê `Status` e `Mensagem`. **Não** lance exceções para regras de
  negócio — use `EnumStatusDoResultado.RegraDeNegocioInvalida`. Ver `Dominio/Venda/Venda.cs`.
- Recebem dependências (repositórios, outros agregados) via **construtor** —
  resolvidas pelo StructureMap.
- Códigos de chave são GUIDs gerados por `GerarCodigoUnico()` (`Guid.NewGuid().ToString("N")`).

### Repositórios
- Interface em `Core/Dominio/<Contexto>/I<Coisa>Repositorio.cs`; implementação em
  `Infra/Repositorio/Repositorio<Coisa>.cs`, herdando de `RepositorioBase`.
- `RepositorioBase` expõe a conexão `Db` (SQLite, connection string `"Repositorio"`),
  CRUD genérico via Dapper.FastCrud (`Inserir`/`Atualizar`/`Excluir`/`Recuperar`/`RecuperarTodos`).
- Consultas específicas usam SQL direto com Dapper (`Db.Query<T>(sql, params)`),
  sempre com **parâmetros nomeados** (`@Param`) — nunca concatene valores no SQL.

### Entidades (`Entity`)
- POCOs com DataAnnotations. Coleções de navegação são `virtual ICollection<>`
  inicializadas no construtor (`HashSet<>`). Nomes de FK seguem o padrão
  `OutraEntidade_CodigoOutraEntidade`.

### IoC (StructureMap)
- Registrado em `ContainerIoc` (existe um em `UI` e outro em `Test` —
  mantenha-os em sincronia ao adicionar dependências novas).
- Ao criar um novo agregado: registre `I<Coisa>Repositorio → Repositorio<Coisa>`.
- Resolva com `ContainerIoc.GetInstance<T>()`.

### UI / WPF
- Padrão **MVVM** leve: cada tela tem `Tela.xaml` + `Tela.xaml.cs` (code-behind) +
  `TelaMvvm.cs` (ViewModel com `INotifyPropertyChanged`). O ViewModel recebe os
  repositórios por construtor e é resolvido pelo `ContainerIoc`.
- Estado de sessão (caixa aberto, usuário logado, parâmetros, listas em memória)
  fica em `InstanceManager` (estático).

### SAT (fiscal)
- `Infra/SAT/SatBase` é a base abstrata; `SatBase.Create(activationCode, SatModelEnum)`
  é a **factory** que instancia o driver da marca (Sweda, Bematech, Elgin, Elgin2,
  Gertec, Urano, Kryptus, Dimep, Tanca) ou `OffLine` (modo sem hardware).
- Ao adicionar suporte a um novo equipamento: crie a classe em `Infra/SAT/`,
  adicione o valor em `Entity/SAT/SatModelEnum.cs` e o `case` em `SatBase.Create`.

## Build e execução

> Requer **Windows + Visual Studio** (WPF, DLLs nativas x86, equipamento SAT).
> Não compila/roda em Linux/macOS nem em CI sem Windows.

Configuração de desenvolvimento (ver `README.md` para detalhes):
1. Compilar em **x86** (DLLs do SAT e da impressora).
2. Definir `Syslaps.Pdv.UI` como **Startup Project**.
3. Copiar as DLLs de `libs/Copiar Conteudo/*.dll` para `Syslaps.Pdv.UI/bin/debug/`.
4. Instalar `misc/AccessDatabaseEngine.exe` (driver para importar a planilha de produtos).
5. Ajustar `App.config`: `connectionStrings["Repositorio"]` (caminho do `db.sl3`),
   `AppSettings` (NomeDoCaixa, Cultura, caminho da planilha etc.).
6. Configurar a tabela `Parametros` (dados da empresa, e-mail, SAT) no SQLite.

Testes: projeto `Syslaps.Pdv.Test` (MSTest) — rodar pelo Test Explorer do Visual Studio.

## Modelo de branches (Git Flow)

O projeto usa **Git Flow**. Resumo (detalhes em `CONTRIBUTING.md`):

- `master` — código em produção; recebe _tags_ de versão. Só vem de `release/*` e `hotfix/*`.
- `develop` — integração do próximo lançamento; base das features.
- `feature/<descricao>` sai e volta para `develop`.
- `release/<versao>` sai de `develop`, vai para `master` (com _tag_) e volta para `develop`.
- `hotfix/<versao>` sai de `master`, vai para `master` (com _tag_) e volta para `develop`.

Nomes em português, minúsculos, com hífen (ex.: `feature/desconto-por-cliente`).

## Ao alterar código, lembre-se

- Mantenha **português** em nomes e comentários.
- Respeite as camadas: regra de negócio em `Core`, acesso a dados/hardware em `Infra`,
  nada de SQL ou lógica de domínio no code-behind da UI.
- Para erros de regra de negócio use `AdicionarMensagem` + `EnumStatusDoResultado`,
  não exceções.
- SQL sempre parametrizado.
- Ao adicionar um repositório/serviço, registre nos **dois** `ContainerIoc` (UI e Test).
- Não comite a pasta `bin`/`obj` (ver `.gitignore`).
