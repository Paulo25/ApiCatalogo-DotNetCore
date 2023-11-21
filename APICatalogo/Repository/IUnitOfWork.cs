namespace APICatalogo.Repository
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        
        //void Commit(); //implementação sincrona
        Task Commit(); //implementação assincrona
    }
}
