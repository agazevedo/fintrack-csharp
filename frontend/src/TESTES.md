# Testes do Frontend

Este documento descreve o que é verificado nos testes automatizados do frontend do Fintrack. Cada teste corresponde a uma situação real de uso.

Hoje o frontend tem apenas **testes unitários**, todos da parte que conversa com a API: login, envio do token de acesso e as operações de categorias, itens de orçamento e despesas. A API não é chamada de verdade; os testes simulam as respostas dela (sucesso, erro, sessão expirada) e verificam se o frontend envia os pedidos certos e reage da forma esperada.

Para rodar os testes, use dentro da pasta `frontend`:

```bash
npm run test:ci
```

---

## Testes unitários

### Comunicação com a API

| Caso de uso | Resultado esperado |
|---|---|
| Um visitante que não fez login faz um pedido à API. | O pedido é enviado sem credenciais de acesso. |
| Um usuário logado faz um pedido à API. | O pedido é enviado junto com o token de acesso do usuário. |
| O frontend faz um pedido à API com informações extras. | O pedido vai para o endereço correto, com as credenciais do usuário e as informações extras. |
| A API responde a um pedido. | A resposta chega ao frontend sem alterações. |
| A sessão do usuário expira enquanto ele usa o sistema. | O token é apagado e o usuário é levado para a tela de login. |
| O usuário erra a senha na tela de login. | Ele continua na tela de login, sem ser redirecionado e sem perder dados salvos. |

### Login e sessão

| Caso de uso | Resultado esperado |
|---|---|
| O usuário preenche usuário e senha e clica em entrar. | Os dados são enviados para o login da API. |
| O usuário faz login com dados corretos. | O login é concluído e a sessão fica salva no navegador. |
| O usuário faz login com dados errados. | O login falha e nenhuma sessão é salva. |
| O usuário tenta fazer login sem conexão com a internet. | O login falha de forma controlada, sem travar a tela. |
| O sistema verifica se o usuário está logado. | Sem sessão salva, ele é tratado como deslogado; com sessão salva, como logado. |
| O usuário clica em sair. | A sessão é encerrada e ele passa a ser tratado como deslogado. |

### Categorias, itens de orçamento e despesas

As três telas de cadastro funcionam do mesmo jeito (listar, criar, editar e excluir), então os mesmos casos de uso são testados para cada uma delas:

| Caso de uso | Resultado esperado |
|---|---|
| O usuário abre a lista. | Os registros são buscados na API com o token do usuário e entregues à tela. |
| A lista não carrega porque a API falhou. | É gerada uma mensagem de erro (ex.: "Erro ao buscar categorias"). |
| O usuário cadastra um novo registro. | Os dados preenchidos são enviados à API e o registro criado é devolvido. |
| O cadastro é recusado pela API (ex.: dados inválidos). | A mensagem de erro é a que a própria API enviou. |
| O usuário edita um registro. | As alterações são enviadas para o registro certo e a versão atualizada é devolvida. |
| A edição é recusada e a API explica o motivo. | A mensagem de erro é a explicação enviada pela API. |
| A edição é recusada e a API não explica o motivo. | É usada uma mensagem padrão (ex.: "Erro ao atualizar categoria"). |
| O usuário exclui um registro. | O registro certo é excluído e a exclusão é confirmada. |
| A exclusão falha (ex.: o registro não existe mais). | É gerada uma mensagem de erro (ex.: "Erro ao deletar categoria"). |

Exemplos de dados usados em cada tela:

| Tela | Exemplo de registro | Mensagens de erro padrão |
|---|---|---|
| Categorias | Categoria "Mercado", do tipo despesa | Erro ao buscar / atualizar / deletar categoria(s) |
| Itens de orçamento | "Aluguel", 1 unidade de R$ 1.500 | Erro ao buscar / atualizar / deletar item(ns) |
| Despesas | 2 unidades de R$ 80 em 01/11/2026 | Erro ao buscar / atualizar / deletar despesa(s) |

---

## Testes de integração

O frontend **ainda não tem** testes de integração nem testes das telas (botões, formulários, navegação). O funcionamento conjunto entre frontend e API é verificado pelos testes de integração da API, descritos em `Fintrack.API.Tests/TESTES.md`.

## Resumo

| Área | Quantidade de testes |
|---|---|
| Comunicação com a API | 6 |
| Login e sessão | 6 |
| Categorias | 9 |
| Itens de orçamento | 9 |
| Despesas | 9 |
| **Total** | **39** |

O que os testes garantem, de forma geral:

- Todo pedido de um usuário logado vai com o token de acesso dele.
- Quando a sessão expira, o usuário volta para a tela de login.
- Login, logout e verificação de sessão funcionam como esperado.
- Em qualquer erro, é gerada uma mensagem clara: a da API, quando ela explica o motivo, ou uma mensagem padrão. Se essa mensagem aparece na tela não é verificado, porque as telas ainda não têm testes.
