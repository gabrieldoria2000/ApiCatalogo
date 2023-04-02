namespace APICatalogo.Pagination;

public class ProdutosParameters
{

    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    private int _PageSize = 10;

    public int PageSize
    {
        get
        {
            return _PageSize;
        }
        set
        {
            //se o valor for maior que 50, ele atribui o valor de maxPageSize, SENÃO ele recebe o valor
            _PageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
