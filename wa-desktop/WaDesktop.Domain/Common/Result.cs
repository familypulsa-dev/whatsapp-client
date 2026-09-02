namespace WaDesktop.Domain.Common
{
    /// <summary>
    /// Hasil operasi tanpa nilai balik. Menggantikan pola throw/catch
    /// di boundary presenter: cek IsSuccess, baca Error bila gagal.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure { get { return !IsSuccess; } }
        public Error Error { get; }

        protected Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
        {
            return new Result(true, null);
        }

        public static Result Failure(Error error)
        {
            return new Result(false, error ?? Error.Unknown("Unknown error"));
        }
    }

    /// <summary>Hasil operasi dengan nilai balik bertipe <typeparamref name="T"/>.</summary>
    public class Result<T> : Result
    {
        private readonly T _value;

        public T Value
        {
            get { return _value; }
        }

        internal Result(T value) : base(true, null)
        {
            _value = value;
        }

        internal Result(Error error) : base(false, error)
        {
            _value = default(T);
        }

        public static new Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        public static new Result<T> Failure(Error error)
        {
            return new Result<T>(error ?? Error.Unknown("Unknown error"));
        }
    }
}
