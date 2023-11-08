using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Produto>>> GetAsync()
        {
            var produtos = await _context.Produtos.AsNoTracking().Take(10).ToListAsync();
            if(produtos is null)
            {
                return NotFound("Produtos não encontrados.");
            }
            return produtos;
        }

        [HttpGet("/primeiro")]
        [HttpGet("primeiro")]
        [HttpGet("testeprimeiro")]
        public ActionResult<Produto> GetPrimeiro()
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault();
            if (produto is null)
            {
                return NotFound("Produto não encontrados.");
            }
            return produto;
        }

        [HttpGet("{id:int:min(1)}/{nome:maxlength(5)}", Name="ObterProduto")]
        public ActionResult<Produto> Get(int id, string nome)
        {
            try
            {
                var parametro = nome;

                var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
                if (produto is null)
                {
                    return NotFound($"Produto com id={id} não encontrado...");
                }
                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
        }

        [HttpGet("obterteste/{id}")]
        public ActionResult<Produto> GetTeste([FromQuery] int id, [BindRequired] string nome)
        {
            //throw new Exception("Exception ao retornar produto pelo id e nome");
            string[] teste = null;
            if(teste.Length > 0)
            {}

            try
            {
                var parametro = nome;

                var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
                if (produto is null)
                {
                    return NotFound($"Produto com id={id} não encontrado...");
                }
                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            /*
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            */

            if(produto is null)
            {
                return BadRequest("Dados invalidos");
            }

            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", 
                new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("id:int")]
        public ActionResult Put(int id, Produto produto)
        {
            if(id != produto.ProdutoId)
            {
                return BadRequest("Dados invalidos");
            }

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(produto);
        }

        [HttpDelete("id:int")]
        public ActionResult Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            // var produto = _context.Produtos.Find(id);

            if(produto is null)
            {
                return NotFound($"Produto com id={id} não encontrado...");
            }

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return Ok(produto);
        }

    }
}
