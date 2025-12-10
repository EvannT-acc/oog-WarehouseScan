using Microsoft.Practices.EnterpriseLibrary.Validation;
using NTwain;

namespace Oog.WarehouseScan.Command
{
    internal class FetchScannersCommand : Domain.Command.Command
    {
        protected log4net.ILog Log { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<DataSource> Scanners { get; private set; }
        static public NTwain.TwainSession TwainSession { get; private set; }

        protected override void OnExtendValidation(ValidationResults vr)
        {
        }

        protected override void ExecuteWhenValidated()
        {
            TwainSession = new NTwain.TwainSession(NTwain.Data.DataGroups.Image | NTwain.Data.DataGroups.Mask | NTwain.Data.DataGroups.Control);
            if (!TwainSession.IsDsmOpen)
            {
                if (TwainSession.Open() == NTwain.Data.ReturnCode.Success)
                {
                    Scanners = TwainSession.GetSources().ToList();
                }
                else
                {
                    throw new InvalidOperationException(Translation.Scanner.TwainException);
                }
            }
        }
    }
}