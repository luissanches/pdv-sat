using System;
using System.Runtime.InteropServices;
using Syslaps.Pdv.Entity.SAT;

namespace Syslaps.Pdv.Infra.SAT
{
  public class Bematech : SatBase
  {
    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr ConsultarStatusOperacional(int sessao, string codigoDeAtivacao);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr EnviarDadosVenda(int sessao, string cod, string dados);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr CancelarUltimaVenda(int sessao, string cod, string chave, string dadoscancel);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr TesteFimAFim(int sessao, string cod, string dados);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr ConsultarSAT(int sessao);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr ConsultarNumeroSessao(int sessao, string cod, int sessao_a_ser_consultada);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr AtivarSAT(int sessao, int tipoCert, string cod_Ativacao, string cnpj, int uf);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr ComunicarCertificadoICPBRASIL(int sessao, string cod, string csr);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr ConfigurarInterfaceDeRede(int sessao, string cod, string xmlConfig);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr AssociarAssinatura(int sessao, string cod, string cnpj, string sign_cnpj);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr DesbloquearSAT(int sessao, string cod_ativacao);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr BloquearSAT(int sessao, string cod_ativacao);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr TrocarCodigoDeAtivacao(int sessao, string cod_ativacao, int opcao, string nova_senha, string conf_senha);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr ExtrairLogs(int sessao, string cod_ativacao);

    [DllImport("Bematech_SAT.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr AtualizarSoftwareSAT(int sessao, string cod_ativacao);

    public override GetStatusResponse GetStatus()
    {
      return new GetStatusResponse().Create(Marshal.PtrToStringAnsi(Bematech.ConsultarStatusOperacional(this.GenerateSessionCode(), this.ActivationCode)));
    }

    public override SendResponse Send(string xml)
    {
      return new SendResponse().Create(Marshal.PtrToStringAnsi(Bematech.EnviarDadosVenda(this.GenerateSessionCode(), this.ActivationCode, xml)));
    }

    public override InquireResponse Inquire(int sessionCode)
    {
      return new InquireResponse().Create(Marshal.PtrToStringAnsi(Bematech.ConsultarNumeroSessao(this.GenerateSessionCode(), this.ActivationCode, sessionCode)));
    }

    public override CancelResponse Cancel(string key, string xml)
    {
      return new CancelResponse().Create(Marshal.PtrToStringAnsi(Bematech.CancelarUltimaVenda(this.GenerateSessionCode(), this.ActivationCode, key, xml)));
    }

    public override SatResponse CheckAvailability()
    {
      return new SatResponse().Create(Marshal.PtrToStringAnsi(Bematech.ConsultarSAT(this.GenerateSessionCode())));
    }

    public override SatResponse CheckCommunication(string xml)
    {
      return (SatResponse) new CheckCommunicationResponse().Create(Marshal.PtrToStringAnsi(Bematech.TesteFimAFim(this.GenerateSessionCode(), this.ActivationCode, xml)));
    }

    public override SatResponse ConfigureLan(string xmlConfiguration)
    {
      return new SatResponse().Create(Marshal.PtrToStringAnsi(Bematech.ConfigurarInterfaceDeRede(this.GenerateSessionCode(), this.ActivationCode, xmlConfiguration)));
    }

    public override SatResponse AssignSignature(string cnpj, string signature)
    {
      return new SatResponse().Create(Marshal.PtrToStringAnsi(Bematech.AssociarAssinatura(this.GenerateSessionCode(), this.ActivationCode, cnpj, signature)));
    }

    public override SatResponse SoftwareUpdate()
    {
      return new SatResponse().Create(Marshal.PtrToStringAnsi(Bematech.AtualizarSoftwareSAT(this.GenerateSessionCode(), this.ActivationCode)));
    }

    public override SatResponse ExtractLog()
    {
      return (SatResponse) new ExtractLogResponse().Create(Marshal.PtrToStringAnsi(Bematech.ExtrairLogs(this.GenerateSessionCode(), this.ActivationCode)));
    }

    public override SatResponse Lock()
    {
      return new SatResponse().Create(Marshal.PtrToStringAnsi(Bematech.BloquearSAT(this.GenerateSessionCode(), this.ActivationCode)));
    }

    public override SatResponse Unlock()
    {
      return new SatResponse().Create(Marshal.PtrToStringAnsi(Bematech.DesbloquearSAT(this.GenerateSessionCode(), this.ActivationCode)));
    }

    public override SatResponse ChangeActivationCode(int option, string newActivationCode, string newActivationCodeConfirmation)
    {
      return new SatResponse().Create(Marshal.PtrToStringAnsi(Bematech.TrocarCodigoDeAtivacao(this.GenerateSessionCode(), this.ActivationCode, option, newActivationCode, newActivationCodeConfirmation)));
    }
  }
}
