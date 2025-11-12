
### README Padronizado

# Sistema de Gestão de Mini Mercado

> Aplicação de console em C# para gestão de estoque e vendas de um mini mercado, focada na aplicação de conceitos de Programação Orientada a Objetos.

[![Status](https://img.shields.io/badge/Status-Funcional-success)](https://github.com/seu-usuario/mini-mercado-csharp)
[![C#](https://img.shields.io/badge/C%23-.NET-239120)](https://docs.microsoft.com/pt-br/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-6.0+-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-Proprietary-red.svg)](LICENSE)

## Sobre o Projeto

O **Sistema de Gerenciamento de Mini Mercado** é uma aplicação de console desenvolvida em C# como um projeto prático para aplicar e solidificar conceitos de **Programação Orientada a Objetos (POO)**. Ele simula as operações essenciais de um pequeno comércio, como controle de estoque e registro de vendas, com persistência de dados em arquivos JSON para manter as informações entre as execuções.

## ✨ Funcionalidades

### 📦 Gestão de Produtos (CRUD)
-   Cadastro, listagem, busca, atualização e exclusão de produtos.
-   Validação para impedir códigos duplicados.

### 🛒 Registro de Vendas
-   Lançamento de vendas com múltiplos itens.
-   Atualização automática da quantidade de produtos em estoque após cada venda.
-   Verificação de disponibilidade de estoque antes de confirmar a venda.

### 📊 Geração de Relatórios
-   Listagem de produtos com estoque abaixo do mínimo definido.
-   Consulta ao histórico completo de vendas realizadas.

### 💾 Persistência de Dados
-   Todos os dados de produtos e vendas são salvos em arquivos `produtos.json` e `vendas.json`, garantindo que as informações não sejam perdidas.

## Tecnologias

### Core
-   **C#** - Linguagem principal.
-   **.NET 6.0+** - Plataforma de desenvolvimento.

### Dados
-   **Newtonsoft.Json** - Biblioteca para serialização e desserialização de dados em formato JSON.

## Pré-requisitos

-   [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) ou superior.

## Instalação e Uso

1.  **Clone o repositório**
    ```bash
    git clone https://github.com/seu-usuario/mini-mercado-csharp.git
    cd mini-mercado-csharp
    ```

2.  **Restaure as dependências**
    O .NET CLI irá restaurar o pacote `Newtonsoft.Json` automaticamente. Se necessário, execute:
    ```bash
    dotnet restore
    ```

3.  **Execute a aplicação**
    ```bash
    dotnet run
    ```
    O menu interativo será exibido no seu console.

## Estrutura do Projeto

O sistema foi modelado utilizando os princípios da POO, com as seguintes classes principais:

-   **`Produto`**: Representa a entidade produto com todos os seus atributos.
-   **`Estoque`**: Classe responsável por gerenciar a coleção de produtos e suas operações (CRUD).
-   **`Venda`** e **`ItemVenda`**: Representam uma transação de venda e os produtos contidos nela.
-   **`GerenciadorVendas`**: Controla o processo de registro e histórico de vendas.
-   **`Sistema`**: Orquestra a interação com o usuário através do menu de console.

## Contribuição

Contribuições para melhorar o projeto são bem-vindas!

1.  Faça um Fork do projeto.
2.  Crie sua Feature Branch (`git checkout -b feature/NovaFuncionalidade`).
3.  Faça Commit de suas mudanças (`git commit -m 'Adiciona funcionalidade X'`).
4.  Faça Push para a Branch (`git push origin feature/NovaFuncionalidade`).
5.  Abra um Pull Request.

## Suporte e Contato

-   **Email**: [g.moreno.souza05@gmail.com](mailto:g.moreno.souza05@gmail.com)
-   **LinkedIn**: [Gustavo Moreno Souza](https://www.linkedin.com/in/gustavo-moreno-8a925b26a/)

## Licença

Este projeto está licenciado sob uma Licença Proprietária.

**Uso Restrito**: Este software foi desenvolvido para fins educacionais e de portfólio. Uso comercial ou redistribuição requer autorização expressa.

---

<div align="center">
  Desenvolvido por Gustavo Moreno Souza
  <br><br>
  <a href="https://www.linkedin.com/in/gustavo-moreno-8a925b26a/" target="_blank">
    <img src="https://cdn-icons-png.flaticon.com/512/174/174857.png" width="24" alt="LinkedIn"/>
  </a>
</div>
