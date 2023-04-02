using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
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

        private readonly IMapper _mapper;

        //Injetar a instancia do contexto no controlador
        public ProdutosController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _Context = context;
            _config = config;
            _mapper = mapper;
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
        public ActionResult<IEnumerable<ProdutoDTO>> get([FromQuery] ProdutosParameters prdPatameters)
        {
            //Abraça com o ActionResult para que o método possa retornar 2 tipos diferentes de resultados: 
            //um enumerable ou um NotFound.
            var prod = _Context.Produtos.AsNoTracking().ToList();

            //var prod = ProdutoRepository.GetProdutos(prdPatameters).ToList();

            //var metadata = new
            //{
            //    prod.totalCount.
            //    prod.pagesize
            //    etc
            //}

            //Response.Headers.Add("X-Paginacao", jsonConvert.SerializableObject(metadata));

            var prodDto = _mapper.Map<List<ProdutoDTO>>(prod);

            if (prod is null)
            {
                return NotFound("Produtos não encontrados");
            }
            //o usuario vai ver as informações de produtoDTO
            return prodDto;
        }

        //produtos/id - RESTRIÇÃO DE ROTA: valores inteiros e maiores que 1. É bom pra evitar que um roteamento seja feito
        //desnecessariamente. Dessa forma, uma consulta desnecessária é evitada no BD
        [HttpGet("{id:int:min(1)}", Name ="obterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {
            var prod = _Context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            var prodDto = _mapper.Map < ProdutoDTO > (prod);

            if (prod is null)
            {
                return NotFound("Produto não encontrado");
            }
            return prodDto;
        }

        //produtos
        [HttpPost]
        public ActionResult Post(ProdutoDTO prodDTO)
        {
            var produto = _mapper.Map<Produto>(prodDTO);

            //atenção...quando for "falar" com o BD, tem que ser Produto mesmo. O BD não entende o DTO

            if (produto is null)
            {
                return BadRequest();
            }
            _Context.Produtos.Add(produto);
            _Context.SaveChanges();

            //esse método CreatedAtRouteResult já retorna o endpoint(URI) do produto criado
            //o nome da rota será obterProduto

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            //vai exibir para o usuario NÃO Produto, mas ProdutoDTO
            return new CreatedAtRouteResult("obterProduto", new { id = produto.ProdutoId }, produtoDTO);
        }

        [HttpPut("/produtos/{id}")]
        public ActionResult Put(int id, ProdutoDTO prodDTO)
        {

            if (id != prodDTO.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(prodDTO);

            //Como estamos trabalhando em um cenário desconectado, o contexto precisa ser informado
            //que a entidade produto está num estado modificado, pra ele poder alterar...Pra isso usa
            //o método Entry

            _Context.Entry(produto).State = EntityState.Modified;

            //Dessa forma o Entity framework core vai saber que essa entidade precisa ser persistida

            _Context.SaveChanges();

            return Ok(prodDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
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

            var produtoDTO = _mapper.Map<Produto>(prod);

            return Ok(produtoDTO);
        }


    }
}
