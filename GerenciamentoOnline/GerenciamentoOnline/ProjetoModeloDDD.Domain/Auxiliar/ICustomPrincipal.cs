using System.Security.Principal;

namespace ProjetoModeloDDD.Domain.Auxiliar
{
    public interface ICustomPrincipal : IPrincipal
    {
        int IdUsuario { get; set; }
        int IdEstab { get; set; }
        int IdEmpresa { get; set; }
        string UserName { get; set; }
    }
}