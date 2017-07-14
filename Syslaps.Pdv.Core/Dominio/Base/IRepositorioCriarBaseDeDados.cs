namespace Syslaps.Pdv.Core.Dominio.Base
{
    public interface IRepositorioCriarBaseDeDados
    {
        void CriarDataBase();

        void ExcluirTabelas();

        void Vacuum();
    }
}