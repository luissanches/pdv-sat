# Contribuindo com o PDV-SAT

Este projeto adota o **Git Flow** como modelo de ramificação. Leia este guia
antes de abrir branches ou Pull Requests.

## Branches permanentes

| Branch | Papel |
|---|---|
| `master` | Código **em produção**. Cada commit aqui é uma versão liberada e recebe uma _tag_ de versão. Só recebe merge de `release/*` e `hotfix/*`. |
| `develop` | Linha de **integração** do próximo lançamento. É a base de onde saem e para onde voltam as `feature/*`. |

## Branches de apoio (temporárias)

| Prefixo | Sai de | Volta para | Para quê |
|---|---|---|---|
| `feature/<descricao>` | `develop` | `develop` | Novas funcionalidades e melhorias. |
| `release/<versao>` | `develop` | `develop` **e** `master` | Estabilização de uma versão (ajustes finais, bump de versão). |
| `hotfix/<versao>` | `master` | `master` **e** `develop` | Correção urgente de algo já em produção. |

Use nomes em **português**, minúsculos e com hífen, ex.:
`feature/desconto-por-cliente`, `release/2.1.0`, `hotfix/2.0.1`.

## Fluxo de uma feature

```bash
# parte sempre da develop atualizada
git checkout develop
git pull origin develop

git checkout -b feature/minha-funcionalidade
# ... commits ...
git push -u origin feature/minha-funcionalidade
```

Abra o **Pull Request da `feature/*` para `develop`**. Após o merge, apague a
branch da feature.

## Fluxo de release

```bash
git checkout -b release/2.1.0 develop
# ajustes finais + atualizar versão
git push -u origin release/2.1.0
```

Merge da `release/*` em `master` (com _tag_ `2.1.0`) **e** de volta em `develop`.

## Fluxo de hotfix

```bash
git checkout -b hotfix/2.0.1 master
# correção urgente
git push -u origin hotfix/2.0.1
```

Merge da `hotfix/*` em `master` (com _tag_) **e** em `develop`.

## Commits

- Mensagens em **português**, no imperativo, descrevendo o "porquê".
- Não comite `bin/`, `obj/` nem arquivos de configuração local com dados sensíveis
  (caminhos de banco, senhas de e-mail, código de ativação do SAT). Ver `.gitignore`.

## Versionamento

Versões seguem `MAJOR.MINOR.PATCH` (SemVer). Toda liberação em `master` recebe uma
_tag_ correspondente.
