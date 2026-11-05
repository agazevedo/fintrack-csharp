# Testes da API

Este documento descreve o que é verificado nos testes automatizados da API do Fintrack. Cada teste corresponde a uma situação real de uso do sistema.

Os testes são de dois tipos:

- **Testes unitários:** verificam cada regra de negócio de forma isolada, sem precisar subir a API.
- **Testes de integração:** sobem a API completa e fazem requisições como um cliente de verdade faria, passando por login, permissões e validações.

Nenhum dos dois precisa de banco de dados instalado; os testes usam um banco temporário que é descartado no final.

Para rodar todos os testes, use na raiz do projeto:

```bash
dotnet test
```

---

## Testes unitários

### Cadastro e login

| Caso de uso | Resultado esperado |
|---|---|
| Uma pessoa se cadastra com usuário e senha novos. | O cadastro é aceito e ela recebe um token de acesso. A senha é guardada de forma criptografada, nunca em texto puro. |
| Uma pessoa tenta se cadastrar com um nome de usuário que já existe. | O cadastro é recusado e nenhum usuário duplicado é criado. |
| Um usuário faz login com usuário e senha corretos. | O login é aceito e ele recebe um token de acesso. |
| Um usuário tenta fazer login com a senha errada. | O acesso é negado. |
| Alguém tenta fazer login com um usuário que não existe. | O acesso é negado. |

### Token de acesso

| Caso de uso | Resultado esperado |
|---|---|
| O sistema gera o token de um usuário que acabou de entrar. | O token identifica corretamente o usuário (id e nome). |
| O sistema gera um token de acesso. | O token vale por 7 dias e foi emitido pelo próprio Fintrack. |
| A API recebe um token gerado por ela mesma. | O token é reconhecido como válido e o usuário é identificado. |

### Categorias

| Caso de uso | Resultado esperado |
|---|---|
| O usuário abre a lista de categorias. | Ele vê apenas as próprias categorias, nunca as de outros usuários. |
| O usuário cria uma nova categoria. | A categoria é criada e fica vinculada a ele. |
| O usuário tenta criar uma categoria com um nome que já usa, mesmo escrito com maiúsculas e minúsculas diferentes (ex.: "Lazer" e "LAZER"). | A criação é recusada. |
| O usuário renomeia uma de suas categorias. | A categoria é atualizada com o novo nome. |
| O usuário tenta editar uma categoria de outra pessoa. | O sistema responde como se a categoria não existisse. |
| O usuário exclui uma de suas categorias. | A categoria é removida. |
| O usuário tenta excluir uma categoria que não existe. | O sistema informa que ela não foi encontrada. |

### Itens de orçamento

| Caso de uso | Resultado esperado |
|---|---|
| O usuário cria um item de orçamento em uma de suas categorias. | O item é criado, vinculado a ele, e o total orçado é calculado. |
| O usuário tenta criar um item usando a categoria de outra pessoa. | A criação é recusada. |
| O usuário abre a lista de itens de orçamento. | Ele vê apenas os próprios itens. |
| O usuário edita um item de orçamento. | Os novos valores são salvos e a data da última alteração é registrada. |
| O usuário tenta editar um item que não existe. | O sistema informa que ele não foi encontrado. |
| O usuário exclui um item de orçamento. | O item é removido. |
| O usuário tenta excluir um item de outra pessoa. | O sistema responde como se o item não existisse. |

### Despesas

| Caso de uso | Resultado esperado |
|---|---|
| O usuário lança uma despesa em um de seus itens de orçamento (ex.: 2 unidades de R$ 80). | A despesa é registrada com o total correto (R$ 160). |
| O usuário tenta lançar uma despesa no item de orçamento de outra pessoa. | O lançamento é recusado. |
| O usuário abre a lista de despesas. | Ele vê apenas as despesas dos próprios itens. |
| O usuário corrige o valor e a data de uma despesa. | A despesa é atualizada. |
| O usuário tenta editar a despesa de outra pessoa. | O sistema responde como se a despesa não existisse. |
| O usuário exclui uma despesa. | A despesa é removida. |
| O usuário tenta excluir uma despesa que não existe. | O sistema informa que ela não foi encontrada. |

### Cálculos financeiros

| Caso de uso | Resultado esperado |
|---|---|
| Uma despesa tem 3 unidades de R$ 12,50. | O total da despesa é R$ 37,50 (quantidade × valor unitário). |
| Um item orçado em R$ 200 tem duas despesas que somam R$ 110. | O sistema mostra R$ 200 orçado, R$ 110 gasto e R$ 90 restante. |
| Um item orçado em R$ 50 ainda não tem despesas. | O gasto é zero e o restante é igual ao valor orçado. |

---

## Testes de integração

Aqui a API é usada de ponta a ponta, como o frontend faria. Em cada teste, um usuário novo é cadastrado para que um teste não interfira no outro.

### Cadastro, login e acesso protegido

| Caso de uso | Resultado esperado |
|---|---|
| Uma pessoa se cadastra pela API. | Ela recebe um token de acesso válido. |
| Uma pessoa tenta se cadastrar com um nome de usuário já usado. | O cadastro é recusado. |
| Uma pessoa tenta se cadastrar com uma senha curta demais. | O cadastro é recusado. |
| Um usuário cadastrado faz login com os dados corretos. | Ele recebe um token de acesso. |
| Um usuário tenta fazer login com a senha errada. | O acesso é negado. |
| Alguém tenta acessar uma área restrita sem estar logado. | O acesso é negado. |
| Alguém tenta acessar uma área restrita com um token falso. | O acesso é negado. |
| Um usuário logado acessa uma área restrita. | O acesso é permitido. |

### Categorias

| Caso de uso | Resultado esperado |
|---|---|
| Alguém tenta listar categorias sem estar logado. | O acesso é negado. |
| O usuário cria a categoria "Mercado" e depois abre a lista. | A categoria aparece na lista com o nome e o tipo corretos. |
| O usuário tenta criar "mercado" quando já tem "Mercado". | A criação é recusada. |
| O usuário tenta criar uma categoria sem informar o nome. | A criação é recusada. |
| O usuário renomeia "Mercado" para "Supermercado". | A categoria é atualizada. |
| O usuário tenta editar uma categoria que não existe. | O sistema informa que ela não foi encontrada. |
| O usuário exclui uma categoria. | Ela deixa de aparecer na lista. |
| Outra pessoa tenta ver, editar ou excluir a categoria de um usuário. | Ela não vê a categoria e não consegue editar nem excluir. |

### Itens de orçamento

| Caso de uso | Resultado esperado |
|---|---|
| Alguém tenta listar itens de orçamento sem estar logado. | O acesso é negado. |
| O usuário cria o item "Aluguel" (2 × R$ 1.000) e depois abre a lista. | O item aparece com a categoria, R$ 2.000 orçado, nada gasto e R$ 2.000 restante. |
| O usuário tenta criar um item em uma categoria que não existe. | A criação é recusada. |
| O usuário tenta criar um item na categoria de outra pessoa. | A criação é recusada. |
| O usuário reajusta o aluguel para R$ 1.200. | O item é atualizado com a nova descrição e o novo total. |
| O usuário tenta editar um item que não existe. | O sistema informa que ele não foi encontrado. |
| O usuário exclui um item de orçamento. | Ele deixa de aparecer na lista. |
| O usuário tenta excluir um item que não existe. | O sistema informa que ele não foi encontrado. |

### Despesas

| Caso de uso | Resultado esperado |
|---|---|
| Alguém tenta listar despesas sem estar logado. | O acesso é negado. |
| O usuário lança uma despesa de 2 × R$ 150 e depois abre a lista. | A despesa aparece com o total de R$ 300, o item e a data corretos. |
| O usuário lança uma despesa de R$ 300 em um item de orçamento. | O item passa a mostrar R$ 300 gasto e o restante diminui no mesmo valor. |
| O usuário tenta lançar uma despesa em um item que não existe. | O lançamento é recusado. |
| O usuário tenta lançar uma despesa no item de outra pessoa. | O lançamento é recusado. |
| O usuário corrige o valor e a data de uma despesa. | A despesa é atualizada. |
| O usuário tenta editar uma despesa que não existe. | O sistema informa que ela não foi encontrada. |
| O usuário exclui uma despesa. | Ela deixa de aparecer na lista. |
| Outra pessoa tenta excluir a despesa de um usuário. | A exclusão não acontece; o sistema responde como se a despesa não existisse. |

---

## Resumo

| Tipo | Quantidade de testes |
|---|---|
| Unitários | 32 |
| Integração | 33 |
| **Total** | **65** |

O que os testes garantem, de forma geral:

- Só quem está logado acessa os dados.
- Cada usuário vê e altera apenas os próprios dados.
- Não é possível criar registros duplicados ou com informações inválidas.
- Os valores de orçado, gasto e restante são sempre calculados corretamente.
