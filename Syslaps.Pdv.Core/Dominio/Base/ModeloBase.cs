using System.Collections.Generic;
using System.Text;
using Syslaps.Pdv.Cross;

namespace Syslaps.Pdv.Core.Dominio.Base
{
    public abstract class ModeloBase : Utils
    {
        private readonly List<ResultadoDoModelo> _listaDeResultadoDoModelo;

        protected ModeloBase()
        {
            _listaDeResultadoDoModelo = new List<ResultadoDoModelo>();
        }



        protected void AdicionarMensagem(string mensagem = "", EnumStatusDoResultado statusDoResultado = EnumStatusDoResultado.MensagemDeSucesso)
        {
            var resultado = new ResultadoDoModelo();
            resultado.StatusDoResultado = statusDoResultado;
            resultado.Messagem = mensagem;
            _listaDeResultadoDoModelo.Add(resultado);
        }

        public EnumStatusDoResultado Status
        {
            get
            {
                if (_listaDeResultadoDoModelo.Exists(x => x.StatusDoResultado == EnumStatusDoResultado.ErroGerenciado))
                    return EnumStatusDoResultado.ErroGerenciado;

                if (_listaDeResultadoDoModelo.Exists(x => x.StatusDoResultado == EnumStatusDoResultado.RegraDeNegocioInvalida))
                    return EnumStatusDoResultado.RegraDeNegocioInvalida;

                return EnumStatusDoResultado.MensagemDeSucesso;
            }
        }

        public string Mensagem
        {
            get
            {
                var stringBuilder = new StringBuilder();
                _listaDeResultadoDoModelo.ForEach(resultado => stringBuilder.AppendLine(resultado.Messagem));
                _listaDeResultadoDoModelo.Clear();
                return stringBuilder.ToString();
            }
        }
    }
}
