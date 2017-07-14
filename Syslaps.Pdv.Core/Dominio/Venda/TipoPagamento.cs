using System.Collections.Generic;
using System.Linq;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Core.Dominio.Venda
{
    public class TipoPagamento : ModeloBase
    {
        private readonly IRepositorioBase repositorio;

        private static List<Entity.TipoPagamento> listaDeTiposDePagamento;

        public List<Entity.TipoPagamento> ListaDeTiposDePagamento
        {
            get
            {
                if (listaDeTiposDePagamento == null)
                    listaDeTiposDePagamento = repositorio.RecuperarTodos<Entity.TipoPagamento>();

                return listaDeTiposDePagamento;
            }
        }

        public Entity.TipoPagamento Dinheiro
        {
            get { return ListaDeTiposDePagamento.SingleOrDefault(x => x.Nome == "Dinheiro"); }
        }

        public Entity.TipoPagamento DebitoRede
        {
            get { return ListaDeTiposDePagamento.SingleOrDefault(x => x.Nome == "Débito Rede"); }
        }

        public Entity.TipoPagamento DebitoModerninha
        {
            get { return ListaDeTiposDePagamento.SingleOrDefault(x => x.Nome == "Débito Moderninha"); }
        }

        public Entity.TipoPagamento CreditoRede
        {
            get { return ListaDeTiposDePagamento.SingleOrDefault(x => x.Nome == "Crédito Rede"); }
        }

        public Entity.TipoPagamento CreditoModerninha
        {
            get { return ListaDeTiposDePagamento.SingleOrDefault(x => x.Nome == "Crédito Moderninha"); }
        }

        public Entity.TipoPagamento Tiket
        {
            get { return ListaDeTiposDePagamento.SingleOrDefault(x => x.Nome == "Tiket"); }
        }

        public TipoPagamento(IRepositorioBase repositorio)
        {
            this.repositorio = repositorio;
        }
    }
}
