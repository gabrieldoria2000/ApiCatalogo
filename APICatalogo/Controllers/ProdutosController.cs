using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _Context;

        private readonly IConfiguration _config;

        //Injetar a instancia do contexto no controlador
        public ProdutosController(AppDbContext context, IConfiguration config)
        {
            _Context = context;
            _config = config;
        }

        //pegar variaveis do arquivo de configuração
        [HttpGet("Autor")]
        public string GetAutor()
        {
            var autor = _config["Autor"];
            var conexao = _config["ConnectionStrings:DefaultConnection"];

            return $"Autor: {autor} Conexao: {conexao}";
        }

        //produtos
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilters))]
        public ActionResult<IEnumerable<Produto>> get()
        {
            //Abraça com o ActionResult para que o método possa retornar 2 tipos diferentes de resultados: 
            //um enumerable ou um NotFound.
            var prod = _Context.Produtos.AsNoTracking().ToList();

            if (prod is null)
            {
                return NotFound("Produtos não encontrados");
            }
            return prod;
        }

        //produtos/id - RESTRIÇÃO DE ROTA: valores inteiros e maiores que 1. É bom pra evitar que um roteamento seja feito
        //desnecessariamente. Dessa forma, uma consulta desnecessária é evitada no BD
        [HttpGet("{id:int:min(1)}", Name ="obterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var prod = _Context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            if (prod is null)
            {
                return NotFound("Produto não encontrado");
            }
            return prod;
        }

        //produtos
        [HttpPost]
        public ActionResult Post(Produto prod)
        {
            if (prod is null)
            {
                return BadRequest();
            }
            _Context.Produtos.Add(prod);
            _Context.SaveChanges();

            //esse método CreatedAtRouteResult já retorna o endpoint(URI) do produto criado
            //o nome da rota será obterProduto
            return new CreatedAtRouteResult("obterProduto", new { id = prod.ProdutoId }, prod);
        }

        [HttpPut("/produtos/{id}")]
        public ActionResult Put(int id, Produto prod)
        {
            if (id != prod.ProdutoId)
            {
                return BadRequest();
            }
            //Como estamos trabalhando em um cenário desconectado, o contexto precisa ser informado
            //que a entidade produto está num estado modificado, pra ele poder alterar...Pra isso usa
            //o método Entry

            _Context.Entry(prod).State = EntityState.Modified;

            //Dessa forma o Entity framework core vai saber que essa entidade precisa ser persistida

            _Context.SaveChanges();

            return Ok(prod);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            //var prod = _Context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            // a vantagem de usar o Find é que ele verifica primeiro na memória, ao inves de ir no BD,
            // mas pra ele funcionar, tem que passar o ID como parametro
        
            var prod = _Context.Produtos.Find(id);

            if (prod is null)
            {
                return NotFound("Produto não encontrado");
            }
            _Context.Produtos.Remove(prod);
            _Context.SaveChanges();

            return Ok(prod);
        }


    }
}
