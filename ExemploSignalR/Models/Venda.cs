using System.ComponentModel.DataAnnotations;

namespace ExemploSignalR.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        public int Quantidade { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DataVenda { get; set; } = DateTime.Now;


    }
}
