using System;
using System.Collections.Generic;
using System.Linq;

namespace srbrettle.FinancialFormulas.Core
{
    /// <summary>
    /// Represents the result of a validation operation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Gets whether the validation succeeded (no errors)
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the list of validation error messages
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Gets the error context for the first error (if any)
        /// </summary>
        public ErrorContext Context { get; }

        private ValidationResult(bool isValid, List<string> errors, ErrorContext context)
        {
            IsValid = isValid;
            Errors = errors?.AsReadOnly() ?? new List<string>().AsReadOnly();
            Context = context;
        }

        /// <summary>
        /// Creates a successful validation result
        /// </summary>
        public static ValidationResult Valid()
        {
            return new ValidationResult(true, new List<string>(), null);
        }

        /// <summary>
        /// Creates a failed validation result with a single error
        /// </summary>
        public static ValidationResult Invalid(string error, ErrorContext context = null)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Error message cannot be null or empty", nameof(error));

            return new ValidationResult(false, new List<string> { error }, context);
        }

        /// <summary>
        /// Creates a failed validation result with multiple errors
        /// </summary>
        public static ValidationResult Invalid(IEnumerable<string> errors, ErrorContext context = null)
        {
            var errorList = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (errorList == null || !errorList.Any())
                throw new ArgumentException("Must provide at least one non-empty error message", nameof(errors));

            return new ValidationResult(false, errorList, context);
        }

        /// <summary>
        /// Combines this validation result with another
        /// </summary>
        public ValidationResult Combine(ValidationResult other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (IsValid && other.IsValid)
                return Valid();

            var allErrors = new List<string>();
            allErrors.AddRange(Errors);
            allErrors.AddRange(other.Errors);

            // Use the first context available
            var context = Context ?? other.Context;

            return new ValidationResult(false, allErrors, context);
        }

        /// <summary>
        /// Throws an ArgumentException if validation failed
        /// </summary>
        public void ThrowIfInvalid()
        {
            if (!IsValid)
            {
                var errorMessage = string.Join("; ", Errors);
                var paramName = Context?.ParameterName ?? "value";
                throw new ArgumentException(errorMessage, paramName);
            }
        }

        /// <summary>
        /// Gets the first error message, or null if valid
        /// </summary>
        public string FirstError => Errors.FirstOrDefault();

        /// <summary>
        /// Gets all error messages combined into a single string
        /// </summary>
        public string AllErrors => string.Join("; ", Errors);

        public override string ToString()
        {
            return IsValid ? "Valid" : $"Invalid: {AllErrors}";
        }
    }
}
