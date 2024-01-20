using AlunosApi.Controllers;
using AlunosApi.Models;
using AlunosApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AlunosApiTests
{
    public class AlunosTestController
    {
        [Fact]
        public async Task GetAlunos_ReturnsOkResult()
        {
            // Arrange
            var alunoServiceMock = new Mock<IAlunoService>();
            alunoServiceMock.Setup(service => service.GetAlunos()).ReturnsAsync(new List<Aluno>());

            var controller = new AlunosController(alunoServiceMock.Object);

            // Act
            var result = await controller.GetAlunos();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAlunosByName_ReturnsNotFoundResult_WhenNoResults()
        {
            // Arrange
            var alunoServiceMock = new Mock<IAlunoService>();
            alunoServiceMock.Setup(service => service.GetAlunosByNome(It.IsAny<string>())).ReturnsAsync((List<Aluno>)null);

            var controller = new AlunosController(alunoServiceMock.Object);

            // Act
            var result = await controller.GetAlunosByName("NonExistentName");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAluno_ReturnsNotFoundResult_WhenNoResult()
        {
            // Arrange
            var alunoServiceMock = new Mock<IAlunoService>();
            alunoServiceMock.Setup(service => service.GetAluno(It.IsAny<int>())).ReturnsAsync((Aluno)null);

            var controller = new AlunosController(alunoServiceMock.Object);

            // Act
            var result = await controller.GetAluno(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }


        [Fact]
        public async Task Create_ReturnsCreatedAtRouteResult()
        {
            // Arrange
            var alunoServiceMock = new Mock<IAlunoService>();
            var controller = new AlunosController(alunoServiceMock.Object);

            var novoAluno = new Aluno
            {
                Nome = "Novo Aluno",
                Email = "novo@email.com",
                Idade = 20
            };

            // Act
            var result = await controller.Create(novoAluno);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsOkResult_WhenIdMatches()
        {
            // Arrange
            var alunoServiceMock = new Mock<IAlunoService>();
            var controller = new AlunosController(alunoServiceMock.Object);

            var alunoExistente = new Aluno
            {
                Id = 1,
                Nome = "Aluno Existente",
                Email = "existente@email.com",
                Idade = 25
            };

            alunoServiceMock.Setup(service => service.UpdateAluno(It.IsAny<Aluno>()))
                .Callback<Aluno>(aluno => alunoExistente = aluno);

            // Act
            var result = await controller.Edit(1, alunoExistente);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Aluno com o id 1 foi atualizado com sucesso!", (result as OkObjectResult)?.Value);
        }

        [Fact]
        public async Task Edit_ReturnsNotFoundResult_WhenIdDoesNotMatch()
        {
            // Arrange
            var alunoServiceMock = new Mock<IAlunoService>();
            var controller = new AlunosController(alunoServiceMock.Object);

            var alunoExistente = new Aluno
            {
                Id = 1,
                Nome = "Aluno Existente",
                Email = "existente@email.com",
                Idade = 25
            };

            // Act
            var result = await controller.Edit(2, alunoExistente);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Aluno com id 2 não encontrado", (result as NotFoundObjectResult)?.Value);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenIdMatches()
        {
            // Arrange
            var alunoServiceMock = new Mock<IAlunoService>();
            var controller = new AlunosController(alunoServiceMock.Object);

            var alunoExistente = new Aluno
            {
                Id = 1,
                Nome = "Aluno Existente",
                Email = "existente@email.com",
                Idade = 25
            };

            alunoServiceMock.Setup(service => service.GetAluno(It.IsAny<int>())).ReturnsAsync(alunoExistente);

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Aluno com id 1 excluído com sucesso!", (result as OkObjectResult)?.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenIdDoesNotMatch()
        {
            // Arrange
            var alunoServiceMock = new Mock<IAlunoService>();
            var controller = new AlunosController(alunoServiceMock.Object);

            alunoServiceMock.Setup(service => service.GetAluno(It.IsAny<int>()))
               .ReturnsAsync((Aluno)null);

            // Act
            var result = await controller.Delete(2);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Aluno com id 2 não encontrado!", (result as NotFoundObjectResult)?.Value);
        }
    }
}
