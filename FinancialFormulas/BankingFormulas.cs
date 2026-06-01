using System;
using srbrettle.FinancialFormulas.Core;
using srbrettle.FinancialFormulas.Mathematics;
using srbrettle.FinancialFormulas.Validation;

namespace srbrettle.FinancialFormulas
{
    /// <summary>
    /// A collection of methods for solving Banking-focused Finance/Accounting equations.
    /// All methods include comprehensive input validation and error handling.
    /// </summary>
    public static class BankingFormulas
    {
        /// <summary>
        /// Calculates Annual Percentage Yield from Stated Annual Interest Rate and Number of Times Compounded.
        /// Formula: APY = (1 + r/n)^n - 1
        /// </summary>
        /// <param name="statedAnnualInterestRate">Stated Annual Interest Rate (must be non-negative)</param>
        /// <param name="numberOfTimesCompounded">Number of Times Compounded (must be positive)</param>
        /// <returns>Decimal value for Annual Percentage Yield</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcAnnualPercentageYield(decimal statedAnnualInterestRate, decimal numberOfTimesCompounded)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(statedAnnualInterestRate, nameof(statedAnnualInterestRate)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(numberOfTimesCompounded, nameof(numberOfTimesCompounded)).ThrowIfInvalid();

            // Validate domain for power operation
            decimal baseValue = 1 + (statedAnnualInterestRate / numberOfTimesCompounded);
            DomainValidator.ValidatePowerInput(baseValue, numberOfTimesCompounded, nameof(baseValue)).ThrowIfInvalid();

            // Use DecimalMath instead of Math
            return DecimalMath.Pow(baseValue, numberOfTimesCompounded) - 1;
        }

        /// <summary>
        /// Attempts to calculate Annual Percentage Yield from Stated Annual Interest Rate and Number of Times Compounded.
        /// Formula: APY = (1 + r/n)^n - 1
        /// </summary>
        /// <param name="statedAnnualInterestRate">Stated Annual Interest Rate (must be non-negative)</param>
        /// <param name="numberOfTimesCompounded">Number of Times Compounded (must be positive)</param>
        /// <returns>Result containing Annual Percentage Yield or error information</returns>
        public static Result<decimal> TryCalcAnnualPercentageYield(decimal statedAnnualInterestRate, decimal numberOfTimesCompounded)
        {
            var validation = ParameterValidator.ValidateNonNegative(statedAnnualInterestRate, nameof(statedAnnualInterestRate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(numberOfTimesCompounded, nameof(numberOfTimesCompounded));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal baseValue = 1 + (statedAnnualInterestRate / numberOfTimesCompounded);
                validation = DomainValidator.ValidatePowerInput(baseValue, numberOfTimesCompounded, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(DecimalMath.Pow(baseValue, numberOfTimesCompounded) - 1);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Balloon Loan Payment from Present Value, Balloon Amount, Rate Per Period and Number of Periods.
        /// Formula: Payment = (PV - BalloonAmount / (1 + r)^n) * (r / (1 - (1 + r)^-n))
        /// </summary>
        /// <param name="presentValue">Present Value (must be non-negative)</param>
        /// <param name="balloonAmount">Balloon Amount (must be non-negative)</param>
        /// <param name="ratePerPeriod">Rate Per Period (must be non-negative)</param>
        /// <param name="numberOfPeriods">Number of Periods (must be positive)</param>
        /// <returns>Decimal value for Balloon Loan Payment</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcBalloonLoanPayment(decimal presentValue, decimal balloonAmount, decimal ratePerPeriod, decimal numberOfPeriods)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(presentValue, nameof(presentValue)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(balloonAmount, nameof(balloonAmount)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(ratePerPeriod, nameof(ratePerPeriod)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(numberOfPeriods, nameof(numberOfPeriods)).ThrowIfInvalid();

            // Validate domain for power operations
            decimal baseValue = 1 + ratePerPeriod;
            DomainValidator.ValidatePowerInput(baseValue, numberOfPeriods, nameof(baseValue)).ThrowIfInvalid();
            DomainValidator.ValidatePowerInput(baseValue, -numberOfPeriods, nameof(baseValue)).ThrowIfInvalid();

            // Calculate components
            decimal pvOfPeriodicPayments = presentValue - (balloonAmount / DecimalMath.Pow(baseValue, numberOfPeriods));

            // Validate division
            decimal denominator = 1 - DecimalMath.Pow(baseValue, -numberOfPeriods);
            DomainValidator.ValidateDivision(ratePerPeriod, denominator, nameof(denominator)).ThrowIfInvalid();

            decimal annuityPaymentFactor = ratePerPeriod / denominator;
            return pvOfPeriodicPayments * annuityPaymentFactor;
        }

        /// <summary>
        /// Attempts to calculate Balloon Loan Payment from Present Value, Balloon Amount, Rate Per Period and Number of Periods.
        /// Formula: Payment = (PV - BalloonAmount / (1 + r)^n) * (r / (1 - (1 + r)^-n))
        /// </summary>
        /// <param name="presentValue">Present Value (must be non-negative)</param>
        /// <param name="balloonAmount">Balloon Amount (must be non-negative)</param>
        /// <param name="ratePerPeriod">Rate Per Period (must be non-negative)</param>
        /// <param name="numberOfPeriods">Number of Periods (must be positive)</param>
        /// <returns>Result containing Balloon Loan Payment or error information</returns>
        public static Result<decimal> TryCalcBalloonLoanPayment(decimal presentValue, decimal balloonAmount, decimal ratePerPeriod, decimal numberOfPeriods)
        {
            var validation = ParameterValidator.ValidateNonNegative(presentValue, nameof(presentValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(balloonAmount, nameof(balloonAmount));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(ratePerPeriod, nameof(ratePerPeriod));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(numberOfPeriods, nameof(numberOfPeriods));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal baseValue = 1 + ratePerPeriod;
                validation = DomainValidator.ValidatePowerInput(baseValue, numberOfPeriods, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                validation = DomainValidator.ValidatePowerInput(baseValue, -numberOfPeriods, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                decimal pvOfPeriodicPayments = presentValue - (balloonAmount / DecimalMath.Pow(baseValue, numberOfPeriods));
                decimal denominator = 1 - DecimalMath.Pow(baseValue, -numberOfPeriods);

                validation = DomainValidator.ValidateDivision(ratePerPeriod, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                decimal annuityPaymentFactor = ratePerPeriod / denominator;
                return Result<decimal>.Success(pvOfPeriodicPayments * annuityPaymentFactor);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Compound Interest from Principal, Rate per Period and Number of Periods.
        /// Formula: CI = P * ((1 + r)^n - 1)
        /// </summary>
        /// <param name="principal">Principal (must be non-negative)</param>
        /// <param name="ratePerPeriod">Rate per Period (must be non-negative)</param>
        /// <param name="numberOfPeriods">Number of Periods (must be non-negative)</param>
        /// <returns>Decimal value for Compound Interest</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcCompoundInterest(decimal principal, decimal ratePerPeriod, decimal numberOfPeriods)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(principal, nameof(principal)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(ratePerPeriod, nameof(ratePerPeriod)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(numberOfPeriods, nameof(numberOfPeriods)).ThrowIfInvalid();

            // Validate domain for power operation
            decimal baseValue = 1 + ratePerPeriod;
            DomainValidator.ValidatePowerInput(baseValue, numberOfPeriods, nameof(baseValue)).ThrowIfInvalid();

            // Use DecimalMath instead of Math
            return principal * (DecimalMath.Pow(baseValue, numberOfPeriods) - 1);
        }

        /// <summary>
        /// Attempts to calculate Compound Interest from Principal, Rate per Period and Number of Periods.
        /// Formula: CI = P * ((1 + r)^n - 1)
        /// </summary>
        /// <param name="principal">Principal (must be non-negative)</param>
        /// <param name="ratePerPeriod">Rate per Period (must be non-negative)</param>
        /// <param name="numberOfPeriods">Number of Periods (must be non-negative)</param>
        /// <returns>Result containing Compound Interest or error information</returns>
        public static Result<decimal> TryCalcCompoundInterest(decimal principal, decimal ratePerPeriod, decimal numberOfPeriods)
        {
            var validation = ParameterValidator.ValidateNonNegative(principal, nameof(principal));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(ratePerPeriod, nameof(ratePerPeriod));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(numberOfPeriods, nameof(numberOfPeriods));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal baseValue = 1 + ratePerPeriod;
                validation = DomainValidator.ValidatePowerInput(baseValue, numberOfPeriods, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(principal * (DecimalMath.Pow(baseValue, numberOfPeriods) - 1));
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Continuous Compounding from Principal, Rate and Time.
        /// Formula: A = P * e^(rt)
        /// </summary>
        /// <param name="principal">Principal (must be non-negative)</param>
        /// <param name="rate">Rate (must be non-negative)</param>
        /// <param name="time">Time (must be non-negative)</param>
        /// <returns>Decimal value for Continuous Compounding</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcContinuousCompounding(decimal principal, decimal rate, decimal time)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(principal, nameof(principal)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(rate, nameof(rate)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(time, nameof(time)).ThrowIfInvalid();

            // Use DecimalMath instead of Math
            return principal * DecimalMath.Exp(rate * time);
        }

        /// <summary>
        /// Attempts to calculate Continuous Compounding from Principal, Rate and Time.
        /// Formula: A = P * e^(rt)
        /// </summary>
        /// <param name="principal">Principal (must be non-negative)</param>
        /// <param name="rate">Rate (must be non-negative)</param>
        /// <param name="time">Time (must be non-negative)</param>
        /// <returns>Result containing Continuous Compounding or error information</returns>
        public static Result<decimal> TryCalcContinuousCompounding(decimal principal, decimal rate, decimal time)
        {
            var validation = ParameterValidator.ValidateNonNegative(principal, nameof(principal));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(rate, nameof(rate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(time, nameof(time));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(principal * DecimalMath.Exp(rate * time));
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Debt to Income Ratio from Monthly Debt Payments and Gross Monthly Income.
        /// Formula: DTI = Monthly Debt Payments / Gross Monthly Income
        /// </summary>
        /// <param name="monthlyDebtPayments">Monthly Debt Payments (must be non-negative)</param>
        /// <param name="grossMonthlyIncome">Gross Monthly Income (must be positive)</param>
        /// <returns>Decimal value for Debt to Income Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcDebtToIncomeRatio(decimal monthlyDebtPayments, decimal grossMonthlyIncome)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(monthlyDebtPayments, nameof(monthlyDebtPayments)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(grossMonthlyIncome, nameof(grossMonthlyIncome)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(monthlyDebtPayments, grossMonthlyIncome, nameof(grossMonthlyIncome)).ThrowIfInvalid();

            return monthlyDebtPayments / grossMonthlyIncome;
        }

        /// <summary>
        /// Attempts to calculate Debt to Income Ratio from Monthly Debt Payments and Gross Monthly Income.
        /// Formula: DTI = Monthly Debt Payments / Gross Monthly Income
        /// </summary>
        /// <param name="monthlyDebtPayments">Monthly Debt Payments (must be non-negative)</param>
        /// <param name="grossMonthlyIncome">Gross Monthly Income (must be positive)</param>
        /// <returns>Result containing Debt to Income Ratio or error information</returns>
        public static Result<decimal> TryCalcDebtToIncomeRatio(decimal monthlyDebtPayments, decimal grossMonthlyIncome)
        {
            var validation = ParameterValidator.ValidateNonNegative(monthlyDebtPayments, nameof(monthlyDebtPayments));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(grossMonthlyIncome, nameof(grossMonthlyIncome));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(monthlyDebtPayments, grossMonthlyIncome, nameof(grossMonthlyIncome));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(monthlyDebtPayments / grossMonthlyIncome);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Balloon Balance of Loan from Present Value, Payment, Rate Per Payment and Number Of Payments.
        /// Formula: Balance = PV * (1 + r)^n - Payment * ((1 + r)^n - 1) / r
        /// </summary>
        /// <param name="presentValue">Present Value (must be non-negative)</param>
        /// <param name="payment">Payment (must be non-negative)</param>
        /// <param name="ratePerPayment">Rate Per Payment (must be non-negative)</param>
        /// <param name="numberOfPayments">Number Of Payments (must be non-negative)</param>
        /// <returns>Decimal value for Balloon Balance of Loan</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcBalloonBalanceOfLoan(decimal presentValue, decimal payment, decimal ratePerPayment, decimal numberOfPayments)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(presentValue, nameof(presentValue)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(payment, nameof(payment)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(ratePerPayment, nameof(ratePerPayment)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(numberOfPayments, nameof(numberOfPayments)).ThrowIfInvalid();

            // Validate domain for power operation
            decimal baseValue = 1 + ratePerPayment;
            DomainValidator.ValidatePowerInput(baseValue, numberOfPayments, nameof(baseValue)).ThrowIfInvalid();

            // Validate division (ratePerPayment could be zero)
            if (ratePerPayment == 0)
            {
                // Special case: zero rate means simple subtraction
                return presentValue - payment * numberOfPayments;
            }

            DomainValidator.ValidateDivision(DecimalMath.Pow(baseValue, numberOfPayments) - 1, ratePerPayment, nameof(ratePerPayment)).ThrowIfInvalid();

            // Use DecimalMath instead of Math
            return presentValue * DecimalMath.Pow(baseValue, numberOfPayments)
                - payment * ((DecimalMath.Pow(baseValue, numberOfPayments) - 1) / ratePerPayment);
        }

        /// <summary>
        /// Attempts to calculate Balloon Balance of Loan from Present Value, Payment, Rate Per Payment and Number Of Payments.
        /// Formula: Balance = PV * (1 + r)^n - Payment * ((1 + r)^n - 1) / r
        /// </summary>
        /// <param name="presentValue">Present Value (must be non-negative)</param>
        /// <param name="payment">Payment (must be non-negative)</param>
        /// <param name="ratePerPayment">Rate Per Payment (must be non-negative)</param>
        /// <param name="numberOfPayments">Number Of Payments (must be non-negative)</param>
        /// <returns>Result containing Balloon Balance of Loan or error information</returns>
        public static Result<decimal> TryCalcBalloonBalanceOfLoan(decimal presentValue, decimal payment, decimal ratePerPayment, decimal numberOfPayments)
        {
            var validation = ParameterValidator.ValidateNonNegative(presentValue, nameof(presentValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(payment, nameof(payment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(ratePerPayment, nameof(ratePerPayment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(numberOfPayments, nameof(numberOfPayments));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal baseValue = 1 + ratePerPayment;
                validation = DomainValidator.ValidatePowerInput(baseValue, numberOfPayments, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                if (ratePerPayment == 0)
                {
                    return Result<decimal>.Success(presentValue - payment * numberOfPayments);
                }

                decimal numerator = DecimalMath.Pow(baseValue, numberOfPayments) - 1;
                validation = DomainValidator.ValidateDivision(numerator, ratePerPayment, nameof(ratePerPayment));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(
                    presentValue * DecimalMath.Pow(baseValue, numberOfPayments)
                    - payment * (numerator / ratePerPayment)
                );
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Loan Payment from Present Value, Rate per Period and Number of Periods.
        /// Formula: Payment = (r * PV) / (1 - (1 + r)^-n)
        /// </summary>
        /// <param name="presentValue">Present Value (must be non-negative)</param>
        /// <param name="ratePerPeriod">Rate per Period (must be non-negative)</param>
        /// <param name="numberOfPeriods">Number of Periods (must be positive)</param>
        /// <returns>Decimal value for Loan Payment</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcLoanPayment(decimal presentValue, decimal ratePerPeriod, decimal numberOfPeriods)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(presentValue, nameof(presentValue)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(ratePerPeriod, nameof(ratePerPeriod)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(numberOfPeriods, nameof(numberOfPeriods)).ThrowIfInvalid();

            // Special case: zero rate means simple division
            if (ratePerPeriod == 0)
            {
                return presentValue / numberOfPeriods;
            }

            // Validate domain for power operation
            decimal baseValue = 1 + ratePerPeriod;
            DomainValidator.ValidatePowerInput(baseValue, -numberOfPeriods, nameof(baseValue)).ThrowIfInvalid();

            // Validate division
            decimal denominator = 1 - DecimalMath.Pow(baseValue, -numberOfPeriods);
            DomainValidator.ValidateDivision(ratePerPeriod * presentValue, denominator, nameof(denominator)).ThrowIfInvalid();

            // Use DecimalMath instead of Math
            return (ratePerPeriod * presentValue) / denominator;
        }

        /// <summary>
        /// Attempts to calculate Loan Payment from Present Value, Rate per Period and Number of Periods.
        /// Formula: Payment = (r * PV) / (1 - (1 + r)^-n)
        /// </summary>
        /// <param name="presentValue">Present Value (must be non-negative)</param>
        /// <param name="ratePerPeriod">Rate per Period (must be non-negative)</param>
        /// <param name="numberOfPeriods">Number of Periods (must be positive)</param>
        /// <returns>Result containing Loan Payment or error information</returns>
        public static Result<decimal> TryCalcLoanPayment(decimal presentValue, decimal ratePerPeriod, decimal numberOfPeriods)
        {
            var validation = ParameterValidator.ValidateNonNegative(presentValue, nameof(presentValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(ratePerPeriod, nameof(ratePerPeriod));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(numberOfPeriods, nameof(numberOfPeriods));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                if (ratePerPeriod == 0)
                {
                    return Result<decimal>.Success(presentValue / numberOfPeriods);
                }

                decimal baseValue = 1 + ratePerPeriod;
                validation = DomainValidator.ValidatePowerInput(baseValue, -numberOfPeriods, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                decimal denominator = 1 - DecimalMath.Pow(baseValue, -numberOfPeriods);
                validation = DomainValidator.ValidateDivision(ratePerPeriod * presentValue, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success((ratePerPeriod * presentValue) / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Remaining Balance on Loan from Present Value, Payment, Rate per Payment and Number of Payments.
        /// Formula: Balance = PV * (1 + r)^n - Payment * ((1 + r)^n - 1) / r
        /// </summary>
        /// <param name="presentValue">Present Value (must be non-negative)</param>
        /// <param name="payment">Payment (must be non-negative)</param>
        /// <param name="ratePerPayment">Rate Per Payment (must be non-negative)</param>
        /// <param name="numberOfPayments">Number Of Payments (must be non-negative)</param>
        /// <returns>Decimal value for Remaining Balance on Loan</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcRemainingBalanceOnLoan(decimal presentValue, decimal payment, decimal ratePerPayment, decimal numberOfPayments)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(presentValue, nameof(presentValue)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(payment, nameof(payment)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(ratePerPayment, nameof(ratePerPayment)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(numberOfPayments, nameof(numberOfPayments)).ThrowIfInvalid();

            // Validate domain for power operation
            decimal baseValue = 1 + ratePerPayment;
            DomainValidator.ValidatePowerInput(baseValue, numberOfPayments, nameof(baseValue)).ThrowIfInvalid();

            // Special case: zero rate means simple subtraction
            if (ratePerPayment == 0)
            {
                return presentValue - payment * numberOfPayments;
            }

            // Validate division
            DomainValidator.ValidateDivision(DecimalMath.Pow(baseValue, numberOfPayments) - 1, ratePerPayment, nameof(ratePerPayment)).ThrowIfInvalid();

            // Use DecimalMath instead of Math
            return presentValue * DecimalMath.Pow(baseValue, numberOfPayments)
                - payment * ((DecimalMath.Pow(baseValue, numberOfPayments) - 1) / ratePerPayment);
        }

        /// <summary>
        /// Attempts to calculate Remaining Balance on Loan from Present Value, Payment, Rate per Payment and Number of Payments.
        /// Formula: Balance = PV * (1 + r)^n - Payment * ((1 + r)^n - 1) / r
        /// </summary>
        /// <param name="presentValue">Present Value (must be non-negative)</param>
        /// <param name="payment">Payment (must be non-negative)</param>
        /// <param name="ratePerPayment">Rate Per Payment (must be non-negative)</param>
        /// <param name="numberOfPayments">Number Of Payments (must be non-negative)</param>
        /// <returns>Result containing Remaining Balance on Loan or error information</returns>
        public static Result<decimal> TryCalcRemainingBalanceOnLoan(decimal presentValue, decimal payment, decimal ratePerPayment, decimal numberOfPayments)
        {
            var validation = ParameterValidator.ValidateNonNegative(presentValue, nameof(presentValue));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(payment, nameof(payment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(ratePerPayment, nameof(ratePerPayment));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(numberOfPayments, nameof(numberOfPayments));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal baseValue = 1 + ratePerPayment;
                validation = DomainValidator.ValidatePowerInput(baseValue, numberOfPayments, nameof(baseValue));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                if (ratePerPayment == 0)
                {
                    return Result<decimal>.Success(presentValue - payment * numberOfPayments);
                }

                decimal numerator = DecimalMath.Pow(baseValue, numberOfPayments) - 1;
                validation = DomainValidator.ValidateDivision(numerator, ratePerPayment, nameof(ratePerPayment));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(
                    presentValue * DecimalMath.Pow(baseValue, numberOfPayments)
                    - payment * (numerator / ratePerPayment)
                );
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Loan to Deposit Ratio from Loans and Deposits.
        /// Formula: LDR = Loans / Deposits
        /// </summary>
        /// <param name="loans">Loans (must be non-negative)</param>
        /// <param name="deposits">Deposits (must be positive)</param>
        /// <returns>Decimal value for Loan to Deposit Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcLoanToDepositRatio(decimal loans, decimal deposits)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(loans, nameof(loans)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(deposits, nameof(deposits)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(loans, deposits, nameof(deposits)).ThrowIfInvalid();

            return loans / deposits;
        }

        /// <summary>
        /// Attempts to calculate Loan to Deposit Ratio from Loans and Deposits.
        /// Formula: LDR = Loans / Deposits
        /// </summary>
        /// <param name="loans">Loans (must be non-negative)</param>
        /// <param name="deposits">Deposits (must be positive)</param>
        /// <returns>Result containing Loan to Deposit Ratio or error information</returns>
        public static Result<decimal> TryCalcLoanToDepositRatio(decimal loans, decimal deposits)
        {
            var validation = ParameterValidator.ValidateNonNegative(loans, nameof(loans));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(deposits, nameof(deposits));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(loans, deposits, nameof(deposits));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(loans / deposits);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Loan to Value Ratio from Loan Amount and Value of Collateral.
        /// Formula: LVR = Loan Amount / Value of Collateral
        /// </summary>
        /// <param name="loanAmount">Loan Amount (must be non-negative)</param>
        /// <param name="valueOfCollateral">Value of Collateral (must be positive)</param>
        /// <returns>Decimal value for Loan to Value Ratio</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcLoanToValueRatio(decimal loanAmount, decimal valueOfCollateral)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(loanAmount, nameof(loanAmount)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(valueOfCollateral, nameof(valueOfCollateral)).ThrowIfInvalid();

            // Validate division
            DomainValidator.ValidateDivision(loanAmount, valueOfCollateral, nameof(valueOfCollateral)).ThrowIfInvalid();

            return loanAmount / valueOfCollateral;
        }

        /// <summary>
        /// Attempts to calculate Loan to Value Ratio from Loan Amount and Value of Collateral.
        /// Formula: LVR = Loan Amount / Value of Collateral
        /// </summary>
        /// <param name="loanAmount">Loan Amount (must be non-negative)</param>
        /// <param name="valueOfCollateral">Value of Collateral (must be positive)</param>
        /// <returns>Result containing Loan to Value Ratio or error information</returns>
        public static Result<decimal> TryCalcLoanToValueRatio(decimal loanAmount, decimal valueOfCollateral)
        {
            var validation = ParameterValidator.ValidateNonNegative(loanAmount, nameof(loanAmount));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(valueOfCollateral, nameof(valueOfCollateral));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                validation = DomainValidator.ValidateDivision(loanAmount, valueOfCollateral, nameof(valueOfCollateral));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(loanAmount / valueOfCollateral);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Simple Interest from Principal, Rate and Time.
        /// Formula: SI = P * r * t
        /// </summary>
        /// <param name="principal">Principal (must be non-negative)</param>
        /// <param name="rate">Rate (must be non-negative)</param>
        /// <param name="time">Time (must be non-negative)</param>
        /// <returns>Decimal value for Simple Interest</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcSimpleInterest(decimal principal, decimal rate, decimal time)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(principal, nameof(principal)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(rate, nameof(rate)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(time, nameof(time)).ThrowIfInvalid();

            return principal * rate * time;
        }

        /// <summary>
        /// Attempts to calculate Simple Interest from Principal, Rate and Time.
        /// Formula: SI = P * r * t
        /// </summary>
        /// <param name="principal">Principal (must be non-negative)</param>
        /// <param name="rate">Rate (must be non-negative)</param>
        /// <param name="time">Time (must be non-negative)</param>
        /// <returns>Result containing Simple Interest or error information</returns>
        public static Result<decimal> TryCalcSimpleInterest(decimal principal, decimal rate, decimal time)
        {
            var validation = ParameterValidator.ValidateNonNegative(principal, nameof(principal));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(rate, nameof(rate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(time, nameof(time));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                return Result<decimal>.Success(principal * rate * time);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Simple Interest Rate from Principal, Interest and Time.
        /// Formula: r = I / (P * t)
        /// </summary>
        /// <param name="principal">Principal (must be positive)</param>
        /// <param name="interest">Interest (must be non-negative)</param>
        /// <param name="time">Time (must be positive)</param>
        /// <returns>Decimal value for Simple Interest Rate</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcSimpleInterestRate(decimal principal, decimal interest, decimal time)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(principal, nameof(principal)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(interest, nameof(interest)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(time, nameof(time)).ThrowIfInvalid();

            // Validate division
            decimal denominator = principal * time;
            DomainValidator.ValidateDivision(interest, denominator, nameof(denominator)).ThrowIfInvalid();

            return interest / denominator;
        }

        /// <summary>
        /// Attempts to calculate Simple Interest Rate from Principal, Interest and Time.
        /// Formula: r = I / (P * t)
        /// </summary>
        /// <param name="principal">Principal (must be positive)</param>
        /// <param name="interest">Interest (must be non-negative)</param>
        /// <param name="time">Time (must be positive)</param>
        /// <returns>Result containing Simple Interest Rate or error information</returns>
        public static Result<decimal> TryCalcSimpleInterestRate(decimal principal, decimal interest, decimal time)
        {
            var validation = ParameterValidator.ValidatePositive(principal, nameof(principal));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(interest, nameof(interest));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(time, nameof(time));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal denominator = principal * time;
                validation = DomainValidator.ValidateDivision(interest, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(interest / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Simple Interest Principal from Interest, Rate and Time.
        /// Formula: P = I / (r * t)
        /// </summary>
        /// <param name="interest">Interest (must be non-negative)</param>
        /// <param name="rate">Rate (must be positive)</param>
        /// <param name="time">Time (must be positive)</param>
        /// <returns>Decimal value for Simple Interest Principal</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcSimpleInterestPrincipal(decimal interest, decimal rate, decimal time)
        {
            // Validate inputs
            ParameterValidator.ValidateNonNegative(interest, nameof(interest)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(rate, nameof(rate)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(time, nameof(time)).ThrowIfInvalid();

            // Validate division
            decimal denominator = rate * time;
            DomainValidator.ValidateDivision(interest, denominator, nameof(denominator)).ThrowIfInvalid();

            return interest / denominator;
        }

        /// <summary>
        /// Attempts to calculate Simple Interest Principal from Interest, Rate and Time.
        /// Formula: P = I / (r * t)
        /// </summary>
        /// <param name="interest">Interest (must be non-negative)</param>
        /// <param name="rate">Rate (must be positive)</param>
        /// <param name="time">Time (must be positive)</param>
        /// <returns>Result containing Simple Interest Principal or error information</returns>
        public static Result<decimal> TryCalcSimpleInterestPrincipal(decimal interest, decimal rate, decimal time)
        {
            var validation = ParameterValidator.ValidateNonNegative(interest, nameof(interest));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(rate, nameof(rate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(time, nameof(time));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal denominator = rate * time;
                validation = DomainValidator.ValidateDivision(interest, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(interest / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates Simple Interest Time from Principal, Interest, and Rate.
        /// Formula: t = I / (r * P)
        /// </summary>
        /// <param name="principal">Principal (must be positive)</param>
        /// <param name="interest">Interest (must be non-negative)</param>
        /// <param name="rate">Rate (must be positive)</param>
        /// <returns>Decimal value for Simple Interest Time</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static decimal CalcSimpleInterestTime(decimal principal, decimal interest, decimal rate)
        {
            // Validate inputs
            ParameterValidator.ValidatePositive(principal, nameof(principal)).ThrowIfInvalid();
            ParameterValidator.ValidateNonNegative(interest, nameof(interest)).ThrowIfInvalid();
            ParameterValidator.ValidatePositive(rate, nameof(rate)).ThrowIfInvalid();

            // Validate division
            decimal denominator = rate * principal;
            DomainValidator.ValidateDivision(interest, denominator, nameof(denominator)).ThrowIfInvalid();

            return interest / denominator;
        }

        /// <summary>
        /// Attempts to calculate Simple Interest Time from Principal, Interest, and Rate.
        /// Formula: t = I / (r * P)
        /// </summary>
        /// <param name="principal">Principal (must be positive)</param>
        /// <param name="interest">Interest (must be non-negative)</param>
        /// <param name="rate">Rate (must be positive)</param>
        /// <returns>Result containing Simple Interest Time or error information</returns>
        public static Result<decimal> TryCalcSimpleInterestTime(decimal principal, decimal interest, decimal rate)
        {
            var validation = ParameterValidator.ValidatePositive(principal, nameof(principal));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidateNonNegative(interest, nameof(interest));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            validation = ParameterValidator.ValidatePositive(rate, nameof(rate));
            if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

            try
            {
                decimal denominator = rate * principal;
                validation = DomainValidator.ValidateDivision(interest, denominator, nameof(denominator));
                if (!validation.IsValid) return Result<decimal>.Failure(validation.FirstError, validation.Context);

                return Result<decimal>.Success(interest / denominator);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Calculation error: {ex.Message}");
            }
        }
    }
}
