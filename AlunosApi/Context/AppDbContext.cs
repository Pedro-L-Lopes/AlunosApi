using AlunosApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames; 

namespace AlunosApi.Context
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // Mapeando tabelas no banco de dados
        public DbSet<Aluno> Alunos { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Aluno>().HasData(
        //        new Aluno
        //        {
        //            Id = 1,
        //            Nome = "Zezin do grau",
        //            Email = "zezin@email.com",
        //            Idade = 20
        //        },
        //         new Aluno
        //         {
        //        Id = 2,
        //            Nome = "Wilson",
        //            Email = "Wilson@email.com",
        //            Idade = 35
        //        }
        //    );
        //}
    }
}
