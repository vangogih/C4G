using System.Text;

namespace C4G.Core.Utils
{
    public readonly struct Result<TValue, TError>
    {
        public readonly TValue Value;
        public readonly TError Error;
        public readonly string StackTrace;
        public readonly bool IsOk;

        private Result(TValue value, TError error, string stackTrace, bool isOk)
        {
            Value = value;
            Error = error;
            StackTrace = stackTrace;
            IsOk = isOk;
        }

        public static Result<TValue, TError> FromValue(TValue value)
        {
            return new Result<TValue, TError>(
                value: value,
                error: default,
                stackTrace: null,
                isOk: true);
        }

        public static Result<TValue, TError> FromError(TError error)
        {
            return new Result<TValue, TError>(
                value: default, 
                error: error,
                stackTrace: StackTraceUtility.GetAssetPathsOnly(skipFrames: 1),
                isOk: false);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"{nameof(IsOk)} - {IsOk}");
            if (IsOk)
            {
                result.AppendLine($"{nameof(Value)} - {Value}");
            }
            else
            {
                result.AppendLine($"{nameof(Error)} - {Error}");
                result.AppendLine($"{nameof(StackTrace)}:");
                result.AppendLine($"{StackTrace}");
            }
            return result.ToString();
        }
    }

    public readonly struct Result<TError>
    {
        public static readonly Result<TError> Ok = new Result<TError>(
            error: default,
            stackTrace: null,
            isOk: true);

        public readonly TError Error;
        public readonly string StackTrace;
        public readonly bool IsOk;

        private Result(TError error, string stackTrace, bool isOk)
        {
            Error = error;
            StackTrace = stackTrace;
            IsOk = isOk;
        }

        public static Result<TError> FromError(TError error)
        {
            return new Result<TError>(
                error: error,
                stackTrace: StackTraceUtility.GetAssetPathsOnly(skipFrames: 1),
                isOk: false);
        }

        public static Result<TError> FromResultWithValue<TValue>(Result<TValue, TError> result)
        {
            if (result.IsOk)
                return Ok;
            return new Result<TError>(
                error: result.Error,
                stackTrace: result.StackTrace,
                isOk: false);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"{nameof(IsOk)} - {IsOk}");
            if (!IsOk)
            {
                result.AppendLine($"{nameof(Error)} - {Error}");
                result.AppendLine($"{nameof(StackTrace)}:");
                result.AppendLine($"{StackTrace}");
            }
            return result.ToString();
        }
    }

    public static class ResultExtensions
    {
        public static Result<TError> WithoutValue<TValue, TError>(this Result<TValue, TError> result)
        {
            return Result<TError>.FromResultWithValue(result);
        }
    }
}