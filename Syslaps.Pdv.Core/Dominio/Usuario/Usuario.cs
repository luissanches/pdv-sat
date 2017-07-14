using System.Linq;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Cross;

namespace Syslaps.Pdv.Core.Dominio.Usuario
{
    public class Usuario : ModeloBase
    {
        private readonly IUsuarioRepositorio repositorio;
        public Entity.Usuario UsuarioLogado { get; private set; }
        
        public Usuario(IUsuarioRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public void LogarUsuario(string nome, string senha)
        {
            UsuarioLogado = repositorio.RecuperarUsuario(nome, senha);
            if (UsuarioLogado == null)
            {
                AdicionarMensagem("Usuário ou senha inválidos.", EnumStatusDoResultado.RegraDeNegocioInvalida);
            }
        }

        public void RegistrarNovoUsuario(string nome, string senha, EnumTipoUsuario tipoUsuario)
        {
            UsuarioLogado = new Entity.Usuario();
            UsuarioLogado.CodigoUsuario = GerarCodigoUnico();
            UsuarioLogado.Nome = nome;
            UsuarioLogado.Senha = senha;
            UsuarioLogado.Tipo = tipoUsuario.ToString();
            ValidarEPersistir();
        }

        private void ValidarEPersistir()
        {
            UsuarioLogado.TryValidateAnnotation().ToList().ForEach(item =>  AdicionarMensagem(item.ErrorMessage, EnumStatusDoResultado.RegraDeNegocioInvalida));
            if (Status != EnumStatusDoResultado.MensagemDeSucesso) return;
            repositorio.Inserir(UsuarioLogado);
            AdicionarMensagem("Usuário registrado com sucesso.");
        }
    }
}
