# Sistema de Gerenciamento de Mini Mercado

Um aplicativo de console em C# para gerenciar o estoque e vendas de um mini mercado, desenvolvido como projeto prático de programação orientada a objetos.

## 📋 Funcionalidades

O sistema oferece as seguintes funcionalidades:

- **Gerenciamento de Produtos**
  - Cadastrar novos produtos
  - Listar todos os produtos
  - Buscar produtos por código ou nome
  - Atualizar informações de produtos
  - Excluir produtos

- **Gerenciamento de Vendas**
  - Registrar vendas de produtos
  - Atualização automática do estoque após vendas
  - Cancelamento de vendas em andamento

- **Relatórios**
  - Produtos com estoque abaixo do mínimo
  - Histórico de vendas realizadas
  - Estatísticas de vendas (total, valor, itens)

- **Persistência de Dados**
  - Armazenamento de dados em arquivos JSON
  - Manutenção dos dados entre execuções do programa

## 🛠️ Estrutura do Projeto

O sistema é organizado nas seguintes classes principais:

- `Produto`: Representa os produtos do mini mercado
- `Estoque`: Gerencia a lista de produtos e operações relacionadas
- `Venda`: Representa uma venda com seus itens
- `ItemVenda`: Representa um item dentro de uma venda
- `GerenciadorVendas`: Gerencia as operações de vendas
- `Sistema`: Controla a interface do usuário no console

## 📝 Campos do Produto

Cada produto possui os seguintes atributos:

| Campo | Descrição |
|-------|-----------|
| Código | Identificador único do produto |
| Nome | Nome do produto |
| Descrição | Descrição detalhada do produto |
| Preço | Valor unitário de venda |
| QuantidadeEmEstoque | Quantidade disponível em estoque |
| EstoqueMinimo | Quantidade mínima que deve haver em estoque |
| DataCadastro | Data de cadastro do produto |

## 📋 Menu do Sistema

O sistema apresenta as seguintes opções no menu principal:

1. Cadastrar Produto
2. Listar Produtos
3. Buscar Produto
4. Atualizar Produto
5. Excluir Produto
6. Realizar Venda
7. Gerar Relatórios
8. Sair

## 🚀 Como Executar

### Pré-requisitos

- .NET Framework 4.7.2 ou superior
- OU .NET Core 3.1 ou superior
- OU .NET 5.0+

### Passos para Execução

1. Clone o repositório:
   ```
   git clone https://github.com/seu-usuario/mini-mercado.git
   ```

2. Navegue até a pasta do projeto:
   ```
   cd mini-mercado
   ```

3. Compile o projeto:
   ```
   dotnet build
   ```

4. Execute o aplicativo:
   ```
   dotnet run
   ```

### Dependências

O projeto utiliza:
- `Newtonsoft.Json` para serialização e desserialização JSON

Para adicionar a dependência:
```
dotnet add package Newtonsoft.Json
```

## 📂 Armazenamento de Dados

Os dados são armazenados em dois arquivos JSON:
- `produtos.json`: Armazena informações dos produtos
- `vendas.json`: Armazena o histórico de vendas realizadas

## 📊 Exemplos de Uso

### Cadastrar um Produto

1. Selecione a opção 1 no menu principal
2. Informe os dados solicitados:
   - Código (número inteiro único)
   - Nome 
   - Descrição
   - Preço
   - Quantidade em estoque
   - Estoque mínimo

### Realizar uma Venda

1. Selecione a opção 6 no menu principal
2. Escolha a opção 1 para adicionar um item
3. Informe o código do produto
4. Informe a quantidade desejada
5. Repita os passos 2-4 para adicionar mais itens
6. Escolha a opção 2 para finalizar a venda

## 🔒 Validações Implementadas

O sistema inclui as seguintes validações:

- Não permite cadastro de produtos com código duplicado
- Verifica se há estoque suficiente para vendas
- Valida entradas numéricas e valores negativos
- Impede exclusão de produtos não existentes
- Verifica dados obrigatórios no cadastro

## ✨ Possíveis Melhorias Futuras

- Interface gráfica com Windows Forms ou WPF
- Integração com banco de dados SQL
- Controle de usuários e permissões
- Impressão de comprovantes de vendas
- Controle financeiro e caixa
- Geração de relatórios mais detalhados
- Controle de promoções e descontos

## 👨‍💻 Autor

[Gustavo Moreno Souza](https://github.com/seu-usuario)

---

Desenvolvido como projeto de estudo e prática de programação em C#.
