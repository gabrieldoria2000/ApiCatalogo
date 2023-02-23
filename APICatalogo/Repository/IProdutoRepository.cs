using APICatalogo.Models;

namespace APICatalogo.Repository
{
    public interface IProdutoRepository: IRepository<Produto>
    {

        //define um método específico
        IEnumerable<Produto> GetProdutosporPreco();
    }
}
