using System.Collections.Generic;

namespace WaDesktop.Domain.Common
{
    /// <summary>Kategori kegagalan operasi, menggantikan pengecekan tipe exception.</summary>
    public enum ErrorType
    {
        None = 0,
        Network,
        Unauthorized,
        Forbidden,
        NotFound,
        Validation,
        Conflict,
        Server,
        Unknown
    }

    /// <summary>Satu pasangan field-pesan untuk error validasi per-properti.</summary>
    public class ErrorField
    {
        public string Field { get; }
        public string Message { get; }

        public ErrorField(string field, string message)
        {
            Field = field;
            Message = message;
        }
    }

    /// <summary>Deskripsi kegagalan operasi: kategori, pesan, dan detail per-field.</summary>
    public class Error
    {
        public ErrorType Type { get; }
        public string Message { get; }
        public IReadOnlyList<ErrorField> ErrorFields { get; }

        public Error(ErrorType type, string message, IList<ErrorField> errorFields = null)
        {
            Type = type;
            Message = message ?? string.Empty;
            ErrorFields = errorFields != null
                ? new List<ErrorField>(errorFields)
                : new List<ErrorField>();
        }

        public static Error Network(string message)
        {
            return new Error(ErrorType.Network, message);
        }

        public static Error Unauthorized(string message = "Session expired")
        {
            return new Error(ErrorType.Unauthorized, message);
        }

        public static Error Validation(string message, IList<ErrorField> fields = null)
        {
            return new Error(ErrorType.Validation, message, fields);
        }

        public static Error Server(string message)
        {
            return new Error(ErrorType.Server, message);
        }

        public static Error Unknown(string message)
        {
            return new Error(ErrorType.Unknown, message);
        }
    }
}
