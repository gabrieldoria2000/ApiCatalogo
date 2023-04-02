using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public interface IProdutoRepository: IRepository<Produto>
    {

        //define um método específico
        IEnumerable<Produto> GetProdutosporPreco();

        PagedList<Produto> GetProdutos(ProdutosParameters prdParameters);
    }
}
