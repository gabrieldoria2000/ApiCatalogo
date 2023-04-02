using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext contexto) : base(contexto)
        {
        }

        public PagedList<Produto> GetProdutos(ProdutosParameters prdParameters)
        {
            //return Get()
            //    .OrderBy(o => o.Nome)
            //    .Skip((prdParameters.PageNumber - 1) * prdParameters.PageSize)
            //    .Take(prdParameters.PageSize)
            //    .ToList();

            return PagedList<Produto>.ToPagedList(Get().OrderBy(on => on.CategoriaId),
                prdParameters.PageNumber, prdParameters.PageSize);
        }

        public IEnumerable<Produto> GetProdutosporPreco()
        {
            return Get().OrderBy(c=> c.Preco).ToList();
        }
    }
}
