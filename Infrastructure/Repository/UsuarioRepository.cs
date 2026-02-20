using Domain.Entity;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class UsuarioRepository : EFRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Usuario> GetUsuarioByEmailESenha(string email, string senha)
        {

            return await _dbSet.FirstOrDefaultAsync(entity => entity.Email == email && entity.Senha == senha);
        }
    }
}
