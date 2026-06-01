using System;
using srbrettle.FinancialFormulas.Core;
using srbrettle.FinancialFormulas.Validation;

namespace srbrettle.FinancialFormulas
{
    /// <summary>
    /// A collection of methods for solving Financial-Markets-focused Finance/Accounting equations.
    /// </summary>
    public static class FinancialMarketsFormulas
    {
        /// <summary>
        /// Calculates Rate of Inflation from Initial Consumer Price Index and Ending Consumer Price Index
        /// </summary>
        /// <param name="initialConsumerPriceIndex">Initial Consumer Price Index (must be non-zero)</param>
        /// <param name="endingConsumerPriceIndex">Ending Consumer Price Index</param>
        /// <returns>Decimal value for Rate of Inflation</returns>
        /// <exception cref="ArgumentException">Thrown when initialConsumerPriceIndex is zero (division by zero)</exception>
        /// <remarks>
        /// Formula: (endingCPI - initialCPI) / initialCPI
        ///
        /// Parameter Constraints:
        /// - initialConsumerPriceIndex: Must be non-zero (denominator in division)
        /// - endingConsumerPriceIndex: Any decimal value
        ///
        /// Example: If initial CPI is 100 and ending CPI is 105, the rate of inflation is 0.05 (5%)
        /// </remarks>
        public static decimal CalcRateOfInflation(decimal initialConsumerPriceIndex, decimal endingConsumerPriceIndex)
        {
            // Validate division by zero
            var divisionValidation = DomainValidator.ValidateDivision(
                endingConsumerPriceIndex - initialConsumerPriceIndex,
                initialConsumerPriceIndex,
                nameof(initialConsumerPriceIndex)
            );

            if (!divisionValidation.IsValid)
            {
                throw new ArgumentException(divisionValidation.FirstError, nameof(initialConsumerPriceIndex));
            }

            return (endingConsumerPriceIndex - initialConsumerPriceIndex) / initialConsumerPriceIndex;
        }

        /// <summary>
        /// Attempts to calculate Rate of Inflation from Initial Consumer Price Index and Ending Consumer Price Index.
        /// Returns a Result object instead of throwing exceptions.
        /// </summary>
        /// <param name="initialConsumerPriceIndex">Initial Consumer Price Index (must be non-zero)</param>
        /// <param name="endingConsumerPriceIndex">Ending Consumer Price Index</param>
        /// <returns>Result containing the Rate of Inflation on success, or error details on failure</returns>
        /// <remarks>
        /// This is a safe variant of CalcRateOfInflation that returns a Result object instead of throwing exceptions.
        /// Use this method when you want to handle validation errors without exception handling.
        ///
        /// Formula: (endingCPI - initialCPI) / initialCPI
        ///
        /// Parameter Constraints:
        /// - initialConsumerPriceIndex: Must be non-zero (denominator in division)
        /// - endingConsumerPriceIndex: Any decimal value
        ///
        /// Example: If initial CPI is 100 and ending CPI is 105, the rate of inflation is 0.05 (5%)
        /// </remarks>
        public static Result<decimal> TryCalcRateOfInflation(decimal initialConsumerPriceIndex, decimal endingConsumerPriceIndex)
        {
            // Validate division by zero
            var divisionValidation = DomainValidator.ValidateDivision(
                endingConsumerPriceIndex - initialConsumerPriceIndex,
                initialConsumerPriceIndex,
                nameof(initialConsumerPriceIndex)
            );

            if (!divisionValidation.IsValid)
            {
                return Result<decimal>.Failure(divisionValidation.FirstError, divisionValidation.Context);
            }

            var result = (endingConsumerPriceIndex - initialConsumerPriceIndex) / initialConsumerPriceIndex;
            return Result<decimal>.Success(result);
        }

        /// <summary>
        /// Calculates Real Rate of Return from Nominal Rate and Inflation Rate
        /// </summary>
        /// <param name="nominalRate">Nominal Rate (any decimal value)</param>
        /// <param name="inflationRate">Inflation Rate (must not equal -1 to avoid division by zero)</param>
        /// <returns>Decimal value for Real Rate of Return</returns>
        /// <exception cref="ArgumentException">Thrown when inflationRate equals -1 (division by zero)</exception>
        /// <remarks>
        /// Formula: ((1 + nominalRate) / (1 + inflationRate)) - 1
        ///
        /// Parameter Constraints:
        /// - nominalRate: Any decimal value
        /// - inflationRate: Must not equal -1 (would cause division by zero in denominator)
        ///
        /// The Fisher equation is used to calculate the real rate of return, which adjusts the nominal
        /// rate for inflation to show the true purchasing power gain or loss.
        ///
        /// Example: If nominal rate is 0.08 (8%) and inflation rate is 0.03 (3%),
        /// the real rate of return is approximately 0.0485 (4.85%)
        /// </remarks>
        public static decimal CalcRealRateOfReturn(decimal nominalRate, decimal inflationRate)
        {
            // Validate division by zero (1 + inflationRate cannot be zero, i.e., inflationRate cannot be -1)
            var divisionValidation = DomainValidator.ValidateDivision(
                1 + nominalRate,
                1 + inflationRate,
                nameof(inflationRate)
            );

            if (!divisionValidation.IsValid)
            {
                throw new ArgumentException(
                    $"Parameter '{nameof(inflationRate)}' cannot be -1. This would result in division by zero (1 + inflationRate = 0).",
                    nameof(inflationRate)
                );
            }

            return ((1 + nominalRate) / (1 + inflationRate)) - 1;
        }

        /// <summary>
        /// Attempts to calculate Real Rate of Return from Nominal Rate and Inflation Rate.
        /// Returns a Result object instead of throwing exceptions.
        /// </summary>
        /// <param name="nominalRate">Nominal Rate (any decimal value)</param>
        /// <param name="inflationRate">Inflation Rate (must not equal -1 to avoid division by zero)</param>
        /// <returns>Result containing the Real Rate of Return on success, or error details on failure</returns>
        /// <remarks>
        /// This is a safe variant of CalcRealRateOfReturn that returns a Result object instead of throwing exceptions.
        /// Use this method when you want to handle validation errors without exception handling.
        ///
        /// Formula: ((1 + nominalRate) / (1 + inflationRate)) - 1
        ///
        /// Parameter Constraints:
        /// - nominalRate: Any decimal value
        /// - inflationRate: Must not equal -1 (would cause division by zero in denominator)
        ///
        /// The Fisher equation is used to calculate the real rate of return, which adjusts the nominal
        /// rate for inflation to show the true purchasing power gain or loss.
        ///
        /// Example: If nominal rate is 0.08 (8%) and inflation rate is 0.03 (3%),
        /// the real rate of return is approximately 0.0485 (4.85%)
        /// </remarks>
        public static Result<decimal> TryCalcRealRateOfReturn(decimal nominalRate, decimal inflationRate)
        {
            // Validate division by zero (1 + inflationRate cannot be zero, i.e., inflationRate cannot be -1)
            var divisionValidation = DomainValidator.ValidateDivision(
                1 + nominalRate,
                1 + inflationRate,
                nameof(inflationRate)
            );

            if (!divisionValidation.IsValid)
            {
                var context = new ErrorContext
                {
                    ParameterName = nameof(inflationRate),
                    ParameterValue = inflationRate,
                    ConstraintViolated = "Inflation rate cannot be -1 (would cause division by zero)",
                    ValidRange = "(-∞, -1) ∪ (-1, +∞)",
                    AdditionalInfo = "The denominator (1 + inflationRate) must not be zero"
                };

                return Result<decimal>.Failure(
                    $"Parameter '{nameof(inflationRate)}' cannot be -1. This would result in division by zero (1 + inflationRate = 0).",
                    context
                );
            }

            var result = ((1 + nominalRate) / (1 + inflationRate)) - 1;
            return Result<decimal>.Success(result);
        }
    }
}
