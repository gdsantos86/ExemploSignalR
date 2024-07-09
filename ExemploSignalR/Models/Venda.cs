namespace ExemploSignalR.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataVenda { get; set; } = DateTime.Now;

        public Produto? Produto { get; set; }
    }
}
