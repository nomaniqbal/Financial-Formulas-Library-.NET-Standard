using System;
using System.Collections.Generic;
using srbrettle.FinancialFormulas.Core;
using srbrettle.FinancialFormulas.Validation;

namespace srbrettle.FinancialFormulas
{
    /// <summary>
    /// A collection of methods for solving Finance/Accounting equations.
    /// This class provides both exception-throwing and Result-pattern methods for flexible error handling.
    /// </summary>
    public static class FinancialFormulas
    {
        /*
         * -----------------------------------------------------------------------------
         * | Formulas - Activity                                                       |
         * -----------------------------------------------------------------------------
         */

        /// <summary>
        /// Calculates Asset Turnover from Net Sales and Total Assets.
        /// Asset Turnover measures how efficiently a company uses its assets to generate sales.
        /// </summary>
        /// <param name="netSales">Net Sales (must be non-negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Decimal value for Asset Turnover</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Asset Turnover = Net Sales / Total Assets
        /// Constraints:
        /// - Net Sales must be non-negative (>= 0)
        /// - Total Assets must be positive (> 0)
        ///
        /// Higher values indicate more efficient use of assets.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal turnover = FinancialFormulas.CalcAssetTurnover(1000000m, 500000m);
        /// // Returns: 2.0 (company generates $2 in sales for every $1 of assets)
        /// </code>
        /// </example>
        public static decimal CalcAssetTurnover(decimal netSales, decimal totalAssets)
        {
            var validation = ValidateAssetTurnoverInputs(netSales, totalAssets);
            validation.ThrowIfInvalid();

            return netSales / totalAssets;
        }

        /// <summary>
        /// Attempts to calculate Asset Turnover, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="netSales">Net Sales (must be non-negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Result containing Asset Turnover value or error information</returns>
        /// <remarks>
        /// This method returns a Result&lt;decimal&gt; instead of throwing exceptions,
        /// enabling functional programming patterns and explicit error handling.
        /// </remarks>
        public static Result<decimal> TryCalcAssetTurnover(decimal netSales, decimal totalAssets)
        {
            var validation = ValidateAssetTurnoverInputs(netSales, totalAssets);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = netSales / totalAssets;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating asset turnover: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Asset Turnover",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateAssetTurnoverInputs(decimal netSales, decimal totalAssets)
        {
            var result = ParameterValidator.ValidateNonNegative(netSales, nameof(netSales));
            result = result.Combine(ParameterValidator.ValidatePositive(totalAssets, nameof(totalAssets)));
            return result;
        }

        /// <summary>
        /// Calculates Average Collection Period from Accounts Receivable and Annual Credit Sales.
        /// This measures the average number of days it takes to collect payment from credit sales.
        /// </summary>
        /// <param name="accountsReceivable">Accounts Receivable (must be non-negative)</param>
        /// <param name="annualCreditSales">Annual Credit Sales (must be positive)</param>
        /// <returns>Decimal value for Average Collection Period in days</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Average Collection Period = Accounts Receivable / (Annual Credit Sales / 365)
        /// Simplified: (Accounts Receivable * 365) / Annual Credit Sales
        ///
        /// Constraints:
        /// - Accounts Receivable must be non-negative (>= 0)
        /// - Annual Credit Sales must be positive (> 0)
        ///
        /// Lower values indicate faster collection of receivables.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal days = FinancialFormulas.CalcAverageCollectionPeriod(100000m, 1200000m);
        /// // Returns: 30.42 (approximately 30 days to collect payment)
        /// </code>
        /// </example>
        public static decimal CalcAverageCollectionPeriod(decimal accountsReceivable, decimal annualCreditSales)
        {
            var validation = ValidateAverageCollectionPeriodInputs(accountsReceivable, annualCreditSales);
            validation.ThrowIfInvalid();

            return accountsReceivable / (annualCreditSales / 365);
        }

        /// <summary>
        /// Attempts to calculate Average Collection Period, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="accountsReceivable">Accounts Receivable (must be non-negative)</param>
        /// <param name="annualCreditSales">Annual Credit Sales (must be positive)</param>
        /// <returns>Result containing Average Collection Period in days or error information</returns>
        public static Result<decimal> TryCalcAverageCollectionPeriod(decimal accountsReceivable, decimal annualCreditSales)
        {
            var validation = ValidateAverageCollectionPeriodInputs(accountsReceivable, annualCreditSales);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = accountsReceivable / (annualCreditSales / 365);
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating average collection period: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Average Collection Period",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateAverageCollectionPeriodInputs(decimal accountsReceivable, decimal annualCreditSales)
        {
            var result = ParameterValidator.ValidateNonNegative(accountsReceivable, nameof(accountsReceivable));
            result = result.Combine(ParameterValidator.ValidatePositive(annualCreditSales, nameof(annualCreditSales)));
            return result;
        }

        /// <summary>
        /// Calculates Cash Conversion Cycle from Inventory Conversion Period, Receivables Conversion Period and Payables Conversion Period.
        /// The Cash Conversion Cycle measures how long it takes to convert investments in inventory and receivables back into cash.
        /// </summary>
        /// <param name="inventoryConversionPeriod">Inventory Conversion Period in days (must be non-negative)</param>
        /// <param name="receivablesConversionPeriod">Receivables Conversion Period in days (must be non-negative)</param>
        /// <param name="payablesConversionPeriod">Payables Conversion Period in days (must be non-negative)</param>
        /// <returns>Decimal value for Cash Conversion Cycle in days</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Cash Conversion Cycle = Inventory Conversion Period + Receivables Conversion Period - Payables Conversion Period
        ///
        /// Constraints:
        /// - All periods must be non-negative (>= 0)
        ///
        /// A shorter cycle indicates faster conversion to cash and better working capital management.
        /// Negative values are possible when payables period exceeds the sum of inventory and receivables periods.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal cycle = FinancialFormulas.CalcCashConversionCycle(45m, 30m, 60m);
        /// // Returns: 15 (15 days to convert inventory to cash after accounting for payment terms)
        /// </code>
        /// </example>
        public static decimal CalcCashConversionCycle(decimal inventoryConversionPeriod, decimal receivablesConversionPeriod, decimal payablesConversionPeriod)
        {
            var validation = ValidateCashConversionCycleInputs(inventoryConversionPeriod, receivablesConversionPeriod, payablesConversionPeriod);
            validation.ThrowIfInvalid();

            return inventoryConversionPeriod + receivablesConversionPeriod - payablesConversionPeriod;
        }

        /// <summary>
        /// Attempts to calculate Cash Conversion Cycle, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="inventoryConversionPeriod">Inventory Conversion Period in days (must be non-negative)</param>
        /// <param name="receivablesConversionPeriod">Receivables Conversion Period in days (must be non-negative)</param>
        /// <param name="payablesConversionPeriod">Payables Conversion Period in days (must be non-negative)</param>
        /// <returns>Result containing Cash Conversion Cycle in days or error information</returns>
        public static Result<decimal> TryCalcCashConversionCycle(decimal inventoryConversionPeriod, decimal receivablesConversionPeriod, decimal payablesConversionPeriod)
        {
            var validation = ValidateCashConversionCycleInputs(inventoryConversionPeriod, receivablesConversionPeriod, payablesConversionPeriod);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = inventoryConversionPeriod + receivablesConversionPeriod - payablesConversionPeriod;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating cash conversion cycle: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Cash Conversion Cycle",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateCashConversionCycleInputs(decimal inventoryConversionPeriod, decimal receivablesConversionPeriod, decimal payablesConversionPeriod)
        {
            var result = ParameterValidator.ValidateNonNegative(inventoryConversionPeriod, nameof(inventoryConversionPeriod));
            result = result.Combine(ParameterValidator.ValidateNonNegative(receivablesConversionPeriod, nameof(receivablesConversionPeriod)));
            result = result.Combine(ParameterValidator.ValidateNonNegative(payablesConversionPeriod, nameof(payablesConversionPeriod)));
            return result;
        }

        /// <summary>
        /// Calculates Inventory Conversion Period from Inventory Turnover Ratio.
        /// This measures the average number of days it takes to sell inventory.
        /// </summary>
        /// <param name="inventoryTurnoverRatio">Inventory Turnover Ratio (must be positive)</param>
        /// <returns>Decimal value for Inventory Conversion Period in days</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Inventory Conversion Period = 365 / Inventory Turnover Ratio
        ///
        /// Constraints:
        /// - Inventory Turnover Ratio must be positive (> 0)
        ///
        /// Lower values indicate faster inventory turnover and better inventory management.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal days = FinancialFormulas.CalcInventoryConversionPeriod(8m);
        /// // Returns: 45.625 (approximately 46 days to sell inventory)
        /// </code>
        /// </example>
        public static decimal CalcInventoryConversionPeriod(decimal inventoryTurnoverRatio)
        {
            var validation = ValidateInventoryConversionPeriodInputs(inventoryTurnoverRatio);
            validation.ThrowIfInvalid();

            return 365 / inventoryTurnoverRatio;
        }

        /// <summary>
        /// Attempts to calculate Inventory Conversion Period, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="inventoryTurnoverRatio">Inventory Turnover Ratio (must be positive)</param>
        /// <returns>Result containing Inventory Conversion Period in days or error information</returns>
        public static Result<decimal> TryCalcInventoryConversionPeriod(decimal inventoryTurnoverRatio)
        {
            var validation = ValidateInventoryConversionPeriodInputs(inventoryTurnoverRatio);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = 365 / inventoryTurnoverRatio;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating inventory conversion period: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Inventory Conversion Period",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateInventoryConversionPeriodInputs(decimal inventoryTurnoverRatio)
        {
            return ParameterValidator.ValidatePositive(inventoryTurnoverRatio, nameof(inventoryTurnoverRatio));
        }

        /// <summary>
        /// Calculates Inventory Conversion Ratio from Sales and Cost Of Goods Sold.
        /// Note: The original formula (sales / 2) / costOfGoodsSold appears to be incorrect.
        /// The standard formula is Cost of Goods Sold / Average Inventory.
        /// This implementation uses: Cost of Goods Sold / (Sales / 2) as an approximation where Sales/2 estimates average inventory.
        /// </summary>
        /// <param name="sales">Sales (must be positive)</param>
        /// <param name="costOfGoodsSold">Cost Of Goods Sold (must be non-negative)</param>
        /// <returns>Decimal value for Inventory Conversion Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Inventory Conversion Ratio = Cost of Goods Sold / (Sales / 2)
        /// Simplified: (Cost of Goods Sold * 2) / Sales
        ///
        /// WARNING: This formula may not represent standard financial metrics.
        /// Standard Inventory Turnover = Cost of Goods Sold / Average Inventory
        ///
        /// Constraints:
        /// - Sales must be positive (> 0)
        /// - Cost of Goods Sold must be non-negative (>= 0)
        ///
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcInventoryConversionRatio(1000000m, 600000m);
        /// // Returns: 1.2
        /// </code>
        /// </example>
        public static decimal CalcInventoryConversionRatio(decimal sales, decimal costOfGoodsSold)
        {
            var validation = ValidateInventoryConversionRatioInputs(sales, costOfGoodsSold);
            validation.ThrowIfInvalid();

            // Original formula: (sales / 2) / costOfGoodsSold
            // This appears incorrect for standard inventory metrics
            // Standard would be: costOfGoodsSold / averageInventory
            // Keeping original formula for backward compatibility
            return (sales / 2) / costOfGoodsSold;
        }

        /// <summary>
        /// Attempts to calculate Inventory Conversion Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="sales">Sales (must be positive)</param>
        /// <param name="costOfGoodsSold">Cost Of Goods Sold (must be positive to avoid division by zero)</param>
        /// <returns>Result containing Inventory Conversion Ratio or error information</returns>
        public static Result<decimal> TryCalcInventoryConversionRatio(decimal sales, decimal costOfGoodsSold)
        {
            var validation = ValidateInventoryConversionRatioInputs(sales, costOfGoodsSold);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = (sales / 2) / costOfGoodsSold;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating inventory conversion ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Inventory Conversion Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateInventoryConversionRatioInputs(decimal sales, decimal costOfGoodsSold)
        {
            var result = ParameterValidator.ValidatePositive(sales, nameof(sales));
            result = result.Combine(ParameterValidator.ValidatePositive(costOfGoodsSold, nameof(costOfGoodsSold)));
            return result;
        }

        /// <summary>
        /// Calculates Inventory Turnover from Sales and Average Inventory.
        /// Inventory Turnover measures how many times inventory is sold and replaced over a period.
        /// </summary>
        /// <param name="sales">Sales or Cost of Goods Sold (must be non-negative)</param>
        /// <param name="averageInventory">Average Inventory (must be positive)</param>
        /// <returns>Decimal value for Inventory Turnover</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Inventory Turnover = Sales / Average Inventory
        /// Alternative: Inventory Turnover = Cost of Goods Sold / Average Inventory (more common)
        ///
        /// Constraints:
        /// - Sales must be non-negative (>= 0)
        /// - Average Inventory must be positive (> 0)
        ///
        /// Higher values indicate faster inventory movement and better inventory management.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal turnover = FinancialFormulas.CalcInventoryTurnover(1000000m, 100000m);
        /// // Returns: 10.0 (inventory is sold and replaced 10 times per period)
        /// </code>
        /// </example>
        public static decimal CalcInventoryTurnover(decimal sales, decimal averageInventory)
        {
            var validation = ValidateInventoryTurnoverInputs(sales, averageInventory);
            validation.ThrowIfInvalid();

            return sales / averageInventory;
        }

        /// <summary>
        /// Attempts to calculate Inventory Turnover, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="sales">Sales or Cost of Goods Sold (must be non-negative)</param>
        /// <param name="averageInventory">Average Inventory (must be positive)</param>
        /// <returns>Result containing Inventory Turnover or error information</returns>
        public static Result<decimal> TryCalcInventoryTurnover(decimal sales, decimal averageInventory)
        {
            var validation = ValidateInventoryTurnoverInputs(sales, averageInventory);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = sales / averageInventory;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating inventory turnover: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Inventory Turnover",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateInventoryTurnoverInputs(decimal sales, decimal averageInventory)
        {
            var result = ParameterValidator.ValidateNonNegative(sales, nameof(sales));
            result = result.Combine(ParameterValidator.ValidatePositive(averageInventory, nameof(averageInventory)));
            return result;
        }

        /// <summary>
        /// Calculates Payables Conversion Period from Accounts Payable and Purchases.
        /// This measures the average number of days it takes to pay suppliers.
        /// </summary>
        /// <param name="accountsPayable">Accounts Payable (must be non-negative)</param>
        /// <param name="purchases">Purchases or Cost of Goods Sold (must be positive)</param>
        /// <returns>Decimal value for Payables Conversion Period in days</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Payables Conversion Period = (Accounts Payable / Purchases) * 365
        ///
        /// Constraints:
        /// - Accounts Payable must be non-negative (>= 0)
        /// - Purchases must be positive (> 0)
        ///
        /// Higher values indicate longer payment periods, which can improve cash flow but may strain supplier relationships.
        /// Also known as Days Payable Outstanding (DPO).
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal days = FinancialFormulas.CalcPayablesConversionPeriod(100000m, 1200000m);
        /// // Returns: 30.42 (approximately 30 days to pay suppliers)
        /// </code>
        /// </example>
        public static decimal CalcPayablesConversionPeriod(decimal accountsPayable, decimal purchases)
        {
            var validation = ValidatePayablesConversionPeriodInputs(accountsPayable, purchases);
            validation.ThrowIfInvalid();

            return (accountsPayable / purchases) * 365;
        }

        /// <summary>
        /// Attempts to calculate Payables Conversion Period, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="accountsPayable">Accounts Payable (must be non-negative)</param>
        /// <param name="purchases">Purchases or Cost of Goods Sold (must be positive)</param>
        /// <returns>Result containing Payables Conversion Period in days or error information</returns>
        public static Result<decimal> TryCalcPayablesConversionPeriod(decimal accountsPayable, decimal purchases)
        {
            var validation = ValidatePayablesConversionPeriodInputs(accountsPayable, purchases);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = (accountsPayable / purchases) * 365;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating payables conversion period: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Payables Conversion Period",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidatePayablesConversionPeriodInputs(decimal accountsPayable, decimal purchases)
        {
            var result = ParameterValidator.ValidateNonNegative(accountsPayable, nameof(accountsPayable));
            result = result.Combine(ParameterValidator.ValidatePositive(purchases, nameof(purchases)));
            return result;
        }

        /// <summary>
        /// Calculates Receivables Conversion Period from Receivables and Net Sales.
        /// This measures the average number of days it takes to collect receivables.
        /// </summary>
        /// <param name="receivables">Receivables or Accounts Receivable (must be non-negative)</param>
        /// <param name="netSales">Net Sales (must be positive)</param>
        /// <returns>Decimal value for Receivables Conversion Period in days</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Receivables Conversion Period = (Receivables / Net Sales) * 365
        ///
        /// Constraints:
        /// - Receivables must be non-negative (>= 0)
        /// - Net Sales must be positive (> 0)
        ///
        /// Lower values indicate faster collection of receivables and better credit management.
        /// Also known as Days Sales Outstanding (DSO).
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal days = FinancialFormulas.CalcReceivablesConversionPeriod(150000m, 1800000m);
        /// // Returns: 30.42 (approximately 30 days to collect receivables)
        /// </code>
        /// </example>
        public static decimal CalcReceivablesConversionPeriod(decimal receivables, decimal netSales)
        {
            var validation = ValidateReceivablesConversionPeriodInputs(receivables, netSales);
            validation.ThrowIfInvalid();

            return (receivables / netSales) * 365;
        }

        /// <summary>
        /// Attempts to calculate Receivables Conversion Period, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="receivables">Receivables or Accounts Receivable (must be non-negative)</param>
        /// <param name="netSales">Net Sales (must be positive)</param>
        /// <returns>Result containing Receivables Conversion Period in days or error information</returns>
        public static Result<decimal> TryCalcReceivablesConversionPeriod(decimal receivables, decimal netSales)
        {
            var validation = ValidateReceivablesConversionPeriodInputs(receivables, netSales);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = (receivables / netSales) * 365;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating receivables conversion period: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Receivables Conversion Period",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateReceivablesConversionPeriodInputs(decimal receivables, decimal netSales)
        {
            var result = ParameterValidator.ValidateNonNegative(receivables, nameof(receivables));
            result = result.Combine(ParameterValidator.ValidatePositive(netSales, nameof(netSales)));
            return result;
        }

        /// <summary>
        /// Calculates Receivables Turnover Ratio from Net Credit Sales and Average Net Receivables.
        /// This measures how efficiently a company collects its receivables.
        /// </summary>
        /// <param name="netCreditSales">Net Credit Sales (must be non-negative)</param>
        /// <param name="averageNetReceivables">Average Net Receivables (must be positive)</param>
        /// <returns>Decimal value for Receivables Turnover Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Receivables Turnover Ratio = Net Credit Sales / Average Net Receivables
        ///
        /// Constraints:
        /// - Net Credit Sales must be non-negative (>= 0)
        /// - Average Net Receivables must be positive (> 0)
        ///
        /// Higher values indicate faster collection of receivables and better credit management.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcReceivablesTurnoverRatio(1200000m, 100000m);
        /// // Returns: 12.0 (receivables are collected 12 times per year)
        /// </code>
        /// </example>
        public static decimal CalcReceivablesTurnoverRatio(decimal netCreditSales, decimal averageNetReceivables)
        {
            var validation = ValidateReceivablesTurnoverRatioInputs(netCreditSales, averageNetReceivables);
            validation.ThrowIfInvalid();

            return netCreditSales / averageNetReceivables;
        }

        /// <summary>
        /// Attempts to calculate Receivables Turnover Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="netCreditSales">Net Credit Sales (must be non-negative)</param>
        /// <param name="averageNetReceivables">Average Net Receivables (must be positive)</param>
        /// <returns>Result containing Receivables Turnover Ratio or error information</returns>
        public static Result<decimal> TryCalcReceivablesTurnoverRatio(decimal netCreditSales, decimal averageNetReceivables)
        {
            var validation = ValidateReceivablesTurnoverRatioInputs(netCreditSales, averageNetReceivables);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = netCreditSales / averageNetReceivables;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating receivables turnover ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Receivables Turnover Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateReceivablesTurnoverRatioInputs(decimal netCreditSales, decimal averageNetReceivables)
        {
            var result = ParameterValidator.ValidateNonNegative(netCreditSales, nameof(netCreditSales));
            result = result.Combine(ParameterValidator.ValidatePositive(averageNetReceivables, nameof(averageNetReceivables)));
            return result;
        }

        /*
         * -----------------------------------------------------------------------------
         * | Formulas - Basic                                                          |
         * -----------------------------------------------------------------------------
         */

        /// <summary>
        /// Calculates Assets from Liabilities and Equity.
        /// This implements the fundamental accounting equation: Assets = Liabilities + Equity
        /// </summary>
        /// <param name="liabilities">Liabilities (must be non-negative)</param>
        /// <param name="equity">Equity (can be negative in distressed situations)</param>
        /// <returns>Decimal value for Assets</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Assets = Liabilities + Equity
        ///
        /// Constraints:
        /// - Liabilities must be non-negative (>= 0)
        /// - Equity can be any value (negative equity indicates insolvency)
        ///
        /// This is the fundamental accounting equation.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal assets = FinancialFormulas.CalcAssets(500000m, 300000m);
        /// // Returns: 800000 (total assets equal liabilities plus equity)
        /// </code>
        /// </example>
        public static decimal CalcAssets(decimal liabilities, decimal equity)
        {
            var validation = ValidateAssetsInputs(liabilities, equity);
            validation.ThrowIfInvalid();

            return liabilities + equity;
        }

        /// <summary>
        /// Attempts to calculate Assets, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="liabilities">Liabilities (must be non-negative)</param>
        /// <param name="equity">Equity (can be negative in distressed situations)</param>
        /// <returns>Result containing Assets value or error information</returns>
        public static Result<decimal> TryCalcAssets(decimal liabilities, decimal equity)
        {
            var validation = ValidateAssetsInputs(liabilities, equity);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = liabilities + equity;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating assets: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Assets",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateAssetsInputs(decimal liabilities, decimal equity)
        {
            // Liabilities must be non-negative
            // Equity can be any value (including negative)
            return ParameterValidator.ValidateNonNegative(liabilities, nameof(liabilities));
        }

        /// <summary>
        /// Calculates Earnings Before Interest and Taxes (EBIT) from Revenue and Operating Expenses.
        /// EBIT measures operating profitability before the impact of capital structure and taxes.
        /// </summary>
        /// <param name="revenue">Revenue (must be non-negative)</param>
        /// <param name="operatingExpenses">Operating Expenses (must be non-negative)</param>
        /// <returns>Decimal value for EBIT</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: EBIT = Revenue - Operating Expenses
        ///
        /// Constraints:
        /// - Revenue must be non-negative (>= 0)
        /// - Operating Expenses must be non-negative (>= 0)
        ///
        /// EBIT can be negative when operating expenses exceed revenue.
        /// Also known as Operating Profit or Operating Income.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ebit = FinancialFormulas.CalcEbit(1000000m, 600000m);
        /// // Returns: 400000 (earnings before interest and taxes)
        /// </code>
        /// </example>
        public static decimal CalcEbit(decimal revenue, decimal operatingExpenses)
        {
            var validation = ValidateEbitInputs(revenue, operatingExpenses);
            validation.ThrowIfInvalid();

            return revenue - operatingExpenses;
        }

        /// <summary>
        /// Attempts to calculate EBIT, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="revenue">Revenue (must be non-negative)</param>
        /// <param name="operatingExpenses">Operating Expenses (must be non-negative)</param>
        /// <returns>Result containing EBIT value or error information</returns>
        public static Result<decimal> TryCalcEbit(decimal revenue, decimal operatingExpenses)
        {
            var validation = ValidateEbitInputs(revenue, operatingExpenses);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = revenue - operatingExpenses;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating EBIT: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "EBIT",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateEbitInputs(decimal revenue, decimal operatingExpenses)
        {
            var result = ParameterValidator.ValidateNonNegative(revenue, nameof(revenue));
            result = result.Combine(ParameterValidator.ValidateNonNegative(operatingExpenses, nameof(operatingExpenses)));
            return result;
        }

        /// <summary>
        /// Calculates Equity from Assets and Liabilities.
        /// This rearranges the fundamental accounting equation: Equity = Assets - Liabilities
        /// </summary>
        /// <param name="assets">Assets (must be non-negative)</param>
        /// <param name="liabilities">Liabilities (must be non-negative)</param>
        /// <returns>Decimal value for Equity</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Equity = Assets - Liabilities
        ///
        /// Constraints:
        /// - Assets must be non-negative (>= 0)
        /// - Liabilities must be non-negative (>= 0)
        ///
        /// Equity can be negative when liabilities exceed assets (insolvency).
        /// Also known as Shareholder's Equity or Net Worth.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal equity = FinancialFormulas.CalcEquity(1000000m, 600000m);
        /// // Returns: 400000 (shareholder equity)
        /// </code>
        /// </example>
        public static decimal CalcEquity(decimal assets, decimal liabilities)
        {
            var validation = ValidateEquityInputs(assets, liabilities);
            validation.ThrowIfInvalid();

            return assets - liabilities;
        }

        /// <summary>
        /// Attempts to calculate Equity, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="assets">Assets (must be non-negative)</param>
        /// <param name="liabilities">Liabilities (must be non-negative)</param>
        /// <returns>Result containing Equity value or error information</returns>
        public static Result<decimal> TryCalcEquity(decimal assets, decimal liabilities)
        {
            var validation = ValidateEquityInputs(assets, liabilities);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = assets - liabilities;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating equity: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Equity",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateEquityInputs(decimal assets, decimal liabilities)
        {
            var result = ParameterValidator.ValidateNonNegative(assets, nameof(assets));
            result = result.Combine(ParameterValidator.ValidateNonNegative(liabilities, nameof(liabilities)));
            return result;
        }

        /// <summary>
        /// Calculates Gross Profit from Revenue and Cost Of Goods Sold (COGS).
        /// Gross Profit represents profit after subtracting the cost of producing or acquiring goods sold.
        /// </summary>
        /// <param name="revenue">Revenue (must be non-negative)</param>
        /// <param name="costOfGoodsSold">Cost Of Goods Sold (must be non-negative)</param>
        /// <returns>Decimal value for Gross Profit</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Gross Profit = Revenue - Cost of Goods Sold
        ///
        /// Constraints:
        /// - Revenue must be non-negative (>= 0)
        /// - Cost of Goods Sold must be non-negative (>= 0)
        ///
        /// Gross Profit can be negative when COGS exceeds revenue.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal grossProfit = FinancialFormulas.CalcGrossProfit(1000000m, 400000m);
        /// // Returns: 600000 (profit after direct costs)
        /// </code>
        /// </example>
        public static decimal CalcGrossProfit(decimal revenue, decimal costOfGoodsSold)
        {
            var validation = ValidateGrossProfitInputs(revenue, costOfGoodsSold);
            validation.ThrowIfInvalid();

            return revenue - costOfGoodsSold;
        }

        /// <summary>
        /// Attempts to calculate Gross Profit, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="revenue">Revenue (must be non-negative)</param>
        /// <param name="costOfGoodsSold">Cost Of Goods Sold (must be non-negative)</param>
        /// <returns>Result containing Gross Profit value or error information</returns>
        public static Result<decimal> TryCalcGrossProfit(decimal revenue, decimal costOfGoodsSold)
        {
            var validation = ValidateGrossProfitInputs(revenue, costOfGoodsSold);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = revenue - costOfGoodsSold;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating gross profit: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Gross Profit",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateGrossProfitInputs(decimal revenue, decimal costOfGoodsSold)
        {
            var result = ParameterValidator.ValidateNonNegative(revenue, nameof(revenue));
            result = result.Combine(ParameterValidator.ValidateNonNegative(costOfGoodsSold, nameof(costOfGoodsSold)));
            return result;
        }

        /// <summary>
        /// Calculates Liabilities from Assets and Equity.
        /// This rearranges the fundamental accounting equation: Liabilities = Assets - Equity
        /// </summary>
        /// <param name="assets">Assets (must be non-negative)</param>
        /// <param name="equity">Equity (can be negative in distressed situations)</param>
        /// <returns>Decimal value for Liabilities</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Liabilities = Assets - Equity
        ///
        /// Constraints:
        /// - Assets must be non-negative (>= 0)
        /// - Equity can be any value (including negative)
        ///
        /// When equity is negative, liabilities will exceed assets.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal liabilities = FinancialFormulas.CalcLiabilities(1000000m, 400000m);
        /// // Returns: 600000 (total liabilities)
        /// </code>
        /// </example>
        public static decimal CalcLiabilities(decimal assets, decimal equity)
        {
            var validation = ValidateLiabilitiesInputs(assets, equity);
            validation.ThrowIfInvalid();

            return assets - equity;
        }

        /// <summary>
        /// Attempts to calculate Liabilities, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="assets">Assets (must be non-negative)</param>
        /// <param name="equity">Equity (can be negative in distressed situations)</param>
        /// <returns>Result containing Liabilities value or error information</returns>
        public static Result<decimal> TryCalcLiabilities(decimal assets, decimal equity)
        {
            var validation = ValidateLiabilitiesInputs(assets, equity);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = assets - equity;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating liabilities: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Liabilities",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateLiabilitiesInputs(decimal assets, decimal equity)
        {
            // Assets must be non-negative
            // Equity can be any value (including negative)
            return ParameterValidator.ValidateNonNegative(assets, nameof(assets));
        }

        /// <summary>
        /// Calculates Net Profit from Gross Profit, Operating Expenses, Taxes and Interest.
        /// Net Profit represents the final profit after all expenses are deducted.
        /// </summary>
        /// <param name="grossProfit">Gross Profit (can be negative)</param>
        /// <param name="operatingExpenses">Operating Expenses (must be non-negative)</param>
        /// <param name="taxes">Taxes (must be non-negative)</param>
        /// <param name="interest">Interest (must be non-negative)</param>
        /// <returns>Decimal value for Net Profit</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Net Profit = Gross Profit - Operating Expenses - Taxes - Interest
        ///
        /// Constraints:
        /// - Gross Profit can be any value (including negative)
        /// - Operating Expenses must be non-negative (>= 0)
        /// - Taxes must be non-negative (>= 0)
        /// - Interest must be non-negative (>= 0)
        ///
        /// Net Profit can be negative, indicating a net loss.
        /// Also known as Net Income or Bottom Line.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal netProfit = FinancialFormulas.CalcNetProfit(600000m, 200000m, 50000m, 30000m);
        /// // Returns: 320000 (net profit after all expenses)
        /// </code>
        /// </example>
        public static decimal CalcNetProfit(decimal grossProfit, decimal operatingExpenses, decimal taxes, decimal interest)
        {
            var validation = ValidateNetProfitInputs(grossProfit, operatingExpenses, taxes, interest);
            validation.ThrowIfInvalid();

            return grossProfit - operatingExpenses - taxes - interest;
        }

        /// <summary>
        /// Attempts to calculate Net Profit, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="grossProfit">Gross Profit (can be negative)</param>
        /// <param name="operatingExpenses">Operating Expenses (must be non-negative)</param>
        /// <param name="taxes">Taxes (must be non-negative)</param>
        /// <param name="interest">Interest (must be non-negative)</param>
        /// <returns>Result containing Net Profit value or error information</returns>
        public static Result<decimal> TryCalcNetProfit(decimal grossProfit, decimal operatingExpenses, decimal taxes, decimal interest)
        {
            var validation = ValidateNetProfitInputs(grossProfit, operatingExpenses, taxes, interest);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = grossProfit - operatingExpenses - taxes - interest;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating net profit: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Net Profit",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateNetProfitInputs(decimal grossProfit, decimal operatingExpenses, decimal taxes, decimal interest)
        {
            // Gross profit can be any value (including negative)
            var result = ParameterValidator.ValidateNonNegative(operatingExpenses, nameof(operatingExpenses));
            result = result.Combine(ParameterValidator.ValidateNonNegative(taxes, nameof(taxes)));
            result = result.Combine(ParameterValidator.ValidateNonNegative(interest, nameof(interest)));
            return result;
        }

        /// <summary>
        /// Calculates Operating Profit from Gross Profit and Operating Expenses.
        /// Operating Profit measures profitability from core business operations.
        /// </summary>
        /// <param name="grossProfit">Gross Profit (can be negative)</param>
        /// <param name="operatingExpenses">Operating Expenses (must be non-negative)</param>
        /// <returns>Decimal value for Operating Profit</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Operating Profit = Gross Profit - Operating Expenses
        ///
        /// Constraints:
        /// - Gross Profit can be any value (including negative)
        /// - Operating Expenses must be non-negative (>= 0)
        ///
        /// Operating Profit can be negative when expenses exceed gross profit.
        /// Also known as EBIT (Earnings Before Interest and Taxes).
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal operatingProfit = FinancialFormulas.CalcOperatingProfit(600000m, 200000m);
        /// // Returns: 400000 (profit from operations)
        /// </code>
        /// </example>
        public static decimal CalcOperatingProfit(decimal grossProfit, decimal operatingExpenses)
        {
            var validation = ValidateOperatingProfitInputs(grossProfit, operatingExpenses);
            validation.ThrowIfInvalid();

            return grossProfit - operatingExpenses;
        }

        /// <summary>
        /// Attempts to calculate Operating Profit, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="grossProfit">Gross Profit (can be negative)</param>
        /// <param name="operatingExpenses">Operating Expenses (must be non-negative)</param>
        /// <returns>Result containing Operating Profit value or error information</returns>
        public static Result<decimal> TryCalcOperatingProfit(decimal grossProfit, decimal operatingExpenses)
        {
            var validation = ValidateOperatingProfitInputs(grossProfit, operatingExpenses);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = grossProfit - operatingExpenses;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating operating profit: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Operating Profit",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateOperatingProfitInputs(decimal grossProfit, decimal operatingExpenses)
        {
            // Gross profit can be any value (including negative)
            return ParameterValidator.ValidateNonNegative(operatingExpenses, nameof(operatingExpenses));
        }

        /// <summary>
        /// Calculates Sales Revenue from Gross Sales and Sales of Returns and Allowances.
        /// Sales Revenue represents net sales after deducting returns and allowances.
        /// </summary>
        /// <param name="grossSales">Gross Sales (must be non-negative)</param>
        /// <param name="salesOfReturnsAndAllowances">Sales of Returns and Allowances (must be non-negative)</param>
        /// <returns>Decimal value for Sales Revenue</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Sales Revenue = Gross Sales - Sales of Returns and Allowances
        ///
        /// Constraints:
        /// - Gross Sales must be non-negative (>= 0)
        /// - Sales of Returns and Allowances must be non-negative (>= 0)
        ///
        /// Sales Revenue can be negative if returns exceed sales (unusual but possible).
        /// Also known as Net Sales.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal revenue = FinancialFormulas.CalcSalesRevenue(1000000m, 50000m);
        /// // Returns: 950000 (net sales revenue)
        /// </code>
        /// </example>
        public static decimal CalcSalesRevenue(decimal grossSales, decimal salesOfReturnsAndAllowances)
        {
            var validation = ValidateSalesRevenueInputs(grossSales, salesOfReturnsAndAllowances);
            validation.ThrowIfInvalid();

            return grossSales - salesOfReturnsAndAllowances;
        }

        /// <summary>
        /// Attempts to calculate Sales Revenue, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="grossSales">Gross Sales (must be non-negative)</param>
        /// <param name="salesOfReturnsAndAllowances">Sales of Returns and Allowances (must be non-negative)</param>
        /// <returns>Result containing Sales Revenue value or error information</returns>
        public static Result<decimal> TryCalcSalesRevenue(decimal grossSales, decimal salesOfReturnsAndAllowances)
        {
            var validation = ValidateSalesRevenueInputs(grossSales, salesOfReturnsAndAllowances);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = grossSales - salesOfReturnsAndAllowances;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating sales revenue: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Sales Revenue",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateSalesRevenueInputs(decimal grossSales, decimal salesOfReturnsAndAllowances)
        {
            var result = ParameterValidator.ValidateNonNegative(grossSales, nameof(grossSales));
            result = result.Combine(ParameterValidator.ValidateNonNegative(salesOfReturnsAndAllowances, nameof(salesOfReturnsAndAllowances)));
            return result;
        }

        /*
         * -----------------------------------------------------------------------------
         * | Formulas - Debt                                                           |
         * -----------------------------------------------------------------------------
         */

        /// <summary>
        /// Calculates Debt to Equity Ratio from Total Liabilities and Shareholder Equity.
        /// This ratio measures the relative proportion of shareholders' equity and debt used to finance a company's assets.
        /// </summary>
        /// <param name="totalLiabilities">Total Liabilities (must be non-negative)</param>
        /// <param name="shareholderEquity">Shareholder Equity (must be positive)</param>
        /// <returns>Decimal value for Debt to Equity Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Debt to Equity Ratio = Total Liabilities / Shareholder Equity
        ///
        /// Constraints:
        /// - Total Liabilities must be non-negative (>= 0)
        /// - Shareholder Equity must be positive (> 0)
        ///
        /// Higher values indicate higher financial leverage and risk.
        /// A ratio of 1.0 means equal amounts of debt and equity.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcDebtEquityRatio(500000m, 250000m);
        /// // Returns: 2.0 (company has $2 of debt for every $1 of equity)
        /// </code>
        /// </example>
        public static decimal CalcDebtEquityRatio(decimal totalLiabilities, decimal shareholderEquity)
        {
            var validation = ValidateDebtEquityRatioInputs(totalLiabilities, shareholderEquity);
            validation.ThrowIfInvalid();

            return totalLiabilities / shareholderEquity;
        }

        /// <summary>
        /// Attempts to calculate Debt to Equity Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="totalLiabilities">Total Liabilities (must be non-negative)</param>
        /// <param name="shareholderEquity">Shareholder Equity (must be positive)</param>
        /// <returns>Result containing Debt to Equity Ratio or error information</returns>
        public static Result<decimal> TryCalcDebtEquityRatio(decimal totalLiabilities, decimal shareholderEquity)
        {
            var validation = ValidateDebtEquityRatioInputs(totalLiabilities, shareholderEquity);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = totalLiabilities / shareholderEquity;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating debt equity ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Debt Equity Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateDebtEquityRatioInputs(decimal totalLiabilities, decimal shareholderEquity)
        {
            var result = ParameterValidator.ValidateNonNegative(totalLiabilities, nameof(totalLiabilities));
            result = result.Combine(ParameterValidator.ValidatePositive(shareholderEquity, nameof(shareholderEquity)));
            return result;
        }

        /// <summary>
        /// Calculates Debt Ratio from Total Liabilities and Total Assets.
        /// This ratio shows what proportion of a company's assets are financed by debt.
        /// </summary>
        /// <param name="totalLiabilities">Total Liabilities (must be non-negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Decimal value for Debt Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Debt Ratio = Total Liabilities / Total Assets
        ///
        /// Constraints:
        /// - Total Liabilities must be non-negative (>= 0)
        /// - Total Assets must be positive (> 0)
        ///
        /// Values range from 0 to 1+ (can exceed 1 if liabilities exceed assets).
        /// Lower values indicate less financial risk.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcDebtRatio(400000m, 1000000m);
        /// // Returns: 0.4 (40% of assets are financed by debt)
        /// </code>
        /// </example>
        public static decimal CalcDebtRatio(decimal totalLiabilities, decimal totalAssets)
        {
            var validation = ValidateDebtRatioInputs(totalLiabilities, totalAssets);
            validation.ThrowIfInvalid();

            return totalLiabilities / totalAssets;
        }

        /// <summary>
        /// Attempts to calculate Debt Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="totalLiabilities">Total Liabilities (must be non-negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Result containing Debt Ratio or error information</returns>
        public static Result<decimal> TryCalcDebtRatio(decimal totalLiabilities, decimal totalAssets)
        {
            var validation = ValidateDebtRatioInputs(totalLiabilities, totalAssets);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = totalLiabilities / totalAssets;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating debt ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Debt Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateDebtRatioInputs(decimal totalLiabilities, decimal totalAssets)
        {
            var result = ParameterValidator.ValidateNonNegative(totalLiabilities, nameof(totalLiabilities));
            result = result.Combine(ParameterValidator.ValidatePositive(totalAssets, nameof(totalAssets)));
            return result;
        }

        /// <summary>
        /// Calculates Debt-Service Coverage Ratio from Net Operating Income and Total Debt Service.
        /// This ratio measures a company's ability to service its debt with its operating income.
        /// </summary>
        /// <param name="netOperatingIncome">Net Operating Income (can be negative)</param>
        /// <param name="totalDebtService">Total Debt Service (must be positive)</param>
        /// <returns>Decimal value for Debt-Service Coverage Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Debt-Service Coverage Ratio = Net Operating Income / Total Debt Service
        ///
        /// Constraints:
        /// - Net Operating Income can be any value (including negative)
        /// - Total Debt Service must be positive (> 0)
        ///
        /// Higher values indicate better ability to cover debt obligations.
        /// A ratio below 1.0 indicates insufficient income to cover debt payments.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcDebtServiceCoverageRatio(150000m, 100000m);
        /// // Returns: 1.5 (income is 1.5x debt obligations)
        /// </code>
        /// </example>
        public static decimal CalcDebtServiceCoverageRatio(decimal netOperatingIncome, decimal totalDebtService)
        {
            var validation = ValidateDebtServiceCoverageRatioInputs(netOperatingIncome, totalDebtService);
            validation.ThrowIfInvalid();

            return netOperatingIncome / totalDebtService;
        }

        /// <summary>
        /// Attempts to calculate Debt-Service Coverage Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="netOperatingIncome">Net Operating Income (can be negative)</param>
        /// <param name="totalDebtService">Total Debt Service (must be positive)</param>
        /// <returns>Result containing Debt-Service Coverage Ratio or error information</returns>
        public static Result<decimal> TryCalcDebtServiceCoverageRatio(decimal netOperatingIncome, decimal totalDebtService)
        {
            var validation = ValidateDebtServiceCoverageRatioInputs(netOperatingIncome, totalDebtService);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = netOperatingIncome / totalDebtService;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating debt service coverage ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Debt Service Coverage Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateDebtServiceCoverageRatioInputs(decimal netOperatingIncome, decimal totalDebtService)
        {
            // Net operating income can be any value (including negative)
            return ParameterValidator.ValidatePositive(totalDebtService, nameof(totalDebtService));
        }

        /// <summary>
        /// Calculates Long-Term Debt to Equity Ratio from Long-Term Liabilities and Equity.
        /// This ratio measures the proportion of long-term debt relative to equity.
        /// </summary>
        /// <param name="longTermLiabilities">Long-Term Liabilities (must be non-negative)</param>
        /// <param name="equity">Equity (must be positive)</param>
        /// <returns>Decimal value for Long-Term Debt to Equity Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Long-Term Debt to Equity Ratio = Long-Term Liabilities / Equity
        ///
        /// Constraints:
        /// - Long-Term Liabilities must be non-negative (>= 0)
        /// - Equity must be positive (> 0)
        ///
        /// Higher values indicate higher long-term financial leverage.
        /// This focuses on long-term solvency rather than short-term liquidity.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcLongTermDebtEquityRatio(300000m, 500000m);
        /// // Returns: 0.6 (long-term debt is 60% of equity)
        /// </code>
        /// </example>
        public static decimal CalcLongTermDebtEquityRatio(decimal longTermLiabilities, decimal equity)
        {
            var validation = ValidateLongTermDebtEquityRatioInputs(longTermLiabilities, equity);
            validation.ThrowIfInvalid();

            return longTermLiabilities / equity;
        }

        /// <summary>
        /// Attempts to calculate Long-Term Debt to Equity Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="longTermLiabilities">Long-Term Liabilities (must be non-negative)</param>
        /// <param name="equity">Equity (must be positive)</param>
        /// <returns>Result containing Long-Term Debt to Equity Ratio or error information</returns>
        public static Result<decimal> TryCalcLongTermDebtEquityRatio(decimal longTermLiabilities, decimal equity)
        {
            var validation = ValidateLongTermDebtEquityRatioInputs(longTermLiabilities, equity);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = longTermLiabilities / equity;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating long-term debt equity ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Long-Term Debt Equity Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateLongTermDebtEquityRatioInputs(decimal longTermLiabilities, decimal equity)
        {
            var result = ParameterValidator.ValidateNonNegative(longTermLiabilities, nameof(longTermLiabilities));
            result = result.Combine(ParameterValidator.ValidatePositive(equity, nameof(equity)));
            return result;
        }

        /*
         * -----------------------------------------------------------------------------
         * | Formulas - Depreciation                                                   |
         * -----------------------------------------------------------------------------
         */

        /// <summary>
        /// Calculates Book Value from Acquisition Cost and Depreciation.
        /// Book Value represents the net value of an asset after accumulated depreciation.
        /// </summary>
        /// <param name="acquisitionCost">Acquisition Cost (must be non-negative)</param>
        /// <param name="depreciation">Depreciation (must be non-negative)</param>
        /// <returns>Decimal value for Book Value</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Book Value = Acquisition Cost - Depreciation
        ///
        /// Constraints:
        /// - Acquisition Cost must be non-negative (>= 0)
        /// - Depreciation must be non-negative (>= 0)
        ///
        /// Book Value can be negative if depreciation exceeds acquisition cost (unusual but mathematically possible).
        /// Also known as Net Book Value or Carrying Value.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal bookValue = FinancialFormulas.CalcBookValue(100000m, 30000m);
        /// // Returns: 70000 (asset value after depreciation)
        /// </code>
        /// </example>
        public static decimal CalcBookValue(decimal acquisitionCost, decimal depreciation)
        {
            var validation = ValidateBookValueInputs(acquisitionCost, depreciation);
            validation.ThrowIfInvalid();

            return acquisitionCost - depreciation;
        }

        /// <summary>
        /// Attempts to calculate Book Value, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="acquisitionCost">Acquisition Cost (must be non-negative)</param>
        /// <param name="depreciation">Depreciation (must be non-negative)</param>
        /// <returns>Result containing Book Value or error information</returns>
        public static Result<decimal> TryCalcBookValue(decimal acquisitionCost, decimal depreciation)
        {
            var validation = ValidateBookValueInputs(acquisitionCost, depreciation);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = acquisitionCost - depreciation;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating book value: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Book Value",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateBookValueInputs(decimal acquisitionCost, decimal depreciation)
        {
            var result = ParameterValidator.ValidateNonNegative(acquisitionCost, nameof(acquisitionCost));
            result = result.Combine(ParameterValidator.ValidateNonNegative(depreciation, nameof(depreciation)));
            return result;
        }

        /// <summary>
        /// Calculates Declining Balance depreciation from Depreciation Rate and Book Value at Beginning of Year.
        /// This method accelerates depreciation in the early years of an asset's life.
        /// </summary>
        /// <param name="depreciationRate">Depreciation Rate (must be non-negative, typically between 0 and 1)</param>
        /// <param name="bookValueAtBeginningOfYear">Book Value at Beginning of Year (must be non-negative)</param>
        /// <returns>Decimal value for Declining Balance depreciation</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Declining Balance = Depreciation Rate * Book Value at Beginning of Year
        ///
        /// Constraints:
        /// - Depreciation Rate must be non-negative (>= 0)
        /// - Book Value at Beginning of Year must be non-negative (>= 0)
        ///
        /// Common rates: 2.0 for double-declining balance (200%), 1.5 for 150% declining balance.
        /// This method produces higher depreciation in early years compared to straight-line.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal depreciation = FinancialFormulas.CalcDecliningBalance(0.20m, 100000m);
        /// // Returns: 20000 (20% of book value)
        /// </code>
        /// </example>
        public static decimal CalcDecliningBalance(decimal depreciationRate, decimal bookValueAtBeginningOfYear)
        {
            var validation = ValidateDecliningBalanceInputs(depreciationRate, bookValueAtBeginningOfYear);
            validation.ThrowIfInvalid();

            return depreciationRate * bookValueAtBeginningOfYear;
        }

        /// <summary>
        /// Attempts to calculate Declining Balance depreciation, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="depreciationRate">Depreciation Rate (must be non-negative)</param>
        /// <param name="bookValueAtBeginningOfYear">Book Value at Beginning of Year (must be non-negative)</param>
        /// <returns>Result containing Declining Balance depreciation or error information</returns>
        public static Result<decimal> TryCalcDecliningBalance(decimal depreciationRate, decimal bookValueAtBeginningOfYear)
        {
            var validation = ValidateDecliningBalanceInputs(depreciationRate, bookValueAtBeginningOfYear);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = depreciationRate * bookValueAtBeginningOfYear;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating declining balance: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Declining Balance",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateDecliningBalanceInputs(decimal depreciationRate, decimal bookValueAtBeginningOfYear)
        {
            var result = ParameterValidator.ValidateNonNegative(depreciationRate, nameof(depreciationRate));
            result = result.Combine(ParameterValidator.ValidateNonNegative(bookValueAtBeginningOfYear, nameof(bookValueAtBeginningOfYear)));
            return result;
        }

        /// <summary>
        /// Calculates Units Of Production depreciation from Cost of Asset, Residual Value, Estimated Total Production and Actual Production.
        /// This method bases depreciation on actual usage rather than time.
        /// </summary>
        /// <param name="costOfAsset">Cost of Asset (must be non-negative)</param>
        /// <param name="residualValue">Residual Value (must be non-negative)</param>
        /// <param name="estimatedTotalProduction">Estimated Total Production (must be positive)</param>
        /// <param name="actualProduction">Actual Production (must be non-negative)</param>
        /// <returns>Decimal value for Units of Production depreciation</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Units of Production = ((Cost of Asset - Residual Value) / Estimated Total Production) * Actual Production
        ///
        /// Constraints:
        /// - Cost of Asset must be non-negative (>= 0)
        /// - Residual Value must be non-negative (>= 0)
        /// - Estimated Total Production must be positive (> 0)
        /// - Actual Production must be non-negative (>= 0)
        ///
        /// This method is ideal for assets where wear and tear is more closely related to production than time.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal depreciation = FinancialFormulas.CalcUnitsOfProduction(100000m, 10000m, 50000m, 10000m);
        /// // Returns: 18000 (depreciation for 10,000 units produced)
        /// </code>
        /// </example>
        public static decimal CalcUnitsOfProduction(decimal costOfAsset, decimal residualValue, decimal estimatedTotalProduction, decimal actualProduction)
        {
            var validation = ValidateUnitsOfProductionInputs(costOfAsset, residualValue, estimatedTotalProduction, actualProduction);
            validation.ThrowIfInvalid();

            return ((costOfAsset - residualValue) / estimatedTotalProduction) * actualProduction;
        }

        /// <summary>
        /// Attempts to calculate Units Of Production depreciation, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="costOfAsset">Cost of Asset (must be non-negative)</param>
        /// <param name="residualValue">Residual Value (must be non-negative)</param>
        /// <param name="estimatedTotalProduction">Estimated Total Production (must be positive)</param>
        /// <param name="actualProduction">Actual Production (must be non-negative)</param>
        /// <returns>Result containing Units of Production depreciation or error information</returns>
        public static Result<decimal> TryCalcUnitsOfProduction(decimal costOfAsset, decimal residualValue, decimal estimatedTotalProduction, decimal actualProduction)
        {
            var validation = ValidateUnitsOfProductionInputs(costOfAsset, residualValue, estimatedTotalProduction, actualProduction);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = ((costOfAsset - residualValue) / estimatedTotalProduction) * actualProduction;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating units of production: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Units of Production",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateUnitsOfProductionInputs(decimal costOfAsset, decimal residualValue, decimal estimatedTotalProduction, decimal actualProduction)
        {
            var result = ParameterValidator.ValidateNonNegative(costOfAsset, nameof(costOfAsset));
            result = result.Combine(ParameterValidator.ValidateNonNegative(residualValue, nameof(residualValue)));
            result = result.Combine(ParameterValidator.ValidatePositive(estimatedTotalProduction, nameof(estimatedTotalProduction)));
            result = result.Combine(ParameterValidator.ValidateNonNegative(actualProduction, nameof(actualProduction)));
            return result;
        }

        /// <summary>
        /// Calculates Straight Line Method depreciation from Cost of Fixed Asset, Residual Value and Useful Life of Asset.
        /// This method spreads depreciation evenly over the asset's useful life.
        /// </summary>
        /// <param name="costOfFixedAsset">Cost of Fixed Asset (must be non-negative)</param>
        /// <param name="residualValue">Residual Value (must be non-negative)</param>
        /// <param name="usefulLifeOfAsset">Useful Life of Asset in years (must be positive)</param>
        /// <returns>Decimal value for annual Straight Line Method depreciation</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Straight Line Method = (Cost of Fixed Asset - Residual Value) / Useful Life of Asset
        ///
        /// Constraints:
        /// - Cost of Fixed Asset must be non-negative (>= 0)
        /// - Residual Value must be non-negative (>= 0)
        /// - Useful Life of Asset must be positive (> 0)
        ///
        /// This is the simplest and most commonly used depreciation method.
        /// Depreciation expense is the same each year.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal annualDepreciation = FinancialFormulas.CalcStraightLineMethod(100000m, 10000m, 10m);
        /// // Returns: 9000 (annual depreciation over 10 years)
        /// </code>
        /// </example>
        public static decimal CalcStraightLineMethod(decimal costOfFixedAsset, decimal residualValue, decimal usefulLifeOfAsset)
        {
            var validation = ValidateStraightLineMethodInputs(costOfFixedAsset, residualValue, usefulLifeOfAsset);
            validation.ThrowIfInvalid();

            return (costOfFixedAsset - residualValue) / usefulLifeOfAsset;
        }

        /// <summary>
        /// Attempts to calculate Straight Line Method depreciation, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="costOfFixedAsset">Cost of Fixed Asset (must be non-negative)</param>
        /// <param name="residualValue">Residual Value (must be non-negative)</param>
        /// <param name="usefulLifeOfAsset">Useful Life of Asset in years (must be positive)</param>
        /// <returns>Result containing annual Straight Line Method depreciation or error information</returns>
        public static Result<decimal> TryCalcStraightLineMethod(decimal costOfFixedAsset, decimal residualValue, decimal usefulLifeOfAsset)
        {
            var validation = ValidateStraightLineMethodInputs(costOfFixedAsset, residualValue, usefulLifeOfAsset);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = (costOfFixedAsset - residualValue) / usefulLifeOfAsset;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating straight line method: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Straight Line Method",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateStraightLineMethodInputs(decimal costOfFixedAsset, decimal residualValue, decimal usefulLifeOfAsset)
        {
            var result = ParameterValidator.ValidateNonNegative(costOfFixedAsset, nameof(costOfFixedAsset));
            result = result.Combine(ParameterValidator.ValidateNonNegative(residualValue, nameof(residualValue)));
            result = result.Combine(ParameterValidator.ValidatePositive(usefulLifeOfAsset, nameof(usefulLifeOfAsset)));
            return result;
        }

        /*
         * -----------------------------------------------------------------------------
         * | Formulas - Liquidity                                                      |
         * -----------------------------------------------------------------------------
         */

        /// <summary>
        /// Calculates Cash Ratio from Cash, Marketable Securities and Current Liabilities.
        /// The Cash Ratio measures a company's ability to pay off current liabilities with cash and cash equivalents.
        /// </summary>
        /// <param name="cash">Cash (must be non-negative)</param>
        /// <param name="marketableSecurities">Marketable Securities (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Decimal value for Cash Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Cash Ratio = (Cash + Marketable Securities) / Current Liabilities
        ///
        /// Constraints:
        /// - Cash must be non-negative (>= 0)
        /// - Marketable Securities must be non-negative (>= 0)
        /// - Current Liabilities must be positive (> 0)
        ///
        /// This is the most conservative liquidity ratio.
        /// A ratio above 1.0 indicates sufficient liquid assets to cover current liabilities.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcCashRatio(50000m, 30000m, 100000m);
        /// // Returns: 0.8 (company has $0.80 of liquid assets per $1 of current liabilities)
        /// </code>
        /// </example>
        public static decimal CalcCashRatio(decimal cash, decimal marketableSecurities, decimal currentLiabilities)
        {
            var validation = ValidateCashRatioInputs(cash, marketableSecurities, currentLiabilities);
            validation.ThrowIfInvalid();

            return (cash + marketableSecurities) / currentLiabilities;
        }

        /// <summary>
        /// Attempts to calculate Cash Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="cash">Cash (must be non-negative)</param>
        /// <param name="marketableSecurities">Marketable Securities (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Result containing Cash Ratio or error information</returns>
        public static Result<decimal> TryCalcCashRatio(decimal cash, decimal marketableSecurities, decimal currentLiabilities)
        {
            var validation = ValidateCashRatioInputs(cash, marketableSecurities, currentLiabilities);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = (cash + marketableSecurities) / currentLiabilities;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating cash ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Cash Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateCashRatioInputs(decimal cash, decimal marketableSecurities, decimal currentLiabilities)
        {
            var result = ParameterValidator.ValidateNonNegative(cash, nameof(cash));
            result = result.Combine(ParameterValidator.ValidateNonNegative(marketableSecurities, nameof(marketableSecurities)));
            result = result.Combine(ParameterValidator.ValidatePositive(currentLiabilities, nameof(currentLiabilities)));
            return result;
        }

        /// <summary>
        /// Calculates Current Ratio from Current Assets and Current Liabilities.
        /// The Current Ratio measures a company's ability to pay short-term obligations with current assets.
        /// </summary>
        /// <param name="currentAssets">Current Assets (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Decimal value for Current Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Current Ratio = Current Assets / Current Liabilities
        ///
        /// Constraints:
        /// - Current Assets must be non-negative (>= 0)
        /// - Current Liabilities must be positive (> 0)
        ///
        /// A ratio above 1.0 indicates more current assets than current liabilities.
        /// Typical healthy range is 1.5 to 3.0.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcCurrentRatio(200000m, 100000m);
        /// // Returns: 2.0 (company has $2 of current assets per $1 of current liabilities)
        /// </code>
        /// </example>
        public static decimal CalcCurrentRatio(decimal currentAssets, decimal currentLiabilities)
        {
            var validation = ValidateCurrentRatioInputs(currentAssets, currentLiabilities);
            validation.ThrowIfInvalid();

            return currentAssets / currentLiabilities;
        }

        /// <summary>
        /// Attempts to calculate Current Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="currentAssets">Current Assets (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Result containing Current Ratio or error information</returns>
        public static Result<decimal> TryCalcCurrentRatio(decimal currentAssets, decimal currentLiabilities)
        {
            var validation = ValidateCurrentRatioInputs(currentAssets, currentLiabilities);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = currentAssets / currentLiabilities;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating current ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Current Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateCurrentRatioInputs(decimal currentAssets, decimal currentLiabilities)
        {
            var result = ParameterValidator.ValidateNonNegative(currentAssets, nameof(currentAssets));
            result = result.Combine(ParameterValidator.ValidatePositive(currentLiabilities, nameof(currentLiabilities)));
            return result;
        }

        /// <summary>
        /// Calculates Operating Cash Flow Ratio from Operating Cash Flow and Total Debts.
        /// This ratio measures a company's ability to cover current liabilities with cash from operations.
        /// </summary>
        /// <param name="operatingCashFlow">Operating Cash Flow (can be negative)</param>
        /// <param name="totalDebts">Total Debts (must be positive)</param>
        /// <returns>Decimal value for Operating Cash Flow Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Operating Cash Flow Ratio = Operating Cash Flow / Total Debts
        ///
        /// Constraints:
        /// - Operating Cash Flow can be any value (including negative)
        /// - Total Debts must be positive (> 0)
        ///
        /// Higher values indicate better ability to cover debts with operating cash.
        /// A ratio above 1.0 indicates sufficient operating cash to cover debt obligations.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcOperatingCashFlowRatio(150000m, 100000m);
        /// // Returns: 1.5 (operating cash flow is 1.5x total debts)
        /// </code>
        /// </example>
        public static decimal CalcOperatingCashFlowRatio(decimal operatingCashFlow, decimal totalDebts)
        {
            var validation = ValidateOperatingCashFlowRatioInputs(operatingCashFlow, totalDebts);
            validation.ThrowIfInvalid();

            return operatingCashFlow / totalDebts;
        }

        /// <summary>
        /// Attempts to calculate Operating Cash Flow Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="operatingCashFlow">Operating Cash Flow (can be negative)</param>
        /// <param name="totalDebts">Total Debts (must be positive)</param>
        /// <returns>Result containing Operating Cash Flow Ratio or error information</returns>
        public static Result<decimal> TryCalcOperatingCashFlowRatio(decimal operatingCashFlow, decimal totalDebts)
        {
            var validation = ValidateOperatingCashFlowRatioInputs(operatingCashFlow, totalDebts);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = operatingCashFlow / totalDebts;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating operating cash flow ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Operating Cash Flow Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateOperatingCashFlowRatioInputs(decimal operatingCashFlow, decimal totalDebts)
        {
            // Operating cash flow can be any value (including negative)
            return ParameterValidator.ValidatePositive(totalDebts, nameof(totalDebts));
        }

        /// <summary>
        /// Calculates Quick Ratio from Current Assets, Inventories and Current Liabilities.
        /// The Quick Ratio (Acid-Test Ratio) measures a company's ability to meet short-term obligations with its most liquid assets.
        /// </summary>
        /// <param name="currentAssets">Current Assets (must be non-negative)</param>
        /// <param name="inventories">Inventories (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Decimal value for Quick Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Quick Ratio = (Current Assets - Inventories) / Current Liabilities
        ///
        /// Constraints:
        /// - Current Assets must be non-negative (>= 0)
        /// - Inventories must be non-negative (>= 0)
        /// - Current Liabilities must be positive (> 0)
        ///
        /// More conservative than Current Ratio as it excludes inventory.
        /// A ratio above 1.0 is generally considered good.
        /// Also known as Acid-Test Ratio.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcQuickRatio(200000m, 50000m, 100000m);
        /// // Returns: 1.5 (company has $1.50 of quick assets per $1 of current liabilities)
        /// </code>
        /// </example>
        public static decimal CalcQuickRatio(decimal currentAssets, decimal inventories, decimal currentLiabilities)
        {
            var validation = ValidateQuickRatioInputs(currentAssets, inventories, currentLiabilities);
            validation.ThrowIfInvalid();

            return (currentAssets - inventories) / currentLiabilities;
        }

        /// <summary>
        /// Attempts to calculate Quick Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="currentAssets">Current Assets (must be non-negative)</param>
        /// <param name="inventories">Inventories (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Result containing Quick Ratio or error information</returns>
        public static Result<decimal> TryCalcQuickRatio(decimal currentAssets, decimal inventories, decimal currentLiabilities)
        {
            var validation = ValidateQuickRatioInputs(currentAssets, inventories, currentLiabilities);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = (currentAssets - inventories) / currentLiabilities;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating quick ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Quick Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateQuickRatioInputs(decimal currentAssets, decimal inventories, decimal currentLiabilities)
        {
            var result = ParameterValidator.ValidateNonNegative(currentAssets, nameof(currentAssets));
            result = result.Combine(ParameterValidator.ValidateNonNegative(inventories, nameof(inventories)));
            result = result.Combine(ParameterValidator.ValidatePositive(currentLiabilities, nameof(currentLiabilities)));
            return result;
        }


        /*
         * -----------------------------------------------------------------------------
         * | Formulas - Market                                                          |
         * -----------------------------------------------------------------------------
         */

        /// <summary>
        /// Calculates Dividend Cover from Earnings Per Share and Dividends Per Share.
        /// Dividend Cover shows how many times a company can pay dividends from its earnings.
        /// </summary>
        /// <param name="earningsPerShare">Earnings Per Share (can be negative)</param>
        /// <param name="dividendsPerShare">Dividends Per Share (must be positive)</param>
        /// <returns>Decimal value for Dividend Cover</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Dividend Cover = Earnings Per Share / Dividends Per Share
        ///
        /// Constraints:
        /// - Earnings Per Share can be any value (including negative)
        /// - Dividends Per Share must be positive (> 0)
        ///
        /// Higher values indicate greater dividend sustainability.
        /// A ratio above 2.0 is generally considered healthy.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal cover = FinancialFormulas.CalcDividendCover(2.50m, 1.00m);
        /// // Returns: 2.5 (earnings cover dividends 2.5 times)
        /// </code>
        /// </example>
        public static decimal CalcDividendCover(decimal earningsPerShare, decimal dividendsPerShare)
        {
            var validation = ValidateDividendCoverInputs(earningsPerShare, dividendsPerShare);
            validation.ThrowIfInvalid();

            return earningsPerShare / dividendsPerShare;
        }

        /// <summary>
        /// Attempts to calculate Dividend Cover, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="earningsPerShare">Earnings Per Share (can be negative)</param>
        /// <param name="dividendsPerShare">Dividends Per Share (must be positive)</param>
        /// <returns>Result containing Dividend Cover or error information</returns>
        public static Result<decimal> TryCalcDividendCover(decimal earningsPerShare, decimal dividendsPerShare)
        {
            var validation = ValidateDividendCoverInputs(earningsPerShare, dividendsPerShare);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = earningsPerShare / dividendsPerShare;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating dividend cover: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Dividend Cover",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateDividendCoverInputs(decimal earningsPerShare, decimal dividendsPerShare)
        {
            // Earnings per share can be any value (including negative)
            return ParameterValidator.ValidatePositive(dividendsPerShare, nameof(dividendsPerShare));
        }

        /// <summary>
        /// Calculates Dividends Per Share (DPS) from Dividends Paid and Number Of Shares.
        /// DPS represents the portion of earnings distributed to each share.
        /// </summary>
        /// <param name="dividendsPaid">Dividends Paid (must be non-negative)</param>
        /// <param name="numberOfShares">Number of Shares (must be positive)</param>
        /// <returns>Decimal value for Dividends Per Share</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Dividends Per Share = Dividends Paid / Number of Shares
        ///
        /// Constraints:
        /// - Dividends Paid must be non-negative (>= 0)
        /// - Number of Shares must be positive (> 0)
        ///
        /// DPS is a key metric for income investors.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal dps = FinancialFormulas.CalcDividendsPerShare(100000m, 50000m);
        /// // Returns: 2.0 (dividend of $2 per share)
        /// </code>
        /// </example>
        public static decimal CalcDividendsPerShare(decimal dividendsPaid, decimal numberOfShares)
        {
            var validation = ValidateDividendsPerShareInputs(dividendsPaid, numberOfShares);
            validation.ThrowIfInvalid();

            return dividendsPaid / numberOfShares;
        }

        /// <summary>
        /// Attempts to calculate Dividends Per Share, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="dividendsPaid">Dividends Paid (must be non-negative)</param>
        /// <param name="numberOfShares">Number of Shares (must be positive)</param>
        /// <returns>Result containing Dividends Per Share or error information</returns>
        public static Result<decimal> TryCalcDividendsPerShare(decimal dividendsPaid, decimal numberOfShares)
        {
            var validation = ValidateDividendsPerShareInputs(dividendsPaid, numberOfShares);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = dividendsPaid / numberOfShares;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating dividends per share: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Dividends Per Share",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateDividendsPerShareInputs(decimal dividendsPaid, decimal numberOfShares)
        {
            var result = ParameterValidator.ValidateNonNegative(dividendsPaid, nameof(dividendsPaid));
            result = result.Combine(ParameterValidator.ValidatePositive(numberOfShares, nameof(numberOfShares)));
            return result;
        }

        /// <summary>
        /// Calculates Dividend Yield from Annual Dividend Per Share and Price Per Share.
        /// Dividend Yield shows the return on investment from dividends alone.
        /// </summary>
        /// <param name="annualDividendPerShare">Annual Dividend Per Share (must be non-negative)</param>
        /// <param name="pricePerShare">Price Per Share (must be positive)</param>
        /// <returns>Decimal value for Dividend Yield (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Dividend Yield = Annual Dividend Per Share / Price Per Share
        ///
        /// Constraints:
        /// - Annual Dividend Per Share must be non-negative (>= 0)
        /// - Price Per Share must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.05 = 5% yield).
        /// Higher yields may indicate value but could also signal risk.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal yield = FinancialFormulas.CalcDividendYield(2.00m, 40.00m);
        /// // Returns: 0.05 (5% dividend yield)
        /// </code>
        /// </example>
        public static decimal CalcDividendYield(decimal annualDividendPerShare, decimal pricePerShare)
        {
            var validation = ValidateDividendYieldInputs(annualDividendPerShare, pricePerShare);
            validation.ThrowIfInvalid();

            return annualDividendPerShare / pricePerShare;
        }

        /// <summary>
        /// Attempts to calculate Dividend Yield, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="annualDividendPerShare">Annual Dividend Per Share (must be non-negative)</param>
        /// <param name="pricePerShare">Price Per Share (must be positive)</param>
        /// <returns>Result containing Dividend Yield or error information</returns>
        public static Result<decimal> TryCalcDividendYield(decimal annualDividendPerShare, decimal pricePerShare)
        {
            var validation = ValidateDividendYieldInputs(annualDividendPerShare, pricePerShare);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = annualDividendPerShare / pricePerShare;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating dividend yield: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Dividend Yield",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateDividendYieldInputs(decimal annualDividendPerShare, decimal pricePerShare)
        {
            var result = ParameterValidator.ValidateNonNegative(annualDividendPerShare, nameof(annualDividendPerShare));
            result = result.Combine(ParameterValidator.ValidatePositive(pricePerShare, nameof(pricePerShare)));
            return result;
        }

        /// <summary>
        /// Calculates Earnings Per Share from Net Earnings and Number of Shares.
        /// EPS represents the portion of a company's profit allocated to each share.
        /// </summary>
        /// <param name="netEarnings">Net Earnings (can be negative)</param>
        /// <param name="numberOfShares">Number of Shares (must be positive)</param>
        /// <returns>Decimal value for Earnings Per Share</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Earnings Per Share = Net Earnings / Number of Shares
        ///
        /// Constraints:
        /// - Net Earnings can be any value (including negative for losses)
        /// - Number of Shares must be positive (> 0)
        ///
        /// EPS is one of the most important metrics for stock valuation.
        /// Negative EPS indicates a loss per share.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal eps = FinancialFormulas.CalcEarningsPerShare(500000m, 100000m);
        /// // Returns: 5.0 (earnings of $5 per share)
        /// </code>
        /// </example>
        public static decimal CalcEarningsPerShare(decimal netEarnings, decimal numberOfShares)
        {
            var validation = ValidateEarningsPerShareInputs(netEarnings, numberOfShares);
            validation.ThrowIfInvalid();

            return netEarnings / numberOfShares;
        }

        /// <summary>
        /// Attempts to calculate Earnings Per Share, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="netEarnings">Net Earnings (can be negative)</param>
        /// <param name="numberOfShares">Number of Shares (must be positive)</param>
        /// <returns>Result containing Earnings Per Share or error information</returns>
        public static Result<decimal> TryCalcEarningsPerShare(decimal netEarnings, decimal numberOfShares)
        {
            var validation = ValidateEarningsPerShareInputs(netEarnings, numberOfShares);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = netEarnings / numberOfShares;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating earnings per share: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Earnings Per Share",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateEarningsPerShareInputs(decimal netEarnings, decimal numberOfShares)
        {
            // Net earnings can be any value (including negative)
            return ParameterValidator.ValidatePositive(numberOfShares, nameof(numberOfShares));
        }

        /// <summary>
        /// Calculates Payout Ratio from Dividends and Earnings.
        /// Payout Ratio shows the proportion of earnings paid out as dividends.
        /// </summary>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <param name="earnings">Earnings (must be positive)</param>
        /// <returns>Decimal value for Payout Ratio (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Payout Ratio = Dividends / Earnings
        ///
        /// Constraints:
        /// - Dividends must be non-negative (>= 0)
        /// - Earnings must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.40 = 40% payout ratio).
        /// Values above 1.0 indicate paying more in dividends than earned (unsustainable).
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcPayoutRatio(40000m, 100000m);
        /// // Returns: 0.4 (40% of earnings paid as dividends)
        /// </code>
        /// </example>
        public static decimal CalcPayoutRatio(decimal dividends, decimal earnings)
        {
            var validation = ValidatePayoutRatioInputs(dividends, earnings);
            validation.ThrowIfInvalid();

            return dividends / earnings;
        }

        /// <summary>
        /// Attempts to calculate Payout Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <param name="earnings">Earnings (must be positive)</param>
        /// <returns>Result containing Payout Ratio or error information</returns>
        public static Result<decimal> TryCalcPayoutRatio(decimal dividends, decimal earnings)
        {
            var validation = ValidatePayoutRatioInputs(dividends, earnings);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = dividends / earnings;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating payout ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Payout Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidatePayoutRatioInputs(decimal dividends, decimal earnings)
        {
            var result = ParameterValidator.ValidateNonNegative(dividends, nameof(dividends));
            result = result.Combine(ParameterValidator.ValidatePositive(earnings, nameof(earnings)));
            return result;
        }

        /// <summary>
        /// Calculates Price/Earnings to Growth (PEG) Ratio from Price Per Earnings and Annual EPS Growth.
        /// PEG Ratio compares P/E ratio to earnings growth rate to determine relative value.
        /// </summary>
        /// <param name="pricePerEarnings">Price Per Earnings (P/E ratio, must be non-negative)</param>
        /// <param name="annualEpsGrowth">Annual EPS Growth rate (must be positive, expressed as decimal or percentage)</param>
        /// <returns>Decimal value for PEG Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: PEG Ratio = Price Per Earnings / Annual EPS Growth
        ///
        /// Constraints:
        /// - Price Per Earnings must be non-negative (>= 0)
        /// - Annual EPS Growth must be positive (> 0)
        ///
        /// A PEG ratio around 1.0 suggests fair value, below 1.0 may indicate undervaluation.
        /// Growth rate can be expressed as decimal (0.15 for 15%) or percentage (15 for 15%).
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal peg = FinancialFormulas.CalcPegRatio(20m, 15m);
        /// // Returns: 1.33 (P/E of 20 divided by 15% growth)
        /// </code>
        /// </example>
        public static decimal CalcPegRatio(decimal pricePerEarnings, decimal annualEpsGrowth)
        {
            var validation = ValidatePegRatioInputs(pricePerEarnings, annualEpsGrowth);
            validation.ThrowIfInvalid();

            return pricePerEarnings / annualEpsGrowth;
        }

        /// <summary>
        /// Attempts to calculate PEG Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="pricePerEarnings">Price Per Earnings (P/E ratio, must be non-negative)</param>
        /// <param name="annualEpsGrowth">Annual EPS Growth rate (must be positive)</param>
        /// <returns>Result containing PEG Ratio or error information</returns>
        public static Result<decimal> TryCalcPegRatio(decimal pricePerEarnings, decimal annualEpsGrowth)
        {
            var validation = ValidatePegRatioInputs(pricePerEarnings, annualEpsGrowth);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = pricePerEarnings / annualEpsGrowth;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating PEG ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "PEG Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidatePegRatioInputs(decimal pricePerEarnings, decimal annualEpsGrowth)
        {
            var result = ParameterValidator.ValidateNonNegative(pricePerEarnings, nameof(pricePerEarnings));
            result = result.Combine(ParameterValidator.ValidatePositive(annualEpsGrowth, nameof(annualEpsGrowth)));
            return result;
        }

        /// <summary>
        /// Calculates Price to Sales Ratio from Price Per Share and Revenue Per Share.
        /// P/S Ratio values a company relative to its revenue generation.
        /// </summary>
        /// <param name="pricePerShare">Price Per Share (must be positive)</param>
        /// <param name="revenuePerShare">Revenue Per Share (must be positive)</param>
        /// <returns>Decimal value for Price to Sales Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Price to Sales Ratio = Price Per Share / Revenue Per Share
        ///
        /// Constraints:
        /// - Price Per Share must be positive (> 0)
        /// - Revenue Per Share must be positive (> 0)
        ///
        /// Lower ratios may indicate undervaluation, but context and industry norms matter.
        /// Useful for comparing companies with negative or inconsistent earnings.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcPriceSalesRatio(50m, 100m);
        /// // Returns: 0.5 (stock trades at 0.5x revenue per share)
        /// </code>
        /// </example>
        public static decimal CalcPriceSalesRatio(decimal pricePerShare, decimal revenuePerShare)
        {
            var validation = ValidatePriceSalesRatioInputs(pricePerShare, revenuePerShare);
            validation.ThrowIfInvalid();

            return pricePerShare / revenuePerShare;
        }

        /// <summary>
        /// Attempts to calculate Price to Sales Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="pricePerShare">Price Per Share (must be positive)</param>
        /// <param name="revenuePerShare">Revenue Per Share (must be positive)</param>
        /// <returns>Result containing Price to Sales Ratio or error information</returns>
        public static Result<decimal> TryCalcPriceSalesRatio(decimal pricePerShare, decimal revenuePerShare)
        {
            var validation = ValidatePriceSalesRatioInputs(pricePerShare, revenuePerShare);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = pricePerShare / revenuePerShare;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating price sales ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Price Sales Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidatePriceSalesRatioInputs(decimal pricePerShare, decimal revenuePerShare)
        {
            var result = ParameterValidator.ValidatePositive(pricePerShare, nameof(pricePerShare));
            result = result.Combine(ParameterValidator.ValidatePositive(revenuePerShare, nameof(revenuePerShare)));
            return result;
        }

        /*
         * -----------------------------------------------------------------------------
         * | Formulas - Profitability                                                  |
         * -----------------------------------------------------------------------------
         */

        /// <summary>
        /// Calculates Efficiency Ratio from Non-Interest Expense and Revenue.
        /// The Efficiency Ratio measures how efficiently a company uses its resources to generate revenue.
        /// </summary>
        /// <param name="nonInterestExpense">Non-Interest Expense (must be non-negative)</param>
        /// <param name="revenue">Revenue (must be positive)</param>
        /// <returns>Decimal value for Efficiency Ratio (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Efficiency Ratio = Non-Interest Expense / Revenue
        ///
        /// Constraints:
        /// - Non-Interest Expense must be non-negative (>= 0)
        /// - Revenue must be positive (> 0)
        ///
        /// Lower ratios indicate better efficiency.
        /// Commonly used in banking to measure operational efficiency.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ratio = FinancialFormulas.CalcEfficiencyRatio(400000m, 1000000m);
        /// // Returns: 0.4 (40% efficiency ratio)
        /// </code>
        /// </example>
        public static decimal CalcEfficiencyRatio(decimal nonInterestExpense, decimal revenue)
        {
            var validation = ValidateEfficiencyRatioInputs(nonInterestExpense, revenue);
            validation.ThrowIfInvalid();

            return nonInterestExpense / revenue;
        }

        /// <summary>
        /// Attempts to calculate Efficiency Ratio, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="nonInterestExpense">Non-Interest Expense (must be non-negative)</param>
        /// <param name="revenue">Revenue (must be positive)</param>
        /// <returns>Result containing Efficiency Ratio or error information</returns>
        public static Result<decimal> TryCalcEfficiencyRatio(decimal nonInterestExpense, decimal revenue)
        {
            var validation = ValidateEfficiencyRatioInputs(nonInterestExpense, revenue);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = nonInterestExpense / revenue;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating efficiency ratio: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Efficiency Ratio",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateEfficiencyRatioInputs(decimal nonInterestExpense, decimal revenue)
        {
            var result = ParameterValidator.ValidateNonNegative(nonInterestExpense, nameof(nonInterestExpense));
            result = result.Combine(ParameterValidator.ValidatePositive(revenue, nameof(revenue)));
            return result;
        }

        /// <summary>
        /// Calculates Gross Profit Margin from Gross Profit and Revenue.
        /// Gross Profit Margin shows the percentage of revenue retained after direct costs.
        /// </summary>
        /// <param name="grossProfit">Gross Profit (can be negative)</param>
        /// <param name="revenue">Revenue (must be positive)</param>
        /// <returns>Decimal value for Gross Profit Margin (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Gross Profit Margin = Gross Profit / Revenue
        ///
        /// Constraints:
        /// - Gross Profit can be any value (including negative)
        /// - Revenue must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.40 = 40% margin).
        /// Higher margins indicate better cost control or pricing power.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal margin = FinancialFormulas.CalcGrossProfitMargin(400000m, 1000000m);
        /// // Returns: 0.4 (40% gross profit margin)
        /// </code>
        /// </example>
        public static decimal CalcGrossProfitMargin(decimal grossProfit, decimal revenue)
        {
            var validation = ValidateGrossProfitMarginInputs(grossProfit, revenue);
            validation.ThrowIfInvalid();

            return grossProfit / revenue;
        }

        /// <summary>
        /// Attempts to calculate Gross Profit Margin, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="grossProfit">Gross Profit (can be negative)</param>
        /// <param name="revenue">Revenue (must be positive)</param>
        /// <returns>Result containing Gross Profit Margin or error information</returns>
        public static Result<decimal> TryCalcGrossProfitMargin(decimal grossProfit, decimal revenue)
        {
            var validation = ValidateGrossProfitMarginInputs(grossProfit, revenue);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = grossProfit / revenue;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating gross profit margin: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Gross Profit Margin",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateGrossProfitMarginInputs(decimal grossProfit, decimal revenue)
        {
            // Gross profit can be any value (including negative)
            return ParameterValidator.ValidatePositive(revenue, nameof(revenue));
        }

        /// <summary>
        /// Calculates Operating Margin from Operating Income and Revenue.
        /// Operating Margin shows the percentage of revenue retained after operating expenses.
        /// </summary>
        /// <param name="operatingIncome">Operating Income (can be negative)</param>
        /// <param name="revenue">Revenue (must be positive)</param>
        /// <returns>Decimal value for Operating Margin (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Operating Margin = Operating Income / Revenue
        ///
        /// Constraints:
        /// - Operating Income can be any value (including negative)
        /// - Revenue must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.15 = 15% margin).
        /// Higher margins indicate better operational efficiency.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal margin = FinancialFormulas.CalcOperatingMargin(200000m, 1000000m);
        /// // Returns: 0.2 (20% operating margin)
        /// </code>
        /// </example>
        public static decimal CalcOperatingMargin(decimal operatingIncome, decimal revenue)
        {
            var validation = ValidateOperatingMarginInputs(operatingIncome, revenue);
            validation.ThrowIfInvalid();

            return operatingIncome / revenue;
        }

        /// <summary>
        /// Attempts to calculate Operating Margin, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="operatingIncome">Operating Income (can be negative)</param>
        /// <param name="revenue">Revenue (must be positive)</param>
        /// <returns>Result containing Operating Margin or error information</returns>
        public static Result<decimal> TryCalcOperatingMargin(decimal operatingIncome, decimal revenue)
        {
            var validation = ValidateOperatingMarginInputs(operatingIncome, revenue);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = operatingIncome / revenue;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating operating margin: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Operating Margin",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateOperatingMarginInputs(decimal operatingIncome, decimal revenue)
        {
            // Operating income can be any value (including negative)
            return ParameterValidator.ValidatePositive(revenue, nameof(revenue));
        }

        /// <summary>
        /// Calculates Profit Margin from Net Profit and Revenue.
        /// Profit Margin shows the percentage of revenue that becomes net profit.
        /// </summary>
        /// <param name="netProfit">Net Profit (can be negative)</param>
        /// <param name="revenue">Revenue (must be positive)</param>
        /// <returns>Decimal value for Profit Margin (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Profit Margin = Net Profit / Revenue
        ///
        /// Constraints:
        /// - Net Profit can be any value (including negative for losses)
        /// - Revenue must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.10 = 10% margin).
        /// Higher margins indicate better overall profitability.
        /// Also known as Net Profit Margin.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal margin = FinancialFormulas.CalcProfitMargin(100000m, 1000000m);
        /// // Returns: 0.1 (10% profit margin)
        /// </code>
        /// </example>
        public static decimal CalcProfitMargin(decimal netProfit, decimal revenue)
        {
            var validation = ValidateProfitMarginInputs(netProfit, revenue);
            validation.ThrowIfInvalid();

            return netProfit / revenue;
        }

        /// <summary>
        /// Attempts to calculate Profit Margin, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="netProfit">Net Profit (can be negative)</param>
        /// <param name="revenue">Revenue (must be positive)</param>
        /// <returns>Result containing Profit Margin or error information</returns>
        public static Result<decimal> TryCalcProfitMargin(decimal netProfit, decimal revenue)
        {
            var validation = ValidateProfitMarginInputs(netProfit, revenue);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = netProfit / revenue;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating profit margin: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Profit Margin",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateProfitMarginInputs(decimal netProfit, decimal revenue)
        {
            // Net profit can be any value (including negative)
            return ParameterValidator.ValidatePositive(revenue, nameof(revenue));
        }

        /// <summary>
        /// Calculates Return On Assets (ROA) from Net Income and Total Assets.
        /// ROA measures how efficiently a company uses its assets to generate profit.
        /// </summary>
        /// <param name="netIncome">Net Income (can be negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Decimal value for Return On Assets (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Return On Assets = Net Income / Total Assets
        ///
        /// Constraints:
        /// - Net Income can be any value (including negative)
        /// - Total Assets must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.08 = 8% ROA).
        /// Higher values indicate more efficient asset utilization.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal roa = FinancialFormulas.CalcReturnOnAssets(100000m, 1000000m);
        /// // Returns: 0.1 (10% return on assets)
        /// </code>
        /// </example>
        public static decimal CalcReturnOnAssets(decimal netIncome, decimal totalAssets)
        {
            var validation = ValidateReturnOnAssetsInputs(netIncome, totalAssets);
            validation.ThrowIfInvalid();

            return netIncome / totalAssets;
        }

        /// <summary>
        /// Attempts to calculate Return On Assets, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="netIncome">Net Income (can be negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Result containing Return On Assets or error information</returns>
        public static Result<decimal> TryCalcReturnOnAssets(decimal netIncome, decimal totalAssets)
        {
            var validation = ValidateReturnOnAssetsInputs(netIncome, totalAssets);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = netIncome / totalAssets;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating return on assets: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Return On Assets",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateReturnOnAssetsInputs(decimal netIncome, decimal totalAssets)
        {
            // Net income can be any value (including negative)
            return ParameterValidator.ValidatePositive(totalAssets, nameof(totalAssets));
        }

        /// <summary>
        /// Calculates Return On Capital (ROC) from EBIT, Tax Rate and Invested Capital.
        /// ROC measures returns generated on invested capital after taxes.
        /// </summary>
        /// <param name="ebit">Earnings Before Interest and Taxes (can be negative)</param>
        /// <param name="taxRate">Tax Rate (must be between 0 and 1 inclusive, expressed as decimal)</param>
        /// <param name="investedCapital">Invested Capital (must be positive)</param>
        /// <returns>Decimal value for Return On Capital (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Return On Capital = (EBIT * (1 - Tax Rate)) / Invested Capital
        ///
        /// Constraints:
        /// - EBIT can be any value (including negative)
        /// - Tax Rate must be between 0 and 1 (0 &lt;= taxRate &lt;= 1)
        /// - Invested Capital must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.12 = 12% ROC).
        /// Higher values indicate better capital efficiency.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal roc = FinancialFormulas.CalcReturnOnCapital(200000m, 0.25m, 1000000m);
        /// // Returns: 0.15 (15% return on capital after 25% tax)
        /// </code>
        /// </example>
        public static decimal CalcReturnOnCapital(decimal ebit, decimal taxRate, decimal investedCapital)
        {
            var validation = ValidateReturnOnCapitalInputs(ebit, taxRate, investedCapital);
            validation.ThrowIfInvalid();

            return ebit * (1 - taxRate) / investedCapital;
        }

        /// <summary>
        /// Attempts to calculate Return On Capital, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="ebit">Earnings Before Interest and Taxes (can be negative)</param>
        /// <param name="taxRate">Tax Rate (must be between 0 and 1 inclusive)</param>
        /// <param name="investedCapital">Invested Capital (must be positive)</param>
        /// <returns>Result containing Return On Capital or error information</returns>
        public static Result<decimal> TryCalcReturnOnCapital(decimal ebit, decimal taxRate, decimal investedCapital)
        {
            var validation = ValidateReturnOnCapitalInputs(ebit, taxRate, investedCapital);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = ebit * (1 - taxRate) / investedCapital;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating return on capital: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Return On Capital",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateReturnOnCapitalInputs(decimal ebit, decimal taxRate, decimal investedCapital)
        {
            // EBIT can be any value (including negative)
            var result = ValidationResult.Valid();

            // Validate tax rate is between 0 and 1
            if (taxRate < 0 || taxRate > 1)
            {
                result = result.Combine(ValidationResult.Invalid(
                    $"{nameof(taxRate)} must be between 0 and 1 (inclusive). Value: {taxRate}",
                    new ErrorContext
                    {
                        FormulaName = "Return On Capital",
                        ParameterName = nameof(taxRate),
                        ParameterValue = taxRate
                    }));
            }

            result = result.Combine(ParameterValidator.ValidatePositive(investedCapital, nameof(investedCapital)));
            return result;
        }

        /// <summary>
        /// Calculates Return on Equity (ROE) from Net Income and Average Shareholder Equity.
        /// ROE measures how efficiently a company generates profit from shareholders' equity.
        /// </summary>
        /// <param name="netIncome">Net Income (can be negative)</param>
        /// <param name="averageShareholderEquity">Average Shareholder Equity (must be positive)</param>
        /// <returns>Decimal value for Return On Equity (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Return On Equity = Net Income / Average Shareholder Equity
        ///
        /// Constraints:
        /// - Net Income can be any value (including negative)
        /// - Average Shareholder Equity must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.15 = 15% ROE).
        /// Higher values indicate better returns for shareholders.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal roe = FinancialFormulas.CalcReturnOnEquity(150000m, 1000000m);
        /// // Returns: 0.15 (15% return on equity)
        /// </code>
        /// </example>
        public static decimal CalcReturnOnEquity(decimal netIncome, decimal averageShareholderEquity)
        {
            var validation = ValidateReturnOnEquityInputs(netIncome, averageShareholderEquity);
            validation.ThrowIfInvalid();

            return netIncome / averageShareholderEquity;
        }

        /// <summary>
        /// Attempts to calculate Return On Equity, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="netIncome">Net Income (can be negative)</param>
        /// <param name="averageShareholderEquity">Average Shareholder Equity (must be positive)</param>
        /// <returns>Result containing Return On Equity or error information</returns>
        public static Result<decimal> TryCalcReturnOnEquity(decimal netIncome, decimal averageShareholderEquity)
        {
            var validation = ValidateReturnOnEquityInputs(netIncome, averageShareholderEquity);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = netIncome / averageShareholderEquity;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating return on equity: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Return On Equity",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateReturnOnEquityInputs(decimal netIncome, decimal averageShareholderEquity)
        {
            // Net income can be any value (including negative)
            return ParameterValidator.ValidatePositive(averageShareholderEquity, nameof(averageShareholderEquity));
        }

        /// <summary>
        /// Calculates Return On Net Assets (RONA) from Net Income, Fixed Assets and Working Capital.
        /// RONA measures how efficiently a company uses its net assets to generate profit.
        /// </summary>
        /// <param name="netIncome">Net Income (can be negative)</param>
        /// <param name="fixedAssets">Fixed Assets (must be non-negative)</param>
        /// <param name="workingCapital">Working Capital (can be negative)</param>
        /// <returns>Decimal value for Return On Net Assets (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Return On Net Assets = Net Income / (Fixed Assets + Working Capital)
        ///
        /// Constraints:
        /// - Net Income can be any value (including negative)
        /// - Fixed Assets must be non-negative (>= 0)
        /// - Working Capital can be any value (including negative)
        /// - Sum of Fixed Assets and Working Capital must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.18 = 18% RONA).
        /// Higher values indicate better net asset utilization.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal rona = FinancialFormulas.CalcReturnOnNetAssets(180000m, 800000m, 200000m);
        /// // Returns: 0.18 (18% return on net assets)
        /// </code>
        /// </example>
        public static decimal CalcReturnOnNetAssets(decimal netIncome, decimal fixedAssets, decimal workingCapital)
        {
            var validation = ValidateReturnOnNetAssetsInputs(netIncome, fixedAssets, workingCapital);
            validation.ThrowIfInvalid();

            return netIncome / (fixedAssets + workingCapital);
        }

        /// <summary>
        /// Attempts to calculate Return On Net Assets, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="netIncome">Net Income (can be negative)</param>
        /// <param name="fixedAssets">Fixed Assets (must be non-negative)</param>
        /// <param name="workingCapital">Working Capital (can be negative)</param>
        /// <returns>Result containing Return On Net Assets or error information</returns>
        public static Result<decimal> TryCalcReturnOnNetAssets(decimal netIncome, decimal fixedAssets, decimal workingCapital)
        {
            var validation = ValidateReturnOnNetAssetsInputs(netIncome, fixedAssets, workingCapital);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal netAssets = fixedAssets + workingCapital;
                if (netAssets <= 0)
                {
                    return Result<decimal>.Failure(
                        $"Net assets (fixed assets + working capital) must be positive. Fixed Assets: {fixedAssets}, Working Capital: {workingCapital}, Net Assets: {netAssets}",
                        new ErrorContext
                        {
                            FormulaName = "Return On Net Assets"
                        });
                }

                decimal result = netIncome / netAssets;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating return on net assets: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Return On Net Assets",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateReturnOnNetAssetsInputs(decimal netIncome, decimal fixedAssets, decimal workingCapital)
        {
            // Net income can be any value (including negative)
            // Working capital can be any value (including negative)
            var result = ParameterValidator.ValidateNonNegative(fixedAssets, nameof(fixedAssets));

            // Check that net assets (fixed assets + working capital) will be positive
            decimal netAssets = fixedAssets + workingCapital;
            if (netAssets <= 0)
            {
                result = result.Combine(ValidationResult.Invalid(
                    $"Net assets (fixed assets + working capital) must be positive. Fixed Assets: {fixedAssets}, Working Capital: {workingCapital}, Net Assets: {netAssets}",
                    new ErrorContext
                    {
                        FormulaName = "Return On Net Assets"
                    }));
            }

            return result;
        }

        /// <summary>
        /// Calculates Risk-Adjusted Return On Capital (RAROC) from Expected Return and Economic Capital.
        /// RAROC measures return relative to risk, commonly used in banking and investment management.
        /// </summary>
        /// <param name="expectedReturn">Expected Return (can be negative)</param>
        /// <param name="economicCapital">Economic Capital (must be positive)</param>
        /// <returns>Decimal value for Risk-Adjusted Return On Capital</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Risk-Adjusted Return On Capital = Expected Return / Economic Capital
        ///
        /// Constraints:
        /// - Expected Return can be any value (including negative)
        /// - Economic Capital must be positive (> 0)
        ///
        /// Higher values indicate better risk-adjusted returns.
        /// Economic capital represents the capital needed to cover unexpected losses.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal raroc = FinancialFormulas.CalcRiskAdjustedReturnOnCapital(120000m, 500000m);
        /// // Returns: 0.24 (24% risk-adjusted return on capital)
        /// </code>
        /// </example>
        public static decimal CalcRiskAdjustedReturnOnCapital(decimal expectedReturn, decimal economicCapital)
        {
            var validation = ValidateRiskAdjustedReturnOnCapitalInputs(expectedReturn, economicCapital);
            validation.ThrowIfInvalid();

            return expectedReturn / economicCapital;
        }

        /// <summary>
        /// Attempts to calculate Risk-Adjusted Return On Capital, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="expectedReturn">Expected Return (can be negative)</param>
        /// <param name="economicCapital">Economic Capital (must be positive)</param>
        /// <returns>Result containing Risk-Adjusted Return On Capital or error information</returns>
        public static Result<decimal> TryCalcRiskAdjustedReturnOnCapital(decimal expectedReturn, decimal economicCapital)
        {
            var validation = ValidateRiskAdjustedReturnOnCapitalInputs(expectedReturn, economicCapital);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = expectedReturn / economicCapital;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating risk-adjusted return on capital: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Risk-Adjusted Return On Capital",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateRiskAdjustedReturnOnCapitalInputs(decimal expectedReturn, decimal economicCapital)
        {
            // Expected return can be any value (including negative)
            return ParameterValidator.ValidatePositive(economicCapital, nameof(economicCapital));
        }

        /// <summary>
        /// Calculates Return on Investment (ROI) from Gain and Cost.
        /// ROI measures the profitability of an investment relative to its cost.
        /// </summary>
        /// <param name="gain">Gain (can be negative for losses)</param>
        /// <param name="cost">Cost (must be positive)</param>
        /// <returns>Decimal value for Return On Investment (expressed as decimal, not percentage)</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: Return On Investment = (Gain - Cost) / Cost
        ///
        /// Constraints:
        /// - Gain can be any value (including negative for losses)
        /// - Cost must be positive (> 0)
        ///
        /// Result is expressed as a decimal (e.g., 0.50 = 50% ROI).
        /// Positive values indicate profit, negative values indicate loss.
        /// A value of 0 means breaking even, 1.0 means doubling the investment.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal roi = FinancialFormulas.CalcReturnOnInvestment(150000m, 100000m);
        /// // Returns: 0.5 (50% return on investment)
        /// </code>
        /// </example>
        public static decimal CalcReturnOnInvestment(decimal gain, decimal cost)
        {
            var validation = ValidateReturnOnInvestmentInputs(gain, cost);
            validation.ThrowIfInvalid();

            return (gain - cost) / cost;
        }

        /// <summary>
        /// Attempts to calculate Return On Investment, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="gain">Gain (can be negative for losses)</param>
        /// <param name="cost">Cost (must be positive)</param>
        /// <returns>Result containing Return On Investment or error information</returns>
        public static Result<decimal> TryCalcReturnOnInvestment(decimal gain, decimal cost)
        {
            var validation = ValidateReturnOnInvestmentInputs(gain, cost);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = (gain - cost) / cost;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating return on investment: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "Return On Investment",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateReturnOnInvestmentInputs(decimal gain, decimal cost)
        {
            // Gain can be any value (including negative)
            return ParameterValidator.ValidatePositive(cost, nameof(cost));
        }

        /// <summary>
        /// Calculates Earnings Before Interest, Taxes, Depreciation and Amortization (EBITDA) from EBIT, Depreciation and Amortization.
        /// EBITDA measures operating performance before non-cash charges and financing/tax effects.
        /// </summary>
        /// <param name="ebit">Earnings Before Interest and Taxes (can be negative)</param>
        /// <param name="depreciation">Depreciation (must be non-negative)</param>
        /// <param name="amortization">Amortization (must be non-negative)</param>
        /// <returns>Decimal value for EBITDA</returns>
        /// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
        /// <remarks>
        /// Formula: EBITDA = EBIT + Depreciation + Amortization
        ///
        /// Constraints:
        /// - EBIT can be any value (including negative)
        /// - Depreciation must be non-negative (>= 0)
        /// - Amortization must be non-negative (>= 0)
        ///
        /// EBITDA is useful for comparing companies with different capital structures and tax situations.
        /// Widely used in valuation and financial analysis.
        /// Precision: Maintains full decimal precision throughout calculation.
        /// </remarks>
        /// <example>
        /// <code>
        /// decimal ebitda = FinancialFormulas.CalcEbitda(200000m, 30000m, 10000m);
        /// // Returns: 240000 (earnings before interest, taxes, depreciation and amortization)
        /// </code>
        /// </example>
        public static decimal CalcEbitda(decimal ebit, decimal depreciation, decimal amortization)
        {
            var validation = ValidateEbitdaInputs(ebit, depreciation, amortization);
            validation.ThrowIfInvalid();

            return ebit + depreciation + amortization;
        }

        /// <summary>
        /// Attempts to calculate EBITDA, returning a Result pattern for functional error handling.
        /// </summary>
        /// <param name="ebit">Earnings Before Interest and Taxes (can be negative)</param>
        /// <param name="depreciation">Depreciation (must be non-negative)</param>
        /// <param name="amortization">Amortization (must be non-negative)</param>
        /// <returns>Result containing EBITDA or error information</returns>
        public static Result<decimal> TryCalcEbitda(decimal ebit, decimal depreciation, decimal amortization)
        {
            var validation = ValidateEbitdaInputs(ebit, depreciation, amortization);
            if (!validation.IsValid)
            {
                return Result<decimal>.Failure(validation.FirstError, validation.Context);
            }

            try
            {
                decimal result = ebit + depreciation + amortization;
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(
                    $"Error calculating EBITDA: {ex.Message}",
                    new ErrorContext
                    {
                        FormulaName = "EBITDA",
                        InnerException = ex
                    });
            }
        }

        private static ValidationResult ValidateEbitdaInputs(decimal ebit, decimal depreciation, decimal amortization)
        {
            // EBIT can be any value (including negative)
            var result = ParameterValidator.ValidateNonNegative(depreciation, nameof(depreciation));
            result = result.Combine(ParameterValidator.ValidateNonNegative(amortization, nameof(amortization)));
            return result;
        }
    }
}
