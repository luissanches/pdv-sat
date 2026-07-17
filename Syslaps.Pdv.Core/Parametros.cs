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
        private List<Parametro> _listaDeParametros;

        public Parametros(IRepositorioBase repositorio)
        {
            _repositorio = repositorio;
        }

        public List<Parametro> ListaDeParametros => _listaDeParametros ?? (_listaDeParametros = _repositorio.RecuperarTodos<Parametro>());

        public string TituloDasMensagens => ListaDeParametros.Find(x => x.Nome == "pdv.message.title")?.Valor ?? TituloNoConfig;
        public string SmtpSenderEmail => ListaDeParametros.Find(x => x.Nome == "smtp.sender.email")?.Valor ?? string.Empty;
        public string SmtpSenderName => ListaDeParametros.Find(x => x.Nome == "smtp.sender.name")?.Valor ?? string.Empty;
        public string NomeDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.nome"))?.Valor ?? string.Empty;
        public string NomeFantasiaDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.nomefantasia"))?.Valor ?? string.Empty;
        public string CnpjDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.cnpj"))?.Valor ?? string.Empty;
        public string IeDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.ie"))?.Valor ?? string.Empty;
        public string Endereco => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.endereco"))?.Valor ?? string.Empty;
        public string NumeroDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.numero"))?.Valor ?? string.Empty;
        public string BairroDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.bairro"))?.Valor ?? string.Empty;
        public string CidadeDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.cidade"))?.Valor ?? string.Empty;
        public string TelefoneDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.telefone"))?.Valor ?? string.Empty;
        public string EmailsParaEnviar => ListaDeParametros.Find(x => x.Nome == "receiver.email")?.Valor ?? string.Empty;
        public decimal CfopTributo => ListaDeParametros.Find(x => x.Nome == "cfop.tributos")?.Valor?.ToDecimal() ?? 0m;
        public string CodigoSat => ListaDeParametros.Find(x => x.Nome == "sat.codigo")?.Valor ?? string.Empty;
        public string SHCnpj => ListaDeParametros.Find(x => x.Nome == "sat.sh.cnpj")?.Valor ?? string.Empty;
        public string ModeloSat => ListaDeParametros.Find(x => x.Nome == "sat.modelo")?.Valor ?? "OffLine";
        public bool SatHabilitado => ListaDeParametros.Find(x => x.Nome == "sat.habilitado")?.Valor?.SimNaoToBool() ?? false;
        public string SignAC => ListaDeParametros.Find(x => x.Nome == "sat.signac")?.Valor ?? string.Empty;
        public string NumeroSat => ListaDeParametros.Find(x => x.Nome == "sat.numero")?.Valor ?? string.Empty;
        public string NumCaixa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".numcaixa"))?.Valor ?? string.Empty;
        public string ImDaEmpresa => ListaDeParametros.Find(x => x.Nome == string.Concat("NomeDoCaixa".GetConfigValue(), ".empresa.im"))?.Valor ?? string.Empty;
        public bool GavetaAutomatica => ListaDeParametros.Find(x => x.Nome == "pdv1.gaveta.automatica")?.Valor?.SimNaoToBool() ?? false;
        public string TituloNoConfig => ConfigurationManager.AppSettings["TituloInicial"];
    }
}
