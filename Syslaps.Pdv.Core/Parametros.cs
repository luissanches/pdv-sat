using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;
using System.Configuration;

namespace Syslaps.Pdv.Core
{
    public class Parametros
    {
        private readonly IRepositorioBase _repositorio;
        private static List<Parametro> _listaDeParametros;

        public Parametros(IRepositorioBase repositorio)
        {
            _repositorio = repositorio;
        }

        public List<Parametro> ListaDeParametros => _listaDeParametros ?? (_listaDeParametros = _repositorio.RecuperarTodos<Parametro>());

        public string TituloDasMensagens => ListaDeParametros.Find(x => x.Nome == "pdv.message.title").Valor;
        public string SmtpSenderEmail => ListaDeParametros.Find(x => x.Nome == "smtp.sender.email").Valor;
        public string SmtpSenderName => ListaDeParametros.Find(x => x.Nome == "smtp.sender.name").Valor;
        public string NomeDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.nome")).Valor;
        public string NomeFantasiaDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.nomefantasia")).Valor;
        public string CnpjDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.cnpj")).Valor;
        public string IeDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.ie")).Valor;
        public string Endereco => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.endereco")).Valor;
        public string NumeroDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.numero")).Valor;
        public string BairroDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.bairro")).Valor;
        public string CidadeDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.cidade")).Valor;
        public string TelefoneDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.telefone")).Valor;
        public string EmailsParaEnviar => ListaDeParametros.Find(x => x.Nome == "receiver.email").Valor;
        public decimal CfopTributo => ListaDeParametros.Find(x => x.Nome == "cfop.tributos").Valor.ToDecimal();
        public string CodigoSat => ListaDeParametros.Find(x => x.Nome == "sat.codigo").Valor;
        public string SHCnpj => ListaDeParametros.Find(x => x.Nome == "sat.sh.cnpj").Valor;
        public string ModeloSat => ListaDeParametros.Find(x => x.Nome == "sat.modelo").Valor;
        public bool SatHabilitado => ListaDeParametros.Find(x => x.Nome == "sat.habilitado").Valor.SimNaoToBool();
        public string SignAC => ListaDeParametros.Find(x => x.Nome == "sat.signac").Valor;
        public string NumeroSat => ListaDeParametros.Find(x => x.Nome == "sat.numero").Valor;
        public string NumCaixa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".numcaixa")).Valor;
        public string ImDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.im")).Valor;
        public bool GavetaAutomatica => ListaDeParametros.Find(x => x.Nome == "pdv1.gaveta.automatica").Valor.SimNaoToBool();
        public string TituloNoConfig => ConfigurationManager.AppSettings["TituloInicial"];
    }
}