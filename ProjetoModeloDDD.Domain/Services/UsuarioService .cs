using System.Collections.Generic;
using System.Linq;
using ProjetoModeloDDD.Domain.Entities;
using ProjetoModeloDDD.Domain.Interfaces.Repositories;
using ProjetoModeloDDD.Domain.Interfaces.Services;

namespace ProjetoModeloDDD.Domain.Services
{
    public class UsuarioService : ServiceBase<Usuario>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository UsuarioRepository)
            : base(UsuarioRepository)
        {
            _usuarioRepository = UsuarioRepository;
        }

        public static IRepositoryBase<Usuario> UsuarioRepository { get; }        

        public IEnumerable<Usuario> ValidarEmail(IEnumerable<Usuario> usuarios)
        {
            return usuarios.Where (c => c.Email.Equals(c));
        }
    }
}
