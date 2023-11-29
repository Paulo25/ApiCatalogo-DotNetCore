using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;


namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork context, IMapper mapper)
        {
            _uof = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            //var produtos = await _context.Produtos.AsNoTracking().Take(10).ToListAsync();
            var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);

            if(produtos is null)
            {
                return NotFound("Produtos não encontrados.");
            }

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }


        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorPrecos()
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco();
            var produtosDto =  _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }


        [HttpGet("/primeiro")]
        [HttpGet("primeiro")]
        [HttpGet("testeprimeiro")]
        public async Task<ActionResult<Produto>> GetPrimeiro()
        {
            //var produto = _context.Produtos.AsNoTracking().FirstOrDefault();

            var produto = await _uof.ProdutoRepository.Get().FirstOrDefaultAsync();

            if (produto is null)
            {
                return NotFound("Produto não encontrados.");
            }
            return produto;
        }

        
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            try
            {
                //var parametro = nome;0

                //var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);

                var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

                if (produto is null)
                {
                    return NotFound($"Produto com id={id} não encontrado...");
                }

                var produtoDto = _mapper.Map<ProdutoDTO>(produto);

                return produtoDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
        }

        [HttpGet("obterteste/{id}")]
        public async Task<ActionResult<Produto>> GetTeste([FromQuery] int id, [BindRequired] string nome)
        {
            //throw new Exception("Exception ao retornar produto pelo id e nome");
            string[] teste = null;
            if(teste.Length > 0)
            {}

            try
            {
                var parametro = nome;

                //var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);

                var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

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
        public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto)
        {
            /*
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            */
            var produto = _mapper.Map<Produto>(produtoDto);

            if(produto is null)
            {
                return BadRequest("Dados invalidos");
            }

            //_context.Produtos.Add(produto);
            //_context.SaveChanges();
            _uof.ProdutoRepository.Add(produto);
            await _uof.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto", 
                new { id = produto.ProdutoId }, produtoDTO);
        }

        [HttpPut("id:int")]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDto)
        {
            if(id != produtoDto.ProdutoId)
            {
                return BadRequest("Dados invalidos");
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            //_context.Entry(produto).State = EntityState.Modified;
            //_context.SaveChanges();
            _uof.ProdutoRepository.Update(produto);
            await _uof.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }

        [HttpDelete("id:int")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            //var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            // var produto = _context.Produtos.Find(id);

            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound($"Produto com id={id} não encontrado...");
            }

            //_context.Produtos.Remove(produto);
            //_context.SaveChanges();
            _uof.ProdutoRepository.Delete(produto);
             await _uof.Commit();

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }

    }
}
