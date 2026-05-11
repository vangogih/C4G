using System.Diagnostics.CodeAnalysis;
using System.Text;
using C4G.Core.Utils;

namespace C4G.Core.Errors
{
    [ExcludeFromCodeCoverage]
    public abstract class C4GErrorBase
    {
        public string Message { get; }
        public C4GErrorBase Cause { get; }
        public string ErrorTrace { get; }

        protected C4GErrorBase(string message, C4GErrorBase cause = null)
        {
            Message = message;
            Cause = cause;
            ErrorTrace = StackTraceUtility.GetAssetPathsOnly(skipFrames: 2);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{GetType().Name}");
            sb.AppendLine($"{Message}");
            sb.AppendLine(ErrorTrace);
            if (Cause != null)
            {
                sb.AppendLine("---Caused by---");
                sb.Append(Cause.ToString());
            }
            return sb.ToString();
        }
    }
}