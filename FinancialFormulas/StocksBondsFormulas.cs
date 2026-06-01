using System;
using System.Collections;
using srbrettle.FinancialFormulas.Core;
using srbrettle.FinancialFormulas.Mathematics;
using srbrettle.FinancialFormulas.Validation;

namespace srbrettle.FinancialFormulas
{
    /// <summary>
    /// A collection of methods for solving Stocks-and-Bonds-focused Finance/Accounting equations.
    /// All methods include comprehensive input validation and error handling.
    /// </summary>
    public static class StocksBondsFormulas
    {
        /// <summary>
        /// Calculates Bid Ask Spread from Bid and Ask prices.
        /// Formula: Bid Ask Spread = Ask - Bid
        /// </summary>
        /// <param name="bid">Bid price (must be non-negative)</param>
        /// <param name="ask">Ask price (must be non-negative)</param>
        /// <returns>Decimal value for Bid Ask Spread</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// The bid-ask spread represents the difference between the highest price a buyer is willing to pay
        /// and the lowest price a seller is willing to accept. A wider spread often indicates lower liquidity.
        /// </remarks>
        public static decimal CalcBidAskSpread(decimal bid, decimal ask)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(bid, nameof(bid)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(ask, nameof(ask)).ThrowIfInvalid();

            return ask - bid;
        }

        /// <summary>
        /// Attempts to calculate Bid Ask Spread from Bid and Ask prices.
        /// Formula: Bid Ask Spread = Ask - Bid
        /// </summary>
        /// <param name="bid">Bid price (must be non-negative)</param>
        /// <param name="ask">Ask price (must be non-negative)</param>
        /// <returns>Result containing Bid Ask Spread or error information</returns>
        public static Result<decimal> TryCalcBidAskSpread(decimal bid, decimal ask)
        {
            var validation = ParameterValidator.ValidateNonNegative(bid, nameof(bid));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(ask, nameof(ask));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(ask - bid);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Bond Equivalent Yield from Face Value, Bond Price and Days to Maturity.
        /// Formula: BEY = ((Face Value - Bond Price) / Bond Price) * (365 / Days to Maturity)
        /// </summary>
        /// <param name="faceValue">Face Value (must be non-negative)</param>
        /// <param name="bondPrice">Bond Price (must be positive)</param>
        /// <param name="daysToMaturity">Days to Maturity (must be positive)</param>
        /// <returns>Decimal value for Bond Equivalent Yield</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Bond Equivalent Yield standardizes bond yields on an annual basis for comparison purposes.
        /// It annualizes short-term bond yields to make them comparable to longer-term securities.
        /// </remarks>
        public static decimal CalcBondEquivalentYield(decimal faceValue, decimal bondPrice, decimal daysToMaturity)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(faceValue, nameof(faceValue)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(bondPrice, nameof(bondPrice)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(daysToMaturity, nameof(daysToMaturity)).ThrowIfInvalid();

            // Validate divisions
            DomainValidator.ValidateDivision(faceValue - bondPrice, bondPrice, nameof(bondPrice)).ThrowIfInvalid();
            DomainValidator.ValidateDivision(365m, daysToMaturity, nameof(daysToMaturity)).ThrowIfInvalid();

            return ((faceValue - bondPrice) / bondPrice) * (365m / daysToMaturity);
        }

        /// <summary>
        /// Attempts to calculate Bond Equivalent Yield from Face Value, Bond Price and Days to Maturity.
        /// Formula: BEY = ((Face Value - Bond Price) / Bond Price) * (365 / Days to Maturity)
        /// </summary>
        /// <param name="faceValue">Face Value (must be non-negative)</param>
        /// <param name="bondPrice">Bond Price (must be positive)</param>
        /// <param name="daysToMaturity">Days to Maturity (must be positive)</param>
        /// <returns>Result containing Bond Equivalent Yield or error information</returns>
        public static Result<decimal> TryCalcBondEquivalentYield(decimal faceValue, decimal bondPrice, decimal daysToMaturity)
        {
            var validation = ParameterValidator.ValidateNonNegative(faceValue, nameof(faceValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(bondPrice, nameof(bondPrice));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(daysToMaturity, nameof(daysToMaturity));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(faceValue - bondPrice, bondPrice, nameof(bondPrice));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                validation = DomainValidator.ValidateDivision(365m, daysToMaturity, nameof(daysToMaturity));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(((faceValue - bondPrice) / bondPrice) * (365m / daysToMaturity));
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Book Value per Share from Total Common Stockholders Equity and Number of Common Shares.
        /// Formula: Book Value per Share = Total Common Stockholders Equity / Number of Common Shares
        /// </summary>
        /// <param name="totalCommonStockholdersEquity">Total Common Stockholders Equity</param>
        /// <param name="numberOfCommonShares">Number of Common Shares (must be positive)</param>
        /// <returns>Decimal value for Book Value per Share</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Book value per share represents the theoretical value of each share if the company were liquidated.
        /// It's calculated by dividing shareholders' equity by outstanding shares.
        /// </remarks>
        public static decimal CalcBookValuePerShare(decimal totalCommonStockholdersEquity, decimal numberOfCommonShares)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(numberOfCommonShares, nameof(numberOfCommonShares)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(totalCommonStockholdersEquity, numberOfCommonShares, nameof(numberOfCommonShares)).ThrowIfInvalid();

            return totalCommonStockholdersEquity / numberOfCommonShares;
        }

        /// <summary>
        /// Attempts to calculate Book Value per Share from Total Common Stockholders Equity and Number of Common Shares.
        /// Formula: Book Value per Share = Total Common Stockholders Equity / Number of Common Shares
        /// </summary>
        /// <param name="totalCommonStockholdersEquity">Total Common Stockholders Equity</param>
        /// <param name="numberOfCommonShares">Number of Common Shares (must be positive)</param>
        /// <returns>Result containing Book Value per Share or error information</returns>
        public static Result<decimal> TryCalcBookValuePerShare(decimal totalCommonStockholdersEquity, decimal numberOfCommonShares)
        {
            var validation = ParameterValidator.ValidatePositive(numberOfCommonShares, nameof(numberOfCommonShares));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(totalCommonStockholdersEquity, numberOfCommonShares, nameof(numberOfCommonShares));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(totalCommonStockholdersEquity / numberOfCommonShares);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Capital Asset Pricing Model from Risk-Free Rate, Beta and Return on the Market.
        /// Formula: CAPM = Risk-Free Rate + Beta * (Return on the Market - Risk-Free Rate)
        /// </summary>
        /// <param name="riskFreeRate">Risk-Free Rate (must be non-negative)</param>
        /// <param name="beta">Beta (systematic risk measure)</param>
        /// <param name="returnOnTheMarket">Return on the Market (must be non-negative)</param>
        /// <returns>Decimal value for Capital Asset Pricing Model</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// CAPM calculates the expected return on an asset based on its systematic risk (beta).
        /// Beta measures an asset's volatility relative to the market. Beta > 1 indicates higher volatility.
        /// </remarks>
        public static decimal CalcCapitalAssetPricingModel(decimal riskFreeRate, decimal beta, decimal returnOnTheMarket)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(riskFreeRate, nameof(riskFreeRate)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(returnOnTheMarket, nameof(returnOnTheMarket)).ThrowIfInvalid();

            return riskFreeRate + beta * (returnOnTheMarket - riskFreeRate);
        }

        /// <summary>
        /// Attempts to calculate Capital Asset Pricing Model from Risk-Free Rate, Beta and Return on the Market.
        /// Formula: CAPM = Risk-Free Rate + Beta * (Return on the Market - Risk-Free Rate)
        /// </summary>
        /// <param name="riskFreeRate">Risk-Free Rate (must be non-negative)</param>
        /// <param name="beta">Beta (systematic risk measure)</param>
        /// <param name="returnOnTheMarket">Return on the Market (must be non-negative)</param>
        /// <returns>Result containing Capital Asset Pricing Model or error information</returns>
        public static Result<decimal> TryCalcCapitalAssetPricingModel(decimal riskFreeRate, decimal beta, decimal returnOnTheMarket)
        {
            var validation = ParameterValidator.ValidateNonNegative(riskFreeRate, nameof(riskFreeRate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(returnOnTheMarket, nameof(returnOnTheMarket));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(riskFreeRate + beta * (returnOnTheMarket - riskFreeRate));
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Capital Gains Yield from Initial Stock Price and Ending Stock Price.
        /// Formula: Capital Gains Yield = (Ending Stock Price - Initial Stock Price) / Initial Stock Price
        /// </summary>
        /// <param name="initialStockPrice">Initial Stock Price (must be positive)</param>
        /// <param name="endingStockPrice">Ending Stock Price (must be non-negative)</param>
        /// <returns>Decimal value for Capital Gains Yield</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Capital gains yield represents the price appreciation component of total stock return.
        /// It does not include dividends. A negative value indicates a capital loss.
        /// </remarks>
        public static decimal CalcCapitalGainsYield(decimal initialStockPrice, decimal endingStockPrice)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(initialStockPrice, nameof(initialStockPrice)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(endingStockPrice, nameof(endingStockPrice)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(endingStockPrice - initialStockPrice, initialStockPrice, nameof(initialStockPrice)).ThrowIfInvalid();

            return (endingStockPrice - initialStockPrice) / initialStockPrice;
        }

        /// <summary>
        /// Attempts to calculate Capital Gains Yield from Initial Stock Price and Ending Stock Price.
        /// Formula: Capital Gains Yield = (Ending Stock Price - Initial Stock Price) / Initial Stock Price
        /// </summary>
        /// <param name="initialStockPrice">Initial Stock Price (must be positive)</param>
        /// <param name="endingStockPrice">Ending Stock Price (must be non-negative)</param>
        /// <returns>Result containing Capital Gains Yield or error information</returns>
        public static Result<decimal> TryCalcCapitalGainsYield(decimal initialStockPrice, decimal endingStockPrice)
        {
            var validation = ParameterValidator.ValidatePositive(initialStockPrice, nameof(initialStockPrice));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(endingStockPrice, nameof(endingStockPrice));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(endingStockPrice - initialStockPrice, initialStockPrice, nameof(initialStockPrice));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success((endingStockPrice - initialStockPrice) / initialStockPrice);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Current Yield from Annual Coupons and Current Bond Price.
        /// Formula: Current Yield = Annual Coupons / Current Bond Price
        /// </summary>
        /// <param name="annualCoupons">Annual Coupons (must be non-negative)</param>
        /// <param name="currentBondPrice">Current Bond Price (must be positive)</param>
        /// <returns>Decimal value for Current Yield</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Current yield measures the annual income return from a bond relative to its current market price.
        /// It does not account for capital gains or losses from price changes.
        /// </remarks>
        public static decimal CalcCurrentYield(decimal annualCoupons, decimal currentBondPrice)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(annualCoupons, nameof(annualCoupons)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(currentBondPrice, nameof(currentBondPrice)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(annualCoupons, currentBondPrice, nameof(currentBondPrice)).ThrowIfInvalid();

            return annualCoupons / currentBondPrice;
        }

        /// <summary>
        /// Attempts to calculate Current Yield from Annual Coupons and Current Bond Price.
        /// Formula: Current Yield = Annual Coupons / Current Bond Price
        /// </summary>
        /// <param name="annualCoupons">Annual Coupons (must be non-negative)</param>
        /// <param name="currentBondPrice">Current Bond Price (must be positive)</param>
        /// <returns>Result containing Current Yield or error information</returns>
        public static Result<decimal> TryCalcCurrentYield(decimal annualCoupons, decimal currentBondPrice)
        {
            var validation = ParameterValidator.ValidateNonNegative(annualCoupons, nameof(annualCoupons));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(currentBondPrice, nameof(currentBondPrice));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(annualCoupons, currentBondPrice, nameof(currentBondPrice));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(annualCoupons / currentBondPrice);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculate Diluted Earnings per Share from Net Income, Average Shares and Other Convertible Instruments.
        /// Formula: Diluted EPS = Net Income / (Average Shares + Other Convertible Instruments)
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="averageShares">Average Shares (must be positive)</param>
        /// <param name="otherConvertibleInstruments">Other Convertible Instruments (must be non-negative)</param>
        /// <returns>Decimal value for Diluted Earnings per Share</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Diluted EPS accounts for all potential shares that could be created through conversion of
        /// convertible securities, options, or warrants. It provides a more conservative earnings metric.
        /// </remarks>
        public static decimal CalcDilutedEarningsPerShare(decimal netIncome, decimal averageShares, decimal otherConvertibleInstruments)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(averageShares, nameof(averageShares)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(otherConvertibleInstruments, nameof(otherConvertibleInstruments)).ThrowIfInvalid();

            // Validate division
            decimal denominator = averageShares + otherConvertibleInstruments;
            DomainValidator.ValidateDivision(netIncome, denominator, nameof(denominator)).ThrowIfInvalid();

            return netIncome / denominator;
        }

        /// <summary>
        /// Attempts to calculate Diluted Earnings per Share from Net Income, Average Shares and Other Convertible Instruments.
        /// Formula: Diluted EPS = Net Income / (Average Shares + Other Convertible Instruments)
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="averageShares">Average Shares (must be positive)</param>
        /// <param name="otherConvertibleInstruments">Other Convertible Instruments (must be non-negative)</param>
        /// <returns>Result containing Diluted Earnings per Share or error information</returns>
        public static Result<decimal> TryCalcDilutedEarningsPerShare(decimal netIncome, decimal averageShares, decimal otherConvertibleInstruments)
        {
            var validation = ParameterValidator.ValidatePositive(averageShares, nameof(averageShares));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(otherConvertibleInstruments, nameof(otherConvertibleInstruments));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal denominator = averageShares + otherConvertibleInstruments;
                validation = DomainValidator.ValidateDivision(netIncome, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(netIncome / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Dividends Payout Ratio from Dividends and Net Income.
        /// Formula: Dividend Payout Ratio = Dividends / Net Income
        /// </summary>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <param name="netIncome">Net Income (must be non-zero)</param>
        /// <returns>Decimal value for Dividends Payout Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Dividend payout ratio shows what percentage of earnings is distributed as dividends.
        /// A high ratio may indicate limited reinvestment, while a low ratio suggests growth focus.
        /// </remarks>
        public static decimal CalcDividendPayoutRatio(decimal dividends, decimal netIncome)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(dividends, nameof(dividends)).ThrowIfInvalid();
            ParameterValidator.ValidateNonZero(netIncome, nameof(netIncome)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(dividends, netIncome, nameof(netIncome)).ThrowIfInvalid();

            return dividends / netIncome;
        }

        /// <summary>
        /// Attempts to calculate Dividends Payout Ratio from Dividends and Net Income.
        /// Formula: Dividend Payout Ratio = Dividends / Net Income
        /// </summary>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <param name="netIncome">Net Income (must be non-zero)</param>
        /// <returns>Result containing Dividends Payout Ratio or error information</returns>
        public static Result<decimal> TryCalcDividendPayoutRatio(decimal dividends, decimal netIncome)
        {
            var validation = ParameterValidator.ValidateNonNegative(dividends, nameof(dividends));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonZero(netIncome, nameof(netIncome));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(dividends, netIncome, nameof(netIncome));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(dividends / netIncome);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Dividend Yield from Dividends for the Period and Initial Price for the Period.
        /// Formula: Dividend Yield = Dividends for the Period / Initial Price for the Period
        /// </summary>
        /// <param name="dividendsForThePeriod">Dividends for the Period (must be non-negative)</param>
        /// <param name="initialPriceForThePeriod">Initial Price for the Period (must be positive)</param>
        /// <returns>Decimal value for Dividend Yield</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Dividend yield measures the annual dividend income relative to stock price.
        /// It's an important metric for income-focused investors.
        /// </remarks>
        public static decimal CalcDividendYield(decimal dividendsForThePeriod, decimal initialPriceForThePeriod)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(dividendsForThePeriod, nameof(dividendsForThePeriod)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(initialPriceForThePeriod, nameof(initialPriceForThePeriod)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(dividendsForThePeriod, initialPriceForThePeriod, nameof(initialPriceForThePeriod)).ThrowIfInvalid();

            return dividendsForThePeriod / initialPriceForThePeriod;
        }

        /// <summary>
        /// Attempts to calculate Dividend Yield from Dividends for the Period and Initial Price for the Period.
        /// Formula: Dividend Yield = Dividends for the Period / Initial Price for the Period
        /// </summary>
        /// <param name="dividendsForThePeriod">Dividends for the Period (must be non-negative)</param>
        /// <param name="initialPriceForThePeriod">Initial Price for the Period (must be positive)</param>
        /// <returns>Result containing Dividend Yield or error information</returns>
        public static Result<decimal> TryCalcDividendYield(decimal dividendsForThePeriod, decimal initialPriceForThePeriod)
        {
            var validation = ParameterValidator.ValidateNonNegative(dividendsForThePeriod, nameof(dividendsForThePeriod));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(initialPriceForThePeriod, nameof(initialPriceForThePeriod));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(dividendsForThePeriod, initialPriceForThePeriod, nameof(initialPriceForThePeriod));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(dividendsForThePeriod / initialPriceForThePeriod);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Dividends per Share from Dividends and Number of Shares.
        /// Formula: Dividends per Share = Dividends / Number of Shares
        /// </summary>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <param name="numberOfShares">Number of Shares (must be positive)</param>
        /// <returns>Decimal value for Dividends per Share</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Dividends per share shows the dividend payment allocated to each outstanding share.
        /// </remarks>
        public static decimal CalcDividendsPerShare(decimal dividends, decimal numberOfShares)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(dividends, nameof(dividends)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(numberOfShares, nameof(numberOfShares)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(dividends, numberOfShares, nameof(numberOfShares)).ThrowIfInvalid();

            return dividends / numberOfShares;
        }

        /// <summary>
        /// Attempts to calculate Dividends per Share from Dividends and Number of Shares.
        /// Formula: Dividends per Share = Dividends / Number of Shares
        /// </summary>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <param name="numberOfShares">Number of Shares (must be positive)</param>
        /// <returns>Result containing Dividends per Share or error information</returns>
        public static Result<decimal> TryCalcDividendsPerShare(decimal dividends, decimal numberOfShares)
        {
            var validation = ParameterValidator.ValidateNonNegative(dividends, nameof(dividends));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(numberOfShares, nameof(numberOfShares));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(dividends, numberOfShares, nameof(numberOfShares));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(dividends / numberOfShares);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Earnings per Share from Net Income and Weighted Average Outstanding Shares.
        /// Formula: EPS = Net Income / Weighted Average Outstanding Shares
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="weightedAverageOutstandingShares">Weighted Average Outstanding Shares (must be positive)</param>
        /// <returns>Decimal value for Earnings per Share</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Earnings per share is a key profitability metric showing the portion of company profit
        /// allocated to each outstanding share. It's widely used in valuation ratios like P/E.
        /// </remarks>
        public static decimal CalcEarningsPerShare(decimal netIncome, decimal weightedAverageOutstandingShares)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(weightedAverageOutstandingShares, nameof(weightedAverageOutstandingShares)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(netIncome, weightedAverageOutstandingShares, nameof(weightedAverageOutstandingShares)).ThrowIfInvalid();

            return netIncome / weightedAverageOutstandingShares;
        }

        /// <summary>
        /// Attempts to calculate Earnings per Share from Net Income and Weighted Average Outstanding Shares.
        /// Formula: EPS = Net Income / Weighted Average Outstanding Shares
        /// </summary>
        /// <param name="netIncome">Net Income</param>
        /// <param name="weightedAverageOutstandingShares">Weighted Average Outstanding Shares (must be positive)</param>
        /// <returns>Result containing Earnings per Share or error information</returns>
        public static Result<decimal> TryCalcEarningsPerShare(decimal netIncome, decimal weightedAverageOutstandingShares)
        {
            var validation = ParameterValidator.ValidatePositive(weightedAverageOutstandingShares, nameof(weightedAverageOutstandingShares));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(netIncome, weightedAverageOutstandingShares, nameof(weightedAverageOutstandingShares));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(netIncome / weightedAverageOutstandingShares);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Equity Multiplier from Total Assets and Stockholders Equity.
        /// Formula: Equity Multiplier = Total Assets / Stockholders Equity
        /// </summary>
        /// <param name="totalAssets">Total Assets (must be non-negative)</param>
        /// <param name="stockholdersEquity">Stockholders Equity (must be positive)</param>
        /// <returns>Decimal value for Equity Multiplier</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Equity multiplier measures financial leverage by showing how much assets are funded by equity.
        /// Higher values indicate greater use of debt financing.
        /// </remarks>
        public static decimal CalcEquityMultiplier(decimal totalAssets, decimal stockholdersEquity)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(totalAssets, nameof(totalAssets)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(stockholdersEquity, nameof(stockholdersEquity)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(totalAssets, stockholdersEquity, nameof(stockholdersEquity)).ThrowIfInvalid();

            return totalAssets / stockholdersEquity;
        }

        /// <summary>
        /// Attempts to calculate Equity Multiplier from Total Assets and Stockholders Equity.
        /// Formula: Equity Multiplier = Total Assets / Stockholders Equity
        /// </summary>
        /// <param name="totalAssets">Total Assets (must be non-negative)</param>
        /// <param name="stockholdersEquity">Stockholders Equity (must be positive)</param>
        /// <returns>Result containing Equity Multiplier or error information</returns>
        public static Result<decimal> TryCalcEquityMultiplier(decimal totalAssets, decimal stockholdersEquity)
        {
            var validation = ParameterValidator.ValidateNonNegative(totalAssets, nameof(totalAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(stockholdersEquity, nameof(stockholdersEquity));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(totalAssets, stockholdersEquity, nameof(stockholdersEquity));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(totalAssets / stockholdersEquity);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Equity Multiplier from Equity Ratio.
        /// Formula: Equity Multiplier = 1 / Equity Ratio
        /// </summary>
        /// <param name="equityRatio">Equity Ratio (must be positive)</param>
        /// <returns>Decimal value for Equity Multiplier</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Alternative calculation of equity multiplier using the equity ratio.
        /// This is the reciprocal of the equity ratio.
        /// </remarks>
        public static decimal CalcEquityMultiplier(decimal equityRatio)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(equityRatio, nameof(equityRatio)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(1m, equityRatio, nameof(equityRatio)).ThrowIfInvalid();

            return 1m / equityRatio;
        }

        /// <summary>
        /// Attempts to calculate Equity Multiplier from Equity Ratio.
        /// Formula: Equity Multiplier = 1 / Equity Ratio
        /// </summary>
        /// <param name="equityRatio">Equity Ratio (must be positive)</param>
        /// <returns>Result containing Equity Multiplier or error information</returns>
        public static Result<decimal> TryCalcEquityMultiplier(decimal equityRatio)
        {
            var validation = ParameterValidator.ValidatePositive(equityRatio, nameof(equityRatio));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(1m, equityRatio, nameof(equityRatio));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(1m / equityRatio);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Estimated Earnings from Forecasted Sales and Forecasted Expenses.
        /// Formula: Estimated Earnings = Forecasted Sales - Forecasted Expenses
        /// </summary>
        /// <param name="forecastedSales">Forecasted Sales (must be non-negative)</param>
        /// <param name="forecastedExpenses">Forecasted Expenses (must be non-negative)</param>
        /// <returns>Decimal value for Estimated Earnings</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Estimated earnings projects future profitability based on sales and expense forecasts.
        /// </remarks>
        public static decimal CalcEstimatedEarnings(decimal forecastedSales, decimal forecastedExpenses)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(forecastedSales, nameof(forecastedSales)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(forecastedExpenses, nameof(forecastedExpenses)).ThrowIfInvalid();

            return forecastedSales - forecastedExpenses;
        }

        /// <summary>
        /// Attempts to calculate Estimated Earnings from Forecasted Sales and Forecasted Expenses.
        /// Formula: Estimated Earnings = Forecasted Sales - Forecasted Expenses
        /// </summary>
        /// <param name="forecastedSales">Forecasted Sales (must be non-negative)</param>
        /// <param name="forecastedExpenses">Forecasted Expenses (must be non-negative)</param>
        /// <returns>Result containing Estimated Earnings or error information</returns>
        public static Result<decimal> TryCalcEstimatedEarnings(decimal forecastedSales, decimal forecastedExpenses)
        {
            var validation = ParameterValidator.ValidateNonNegative(forecastedSales, nameof(forecastedSales));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(forecastedExpenses, nameof(forecastedExpenses));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(forecastedSales - forecastedExpenses);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Estimated Earnings with Profit Margin from Projected Sales and Projected Net Profit Margin.
        /// Formula: Estimated Earnings = Projected Sales * Projected Net Profit Margin
        /// </summary>
        /// <param name="projectedSales">Projected Sales (must be non-negative)</param>
        /// <param name="projectedNetProfitMargin">Projected Net Profit Margin (must be non-negative)</param>
        /// <returns>Decimal value for Estimated Earnings with Profit Margin</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Alternative method to estimate earnings using projected profit margin.
        /// </remarks>
        public static decimal CalcEstimatedEarningsWithProfitMargin(decimal projectedSales, decimal projectedNetProfitMargin)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(projectedSales, nameof(projectedSales)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(projectedNetProfitMargin, nameof(projectedNetProfitMargin)).ThrowIfInvalid();

            return projectedSales * projectedNetProfitMargin;
        }

        /// <summary>
        /// Attempts to calculate Estimated Earnings with Profit Margin from Projected Sales and Projected Net Profit Margin.
        /// Formula: Estimated Earnings = Projected Sales * Projected Net Profit Margin
        /// </summary>
        /// <param name="projectedSales">Projected Sales (must be non-negative)</param>
        /// <param name="projectedNetProfitMargin">Projected Net Profit Margin (must be non-negative)</param>
        /// <returns>Result containing Estimated Earnings with Profit Margin or error information</returns>
        public static Result<decimal> TryCalcEstimatedEarningsWithProfitMargin(decimal projectedSales, decimal projectedNetProfitMargin)
        {
            var validation = ParameterValidator.ValidateNonNegative(projectedSales, nameof(projectedSales));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(projectedNetProfitMargin, nameof(projectedNetProfitMargin));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(projectedSales * projectedNetProfitMargin);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Geometric Mean Return from Rates of Return.
        /// Formula: GMR = ((1 + r1) * (1 + r2) * ... * (1 + rn))^(1/n) - 1
        /// </summary>
        /// <param name="ratesOfReturn">Collection of Rates of Return (must not be null or empty)</param>
        /// <returns>Decimal value for Geometric Mean Return</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <exception cref="ArgumentNullException">Thrown when collection is null</exception>
        /// <remarks>
        /// Geometric mean return provides the average rate of return over multiple periods,
        /// accounting for compounding effects. It's more appropriate than arithmetic mean for investment returns.
        /// </remarks>
        public static decimal CalcGeometricMeanReturn(ICollection ratesOfReturn)
        {
            // Validate collection
            if (ratesOfReturn == null)
                throw new ArgumentNullException(nameof(ratesOfReturn), "Rates of return collection cannot be null");

            if (ratesOfReturn.Count == 0)
            {
                var context = new ErrorContext
                {
                    ParameterName = nameof(ratesOfReturn),
                    ConstraintViolated = "Collection cannot be empty",
                    AdditionalInfo = "At least one rate of return is required to calculate geometric mean"
                };
                throw new ArgumentException("Rates of return collection cannot be empty", nameof(ratesOfReturn));
            }

            int numberOfPeriods = ratesOfReturn.Count;
            decimal root = 1m;

            foreach (decimal rateOfReturn in ratesOfReturn)
            {
                root *= (1m + rateOfReturn);
            }

            // Use DecimalMath.Pow instead of Math.Pow
            decimal exponent = decimal.Divide(1m, numberOfPeriods);
            return DecimalMath.Pow(root, exponent) - 1m;
        }

        /// <summary>
        /// Attempts to calculate Geometric Mean Return from Rates of Return.
        /// Formula: GMR = ((1 + r1) * (1 + r2) * ... * (1 + rn))^(1/n) - 1
        /// </summary>
        /// <param name="ratesOfReturn">Collection of Rates of Return (must not be null or empty)</param>
        /// <returns>Result containing Geometric Mean Return or error information</returns>
        public static Result<decimal> TryCalcGeometricMeanReturn(ICollection ratesOfReturn)
        {
            // Validate collection
            if (ratesOfReturn == null)
                return Result<decimal>.Failure("Rates of return collection cannot be null");

            if (ratesOfReturn.Count == 0)
            {
                var context = new ErrorContext
                {
                    ParameterName = nameof(ratesOfReturn),
                    ConstraintViolated = "Collection cannot be empty",
                    AdditionalInfo = "At least one rate of return is required to calculate geometric mean"
                };
                return Result<decimal>.Failure("Rates of return collection cannot be empty", context);
            }

            try
            {
                int numberOfPeriods = ratesOfReturn.Count;
                decimal root = 1m;

                foreach (decimal rateOfReturn in ratesOfReturn)
                {
                    root *= (1m + rateOfReturn);
                }

                decimal exponent = decimal.Divide(1m, numberOfPeriods);

                var validation = DomainValidator.ValidatePowerInput(root, exponent, nameof(root));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(DecimalMath.Pow(root, exponent) - 1m);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Holding Period Return from Percentage Period Returns.
        /// Formula: HPR = (1 + r1) * (1 + r2) * ... * (1 + rn) - 1
        /// </summary>
        /// <param name="percentagePeriodReturns">Collection of Percentage Period Returns (must not be null or empty)</param>
        /// <returns>Decimal value for Holding Period Return</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <exception cref="ArgumentNullException">Thrown when collection is null</exception>
        /// <remarks>
        /// Holding period return calculates the total return over multiple periods by compounding individual returns.
        /// </remarks>
        public static decimal CalcHoldingPeriodReturn(ICollection percentagePeriodReturns)
        {
            // Validate collection
            if (percentagePeriodReturns == null)
                throw new ArgumentNullException(nameof(percentagePeriodReturns), "Percentage period returns collection cannot be null");

            if (percentagePeriodReturns.Count == 0)
            {
                var context = new ErrorContext
                {
                    ParameterName = nameof(percentagePeriodReturns),
                    ConstraintViolated = "Collection cannot be empty",
                    AdditionalInfo = "At least one period return is required to calculate holding period return"
                };
                throw new ArgumentException("Percentage period returns collection cannot be empty", nameof(percentagePeriodReturns));
            }

            decimal a = 1m;
            foreach (decimal periodReturn in percentagePeriodReturns)
            {
                a *= (1m + periodReturn);
            }

            return a - 1m;
        }

        /// <summary>
        /// Attempts to calculate Holding Period Return from Percentage Period Returns.
        /// Formula: HPR = (1 + r1) * (1 + r2) * ... * (1 + rn) - 1
        /// </summary>
        /// <param name="percentagePeriodReturns">Collection of Percentage Period Returns (must not be null or empty)</param>
        /// <returns>Result containing Holding Period Return or error information</returns>
        public static Result<decimal> TryCalcHoldingPeriodReturn(ICollection percentagePeriodReturns)
        {
            // Validate collection
            if (percentagePeriodReturns == null)
                return Result<decimal>.Failure("Percentage period returns collection cannot be null");

            if (percentagePeriodReturns.Count == 0)
            {
                var context = new ErrorContext
                {
                    ParameterName = nameof(percentagePeriodReturns),
                    ConstraintViolated = "Collection cannot be empty",
                    AdditionalInfo = "At least one period return is required to calculate holding period return"
                };
                return Result<decimal>.Failure("Percentage period returns collection cannot be empty", context);
            }

            try
            {
                decimal a = 1m;
                foreach (decimal periodReturn in percentagePeriodReturns)
                {
                    a *= (1m + periodReturn);
                }

                return Result<decimal>.Success(a - 1m);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Holding Period Return from Periodic Rate and Number of Periods.
        /// Formula: HPR = (1 + periodic rate)^n - 1
        /// </summary>
        /// <param name="periodicRate">Periodic Rate</param>
        /// <param name="numberOfPeriods">Number of Periods (must be non-negative)</param>
        /// <returns>Decimal value for Holding Period Return</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// This overload calculates holding period return using a constant periodic rate over multiple periods.
        /// </remarks>
        public static decimal CalcHoldingPeriodReturn(decimal periodicRate, decimal numberOfPeriods)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(numberOfPeriods, nameof(numberOfPeriods)).ThrowIfInvalid();

            // Validate domain for power operation
            decimal baseValue = 1m + periodicRate;
            DomainValidator.ValidatePowerInput(baseValue, numberOfPeriods, nameof(baseValue)).ThrowIfInvalid();

            // Use DecimalMath.Pow instead of Math.Pow
            return DecimalMath.Pow(baseValue, numberOfPeriods) - 1m;
        }

        /// <summary>
        /// Attempts to calculate Holding Period Return from Periodic Rate and Number of Periods.
        /// Formula: HPR = (1 + periodic rate)^n - 1
        /// </summary>
        /// <param name="periodicRate">Periodic Rate</param>
        /// <param name="numberOfPeriods">Number of Periods (must be non-negative)</param>
        /// <returns>Result containing Holding Period Return or error information</returns>
        public static Result<decimal> TryCalcHoldingPeriodReturn(decimal periodicRate, decimal numberOfPeriods)
        {
            var validation = ParameterValidator.ValidateNonNegative(numberOfPeriods, nameof(numberOfPeriods));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal baseValue = 1m + periodicRate;
                validation = DomainValidator.ValidatePowerInput(baseValue, numberOfPeriods, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(DecimalMath.Pow(baseValue, numberOfPeriods) - 1m);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Holding Period Return from Earnings, Asset Appreciation and Initial Investment.
        /// Formula: HPR = (Earnings + Asset Appreciation) / Initial Investment
        /// </summary>
        /// <param name="earnings">Earnings (must be non-negative)</param>
        /// <param name="assetAppreciation">Asset Appreciation</param>
        /// <param name="initialInvestment">Initial Investment (must be positive)</param>
        /// <returns>Decimal value for Holding Period Return</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// This overload calculates holding period return from income and capital gains components.
        /// </remarks>
        public static decimal CalcHoldingPeriodReturn(decimal earnings, decimal assetAppreciation, decimal initialInvestment)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(earnings, nameof(earnings)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(initialInvestment, nameof(initialInvestment)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(earnings + assetAppreciation, initialInvestment, nameof(initialInvestment)).ThrowIfInvalid();

            return (earnings + assetAppreciation) / initialInvestment;
        }

        /// <summary>
        /// Attempts to calculate Holding Period Return from Earnings, Asset Appreciation and Initial Investment.
        /// Formula: HPR = (Earnings + Asset Appreciation) / Initial Investment
        /// </summary>
        /// <param name="earnings">Earnings (must be non-negative)</param>
        /// <param name="assetAppreciation">Asset Appreciation</param>
        /// <param name="initialInvestment">Initial Investment (must be positive)</param>
        /// <returns>Result containing Holding Period Return or error information</returns>
        public static Result<decimal> TryCalcHoldingPeriodReturn(decimal earnings, decimal assetAppreciation, decimal initialInvestment)
        {
            var validation = ParameterValidator.ValidateNonNegative(earnings, nameof(earnings));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(initialInvestment, nameof(initialInvestment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(earnings + assetAppreciation, initialInvestment, nameof(initialInvestment));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success((earnings + assetAppreciation) / initialInvestment);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Net Asset Value from Fund Assets, Fund Liabilities and Outstanding Shares.
        /// Formula: NAV = (Fund Assets - Fund Liabilities) / Outstanding Shares
        /// </summary>
        /// <param name="fundAssets">Fund Assets (must be non-negative)</param>
        /// <param name="fundLiabilities">Fund Liabilities (must be non-negative)</param>
        /// <param name="outstandingShares">Outstanding Shares (must be positive)</param>
        /// <returns>Decimal value for Net Asset Value</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// NAV represents the per-share value of a mutual fund or ETF.
        /// It's calculated daily based on the closing market prices of securities in the fund's portfolio.
        /// </remarks>
        public static decimal CalcNetAssetValue(decimal fundAssets, decimal fundLiabilities, decimal outstandingShares)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(fundAssets, nameof(fundAssets)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(fundLiabilities, nameof(fundLiabilities)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(outstandingShares, nameof(outstandingShares)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(fundAssets - fundLiabilities, outstandingShares, nameof(outstandingShares)).ThrowIfInvalid();

            return (fundAssets - fundLiabilities) / outstandingShares;
        }

        /// <summary>
        /// Attempts to calculate Net Asset Value from Fund Assets, Fund Liabilities and Outstanding Shares.
        /// Formula: NAV = (Fund Assets - Fund Liabilities) / Outstanding Shares
        /// </summary>
        /// <param name="fundAssets">Fund Assets (must be non-negative)</param>
        /// <param name="fundLiabilities">Fund Liabilities (must be non-negative)</param>
        /// <param name="outstandingShares">Outstanding Shares (must be positive)</param>
        /// <returns>Result containing Net Asset Value or error information</returns>
        public static Result<decimal> TryCalcNetAssetValue(decimal fundAssets, decimal fundLiabilities, decimal outstandingShares)
        {
            var validation = ParameterValidator.ValidateNonNegative(fundAssets, nameof(fundAssets));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(fundLiabilities, nameof(fundLiabilities));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(outstandingShares, nameof(outstandingShares));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(fundAssets - fundLiabilities, outstandingShares, nameof(outstandingShares));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success((fundAssets - fundLiabilities) / outstandingShares);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Preferred Stock Value from Dividend and Discount Rate.
        /// Formula: Preferred Stock Value = Dividend / Discount Rate
        /// </summary>
        /// <param name="dividend">Dividend (must be non-negative)</param>
        /// <param name="discountRate">Discount Rate (must be positive)</param>
        /// <returns>Decimal value for Preferred Stock Value</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Preferred stock value calculation assumes perpetual dividend payments.
        /// Preferred stocks typically pay fixed dividends, making them similar to perpetual bonds.
        /// </remarks>
        public static decimal CalcPreferredStockValue(decimal dividend, decimal discountRate)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(dividend, nameof(dividend)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(discountRate, nameof(discountRate)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(dividend, discountRate, nameof(discountRate)).ThrowIfInvalid();

            return dividend / discountRate;
        }

        /// <summary>
        /// Attempts to calculate Preferred Stock Value from Dividend and Discount Rate.
        /// Formula: Preferred Stock Value = Dividend / Discount Rate
        /// </summary>
        /// <param name="dividend">Dividend (must be non-negative)</param>
        /// <param name="discountRate">Discount Rate (must be positive)</param>
        /// <returns>Result containing Preferred Stock Value or error information</returns>
        public static Result<decimal> TryCalcPreferredStockValue(decimal dividend, decimal discountRate)
        {
            var validation = ParameterValidator.ValidateNonNegative(dividend, nameof(dividend));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(discountRate, nameof(discountRate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(dividend, discountRate, nameof(discountRate));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(dividend / discountRate);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Rate of Return from Dividend and Price.
        /// Formula: Rate of Return = Dividend / Price
        /// </summary>
        /// <param name="dividend">Dividend (must be non-negative)</param>
        /// <param name="price">Price (must be positive)</param>
        /// <returns>Decimal value for Rate of Return</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Simple rate of return based on dividend income relative to purchase price.
        /// </remarks>
        public static decimal CalcRateOfReturn(decimal dividend, decimal price)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(dividend, nameof(dividend)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(price, nameof(price)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(dividend, price, nameof(price)).ThrowIfInvalid();

            return dividend / price;
        }

        /// <summary>
        /// Attempts to calculate Rate of Return from Dividend and Price.
        /// Formula: Rate of Return = Dividend / Price
        /// </summary>
        /// <param name="dividend">Dividend (must be non-negative)</param>
        /// <param name="price">Price (must be positive)</param>
        /// <returns>Result containing Rate of Return or error information</returns>
        public static Result<decimal> TryCalcRateOfReturn(decimal dividend, decimal price)
        {
            var validation = ParameterValidator.ValidateNonNegative(dividend, nameof(dividend));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(price, nameof(price));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(dividend, price, nameof(price));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(dividend / price);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Price to Book Value Ratio from Market Price per Share and Book Value per Share.
        /// Formula: P/B Ratio = Market Price per Share / Book Value per Share
        /// </summary>
        /// <param name="marketPricePerShare">Market Price per Share (must be positive)</param>
        /// <param name="bookValuePerShare">Book Value per Share (must be positive)</param>
        /// <returns>Decimal value for Price to Book Value Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// P/B ratio compares market value to book value. Ratios below 1 may indicate undervaluation.
        /// </remarks>
        public static decimal CalcPriceToBookValueRatio(decimal marketPricePerShare, decimal bookValuePerShare)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(marketPricePerShare, nameof(marketPricePerShare)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(bookValuePerShare, nameof(bookValuePerShare)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(marketPricePerShare, bookValuePerShare, nameof(bookValuePerShare)).ThrowIfInvalid();

            return marketPricePerShare / bookValuePerShare;
        }

        /// <summary>
        /// Attempts to calculate Price to Book Value Ratio from Market Price per Share and Book Value per Share.
        /// Formula: P/B Ratio = Market Price per Share / Book Value per Share
        /// </summary>
        /// <param name="marketPricePerShare">Market Price per Share (must be positive)</param>
        /// <param name="bookValuePerShare">Book Value per Share (must be positive)</param>
        /// <returns>Result containing Price to Book Value Ratio or error information</returns>
        public static Result<decimal> TryCalcPriceToBookValueRatio(decimal marketPricePerShare, decimal bookValuePerShare)
        {
            var validation = ParameterValidator.ValidatePositive(marketPricePerShare, nameof(marketPricePerShare));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(bookValuePerShare, nameof(bookValuePerShare));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(marketPricePerShare, bookValuePerShare, nameof(bookValuePerShare));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(marketPricePerShare / bookValuePerShare);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Price to Earnings Ratio from Price per Share and Earnings per Share.
        /// Formula: P/E Ratio = Price per Share / Earnings per Share
        /// </summary>
        /// <param name="pricePerShare">Price per Share (must be positive)</param>
        /// <param name="earningsPerShare">Earnings per Share (must be non-zero)</param>
        /// <returns>Decimal value for Price to Earnings Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// P/E ratio is a key valuation metric showing how much investors pay per dollar of earnings.
        /// Higher ratios may indicate growth expectations or overvaluation.
        /// </remarks>
        public static decimal CalcPriceToEarningsRatio(decimal pricePerShare, decimal earningsPerShare)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(pricePerShare, nameof(pricePerShare)).ThrowIfInvalid();
            ParameterValidator.ValidateNonZero(earningsPerShare, nameof(earningsPerShare)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(pricePerShare, earningsPerShare, nameof(earningsPerShare)).ThrowIfInvalid();

            return pricePerShare / earningsPerShare;
        }

        /// <summary>
        /// Attempts to calculate Price to Earnings Ratio from Price per Share and Earnings per Share.
        /// Formula: P/E Ratio = Price per Share / Earnings per Share
        /// </summary>
        /// <param name="pricePerShare">Price per Share (must be positive)</param>
        /// <param name="earningsPerShare">Earnings per Share (must be non-zero)</param>
        /// <returns>Result containing Price to Earnings Ratio or error information</returns>
        public static Result<decimal> TryCalcPriceToEarningsRatio(decimal pricePerShare, decimal earningsPerShare)
        {
            var validation = ParameterValidator.ValidatePositive(pricePerShare, nameof(pricePerShare));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonZero(earningsPerShare, nameof(earningsPerShare));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(pricePerShare, earningsPerShare, nameof(earningsPerShare));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(pricePerShare / earningsPerShare);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Price to Sales Ratio from Price per Share and Sales per Share.
        /// Formula: P/S Ratio = Price per Share / Sales per Share
        /// </summary>
        /// <param name="pricePerShare">Price per Share (must be positive)</param>
        /// <param name="salesPerShare">Sales per Share (must be positive)</param>
        /// <returns>Decimal value for Price to Sales Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// P/S ratio compares stock price to revenue per share. Useful for valuing unprofitable companies.
        /// </remarks>
        public static decimal CalcPriceToSalesRatio(decimal pricePerShare, decimal salesPerShare)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(pricePerShare, nameof(pricePerShare)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(salesPerShare, nameof(salesPerShare)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(pricePerShare, salesPerShare, nameof(salesPerShare)).ThrowIfInvalid();

            return pricePerShare / salesPerShare;
        }

        /// <summary>
        /// Attempts to calculate Price to Sales Ratio from Price per Share and Sales per Share.
        /// Formula: P/S Ratio = Price per Share / Sales per Share
        /// </summary>
        /// <param name="pricePerShare">Price per Share (must be positive)</param>
        /// <param name="salesPerShare">Sales per Share (must be positive)</param>
        /// <returns>Result containing Price to Sales Ratio or error information</returns>
        public static Result<decimal> TryCalcPriceToSalesRatio(decimal pricePerShare, decimal salesPerShare)
        {
            var validation = ParameterValidator.ValidatePositive(pricePerShare, nameof(pricePerShare));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(salesPerShare, nameof(salesPerShare));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(pricePerShare, salesPerShare, nameof(salesPerShare));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(pricePerShare / salesPerShare);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Risk Premium from Asset or Investment Return and Risk Free Return.
        /// Formula: Risk Premium = Asset or Investment Return - Risk Free Return
        /// </summary>
        /// <param name="assetOrInvestmentReturn">Asset or Investment Return</param>
        /// <param name="riskFreeReturn">Risk-Free Return (must be non-negative)</param>
        /// <returns>Decimal value for Risk Premium</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Risk premium represents the excess return demanded for taking on additional risk versus risk-free assets.
        /// </remarks>
        public static decimal CalcRiskPremium(decimal assetOrInvestmentReturn, decimal riskFreeReturn)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(riskFreeReturn, nameof(riskFreeReturn)).ThrowIfInvalid();

            return assetOrInvestmentReturn - riskFreeReturn;
        }

        /// <summary>
        /// Attempts to calculate Risk Premium from Asset or Investment Return and Risk Free Return.
        /// Formula: Risk Premium = Asset or Investment Return - Risk Free Return
        /// </summary>
        /// <param name="assetOrInvestmentReturn">Asset or Investment Return</param>
        /// <param name="riskFreeReturn">Risk-Free Return (must be non-negative)</param>
        /// <returns>Result containing Risk Premium or error information</returns>
        public static Result<decimal> TryCalcRiskPremium(decimal assetOrInvestmentReturn, decimal riskFreeReturn)
        {
            var validation = ParameterValidator.ValidateNonNegative(riskFreeReturn, nameof(riskFreeReturn));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(assetOrInvestmentReturn - riskFreeReturn);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Stock Present Value with Constant Growth from Estimated Dividends for Next Period, Required Rate Of Return and Growth Rate.
        /// Formula: Stock PV = Estimated Dividends for Next Period / (Required Rate Of Return - Growth Rate)
        /// </summary>
        /// <param name="estimatedDividendsForNextPeriod">Estimated Dividends for Next Period (must be non-negative)</param>
        /// <param name="requiredRateOfReturn">Required Rate Of Return (must be positive)</param>
        /// <param name="growthRate">Growth Rate (must be less than required rate of return)</param>
        /// <returns>Decimal value for Stock Present Value with Constant Growth</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Gordon Growth Model calculates stock value assuming constant dividend growth.
        /// Growth rate must be less than required return for the model to be valid.
        /// </remarks>
        public static decimal CalcStockPresentValueWithConstantGrowth(decimal estimatedDividendsForNextPeriod, decimal requiredRateOfReturn, decimal growthRate)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(estimatedDividendsForNextPeriod, nameof(estimatedDividendsForNextPeriod)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(requiredRateOfReturn, nameof(requiredRateOfReturn)).ThrowIfInvalid();

            // Validate growth rate is less than required rate of return
            DomainValidator.ValidateGrowthRate(growthRate, requiredRateOfReturn).ThrowIfInvalid();

            // Validate division
            decimal denominator = requiredRateOfReturn - growthRate;
            DomainValidator.ValidateDivision(estimatedDividendsForNextPeriod, denominator, nameof(denominator)).ThrowIfInvalid();

            return estimatedDividendsForNextPeriod / denominator;
        }

        /// <summary>
        /// Attempts to calculate Stock Present Value with Constant Growth from Estimated Dividends for Next Period, Required Rate Of Return and Growth Rate.
        /// Formula: Stock PV = Estimated Dividends for Next Period / (Required Rate Of Return - Growth Rate)
        /// </summary>
        /// <param name="estimatedDividendsForNextPeriod">Estimated Dividends for Next Period (must be non-negative)</param>
        /// <param name="requiredRateOfReturn">Required Rate Of Return (must be positive)</param>
        /// <param name="growthRate">Growth Rate (must be less than required rate of return)</param>
        /// <returns>Result containing Stock Present Value with Constant Growth or error information</returns>
        public static Result<decimal> TryCalcStockPresentValueWithConstantGrowth(decimal estimatedDividendsForNextPeriod, decimal requiredRateOfReturn, decimal growthRate)
        {
            var validation = ParameterValidator.ValidateNonNegative(estimatedDividendsForNextPeriod, nameof(estimatedDividendsForNextPeriod));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(requiredRateOfReturn, nameof(requiredRateOfReturn));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = DomainValidator.ValidateGrowthRate(growthRate, requiredRateOfReturn);
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal denominator = requiredRateOfReturn - growthRate;
                validation = DomainValidator.ValidateDivision(estimatedDividendsForNextPeriod, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(estimatedDividendsForNextPeriod / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Growth Rate from Retention Rate and Return on Equity.
        /// Formula: Growth Rate = Retention Rate * Return on Equity
        /// </summary>
        /// <param name="retentionRate">Retention Rate (must be non-negative)</param>
        /// <param name="returnOnEquity">Return on Equity</param>
        /// <returns>Decimal value for Growth Rate</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Sustainable growth rate calculation based on retained earnings and profitability.
        /// </remarks>
        public static decimal CalcGrowthRate(decimal retentionRate, decimal returnOnEquity)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(retentionRate, nameof(retentionRate)).ThrowIfInvalid();

            return retentionRate * returnOnEquity;
        }

        /// <summary>
        /// Attempts to calculate Growth Rate from Retention Rate and Return on Equity.
        /// Formula: Growth Rate = Retention Rate * Return on Equity
        /// </summary>
        /// <param name="retentionRate">Retention Rate (must be non-negative)</param>
        /// <param name="returnOnEquity">Return on Equity</param>
        /// <returns>Result containing Growth Rate or error information</returns>
        public static Result<decimal> TryCalcGrowthRate(decimal retentionRate, decimal returnOnEquity)
        {
            var validation = ParameterValidator.ValidateNonNegative(retentionRate, nameof(retentionRate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(retentionRate * returnOnEquity);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Required Rate of Return from Dividend Yield and Growth Rate.
        /// Formula: Required Rate of Return = Dividend Yield + Growth Rate
        /// </summary>
        /// <param name="dividendYield">Dividend Yield (must be non-negative)</param>
        /// <param name="growthRate">Growth Rate</param>
        /// <returns>Decimal value for Required Rate of Return</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Total required return is the sum of dividend yield and expected capital appreciation.
        /// </remarks>
        public static decimal CalcRequiredRateOfReturn(decimal dividendYield, decimal growthRate)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(dividendYield, nameof(dividendYield)).ThrowIfInvalid();

            return dividendYield + growthRate;
        }

        /// <summary>
        /// Attempts to calculate Required Rate of Return from Dividend Yield and Growth Rate.
        /// Formula: Required Rate of Return = Dividend Yield + Growth Rate
        /// </summary>
        /// <param name="dividendYield">Dividend Yield (must be non-negative)</param>
        /// <param name="growthRate">Growth Rate</param>
        /// <returns>Result containing Required Rate of Return or error information</returns>
        public static Result<decimal> TryCalcRequiredRateOfReturn(decimal dividendYield, decimal growthRate)
        {
            var validation = ParameterValidator.ValidateNonNegative(dividendYield, nameof(dividendYield));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(dividendYield + growthRate);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Stock Present Value with Zero Growth from Dividends per Period and Required Rate of Return.
        /// Formula: Stock PV = Dividends per Period / Required Rate of Return
        /// </summary>
        /// <param name="dividendsPerPeriod">Dividends per Period (must be non-negative)</param>
        /// <param name="requiredRateOfReturn">Required Rate of Return (must be positive)</param>
        /// <returns>Decimal value for Stock Present Value with Zero Growth</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Zero growth model assumes perpetual constant dividend payments with no growth.
        /// Appropriate for mature companies with stable dividend policies.
        /// </remarks>
        public static decimal CalcStockPresentValueWithZeroGrowth(decimal dividendsPerPeriod, decimal requiredRateOfReturn)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(dividendsPerPeriod, nameof(dividendsPerPeriod)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(requiredRateOfReturn, nameof(requiredRateOfReturn)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(dividendsPerPeriod, requiredRateOfReturn, nameof(requiredRateOfReturn)).ThrowIfInvalid();

            return dividendsPerPeriod / requiredRateOfReturn;
        }

        /// <summary>
        /// Attempts to calculate Stock Present Value with Zero Growth from Dividends per Period and Required Rate of Return.
        /// Formula: Stock PV = Dividends per Period / Required Rate of Return
        /// </summary>
        /// <param name="dividendsPerPeriod">Dividends per Period (must be non-negative)</param>
        /// <param name="requiredRateOfReturn">Required Rate of Return (must be positive)</param>
        /// <returns>Result containing Stock Present Value with Zero Growth or error information</returns>
        public static Result<decimal> TryCalcStockPresentValueWithZeroGrowth(decimal dividendsPerPeriod, decimal requiredRateOfReturn)
        {
            var validation = ParameterValidator.ValidateNonNegative(dividendsPerPeriod, nameof(dividendsPerPeriod));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(requiredRateOfReturn, nameof(requiredRateOfReturn));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(dividendsPerPeriod, requiredRateOfReturn, nameof(requiredRateOfReturn));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(dividendsPerPeriod / requiredRateOfReturn);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Tax Equivalent Yield from Tax-Free Yield and Tax Rate.
        /// Formula: Tax Equivalent Yield = Tax-Free Yield / (1 - Tax Rate)
        /// </summary>
        /// <param name="taxFreeYield">Tax-Free Yield (must be non-negative)</param>
        /// <param name="taxRate">Tax Rate (must be between 0 and 1)</param>
        /// <returns>Decimal value for Tax Equivalent Yield</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Tax equivalent yield converts tax-free bond yields to equivalent taxable yields for comparison.
        /// Tax rate should be in decimal format (e.g., 0.25 for 25%).
        /// </remarks>
        public static decimal CalcTaxEquivalentYield(decimal taxFreeYield, decimal taxRate)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(taxFreeYield, nameof(taxFreeYield)).ThrowIfInvalid();
            ParameterValidator.ValidatePercentage(taxRate, nameof(taxRate)).ThrowIfInvalid();

            // Validate division
            decimal denominator = 1m - taxRate;
            DomainValidator.ValidateDivision(taxFreeYield, denominator, nameof(denominator)).ThrowIfInvalid();

            return taxFreeYield / denominator;
        }

        /// <summary>
        /// Attempts to calculate Tax Equivalent Yield from Tax-Free Yield and Tax Rate.
        /// Formula: Tax Equivalent Yield = Tax-Free Yield / (1 - Tax Rate)
        /// </summary>
        /// <param name="taxFreeYield">Tax-Free Yield (must be non-negative)</param>
        /// <param name="taxRate">Tax Rate (must be between 0 and 1)</param>
        /// <returns>Result containing Tax Equivalent Yield or error information</returns>
        public static Result<decimal> TryCalcTaxEquivalentYield(decimal taxFreeYield, decimal taxRate)
        {
            var validation = ParameterValidator.ValidateNonNegative(taxFreeYield, nameof(taxFreeYield));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePercentage(taxRate, nameof(taxRate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal denominator = 1m - taxRate;
                validation = DomainValidator.ValidateDivision(taxFreeYield, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(taxFreeYield / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Total Stock Return Percentage from Initial Stock Price, Ending Stock Price and Dividends.
        /// Formula: Total Stock Return % = ((Ending Stock Price - Initial Stock Price) + Dividends) / Initial Stock Price
        /// </summary>
        /// <param name="initialStockPrice">Initial Stock Price (must be positive)</param>
        /// <param name="endingStockPrice">Ending Stock Price (must be non-negative)</param>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <returns>Decimal value for Total Stock Return Percentage</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Total return includes both capital appreciation and dividend income.
        /// </remarks>
        public static decimal CalcTotalStockReturnPercentage(decimal initialStockPrice, decimal endingStockPrice, decimal dividends)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(initialStockPrice, nameof(initialStockPrice)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(endingStockPrice, nameof(endingStockPrice)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(dividends, nameof(dividends)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision((endingStockPrice - initialStockPrice) + dividends, initialStockPrice, nameof(initialStockPrice)).ThrowIfInvalid();

            return ((endingStockPrice - initialStockPrice) + dividends) / initialStockPrice;
        }

        /// <summary>
        /// Attempts to calculate Total Stock Return Percentage from Initial Stock Price, Ending Stock Price and Dividends.
        /// Formula: Total Stock Return % = ((Ending Stock Price - Initial Stock Price) + Dividends) / Initial Stock Price
        /// </summary>
        /// <param name="initialStockPrice">Initial Stock Price (must be positive)</param>
        /// <param name="endingStockPrice">Ending Stock Price (must be non-negative)</param>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <returns>Result containing Total Stock Return Percentage or error information</returns>
        public static Result<decimal> TryCalcTotalStockReturnPercentage(decimal initialStockPrice, decimal endingStockPrice, decimal dividends)
        {
            var validation = ParameterValidator.ValidatePositive(initialStockPrice, nameof(initialStockPrice));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(endingStockPrice, nameof(endingStockPrice));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(dividends, nameof(dividends));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision((endingStockPrice - initialStockPrice) + dividends, initialStockPrice, nameof(initialStockPrice));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(((endingStockPrice - initialStockPrice) + dividends) / initialStockPrice);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Total Stock Return Cash from Initial Stock Price, Ending Stock Price and Dividends.
        /// Formula: Total Stock Return Cash = (Ending Stock Price - Initial Stock Price) + Dividends
        /// </summary>
        /// <param name="initialStockPrice">Initial Stock Price (must be non-negative)</param>
        /// <param name="endingStockPrice">Ending Stock Price (must be non-negative)</param>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <returns>Decimal value for Total Stock Return Cash</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Total return in absolute dollar terms rather than percentage.
        /// </remarks>
        public static decimal CalcTotalStockReturnCash(decimal initialStockPrice, decimal endingStockPrice, decimal dividends)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(initialStockPrice, nameof(initialStockPrice)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(endingStockPrice, nameof(endingStockPrice)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(dividends, nameof(dividends)).ThrowIfInvalid();

            return (endingStockPrice - initialStockPrice) + dividends;
        }

        /// <summary>
        /// Attempts to calculate Total Stock Return Cash from Initial Stock Price, Ending Stock Price and Dividends.
        /// Formula: Total Stock Return Cash = (Ending Stock Price - Initial Stock Price) + Dividends
        /// </summary>
        /// <param name="initialStockPrice">Initial Stock Price (must be non-negative)</param>
        /// <param name="endingStockPrice">Ending Stock Price (must be non-negative)</param>
        /// <param name="dividends">Dividends (must be non-negative)</param>
        /// <returns>Result containing Total Stock Return Cash or error information</returns>
        public static Result<decimal> TryCalcTotalStockReturnCash(decimal initialStockPrice, decimal endingStockPrice, decimal dividends)
        {
            var validation = ParameterValidator.ValidateNonNegative(initialStockPrice, nameof(initialStockPrice));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(endingStockPrice, nameof(endingStockPrice));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(dividends, nameof(dividends));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success((endingStockPrice - initialStockPrice) + dividends);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Total Stock Return by Yields from Dividend Yield and Capital Gains Yield.
        /// Formula: Total Stock Return = Dividend Yield + Capital Gains Yield
        /// </summary>
        /// <param name="dividendYield">Dividend Yield (must be non-negative)</param>
        /// <param name="capitalGainsYield">Capital Gains Yield</param>
        /// <returns>Decimal value for Total Stock Return by Yields</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Total return broken down by yield components.
        /// </remarks>
        public static decimal CalcTotalStockReturnFromYields(decimal dividendYield, decimal capitalGainsYield)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(dividendYield, nameof(dividendYield)).ThrowIfInvalid();

            return dividendYield + capitalGainsYield;
        }

        /// <summary>
        /// Attempts to calculate Total Stock Return by Yields from Dividend Yield and Capital Gains Yield.
        /// Formula: Total Stock Return = Dividend Yield + Capital Gains Yield
        /// </summary>
        /// <param name="dividendYield">Dividend Yield (must be non-negative)</param>
        /// <param name="capitalGainsYield">Capital Gains Yield</param>
        /// <returns>Result containing Total Stock Return by Yields or error information</returns>
        public static Result<decimal> TryCalcTotalStockReturnFromYields(decimal dividendYield, decimal capitalGainsYield)
        {
            var validation = ParameterValidator.ValidateNonNegative(dividendYield, nameof(dividendYield));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(dividendYield + capitalGainsYield);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Approximate Yield to Maturity from Coupon/Interest Payment, Face Value, Price and Years to Maturity.
        /// Formula: Approx YTM = (Coupon + (Face Value - Price) / Years) / ((Face Value + Price) / 2)
        /// </summary>
        /// <param name="couponOrInterestPayment">Coupon/Interest Payment (must be non-negative)</param>
        /// <param name="faceValue">Face Value (must be non-negative)</param>
        /// <param name="price">Price (must be positive)</param>
        /// <param name="yearsToMaturity">Years to Maturity (must be positive)</param>
        /// <returns>Decimal value for Approximate Yield to Maturity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Approximate YTM provides a quick estimate of bond yield to maturity.
        /// For precise calculations, use iterative methods. The denominator is the average of face value and price.
        /// </remarks>
        public static decimal CalcApproxYieldToMaturity(decimal couponOrInterestPayment, decimal faceValue, decimal price, decimal yearsToMaturity)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(couponOrInterestPayment, nameof(couponOrInterestPayment)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(faceValue, nameof(faceValue)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(price, nameof(price)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(yearsToMaturity, nameof(yearsToMaturity)).ThrowIfInvalid();

            // Calculate numerator and denominator
            decimal numerator = couponOrInterestPayment + ((faceValue - price) / yearsToMaturity);
            decimal denominator = (faceValue + price) / 2m;

            // Validate division
            DomainValidator.ValidateDivision(numerator, denominator, nameof(denominator)).ThrowIfInvalid();

            return numerator / denominator;
        }

        /// <summary>
        /// Attempts to calculate Approximate Yield to Maturity from Coupon/Interest Payment, Face Value, Price and Years to Maturity.
        /// Formula: Approx YTM = (Coupon + (Face Value - Price) / Years) / ((Face Value + Price) / 2)
        /// </summary>
        /// <param name="couponOrInterestPayment">Coupon/Interest Payment (must be non-negative)</param>
        /// <param name="faceValue">Face Value (must be non-negative)</param>
        /// <param name="price">Price (must be positive)</param>
        /// <param name="yearsToMaturity">Years to Maturity (must be positive)</param>
        /// <returns>Result containing Approximate Yield to Maturity or error information</returns>
        public static Result<decimal> TryCalcApproxYieldToMaturity(decimal couponOrInterestPayment, decimal faceValue, decimal price, decimal yearsToMaturity)
        {
            var validation = ParameterValidator.ValidateNonNegative(couponOrInterestPayment, nameof(couponOrInterestPayment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(faceValue, nameof(faceValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(price, nameof(price));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(yearsToMaturity, nameof(yearsToMaturity));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal numerator = couponOrInterestPayment + ((faceValue - price) / yearsToMaturity);
                decimal denominator = (faceValue + price) / 2m;

                validation = DomainValidator.ValidateDivision(numerator, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(numerator / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Zero Coupon Bond Value from Face Value, Rate/Yield and Time to Maturity.
        /// Formula: Bond Value = Face Value / (1 + Rate)^Time
        /// </summary>
        /// <param name="faceValue">Face Value (must be non-negative)</param>
        /// <param name="rateOrYield">Rate (Yield) (must be non-negative)</param>
        /// <param name="timeToMaturity">Time to Maturity (must be non-negative)</param>
        /// <returns>Decimal value for Zero Coupon Bond Value</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Zero coupon bonds pay no periodic interest, selling at a discount and maturing at face value.
        /// The value is calculated using present value discounting.
        /// </remarks>
        public static decimal CalcZeroCouponBondValue(decimal faceValue, decimal rateOrYield, decimal timeToMaturity)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(faceValue, nameof(faceValue)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(rateOrYield, nameof(rateOrYield)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(timeToMaturity, nameof(timeToMaturity)).ThrowIfInvalid();

            // Validate domain for power operation
            decimal baseValue = 1m + rateOrYield;
            DomainValidator.ValidatePowerInput(baseValue, timeToMaturity, nameof(baseValue)).ThrowIfInvalid();

            // Use DecimalMath.Pow instead of Math.Pow
            decimal denominator = DecimalMath.Pow(baseValue, timeToMaturity);
            DomainValidator.ValidateDivision(faceValue, denominator, nameof(denominator)).ThrowIfInvalid();

            return faceValue / denominator;
        }

        /// <summary>
        /// Attempts to calculate Zero Coupon Bond Value from Face Value, Rate/Yield and Time to Maturity.
        /// Formula: Bond Value = Face Value / (1 + Rate)^Time
        /// </summary>
        /// <param name="faceValue">Face Value (must be non-negative)</param>
        /// <param name="rateOrYield">Rate (Yield) (must be non-negative)</param>
        /// <param name="timeToMaturity">Time to Maturity (must be non-negative)</param>
        /// <returns>Result containing Zero Coupon Bond Value or error information</returns>
        public static Result<decimal> TryCalcZeroCouponBondValue(decimal faceValue, decimal rateOrYield, decimal timeToMaturity)
        {
            var validation = ParameterValidator.ValidateNonNegative(faceValue, nameof(faceValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(rateOrYield, nameof(rateOrYield));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(timeToMaturity, nameof(timeToMaturity));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal baseValue = 1m + rateOrYield;
                validation = DomainValidator.ValidatePowerInput(baseValue, timeToMaturity, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                decimal denominator = DecimalMath.Pow(baseValue, timeToMaturity);
                validation = DomainValidator.ValidateDivision(faceValue, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(faceValue / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Zero Coupon Bond Yield from Face Value, Present Value and Time to Maturity.
        /// Formula: Yield = (Face Value / Present Value)^(1 / Time) - 1
        /// </summary>
        /// <param name="faceValue">Face Value (must be positive)</param>
        /// <param name="presentValue">Present Value (must be positive)</param>
        /// <param name="timeToMaturity">Time to Maturity (must be positive)</param>
        /// <returns>Decimal value for Zero Coupon Bond Yield</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <remarks>
        /// Calculates the yield (internal rate of return) for a zero coupon bond.
        /// This is a complex power operation: (FV/PV)^(1/Time) - 1
        /// </remarks>
        public static decimal CalcZeroCouponBondYield(decimal faceValue, decimal presentValue, decimal timeToMaturity)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(faceValue, nameof(faceValue)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(presentValue, nameof(presentValue)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(timeToMaturity, nameof(timeToMaturity)).ThrowIfInvalid();

            // Validate divisions
            DomainValidator.ValidateDivision(faceValue, presentValue, nameof(presentValue)).ThrowIfInvalid();
            DomainValidator.ValidateDivision(1m, timeToMaturity, nameof(timeToMaturity)).ThrowIfInvalid();

            // Calculate base and exponent for power operation
            decimal baseValue = faceValue / presentValue;
            decimal exponent = 1m / timeToMaturity;

            // Validate domain for power operation
            DomainValidator.ValidatePowerInput(baseValue, exponent, nameof(baseValue)).ThrowIfInvalid();

            // Use DecimalMath.Pow instead of Math.Pow
            return DecimalMath.Pow(baseValue, exponent) - 1m;
        }

        /// <summary>
        /// Attempts to calculate Zero Coupon Bond Yield from Face Value, Present Value and Time to Maturity.
        /// Formula: Yield = (Face Value / Present Value)^(1 / Time) - 1
        /// </summary>
        /// <param name="faceValue">Face Value (must be positive)</param>
        /// <param name="presentValue">Present Value (must be positive)</param>
        /// <param name="timeToMaturity">Time to Maturity (must be positive)</param>
        /// <returns>Result containing Zero Coupon Bond Yield or error information</returns>
        public static Result<decimal> TryCalcZeroCouponBondYield(decimal faceValue, decimal presentValue, decimal timeToMaturity)
        {
            var validation = ParameterValidator.ValidatePositive(faceValue, nameof(faceValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(presentValue, nameof(presentValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(timeToMaturity, nameof(timeToMaturity));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(faceValue, presentValue, nameof(presentValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                validation = DomainValidator.ValidateDivision(1m, timeToMaturity, nameof(timeToMaturity));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                decimal baseValue = faceValue / presentValue;
                decimal exponent = 1m / timeToMaturity;

                validation = DomainValidator.ValidatePowerInput(baseValue, exponent, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(DecimalMath.Pow(baseValue, exponent) - 1m);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }
    }
}
