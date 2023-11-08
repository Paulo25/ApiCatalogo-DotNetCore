﻿using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriasController(AppDbContext context, IConfiguration config, ILogger<ProdutosController> logger)
        {
            _context = context;
            _configuration = config;
            _logger = logger;
        }


        [HttpGet("autor")]
        public String GetAutor()
        {
            var autor = _configuration["autor"];
            var stringConexao = _configuration["ConnectionStrings:DefaultConnection"];

            return $"Autor: {autor} conexão: {stringConexao}";
        }

        [HttpGet("saudacao/{nome}")]
        public ActionResult<String> GetSaudacao([FromServices] IMeuServico meuservico, string nome)
        {
            return meuservico.Saudacao(nome);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetAsync()
        {
            try
            {
                var categorias = await _context.Categorias.AsNoTracking().Take(10).ToListAsync();
                if (categorias is null)
                {
                    return NotFound("Categorias não encontradas.");
                }
                return categorias;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
        }


        [HttpGet("id:int", Name ="ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            _logger.LogInformation($"============GET api/catgegorias/id {id} =============");
            var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);

            if(categoria is null)
            {
                _logger.LogInformation($"============GET api/catgegorias/id {id} NOT FOUND=============");
                return NotFound($"Categoria com id={id} não encontrada...");
            }

            return categoria;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).AsNoTracking().Take(10).ToList();
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if(categoria is null)
            {
                return BadRequest("Dados invalidos");
            }
            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId}, categoria);
        }

        [HttpPut("id:int")]
        public ActionResult<Categoria> Put(int id, Categoria categoria)
        {
            if(id != categoria.CategoriaId)
            {
                return BadRequest("Dados invalidos");
            }

            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(categoria);
        }

        [HttpDelete("id:int")]
        public ActionResult Delete(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId.Equals(id));

            if(categoria is null)
            {
                return NotFound($"Categoria com id={id} não encontrada...");
            }

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();

            return Ok(categoria);
        }
    }
}
