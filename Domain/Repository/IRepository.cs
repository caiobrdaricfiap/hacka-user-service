using Domain.Entity;

namespace Domain.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<IList<T>> GetAll();
        Task<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}