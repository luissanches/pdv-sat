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

        public async void Inserir<TEntity>(TEntity entity)
        {
            await Db.InsertAsync(entity);
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
