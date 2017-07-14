using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syslaps.Pdv.Core.Dominio.Base
{
    public interface IRepositorioBase
    {
        void Inserir<TEntity>(TEntity entity);

        Task<bool> Atualizar<TEntity>(TEntity entity);

        Task<bool> Excluir<TEntity>(TEntity entity);

        TEntity Recuperar<TEntity>(TEntity entity);

        List<TEntity> RecuperarTodos<TEntity>();

        void LimparLogDaBase();
    }
}