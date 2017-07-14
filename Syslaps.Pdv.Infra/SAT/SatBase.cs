using System;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Core.Dominio.SAT;
using Syslaps.Pdv.Entity.SAT;

namespace Syslaps.Pdv.Infra.SAT
{
    public abstract class SatBase : ISat
    {
        internal string ActivationCode { get; set; }

        public static SatBase Create(string activationCode, SatModelEnum model)
        {
            SatBase satBase;
            switch (model)
            {
                case SatModelEnum.Sweda:
                    satBase = (SatBase)new Sweda();
                    break;
                case SatModelEnum.Bematech:
                    satBase = (SatBase)new Bematech();
                    break;
                case SatModelEnum.Elgin:
                    satBase = (SatBase)new Elgin();
                    break;
                case SatModelEnum.Elgin2:
                    satBase = (SatBase)new Elgin2();
                    break;
                case SatModelEnum.Gertec:
                    satBase = (SatBase)new Gertec();
                    break;
                case SatModelEnum.Urano:
                    satBase = (SatBase)new Urano();
                    break;
                case SatModelEnum.Kryptus:
                    satBase = (SatBase)new Kryptus();
                    break;
                case SatModelEnum.Dimep:
                    satBase = (SatBase)new Dimep();
                    break;
                case SatModelEnum.Tanca:
                    satBase = (SatBase)new Tanca();
                    break;
                case SatModelEnum.OffLine:
                    satBase = (SatBase)new OffLine();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("model");
            }
            satBase.ActivationCode = activationCode;
            return satBase;
        }

        public abstract GetStatusResponse GetStatus();

        public abstract SendResponse Send(string xml);

        public abstract InquireResponse Inquire(int sessionCode);

        public abstract CancelResponse Cancel(string key, string xml);

        public abstract SatResponse CheckAvailability();

        public abstract SatResponse CheckCommunication(string xml);

        public abstract SatResponse ConfigureLan(string xmlConfiguration);

        public abstract SatResponse AssignSignature(string cnpj, string signature);

        public abstract SatResponse SoftwareUpdate();

        public abstract SatResponse ExtractLog();

        public abstract SatResponse Lock();

        public abstract SatResponse Unlock();

        public abstract SatResponse ChangeActivationCode(int option, string newActivationCode, string newActivationCodeConfirmation);

        internal int GenerateSessionCode()
        {
            return new Random().Next(999999);
        }
    }
}
