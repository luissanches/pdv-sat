using System;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Core.Dominio.Cliente
{
    public class ClienteCampanha : ModeloBase
    {
        private IClienteCampanhaRepositorio _repositorio;

        public ClienteCampanha(IClienteCampanhaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public void RegistrarClienteNaCampanha(Entity.ClienteCampanha clienteCampanha, EnumTipoCampanha tipoCampanha)
        {
            if (string.IsNullOrEmpty(clienteCampanha.CodigoClienteCampanha))
            {
                clienteCampanha.CodigoClienteCampanha = GerarCodigoUnico();
                clienteCampanha.NomeCampanha = tipoCampanha.ToString();
                clienteCampanha.DataCadastro = DateTime.Now;
                _repositorio.Inserir(clienteCampanha);
            }
            else
            {
                _repositorio.Atualizar(clienteCampanha);
            }
            
            AdicionarMensagem("Cliente registrado na campanha com sucesso.");
        }

        public Entity.ClienteCampanha RecuperarcClienteNaCampanha(Entity.ClienteCampanha clienteCampanha)
        {
            return _repositorio.RecuperarcClienteNaCampanha(clienteCampanha);
        }

    }
}