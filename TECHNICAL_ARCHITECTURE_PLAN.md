# Technical Architecture & Design Plan
## Financial Formulas Library v2.0

---

## 1. Architecture Overview

### 1.1 High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Public API Layer                         │
│  FinancialFormulas | BankingFormulas | CorporateFormulas    │
│  GeneralFinanceFormulas | StocksBondsFormulas | Markets     │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                  Validation Layer                            │
│  InputValidator | DomainValidator | BusinessRuleValidator   │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                 Calculation Engine                           │
│  DecimalMath | PrecisionCalculator | FormulaEngine          │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                   Core Utilities                             │
│  Result<T> | ValidationResult | ErrorContext                │
└─────────────────────────────────────────────────────────────┘
```

### 1.2 Project Structure

```
FinancialFormulas/
├── Core/
│   ├── Result.cs                    # Result<T> pattern
│   ├── ValidationResult.cs          # Validation results
│   ├── ErrorContext.cs              # Error details
│   └── FinancialException.cs        # Custom exceptions
│
├── Validation/
│   ├── IValidator.cs                # Validator interface
│   ├── ParameterValidator.cs        # Parameter validation
│   ├── DomainValidator.cs           # Mathematical domain validation
│   ├── BusinessRuleValidator.cs     # Business logic validation
│   └── ValidationRules/             # Specific validation rules
│       ├── RangeRule.cs
│       ├── NonZeroRule.cs
│       ├── PositiveRule.cs
│       └── PercentageRule.cs
│
├── Mathematics/
│   ├── DecimalMath.cs               # Decimal-based math operations
│   ├── PrecisionHelper.cs           # Precision management
│   └── Constants.cs                 # Mathematical constants
│
├── Formulas/
│   ├── FinancialFormulas.cs         # Refactored
│   ├── BankingFormulas.cs           # Refactored
│   ├── CorporateFormulas.cs         # Refactored
│   ├── GeneralFinanceFormulas.cs    # Refactored
│   ├── StocksBondsFormulas.cs       # Refactored
│   └── FinancialMarketsFormulas.cs  # Refactored
│
├── Extensions/
│   ├── DecimalExtensions.cs         # Extension methods
│   └── ResultExtensions.cs          # Result pattern helpers
│
└── Metadata/
    ├── FormulaMetadata.cs           # Formula information
    └── ParameterMetadata.cs         # Parameter constraints
```

---

## 2. Core Components Design

### 2.1 Result<T> Pattern

**Purpose**: Enable graceful error handling without exceptions

```csharp
namespace srbrettle.FinancialFormulas.Core
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }
        public ErrorContext ErrorContext { get; }

        public static Result<T> Success(T value);
        public static Result<T> Failure(string error, ErrorContext context = null);

        public TResult Match<TResult>(
            Func<T, TResult> onSuccess,
            Func<string, TResult> onFailure);
    }

    public class ErrorContext
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public string ConstraintViolated { get; set; }
        public string ValidRange { get; set; }
        public Exception InnerException { get; set; }
    }
}
```

### 2.2 Validation Framework

**Purpose**: Centralized, reusable input validation

```csharp
namespace srbrettle.FinancialFormulas.Validation
{
    public interface IValidationRule<T>
    {
        ValidationResult Validate(T value, string parameterName);
    }

    public class ParameterValidator
    {
        public static ValidationResult ValidatePositive(decimal value, string paramName);
        public static ValidationResult ValidateNonZero(decimal value, string paramName);
        public static ValidationResult ValidateNonNegative(decimal value, string paramName);
        public static ValidationResult ValidatePercentage(decimal value, string paramName);
        public static ValidationResult ValidateRange(decimal value, decimal min, decimal max, string paramName);
    }

    public class DomainValidator
    {
        public static ValidationResult ValidateLogarithmInput(decimal value, string paramName);
        public static ValidationResult ValidatePowerInput(decimal baseValue, decimal exponent, string paramName);
        public static ValidationResult ValidateDivision(decimal numerator, decimal denominator, string paramName);
        public static ValidationResult ValidateGrowthRate(decimal growthRate, decimal discountRate);
    }

    public class ValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }
        public ErrorContext Context { get; }

        public static ValidationResult Valid();
        public static ValidationResult Invalid(string error, ErrorContext context = null);
        public ValidationResult Combine(ValidationResult other);
    }
}
```

### 2.3 Decimal Mathematics

**Purpose**: High-precision calculations without double conversion

```csharp
namespace srbrettle.FinancialFormulas.Mathematics
{
    public static class DecimalMath
    {
        // Precision constants
        public const int DEFAULT_PRECISION = 28;
        public const int MAX_ITERATIONS = 100;
        public const decimal EPSILON = 0.0000000001m;

        // Core operations
        public static decimal Pow(decimal baseValue, decimal exponent);
        public static decimal Log(decimal value);
        public static decimal Log(decimal value, decimal baseValue);
        public static decimal Exp(decimal exponent);
        public static decimal Sqrt(decimal value);

        // Helper methods
        public static decimal Taylor(decimal x, Func<int, decimal> coefficient, int terms);
        public static decimal NewtonRaphson(Func<decimal, decimal> f, Func<decimal, decimal> df, decimal initialGuess);
    }

    public static class PrecisionHelper
    {
        public static decimal Round(decimal value, int decimals, MidpointRounding mode);
        public static bool AreEqual(decimal a, decimal b, decimal tolerance = 0.0001m);
        public static int GetPrecision(decimal value);
    }
}
```

---

## 3. Formula Refactoring Pattern

### 3.1 Standard Method Template

**Every formula method follows this pattern:**

```csharp
/// <summary>
/// Calculates [Formula Name] from [parameters]
/// </summary>
/// <param name="param1">[Description] (must be [constraint])</param>
/// <param name="param2">[Description] (must be [constraint])</param>
/// <returns>Decimal value for [Formula Name]</returns>
/// <exception cref="ArgumentException">Thrown when parameters violate constraints</exception>
/// <remarks>
/// Formula: [Mathematical Formula]
/// Constraints: [List all constraints]
/// Reference: [Citation if available]
/// Precision: [Expected precision level]
/// </remarks>
/// <example>
/// <code>
/// decimal ratio = FinancialFormulas.CalcCurrentRatio(1000m, 500m);
/// // Returns: 2.0
/// </code>
/// </example>
public static decimal CalcCurrentRatio(decimal currentAssets, decimal currentLiabilities)
{
    // 1. Validate inputs
    var validation = ValidateInputs(currentAssets, currentLiabilities);
    if (!validation.IsValid)
    {
        throw new ArgumentException(validation.Errors.First(), validation.Context.ParameterName);
    }

    // 2. Perform calculation
    try
    {
        return currentAssets / currentLiabilities;
    }
    catch (Exception ex)
    {
        throw new FinancialCalculationException(
            $"Error calculating current ratio: {ex.Message}", ex);
    }
}

/// <summary>
/// Attempts to calculate [Formula Name], returning Result pattern
/// </summary>
public static Result<decimal> TryCalcCurrentRatio(decimal currentAssets, decimal currentLiabilities)
{
    var validation = ValidateInputs(currentAssets, currentLiabilities);
    if (!validation.IsValid)
    {
        return Result<decimal>.Failure(validation.Errors.First(), validation.Context);
    }

    try
    {
        decimal result = currentAssets / currentLiabilities;
        return Result<decimal>.Success(result);
    }
    catch (Exception ex)
    {
        return Result<decimal>.Failure(
            $"Calculation error: {ex.Message}",
            new ErrorContext { InnerException = ex });
    }
}

// Private validation method
private static ValidationResult ValidateInputs(decimal currentAssets, decimal currentLiabilities)
{
    var result = ValidationResult.Valid();

    result = result.Combine(
        ParameterValidator.ValidateNonNegative(currentAssets, nameof(currentAssets)));

    result = result.Combine(
        ParameterValidator.ValidatePositive(currentLiabilities, nameof(currentLiabilities)));

    return result;
}
```

### 3.2 Complex Formula Pattern (with domain validation)

```csharp
public static decimal CalcPresentValueOfGrowingPerpetuity(
    decimal dividendOrCouponAtFirstPeriod,
    decimal discountRate,
    decimal growthRate)
{
    // 1. Basic parameter validation
    ParameterValidator.ValidatePositive(dividendOrCouponAtFirstPeriod, nameof(dividendOrCouponAtFirstPeriod));
    ParameterValidator.ValidatePositive(discountRate, nameof(discountRate));
    ParameterValidator.ValidateNonNegative(growthRate, nameof(growthRate));

    // 2. Domain-specific validation
    var domainValidation = DomainValidator.ValidateGrowthRate(growthRate, discountRate);
    if (!domainValidation.IsValid)
    {
        throw new ArgumentException(
            $"Growth rate ({growthRate}) must be less than discount rate ({discountRate}). " +
            "A growth rate >= discount rate results in infinite present value.",
            nameof(growthRate));
    }

    // 3. Check for division by zero
    var divisionValidation = DomainValidator.ValidateDivision(
        dividendOrCouponAtFirstPeriod,
        discountRate - growthRate,
        "discountRate - growthRate");

    if (!divisionValidation.IsValid)
    {
        throw new ArgumentException(divisionValidation.Errors.First());
    }

    // 4. Perform calculation
    return dividendOrCouponAtFirstPeriod / (discountRate - growthRate);
}
```

---

## 4. Testing Architecture

### 4.1 Test Organization

```
Test/
├── Unit/
│   ├── Formulas/
│   │   ├── FinancialFormulasTests.cs
│   │   ├── BankingFormulasTests.cs
│   │   ├── CorporateFormulasTests.cs
│   │   ├── GeneralFinanceFormulasTests.cs
│   │   ├── StocksBondsFormulasTests.cs
│   │   └── FinancialMarketsFormulasTests.cs
│   │
│   ├── Validation/
│   │   ├── ParameterValidatorTests.cs
│   │   ├── DomainValidatorTests.cs
│   │   └── ValidationRulesTests.cs
│   │
│   └── Mathematics/
│       ├── DecimalMathTests.cs
│       └── PrecisionHelperTests.cs
│
├── Integration/
│   ├── ChainedCalculationsTests.cs
│   ├── RealWorldScenariosTests.cs
│   └── PerformanceTests.cs
│
├── EdgeCases/
│   ├── BoundaryValueTests.cs
│   ├── NegativeInputTests.cs
│   ├── ZeroInputTests.cs
│   ├── ExtremeValueTests.cs
│   └── PrecisionTests.cs
│
└── TestData/
    ├── StandardTestCases.json
    ├── EdgeCaseTestCases.json
    └── RealWorldTestCases.json
```

### 4.2 Test Data Structure

```csharp
public class FormulaTestCase
{
    public string FormulaName { get; set; }
    public Dictionary<string, decimal> Inputs { get; set; }
    public decimal ExpectedOutput { get; set; }
    public decimal Tolerance { get; set; } = 0.0001m;
    public string Description { get; set; }
    public string Reference { get; set; }
}

public class ValidationTestCase
{
    public string FormulaName { get; set; }
    public Dictionary<string, decimal> Inputs { get; set; }
    public bool ShouldSucceed { get; set; }
    public string ExpectedErrorMessage { get; set; }
    public string ExpectedParameterName { get; set; }
}
```

### 4.3 Test Categories

**Category 1: Valid Input Tests**
- Standard business scenarios
- Verify mathematical correctness
- Compare against authoritative sources

**Category 2: Invalid Input Tests**
- Negative values where positive required
- Zero values causing division errors
- Out-of-range percentages
- Domain violations (log of negative, etc.)

**Category 3: Boundary Tests**
- Maximum/minimum valid values
- Values approaching constraints
- Edge of valid ranges

**Category 4: Precision Tests**
- Verify decimal precision maintained
- Compare with double-based calculations
- Accumulated rounding error tests

**Category 5: Performance Tests**
- Benchmark calculation speed
- Memory usage profiling
- Concurrent execution tests

---

## 5. Implementation Strategy

### 5.1 Phase 1: Foundation (Days 1-3)

**Day 1: Core Infrastructure**
- Implement Result<T> pattern
- Implement ValidationResult
- Implement ErrorContext
- Create custom exceptions

**Day 2: Validation Framework**
- Implement ParameterValidator
- Implement DomainValidator
- Implement validation rules
- Create unit tests for validators

**Day 3: Mathematics Library**
- Implement DecimalMath.Pow
- Implement DecimalMath.Log
- Implement DecimalMath.Exp
- Create precision tests

### 5.2 Phase 2: Formula Refactoring (Days 4-10)

**Day 4-5: FinancialFormulas.cs (46 methods)**
- Refactor Activity formulas (12 methods)
- Refactor Basic formulas (8 methods)
- Refactor Debt formulas (4 methods)
- Refactor Depreciation formulas (4 methods)
- Add validation and tests

**Day 6: BankingFormulas.cs (15 methods)**
- Refactor all banking formulas
- Fix compound interest precision
- Add validation and tests

**Day 7: CorporateFormulas.cs (19 methods)**
- Refactor corporate formulas
- Fix NPV calculation
- Add validation and tests

**Day 8: GeneralFinanceFormulas.cs (30 methods)**
- Refactor annuity formulas
- Refactor present/future value formulas
- Add validation and tests

**Day 9: StocksBondsFormulas.cs (34 methods)**
- Refactor stock formulas
- Refactor bond formulas
- Add validation and tests

**Day 10: FinancialMarketsFormulas.cs (2 methods)**
- Refactor markets formulas
- Final integration
- Add validation and tests

### 5.3 Phase 3: Testing (Days 11-14)

**Day 11-12: Unit Tests**
- Write 1000+ unit test cases
- Cover all valid scenarios
- Cover all invalid scenarios
- Achieve 95%+ coverage

**Day 13: Integration & Edge Cases**
- Write integration tests
- Write edge case tests
- Property-based testing
- Precision validation

**Day 14: Performance & Benchmarking**
- Performance benchmarks
- Memory profiling
- Optimization passes
- Final test suite run

### 5.4 Phase 4: Documentation (Days 15-16)

**Day 15: API Documentation**
- Complete XML documentation
- Generate API reference
- Create usage examples
- Migration guide

**Day 16: Final Polish**
- Code review
- Documentation review
- Package preparation
- Release checklist

---

## 6. Quality Gates

### 6.1 Code Quality Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Code Coverage | ≥ 95% | Unit + Integration tests |
| Cyclomatic Complexity | ≤ 10 | Per method |
| Method Length | ≤ 50 lines | Excluding documentation |
| Documentation Coverage | 100% | All public APIs |
| Zero Warnings | 0 | Compiler + Analyzer |

### 6.2 Performance Metrics

| Operation | Target | Baseline |
|-----------|--------|----------|
| Simple calculation | < 1ms | TBD |
| Complex calculation | < 10ms | TBD |
| Validation overhead | < 10% | vs unvalidated |
| Memory footprint | < 5MB | Library size |

### 6.3 Validation Metrics

| Metric | Target |
|--------|--------|
| Division by zero prevented | 100% |
| Domain errors prevented | 100% |
| Invalid inputs rejected | 100% |
| Error messages clarity | Manual review |

---

## 7. Dependencies

### 7.1 Production Dependencies
- None (zero external dependencies for core library)

### 7.2 Development Dependencies
- xUnit (testing framework)
- xUnit.runner.visualstudio (test runner)
- Microsoft.NET.Test.Sdk (test SDK)
- FluentAssertions (test assertions)
- Benchmark.NET (performance testing)

### 7.3 Build Dependencies
- .NET SDK 6.0+ (cross-platform)
- NuGet (package management)

---

## 8. Deployment Strategy

### 8.1 Versioning
- **v2.0.0** - Major version (breaking changes)
- Semantic versioning: MAJOR.MINOR.PATCH
- Maintain v1.x for backward compatibility period

### 8.2 Package Distribution
- NuGet.org primary distribution
- GitHub Packages mirror
- Strong-named assembly for enterprise
- Symbols package for debugging

### 8.3 Release Checklist
- [ ] All tests passing
- [ ] Code coverage ≥ 95%
- [ ] Documentation complete
- [ ] Performance benchmarks acceptable
- [ ] NuGet package created
- [ ] Release notes generated
- [ ] Git tag created
- [ ] GitHub release published

---

## 9. Monitoring & Maintenance

### 9.1 Telemetry (Optional)
- No telemetry in core library
- Users can add custom logging
- Diagnostic events for debugging

### 9.2 Issue Tracking
- GitHub Issues for bug reports
- Templates for bug/feature requests
- Automated issue labeling
- Response SLA: 48 hours

### 9.3 Maintenance Plan
- Monthly dependency updates
- Quarterly feature releases
- Security patches as needed
- Annual major version review

---

## 10. Success Criteria

### 10.1 Technical Success
- ✅ Zero runtime exceptions from invalid inputs
- ✅ 95%+ test coverage achieved
- ✅ All formulas mathematically verified
- ✅ Performance within 10% of v1.x
- ✅ Backward compatible API

### 10.2 Process Success
- ✅ Delivered on time (16 days)
- ✅ All quality gates passed
- ✅ Documentation complete
- ✅ Automated CI/CD pipeline
- ✅ NuGet package published

---

**Document Version**: 1.0
**Date**: 2025-10-09
**Status**: APPROVED - READY FOR EXECUTION
