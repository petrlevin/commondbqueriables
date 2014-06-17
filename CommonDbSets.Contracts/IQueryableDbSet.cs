using System.Linq;

namespace CommonDbSets.Contracts
{
    public interface IQueryableDbSet<T>:IQueryable<T>
    {
        T Add(T entity);
        T Attach(T entity);
        T Create();
        T Remove(T entity);
       
    }
}
