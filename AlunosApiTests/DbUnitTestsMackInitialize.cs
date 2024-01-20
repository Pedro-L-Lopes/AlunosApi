using AlunosApi.Context;
using AlunosApi.Models;

namespace ApiCatalogoXUnitTests
{
    public class DbUnitTestsMackInitialize
    {
        public DbUnitTestsMackInitialize() { }

        public void Seed(AppDbContext context)
        {
            context.Alunos.Add(
                new Aluno { Id = 15, Email = "alunoteste@teste.com", Nome = "Aluno teste", Idade = 20}
                );
        }
    }
}
