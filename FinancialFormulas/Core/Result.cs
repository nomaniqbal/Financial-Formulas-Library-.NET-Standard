using System;

namespace srbrettle.FinancialFormulas.Core
{
    /// <summary>
    /// Represents the result of an operation that can either succeed with a value or fail with an error.
    /// Implements the Result pattern for functional error handling.
    /// </summary>
    /// <typeparam name="T">The type of the success value</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Gets whether the operation succeeded
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets whether the operation failed
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the success value (throws if accessed when IsSuccess is false)
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the error message (null if IsSuccess is true)
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Gets additional error context (null if IsSuccess is true)
        /// </summary>
        public ErrorContext ErrorContext { get; }

        private Result(bool isSuccess, T value, string error, ErrorContext context)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            ErrorContext = context;
        }

        /// <summary>
        /// Creates a successful result with the given value
        /// </summary>
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null, null);
        }

        /// <summary>
        /// Creates a failed result with the given error message
        /// </summary>
        public static Result<T> Failure(string error, ErrorContext context = null)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Error message cannot be null or empty", nameof(error));

            return new Result<T>(false, default(T), error, context);
        }

        /// <summary>
        /// Pattern matching for result handling
        /// </summary>
        public TResult Match<TResult>(
            Func<T, TResult> onSuccess,
            Func<string, TResult> onFailure)
        {
            if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
            if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

            return IsSuccess ? onSuccess(Value) : onFailure(Error);
        }

        /// <summary>
        /// Executes an action based on the result state
        /// </summary>
        public void Match(
            Action<T> onSuccess,
            Action<string> onFailure)
        {
            if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
            if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

            if (IsSuccess)
                onSuccess(Value);
            else
                onFailure(Error);
        }

        /// <summary>
        /// Maps the success value to a new type
        /// </summary>
        public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return IsSuccess
                ? Result<TNew>.Success(mapper(Value))
                : Result<TNew>.Failure(Error, ErrorContext);
        }

        /// <summary>
        /// Binds a function that returns a Result
        /// </summary>
        public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return IsSuccess
                ? binder(Value)
                : Result<TNew>.Failure(Error, ErrorContext);
        }

        public override string ToString()
        {
            return IsSuccess
                ? $"Success: {Value}"
                : $"Failure: {Error}";
        }
    }
}
