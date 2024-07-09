using ExemploSignalR.Models;
using Microsoft.EntityFrameworkCore;

namespace ExemploSignalR.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
    }
}
