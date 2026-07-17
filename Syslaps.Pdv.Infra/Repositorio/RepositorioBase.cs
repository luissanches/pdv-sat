using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.FastCrud;
using StructureMap.TypeRules;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioBase : IRepositorioBase, IDisposable
    {
        bool disposed = false;
        private static SQLiteConnection sqLiteConnection;

        protected internal SQLiteConnection Db
        {
            get
            {
                if (sqLiteConnection == null)
                    sqLiteConnection = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Repositorio"].ConnectionString);

                if (sqLiteConnection.State != ConnectionState.Open)
                    sqLiteConnection.Open();


                return sqLiteConnection;
            }
        }

        public void Inserir<TEntity>(TEntity entity)
        {
            Db.Insert(entity);
        }

        public async Task<bool> Atualizar<TEntity>(TEntity entity)
        {
            return await Db.UpdateAsync(entity);
        }

        public async Task<bool> Excluir<TEntity>(TEntity entity)
        {
            return await Db.DeleteAsync(entity);
        }

        public TEntity Recuperar<TEntity>(TEntity entity)
        {
            return Db.Get<TEntity>(entity);
        }

        public List<TEntity> RecuperarTodos<TEntity>()
        {
            return Db.Query<TEntity>($"select * from {typeof(TEntity).GetName()}").ToList();
        }

        public bool TabelasExistem()
        {
            var count = Db.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='TipoPagamento'");
            return count > 0;
        }

        public void LimparLogDaBase()
        {
            Db.Execute(@"vacuum;");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                sqLiteConnection.Dispose();
                sqLiteConnection = null;
            }

            disposed = true;
        }
    }
}
