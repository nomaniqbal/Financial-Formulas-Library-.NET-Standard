using System;
using srbrettle.FinancialFormulas.Core;

namespace srbrettle.FinancialFormulas.Validation
{
    /// <summary>
    /// Provides mathematical domain validation methods for financial calculations
    /// </summary>
    public static class DomainValidator
    {
        /// <summary>
        /// Validates that a value is valid for logarithm operations (must be positive)
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateLogarithmInput(decimal value, string paramName)
        {
            if (value > 0)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = value,
                ConstraintViolated = "Logarithm input must be positive",
                ValidRange = "(0, +∞)",
                AdditionalInfo = "Logarithms are undefined for zero and negative numbers"
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be positive for logarithm operations. Provided value: {value}",
                context
            );
        }

        /// <summary>
        /// Validates that base and exponent values are valid for power operations
        /// </summary>
        /// <param name="baseValue">The base value</param>
        /// <param name="exponent">The exponent value</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidatePowerInput(decimal baseValue, decimal exponent, string paramName)
        {
            // Allow positive base with any exponent
            if (baseValue > 0)
            {
                return ValidationResult.Valid();
            }

            // Zero base is only valid with positive exponent
            if (baseValue == 0 && exponent > 0)
            {
                return ValidationResult.Valid();
            }

            // Negative base is valid only with integer exponents
            if (baseValue < 0 && exponent == Math.Floor(exponent))
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = baseValue,
                ConstraintViolated = "Invalid base-exponent combination",
                ValidRange = "Base > 0 (any exponent), Base = 0 (positive exponent), Base < 0 (integer exponent only)",
                AdditionalInfo = $"Base: {baseValue}, Exponent: {exponent}"
            };

            string errorMessage;
            if (baseValue == 0)
            {
                errorMessage = $"Parameter '{paramName}': Zero base requires a positive exponent. Base: {baseValue}, Exponent: {exponent}";
            }
            else
            {
                errorMessage = $"Parameter '{paramName}': Negative base requires an integer exponent. Base: {baseValue}, Exponent: {exponent}";
            }

            return ValidationResult.Invalid(errorMessage, context);
        }

        /// <summary>
        /// Validates division operation parameters (denominator must not be zero)
        /// </summary>
        /// <param name="numerator">The numerator value</param>
        /// <param name="denominator">The denominator value</param>
        /// <param name="denominatorName">The name of the denominator parameter</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateDivision(decimal numerator, decimal denominator, string denominatorName)
        {
            if (denominator != 0)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = denominatorName,
                ParameterValue = denominator,
                ConstraintViolated = "Denominator cannot be zero",
                ValidRange = "(-∞, 0) ∪ (0, +∞)",
                AdditionalInfo = $"Division by zero is undefined. Numerator: {numerator}"
            };

            return ValidationResult.Invalid(
                $"Parameter '{denominatorName}' cannot be zero. Division by zero is undefined.",
                context
            );
        }

        /// <summary>
        /// Validates that growth rate is less than discount rate (required for perpetuity calculations)
        /// </summary>
        /// <param name="growthRate">The growth rate</param>
        /// <param name="discountRate">The discount rate</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateGrowthRate(decimal growthRate, decimal discountRate)
        {
            if (growthRate < discountRate)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = "growthRate",
                ParameterValue = growthRate,
                ConstraintViolated = "Growth rate must be less than discount rate",
                ValidRange = $"Growth rate < {discountRate}",
                AdditionalInfo = $"Growth rate: {growthRate}, Discount rate: {discountRate}. " +
                               "For perpetuity calculations, growth rate must be strictly less than discount rate."
            };

            return ValidationResult.Invalid(
                $"Growth rate ({growthRate}) must be less than discount rate ({discountRate}) for valid perpetuity calculations.",
                context
            );
        }
    }
}
