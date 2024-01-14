using AlunosApi.Models;
using AlunosApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlunosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosController : ControllerBase
    {
        private IAlunoService _alunoService;

        public AlunosController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Aluno>>> GetAlunos()
        {
            var alunos = await _alunoService.GetAlunos();
            return Ok(alunos);
        }

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

        [HttpPost]
        public async Task<ActionResult> Create(Aluno aluno)
        {
            await _alunoService.CreateAluno(aluno);

            return CreatedAtRoute(nameof(GetAluno), new { id = aluno.Id }, aluno);
        }

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
