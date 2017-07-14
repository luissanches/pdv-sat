using log4net;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Infra
{
    public class Logger : IInfraLogger
    {
        public ILog Log()
        {
            return LogManager.GetLogger("LogInFile");
        }
    }
}
