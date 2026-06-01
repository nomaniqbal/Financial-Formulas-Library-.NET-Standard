using System;
using srbrettle.FinancialFormulas.Core;

namespace srbrettle.FinancialFormulas.Validation
{
    /// <summary>
    /// Provides financial business rule validation methods
    /// </summary>
    public static class BusinessRuleValidator
    {
        /// <summary>
        /// Validates that a time period is positive
        /// </summary>
        /// <param name="period">The time period to validate</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateTimePeriod(decimal period, string paramName)
        {
            if (period > 0)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = period,
                ConstraintViolated = "Time period must be positive",
                ValidRange = "(0, +∞)",
                AdditionalInfo = "Time periods represent duration and must be greater than zero"
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be a positive time period. Provided value: {period}",
                context
            );
        }

        /// <summary>
        /// Validates that an interest rate is within a reasonable range
        /// </summary>
        /// <param name="rate">The interest rate to validate (in decimal format, e.g., 0.05 for 5%)</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateInterestRate(decimal rate, string paramName)
        {
            const decimal MinReasonableRate = -0.10m;  // -10% (negative rates are possible in some scenarios)
            const decimal MaxReasonableRate = 1.00m;    // 100% (hyperinflation scenarios)

            if (rate >= MinReasonableRate && rate <= MaxReasonableRate)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = rate,
                ConstraintViolated = "Interest rate outside reasonable range",
                ValidRange = $"[{MinReasonableRate}, {MaxReasonableRate}] or [{MinReasonableRate * 100}%, {MaxReasonableRate * 100}%]",
                AdditionalInfo = "Interest rates should be in decimal format (e.g., 0.05 for 5%). " +
                               "Rates below -10% or above 100% are considered unrealistic for most financial calculations."
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be a reasonable interest rate between {MinReasonableRate} and {MaxReasonableRate}. Provided value: {rate}",
                context
            );
        }

        /// <summary>
        /// Validates that a price is positive
        /// </summary>
        /// <param name="price">The price to validate</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidatePrice(decimal price, string paramName)
        {
            if (price > 0)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = price,
                ConstraintViolated = "Price must be positive",
                ValidRange = "(0, +∞)",
                AdditionalInfo = "Prices represent monetary value and must be greater than zero"
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be a positive price. Provided value: {price}",
                context
            );
        }

        /// <summary>
        /// Validates that a quantity is non-negative
        /// </summary>
        /// <param name="quantity">The quantity to validate</param>
        /// <param name="paramName">The name of the parameter being validated</param>
        /// <returns>A ValidationResult indicating success or failure</returns>
        public static ValidationResult ValidateQuantity(decimal quantity, string paramName)
        {
            if (quantity >= 0)
            {
                return ValidationResult.Valid();
            }

            var context = new ErrorContext
            {
                ParameterName = paramName,
                ParameterValue = quantity,
                ConstraintViolated = "Quantity must be non-negative",
                ValidRange = "[0, +∞)",
                AdditionalInfo = "Quantities cannot be negative. Use zero for empty quantities."
            };

            return ValidationResult.Invalid(
                $"Parameter '{paramName}' must be a non-negative quantity. Provided value: {quantity}",
                context
            );
        }
    }
}
