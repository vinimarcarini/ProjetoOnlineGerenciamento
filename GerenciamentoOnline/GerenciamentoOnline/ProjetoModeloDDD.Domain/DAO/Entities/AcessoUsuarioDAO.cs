using Castle.ActiveRecord;
using ProjetoModeloDDD.Domain.Entities;

namespace ProjetoModeloDDD.Domain.DAO.Entities
{
    public class AcessoUsuarioDAO
    {
        public static AcessoUsuario FindByPrimaryKey(int idAcessoUsuario)
        {
            return ActiveRecordMediator<AcessoUsuario>.FindByPrimaryKey(idAcessoUsuario, false);
        }
    }
}