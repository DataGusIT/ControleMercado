using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MiniMercado
{
    // Classe que representa um produto
    public class Produto
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeEmEstoque { get; set; }
        public int EstoqueMinimo { get; set; }
        public DateTime DataCadastro { get; set; }

        public override string ToString()
        {
            return $"Código: {Codigo} | Nome: {Nome} | Preço: R${Preco:F2} | Estoque: {QuantidadeEmEstoque} | Estoque Mínimo: {EstoqueMinimo}";
        }
    }

    // Classe que representa uma venda
    public class Venda
    {
        public int Id { get; set; }
        public List<ItemVenda> Itens { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal => Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

        public Venda()
        {
            Itens = new List<ItemVenda>();
            DataVenda = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Venda #{Id} - Data: {DataVenda} - Valor Total: R${ValorTotal:F2} - Itens: {Itens.Count}";
        }
    }

    // Classe que representa um item de venda
    public class ItemVenda
    {
        public int CodigoProduto { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;

        public override string ToString()
        {
            return $"{NomeProduto} - Qtd: {Quantidade} - Preço: R${PrecoUnitario:F2} - Subtotal: R${Subtotal:F2}";
        }
    }

    // Classe que gerencia o estoque
    public class Estoque
    {
        private List<Produto> _produtos;
        private string _arquivoProdutos = "produtos.json";

        public Estoque()
        {
            CarregarProdutos();
        }

        // Métodos para gerenciar produtos
        public void AdicionarProduto(Produto produto)
        {
            if (_produtos.Any(p => p.Codigo == produto.Codigo))
            {
                throw new Exception("Já existe um produto com este código.");
            }

            _produtos.Add(produto);
            SalvarProdutos();
        }

        public List<Produto> ListarProdutos()
        {
            return _produtos;
        }

        public Produto BuscarProdutoPorCodigo(int codigo)
        {
            return _produtos.FirstOrDefault(p => p.Codigo == codigo);
        }

        public List<Produto> BuscarProdutosPorNome(string nome)
        {
            return _produtos.Where(p => p.Nome.ToLower().Contains(nome.ToLower())).ToList();
        }

        public void AtualizarProduto(Produto produto)
        {
            var produtoExistente = BuscarProdutoPorCodigo(produto.Codigo);
            if (produtoExistente == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            // Atualiza as propriedades
            produtoExistente.Nome = produto.Nome;
            produtoExistente.Descricao = produto.Descricao;
            produtoExistente.Preco = produto.Preco;
            produtoExistente.QuantidadeEmEstoque = produto.QuantidadeEmEstoque;
            produtoExistente.EstoqueMinimo = produto.EstoqueMinimo;

            SalvarProdutos();
        }

        public void ExcluirProduto(int codigo)
        {
            var produto = BuscarProdutoPorCodigo(codigo);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            _produtos.Remove(produto);
            SalvarProdutos();
        }

        public void AtualizarEstoque(int codigoProduto, int quantidade)
        {
            var produto = BuscarProdutoPorCodigo(codigoProduto);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            if (produto.QuantidadeEmEstoque < quantidade)
            {
                throw new Exception("Quantidade insuficiente em estoque.");
            }

            produto.QuantidadeEmEstoque -= quantidade;
            SalvarProdutos();
        }

        public List<Produto> ListarProdutosComEstoqueBaixo()
        {
            return _produtos.Where(p => p.QuantidadeEmEstoque <= p.EstoqueMinimo).ToList();
        }

        // Métodos para persistência de dados
        private void CarregarProdutos()
        {
            if (File.Exists(_arquivoProdutos))
            {
                string json = File.ReadAllText(_arquivoProdutos);
                _produtos = JsonSerializer.Deserialize<List<Produto>>(json);
            }
            else
            {
                _produtos = new List<Produto>();
            }
        }

        private void SalvarProdutos()
        {
            string json = JsonSerializer.Serialize(_produtos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_arquivoProdutos, json);
        }
    }

    // Classe que gerencia as vendas
    public class GerenciadorVendas
    {
        private List<Venda> _vendas;
        private string _arquivoVendas = "vendas.json";
        private int _proximoIdVenda = 1;
        private Estoque _estoque;

        public GerenciadorVendas(Estoque estoque)
        {
            _estoque = estoque;
            CarregarVendas();
        }

        public Venda IniciarVenda()
        {
            Venda venda = new Venda
            {
                Id = _proximoIdVenda++
            };
            return venda;
        }

        public void AdicionarItem(Venda venda, int codigoProduto, int quantidade)
        {
            Produto produto = _estoque.BuscarProdutoPorCodigo(codigoProduto);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            if (produto.QuantidadeEmEstoque < quantidade)
            {
                throw new Exception($"Quantidade insuficiente em estoque. Disponível: {produto.QuantidadeEmEstoque}");
            }

            ItemVenda item = new ItemVenda
            {
                CodigoProduto = produto.Codigo,
                NomeProduto = produto.Nome,
                Quantidade = quantidade,
                PrecoUnitario = produto.Preco
            };

            venda.Itens.Add(item);
        }

        public void FinalizarVenda(Venda venda)
        {
            if (venda.Itens.Count == 0)
            {
                throw new Exception("Não é possível finalizar uma venda sem itens.");
            }

            // Atualiza o estoque para cada item da venda
            foreach (var item in venda.Itens)
            {
                _estoque.AtualizarEstoque(item.CodigoProduto, item.Quantidade);
            }

            // Adiciona a venda à lista de vendas
            _vendas.Add(venda);
            SalvarVendas();
        }

        public List<Venda> ListarVendas()
        {
            return _vendas;
        }

        // Métodos para persistência de dados
        private void CarregarVendas()
        {
            if (File.Exists(_arquivoVendas))
            {
                string json = File.ReadAllText(_arquivoVendas);
                _vendas = JsonSerializer.Deserialize<List<Venda>>(json);

                // Encontra o próximo ID de venda
                if (_vendas.Count > 0)
                {
                    _proximoIdVenda = _vendas.Max(v => v.Id) + 1;
                }
            }
            else
            {
                _vendas = new List<Venda>();
            }
        }

        private void SalvarVendas()
        {
            string json = JsonSerializer.Serialize(_vendas, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_arquivoVendas, json);
        }
    }

    // Classe principal que controla a interface do usuário
    public class Sistema
    {
        private Estoque _estoque;
        private GerenciadorVendas _gerenciadorVendas;

        public Sistema()
        {
            _estoque = new Estoque();
            _gerenciadorVendas = new GerenciadorVendas(_estoque);
        }

        public void Executar()
        {
            bool continuar = true;

            while (continuar)
            {
                MostrarMenu();
                string opcao = Console.ReadLine();

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            CadastrarProduto();
                            break;
                        case "2":
                            ListarProdutos();
                            break;
                        case "3":
                            BuscarProduto();
                            break;
                        case "4":
                            AtualizarProduto();
                            break;
                        case "5":
                            ExcluirProduto();
                            break;
                        case "6":
                            RealizarVenda();
                            break;
                        case "7":
                            GerarRelatorios();
                            break;
                        case "8":
                            Console.WriteLine("Saindo do sistema...");
                            continuar = false;
                            break;
                        default:
                            Console.WriteLine("Opção inválida! Pressione qualquer tecla para continuar...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        private void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("=== SISTEMA DE GERENCIAMENTO DE MINI MERCADO ===");
            Console.WriteLine("1. Cadastrar Produto");
            Console.WriteLine("2. Listar Produtos");
            Console.WriteLine("3. Buscar Produto");
            Console.WriteLine("4. Atualizar Produto");
            Console.WriteLine("5. Excluir Produto");
            Console.WriteLine("6. Realizar Venda");
            Console.WriteLine("7. Gerar Relatórios");
            Console.WriteLine("8. Sair");
            Console.Write("Escolha uma opção: ");
        }

        private void CadastrarProduto()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRO DE PRODUTO ===");

            Console.Write("Código: ");
            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                throw new Exception("Código inválido!");
            }

            Console.Write("Nome: ");
            string nome = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new Exception("Nome não pode ser vazio!");
            }

            Console.Write("Descrição: ");
            string descricao = Console.ReadLine();

            Console.Write("Preço: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal preco) || preco <= 0)
            {
                throw new Exception("Preço inválido!");
            }

            Console.Write("Quantidade em Estoque: ");
            if (!int.TryParse(Console.ReadLine(), out int quantidadeEmEstoque) || quantidadeEmEstoque < 0)
            {
                throw new Exception("Quantidade em estoque inválida!");
            }

            Console.Write("Estoque Mínimo: ");
            if (!int.TryParse(Console.ReadLine(), out int estoqueMinimo) || estoqueMinimo < 0)
            {
                throw new Exception("Estoque mínimo inválido!");
            }

            Produto novoProduto = new Produto
            {
                Codigo = codigo,
                Nome = nome,
                Descricao = descricao,
                Preco = preco,
                QuantidadeEmEstoque = quantidadeEmEstoque,
                EstoqueMinimo = estoqueMinimo,
                DataCadastro = DateTime.Now
            };

            _estoque.AdicionarProduto(novoProduto);
            Console.WriteLine("Produto cadastrado com sucesso!");
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        private void ListarProdutos()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA DE PRODUTOS ===");

            var produtos = _estoque.ListarProdutos();
            if (produtos.Count == 0)
            {
                Console.WriteLine("Nenhum produto cadastrado.");
            }
            else
            {
                foreach (var produto in produtos)
                {
                    Console.WriteLine(produto);
                }
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        private void BuscarProduto()
        {
            Console.Clear();
            Console.WriteLine("=== BUSCAR PRODUTO ===");
            Console.WriteLine("1. Buscar por Código");
            Console.WriteLine("2. Buscar por Nome");
            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    Console.Write("Digite o código do produto: ");
                    if (!int.TryParse(Console.ReadLine(), out int codigo))
                    {
                        throw new Exception("Código inválido!");
                    }

                    var produto = _estoque.BuscarProdutoPorCodigo(codigo);
                    if (produto == null)
                    {
                        Console.WriteLine("Produto não encontrado.");
                    }
                    else
                    {
                        Console.WriteLine("\nProduto encontrado:");
                        Console.WriteLine(produto);
                    }
                    break;

                case "2":
                    Console.Write("Digite o nome do produto (ou parte dele): ");
                    string nome = Console.ReadLine();
                    var produtos = _estoque.BuscarProdutosPorNome(nome);

                    if (produtos.Count == 0)
                    {
                        Console.WriteLine("Nenhum produto encontrado.");
                    }
                    else
                    {
                        Console.WriteLine("\nProdutos encontrados:");
                        foreach (var p in produtos)
                        {
                            Console.WriteLine(p);
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        private void AtualizarProduto()
        {
            Console.Clear();
            Console.WriteLine("=== ATUALIZAR PRODUTO ===");

            Console.Write("Digite o código do produto que deseja atualizar: ");
            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                throw new Exception("Código inválido!");
            }

            var produto = _estoque.BuscarProdutoPorCodigo(codigo);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            Console.WriteLine($"Atualizando produto: {produto}");

            Console.Write($"Nome ({produto.Nome}): ");
            string nome = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nome))
            {
                produto.Nome = nome;
            }

            Console.Write($"Descrição ({produto.Descricao}): ");
            string descricao = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(descricao))
            {
                produto.Descricao = descricao;
            }

            Console.Write($"Preço (R${produto.Preco:F2}): ");
            string precoStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(precoStr))
            {
                if (decimal.TryParse(precoStr, out decimal preco) && preco > 0)
                {
                    produto.Preco = preco;
                }
                else
                {
                    throw new Exception("Preço inválido!");
                }
            }

            Console.Write($"Quantidade em Estoque ({produto.QuantidadeEmEstoque}): ");
            string quantidadeStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(quantidadeStr))
            {
                if (int.TryParse(quantidadeStr, out int quantidade) && quantidade >= 0)
                {
                    produto.QuantidadeEmEstoque = quantidade;
                }
                else
                {
                    throw new Exception("Quantidade em estoque inválida!");
                }
            }

            Console.Write($"Estoque Mínimo ({produto.EstoqueMinimo}): ");
            string estoqueMinStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(estoqueMinStr))
            {
                if (int.TryParse(estoqueMinStr, out int estoqueMin) && estoqueMin >= 0)
                {
                    produto.EstoqueMinimo = estoqueMin;
                }
                else
                {
                    throw new Exception("Estoque mínimo inválido!");
                }
            }

            _estoque.AtualizarProduto(produto);
            Console.WriteLine("Produto atualizado com sucesso!");
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        private void ExcluirProduto()
        {
            Console.Clear();
            Console.WriteLine("=== EXCLUIR PRODUTO ===");

            Console.Write("Digite o código do produto que deseja excluir: ");
            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                throw new Exception("Código inválido!");
            }

            var produto = _estoque.BuscarProdutoPorCodigo(codigo);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            Console.WriteLine($"Tem certeza que deseja excluir o produto: {produto}? (S/N)");
            string confirmacao = Console.ReadLine().ToUpper();

            if (confirmacao == "S")
            {
                _estoque.ExcluirProduto(codigo);
                Console.WriteLine("Produto excluído com sucesso!");
            }
            else
            {
                Console.WriteLine("Operação cancelada pelo usuário.");
            }

            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        private void RealizarVenda()
        {
            Console.Clear();
            Console.WriteLine("=== REALIZAR VENDA ===");

            Venda venda = _gerenciadorVendas.IniciarVenda();
            bool continuarVenda = true;

            while (continuarVenda)
            {
                Console.Clear();
                Console.WriteLine($"Venda #{venda.Id} - Itens adicionados: {venda.Itens.Count}");

                if (venda.Itens.Count > 0)
                {
                    Console.WriteLine("\nItens no carrinho:");
                    foreach (var item in venda.Itens)
                    {
                        Console.WriteLine(item);
                    }
                    Console.WriteLine($"\nValor Total: R${venda.ValorTotal:F2}");
                }

                Console.WriteLine("\n1. Adicionar Item");
                Console.WriteLine("2. Finalizar Venda");
                Console.WriteLine("3. Cancelar Venda");
                Console.Write("Escolha uma opção: ");

                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        AdicionarItemVenda(venda);
                        break;

                    case "2":
                        if (venda.Itens.Count == 0)
                        {
                            Console.WriteLine("Não é possível finalizar uma venda sem itens.");
                        }
                        else
                        {
                            _gerenciadorVendas.FinalizarVenda(venda);
                            Console.WriteLine("Venda finalizada com sucesso!");
                            continuarVenda = false;
                        }
                        break;

                    case "3":
                        Console.WriteLine("Venda cancelada pelo usuário.");
                        continuarVenda = false;
                        break;

                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }

                if (continuarVenda)
                {
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Pressione qualquer tecla para voltar ao menu principal...");
            Console.ReadKey();
        }

        private void AdicionarItemVenda(Venda venda)
        {
            Console.WriteLine("\n=== ADICIONAR ITEM ===");

            Console.Write("Digite o código do produto: ");
            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                throw new Exception("Código inválido!");
            }

            var produto = _estoque.BuscarProdutoPorCodigo(codigo);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }

            Console.WriteLine($"Produto: {produto.Nome} - Preço: R${produto.Preco:F2} - Estoque: {produto.QuantidadeEmEstoque}");

            Console.Write("Quantidade: ");
            if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
            {
                throw new Exception("Quantidade inválida!");
            }

            _gerenciadorVendas.AdicionarItem(venda, codigo, quantidade);
            Console.WriteLine("Item adicionado com sucesso!");
        }

        private void GerarRelatorios()
        {
            Console.Clear();
            Console.WriteLine("=== RELATÓRIOS ===");
            Console.WriteLine("1. Produtos com Estoque Baixo");
            Console.WriteLine("2. Vendas Realizadas");
            Console.WriteLine("3. Voltar");
            Console.Write("Escolha uma opção: ");

            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    RelatorioEstoqueBaixo();
                    break;

                case "2":
                    RelatorioVendas();
                    break;

                case "3":
                    return;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        private void RelatorioEstoqueBaixo()
        {
            Console.Clear();
            Console.WriteLine("=== RELATÓRIO DE PRODUTOS COM ESTOQUE BAIXO ===");

            var produtosEstoqueBaixo = _estoque.ListarProdutosComEstoqueBaixo();

            if (produtosEstoqueBaixo.Count == 0)
            {
                Console.WriteLine("Não há produtos com estoque baixo.");
            }
            else
            {
                Console.WriteLine("Produtos com estoque abaixo do mínimo:");
                foreach (var produto in produtosEstoqueBaixo)
                {
                    Console.WriteLine($"{produto.Nome} - Estoque Atual: {produto.QuantidadeEmEstoque} - Estoque Mínimo: {produto.EstoqueMinimo}");
                }
            }
        }

        private void RelatorioVendas()
        {
            Console.Clear();
            Console.WriteLine("=== RELATÓRIO DE VENDAS ===");

            var vendas = _gerenciadorVendas.ListarVendas();

            if (vendas.Count == 0)
            {
                Console.WriteLine("Não há vendas registradas.");
            }
            else
            {
                decimal valorTotalVendas = vendas.Sum(v => v.ValorTotal);
                int totalItensVendidos = vendas.Sum(v => v.Itens.Sum(i => i.Quantidade));

                Console.WriteLine($"Total de vendas: {vendas.Count}");
                Console.WriteLine($"Valor total: R${valorTotalVendas:F2}");
                Console.WriteLine($"Total de itens vendidos: {totalItensVendidos}");
                Console.WriteLine("\nÚltimas vendas:");

                foreach (var venda in vendas.OrderByDescending(v => v.DataVenda).Take(5))
                {
                    Console.WriteLine($"\n{venda}");
                    foreach (var item in venda.Itens)
                    {
                        Console.WriteLine($"  - {item}");
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Sistema sistema = new Sistema();
            sistema.Executar();
        }
    }
}