using ProdutoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProdutoAPI.Data
{
    public class ProdutoContextDb: DbContext
    {
        public ProdutoContextDb(DbContextOptions<ProdutoContextDb> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Codigo)
                .IsRequired()
                .HasColumnType("char(10)");

            modelBuilder.Entity<Produto>()
                .Property(p => p.Nome)
                .IsRequired()
                .HasColumnType("varchar(max)");

            modelBuilder.Entity<Produto>()
                .ToTable("Produtos");

            base.OnModelCreating(modelBuilder);
        }
    }
}
