namespace ProdutoAPI.Models
{
    public class Produto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
    }
}
