using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;
    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> get()
    {
        //Abraça com o ActionResult para que o método possa retornar 2 tipos diferentes de resultados: 
        //um enumerable ou um NotFound.

        //AsNotracking apenas para consultas - para melhorar o desempenho
        //só pode fazer isso se tiver certeza que o retorno dessa consulta, os objetos não vão precisar
        //serem alterados.
        var cat = _context.Categorias.AsNoTracking().ToList();

        if (cat is null)
        {
            return NotFound("Categoria não encontrada");
        }
        return cat;
    }

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> getCategoriasProduto()
    {
        //o método de extenção include permite carregar entidades relacionadas :)
        //var cat = _context.Categorias.Include(p=> p.Produtos).ToList();

        var cat = _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <=5).ToList();

        if (cat is null)
        {
            return NotFound("Categoria não encontrada");
        }
        return cat;
    }

    [HttpGet("{id:int}", Name = "obterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
        var cat = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

        if (cat is null)
        {
            return NotFound("Categoria não encontrada");
        }
        return Ok(cat);
    }

    [HttpPost]
    public ActionResult Post(Categoria cat)
    {
        if (cat is null)
        {
            return BadRequest();
        }
        _context.Categorias.Add(cat);
        _context.SaveChanges();

        //esse método CreatedAtRouteResult já retorna o endpoint(URI) do produto criado
        //o nome da rota será obterProduto
        return new CreatedAtRouteResult("obterCategoria", new { id = cat.CategoriaId }, cat);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria cat)
    {
        if (id != cat.CategoriaId)
        {
            return BadRequest();
        }
        //Como estamos trabalhando em um cenário desconectado, o contexto precisa ser informado
        //que a entidade produto está num estado modificado, pra ele poder alterar...Pra isso usa
        //o método Entry

        _context.Entry(cat).State = EntityState.Modified;

        //Dessa forma o Entity framework core vai saber que essa entidade precisa ser persistida

        _context.SaveChanges();

        return Ok(cat);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        //var prod = _Context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

        // a vantagem de usar o Find é que ele verifica primeiro na memória, ao inves de ir no BD,
        // mas pra ele funcionar, tem que passar o ID como parametro

        var cat = _context.Categorias.Find(id);

        if (cat is null)
        {
            return NotFound("Categoria não encontrada");
        }
        _context.Categorias.Remove(cat);
        _context.SaveChanges();

        return Ok(cat);
    }

}
