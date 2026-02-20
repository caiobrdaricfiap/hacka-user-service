using Domain.Entity;

namespace Domain.Repository
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario> GetUsuarioByEmailESenha(string email, string senha);
    }
}
