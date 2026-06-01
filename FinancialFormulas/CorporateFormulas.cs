using System;
using System.Collections;
using srbrettle.FinancialFormulas.Core;
using srbrettle.FinancialFormulas.Mathematics;
using srbrettle.FinancialFormulas.Validation;

namespace srbrettle.FinancialFormulas
{
    /// <summary>
    /// A collection of methods for solving Corporate Finance/Accounting equations.
    /// All methods include comprehensive input validation and error handling.
    /// </summary>
    public static class CorporateFormulas
    {
        /// <summary>
        /// Calculates Asset to Sales Ratio from Total Assets and Sales Revenue.
        /// Formula: Asset to Sales Ratio = Total Assets / Sales Revenue
        /// </summary>
        /// <param name="totalAssets">Total Assets (must be non-negative)</param>
        /// <param name="salesRevenue">Sales Revenue (must be positive)</param>
        /// <returns>Decimal value for Asset to Sales Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcAssetToSalesRatio(decimal totalAssets, decimal salesRevenue)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(totalAssets, nameof(totalAssets)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(salesRevenue, nameof(salesRevenue)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(totalAssets, salesRevenue, nameof(salesRevenue)).ThrowIfInvalid();

            return totalAssets / salesRevenue;
        }

        /// <summary>
        /// Attempts to calculate Asset to Sales Ratio from Total Assets and Sales Revenue.
        /// Formula: Asset to Sales Ratio = Total Assets / Sales Revenue
        /// </summary>
        /// <param name="totalAssets">Total Assets (must be non-negative)</param>
        /// <param name="salesRevenue">Sales Revenue (must be positive)</param>
        /// <returns>Result containing Asset to Sales Ratio or error information</returns>
        public static Result<decimal> TryCalcAssetToSalesRatio(decimal totalAssets, decimal salesRevenue)
        {
            var validation = ParameterValidator.ValidateNonNegative(totalAssets, nameof(totalAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(salesRevenue, nameof(salesRevenue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(totalAssets, salesRevenue, nameof(salesRevenue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(totalAssets / salesRevenue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Asset Turnover Ratio from Sales Revenue and Total Assets.
        /// Formula: Asset Turnover Ratio = Sales Revenue / Total Assets
        /// </summary>
        /// <param name="salesRevenue">Sales Revenue (must be non-negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Decimal value for Asset Turnover Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcAssetTurnoverRatio(decimal salesRevenue, decimal totalAssets)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(salesRevenue, nameof(salesRevenue)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(totalAssets, nameof(totalAssets)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(salesRevenue, totalAssets, nameof(totalAssets)).ThrowIfInvalid();

            return salesRevenue / totalAssets;
        }

        /// <summary>
        /// Attempts to calculate Asset Turnover Ratio from Sales Revenue and Total Assets.
        /// Formula: Asset Turnover Ratio = Sales Revenue / Total Assets
        /// </summary>
        /// <param name="salesRevenue">Sales Revenue (must be non-negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Result containing Asset Turnover Ratio or error information</returns>
        public static Result<decimal> TryCalcAssetTurnoverRatio(decimal salesRevenue, decimal totalAssets)
        {
            var validation = ParameterValidator.ValidateNonNegative(salesRevenue, nameof(salesRevenue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(totalAssets, nameof(totalAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(salesRevenue, totalAssets, nameof(totalAssets));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(salesRevenue / totalAssets);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Average Collection Period from Receivables Turnover.
        /// Formula: Average Collection Period = 365 / Receivables Turnover
        /// </summary>
        /// <param name="receivablesTurnover">Receivables Turnover (must be positive)</param>
        /// <returns>Decimal value for Average Collection Period</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcAverageCollectionPeriod(decimal receivablesTurnover)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(receivablesTurnover, nameof(receivablesTurnover)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(365, receivablesTurnover, nameof(receivablesTurnover)).ThrowIfInvalid();

            return 365 / receivablesTurnover;
        }

        /// <summary>
        /// Attempts to calculate Average Collection Period from Receivables Turnover.
        /// Formula: Average Collection Period = 365 / Receivables Turnover
        /// </summary>
        /// <param name="receivablesTurnover">Receivables Turnover (must be positive)</param>
        /// <returns>Result containing Average Collection Period or error information</returns>
        public static Result<decimal> TryCalcAverageCollectionPeriod(decimal receivablesTurnover)
        {
            var validation = ParameterValidator.ValidatePositive(receivablesTurnover, nameof(receivablesTurnover));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(365, receivablesTurnover, nameof(receivablesTurnover));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(365 / receivablesTurnover);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Contribution Margin from Price per Product and Variable Cost per Product.
        /// Formula: Contribution Margin = Price per Product - Variable Cost per Product
        /// </summary>
        /// <param name="pricePerProduct">Price per Product (must be non-negative)</param>
        /// <param name="variableCostPerProduct">Variable Cost Per Product (must be non-negative)</param>
        /// <returns>Decimal value for Contribution Margin</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcContributionMargin(decimal pricePerProduct, decimal variableCostPerProduct)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(pricePerProduct, nameof(pricePerProduct)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(variableCostPerProduct, nameof(variableCostPerProduct)).ThrowIfInvalid();

            return pricePerProduct - variableCostPerProduct;
        }

        /// <summary>
        /// Attempts to calculate Contribution Margin from Price per Product and Variable Cost per Product.
        /// Formula: Contribution Margin = Price per Product - Variable Cost per Product
        /// </summary>
        /// <param name="pricePerProduct">Price per Product (must be non-negative)</param>
        /// <param name="variableCostPerProduct">Variable Cost Per Product (must be non-negative)</param>
        /// <returns>Result containing Contribution Margin or error information</returns>
        public static Result<decimal> TryCalcContributionMargin(decimal pricePerProduct, decimal variableCostPerProduct)
        {
            var validation = ParameterValidator.ValidateNonNegative(pricePerProduct, nameof(pricePerProduct));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(variableCostPerProduct, nameof(variableCostPerProduct));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(pricePerProduct - variableCostPerProduct);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Current Ratio from Current Assets and Current Liabilities.
        /// Formula: Current Ratio = Current Assets / Current Liabilities
        /// </summary>
        /// <param name="currentAssets">Current Assets (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Decimal value for Current Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcCurrentRatio(decimal currentAssets, decimal currentLiabilities)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(currentAssets, nameof(currentAssets)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(currentLiabilities, nameof(currentLiabilities)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(currentAssets, currentLiabilities, nameof(currentLiabilities)).ThrowIfInvalid();

            return currentAssets / currentLiabilities;
        }

        /// <summary>
        /// Attempts to calculate Current Ratio from Current Assets and Current Liabilities.
        /// Formula: Current Ratio = Current Assets / Current Liabilities
        /// </summary>
        /// <param name="currentAssets">Current Assets (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Result containing Current Ratio or error information</returns>
        public static Result<decimal> TryCalcCurrentRatio(decimal currentAssets, decimal currentLiabilities)
        {
            var validation = ParameterValidator.ValidateNonNegative(currentAssets, nameof(currentAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(currentLiabilities, nameof(currentLiabilities));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(currentAssets, currentLiabilities, nameof(currentLiabilities));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(currentAssets / currentLiabilities);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Days In Inventory from Inventory Turnover.
        /// Formula: Days In Inventory = 365 / Inventory Turnover
        /// </summary>
        /// <param name="inventoryTurnover">Inventory Turnover (must be positive)</param>
        /// <returns>Decimal value for Days In Inventory</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcDaysInInventory(decimal inventoryTurnover)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(inventoryTurnover, nameof(inventoryTurnover)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(365, inventoryTurnover, nameof(inventoryTurnover)).ThrowIfInvalid();

            return 365 / inventoryTurnover;
        }

        /// <summary>
        /// Attempts to calculate Days In Inventory from Inventory Turnover.
        /// Formula: Days In Inventory = 365 / Inventory Turnover
        /// </summary>
        /// <param name="inventoryTurnover">Inventory Turnover (must be positive)</param>
        /// <returns>Result containing Days In Inventory or error information</returns>
        public static Result<decimal> TryCalcDaysInInventory(decimal inventoryTurnover)
        {
            var validation = ParameterValidator.ValidatePositive(inventoryTurnover, nameof(inventoryTurnover));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(365, inventoryTurnover, nameof(inventoryTurnover));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(365 / inventoryTurnover);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Debt Coverage Ratio from Net Operating Income and Debt Service.
        /// Formula: Debt Coverage Ratio = Net Operating Income / Debt Service
        /// </summary>
        /// <param name="netOperatingIncome">Net Operating Income</param>
        /// <param name="debtService">Debt Service (must be non-zero)</param>
        /// <returns>Decimal value for Debt Coverage Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcDebtCoverageRatio(decimal netOperatingIncome, decimal debtService)
        {
            // Validate inputs
            ParameterValidator.ValidateNonZero(debtService, nameof(debtService)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(netOperatingIncome, debtService, nameof(debtService)).ThrowIfInvalid();

            return netOperatingIncome / debtService;
        }

        /// <summary>
        /// Attempts to calculate Debt Coverage Ratio from Net Operating Income and Debt Service.
        /// Formula: Debt Coverage Ratio = Net Operating Income / Debt Service
        /// </summary>
        /// <param name="netOperatingIncome">Net Operating Income</param>
        /// <param name="debtService">Debt Service (must be non-zero)</param>
        /// <returns>Result containing Debt Coverage Ratio or error information</returns>
        public static Result<decimal> TryCalcDebtCoverageRatio(decimal netOperatingIncome, decimal debtService)
        {
            var validation = ParameterValidator.ValidateNonZero(debtService, nameof(debtService));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(netOperatingIncome, debtService, nameof(debtService));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(netOperatingIncome / debtService);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Debt Ratio from Total Liabilities and Total Assets.
        /// Formula: Debt Ratio = Total Liabilities / Total Assets
        /// </summary>
        /// <param name="totalLiabilities">Total Liabilities (must be non-negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Decimal value for Debt Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcDebtRatio(decimal totalLiabilities, decimal totalAssets)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(totalLiabilities, nameof(totalLiabilities)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(totalAssets, nameof(totalAssets)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(totalLiabilities, totalAssets, nameof(totalAssets)).ThrowIfInvalid();

            return totalLiabilities / totalAssets;
        }

        /// <summary>
        /// Attempts to calculate Debt Ratio from Total Liabilities and Total Assets.
        /// Formula: Debt Ratio = Total Liabilities / Total Assets
        /// </summary>
        /// <param name="totalLiabilities">Total Liabilities (must be non-negative)</param>
        /// <param name="totalAssets">Total Assets (must be positive)</param>
        /// <returns>Result containing Debt Ratio or error information</returns>
        public static Result<decimal> TryCalcDebtRatio(decimal totalLiabilities, decimal totalAssets)
        {
            var validation = ParameterValidator.ValidateNonNegative(totalLiabilities, nameof(totalLiabilities));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(totalAssets, nameof(totalAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(totalLiabilities, totalAssets, nameof(totalAssets));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(totalLiabilities / totalAssets);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Debt to Equity Ratio from Total Liabilities and Total Equity.
        /// Formula: Debt to Equity Ratio = Total Liabilities / Total Equity
        /// </summary>
        /// <param name="totalLiabilities">Total Liabilities (must be non-negative)</param>
        /// <param name="totalEquity">Total Equity (must be positive)</param>
        /// <returns>Decimal value for Debt to Equity Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcDebtToEquityRatio(decimal totalLiabilities, decimal totalEquity)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(totalLiabilities, nameof(totalLiabilities)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(totalEquity, nameof(totalEquity)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(totalLiabilities, totalEquity, nameof(totalEquity)).ThrowIfInvalid();

            return totalLiabilities / totalEquity;
        }

        /// <summary>
        /// Attempts to calculate Debt to Equity Ratio from Total Liabilities and Total Equity.
        /// Formula: Debt to Equity Ratio = Total Liabilities / Total Equity
        /// </summary>
        /// <param name="totalLiabilities">Total Liabilities (must be non-negative)</param>
        /// <param name="totalEquity">Total Equity (must be positive)</param>
        /// <returns>Result containing Debt to Equity Ratio or error information</returns>
        public static Result<decimal> TryCalcDebtToEquityRatio(decimal totalLiabilities, decimal totalEquity)
        {
            var validation = ParameterValidator.ValidateNonNegative(totalLiabilities, nameof(totalLiabilities));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(totalEquity, nameof(totalEquity));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(totalLiabilities, totalEquity, nameof(totalEquity));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(totalLiabilities / totalEquity);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Discounted Payback Period from Initial Investment, Rate and Periodic Cash Flow.
        /// Formula: DPP = ln(1 / (1 - (Initial Investment * Rate) / Periodic Cash Flow)) / ln(1 + Rate)
        /// </summary>
        /// <param name="initialInvestment">Initial Investment (must be non-negative)</param>
        /// <param name="rate">Rate (must be non-negative and less than cash flow ratio)</param>
        /// <param name="periodicCashFlow">Periodic Cash Flow (must be positive)</param>
        /// <returns>Decimal value for Discounted Payback Period</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcDiscountedPaybackPeriod(decimal initialInvestment, decimal rate, decimal periodicCashFlow)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(initialInvestment, nameof(initialInvestment)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(rate, nameof(rate)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(periodicCashFlow, nameof(periodicCashFlow)).ThrowIfInvalid();

            // Validate domain: 1 - ((initialInvestment * rate) / periodicCashFlow) must be positive for log
            decimal innerExpression = 1 - ((initialInvestment * rate) / periodicCashFlow);
            if (innerExpression <= 0)
            {
                throw new ArgumentException($"Invalid combination: (1 - (initialInvestment * rate / periodicCashFlow)) must be positive. Current value: {innerExpression}");
            }

            decimal numeratorArg = 1 / innerExpression;
            DomainValidator.ValidateLogarithmInput(numeratorArg, nameof(numeratorArg)).ThrowIfInvalid();

            decimal denominatorArg = 1 + rate;
            DomainValidator.ValidateLogarithmInput(denominatorArg, nameof(denominatorArg)).ThrowIfInvalid();

            // Use DecimalMath instead of Math
            return DecimalMath.Log(numeratorArg) / DecimalMath.Log(denominatorArg);
        }

        /// <summary>
        /// Attempts to calculate Discounted Payback Period from Initial Investment, Rate and Periodic Cash Flow.
        /// Formula: DPP = ln(1 / (1 - (Initial Investment * Rate) / Periodic Cash Flow)) / ln(1 + Rate)
        /// </summary>
        /// <param name="initialInvestment">Initial Investment (must be non-negative)</param>
        /// <param name="rate">Rate (must be non-negative and less than cash flow ratio)</param>
        /// <param name="periodicCashFlow">Periodic Cash Flow (must be positive)</param>
        /// <returns>Result containing Discounted Payback Period or error information</returns>
        public static Result<decimal> TryCalcDiscountedPaybackPeriod(decimal initialInvestment, decimal rate, decimal periodicCashFlow)
        {
            var validation = ParameterValidator.ValidateNonNegative(initialInvestment, nameof(initialInvestment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(rate, nameof(rate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(periodicCashFlow, nameof(periodicCashFlow));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal innerExpression = 1 - ((initialInvestment * rate) / periodicCashFlow);
                if (innerExpression <= 0)
                {
                    return Result<decimal>.Failure($"Invalid combination: (1 - (initialInvestment * rate / periodicCashFlow)) must be positive. Current value: {innerExpression}");
                }

                decimal numeratorArg = 1 / innerExpression;
                validation = DomainValidator.ValidateLogarithmInput(numeratorArg, nameof(numeratorArg));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                decimal denominatorArg = 1 + rate;
                validation = DomainValidator.ValidateLogarithmInput(denominatorArg, nameof(denominatorArg));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(DecimalMath.Log(numeratorArg) / DecimalMath.Log(denominatorArg));
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Equivalent Annual Annuity from Net Present Value, Rate per Period and Number of Periods.
        /// Formula: EAA = (Rate * NPV) / (1 - (1 + Rate)^-Periods)
        /// </summary>
        /// <param name="netPresentValue">Net Present Value</param>
        /// <param name="ratePerPeriod">Rate per Period (must be non-negative)</param>
        /// <param name="numberOfPeriods">Number of Periods (must be positive)</param>
        /// <returns>Decimal value for Equivalent Annual Annuity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcEquivalentAnnualAnnuity(decimal netPresentValue, decimal ratePerPeriod, decimal numberOfPeriods)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(ratePerPeriod, nameof(ratePerPeriod)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(numberOfPeriods, nameof(numberOfPeriods)).ThrowIfInvalid();

            // Special case: zero rate
            if (ratePerPeriod == 0)
            {
                return netPresentValue / numberOfPeriods;
            }

            // Validate domain for power operation
            decimal baseValue = 1 + ratePerPeriod;
            DomainValidator.ValidatePowerInput(baseValue, -numberOfPeriods, nameof(baseValue)).ThrowIfInvalid();

            // Validate division
            decimal denominator = 1 - DecimalMath.Pow(baseValue, -numberOfPeriods);
            DomainValidator.ValidateDivision(ratePerPeriod * netPresentValue, denominator, nameof(denominator)).ThrowIfInvalid();

            // Use DecimalMath instead of Math
            return (ratePerPeriod * netPresentValue) / denominator;
        }

        /// <summary>
        /// Attempts to calculate Equivalent Annual Annuity from Net Present Value, Rate per Period and Number of Periods.
        /// Formula: EAA = (Rate * NPV) / (1 - (1 + Rate)^-Periods)
        /// </summary>
        /// <param name="netPresentValue">Net Present Value</param>
        /// <param name="ratePerPeriod">Rate per Period (must be non-negative)</param>
        /// <param name="numberOfPeriods">Number of Periods (must be positive)</param>
        /// <returns>Result containing Equivalent Annual Annuity or error information</returns>
        public static Result<decimal> TryCalcEquivalentAnnualAnnuity(decimal netPresentValue, decimal ratePerPeriod, decimal numberOfPeriods)
        {
            var validation = ParameterValidator.ValidateNonNegative(ratePerPeriod, nameof(ratePerPeriod));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(numberOfPeriods, nameof(numberOfPeriods));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                if (ratePerPeriod == 0)
                {
                    return Result<decimal>.Success(netPresentValue / numberOfPeriods);
                }

                decimal baseValue = 1 + ratePerPeriod;
                validation = DomainValidator.ValidatePowerInput(baseValue, -numberOfPeriods, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                decimal denominator = 1 - DecimalMath.Pow(baseValue, -numberOfPeriods);
                validation = DomainValidator.ValidateDivision(ratePerPeriod * netPresentValue, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success((ratePerPeriod * netPresentValue) / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Free Cash Flow to Equity from Net Income, Depreciation and Amortization, Capital Expenditures, Change in Working Capital and Net Borrowing.
        /// Formula: FCFE = Net Income + D&A - CapEx - Change in WC + Net Borrowing
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="depreciationAndAmortization">Depreciation and Amortization (must be non-negative)</param>
        /// <param name="capitalExpenditure">Capital Expenditures (must be non-negative)</param>
        /// <param name="changeInWorkingCapital">Change in Working Capital</param>
        /// <param name="netBorrowing">Net Borrowing</param>
        /// <returns>Decimal value for Free Cash Flow to Equity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcFreeCashFlowToEquity(decimal netIncome, decimal depreciationAndAmortization, decimal capitalExpenditure, decimal changeInWorkingCapital, decimal netBorrowing)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(depreciationAndAmortization, nameof(depreciationAndAmortization)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(capitalExpenditure, nameof(capitalExpenditure)).ThrowIfInvalid();

            return netIncome + depreciationAndAmortization - changeInWorkingCapital - capitalExpenditure + netBorrowing;
        }

        /// <summary>
        /// Attempts to calculate Free Cash Flow to Equity from Net Income, Depreciation and Amortization, Capital Expenditures, Change in Working Capital and Net Borrowing.
        /// Formula: FCFE = Net Income + D&A - CapEx - Change in WC + Net Borrowing
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="depreciationAndAmortization">Depreciation and Amortization (must be non-negative)</param>
        /// <param name="capitalExpenditure">Capital Expenditures (must be non-negative)</param>
        /// <param name="changeInWorkingCapital">Change in Working Capital</param>
        /// <param name="netBorrowing">Net Borrowing</param>
        /// <returns>Result containing Free Cash Flow to Equity or error information</returns>
        public static Result<decimal> TryCalcFreeCashFlowToEquity(decimal netIncome, decimal depreciationAndAmortization, decimal capitalExpenditure, decimal changeInWorkingCapital, decimal netBorrowing)
        {
            var validation = ParameterValidator.ValidateNonNegative(depreciationAndAmortization, nameof(depreciationAndAmortization));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(capitalExpenditure, nameof(capitalExpenditure));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(netIncome + depreciationAndAmortization - changeInWorkingCapital - capitalExpenditure + netBorrowing);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Free Cash Flow to Firm from EBIT, Tax rate, Depreciation and Amortization, Capital Expenditures and Change in Working Capital.
        /// Formula: FCFF = EBIT * (1 - Tax Rate) + D&A - CapEx - Change in WC
        /// </summary>
        /// <param name="ebit">EBIT (Earnings Before Interest and Taxes)</param>
        /// <param name="taxRate">Tax Rate (must be between 0 and 1)</param>
        /// <param name="depreciationAndAmortization">Depreciation and Amortization (must be non-negative)</param>
        /// <param name="capitalExpenditure">Capital Expenditures (must be non-negative)</param>
        /// <param name="changeInWorkingCapital">Change in Working Capital</param>
        /// <returns>Decimal value for Free Cash Flow to Firm</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcFreeCashFlowToFirm(decimal ebit, decimal taxRate, decimal depreciationAndAmortization, decimal capitalExpenditure, decimal changeInWorkingCapital)
        {
            // Validate inputs
            ParameterValidator.ValidatePercentage(taxRate, nameof(taxRate)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(depreciationAndAmortization, nameof(depreciationAndAmortization)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(capitalExpenditure, nameof(capitalExpenditure)).ThrowIfInvalid();

            return ebit * (1 - taxRate) + depreciationAndAmortization - capitalExpenditure - changeInWorkingCapital;
        }

        /// <summary>
        /// Attempts to calculate Free Cash Flow to Firm from EBIT, Tax rate, Depreciation and Amortization, Capital Expenditures and Change in Working Capital.
        /// Formula: FCFF = EBIT * (1 - Tax Rate) + D&A - CapEx - Change in WC
        /// </summary>
        /// <param name="ebit">EBIT (Earnings Before Interest and Taxes)</param>
        /// <param name="taxRate">Tax Rate (must be between 0 and 1)</param>
        /// <param name="depreciationAndAmortization">Depreciation and Amortization (must be non-negative)</param>
        /// <param name="capitalExpenditure">Capital Expenditures (must be non-negative)</param>
        /// <param name="changeInWorkingCapital">Change in Working Capital</param>
        /// <returns>Result containing Free Cash Flow to Firm or error information</returns>
        public static Result<decimal> TryCalcFreeCashFlowToFirm(decimal ebit, decimal taxRate, decimal depreciationAndAmortization, decimal capitalExpenditure, decimal changeInWorkingCapital)
        {
            var validation = ParameterValidator.ValidatePercentage(taxRate, nameof(taxRate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(depreciationAndAmortization, nameof(depreciationAndAmortization));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(capitalExpenditure, nameof(capitalExpenditure));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(ebit * (1 - taxRate) + depreciationAndAmortization - capitalExpenditure - changeInWorkingCapital);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Interest Coverage Ratio from EBIT and Interest Expense.
        /// Formula: Interest Coverage Ratio = EBIT / Interest Expense
        /// </summary>
        /// <param name="ebit">EBIT (Earnings Before Interest and Taxes)</param>
        /// <param name="interestExpense">Interest Expense (must be non-zero)</param>
        /// <returns>Decimal value for Interest Coverage Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcInterestCoverageRatio(decimal ebit, decimal interestExpense)
        {
            // Validate inputs
            ParameterValidator.ValidateNonZero(interestExpense, nameof(interestExpense)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(ebit, interestExpense, nameof(interestExpense)).ThrowIfInvalid();

            return ebit / interestExpense;
        }

        /// <summary>
        /// Attempts to calculate Interest Coverage Ratio from EBIT and Interest Expense.
        /// Formula: Interest Coverage Ratio = EBIT / Interest Expense
        /// </summary>
        /// <param name="ebit">EBIT (Earnings Before Interest and Taxes)</param>
        /// <param name="interestExpense">Interest Expense (must be non-zero)</param>
        /// <returns>Result containing Interest Coverage Ratio or error information</returns>
        public static Result<decimal> TryCalcInterestCoverageRatio(decimal ebit, decimal interestExpense)
        {
            var validation = ParameterValidator.ValidateNonZero(interestExpense, nameof(interestExpense));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(ebit, interestExpense, nameof(interestExpense));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(ebit / interestExpense);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Inventory Turnover Ratio from Sales and Inventory.
        /// Formula: Inventory Turnover Ratio = Sales / Inventory
        /// </summary>
        /// <param name="sales">Sales (must be non-negative)</param>
        /// <param name="inventory">Inventory (must be positive)</param>
        /// <returns>Decimal value for Inventory Turnover Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcInventoryTurnoverRatio(decimal sales, decimal inventory)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(sales, nameof(sales)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(inventory, nameof(inventory)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(sales, inventory, nameof(inventory)).ThrowIfInvalid();

            return sales / inventory;
        }

        /// <summary>
        /// Attempts to calculate Inventory Turnover Ratio from Sales and Inventory.
        /// Formula: Inventory Turnover Ratio = Sales / Inventory
        /// </summary>
        /// <param name="sales">Sales (must be non-negative)</param>
        /// <param name="inventory">Inventory (must be positive)</param>
        /// <returns>Result containing Inventory Turnover Ratio or error information</returns>
        public static Result<decimal> TryCalcInventoryTurnoverRatio(decimal sales, decimal inventory)
        {
            var validation = ParameterValidator.ValidateNonNegative(sales, nameof(sales));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(inventory, nameof(inventory));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(sales, inventory, nameof(inventory));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(sales / inventory);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Net Present Value from Initial Investment, Cash Flows and Discount Rate.
        /// Formula: NPV = -Initial Investment + Sum(Cash Flow[i] / (1 + Discount Rate)^i)
        /// </summary>
        /// <param name="initialInvestment">Initial Investment (must be non-negative)</param>
        /// <param name="cashFlows">Cash Flows (collection of decimal values, must not be null or empty)</param>
        /// <param name="discountRate">Discount Rate (must be greater than -1)</param>
        /// <returns>Decimal value for Net Present Value</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcNetPresentValue(decimal initialInvestment, ICollection cashFlows, decimal discountRate)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(initialInvestment, nameof(initialInvestment)).ThrowIfInvalid();

            if (cashFlows == null)
            {
                throw new ArgumentNullException(nameof(cashFlows), "Cash flows collection cannot be null");
            }

            if (cashFlows.Count == 0)
            {
                throw new ArgumentException("Cash flows collection cannot be empty", nameof(cashFlows));
            }

            if (discountRate <= -1)
            {
                throw new ArgumentException($"Discount rate must be greater than -1. Provided value: {discountRate}", nameof(discountRate));
            }

            decimal sumTotal = 0;
            int count = 1;
            decimal baseValue = 1 + discountRate;

            foreach (decimal cashFlowAtPeriod in cashFlows)
            {
                // Validate domain for power operation
                DomainValidator.ValidatePowerInput(baseValue, count, nameof(baseValue)).ThrowIfInvalid();

                // Use DecimalMath instead of Math
                sumTotal += cashFlowAtPeriod / DecimalMath.Pow(baseValue, count);
                count++;
            }

            return -initialInvestment + sumTotal;
        }

        /// <summary>
        /// Attempts to calculate Net Present Value from Initial Investment, Cash Flows and Discount Rate.
        /// Formula: NPV = -Initial Investment + Sum(Cash Flow[i] / (1 + Discount Rate)^i)
        /// </summary>
        /// <param name="initialInvestment">Initial Investment (must be non-negative)</param>
        /// <param name="cashFlows">Cash Flows (collection of decimal values, must not be null or empty)</param>
        /// <param name="discountRate">Discount Rate (must be greater than -1)</param>
        /// <returns>Result containing Net Present Value or error information</returns>
        public static Result<decimal> TryCalcNetPresentValue(decimal initialInvestment, ICollection cashFlows, decimal discountRate)
        {
            var validation = ParameterValidator.ValidateNonNegative(initialInvestment, nameof(initialInvestment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            if (cashFlows == null)
            {
                return Result<decimal>.Failure("Cash flows collection cannot be null");
            }

            if (cashFlows.Count == 0)
            {
                return Result<decimal>.Failure("Cash flows collection cannot be empty");
            }

            if (discountRate <= -1)
            {
                return Result<decimal>.Failure($"Discount rate must be greater than -1. Provided value: {discountRate}");
            }

            try
            {
                decimal sumTotal = 0;
                int count = 1;
                decimal baseValue = 1 + discountRate;

                foreach (decimal cashFlowAtPeriod in cashFlows)
                {
                    validation = DomainValidator.ValidatePowerInput(baseValue, count, nameof(baseValue));
                    if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                    sumTotal += cashFlowAtPeriod / DecimalMath.Pow(baseValue, count);
                    count++;
                }

                return Result<decimal>.Success(-initialInvestment + sumTotal);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Net Profit Margin from Net Income and Sales Revenue.
        /// Formula: Net Profit Margin = Net Income / Sales Revenue
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="salesRevenue">Sales Revenue (must be non-zero)</param>
        /// <returns>Decimal value for Net Profit Margin</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcNetProfitMargin(decimal netIncome, decimal salesRevenue)
        {
            // Validate inputs
            ParameterValidator.ValidateNonZero(salesRevenue, nameof(salesRevenue)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(netIncome, salesRevenue, nameof(salesRevenue)).ThrowIfInvalid();

            return netIncome / salesRevenue;
        }

        /// <summary>
        /// Attempts to calculate Net Profit Margin from Net Income and Sales Revenue.
        /// Formula: Net Profit Margin = Net Income / Sales Revenue
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="salesRevenue">Sales Revenue (must be non-zero)</param>
        /// <returns>Result containing Net Profit Margin or error information</returns>
        public static Result<decimal> TryCalcNetProfitMargin(decimal netIncome, decimal salesRevenue)
        {
            var validation = ParameterValidator.ValidateNonZero(salesRevenue, nameof(salesRevenue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(netIncome, salesRevenue, nameof(salesRevenue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(netIncome / salesRevenue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Net Working Capital from Current Assets and Current Liabilities.
        /// Formula: Net Working Capital = Current Assets - Current Liabilities
        /// </summary>
        /// <param name="currentAssets">Current Assets (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be non-negative)</param>
        /// <returns>Decimal value for Net Working Capital</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcNetWorkingCapital(decimal currentAssets, decimal currentLiabilities)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(currentAssets, nameof(currentAssets)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(currentLiabilities, nameof(currentLiabilities)).ThrowIfInvalid();

            return currentAssets - currentLiabilities;
        }

        /// <summary>
        /// Attempts to calculate Net Working Capital from Current Assets and Current Liabilities.
        /// Formula: Net Working Capital = Current Assets - Current Liabilities
        /// </summary>
        /// <param name="currentAssets">Current Assets (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be non-negative)</param>
        /// <returns>Result containing Net Working Capital or error information</returns>
        public static Result<decimal> TryCalcNetWorkingCapital(decimal currentAssets, decimal currentLiabilities)
        {
            var validation = ParameterValidator.ValidateNonNegative(currentAssets, nameof(currentAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(currentLiabilities, nameof(currentLiabilities));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(currentAssets - currentLiabilities);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Payback Period from Initial Investment and Periodic Cash Flow.
        /// Formula: Payback Period = Initial Investment / Periodic Cash Flow
        /// </summary>
        /// <param name="initialInvestment">Initial Investment (must be non-negative)</param>
        /// <param name="periodicCashFlow">Periodic Cash Flow (must be positive)</param>
        /// <returns>Decimal value for Payback Period</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcPaybackPeriod(decimal initialInvestment, decimal periodicCashFlow)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(initialInvestment, nameof(initialInvestment)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(periodicCashFlow, nameof(periodicCashFlow)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(initialInvestment, periodicCashFlow, nameof(periodicCashFlow)).ThrowIfInvalid();

            return initialInvestment / periodicCashFlow;
        }

        /// <summary>
        /// Attempts to calculate Payback Period from Initial Investment and Periodic Cash Flow.
        /// Formula: Payback Period = Initial Investment / Periodic Cash Flow
        /// </summary>
        /// <param name="initialInvestment">Initial Investment (must be non-negative)</param>
        /// <param name="periodicCashFlow">Periodic Cash Flow (must be positive)</param>
        /// <returns>Result containing Payback Period or error information</returns>
        public static Result<decimal> TryCalcPaybackPeriod(decimal initialInvestment, decimal periodicCashFlow)
        {
            var validation = ParameterValidator.ValidateNonNegative(initialInvestment, nameof(initialInvestment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(periodicCashFlow, nameof(periodicCashFlow));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(initialInvestment, periodicCashFlow, nameof(periodicCashFlow));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(initialInvestment / periodicCashFlow);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Quick Ratio from Quick Assets and Current Liabilities.
        /// Formula: Quick Ratio = Quick Assets / Current Liabilities
        /// </summary>
        /// <param name="quickAssets">Quick Assets (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Decimal value for Quick Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcQuickRatio(decimal quickAssets, decimal currentLiabilities)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(quickAssets, nameof(quickAssets)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(currentLiabilities, nameof(currentLiabilities)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(quickAssets, currentLiabilities, nameof(currentLiabilities)).ThrowIfInvalid();

            return quickAssets / currentLiabilities;
        }

        /// <summary>
        /// Attempts to calculate Quick Ratio from Quick Assets and Current Liabilities.
        /// Formula: Quick Ratio = Quick Assets / Current Liabilities
        /// </summary>
        /// <param name="quickAssets">Quick Assets (must be non-negative)</param>
        /// <param name="currentLiabilities">Current Liabilities (must be positive)</param>
        /// <returns>Result containing Quick Ratio or error information</returns>
        public static Result<decimal> TryCalcQuickRatio(decimal quickAssets, decimal currentLiabilities)
        {
            var validation = ParameterValidator.ValidateNonNegative(quickAssets, nameof(quickAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(currentLiabilities, nameof(currentLiabilities));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(quickAssets, currentLiabilities, nameof(currentLiabilities));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(quickAssets / currentLiabilities);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Receivables Turnover Ratio from Sales Revenue and Average Accounts Receivable.
        /// Formula: Receivables Turnover Ratio = Sales Revenue / Average Accounts Receivable
        /// </summary>
        /// <param name="salesRevenue">Sales Revenue (must be non-negative)</param>
        /// <param name="averageAccountsReceivable">Average Accounts Receivable (must be positive)</param>
        /// <returns>Decimal value for Receivables Turnover Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcReceivablesTurnoverRatio(decimal salesRevenue, decimal averageAccountsReceivable)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(salesRevenue, nameof(salesRevenue)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(averageAccountsReceivable, nameof(averageAccountsReceivable)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(salesRevenue, averageAccountsReceivable, nameof(averageAccountsReceivable)).ThrowIfInvalid();

            return salesRevenue / averageAccountsReceivable;
        }

        /// <summary>
        /// Attempts to calculate Receivables Turnover Ratio from Sales Revenue and Average Accounts Receivable.
        /// Formula: Receivables Turnover Ratio = Sales Revenue / Average Accounts Receivable
        /// </summary>
        /// <param name="salesRevenue">Sales Revenue (must be non-negative)</param>
        /// <param name="averageAccountsReceivable">Average Accounts Receivable (must be positive)</param>
        /// <returns>Result containing Receivables Turnover Ratio or error information</returns>
        public static Result<decimal> TryCalcReceivablesTurnoverRatio(decimal salesRevenue, decimal averageAccountsReceivable)
        {
            var validation = ParameterValidator.ValidateNonNegative(salesRevenue, nameof(salesRevenue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(averageAccountsReceivable, nameof(averageAccountsReceivable));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(salesRevenue, averageAccountsReceivable, nameof(averageAccountsReceivable));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(salesRevenue / averageAccountsReceivable);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Retention Ratio from Net Income and Dividends.
        /// Formula: Retention Ratio = (Net Income - Dividends) / Net Income
        /// </summary>
        /// <param name="netIncome">Net Income (must be non-zero)</param>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <returns>Decimal value for Retention Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcRetentionRatio(decimal netIncome, decimal dividends)
        {
            // Validate inputs
            ParameterValidator.ValidateNonZero(netIncome, nameof(netIncome)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(dividends, nameof(dividends)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(netIncome - dividends, netIncome, nameof(netIncome)).ThrowIfInvalid();

            return (netIncome - dividends) / netIncome;
        }

        /// <summary>
        /// Attempts to calculate Retention Ratio from Net Income and Dividends.
        /// Formula: Retention Ratio = (Net Income - Dividends) / Net Income
        /// </summary>
        /// <param name="netIncome">Net Income (must be non-zero)</param>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <returns>Result containing Retention Ratio or error information</returns>
        public static Result<decimal> TryCalcRetentionRatio(decimal netIncome, decimal dividends)
        {
            var validation = ParameterValidator.ValidateNonZero(netIncome, nameof(netIncome));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(dividends, nameof(dividends));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(netIncome - dividends, netIncome, nameof(netIncome));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success((netIncome - dividends) / netIncome);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Return on Assets from Net Income and Average Total Assets.
        /// Formula: Return on Assets = Net Income / Average Total Assets
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="averageTotalAssets">Average Total Assets (must be positive)</param>
        /// <returns>Decimal value for Return on Assets</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcReturnOnAssets(decimal netIncome, decimal averageTotalAssets)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(averageTotalAssets, nameof(averageTotalAssets)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(netIncome, averageTotalAssets, nameof(averageTotalAssets)).ThrowIfInvalid();

            return netIncome / averageTotalAssets;
        }

        /// <summary>
        /// Attempts to calculate Return on Assets from Net Income and Average Total Assets.
        /// Formula: Return on Assets = Net Income / Average Total Assets
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="averageTotalAssets">Average Total Assets (must be positive)</param>
        /// <returns>Result containing Return on Assets or error information</returns>
        public static Result<decimal> TryCalcReturnOnAssets(decimal netIncome, decimal averageTotalAssets)
        {
            var validation = ParameterValidator.ValidatePositive(averageTotalAssets, nameof(averageTotalAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(netIncome, averageTotalAssets, nameof(averageTotalAssets));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(netIncome / averageTotalAssets);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Return on Equity from Net Income and Average Stockholders Equity.
        /// Formula: Return on Equity = Net Income / Average Stockholders Equity
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="averageStockholdersEquity">Average Stockholders Equity (must be positive)</param>
        /// <returns>Decimal value for Return on Equity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcReturnOnEquity(decimal netIncome, decimal averageStockholdersEquity)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(averageStockholdersEquity, nameof(averageStockholdersEquity)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(netIncome, averageStockholdersEquity, nameof(averageStockholdersEquity)).ThrowIfInvalid();

            return netIncome / averageStockholdersEquity;
        }

        /// <summary>
        /// Attempts to calculate Return on Equity from Net Income and Average Stockholders Equity.
        /// Formula: Return on Equity = Net Income / Average Stockholders Equity
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="averageStockholdersEquity">Average Stockholders Equity (must be positive)</param>
        /// <returns>Result containing Return on Equity or error information</returns>
        public static Result<decimal> TryCalcReturnOnEquity(decimal netIncome, decimal averageStockholdersEquity)
        {
            var validation = ParameterValidator.ValidatePositive(averageStockholdersEquity, nameof(averageStockholdersEquity));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(netIncome, averageStockholdersEquity, nameof(averageStockholdersEquity));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(netIncome / averageStockholdersEquity);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Return on Investment from Earnings and Initial Investment.
        /// Formula: Return on Investment = (Earnings - Initial Investment) / Initial Investment
        /// </summary>
        /// <param name="earnings">Earnings</param>
        /// <param name="initialInvestment">Initial Investment (must be non-zero)</param>
        /// <returns>Decimal value for Return on Investment</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcReturnOnInvestment(decimal earnings, decimal initialInvestment)
        {
            // Validate inputs
            ParameterValidator.ValidateNonZero(initialInvestment, nameof(initialInvestment)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(earnings - initialInvestment, initialInvestment, nameof(initialInvestment)).ThrowIfInvalid();

            return (earnings - initialInvestment) / initialInvestment;
        }

        /// <summary>
        /// Attempts to calculate Return on Investment from Earnings and Initial Investment.
        /// Formula: Return on Investment = (Earnings - Initial Investment) / Initial Investment
        /// </summary>
        /// <param name="earnings">Earnings</param>
        /// <param name="initialInvestment">Initial Investment (must be non-zero)</param>
        /// <returns>Result containing Return on Investment or error information</returns>
        public static Result<decimal> TryCalcReturnOnInvestment(decimal earnings, decimal initialInvestment)
        {
            var validation = ParameterValidator.ValidateNonZero(initialInvestment, nameof(initialInvestment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(earnings - initialInvestment, initialInvestment, nameof(initialInvestment));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success((earnings - initialInvestment) / initialInvestment);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }
    }
}
