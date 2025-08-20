namespace C4G.Core.Utils
{
    public readonly struct Result<TValue, TError>
    {
        public readonly TValue Value;
        public readonly TError Error;
        public readonly bool IsOk;

        private Result(TValue value, TError error, bool isOk)
        {
            Value = value;
            Error = error;
            IsOk = isOk;
        }

        public static Result<TValue, TError> FromValue(TValue value)
        {
            return new Result<TValue, TError>(value, default, true);
        }

        public static Result<TValue, TError> FromError(TError error)
        {
            return new Result<TValue, TError>(default, error, false);
        }
    }

    public readonly struct Result<TError>
    {
        public static readonly Result<TError> Ok = new Result<TError>(default, true);

        public readonly TError Error;
        public readonly bool IsOk;

        private Result(TError error, bool isOk)
        {
            Error = error;
            IsOk = isOk;
        }

        public static Result<TError> FromError(TError error)
        {
            return new Result<TError>(error, false);
        }
    }

    public static class ResultExtensions
    {
        public static Result<TError> WithoutValue<TValue, TError>(this Result<TValue, TError> result)
        {
            return result.IsOk ? Result<TError>.Ok : Result<TError>.FromError(result.Error);
        }
    }
}