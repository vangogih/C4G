using System.Text;

namespace C4G.Core.Errors
{
    public abstract class C4GErrorBase
    {
        public string Message { get; }
        public C4GErrorBase Cause { get; }

        protected C4GErrorBase(string message, C4GErrorBase cause = null)
        {
            Message = message;
            Cause = cause;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"C4G Error. Type '{GetType()}'.\n{Message}\nCause\n{Cause}");
            return stringBuilder.ToString();
        }
    }
}