using AlunosApi.Models;
using AlunosApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlunosApi.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlunosController : ControllerBase 
    {
        private IAlunoService _alunoService;

        public AlunosController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        /// <summary>
        /// Obtém uma lista de todos os alunos.
        /// </summary>
        /// <returns>
        /// ActionResult contendo uma coleção de objetos do tipo Aluno, representando todos os alunos cadastrados.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Aluno>>> GetAlunos()
        {
            var alunos = await _alunoService.GetAlunos();
            return Ok(alunos);
        }

        /// <summary>
        /// Obtém uma lista de alunos cujos nomes correspondem parcial ou totalmente ao parâmetro fornecido.
        /// </summary>
        /// <param name="nome">O nome (total ou parcial) a ser utilizado na pesquisa de alunos.</param>
        /// <returns>
        /// Um ActionResult contendo uma coleção de objetos do tipo Aluno, representando os alunos encontrados.
        /// Retorna NotFound se nenhum aluno for encontrado com o nome fornecido.
        /// </returns>
        [HttpGet("AlunosPorNome")]
        public async Task<ActionResult<IAsyncEnumerable<Aluno>>> GetAlunosByName([FromQuery] string nome)
        {
            var alunos = await _alunoService.GetAlunosByNome(nome);

            if (alunos == null)
            {
                return NotFound($"Não foram encontrados resultados para {nome}");
            }

            return Ok(alunos);
        }

        /// <summary>
        /// Obtém um aluno pelo seu ID.
        /// </summary>
        /// <param name="id">O ID único do aluno a ser recuperado.</param>
        /// <returns>
        /// Um ActionResult contendo um objeto do tipo Aluno, representando o aluno encontrado.
        /// Retorna NotFound se nenhum aluno for encontrado com o ID fornecido.
        /// </returns>
        [HttpGet("{id:int}", Name = "GetAluno")]
        public async Task<ActionResult<Aluno>> GetAluno(int id)
        {
            var aluno = await _alunoService.GetAluno(id);

            if (aluno == null)
            {
                return NotFound($"Não existe aluno com o id {id}");
            }

            return Ok(aluno);
        }

        /// <summary>
        /// Cria um novo aluno.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST api/Aluno
        ///     {
        ///         "nome": "Novo Aluno",
        ///         "email": "novo@email.com,
        ///         "idade": 20,
        ///     }
        /// </remarks>
        /// <param name="aluno">Objeto Aluno contendo as informações do novo aluno a ser criado.</param>
        /// <returns>
        /// ActionResult indicando que o aluno foi criado com sucesso.
        /// Retorna um ActionResult contendo o objeto aluno criado, incluindo o ID atribuído.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Create(Aluno aluno)
        {
            await _alunoService.CreateAluno(aluno);

            return CreatedAtRoute(nameof(GetAluno), new { id = aluno.Id }, aluno);
        }

        /// <summary>
        /// Atualiza as informações de um aluno pelo seu ID.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT api/Aluno/123
        ///     {
        ///         "id": 123,
        ///         "nome": "Novo Nome",
        ///         "email": "novo@email.com,
        ///         "idade": 25,
        ///     }
        /// </remarks>
        /// <param name="id">O ID único do aluno a ser atualizado.</param>
        /// <param name="aluno">Objeto Aluno contendo as informações atualizadas.</param>
        /// <returns>
        /// ActionResult indicando que o aluno foi atualizado com sucesso.
        /// Retorna NotFound se o ID do aluno no corpo da solicitação não coincidir com o ID no parâmetro.
        /// </returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Edit(int id, [FromBody] Aluno aluno)
        {
            if (aluno.Id == id)
            {
                await _alunoService.UpdateAluno(aluno);
                return Ok($"Aluno com o id {id} foi atualizado com sucesso!");
            }
            else
            {
                return NotFound($"Aluno com id {id} não encontrado");
            }
        }

        /// <summary>
        /// Exclui um aluno pelo seu ID.
        /// </summary>
        /// <param name="id">O ID único do aluno a ser excluído.</param>
        /// <returns>
        /// Um ActionResult indicando que o aluno foi excluído com sucesso.
        /// Retorna NotFound se nenhum aluno for encontrado com o ID fornecido.
        /// </returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var aluno = await _alunoService.GetAluno(id);

            if (aluno != null)
            {
                await _alunoService.DeleteAluno(aluno);
                return Ok($"Aluno com id {id} excluído com sucesso!");
            }
            else
            {
                return NotFound($"Aluno com id {id} não encontrado!");
            }
        }

    }
}
