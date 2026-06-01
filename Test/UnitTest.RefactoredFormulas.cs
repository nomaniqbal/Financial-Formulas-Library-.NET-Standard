using System;
using Xunit;
using srbrettle.FinancialFormulas;
using srbrettle.FinancialFormulas.Core;
using srbrettle.FinancialFormulas.Validation;

namespace UnitTest_FinancialFormulas
{
    /// <summary>
    /// Comprehensive test suite for refactored Financial Formulas Library
    /// Tests validation, error handling, and Result pattern functionality
    /// </summary>

    #region Result Pattern Tests

    public class UnitTest_ResultPattern
    {
        [Fact]
        public void Result_Success_IsSuccessTrue()
        {
            var result = Result<decimal>.Success(100m);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(100m, result.Value);
            Assert.Null(result.Error);
            Assert.Null(result.ErrorContext);
        }

        [Fact]
        public void Result_Failure_IsSuccessFalse()
        {
            var result = Result<decimal>.Failure("Test error");
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal("Test error", result.Error);
            Assert.Equal(default(decimal), result.Value);
        }

        [Fact]
        public void Result_Failure_WithNullError_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Result<decimal>.Failure(null));
        }

        [Fact]
        public void Result_Failure_WithEmptyError_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Result<decimal>.Failure(""));
        }

        [Fact]
        public void Result_Map_TransformsSuccessValue()
        {
            var result = Result<decimal>.Success(100m);
            var mapped = result.Map(x => x * 2);
            Assert.True(mapped.IsSuccess);
            Assert.Equal(200m, mapped.Value);
        }

        [Fact]
        public void Result_Map_PreservesFailure()
        {
            var result = Result<decimal>.Failure("Test error");
            var mapped = result.Map(x => x * 2);
            Assert.False(mapped.IsSuccess);
            Assert.Equal("Test error", mapped.Error);
        }

        [Fact]
        public void Result_Bind_ChainsSuccessfully()
        {
            var result = Result<decimal>.Success(100m);
            var bound = result.Bind(x => Result<decimal>.Success(x * 2));
            Assert.True(bound.IsSuccess);
            Assert.Equal(200m, bound.Value);
        }

        [Fact]
        public void Result_Bind_PreservesFailure()
        {
            var result = Result<decimal>.Failure("Test error");
            var bound = result.Bind(x => Result<decimal>.Success(x * 2));
            Assert.False(bound.IsSuccess);
            Assert.Equal("Test error", bound.Error);
        }

        [Fact]
        public void Result_Match_ExecutesSuccessPath()
        {
            var result = Result<decimal>.Success(100m);
            var matched = result.Match(
                onSuccess: x => $"Success: {x}",
                onFailure: e => $"Failure: {e}"
            );
            Assert.Equal("Success: 100", matched);
        }

        [Fact]
        public void Result_Match_ExecutesFailurePath()
        {
            var result = Result<decimal>.Failure("Test error");
            var matched = result.Match(
                onSuccess: x => $"Success: {x}",
                onFailure: e => $"Failure: {e}"
            );
            Assert.Equal("Failure: Test error", matched);
        }

        [Fact]
        public void Result_ToString_FormatsCorrectly()
        {
            var success = Result<decimal>.Success(100m);
            Assert.Equal("Success: 100", success.ToString());

            var failure = Result<decimal>.Failure("Test error");
            Assert.Equal("Failure: Test error", failure.ToString());
        }
    }

    #endregion

    #region ValidationResult Tests

    public class UnitTest_ValidationResult
    {
        [Fact]
        public void ValidationResult_Valid_IsValidTrue()
        {
            var result = ValidationResult.Valid();
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Null(result.FirstError);
            Assert.Null(result.Context);
        }

        [Fact]
        public void ValidationResult_Invalid_IsValidFalse()
        {
            var result = ValidationResult.Invalid("Test error");
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Test error", result.FirstError);
        }

        [Fact]
        public void ValidationResult_Invalid_WithNullError_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => ValidationResult.Invalid((string)null));
        }

        [Fact]
        public void ValidationResult_Invalid_WithEmptyError_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => ValidationResult.Invalid(""));
        }

        [Fact]
        public void ValidationResult_Invalid_WithMultipleErrors()
        {
            var errors = new[] { "Error 1", "Error 2", "Error 3" };
            var result = ValidationResult.Invalid(errors);
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
            Assert.Equal("Error 1", result.FirstError);
        }

        [Fact]
        public void ValidationResult_Combine_BothValid_ReturnsValid()
        {
            var result1 = ValidationResult.Valid();
            var result2 = ValidationResult.Valid();
            var combined = result1.Combine(result2);
            Assert.True(combined.IsValid);
        }

        [Fact]
        public void ValidationResult_Combine_OneInvalid_ReturnsInvalid()
        {
            var result1 = ValidationResult.Valid();
            var result2 = ValidationResult.Invalid("Error");
            var combined = result1.Combine(result2);
            Assert.False(combined.IsValid);
            Assert.Single(combined.Errors);
        }

        [Fact]
        public void ValidationResult_Combine_BothInvalid_CombinesErrors()
        {
            var result1 = ValidationResult.Invalid("Error 1");
            var result2 = ValidationResult.Invalid("Error 2");
            var combined = result1.Combine(result2);
            Assert.False(combined.IsValid);
            Assert.Equal(2, combined.Errors.Count);
        }

        [Fact]
        public void ValidationResult_ThrowIfInvalid_ThrowsWhenInvalid()
        {
            var result = ValidationResult.Invalid("Test error");
            Assert.Throws<ArgumentException>(() => result.ThrowIfInvalid());
        }

        [Fact]
        public void ValidationResult_ThrowIfInvalid_DoesNotThrowWhenValid()
        {
            var result = ValidationResult.Valid();
            result.ThrowIfInvalid(); // Should not throw
        }

        [Fact]
        public void ValidationResult_AllErrors_CombinesMessages()
        {
            var result = ValidationResult.Invalid(new[] { "Error 1", "Error 2" });
            Assert.Equal("Error 1; Error 2", result.AllErrors);
        }
    }

    #endregion

    #region ErrorContext Tests

    public class UnitTest_ErrorContext
    {
        [Fact]
        public void ErrorContext_Properties_SetCorrectly()
        {
            var context = new ErrorContext
            {
                ParameterName = "testParam",
                ParameterValue = -5m,
                ConstraintViolated = "Must be positive",
                ValidRange = "(0, +∞)",
                FormulaName = "TestFormula"
            };

            Assert.Equal("testParam", context.ParameterName);
            Assert.Equal(-5m, context.ParameterValue);
            Assert.Equal("Must be positive", context.ConstraintViolated);
            Assert.Equal("(0, +∞)", context.ValidRange);
            Assert.Equal("TestFormula", context.FormulaName);
        }

        [Fact]
        public void ErrorContext_ToDetailedMessage_FormatsCorrectly()
        {
            var context = new ErrorContext
            {
                ParameterName = "testParam",
                ParameterValue = -5m,
                ConstraintViolated = "Must be positive",
                ValidRange = "(0, +∞)"
            };

            var message = context.ToDetailedMessage();
            Assert.Contains("testParam", message);
            Assert.Contains("-5", message);
            Assert.Contains("Must be positive", message);
            Assert.Contains("(0, +∞)", message);
        }

        [Fact]
        public void ErrorContext_ToString_CallsToDetailedMessage()
        {
            var context = new ErrorContext
            {
                ParameterName = "testParam",
                ParameterValue = -5m
            };

            var toString = context.ToString();
            var detailed = context.ToDetailedMessage();
            Assert.Equal(detailed, toString);
        }

        [Fact]
        public void ErrorContext_WithInnerException_IncludesExceptionMessage()
        {
            var innerEx = new InvalidOperationException("Inner error");
            var context = new ErrorContext
            {
                InnerException = innerEx
            };

            var message = context.ToDetailedMessage();
            Assert.Contains("Inner error", message);
        }

        [Fact]
        public void ErrorContext_WithAdditionalInfo_IncludesInfo()
        {
            var context = new ErrorContext
            {
                AdditionalInfo = "This is extra information"
            };

            var message = context.ToDetailedMessage();
            Assert.Contains("This is extra information", message);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcAnnualPercentageYield

    public class UnitTest_BankingFormulas_AnnualPercentageYield
    {
        [Fact]
        public void CalcAnnualPercentageYield_NegativeRate_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcAnnualPercentageYield(-0.05m, 12m));
        }

        [Fact]
        public void TryCalcAnnualPercentageYield_NegativeRate_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcAnnualPercentageYield(-0.05m, 12m);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("statedAnnualInterestRate", result.Error);
        }

        [Fact]
        public void TryCalcAnnualPercentageYield_NegativeCompounding_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcAnnualPercentageYield(0.05m, -12m);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("numberOfTimesCompounded", result.Error);
        }

        [Fact]
        public void TryCalcAnnualPercentageYield_ZeroCompounding_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcAnnualPercentageYield(0.05m, 0m);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public void TryCalcAnnualPercentageYield_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcAnnualPercentageYield(0.04m, 12m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0.0407415m, Math.Round(result.Value, 7, MidpointRounding.AwayFromZero));
        }

        [Fact]
        public void TryCalcAnnualPercentageYield_ZeroRate_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcAnnualPercentageYield(0m, 12m);
            Assert.True(result.IsSuccess);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcCompoundInterest

    public class UnitTest_BankingFormulas_CompoundInterest
    {
        [Fact]
        public void CalcCompoundInterest_NegativePrincipal_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcCompoundInterest(-1000m, 0.05m, 10m));
        }

        [Fact]
        public void TryCalcCompoundInterest_NegativePrincipal_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcCompoundInterest(-1000m, 0.05m, 10m);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("principal", result.Error);
            Assert.NotNull(result.ErrorContext);
        }

        [Fact]
        public void TryCalcCompoundInterest_NegativeRate_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcCompoundInterest(1000m, -0.05m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("ratePerPeriod", result.Error);
        }

        [Fact]
        public void TryCalcCompoundInterest_NegativePeriods_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcCompoundInterest(1000m, 0.05m, -10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("numberOfPeriods", result.Error);
        }

        [Fact]
        public void TryCalcCompoundInterest_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcCompoundInterest(1000m, 0.07m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(967.15m, Math.Round(result.Value, 2, MidpointRounding.AwayFromZero));
        }

        [Fact]
        public void TryCalcCompoundInterest_ZeroValues_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcCompoundInterest(0m, 0.05m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcDebtToIncomeRatio

    public class UnitTest_BankingFormulas_DebtToIncomeRatio
    {
        [Fact]
        public void CalcDebtToIncomeRatio_NegativeDebt_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcDebtToIncomeRatio(-100m, 1000m));
        }

        [Fact]
        public void TryCalcDebtToIncomeRatio_NegativeDebt_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcDebtToIncomeRatio(-100m, 1000m);
            Assert.False(result.IsSuccess);
            Assert.Contains("monthlyDebtPayments", result.Error);
        }

        [Fact]
        public void TryCalcDebtToIncomeRatio_ZeroIncome_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcDebtToIncomeRatio(100m, 0m);
            Assert.False(result.IsSuccess);
            Assert.Contains("grossMonthlyIncome", result.Error);
        }

        [Fact]
        public void TryCalcDebtToIncomeRatio_NegativeIncome_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcDebtToIncomeRatio(100m, -1000m);
            Assert.False(result.IsSuccess);
            Assert.Contains("grossMonthlyIncome", result.Error);
        }

        [Fact]
        public void TryCalcDebtToIncomeRatio_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcDebtToIncomeRatio(250m, 1000m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0.25m, result.Value);
        }

        [Fact]
        public void TryCalcDebtToIncomeRatio_ZeroDebt_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcDebtToIncomeRatio(0m, 1000m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcLoanPayment

    public class UnitTest_BankingFormulas_LoanPayment
    {
        [Fact]
        public void CalcLoanPayment_NegativePrincipal_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcLoanPayment(-1000m, 0.04m, 10m));
        }

        [Fact]
        public void TryCalcLoanPayment_NegativePrincipal_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcLoanPayment(-1000m, 0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("presentValue", result.Error);
        }

        [Fact]
        public void TryCalcLoanPayment_NegativeRate_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcLoanPayment(1000m, -0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("ratePerPeriod", result.Error);
        }

        [Fact]
        public void TryCalcLoanPayment_ZeroPeriods_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcLoanPayment(1000m, 0.04m, 0m);
            Assert.False(result.IsSuccess);
            Assert.Contains("numberOfPeriods", result.Error);
        }

        [Fact]
        public void TryCalcLoanPayment_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcLoanPayment(1000m, 0.04m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(123.29m, Math.Round(result.Value, 2, MidpointRounding.AwayFromZero));
        }

        [Fact]
        public void TryCalcLoanPayment_ZeroRate_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcLoanPayment(1000m, 0m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(100m, result.Value); // Simple division
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcSimpleInterest

    public class UnitTest_BankingFormulas_SimpleInterest
    {
        [Fact]
        public void CalcSimpleInterest_NegativePrincipal_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcSimpleInterest(-1000m, 0.04m, 10m));
        }

        [Fact]
        public void TryCalcSimpleInterest_NegativePrincipal_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterest(-1000m, 0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("principal", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterest_NegativeRate_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterest(1000m, -0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("rate", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterest_NegativeTime_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterest(1000m, 0.04m, -10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("time", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterest_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcSimpleInterest(1000m, 0.04m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(400m, result.Value);
        }

        [Fact]
        public void TryCalcSimpleInterest_ZeroValues_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcSimpleInterest(0m, 0m, 0m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcSimpleInterestRate

    public class UnitTest_BankingFormulas_SimpleInterestRate
    {
        [Fact]
        public void CalcSimpleInterestRate_ZeroPrincipal_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcSimpleInterestRate(0m, 400m, 10m));
        }

        [Fact]
        public void TryCalcSimpleInterestRate_ZeroPrincipal_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestRate(0m, 400m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("principal", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestRate_NegativeInterest_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestRate(1000m, -400m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("interest", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestRate_ZeroTime_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestRate(1000m, 400m, 0m);
            Assert.False(result.IsSuccess);
            Assert.Contains("time", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestRate_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcSimpleInterestRate(1000m, 400m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0.04m, result.Value);
        }

        [Fact]
        public void TryCalcSimpleInterestRate_ZeroInterest_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcSimpleInterestRate(1000m, 0m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcSimpleInterestPrincipal

    public class UnitTest_BankingFormulas_SimpleInterestPrincipal
    {
        [Fact]
        public void CalcSimpleInterestPrincipal_ZeroRate_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcSimpleInterestPrincipal(400m, 0m, 10m));
        }

        [Fact]
        public void TryCalcSimpleInterestPrincipal_ZeroRate_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestPrincipal(400m, 0m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("rate", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestPrincipal_ZeroTime_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestPrincipal(400m, 0.04m, 0m);
            Assert.False(result.IsSuccess);
            Assert.Contains("time", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestPrincipal_NegativeInterest_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestPrincipal(-400m, 0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("interest", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestPrincipal_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcSimpleInterestPrincipal(400m, 0.04m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(1000m, result.Value);
        }

        [Fact]
        public void TryCalcSimpleInterestPrincipal_ZeroInterest_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcSimpleInterestPrincipal(0m, 0.04m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcSimpleInterestTime

    public class UnitTest_BankingFormulas_SimpleInterestTime
    {
        [Fact]
        public void CalcSimpleInterestTime_ZeroPrincipal_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcSimpleInterestTime(0m, 400m, 0.04m));
        }

        [Fact]
        public void TryCalcSimpleInterestTime_ZeroPrincipal_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestTime(0m, 400m, 0.04m);
            Assert.False(result.IsSuccess);
            Assert.Contains("principal", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestTime_ZeroRate_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestTime(1000m, 400m, 0m);
            Assert.False(result.IsSuccess);
            Assert.Contains("rate", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestTime_NegativeInterest_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcSimpleInterestTime(1000m, -400m, 0.04m);
            Assert.False(result.IsSuccess);
            Assert.Contains("interest", result.Error);
        }

        [Fact]
        public void TryCalcSimpleInterestTime_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcSimpleInterestTime(1000m, 400m, 0.04m);
            Assert.True(result.IsSuccess);
            Assert.Equal(10m, result.Value);
        }

        [Fact]
        public void TryCalcSimpleInterestTime_ZeroInterest_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcSimpleInterestTime(1000m, 0m, 0.04m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcLoanToDepositRatio

    public class UnitTest_BankingFormulas_LoanToDepositRatio
    {
        [Fact]
        public void CalcLoanToDepositRatio_ZeroDeposits_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcLoanToDepositRatio(10000m, 0m));
        }

        [Fact]
        public void TryCalcLoanToDepositRatio_ZeroDeposits_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcLoanToDepositRatio(10000m, 0m);
            Assert.False(result.IsSuccess);
            Assert.Contains("deposits", result.Error);
        }

        [Fact]
        public void TryCalcLoanToDepositRatio_NegativeLoans_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcLoanToDepositRatio(-10000m, 4000m);
            Assert.False(result.IsSuccess);
            Assert.Contains("loans", result.Error);
        }

        [Fact]
        public void TryCalcLoanToDepositRatio_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcLoanToDepositRatio(10000m, 4000m);
            Assert.True(result.IsSuccess);
            Assert.Equal(2.5m, result.Value);
        }

        [Fact]
        public void TryCalcLoanToDepositRatio_ZeroLoans_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcLoanToDepositRatio(0m, 4000m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcLoanToValueRatio

    public class UnitTest_BankingFormulas_LoanToValueRatio
    {
        [Fact]
        public void CalcLoanToValueRatio_ZeroCollateral_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcLoanToValueRatio(150000m, 0m));
        }

        [Fact]
        public void TryCalcLoanToValueRatio_ZeroCollateral_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcLoanToValueRatio(150000m, 0m);
            Assert.False(result.IsSuccess);
            Assert.Contains("valueOfCollateral", result.Error);
        }

        [Fact]
        public void TryCalcLoanToValueRatio_NegativeLoanAmount_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcLoanToValueRatio(-150000m, 130000m);
            Assert.False(result.IsSuccess);
            Assert.Contains("loanAmount", result.Error);
        }

        [Fact]
        public void TryCalcLoanToValueRatio_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcLoanToValueRatio(150000m, 130000m);
            Assert.True(result.IsSuccess);
            Assert.Equal(1.15m, Math.Round(result.Value, 2, MidpointRounding.AwayFromZero));
        }

        [Fact]
        public void TryCalcLoanToValueRatio_ZeroLoan_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcLoanToValueRatio(0m, 130000m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcBalloonLoanPayment

    public class UnitTest_BankingFormulas_BalloonLoanPayment
    {
        [Fact]
        public void CalcBalloonLoanPayment_NegativePresentValue_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcBalloonLoanPayment(-10000m, 2000m, 0.04m, 10m));
        }

        [Fact]
        public void TryCalcBalloonLoanPayment_NegativePresentValue_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcBalloonLoanPayment(-10000m, 2000m, 0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("presentValue", result.Error);
        }

        [Fact]
        public void TryCalcBalloonLoanPayment_NegativeBalloonAmount_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcBalloonLoanPayment(10000m, -2000m, 0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("balloonAmount", result.Error);
        }

        [Fact]
        public void TryCalcBalloonLoanPayment_ZeroPeriods_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcBalloonLoanPayment(10000m, 2000m, 0.04m, 0m);
            Assert.False(result.IsSuccess);
            Assert.Contains("numberOfPeriods", result.Error);
        }

        [Fact]
        public void TryCalcBalloonLoanPayment_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcBalloonLoanPayment(10000m, 2000m, 0.04m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(1066.33m, Math.Round(result.Value, 2, MidpointRounding.AwayFromZero));
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcBalloonBalanceOfLoan

    public class UnitTest_BankingFormulas_BalloonBalanceOfLoan
    {
        [Fact]
        public void CalcBalloonBalanceOfLoan_NegativePresentValue_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcBalloonBalanceOfLoan(-100000m, 500m, 0.04m, 25m));
        }

        [Fact]
        public void TryCalcBalloonBalanceOfLoan_NegativePresentValue_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcBalloonBalanceOfLoan(-100000m, 500m, 0.04m, 25m);
            Assert.False(result.IsSuccess);
            Assert.Contains("presentValue", result.Error);
        }

        [Fact]
        public void TryCalcBalloonBalanceOfLoan_NegativePayment_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcBalloonBalanceOfLoan(100000m, -500m, 0.04m, 25m);
            Assert.False(result.IsSuccess);
            Assert.Contains("payment", result.Error);
        }

        [Fact]
        public void TryCalcBalloonBalanceOfLoan_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcBalloonBalanceOfLoan(100000m, 500m, 0.04m, 25m);
            Assert.True(result.IsSuccess);
            Assert.Equal(245760.68m, Math.Round(result.Value, 2, MidpointRounding.AwayFromZero));
        }

        [Fact]
        public void TryCalcBalloonBalanceOfLoan_ZeroRate_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcBalloonBalanceOfLoan(10000m, 100m, 0m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(9000m, result.Value); // Simple subtraction when rate is zero
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcRemainingBalanceOnLoan

    public class UnitTest_BankingFormulas_RemainingBalanceOnLoan
    {
        [Fact]
        public void CalcRemainingBalanceOnLoan_NegativePresentValue_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcRemainingBalanceOnLoan(-10000m, 250m, 0.04m, 10m));
        }

        [Fact]
        public void TryCalcRemainingBalanceOnLoan_NegativePresentValue_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcRemainingBalanceOnLoan(-10000m, 250m, 0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("presentValue", result.Error);
        }

        [Fact]
        public void TryCalcRemainingBalanceOnLoan_NegativePayment_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcRemainingBalanceOnLoan(10000m, -250m, 0.04m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("payment", result.Error);
        }

        [Fact]
        public void TryCalcRemainingBalanceOnLoan_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcRemainingBalanceOnLoan(10000m, 250m, 0.04m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(11800.92m, Math.Round(result.Value, 2, MidpointRounding.AwayFromZero));
        }

        [Fact]
        public void TryCalcRemainingBalanceOnLoan_ZeroRate_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcRemainingBalanceOnLoan(10000m, 100m, 0m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(9000m, result.Value);
        }
    }

    #endregion

    #region BankingFormulas Validation Tests - CalcContinuousCompounding

    public class UnitTest_BankingFormulas_ContinuousCompounding
    {
        [Fact]
        public void CalcContinuousCompounding_NegativePrincipal_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                BankingFormulas.CalcContinuousCompounding(-1000m, 0.07m, 10m));
        }

        [Fact]
        public void TryCalcContinuousCompounding_NegativePrincipal_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcContinuousCompounding(-1000m, 0.07m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("principal", result.Error);
        }

        [Fact]
        public void TryCalcContinuousCompounding_NegativeRate_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcContinuousCompounding(1000m, -0.07m, 10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("rate", result.Error);
        }

        [Fact]
        public void TryCalcContinuousCompounding_NegativeTime_ReturnsFailure()
        {
            var result = BankingFormulas.TryCalcContinuousCompounding(1000m, 0.07m, -10m);
            Assert.False(result.IsSuccess);
            Assert.Contains("time", result.Error);
        }

        [Fact]
        public void TryCalcContinuousCompounding_ValidInputs_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcContinuousCompounding(1000m, 0.07m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(2013.75m, Math.Round(result.Value, 2, MidpointRounding.AwayFromZero));
        }

        [Fact]
        public void TryCalcContinuousCompounding_ZeroValues_ReturnsSuccess()
        {
            var result = BankingFormulas.TryCalcContinuousCompounding(0m, 0.07m, 10m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }
    }

    #endregion

    #region FinancialMarketsFormulas Validation Tests - CalcRateOfInflation

    public class UnitTest_FinancialMarketsFormulas_RateOfInflation
    {
        [Fact]
        public void CalcRateOfInflation_ZeroInitialCPI_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                FinancialMarketsFormulas.CalcRateOfInflation(0m, 105m));
        }

        [Fact]
        public void TryCalcRateOfInflation_ZeroInitialCPI_ReturnsFailure()
        {
            var result = FinancialMarketsFormulas.TryCalcRateOfInflation(0m, 105m);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("initialConsumerPriceIndex", result.Error);
            Assert.NotNull(result.ErrorContext);
        }

        [Fact]
        public void TryCalcRateOfInflation_ValidInputs_ReturnsSuccess()
        {
            var result = FinancialMarketsFormulas.TryCalcRateOfInflation(100m, 105m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0.05m, result.Value);
        }

        [Fact]
        public void TryCalcRateOfInflation_NegativeInflation_ReturnsSuccess()
        {
            var result = FinancialMarketsFormulas.TryCalcRateOfInflation(100m, 95m);
            Assert.True(result.IsSuccess);
            Assert.Equal(-0.05m, result.Value);
        }

        [Fact]
        public void TryCalcRateOfInflation_SameCPI_ReturnsZero()
        {
            var result = FinancialMarketsFormulas.TryCalcRateOfInflation(100m, 100m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value);
        }

        [Fact]
        public void TryCalcRateOfInflation_NegativeCPI_ReturnsSuccess()
        {
            var result = FinancialMarketsFormulas.TryCalcRateOfInflation(-100m, -95m);
            Assert.True(result.IsSuccess);
        }
    }

    #endregion

    #region FinancialMarketsFormulas Validation Tests - CalcRealRateOfReturn

    public class UnitTest_FinancialMarketsFormulas_RealRateOfReturn
    {
        [Fact]
        public void CalcRealRateOfReturn_InflationRateNegativeOne_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                FinancialMarketsFormulas.CalcRealRateOfReturn(0.08m, -1m));
        }

        [Fact]
        public void TryCalcRealRateOfReturn_InflationRateNegativeOne_ReturnsFailure()
        {
            var result = FinancialMarketsFormulas.TryCalcRealRateOfReturn(0.08m, -1m);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("inflationRate", result.Error);
            Assert.NotNull(result.ErrorContext);
            Assert.Contains("division by zero", result.Error);
        }

        [Fact]
        public void TryCalcRealRateOfReturn_ValidInputs_ReturnsSuccess()
        {
            var result = FinancialMarketsFormulas.TryCalcRealRateOfReturn(0.08m, 0.03m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0.0485m, Math.Round(result.Value, 4, MidpointRounding.AwayFromZero));
        }

        [Fact]
        public void TryCalcRealRateOfReturn_ZeroInflation_ReturnsSuccess()
        {
            var result = FinancialMarketsFormulas.TryCalcRealRateOfReturn(0.08m, 0m);
            Assert.True(result.IsSuccess);
            Assert.Equal(0.08m, result.Value);
        }

        [Fact]
        public void TryCalcRealRateOfReturn_NegativeNominalRate_ReturnsSuccess()
        {
            var result = FinancialMarketsFormulas.TryCalcRealRateOfReturn(-0.05m, 0.02m);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void TryCalcRealRateOfReturn_HighInflation_ReturnsSuccess()
        {
            var result = FinancialMarketsFormulas.TryCalcRealRateOfReturn(0.05m, 0.10m);
            Assert.True(result.IsSuccess);
        }
    }

    #endregion

    #region ParameterValidator Tests

    public class UnitTest_ParameterValidator
    {
        [Fact]
        public void ValidatePositive_PositiveValue_ReturnsValid()
        {
            var result = ParameterValidator.ValidatePositive(10m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidatePositive_ZeroValue_ReturnsInvalid()
        {
            var result = ParameterValidator.ValidatePositive(0m, "test");
            Assert.False(result.IsValid);
            Assert.Contains("test", result.FirstError);
        }

        [Fact]
        public void ValidatePositive_NegativeValue_ReturnsInvalid()
        {
            var result = ParameterValidator.ValidatePositive(-10m, "test");
            Assert.False(result.IsValid);
            Assert.NotNull(result.Context);
        }

        [Fact]
        public void ValidateNonNegative_PositiveValue_ReturnsValid()
        {
            var result = ParameterValidator.ValidateNonNegative(10m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateNonNegative_ZeroValue_ReturnsValid()
        {
            var result = ParameterValidator.ValidateNonNegative(0m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateNonNegative_NegativeValue_ReturnsInvalid()
        {
            var result = ParameterValidator.ValidateNonNegative(-10m, "test");
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidateNonZero_PositiveValue_ReturnsValid()
        {
            var result = ParameterValidator.ValidateNonZero(10m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateNonZero_NegativeValue_ReturnsValid()
        {
            var result = ParameterValidator.ValidateNonZero(-10m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateNonZero_ZeroValue_ReturnsInvalid()
        {
            var result = ParameterValidator.ValidateNonZero(0m, "test");
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidatePercentage_ValidPercentage_ReturnsValid()
        {
            var result = ParameterValidator.ValidatePercentage(0.5m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidatePercentage_Zero_ReturnsValid()
        {
            var result = ParameterValidator.ValidatePercentage(0m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidatePercentage_One_ReturnsValid()
        {
            var result = ParameterValidator.ValidatePercentage(1m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidatePercentage_GreaterThanOne_ReturnsInvalid()
        {
            var result = ParameterValidator.ValidatePercentage(1.5m, "test");
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidatePercentage_Negative_ReturnsInvalid()
        {
            var result = ParameterValidator.ValidatePercentage(-0.5m, "test");
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidateRange_ValueInRange_ReturnsValid()
        {
            var result = ParameterValidator.ValidateRange(50m, 0m, 100m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateRange_ValueAtMin_ReturnsValid()
        {
            var result = ParameterValidator.ValidateRange(0m, 0m, 100m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateRange_ValueAtMax_ReturnsValid()
        {
            var result = ParameterValidator.ValidateRange(100m, 0m, 100m, "test");
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateRange_ValueBelowMin_ReturnsInvalid()
        {
            var result = ParameterValidator.ValidateRange(-10m, 0m, 100m, "test");
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidateRange_ValueAboveMax_ReturnsInvalid()
        {
            var result = ParameterValidator.ValidateRange(110m, 0m, 100m, "test");
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidateRange_InvalidMinMax_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                ParameterValidator.ValidateRange(50m, 100m, 0m, "test"));
        }
    }

    #endregion
}
