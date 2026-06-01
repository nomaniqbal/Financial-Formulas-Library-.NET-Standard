using System;
using srbrettle.FinancialFormulas.Core;

namespace srbrettle.FinancialFormulas.Validation
{
    /// <summary>
    /// Provides standard parameter validation methods for financial calculations
    /// </summary>
    public static class ParameterValidator
    {
        /// <summary>
        /// Validates that a value is positive (greater than zero)
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidatePositive(decimal value, string paramName)
        {
            if (value > 0)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = value,
                ConstraintViolated = "Value must be positive (greater than zero)",
                ValidRange = "(0, +∞)"
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be positive. Provided value: {value}",
                context
            );
        }

        /// <summary>
        /// Validates that a value is non-negative (greater than or equal to zero)
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateNonNegative(decimal value, string paramName)
        {
            if (value >= 0)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = value,
                ConstraintViolated = "Value must be non-negative (greater than or equal to zero)",
                ValidRange = "[0, +∞)"
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be non-negative. Provided value: {value}",
                context
            );
        }

        /// <summary>
        /// Validates that a value is non-zero
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateNonZero(decimal value, string paramName)
        {
            if (value != 0)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = value,
                ConstraintViolated = "Value must be non-zero",
                ValidRange = "(-∞, 0) ∪ (0, +∞)"
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be non-zero. Zero is not allowed.",
                context
            );
        }

        /// <summary>
        /// Validates that a value represents a valid percentage (between 0 and 1)
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidatePercentage(decimal value, string paramName)
        {
            if (value >= 0 && value <= 1)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = value,
                ConstraintViolated = "Percentage must be between 0 and 1",
                ValidRange = "[0, 1]",
                AdditionalInfo = "Use decimal format (e.g., 0.05 for 5%)"
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be a valid percentage between 0 and 1. Provided value: {value}",
                context
            );
        }

        /// <summary>
        /// Validates that a value falls within a specified range
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="min">The minimum allowed value (inclusive)</param>
        /// <param name="max">The maximum allowed value (inclusive)</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateRange(decimal value, decimal min, decimal max, string paramName)
        {
            if (min > max)
            {
                throw new ArgumentException($"Minimum value ({min}) cannot be greater than maximum value ({max})");
            }

            if (value >= min && value <= max)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = value,
                ConstraintViolated = $"Value must be within the specified range",
                ValidRange = $"[{min}, {max}]"
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be between {min} and {max}. Provided value: {value}",
                context
            );
        }
    }
}
