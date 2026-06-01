# Financial Formulas Library v2.0 - API Documentation

## Table of Contents

1. [Getting Started](#getting-started)
2. [Core Components](#core-components)
3. [Validation Framework](#validation-framework)
4. [Mathematics Library](#mathematics-library)
5. [Banking Formulas](#banking-formulas)
6. [Financial Markets Formulas](#financial-markets-formulas)
7. [Result Pattern Usage](#result-pattern-usage)
8. [Error Handling](#error-handling)
9. [Migration Guide (v1.x to v2.0)](#migration-guide-v1x-to-v20)
10. [Examples](#examples)

---

## Getting Started

### Installation

#### Using NuGet
```powershell
Install-Package FinancialFormulas -Version 2.0.0
```

Or via .NET CLI:
```bash
dotnet add package FinancialFormulas --version 2.0.0
```

### Basic Usage

```csharp
using srbrettle.FinancialFormulas;
using srbrettle.FinancialFormulas.Core;
using srbrettle.FinancialFormulas.Validation;
using srbrettle.FinancialFormulas.Mathematics;

// Exception-based approach (traditional)
decimal payment = BankingFormulas.CalcLoanPayment(100000m, 0.05m / 12, 360);

// Result-based approach (functional)
Result<decimal> result = BankingFormulas.TryCalcLoanPayment(100000m, 0.05m / 12, 360);
if (result.IsSuccess)
{
    Console.WriteLine($"Monthly Payment: {result.Value:C}");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### Namespace Organization

- **srbrettle.FinancialFormulas** - Main formula classes (BankingFormulas, FinancialFormulas, etc.)
- **srbrettle.FinancialFormulas.Core** - Result pattern and error handling
- **srbrettle.FinancialFormulas.Validation** - Input validation utilities
- **srbrettle.FinancialFormulas.Mathematics** - High-precision decimal math operations

---

## Core Components

### Result<T>

The `Result<T>` class implements the Result pattern for functional error handling, providing a type-safe way to handle success and failure states.

#### Purpose
- Eliminate exceptions for expected error cases
- Make error handling explicit and type-safe
- Enable functional programming patterns (Map, Bind, Match)
- Provide detailed error context

#### Properties

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }        // True if operation succeeded
    public bool IsFailure { get; }        // True if operation failed
    public T Value { get; }               // Success value (throws if IsFailure)
    public string Error { get; }          // Error message (null if IsSuccess)
    public ErrorContext ErrorContext { get; } // Detailed error information
}
```

#### Creating Results

```csharp
// Success
var success = Result<decimal>.Success(42.5m);

// Failure
var failure = Result<decimal>.Failure("Invalid input");

// Failure with context
var context = new ErrorContext
{
    ParameterName = "rate",
    ParameterValue = -0.05m,
    ConstraintViolated = "Rate must be non-negative",
    ValidRange = "[0, +∞)"
};
var failure = Result<decimal>.Failure("Rate cannot be negative", context);
```

#### Pattern Matching

```csharp
// Functional pattern matching with return value
string message = result.Match(
    onSuccess: value => $"Calculated value: {value:C}",
    onFailure: error => $"Calculation failed: {error}"
);

// Pattern matching with side effects
result.Match(
    onSuccess: value => Console.WriteLine($"Success: {value}"),
    onFailure: error => Console.WriteLine($"Error: {error}")
);
```

#### Map and Bind

```csharp
// Map: Transform success value
Result<decimal> amount = Result<decimal>.Success(1000m);
Result<string> formatted = amount.Map(val => $"${val:F2}");
// formatted.Value == "$1000.00"

// Bind: Chain operations that return Results
Result<decimal> principal = Result<decimal>.Success(10000m);
Result<decimal> interest = principal.Bind(p =>
    BankingFormulas.TryCalcSimpleInterest(p, 0.05m, 1m)
);
```

#### Full Example

```csharp
Result<decimal> CalculateLoanTotal(decimal principal, decimal rate, decimal years)
{
    return BankingFormulas.TryCalcSimpleInterest(principal, rate, years)
        .Map(interest => principal + interest);
}

var result = CalculateLoanTotal(10000m, 0.05m, 5m);
result.Match(
    onSuccess: total => Console.WriteLine($"Total amount: {total:C}"),
    onFailure: error => Console.WriteLine($"Calculation error: {error}")
);
```

---

### ErrorContext

The `ErrorContext` class provides detailed diagnostic information about errors.

#### Purpose
- Capture comprehensive error details
- Provide debugging information
- Enable better error messages
- Track validation failures

#### Properties

```csharp
public class ErrorContext
{
    public string ParameterName { get; set; }        // Name of problematic parameter
    public object ParameterValue { get; set; }       // Value that caused error
    public string ConstraintViolated { get; set; }   // Description of constraint
    public string ValidRange { get; set; }           // Valid range/values
    public string FormulaName { get; set; }          // Formula that failed
    public Exception InnerException { get; set; }    // Underlying exception
    public string AdditionalInfo { get; set; }       // Extra diagnostic info
}
```

#### Usage Examples

```csharp
// Creating error context
var context = new ErrorContext
{
    ParameterName = "interestRate",
    ParameterValue = -0.05m,
    ConstraintViolated = "Interest rate must be non-negative",
    ValidRange = "[0, +∞)",
    FormulaName = "Simple Interest",
    AdditionalInfo = "Negative rates are not supported"
};

// Generating detailed messages
string detailedMessage = context.ToDetailedMessage();
/* Output:
Formula: Simple Interest
Parameter: interestRate
Value: -0.05
Constraint: Interest rate must be non-negative
Valid Range: [0, +∞)
Additional Info: Negative rates are not supported
*/

// Accessing from Result
Result<decimal> result = BankingFormulas.TryCalcSimpleInterest(-100m, 0.05m, 1m);
if (result.IsFailure && result.ErrorContext != null)
{
    Console.WriteLine(result.ErrorContext.ToDetailedMessage());
}
```

---

### ValidationResult

The `ValidationResult` class represents the outcome of validation operations and can combine multiple validation checks.

#### Purpose
- Track validation success/failure
- Aggregate multiple validation errors
- Provide error context
- Enable validation composition

#### Properties

```csharp
public class ValidationResult
{
    public bool IsValid { get; }                        // True if validation passed
    public IReadOnlyList<string> Errors { get; }        // List of error messages
    public ErrorContext Context { get; }                // Context for first error
    public string FirstError { get; }                   // First error message
    public string AllErrors { get; }                    // All errors combined
}
```

#### Creating ValidationResults

```csharp
// Valid result
var valid = ValidationResult.Valid();

// Invalid with single error
var invalid = ValidationResult.Invalid("Value must be positive");

// Invalid with multiple errors
var errors = new[] { "Error 1", "Error 2", "Error 3" };
var invalid = ValidationResult.Invalid(errors);

// Invalid with error context
var context = new ErrorContext
{
    ParameterName = "amount",
    ParameterValue = -100m
};
var invalid = ValidationResult.Invalid("Amount must be positive", context);
```

#### Combining Validations

```csharp
// Combine multiple validation results
var validation1 = ParameterValidator.ValidatePositive(amount, nameof(amount));
var validation2 = ParameterValidator.ValidatePositive(rate, nameof(rate));
var validation3 = ParameterValidator.ValidatePositive(time, nameof(time));

var combined = validation1
    .Combine(validation2)
    .Combine(validation3);

if (!combined.IsValid)
{
    Console.WriteLine($"Validation failed: {combined.AllErrors}");
}
```

#### ThrowIfInvalid

```csharp
// Throw exception if validation fails
var validation = ParameterValidator.ValidatePositive(value, "value");
validation.ThrowIfInvalid(); // Throws ArgumentException if invalid
```

#### Usage Examples

```csharp
// Example 1: Single validation
ValidationResult ValidateLoanAmount(decimal amount)
{
    if (amount < 1000m || amount > 1000000m)
    {
        return ValidationResult.Invalid(
            "Loan amount must be between $1,000 and $1,000,000",
            new ErrorContext
            {
                ParameterName = "amount",
                ParameterValue = amount,
                ValidRange = "[1000, 1000000]"
            }
        );
    }
    return ValidationResult.Valid();
}

// Example 2: Multiple validations
ValidationResult ValidateLoanParameters(decimal amount, decimal rate, decimal years)
{
    var result = ParameterValidator.ValidatePositive(amount, nameof(amount));
    result = result.Combine(ParameterValidator.ValidateRange(rate, 0m, 1m, nameof(rate)));
    result = result.Combine(ParameterValidator.ValidateRange(years, 1m, 30m, nameof(years)));
    return result;
}

// Example 3: Using validation
var validation = ValidateLoanParameters(10000m, 0.05m, 15m);
if (validation.IsValid)
{
    // Proceed with calculation
}
else
{
    Console.WriteLine($"Validation errors: {validation.AllErrors}");
}
```

---

## Validation Framework

### ParameterValidator

Static class providing standard parameter validation methods for financial calculations.

#### ValidatePositive

Validates that a value is greater than zero.

```csharp
public static ValidationResult ValidatePositive(decimal value, string paramName)
```

**Parameters:**
- `value`: The value to validate
- `paramName`: The name of the parameter

**Returns:** `ValidationResult` indicating success or failure

**Valid Range:** (0, +∞)

**Example:**
```csharp
var validation = ParameterValidator.ValidatePositive(100m, "principal");
if (!validation.IsValid)
{
    Console.WriteLine(validation.FirstError);
    // Output: "Parameter 'principal' must be positive. Provided value: 0"
}
```

#### ValidateNonNegative

Validates that a value is greater than or equal to zero.

```csharp
public static ValidationResult ValidateNonNegative(decimal value, string paramName)
```

**Parameters:**
- `value`: The value to validate
- `paramName`: The name of the parameter

**Returns:** `ValidationResult` indicating success or failure

**Valid Range:** [0, +∞)

**Example:**
```csharp
var validation = ParameterValidator.ValidateNonNegative(0m, "balance");
// Returns: Valid (0 is acceptable)

var validation2 = ParameterValidator.ValidateNonNegative(-10m, "balance");
// Returns: Invalid - "Parameter 'balance' must be non-negative. Provided value: -10"
```

#### ValidateNonZero

Validates that a value is not equal to zero.

```csharp
public static ValidationResult ValidateNonZero(decimal value, string paramName)
```

**Parameters:**
- `value`: The value to validate
- `paramName`: The name of the parameter

**Returns:** `ValidationResult` indicating success or failure

**Valid Range:** (-∞, 0) ∪ (0, +∞)

**Example:**
```csharp
var validation = ParameterValidator.ValidateNonZero(0m, "divisor");
// Returns: Invalid - "Parameter 'divisor' must be non-zero. Zero is not allowed."
```

#### ValidatePercentage

Validates that a value represents a valid percentage (between 0 and 1).

```csharp
public static ValidationResult ValidatePercentage(decimal value, string paramName)
```

**Parameters:**
- `value`: The value to validate (in decimal format, e.g., 0.05 for 5%)
- `paramName`: The name of the parameter

**Returns:** `ValidationResult` indicating success or failure

**Valid Range:** [0, 1]

**Example:**
```csharp
var validation = ParameterValidator.ValidatePercentage(0.05m, "interestRate");
// Returns: Valid

var validation2 = ParameterValidator.ValidatePercentage(5m, "interestRate");
// Returns: Invalid - "Parameter 'interestRate' must be a valid percentage between 0 and 1"
// Note: Use 0.05 for 5%, not 5
```

#### ValidateRange

Validates that a value falls within a specified range (inclusive).

```csharp
public static ValidationResult ValidateRange(decimal value, decimal min, decimal max, string paramName)
```

**Parameters:**
- `value`: The value to validate
- `min`: Minimum allowed value (inclusive)
- `max`: Maximum allowed value (inclusive)
- `paramName`: The name of the parameter

**Returns:** `ValidationResult` indicating success or failure

**Valid Range:** [min, max]

**Example:**
```csharp
var validation = ParameterValidator.ValidateRange(15m, 1m, 30m, "loanYears");
// Returns: Valid

var validation2 = ParameterValidator.ValidateRange(35m, 1m, 30m, "loanYears");
// Returns: Invalid - "Parameter 'loanYears' must be between 1 and 30. Provided value: 35"
```

---

### DomainValidator

Static class providing mathematical domain validation methods.

#### ValidateLogarithmInput

Validates that a value is valid for logarithm operations (must be positive).

```csharp
public static ValidationResult ValidateLogarithmInput(decimal value, string paramName)
```

**Parameters:**
- `value`: The value to validate
- `paramName`: The name of the parameter

**Returns:** `ValidationResult` indicating success or failure

**Valid Range:** (0, +∞)

**Mathematical Background:** Logarithms are undefined for zero and negative numbers.

**Example:**
```csharp
var validation = DomainValidator.ValidateLogarithmInput(100m, "value");
// Returns: Valid

var validation2 = DomainValidator.ValidateLogarithmInput(0m, "value");
// Returns: Invalid - "Parameter 'value' must be positive for logarithm operations"
```

#### ValidatePowerInput

Validates that base and exponent values are valid for power operations.

```csharp
public static ValidationResult ValidatePowerInput(decimal baseValue, decimal exponent, string paramName)
```

**Parameters:**
- `baseValue`: The base value
- `exponent`: The exponent value
- `paramName`: The name of the parameter

**Returns:** `ValidationResult` indicating success or failure

**Valid Combinations:**
- Base > 0: Any exponent allowed
- Base = 0: Positive exponent only
- Base < 0: Integer exponent only

**Example:**
```csharp
var validation1 = DomainValidator.ValidatePowerInput(2m, 3m, "base");
// Returns: Valid (2^3 = 8)

var validation2 = DomainValidator.ValidatePowerInput(-2m, 3m, "base");
// Returns: Valid ((-2)^3 = -8, integer exponent)

var validation3 = DomainValidator.ValidatePowerInput(-2m, 2.5m, "base");
// Returns: Invalid - "Negative base requires an integer exponent"

var validation4 = DomainValidator.ValidatePowerInput(0m, -1m, "base");
// Returns: Invalid - "Zero base requires a positive exponent"
```

#### ValidateDivision

Validates division operation parameters (denominator must not be zero).

```csharp
public static ValidationResult ValidateDivision(decimal numerator, decimal denominator, string denominatorName)
```

**Parameters:**
- `numerator`: The numerator value
- `denominator`: The denominator value
- `denominatorName`: The name of the denominator parameter

**Returns:** `ValidationResult` indicating success or failure

**Valid Range (denominator):** (-∞, 0) ∪ (0, +∞)

**Example:**
```csharp
var validation = DomainValidator.ValidateDivision(100m, 5m, "divisor");
// Returns: Valid

var validation2 = DomainValidator.ValidateDivision(100m, 0m, "divisor");
// Returns: Invalid - "Parameter 'divisor' cannot be zero. Division by zero is undefined."
```

#### ValidateGrowthRate

Validates that growth rate is less than discount rate (required for perpetuity calculations).

```csharp
public static ValidationResult ValidateGrowthRate(decimal growthRate, decimal discountRate)
```

**Parameters:**
- `growthRate`: The growth rate
- `discountRate`: The discount rate

**Returns:** `ValidationResult` indicating success or failure

**Constraint:** growthRate < discountRate

**Mathematical Background:** For perpetuity calculations to converge, the growth rate must be strictly less than the discount rate.

**Example:**
```csharp
var validation = DomainValidator.ValidateGrowthRate(0.03m, 0.08m);
// Returns: Valid (3% < 8%)

var validation2 = DomainValidator.ValidateGrowthRate(0.08m, 0.08m);
// Returns: Invalid - "Growth rate (0.08) must be less than discount rate (0.08)"
```

---

## Mathematics Library

### DecimalMath

High-precision mathematical operations for financial calculations using decimal type exclusively. All operations avoid double conversion to maintain precision.

#### Mathematical Constants

Available in `srbrettle.FinancialFormulas.Mathematics.Constants`:

```csharp
public static class Constants
{
    public const decimal E = 2.7182818284590452353602874713527m;      // Euler's number
    public const decimal PI = 3.1415926535897932384626433832795m;     // Pi
    public const decimal LN2 = 0.69314718055994530941723212145818m;   // Natural log of 2
    public const decimal LN10 = 2.3025850929940456840179914546844m;   // Natural log of 10
    public const decimal SQRT2 = 1.4142135623730950488016887242097m;  // Square root of 2
    public const decimal PHI = 1.6180339887498948482045868343656m;    // Golden ratio
}
```

#### Pow - Power Function

Calculates base raised to the power of exponent with high precision.

```csharp
public static decimal Pow(decimal baseValue, decimal exponent)
public static decimal Pow(decimal baseValue, int exponent)  // Optimized for integers
```

**Parameters:**
- `baseValue`: The base value
- `exponent`: The exponent (can be fractional)

**Returns:** baseValue^exponent

**Exceptions:**
- `ArgumentException`: If baseValue is negative and exponent is non-integer, or if baseValue is zero and exponent is negative/zero
- `OverflowException`: If result exceeds decimal range

**Algorithm:** Uses formula base^exp = e^(exp × ln(base)) for non-integer exponents. Integer exponents use binary exponentiation for efficiency.

**Special Cases:**
- base^0 = 1 (except 0^0 which throws exception)
- base^1 = base
- 1^exp = 1
- 0^exp = 0 (for positive exp)

**Examples:**
```csharp
// Basic usage
decimal result = DecimalMath.Pow(2m, 3m);           // 8
decimal result2 = DecimalMath.Pow(1.05m, 12m);      // 1.7958563260221...

// Fractional exponents
decimal sqrt = DecimalMath.Pow(16m, 0.5m);          // 4 (square root)
decimal cubeRoot = DecimalMath.Pow(27m, 1m/3m);     // 3 (cube root)

// Negative exponents
decimal reciprocal = DecimalMath.Pow(2m, -1m);      // 0.5

// Financial calculation example
decimal futureValue = 1000m * DecimalMath.Pow(1.05m, 10m);  // Compound growth
```

**Performance Notes:**
- Integer exponents: O(log n) time complexity
- Fractional exponents: Uses Taylor series, typically converges within 50 iterations
- Precision: Maintains full 28-digit decimal precision

#### Exp - Exponential Function

Calculates e raised to the specified power.

```csharp
public static decimal Exp(decimal exponent)
```

**Parameters:**
- `exponent`: The exponent to raise e to

**Returns:** e^exponent

**Exceptions:**
- `OverflowException`: If exponent > 27 (exceeds decimal range)

**Algorithm:** Uses Taylor series expansion with range reduction for large exponents.

**Special Cases:**
- e^0 = 1
- e^1 = e (2.71828...)
- e^x ≈ 0 for x < -27

**Examples:**
```csharp
decimal e = DecimalMath.Exp(1m);                    // 2.71828...
decimal result = DecimalMath.Exp(2m);               // 7.38905...

// Continuous compounding
decimal principal = 1000m;
decimal rate = 0.05m;
decimal time = 5m;
decimal futureValue = principal * DecimalMath.Exp(rate * time);  // 1284.03
```

#### Log - Natural Logarithm

Calculates the natural logarithm (base e) of a value.

```csharp
public static decimal Log(decimal value)
public static decimal Log(decimal value, decimal baseValue)  // Custom base
```

**Parameters:**
- `value`: The value to calculate the logarithm of (must be positive)
- `baseValue`: Optional custom base (must be positive and not equal to 1)

**Returns:** ln(value) or log_base(value)

**Exceptions:**
- `ArgumentException`: If value ≤ 0, or if baseValue ≤ 0 or baseValue = 1

**Algorithm:** Uses Newton-Raphson iteration with range reduction for improved convergence.

**Special Cases:**
- ln(1) = 0
- ln(e) = 1
- log_b(1) = 0
- log_b(b) = 1

**Examples:**
```csharp
// Natural logarithm
decimal ln = DecimalMath.Log(2.71828m);             // ≈ 1
decimal ln2 = DecimalMath.Log(10m);                 // ≈ 2.302585

// Custom base logarithm
decimal log10 = DecimalMath.Log(100m, 10m);         // 2
decimal log2 = DecimalMath.Log(8m, 2m);             // 3

// Solving for time in compound interest
decimal futureValue = 2000m;
decimal presentValue = 1000m;
decimal rate = 0.05m;
decimal time = DecimalMath.Log(futureValue / presentValue) / DecimalMath.Log(1 + rate);
// How long to double money at 5%: ≈ 14.2 years
```

#### Log10 and Log2

Base 10 and base 2 logarithms.

```csharp
public static decimal Log10(decimal value)
public static decimal Log2(decimal value)
```

**Examples:**
```csharp
decimal log10 = DecimalMath.Log10(1000m);           // 3
decimal log2 = DecimalMath.Log2(1024m);             // 10
```

#### Sqrt - Square Root

Calculates the square root of a value using Newton-Raphson method.

```csharp
public static decimal Sqrt(decimal value)
```

**Parameters:**
- `value`: The value to calculate the square root of (must be non-negative)

**Returns:** √value

**Exceptions:**
- `ArgumentException`: If value < 0

**Algorithm:** Newton-Raphson iteration: x(n+1) = (x(n) + value/x(n)) / 2

**Special Cases:**
- √0 = 0
- √1 = 1

**Examples:**
```csharp
decimal sqrt = DecimalMath.Sqrt(16m);               // 4
decimal sqrt2 = DecimalMath.Sqrt(2m);               // 1.414213562373095...

// Standard deviation calculation
decimal variance = 100m;
decimal stdDev = DecimalMath.Sqrt(variance);        // 10
```

#### NthRoot and Cbrt

Calculate nth root and cube root.

```csharp
public static decimal NthRoot(decimal value, int n)
public static decimal Cbrt(decimal value)
```

**Parameters:**
- `value`: The value to find the root of
- `n`: The degree of the root (must not be zero)

**Returns:** n√value

**Exceptions:**
- `ArgumentException`: If n = 0, or if value < 0 and n is even

**Examples:**
```csharp
decimal cubeRoot = DecimalMath.NthRoot(27m, 3);     // 3
decimal fourthRoot = DecimalMath.NthRoot(16m, 4);   // 2
decimal cbrt = DecimalMath.Cbrt(8m);                // 2

// Geometric mean
decimal[] values = { 2m, 8m, 32m };
decimal product = values[0] * values[1] * values[2];
decimal geometricMean = DecimalMath.NthRoot(product, values.Length);  // 8
```

#### Basic Operations

Standard mathematical operations with decimal precision.

```csharp
public static decimal Abs(decimal value)           // Absolute value
public static decimal Floor(decimal value)         // Round down
public static decimal Ceiling(decimal value)       // Round up
public static decimal Round(decimal value)         // Round to nearest
public static decimal Round(decimal value, int decimals)  // Round to decimals
public static decimal Max(decimal value1, decimal value2) // Maximum
public static decimal Min(decimal value1, decimal value2) // Minimum
```

**Examples:**
```csharp
decimal abs = DecimalMath.Abs(-5.5m);               // 5.5
decimal floor = DecimalMath.Floor(5.7m);            // 5
decimal ceiling = DecimalMath.Ceiling(5.3m);        // 6
decimal rounded = DecimalMath.Round(5.567m, 2);     // 5.57
decimal max = DecimalMath.Max(5m, 10m);             // 10
decimal min = DecimalMath.Min(5m, 10m);             // 5
```

#### Precision and Performance

**Precision:**
- All operations maintain 28-digit decimal precision
- No conversion to/from double types
- Suitable for financial calculations requiring exact decimal arithmetic

**Performance Characteristics:**
- **Pow (integer exponent):** O(log n) - very fast
- **Pow (fractional exponent):** ~50-100 iterations typical
- **Exp:** ~20-50 iterations typical
- **Log:** ~30-60 iterations typical
- **Sqrt:** ~10-20 iterations typical

**Convergence:**
- Maximum iterations: 100
- Convergence threshold (epsilon): 0.0000000001m
- All iterative methods guaranteed to converge or throw exception

---

## Banking Formulas

The `BankingFormulas` class provides 14 essential banking and loan calculation methods. Each method has both exception-throwing and Result-pattern variants.

### Method Summary

| Method | Purpose | Formula |
|--------|---------|---------|
| CalcAnnualPercentageYield | APY from stated rate | (1 + r/n)^n - 1 |
| CalcBalloonLoanPayment | Payment for balloon loan | (PV - B/(1+r)^n) × r/(1-(1+r)^-n) |
| CalcCompoundInterest | Compound interest earned | P × ((1+r)^n - 1) |
| CalcContinuousCompounding | Continuously compounded value | P × e^(rt) |
| CalcDebtToIncomeRatio | DTI ratio | Monthly Debt / Gross Income |
| CalcBalloonBalanceOfLoan | Remaining balloon balance | PV×(1+r)^n - PMT×((1+r)^n-1)/r |
| CalcLoanPayment | Periodic loan payment | (r × PV) / (1 - (1+r)^-n) |
| CalcRemainingBalanceOnLoan | Remaining loan balance | PV×(1+r)^n - PMT×((1+r)^n-1)/r |
| CalcLoanToDepositRatio | L/D ratio | Loans / Deposits |
| CalcLoanToValueRatio | LTV ratio | Loan / Collateral Value |
| CalcSimpleInterest | Simple interest earned | P × r × t |
| CalcSimpleInterestRate | Calculate rate from interest | I / (P × t) |
| CalcSimpleInterestPrincipal | Calculate principal | I / (r × t) |
| CalcSimpleInterestTime | Calculate time period | I / (r × P) |

---

### CalcAnnualPercentageYield

Calculates Annual Percentage Yield (APY) from stated annual interest rate and compounding frequency.

**Formula:** APY = (1 + r/n)^n - 1

**Method Signature:**
```csharp
public static decimal CalcAnnualPercentageYield(
    decimal statedAnnualInterestRate,
    decimal numberOfTimesCompounded)

public static Result<decimal> TryCalcAnnualPercentageYield(
    decimal statedAnnualInterestRate,
    decimal numberOfTimesCompounded)
```

**Parameters:**
- `statedAnnualInterestRate`: Stated annual interest rate (must be non-negative, typically 0-1 for 0-100%)
- `numberOfTimesCompounded`: Number of times interest compounds per year (must be positive)

**Parameter Constraints:**
- statedAnnualInterestRate ≥ 0
- numberOfTimesCompounded > 0

**Returns:** The effective annual percentage yield as a decimal (e.g., 0.0512 represents 5.12%)

**Example:**
```csharp
// Exception-based
decimal apy = BankingFormulas.CalcAnnualPercentageYield(0.05m, 12m);
// Returns: 0.05116... (5.116% effective rate with monthly compounding)

// Result-based
Result<decimal> result = BankingFormulas.TryCalcAnnualPercentageYield(0.05m, 12m);
if (result.IsSuccess)
{
    Console.WriteLine($"APY: {result.Value:P2}");  // APY: 5.12%
}

// Quarterly compounding
decimal apy2 = BankingFormulas.CalcAnnualPercentageYield(0.06m, 4m);
// Returns: 0.06136... (6.136%)

// Daily compounding
decimal apy3 = BankingFormulas.CalcAnnualPercentageYield(0.05m, 365m);
// Returns: 0.05127... (5.127%)
```

**Common Use Cases:**
- Comparing savings accounts with different compounding frequencies
- Calculating effective return on certificates of deposit (CDs)
- Determining true yield on money market accounts

---

### CalcBalloonLoanPayment

Calculates the periodic payment for a balloon loan, where a large final payment is due at maturity.

**Formula:** Payment = (PV - BalloonAmount / (1 + r)^n) × (r / (1 - (1 + r)^-n))

**Method Signature:**
```csharp
public static decimal CalcBalloonLoanPayment(
    decimal presentValue,
    decimal balloonAmount,
    decimal ratePerPeriod,
    decimal numberOfPeriods)

public static Result<decimal> TryCalcBalloonLoanPayment(
    decimal presentValue,
    decimal balloonAmount,
    decimal ratePerPeriod,
    decimal numberOfPeriods)
```

**Parameters:**
- `presentValue`: Initial loan amount (must be non-negative)
- `balloonAmount`: Final balloon payment amount (must be non-negative)
- `ratePerPeriod`: Interest rate per payment period (must be non-negative)
- `numberOfPeriods`: Total number of payment periods (must be positive)

**Parameter Constraints:**
- presentValue ≥ 0
- balloonAmount ≥ 0
- ratePerPeriod ≥ 0
- numberOfPeriods > 0

**Returns:** The periodic payment amount (excluding the final balloon payment)

**Example:**
```csharp
// 5-year balloon loan: $200,000 loan, $50,000 balloon, 6% annual rate, monthly payments
decimal monthlyPayment = BankingFormulas.CalcBalloonLoanPayment(
    200000m,        // Loan amount
    50000m,         // Balloon payment
    0.06m / 12m,    // Monthly rate (6% / 12)
    60m             // 5 years × 12 months
);
// Returns: ≈$1,069.72 monthly payment

// Result-based approach
Result<decimal> result = BankingFormulas.TryCalcBalloonLoanPayment(
    200000m, 50000m, 0.06m / 12m, 60m
);
result.Match(
    onSuccess: pmt => Console.WriteLine($"Monthly payment: {pmt:C}"),
    onFailure: err => Console.WriteLine($"Error: {err}")
);

// Commercial real estate example
decimal commercialPayment = BankingFormulas.CalcBalloonLoanPayment(
    1000000m,       // $1M loan
    300000m,        // $300K balloon
    0.05m / 12m,    // 5% annual rate
    84m             // 7 years
);
```

**Common Use Cases:**
- Commercial real estate loans
- Business equipment financing
- Short-term financing with expected refinancing

---

### CalcCompoundInterest

Calculates the compound interest earned on a principal amount.

**Formula:** CI = P × ((1 + r)^n - 1)

**Method Signature:**
```csharp
public static decimal CalcCompoundInterest(
    decimal principal,
    decimal ratePerPeriod,
    decimal numberOfPeriods)

public static Result<decimal> TryCalcCompoundInterest(
    decimal principal,
    decimal ratePerPeriod,
    decimal numberOfPeriods)
```

**Parameters:**
- `principal`: Initial principal amount (must be non-negative)
- `ratePerPeriod`: Interest rate per compounding period (must be non-negative)
- `numberOfPeriods`: Number of compounding periods (must be non-negative)

**Parameter Constraints:**
- principal ≥ 0
- ratePerPeriod ≥ 0
- numberOfPeriods ≥ 0

**Returns:** The total compound interest earned (not including principal)

**Example:**
```csharp
// Interest on $10,000 at 5% for 10 years, compounded annually
decimal interest = BankingFormulas.CalcCompoundInterest(10000m, 0.05m, 10m);
// Returns: $6,288.95 interest earned

// Monthly compounding
decimal monthlyInterest = BankingFormulas.CalcCompoundInterest(
    10000m,         // Principal
    0.05m / 12m,    // Monthly rate
    120m            // 10 years × 12 months
);
// Returns: $6,467.75 (more interest due to more frequent compounding)

// Calculate total value (principal + interest)
decimal principal = 5000m;
decimal rate = 0.06m;
decimal years = 5m;
decimal interest = BankingFormulas.CalcCompoundInterest(principal, rate, years);
decimal totalValue = principal + interest;
Console.WriteLine($"Total value: {totalValue:C}");  // $6,691.13

// Result-based approach
Result<decimal> result = BankingFormulas.TryCalcCompoundInterest(10000m, 0.05m, 10m);
if (result.IsSuccess)
{
    Console.WriteLine($"Interest earned: {result.Value:C}");
}
```

**Common Use Cases:**
- Calculating investment growth
- Savings account interest projections
- Certificate of deposit (CD) earnings
- Retirement account growth estimates

---

### CalcContinuousCompounding

Calculates the future value with continuous compounding (theoretical maximum compounding).

**Formula:** A = P × e^(rt)

**Method Signature:**
```csharp
public static decimal CalcContinuousCompounding(
    decimal principal,
    decimal rate,
    decimal time)

public static Result<decimal> TryCalcContinuousCompounding(
    decimal principal,
    decimal rate,
    decimal time)
```

**Parameters:**
- `principal`: Initial principal amount (must be non-negative)
- `rate`: Annual interest rate (must be non-negative)
- `time`: Time in years (must be non-negative)

**Parameter Constraints:**
- principal ≥ 0
- rate ≥ 0
- time ≥ 0

**Returns:** The future value with continuous compounding

**Example:**
```csharp
// $10,000 at 5% for 10 years with continuous compounding
decimal futureValue = BankingFormulas.CalcContinuousCompounding(10000m, 0.05m, 10m);
// Returns: $16,487.21

// Compare compounding frequencies
decimal annual = 10000m * DecimalMath.Pow(1.05m, 10m);              // $16,288.95
decimal monthly = BankingFormulas.CalcCompoundInterest(10000m, 0.05m/12m, 120m) + 10000m;  // $16,470.09
decimal continuous = BankingFormulas.CalcContinuousCompounding(10000m, 0.05m, 10m);  // $16,487.21
// Continuous compounding provides theoretical maximum

// Result-based approach
Result<decimal> result = BankingFormulas.TryCalcContinuousCompounding(5000m, 0.06m, 5m);
result.Match(
    onSuccess: fv => Console.WriteLine($"Future value: {fv:C}"),
    onFailure: err => Console.WriteLine($"Error: {err}")
);
```

**Common Use Cases:**
- Theoretical maximum return calculations
- Financial modeling and analysis
- Comparing with discrete compounding methods
- Academic and research applications

---

### CalcDebtToIncomeRatio

Calculates the debt-to-income (DTI) ratio, a key metric for loan qualification.

**Formula:** DTI = Monthly Debt Payments / Gross Monthly Income

**Method Signature:**
```csharp
public static decimal CalcDebtToIncomeRatio(
    decimal monthlyDebtPayments,
    decimal grossMonthlyIncome)

public static Result<decimal> TryCalcDebtToIncomeRatio(
    decimal monthlyDebtPayments,
    decimal grossMonthlyIncome)
```

**Parameters:**
- `monthlyDebtPayments`: Total monthly debt obligations (must be non-negative)
- `grossMonthlyIncome`: Gross monthly income before taxes (must be positive)

**Parameter Constraints:**
- monthlyDebtPayments ≥ 0
- grossMonthlyIncome > 0

**Returns:** The debt-to-income ratio as a decimal (e.g., 0.36 represents 36%)

**Example:**
```csharp
// Monthly debt: $2,000, Monthly income: $6,000
decimal dti = BankingFormulas.CalcDebtToIncomeRatio(2000m, 6000m);
// Returns: 0.333... (33.3% DTI ratio)

// Check if qualified for mortgage (typically < 43%)
decimal monthlyDebt = 1500m;  // Credit cards, car loan, etc.
decimal monthlyIncome = 5000m;
Result<decimal> result = BankingFormulas.TryCalcDebtToIncomeRatio(monthlyDebt, monthlyIncome);

if (result.IsSuccess)
{
    decimal dtiPercent = result.Value * 100;
    if (dtiPercent < 43m)
    {
        Console.WriteLine($"DTI: {dtiPercent:F1}% - Likely qualified");
    }
    else
    {
        Console.WriteLine($"DTI: {dtiPercent:F1}% - May not qualify");
    }
}

// Calculate maximum affordable debt payment
decimal income = 7000m;
decimal maxDtiRatio = 0.36m;  // Target 36% DTI
decimal maxDebtPayment = income * maxDtiRatio;
Console.WriteLine($"Maximum monthly debt: {maxDebtPayment:C}");  // $2,520
```

**Common Use Cases:**
- Mortgage qualification assessment
- Personal loan eligibility
- Financial health evaluation
- Budgeting and debt management

**Industry Standards:**
- DTI < 36%: Good (preferential rates)
- DTI 36-43%: Acceptable (standard rates)
- DTI > 43%: High (may be declined or require compensating factors)

---

### CalcBalloonBalanceOfLoan

Calculates the remaining balance on a balloon loan after making regular payments.

**Formula:** Balance = PV × (1 + r)^n - Payment × ((1 + r)^n - 1) / r

**Method Signature:**
```csharp
public static decimal CalcBalloonBalanceOfLoan(
    decimal presentValue,
    decimal payment,
    decimal ratePerPayment,
    decimal numberOfPayments)

public static Result<decimal> TryCalcBalloonBalanceOfLoan(
    decimal presentValue,
    decimal payment,
    decimal ratePerPayment,
    decimal numberOfPayments)
```

**Parameters:**
- `presentValue`: Initial loan amount (must be non-negative)
- `payment`: Regular payment amount (must be non-negative)
- `ratePerPayment`: Interest rate per payment period (must be non-negative)
- `numberOfPayments`: Number of payments made (must be non-negative)

**Parameter Constraints:**
- presentValue ≥ 0
- payment ≥ 0
- ratePerPayment ≥ 0
- numberOfPayments ≥ 0

**Returns:** The remaining loan balance (balloon payment amount)

**Special Case:** If ratePerPayment = 0, uses simple subtraction: PV - Payment × n

**Example:**
```csharp
// Balloon loan: $200,000, $1,200 monthly payment, 6% annual, after 60 payments
decimal balance = BankingFormulas.CalcBalloonBalanceOfLoan(
    200000m,        // Original loan
    1200m,          // Monthly payment
    0.06m / 12m,    // Monthly rate
    60m             // 5 years of payments
);
// Returns: ≈$153,388 (balloon payment due)

// Track balance over time
decimal loan = 100000m;
decimal payment = 500m;
decimal monthlyRate = 0.05m / 12m;

for (int months = 12; months <= 60; months += 12)
{
    decimal balance = BankingFormulas.CalcBalloonBalanceOfLoan(
        loan, payment, monthlyRate, months
    );
    Console.WriteLine($"After {months} months: {balance:C}");
}

// Result-based approach with error handling
Result<decimal> result = BankingFormulas.TryCalcBalloonBalanceOfLoan(
    200000m, 1200m, 0.06m / 12m, 60m
);
result.Match(
    onSuccess: bal => Console.WriteLine($"Balloon payment: {bal:C}"),
    onFailure: err => Console.WriteLine($"Calculation error: {err}")
);
```

**Common Use Cases:**
- Determining balloon payment amount
- Refinancing planning
- Loan amortization analysis
- Financial statement preparation

---

### CalcLoanPayment

Calculates the periodic payment for a fully amortizing loan.

**Formula:** Payment = (r × PV) / (1 - (1 + r)^-n)

**Method Signature:**
```csharp
public static decimal CalcLoanPayment(
    decimal presentValue,
    decimal ratePerPeriod,
    decimal numberOfPeriods)

public static Result<decimal> TryCalcLoanPayment(
    decimal presentValue,
    decimal ratePerPeriod,
    decimal numberOfPeriods)
```

**Parameters:**
- `presentValue`: Loan amount (must be non-negative)
- `ratePerPeriod`: Interest rate per payment period (must be non-negative)
- `numberOfPeriods`: Total number of payment periods (must be positive)

**Parameter Constraints:**
- presentValue ≥ 0
- ratePerPeriod ≥ 0
- numberOfPeriods > 0

**Returns:** The periodic payment amount

**Special Case:** If ratePerPeriod = 0, uses simple division: PV / n

**Example:**
```csharp
// 30-year mortgage: $300,000 at 6.5% annual rate, monthly payments
decimal monthlyPayment = BankingFormulas.CalcLoanPayment(
    300000m,        // Loan amount
    0.065m / 12m,   // Monthly rate (6.5% / 12)
    360m            // 30 years × 12 months
);
// Returns: $1,896.20

// Auto loan: $25,000 at 4% for 5 years
decimal autoPayment = BankingFormulas.CalcLoanPayment(
    25000m,         // Loan amount
    0.04m / 12m,    // Monthly rate
    60m             // 5 years
);
// Returns: $460.41

// Compare loan terms
decimal loan = 200000m;
decimal annualRate = 0.05m;

decimal payment15yr = BankingFormulas.CalcLoanPayment(loan, annualRate/12m, 180m);
decimal payment30yr = BankingFormulas.CalcLoanPayment(loan, annualRate/12m, 360m);

Console.WriteLine($"15-year: {payment15yr:C}/month, Total: {payment15yr * 180:C}");
Console.WriteLine($"30-year: {payment30yr:C}/month, Total: {payment30yr * 360:C}");

// Result-based with validation
Result<decimal> result = BankingFormulas.TryCalcLoanPayment(
    300000m, 0.065m / 12m, 360m
);
if (result.IsSuccess)
{
    decimal annualPayment = result.Value * 12;
    Console.WriteLine($"Monthly: {result.Value:C}, Annual: {annualPayment:C}");
}
```

**Common Use Cases:**
- Mortgage payment calculation
- Auto loan payments
- Personal loan amortization
- Affordability analysis

---

### CalcRemainingBalanceOnLoan

Calculates the remaining principal balance on a loan after making payments.

**Formula:** Balance = PV × (1 + r)^n - Payment × ((1 + r)^n - 1) / r

**Method Signature:**
```csharp
public static decimal CalcRemainingBalanceOnLoan(
    decimal presentValue,
    decimal payment,
    decimal ratePerPayment,
    decimal numberOfPayments)

public static Result<decimal> TryCalcRemainingBalanceOnLoan(
    decimal presentValue,
    decimal payment,
    decimal ratePerPayment,
    decimal numberOfPayments)
```

**Parameters:**
- `presentValue`: Original loan amount (must be non-negative)
- `payment`: Regular payment amount (must be non-negative)
- `ratePerPayment`: Interest rate per payment period (must be non-negative)
- `numberOfPayments`: Number of payments made (must be non-negative)

**Parameter Constraints:**
- presentValue ≥ 0
- payment ≥ 0
- ratePerPayment ≥ 0
- numberOfPayments ≥ 0

**Returns:** The remaining principal balance

**Special Case:** If ratePerPayment = 0, uses simple subtraction: PV - Payment × n

**Example:**
```csharp
// After 5 years on 30-year mortgage
decimal originalLoan = 300000m;
decimal monthlyPayment = 1896.20m;
decimal monthlyRate = 0.065m / 12m;
decimal paymentsMade = 60m;  // 5 years

decimal balance = BankingFormulas.CalcRemainingBalanceOnLoan(
    originalLoan, monthlyPayment, monthlyRate, paymentsMade
);
// Returns: ≈$280,073 remaining

// Generate amortization schedule
decimal loan = 100000m;
decimal payment = 843.86m;
decimal rate = 0.05m / 12m;

Console.WriteLine("Year\tBalance\t\tPrincipal Paid");
for (int year = 1; year <= 15; year++)
{
    decimal balance = BankingFormulas.CalcRemainingBalanceOnLoan(
        loan, payment, rate, year * 12m
    );
    decimal principalPaid = loan - balance;
    Console.WriteLine($"{year}\t{balance:C}\t{principalPaid:C}");
}

// Calculate equity in home
decimal homeValue = 350000m;
decimal mortgageBalance = BankingFormulas.CalcRemainingBalanceOnLoan(
    300000m, 1896.20m, 0.065m / 12m, 60m
);
decimal equity = homeValue - mortgageBalance;
Console.WriteLine($"Home equity: {equity:C}");  // $69,927

// Result-based approach
Result<decimal> result = BankingFormulas.TryCalcRemainingBalanceOnLoan(
    300000m, 1896.20m, 0.065m / 12m, 60m
);
result.Match(
    onSuccess: bal => Console.WriteLine($"Remaining balance: {bal:C}"),
    onFailure: err => Console.WriteLine($"Error: {err}")
);
```

**Common Use Cases:**
- Loan payoff calculations
- Refinancing analysis
- Home equity determination
- Amortization schedules

---

### CalcLoanToDepositRatio

Calculates the loan-to-deposit ratio, a key banking liquidity metric.

**Formula:** LDR = Loans / Deposits

**Method Signature:**
```csharp
public static decimal CalcLoanToDepositRatio(
    decimal loans,
    decimal deposits)

public static Result<decimal> TryCalcLoanToDepositRatio(
    decimal loans,
    decimal deposits)
```

**Parameters:**
- `loans`: Total loans outstanding (must be non-negative)
- `deposits`: Total deposits (must be positive)

**Parameter Constraints:**
- loans ≥ 0
- deposits > 0

**Returns:** The loan-to-deposit ratio as a decimal (e.g., 0.85 represents 85%)

**Example:**
```csharp
// Bank with $500M loans and $600M deposits
decimal ldr = BankingFormulas.CalcLoanToDepositRatio(500000000m, 600000000m);
// Returns: 0.833... (83.3% LDR)

// Assess bank liquidity
Result<decimal> result = BankingFormulas.TryCalcLoanToDepositRatio(
    450000000m, 500000000m
);
if (result.IsSuccess)
{
    decimal ldrPercent = result.Value * 100;
    string assessment = ldrPercent < 80 ? "Conservative" :
                        ldrPercent < 90 ? "Moderate" : "Aggressive";
    Console.WriteLine($"LDR: {ldrPercent:F1}% - {assessment} lending");
}

// Track ratio over time
decimal[,] data = {
    { 400000000m, 500000000m },  // Q1
    { 425000000m, 520000000m },  // Q2
    { 450000000m, 530000000m },  // Q3
    { 475000000m, 550000000m }   // Q4
};

for (int q = 0; q < 4; q++)
{
    decimal ldr = BankingFormulas.CalcLoanToDepositRatio(data[q, 0], data[q, 1]);
    Console.WriteLine($"Q{q + 1} LDR: {ldr:P1}");
}
```

**Common Use Cases:**
- Banking liquidity analysis
- Credit risk assessment
- Regulatory compliance monitoring
- Bank performance evaluation

**Industry Standards:**
- LDR < 80%: Conservative (high liquidity)
- LDR 80-90%: Moderate (balanced)
- LDR > 90%: Aggressive (lower liquidity)
- LDR > 100%: High risk (loans exceed deposits)

---

### CalcLoanToValueRatio

Calculates the loan-to-value ratio, used in lending decisions.

**Formula:** LTV = Loan Amount / Value of Collateral

**Method Signature:**
```csharp
public static decimal CalcLoanToValueRatio(
    decimal loanAmount,
    decimal valueOfCollateral)

public static Result<decimal> TryCalcLoanToValueRatio(
    decimal loanAmount,
    decimal valueOfCollateral)
```

**Parameters:**
- `loanAmount`: Amount of loan requested (must be non-negative)
- `valueOfCollateral`: Appraised value of collateral (must be positive)

**Parameter Constraints:**
- loanAmount ≥ 0
- valueOfCollateral > 0

**Returns:** The loan-to-value ratio as a decimal (e.g., 0.80 represents 80%)

**Example:**
```csharp
// Mortgage: $280,000 loan on $350,000 home
decimal ltv = BankingFormulas.CalcLoanToValueRatio(280000m, 350000m);
// Returns: 0.80 (80% LTV)

// Check if PMI required (typically LTV > 80%)
Result<decimal> result = BankingFormulas.TryCalcLoanToValueRatio(
    285000m, 350000m
);
if (result.IsSuccess)
{
    decimal ltvPercent = result.Value * 100;
    bool pmiRequired = ltvPercent > 80;
    Console.WriteLine($"LTV: {ltvPercent:F1}%");
    Console.WriteLine($"PMI Required: {(pmiRequired ? "Yes" : "No")}");
}

// Calculate maximum loan amount
decimal homeValue = 400000m;
decimal maxLtvRatio = 0.80m;  // Bank's max LTV
decimal maxLoan = homeValue * maxLtvRatio;
Console.WriteLine($"Maximum loan: {maxLoan:C}");  // $320,000

// Calculate required down payment
decimal purchasePrice = 350000m;
decimal desiredLtv = 0.80m;
decimal loanAmount = purchasePrice * desiredLtv;
decimal downPayment = purchasePrice - loanAmount;
Console.WriteLine($"Down payment needed: {downPayment:C}");  // $70,000

// Refinance scenario
decimal currentLoan = 200000m;
decimal currentValue = 300000m;
decimal currentLtv = BankingFormulas.CalcLoanToValueRatio(currentLoan, currentValue);
Console.WriteLine($"Current LTV: {currentLtv:P1}");  // 66.7% - equity built up
```

**Common Use Cases:**
- Mortgage underwriting
- Refinancing evaluation
- Private Mortgage Insurance (PMI) determination
- Home equity loan qualification

**Industry Standards:**
- LTV ≤ 80%: Conventional loan, no PMI required
- LTV 80-95%: PMI typically required
- LTV > 95%: May require special programs (FHA, VA)
- LTV > 100%: Underwater mortgage (negative equity)

---

### CalcSimpleInterest

Calculates simple interest earned on a principal amount.

**Formula:** SI = P × r × t

**Method Signature:**
```csharp
public static decimal CalcSimpleInterest(
    decimal principal,
    decimal rate,
    decimal time)

public static Result<decimal> TryCalcSimpleInterest(
    decimal principal,
    decimal rate,
    decimal time)
```

**Parameters:**
- `principal`: Initial principal amount (must be non-negative)
- `rate`: Interest rate per time period (must be non-negative)
- `time`: Number of time periods (must be non-negative)

**Parameter Constraints:**
- principal ≥ 0
- rate ≥ 0
- time ≥ 0

**Returns:** The simple interest earned (not including principal)

**Example:**
```csharp
// Interest on $10,000 at 5% for 3 years
decimal interest = BankingFormulas.CalcSimpleInterest(10000m, 0.05m, 3m);
// Returns: $1,500

// Short-term loan: $5,000 at 8% for 6 months (0.5 years)
decimal interest = BankingFormulas.CalcSimpleInterest(5000m, 0.08m, 0.5m);
// Returns: $200

// Calculate total amount due
decimal principal = 8000m;
decimal rate = 0.06m;
decimal years = 2m;
decimal interest = BankingFormulas.CalcSimpleInterest(principal, rate, years);
decimal totalDue = principal + interest;
Console.WriteLine($"Principal: {principal:C}");
Console.WriteLine($"Interest: {interest:C}");
Console.WriteLine($"Total due: {totalDue:C}");
// Principal: $8,000.00
// Interest: $960.00
// Total due: $8,960.00

// Compare simple vs compound interest
decimal p = 10000m;
decimal r = 0.05m;
decimal t = 5m;

decimal simpleInterest = BankingFormulas.CalcSimpleInterest(p, r, t);
decimal compoundInterest = BankingFormulas.CalcCompoundInterest(p, r, t);

Console.WriteLine($"Simple Interest: {simpleInterest:C}");      // $2,500
Console.WriteLine($"Compound Interest: {compoundInterest:C}");  // $2,762.82
// Compound earns $262.82 more

// Result-based approach
Result<decimal> result = BankingFormulas.TryCalcSimpleInterest(10000m, 0.05m, 3m);
result.Match(
    onSuccess: i => Console.WriteLine($"Interest: {i:C}"),
    onFailure: err => Console.WriteLine($"Error: {err}")
);
```

**Common Use Cases:**
- Short-term loans
- Promissory notes
- Treasury bills
- Simple interest bonds
- Quick interest estimates

---

### CalcSimpleInterestRate

Calculates the interest rate given principal, interest earned, and time period.

**Formula:** r = I / (P × t)

**Method Signature:**
```csharp
public static decimal CalcSimpleInterestRate(
    decimal principal,
    decimal interest,
    decimal time)

public static Result<decimal> TryCalcSimpleInterestRate(
    decimal principal,
    decimal interest,
    decimal time)
```

**Parameters:**
- `principal`: Principal amount (must be positive)
- `interest`: Interest earned (must be non-negative)
- `time`: Time period (must be positive)

**Parameter Constraints:**
- principal > 0
- interest ≥ 0
- time > 0

**Returns:** The interest rate per time period as a decimal (e.g., 0.05 represents 5%)

**Example:**
```csharp
// What rate earned $500 interest on $10,000 over 2 years?
decimal rate = BankingFormulas.CalcSimpleInterestRate(10000m, 500m, 2m);
// Returns: 0.025 (2.5%)

// Compare investment returns
decimal investment = 5000m;
decimal interest1 = 300m;  // Account A
decimal interest2 = 350m;  // Account B
decimal time = 1m;

decimal rateA = BankingFormulas.CalcSimpleInterestRate(investment, interest1, time);
decimal rateB = BankingFormulas.CalcSimpleInterestRate(investment, interest2, time);

Console.WriteLine($"Account A rate: {rateA:P2}");  // 6.00%
Console.WriteLine($"Account B rate: {rateB:P2}");  // 7.00%

// Determine if rate meets target
decimal principal = 20000m;
decimal interestEarned = 1200m;
decimal years = 2m;

Result<decimal> result = BankingFormulas.TryCalcSimpleInterestRate(
    principal, interestEarned, years
);

if (result.IsSuccess)
{
    decimal targetRate = 0.04m;  // 4% target
    if (result.Value >= targetRate)
    {
        Console.WriteLine($"Rate of {result.Value:P2} meets target");
    }
    else
    {
        Console.WriteLine($"Rate of {result.Value:P2} below target");
    }
}
```

**Common Use Cases:**
- Evaluating investment performance
- Comparing loan offers
- Calculating historical returns
- Reverse engineering interest rates

---

### CalcSimpleInterestPrincipal

Calculates the principal amount given interest earned, rate, and time period.

**Formula:** P = I / (r × t)

**Method Signature:**
```csharp
public static decimal CalcSimpleInterestPrincipal(
    decimal interest,
    decimal rate,
    decimal time)

public static Result<decimal> TryCalcSimpleInterestPrincipal(
    decimal interest,
    decimal rate,
    decimal time)
```

**Parameters:**
- `interest`: Interest amount (must be non-negative)
- `rate`: Interest rate per time period (must be positive)
- `time`: Time period (must be positive)

**Parameter Constraints:**
- interest ≥ 0
- rate > 0
- time > 0

**Returns:** The principal amount

**Example:**
```csharp
// What principal earned $600 at 5% over 3 years?
decimal principal = BankingFormulas.CalcSimpleInterestPrincipal(600m, 0.05m, 3m);
// Returns: $4,000

// Investment planning
decimal desiredInterest = 2000m;  // Want to earn $2,000
decimal rate = 0.04m;             // 4% available rate
decimal years = 5m;

decimal requiredPrincipal = BankingFormulas.CalcSimpleInterestPrincipal(
    desiredInterest, rate, years
);
Console.WriteLine($"Need to invest: {requiredPrincipal:C}");  // $10,000

// Loan amount calculation
decimal interestCharged = 500m;
decimal annualRate = 0.10m;
decimal loanTerm = 1m;  // 1 year

Result<decimal> result = BankingFormulas.TryCalcSimpleInterestPrincipal(
    interestCharged, annualRate, loanTerm
);
if (result.IsSuccess)
{
    Console.WriteLine($"Original loan amount: {result.Value:C}");  // $5,000
}
```

**Common Use Cases:**
- Investment goal planning
- Loan amount determination
- Savings calculations
- Financial planning

---

### CalcSimpleInterestTime

Calculates the time period required given principal, interest earned, and rate.

**Formula:** t = I / (r × P)

**Method Signature:**
```csharp
public static decimal CalcSimpleInterestTime(
    decimal principal,
    decimal interest,
    decimal rate)

public static Result<decimal> TryCalcSimpleInterestTime(
    decimal principal,
    decimal interest,
    decimal rate)
```

**Parameters:**
- `principal`: Principal amount (must be positive)
- `interest`: Interest amount (must be non-negative)
- `rate`: Interest rate per time period (must be positive)

**Parameter Constraints:**
- principal > 0
- interest ≥ 0
- rate > 0

**Returns:** The time period in the same units as the rate

**Example:**
```csharp
// How long to earn $1,000 on $10,000 at 5%?
decimal time = BankingFormulas.CalcSimpleInterestTime(10000m, 1000m, 0.05m);
// Returns: 2 (2 years)

// Investment timeline
decimal investment = 5000m;
decimal targetInterest = 750m;
decimal annualRate = 0.06m;

Result<decimal> result = BankingFormulas.TryCalcSimpleInterestTime(
    investment, targetInterest, annualRate
);
if (result.IsSuccess)
{
    Console.WriteLine($"Time to reach goal: {result.Value:F1} years");  // 2.5 years
}

// Compare investment durations
decimal principal = 20000m;
decimal desiredReturn = 3000m;

decimal timeAt4Percent = BankingFormulas.CalcSimpleInterestTime(
    principal, desiredReturn, 0.04m
);
decimal timeAt5Percent = BankingFormulas.CalcSimpleInterestTime(
    principal, desiredReturn, 0.05m
);

Console.WriteLine($"At 4%: {timeAt4Percent:F1} years");  // 3.75 years
Console.WriteLine($"At 5%: {timeAt5Percent:F1} years");  // 3.0 years
// Higher rate reaches goal faster

// Loan term calculation
decimal loanAmount = 15000m;
decimal interestWillingToPay = 1800m;
decimal loanRate = 0.08m;

decimal maxTerm = BankingFormulas.CalcSimpleInterestTime(
    loanAmount, interestWillingToPay, loanRate
);
Console.WriteLine($"Maximum loan term: {maxTerm:F1} years");  // 1.5 years
```

**Common Use Cases:**
- Investment timeline planning
- Loan term determination
- Savings goal calculations
- Time-to-goal estimates

---

## Financial Markets Formulas

The `FinancialMarketsFormulas` class provides formulas for analyzing financial markets and economic indicators.

### CalcRateOfInflation

Calculates the rate of inflation based on Consumer Price Index (CPI) changes.

**Formula:** Rate of Inflation = (Ending CPI - Initial CPI) / Initial CPI

**Method Signature:**
```csharp
public static decimal CalcRateOfInflation(
    decimal initialConsumerPriceIndex,
    decimal endingConsumerPriceIndex)

public static Result<decimal> TryCalcRateOfInflation(
    decimal initialConsumerPriceIndex,
    decimal endingConsumerPriceIndex)
```

**Parameters:**
- `initialConsumerPriceIndex`: CPI at start of period (must be non-zero)
- `endingConsumerPriceIndex`: CPI at end of period (any value)

**Parameter Constraints:**
- initialConsumerPriceIndex ≠ 0 (division by zero)

**Returns:** The inflation rate as a decimal (e.g., 0.023 represents 2.3% inflation)

**Example:**
```csharp
// CPI increased from 250 to 255
decimal inflation = FinancialMarketsFormulas.CalcRateOfInflation(250m, 255m);
// Returns: 0.02 (2% inflation)

// Deflation scenario
decimal deflation = FinancialMarketsFormulas.CalcRateOfInflation(260m, 255m);
// Returns: -0.0192 (-1.92% deflation)

// Year-over-year inflation
decimal cpi2022 = 296.808m;
decimal cpi2023 = 304.127m;
Result<decimal> result = FinancialMarketsFormulas.TryCalcRateOfInflation(
    cpi2022, cpi2023
);
if (result.IsSuccess)
{
    Console.WriteLine($"Annual inflation: {result.Value:P2}");  // 2.47%
}

// Multi-year analysis
decimal[,] cpiData = {
    { 2020m, 258.811m },
    { 2021m, 270.970m },
    { 2022m, 296.808m },
    { 2023m, 304.127m }
};

Console.WriteLine("Year\tInflation");
for (int i = 1; i < 4; i++)
{
    decimal rate = FinancialMarketsFormulas.CalcRateOfInflation(
        cpiData[i - 1, 1], cpiData[i, 1]
    );
    Console.WriteLine($"{cpiData[i, 0]}\t{rate:P2}");
}
```

**Common Use Cases:**
- Economic analysis
- Cost of living adjustments (COLA)
- Investment return adjustments
- Salary negotiations
- Long-term financial planning

---

### CalcRealRateOfReturn

Calculates the real rate of return by adjusting nominal return for inflation (Fisher equation).

**Formula:** Real Rate = ((1 + Nominal Rate) / (1 + Inflation Rate)) - 1

**Method Signature:**
```csharp
public static decimal CalcRealRateOfReturn(
    decimal nominalRate,
    decimal inflationRate)

public static Result<decimal> TryCalcRealRateOfReturn(
    decimal nominalRate,
    decimal inflationRate)
```

**Parameters:**
- `nominalRate`: Nominal (stated) rate of return (any value)
- `inflationRate`: Inflation rate (must not equal -1 to avoid division by zero)

**Parameter Constraints:**
- inflationRate ≠ -1 (would cause division by zero in denominator)

**Returns:** The real rate of return as a decimal (e.g., 0.0485 represents 4.85%)

**Example:**
```csharp
// 8% nominal return with 3% inflation
decimal realReturn = FinancialMarketsFormulas.CalcRealRateOfReturn(0.08m, 0.03m);
// Returns: 0.0485 (4.85% real return)

// Investment analysis
decimal nominalReturn = 0.10m;  // 10% stated return
decimal inflation = 0.025m;      // 2.5% inflation

Result<decimal> result = FinancialMarketsFormulas.TryCalcRealRateOfReturn(
    nominalReturn, inflation
);
if (result.IsSuccess)
{
    Console.WriteLine($"Nominal return: {nominalReturn:P2}");
    Console.WriteLine($"Inflation: {inflation:P2}");
    Console.WriteLine($"Real return: {result.Value:P2}");
    // Nominal return: 10.00%
    // Inflation: 2.50%
    // Real return: 7.32%
}

// Compare investment options
decimal[,] investments = {
    { 0.08m, 0.03m },  // Option A: 8% return, 3% inflation
    { 0.12m, 0.05m },  // Option B: 12% return, 5% inflation
    { 0.06m, 0.02m }   // Option C: 6% return, 2% inflation
};

for (int i = 0; i < 3; i++)
{
    decimal real = FinancialMarketsFormulas.CalcRealRateOfReturn(
        investments[i, 0], investments[i, 1]
    );
    char option = (char)('A' + i);
    Console.WriteLine($"Option {option} real return: {real:P2}");
}
// Option A real return: 4.85%
// Option B real return: 6.67%
// Option C real return: 3.92%

// High inflation scenario
decimal nominalRate = 0.05m;
decimal highInflation = 0.08m;
decimal realRate = FinancialMarketsFormulas.CalcRealRateOfReturn(
    nominalRate, highInflation
);
// Returns: -0.0278 (-2.78% - losing purchasing power)
```

**Common Use Cases:**
- Investment performance evaluation
- Retirement planning
- Portfolio analysis
- Economic comparisons across time periods
- Purchasing power calculations

---

## Result Pattern Usage

### Complete Guide with Examples

#### Success Case

```csharp
// Simple success case
Result<decimal> result = Result<decimal>.Success(42.5m);
Console.WriteLine(result.IsSuccess);  // True
Console.WriteLine(result.Value);      // 42.5

// From calculation
Result<decimal> interest = BankingFormulas.TryCalcSimpleInterest(1000m, 0.05m, 1m);
if (interest.IsSuccess)
{
    Console.WriteLine($"Interest: {interest.Value:C}");  // Interest: $50.00
}
```

#### Failure Case

```csharp
// Simple failure
Result<decimal> result = Result<decimal>.Failure("Invalid input");
Console.WriteLine(result.IsFailure);  // True
Console.WriteLine(result.Error);      // "Invalid input"

// From calculation
Result<decimal> result = BankingFormulas.TryCalcSimpleInterest(-1000m, 0.05m, 1m);
if (result.IsFailure)
{
    Console.WriteLine($"Error: {result.Error}");
    // Error: Parameter 'principal' must be non-negative. Provided value: -1000
}
```

#### Pattern Matching

```csharp
// With return value
Result<decimal> result = BankingFormulas.TryCalcLoanPayment(100000m, 0.05m/12m, 360m);

string message = result.Match(
    onSuccess: value => $"Monthly payment: {value:C}",
    onFailure: error => $"Calculation failed: {error}"
);
Console.WriteLine(message);

// With side effects
result.Match(
    onSuccess: value => {
        Console.WriteLine($"Success! Payment: {value:C}");
        SaveToDatabase(value);
    },
    onFailure: error => {
        Console.WriteLine($"Error: {error}");
        LogError(error);
    }
);

// Inline pattern matching
Console.WriteLine(
    BankingFormulas.TryCalcSimpleInterest(1000m, 0.05m, 1m).Match(
        onSuccess: i => $"Interest: {i:C}",
        onFailure: e => $"Failed: {e}"
    )
);
```

#### Map - Transform Success Value

```csharp
// Simple transformation
Result<decimal> amount = Result<decimal>.Success(1000m);
Result<string> formatted = amount.Map(val => $"${val:F2}");
Console.WriteLine(formatted.Value);  // $1000.00

// Chain transformations
Result<decimal> principal = Result<decimal>.Success(5000m);
Result<string> message = principal
    .Map(p => p * 1.05m)  // Apply 5% growth
    .Map(fv => Math.Round(fv, 2))  // Round to cents
    .Map(rounded => $"Future value: ${rounded:F2}");
Console.WriteLine(message.Value);  // Future value: $5250.00

// Map preserves errors
Result<decimal> error = Result<decimal>.Failure("Invalid input");
Result<string> mapped = error.Map(val => val.ToString());
Console.WriteLine(mapped.IsFailure);  // True
Console.WriteLine(mapped.Error);      // "Invalid input"

// Real-world example: Calculate and format
Result<string> FormatLoanPayment(decimal amount, decimal rate, decimal periods)
{
    return BankingFormulas.TryCalcLoanPayment(amount, rate, periods)
        .Map(payment => $"Your monthly payment is {payment:C}");
}

var result = FormatLoanPayment(100000m, 0.05m/12m, 360m);
Console.WriteLine(result.Value);  // Your monthly payment is $536.82
```

#### Bind - Chain Operations that Return Results

```csharp
// Simple chaining
Result<decimal> principal = Result<decimal>.Success(10000m);
Result<decimal> interest = principal.Bind(p =>
    BankingFormulas.TryCalcSimpleInterest(p, 0.05m, 1m)
);

// Complex chaining
Result<decimal> CalculateTotalLoanCost(
    decimal amount, decimal annualRate, decimal years)
{
    return BankingFormulas.TryCalcLoanPayment(
            amount, annualRate / 12m, years * 12m
        )
        .Map(monthlyPayment => monthlyPayment * years * 12m)  // Total payments
        .Map(totalPaid => totalPaid - amount);  // Total interest
}

var totalCost = CalculateTotalLoanCost(200000m, 0.065m, 30m);
totalCost.Match(
    onSuccess: cost => Console.WriteLine($"Total interest: {cost:C}"),
    onFailure: error => Console.WriteLine($"Error: {error}")
);

// Bind stops on first error
Result<decimal> ComplexCalculation(decimal value)
{
    return Result<decimal>.Success(value)
        .Bind(v => Validation1(v))  // If this fails, chain stops
        .Bind(v => Validation2(v))  // Never called if Validation1 failed
        .Bind(v => Calculation(v)); // Never called if either validation failed
}

// Real-world: Multi-step calculation
Result<decimal> CalculateAffordableHome(
    decimal income, decimal downPayment, decimal rate)
{
    // Step 1: Calculate max monthly payment (28% of income)
    return Result<decimal>.Success(income * 0.28m)
        // Step 2: Calculate max loan amount
        .Bind(maxPayment => CalculateMaxLoan(maxPayment, rate, 360m))
        // Step 3: Add down payment for total home price
        .Map(maxLoan => maxLoan + downPayment);
}

Result<decimal> CalculateMaxLoan(decimal payment, decimal rate, decimal periods)
{
    // Reverse loan payment formula
    if (rate == 0) return Result<decimal>.Success(payment * periods);

    decimal maxLoan = payment * (1 - DecimalMath.Pow(1 + rate, -periods)) / rate;
    return Result<decimal>.Success(maxLoan);
}

var affordableHome = CalculateAffordableHome(6000m, 50000m, 0.065m / 12m);
Console.WriteLine(affordableHome.Match(
    onSuccess: price => $"Can afford home up to: {price:C}",
    onFailure: error => $"Calculation error: {error}"
));
```

#### Chaining Calculations

```csharp
// Example 1: Loan analysis
Result<LoanAnalysis> AnalyzeLoan(
    decimal amount, decimal rate, decimal years)
{
    return BankingFormulas.TryCalcLoanPayment(amount, rate / 12m, years * 12m)
        .Map(payment => new LoanAnalysis
        {
            MonthlyPayment = payment,
            TotalPaid = payment * years * 12m,
            TotalInterest = payment * years * 12m - amount,
            LoanAmount = amount
        });
}

// Example 2: Investment projection
Result<decimal> ProjectInvestment(
    decimal initial, decimal monthlyContribution,
    decimal annualRate, decimal years)
{
    // Calculate compound growth on initial
    return BankingFormulas.TryCalcContinuousCompounding(
            initial, annualRate, years
        )
        // Add future value of monthly contributions
        .Map(initialGrowth => {
            decimal monthlyRate = annualRate / 12m;
            decimal months = years * 12m;

            // Future value of annuity
            decimal contributionsValue = monthlyContribution *
                (DecimalMath.Pow(1 + monthlyRate, months) - 1) / monthlyRate;

            return initialGrowth + contributionsValue;
        });
}

var projection = ProjectInvestment(10000m, 500m, 0.07m, 10m);
projection.Match(
    onSuccess: fv => Console.WriteLine($"Projected value: {fv:C}"),
    onFailure: err => Console.WriteLine($"Error: {err}")
);

// Example 3: Error handling in chains
Result<decimal> SafeCalculation(decimal input)
{
    return ValidateInput(input)
        .Bind(validated => PerformCalculation(validated))
        .Bind(result => ValidateOutput(result))
        .Map(final => RoundResult(final));
}

Result<decimal> ValidateInput(decimal value)
{
    return value > 0
        ? Result<decimal>.Success(value)
        : Result<decimal>.Failure("Input must be positive");
}

Result<decimal> PerformCalculation(decimal value)
{
    try
    {
        decimal result = value * 1.05m;  // Some calculation
        return Result<decimal>.Success(result);
    }
    catch (Exception ex)
    {
        return Result<decimal>.Failure($"Calculation failed: {ex.Message}");
    }
}
```

---

## Error Handling

### Exception-Based Approach

Traditional exception handling for predictable control flow.

```csharp
try
{
    decimal payment = BankingFormulas.CalcLoanPayment(100000m, 0.05m / 12m, 360m);
    Console.WriteLine($"Monthly payment: {payment:C}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
    // Handle validation error
}
catch (OverflowException ex)
{
    Console.WriteLine($"Calculation overflow: {ex.Message}");
    // Handle numeric overflow
}

// With validation
decimal amount = GetUserInput();
try
{
    var validation = ParameterValidator.ValidatePositive(amount, "amount");
    validation.ThrowIfInvalid();  // Throws if validation fails

    decimal result = BankingFormulas.CalcSimpleInterest(amount, 0.05m, 1m);
    Console.WriteLine($"Interest: {result:C}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
}
```

**When to Use:**
- Unexpected error conditions
- Integration with existing exception-based code
- When you want exceptions to bubble up the call stack
- Simpler error handling for straightforward cases

---

### Result-Based Approach

Functional error handling with explicit error states.

```csharp
// Basic usage
Result<decimal> result = BankingFormulas.TryCalcLoanPayment(
    100000m, 0.05m / 12m, 360m
);

if (result.IsSuccess)
{
    Console.WriteLine($"Payment: {result.Value:C}");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
    if (result.ErrorContext != null)
    {
        Console.WriteLine(result.ErrorContext.ToDetailedMessage());
    }
}

// With pattern matching (cleaner)
result.Match(
    onSuccess: payment => Console.WriteLine($"Payment: {payment:C}"),
    onFailure: error => Console.WriteLine($"Failed: {error}")
);

// Multiple calculations
Result<decimal> Calculate()
{
    return BankingFormulas.TryCalcSimpleInterest(10000m, 0.05m, 1m)
        .Bind(interest => BankingFormulas.TryCalcCompoundInterest(
            10000m + interest, 0.05m, 1m
        ));
}

// Error propagation
Result<decimal> ComplexCalculation(decimal input)
{
    // If any step fails, error propagates automatically
    return ValidateInput(input)
        .Bind(validated => Step1(validated))
        .Bind(result1 => Step2(result1))
        .Bind(result2 => Step3(result2));
}
```

**When to Use:**
- Expected validation failures
- Functional programming style
- When you want explicit error handling
- Chaining multiple operations
- API design where errors are part of the contract

---

### When to Use Each Approach

| Scenario | Recommended Approach | Reason |
|----------|---------------------|---------|
| User input validation | Result-based | Expected to fail frequently |
| Internal calculations | Exception-based | Unexpected failures |
| API endpoints | Result-based | Explicit error states |
| Data pipeline | Result-based | Easy error propagation |
| Simple scripts | Exception-based | Less boilerplate |
| Complex workflows | Result-based | Better composition |
| Library code | Both available | Let consumers choose |

---

### Error Context Usage

**Accessing Detailed Error Information:**

```csharp
Result<decimal> result = BankingFormulas.TryCalcLoanPayment(-100m, 0.05m, 360m);

if (result.IsFailure && result.ErrorContext != null)
{
    var ctx = result.ErrorContext;

    Console.WriteLine($"Parameter: {ctx.ParameterName}");
    Console.WriteLine($"Value: {ctx.ParameterValue}");
    Console.WriteLine($"Constraint: {ctx.ConstraintViolated}");
    Console.WriteLine($"Valid Range: {ctx.ValidRange}");
    Console.WriteLine($"Formula: {ctx.FormulaName}");

    // Or get formatted message
    Console.WriteLine(ctx.ToDetailedMessage());
}
```

**Logging Error Context:**

```csharp
void LogCalculationError(Result<decimal> result)
{
    if (result.IsFailure)
    {
        _logger.LogError(result.Error);

        if (result.ErrorContext != null)
        {
            _logger.LogDebug(result.ErrorContext.ToDetailedMessage());

            if (result.ErrorContext.InnerException != null)
            {
                _logger.LogError(
                    result.ErrorContext.InnerException,
                    "Underlying exception"
                );
            }
        }
    }
}
```

**Custom Error Contexts:**

```csharp
Result<decimal> CalculateWithContext(decimal value)
{
    if (value < 0)
    {
        var context = new ErrorContext
        {
            ParameterName = "value",
            ParameterValue = value,
            ConstraintViolated = "Must be non-negative",
            ValidRange = "[0, +∞)",
            FormulaName = "Custom Calculation",
            AdditionalInfo = "Negative values indicate data quality issues"
        };

        return Result<decimal>.Failure(
            "Invalid value for calculation",
            context
        );
    }

    // Perform calculation
    return Result<decimal>.Success(value * 1.05m);
}
```

---

## Migration Guide (v1.x to v2.0)

### Breaking Changes

**Good News: NONE!**

Version 2.0 is 100% backward compatible with v1.x. All existing code will continue to work without modifications.

### What's New in v2.0

#### 1. Result Pattern Methods

Every method now has a `Try*` variant that returns `Result<T>` instead of throwing exceptions.

**v1.x (still works):**
```csharp
try
{
    decimal payment = BankingFormulas.CalcLoanPayment(100000m, 0.05m/12m, 360m);
    Console.WriteLine($"Payment: {payment:C}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

**v2.0 (new option):**
```csharp
Result<decimal> result = BankingFormulas.TryCalcLoanPayment(
    100000m, 0.05m/12m, 360m
);
result.Match(
    onSuccess: payment => Console.WriteLine($"Payment: {payment:C}"),
    onFailure: error => Console.WriteLine($"Error: {error}")
);
```

#### 2. Comprehensive Validation Framework

New validation utilities for common constraints.

**v1.x:**
```csharp
if (value <= 0)
    throw new ArgumentException("Value must be positive");
```

**v2.0:**
```csharp
var validation = ParameterValidator.ValidatePositive(value, "value");
validation.ThrowIfInvalid();

// Or with Result pattern
if (!validation.IsValid)
{
    return Result<decimal>.Failure(validation.FirstError, validation.Context);
}
```

#### 3. High-Precision Decimal Mathematics

New `DecimalMath` class for financial-grade calculations.

**v1.x:**
```csharp
// Used Math.Pow with double conversion (precision loss)
double result = Math.Pow((double)baseValue, (double)exponent);
decimal value = (decimal)result;
```

**v2.0:**
```csharp
// Pure decimal precision, no double conversion
decimal value = DecimalMath.Pow(baseValue, exponent);
```

#### 4. Enhanced Error Context

Detailed error information for debugging.

**v1.x:**
```csharp
// Only exception message available
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);  // Basic error
}
```

**v2.0:**
```csharp
Result<decimal> result = BankingFormulas.TryCalcLoanPayment(...);
if (result.IsFailure && result.ErrorContext != null)
{
    Console.WriteLine(result.ErrorContext.ToDetailedMessage());
    // Detailed: parameter name, value, constraint, valid range, etc.
}
```

---

### Recommended Updates

While not required, consider these improvements:

#### 1. Add Result Pattern for User Input

**Before:**
```csharp
try
{
    decimal value = GetUserInput();
    decimal result = BankingFormulas.CalcSimpleInterest(value, 0.05m, 1m);
    DisplayResult(result);
}
catch (ArgumentException ex)
{
    ShowError(ex.Message);
}
```

**After:**
```csharp
decimal value = GetUserInput();
Result<decimal> result = BankingFormulas.TryCalcSimpleInterest(value, 0.05m, 1m);

result.Match(
    onSuccess: interest => DisplayResult(interest),
    onFailure: error => ShowError(error)
);
```

#### 2. Chain Related Calculations

**Before:**
```csharp
try
{
    decimal payment = BankingFormulas.CalcLoanPayment(100000m, 0.05m/12m, 360m);
    decimal totalPaid = payment * 360m;
    decimal totalInterest = totalPaid - 100000m;
    return totalInterest;
}
catch (ArgumentException ex)
{
    LogError(ex.Message);
    return 0m;
}
```

**After:**
```csharp
return BankingFormulas.TryCalcLoanPayment(100000m, 0.05m/12m, 360m)
    .Map(payment => payment * 360m)
    .Map(totalPaid => totalPaid - 100000m)
    .Match(
        onSuccess: interest => interest,
        onFailure: error => { LogError(error); return 0m; }
    );
```

#### 3. Use DecimalMath for Custom Calculations

**Before:**
```csharp
// With precision loss
decimal futureValue = principal * (decimal)Math.Pow((double)(1 + rate), (double)years);
```

**After:**
```csharp
// Full decimal precision
decimal futureValue = principal * DecimalMath.Pow(1 + rate, years);
```

#### 4. Leverage Validation Framework

**Before:**
```csharp
void ValidateInputs(decimal amount, decimal rate, decimal time)
{
    if (amount <= 0)
        throw new ArgumentException("Amount must be positive");
    if (rate < 0 || rate > 1)
        throw new ArgumentException("Rate must be between 0 and 1");
    if (time <= 0)
        throw new ArgumentException("Time must be positive");
}
```

**After:**
```csharp
ValidationResult ValidateInputs(decimal amount, decimal rate, decimal time)
{
    var result = ParameterValidator.ValidatePositive(amount, nameof(amount));
    result = result.Combine(ParameterValidator.ValidateRange(rate, 0m, 1m, nameof(rate)));
    result = result.Combine(ParameterValidator.ValidatePositive(time, nameof(time)));
    return result;
}

// Usage
var validation = ValidateInputs(amount, rate, time);
if (!validation.IsValid)
{
    // Handle all errors at once
    Console.WriteLine($"Validation errors: {validation.AllErrors}");
}
```

---

### Migration Examples

#### Example 1: Simple Calculation

**v1.x code (no changes needed):**
```csharp
public decimal CalculateMonthlyPayment(decimal loan, decimal rate, int years)
{
    return BankingFormulas.CalcLoanPayment(loan, rate / 12m, years * 12m);
}
```

**Optional v2.0 enhancement:**
```csharp
public Result<decimal> CalculateMonthlyPayment(decimal loan, decimal rate, int years)
{
    return BankingFormulas.TryCalcLoanPayment(loan, rate / 12m, years * 12m);
}
```

#### Example 2: With Validation

**v1.x code (continues to work):**
```csharp
public decimal CalculateInterest(decimal principal, decimal rate, decimal time)
{
    if (principal <= 0)
        throw new ArgumentException("Principal must be positive");
    if (rate < 0)
        throw new ArgumentException("Rate cannot be negative");
    if (time <= 0)
        throw new ArgumentException("Time must be positive");

    return BankingFormulas.CalcSimpleInterest(principal, rate, time);
}
```

**v2.0 enhancement with validators:**
```csharp
public Result<decimal> CalculateInterest(decimal principal, decimal rate, decimal time)
{
    // Validate all inputs
    var validation = ParameterValidator.ValidatePositive(principal, nameof(principal))
        .Combine(ParameterValidator.ValidateNonNegative(rate, nameof(rate)))
        .Combine(ParameterValidator.ValidatePositive(time, nameof(time)));

    if (!validation.IsValid)
    {
        return Result<decimal>.Failure(validation.AllErrors, validation.Context);
    }

    // Use Result pattern for calculation
    return BankingFormulas.TryCalcSimpleInterest(principal, rate, time);
}
```

#### Example 3: Error Handling

**v1.x code (still valid):**
```csharp
public void ProcessLoan(LoanRequest request)
{
    try
    {
        decimal payment = BankingFormulas.CalcLoanPayment(
            request.Amount,
            request.Rate / 12m,
            request.Years * 12m
        );

        SaveLoanDetails(request, payment);
        NotifyCustomer(payment);
    }
    catch (ArgumentException ex)
    {
        LogError($"Invalid loan parameters: {ex.Message}");
        NotifyCustomerOfError();
    }
}
```

**v2.0 with Result pattern:**
```csharp
public void ProcessLoan(LoanRequest request)
{
    BankingFormulas.TryCalcLoanPayment(
        request.Amount,
        request.Rate / 12m,
        request.Years * 12m
    ).Match(
        onSuccess: payment => {
            SaveLoanDetails(request, payment);
            NotifyCustomer(payment);
        },
        onFailure: error => {
            LogError($"Invalid loan parameters: {error}");
            NotifyCustomerOfError();
        }
    );
}
```

---

## Examples

### Real-World Scenarios

#### Loan Calculations

**Scenario 1: Mortgage Affordability Calculator**

```csharp
public class MortgageCalculator
{
    public Result<MortgageAnalysis> CalculateAffordability(
        decimal annualIncome,
        decimal monthlyDebts,
        decimal downPayment,
        decimal annualRate,
        int years)
    {
        // Validate inputs
        var validation = ParameterValidator.ValidatePositive(annualIncome, nameof(annualIncome))
            .Combine(ParameterValidator.ValidateNonNegative(monthlyDebts, nameof(monthlyDebts)))
            .Combine(ParameterValidator.ValidateNonNegative(downPayment, nameof(downPayment)))
            .Combine(ParameterValidator.ValidateRange(annualRate, 0m, 0.20m, nameof(annualRate)))
            .Combine(ParameterValidator.ValidateRange(years, 1m, 30m, nameof(years)));

        if (!validation.IsValid)
        {
            return Result<MortgageAnalysis>.Failure(
                validation.AllErrors,
                validation.Context
            );
        }

        decimal grossMonthlyIncome = annualIncome / 12m;
        decimal monthlyRate = annualRate / 12m;
        decimal months = years * 12m;

        // Calculate DTI
        return BankingFormulas.TryCalcDebtToIncomeRatio(monthlyDebts, grossMonthlyIncome)
            .Bind(dti => {
                // Max monthly payment (28% of gross income)
                decimal maxPayment = grossMonthlyIncome * 0.28m;

                // Calculate max loan amount
                decimal maxLoan = maxPayment * (1 - DecimalMath.Pow(1 + monthlyRate, -months)) / monthlyRate;

                // Total home price
                decimal maxHomePrice = maxLoan + downPayment;

                return Result<MortgageAnalysis>.Success(new MortgageAnalysis
                {
                    MaxHomePrice = maxHomePrice,
                    MaxLoanAmount = maxLoan,
                    MonthlyPayment = maxPayment,
                    DownPayment = downPayment,
                    DebtToIncomeRatio = dti,
                    IsQualified = dti < 0.43m
                });
            });
    }
}

public class MortgageAnalysis
{
    public decimal MaxHomePrice { get; set; }
    public decimal MaxLoanAmount { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal DownPayment { get; set; }
    public decimal DebtToIncomeRatio { get; set; }
    public bool IsQualified { get; set; }
}

// Usage
var calculator = new MortgageCalculator();
var result = calculator.CalculateAffordability(
    annualIncome: 75000m,
    monthlyDebts: 500m,
    downPayment: 40000m,
    annualRate: 0.065m,
    years: 30
);

result.Match(
    onSuccess: analysis => {
        Console.WriteLine($"Maximum Home Price: {analysis.MaxHomePrice:C}");
        Console.WriteLine($"Monthly Payment: {analysis.MonthlyPayment:C}");
        Console.WriteLine($"DTI Ratio: {analysis.DebtToIncomeRatio:P1}");
        Console.WriteLine($"Qualified: {(analysis.IsQualified ? "Yes" : "No")}");
    },
    onFailure: error => Console.WriteLine($"Calculation failed: {error}")
);
```

**Scenario 2: Loan Comparison Tool**

```csharp
public class LoanComparison
{
    public Result<List<LoanOption>> CompareLoanOptions(
        decimal loanAmount,
        List<(decimal rate, int years)> options)
    {
        var results = new List<LoanOption>();

        foreach (var (rate, years) in options)
        {
            var result = BankingFormulas.TryCalcLoanPayment(
                loanAmount,
                rate / 12m,
                years * 12m
            );

            if (result.IsSuccess)
            {
                decimal monthlyPayment = result.Value;
                decimal totalPaid = monthlyPayment * years * 12m;
                decimal totalInterest = totalPaid - loanAmount;

                results.Add(new LoanOption
                {
                    AnnualRate = rate,
                    Years = years,
                    MonthlyPayment = monthlyPayment,
                    TotalInterest = totalInterest,
                    TotalPaid = totalPaid
                });
            }
        }

        if (results.Count == 0)
        {
            return Result<List<LoanOption>>.Failure(
                "No valid loan options calculated"
            );
        }

        return Result<List<LoanOption>>.Success(
            results.OrderBy(o => o.TotalPaid).ToList()
        );
    }
}

public class LoanOption
{
    public decimal AnnualRate { get; set; }
    public int Years { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalPaid { get; set; }
}

// Usage
var comparison = new LoanComparison();
var options = new List<(decimal, int)>
{
    (0.055m, 15),
    (0.065m, 30),
    (0.045m, 10)
};

var result = comparison.CompareLoanOptions(200000m, options);
result.Match(
    onSuccess: loans => {
        Console.WriteLine("Loan Options (sorted by total cost):\n");
        foreach (var loan in loans)
        {
            Console.WriteLine($"{loan.Years}-year at {loan.AnnualRate:P2}:");
            Console.WriteLine($"  Monthly: {loan.MonthlyPayment:C}");
            Console.WriteLine($"  Total Interest: {loan.TotalInterest:C}");
            Console.WriteLine($"  Total Paid: {loan.TotalPaid:C}\n");
        }
    },
    onFailure: error => Console.WriteLine($"Error: {error}")
);
```

---

#### Investment Returns

**Scenario 1: Investment Growth Calculator**

```csharp
public class InvestmentCalculator
{
    public Result<InvestmentProjection> ProjectGrowth(
        decimal initialInvestment,
        decimal monthlyContribution,
        decimal annualReturn,
        int years)
    {
        // Validate inputs
        var validation = ParameterValidator.ValidateNonNegative(
                initialInvestment, nameof(initialInvestment))
            .Combine(ParameterValidator.ValidateNonNegative(
                monthlyContribution, nameof(monthlyContribution)))
            .Combine(ParameterValidator.ValidateRange(
                annualReturn, -0.50m, 2.0m, nameof(annualReturn)))
            .Combine(ParameterValidator.ValidateRange(
                years, 1m, 50m, nameof(years)));

        if (!validation.IsValid)
        {
            return Result<InvestmentProjection>.Failure(
                validation.AllErrors, validation.Context
            );
        }

        decimal monthlyRate = annualReturn / 12m;
        decimal months = years * 12m;

        // Future value of initial investment
        return BankingFormulas.TryCalcContinuousCompounding(
                initialInvestment, annualReturn, years
            )
            .Map(initialGrowth => {
                // Future value of monthly contributions (annuity)
                decimal contributionGrowth = 0m;
                if (monthlyContribution > 0 && monthlyRate != 0)
                {
                    contributionGrowth = monthlyContribution *
                        (DecimalMath.Pow(1 + monthlyRate, months) - 1) / monthlyRate;
                }
                else if (monthlyContribution > 0)
                {
                    contributionGrowth = monthlyContribution * months;
                }

                decimal totalValue = initialGrowth + contributionGrowth;
                decimal totalContributions = initialInvestment + (monthlyContribution * months);
                decimal totalGains = totalValue - totalContributions;

                return new InvestmentProjection
                {
                    FutureValue = totalValue,
                    TotalContributions = totalContributions,
                    TotalGains = totalGains,
                    AnnualReturn = annualReturn,
                    Years = years
                };
            });
    }
}

public class InvestmentProjection
{
    public decimal FutureValue { get; set; }
    public decimal TotalContributions { get; set; }
    public decimal TotalGains { get; set; }
    public decimal AnnualReturn { get; set; }
    public int Years { get; set; }

    public decimal ReturnOnInvestment =>
        TotalContributions > 0 ? TotalGains / TotalContributions : 0m;
}

// Usage
var calculator = new InvestmentCalculator();
var result = calculator.ProjectGrowth(
    initialInvestment: 10000m,
    monthlyContribution: 500m,
    annualReturn: 0.07m,
    years: 20
);

result.Match(
    onSuccess: projection => {
        Console.WriteLine($"Future Value: {projection.FutureValue:C}");
        Console.WriteLine($"Total Contributions: {projection.TotalContributions:C}");
        Console.WriteLine($"Investment Gains: {projection.TotalGains:C}");
        Console.WriteLine($"ROI: {projection.ReturnOnInvestment:P1}");
    },
    onFailure: error => Console.WriteLine($"Error: {error}")
);
```

**Scenario 2: Inflation-Adjusted Returns**

```csharp
public class RealReturnCalculator
{
    public Result<RealReturnAnalysis> CalculateRealReturn(
        decimal initialInvestment,
        decimal nominalReturn,
        decimal inflationRate,
        int years)
    {
        // Calculate nominal future value
        return BankingFormulas.TryCalcContinuousCompounding(
                initialInvestment, nominalReturn, years
            )
            .Bind(nominalValue =>
                // Calculate real rate of return
                FinancialMarketsFormulas.TryCalcRealRateOfReturn(
                    nominalReturn, inflationRate
                )
                .Bind(realRate =>
                    // Calculate real future value
                    BankingFormulas.TryCalcContinuousCompounding(
                        initialInvestment, realRate, years
                    )
                    .Map(realValue => new RealReturnAnalysis
                    {
                        NominalValue = nominalValue,
                        RealValue = realValue,
                        NominalReturn = nominalReturn,
                        RealReturn = realRate,
                        InflationRate = inflationRate,
                        PurchasingPowerLoss = nominalValue - realValue
                    })
                )
            );
    }
}

public class RealReturnAnalysis
{
    public decimal NominalValue { get; set; }
    public decimal RealValue { get; set; }
    public decimal NominalReturn { get; set; }
    public decimal RealReturn { get; set; }
    public decimal InflationRate { get; set; }
    public decimal PurchasingPowerLoss { get; set; }
}

// Usage
var calculator = new RealReturnCalculator();
var result = calculator.CalculateRealReturn(
    initialInvestment: 50000m,
    nominalReturn: 0.08m,
    inflationRate: 0.03m,
    years: 10
);

result.Match(
    onSuccess: analysis => {
        Console.WriteLine($"Nominal Future Value: {analysis.NominalValue:C}");
        Console.WriteLine($"Real Future Value: {analysis.RealValue:C}");
        Console.WriteLine($"Nominal Return: {analysis.NominalReturn:P2}");
        Console.WriteLine($"Real Return: {analysis.RealReturn:P2}");
        Console.WriteLine($"Purchasing Power Loss: {analysis.PurchasingPowerLoss:C}");
    },
    onFailure: error => Console.WriteLine($"Error: {error}")
);
```

---

#### Financial Ratios

**Banking Health Dashboard**

```csharp
public class BankingHealthDashboard
{
    public Result<BankHealthMetrics> AnalyzeBank(BankData data)
    {
        // Calculate multiple ratios
        var ldrResult = BankingFormulas.TryCalcLoanToDepositRatio(
            data.TotalLoans, data.TotalDeposits
        );

        var ltvResults = data.Loans.Select(loan =>
            BankingFormulas.TryCalcLoanToValueRatio(
                loan.Amount, loan.CollateralValue
            )
        ).ToList();

        if (ldrResult.IsFailure)
        {
            return Result<BankHealthMetrics>.Failure(
                ldrResult.Error, ldrResult.ErrorContext
            );
        }

        // Calculate average LTV
        var validLtvs = ltvResults
            .Where(r => r.IsSuccess)
            .Select(r => r.Value)
            .ToList();

        if (validLtvs.Count == 0)
        {
            return Result<BankHealthMetrics>.Failure(
                "No valid LTV ratios calculated"
            );
        }

        decimal avgLtv = validLtvs.Average();
        decimal ldr = ldrResult.Value;

        // Assess health
        string ldrAssessment = ldr < 0.80m ? "Conservative" :
                               ldr < 0.90m ? "Moderate" : "Aggressive";

        string ltvAssessment = avgLtv < 0.70m ? "Low Risk" :
                               avgLtv < 0.85m ? "Moderate Risk" : "High Risk";

        return Result<BankHealthMetrics>.Success(new BankHealthMetrics
        {
            LoanToDepositRatio = ldr,
            LDRAssessment = ldrAssessment,
            AverageLTV = avgLtv,
            LTVAssessment = ltvAssessment,
            TotalLoans = data.TotalLoans,
            TotalDeposits = data.TotalDeposits,
            LoanCount = data.Loans.Count
        });
    }
}

public class BankData
{
    public decimal TotalLoans { get; set; }
    public decimal TotalDeposits { get; set; }
    public List<LoanInfo> Loans { get; set; }
}

public class LoanInfo
{
    public decimal Amount { get; set; }
    public decimal CollateralValue { get; set; }
}

public class BankHealthMetrics
{
    public decimal LoanToDepositRatio { get; set; }
    public string LDRAssessment { get; set; }
    public decimal AverageLTV { get; set; }
    public string LTVAssessment { get; set; }
    public decimal TotalLoans { get; set; }
    public decimal TotalDeposits { get; set; }
    public int LoanCount { get; set; }
}

// Usage
var dashboard = new BankingHealthDashboard();
var bankData = new BankData
{
    TotalLoans = 450000000m,
    TotalDeposits = 550000000m,
    Loans = new List<LoanInfo>
    {
        new LoanInfo { Amount = 200000m, CollateralValue = 250000m },
        new LoanInfo { Amount = 350000m, CollateralValue = 450000m },
        new LoanInfo { Amount = 180000m, CollateralValue = 230000m }
    }
};

var result = dashboard.AnalyzeBank(bankData);
result.Match(
    onSuccess: metrics => {
        Console.WriteLine("=== Bank Health Dashboard ===");
        Console.WriteLine($"Total Loans: {metrics.TotalLoans:C}");
        Console.WriteLine($"Total Deposits: {metrics.TotalDeposits:C}");
        Console.WriteLine($"Loan-to-Deposit Ratio: {metrics.LoanToDepositRatio:P1}");
        Console.WriteLine($"LDR Assessment: {metrics.LDRAssessment}");
        Console.WriteLine($"Average LTV: {metrics.AverageLTV:P1}");
        Console.WriteLine($"LTV Assessment: {metrics.LTVAssessment}");
        Console.WriteLine($"Loan Portfolio Size: {metrics.LoanCount} loans");
    },
    onFailure: error => Console.WriteLine($"Analysis failed: {error}")
);
```

---

#### Error Handling Patterns

**Complete Error Handling Example**

```csharp
public class RobustLoanProcessor
{
    private readonly ILogger _logger;

    public Result<LoanApproval> ProcessLoanApplication(LoanApplication app)
    {
        _logger.LogInformation($"Processing loan application {app.ApplicationId}");

        // Step 1: Validate application data
        var validation = ValidateApplication(app);
        if (!validation.IsValid)
        {
            _logger.LogWarning(
                $"Application {app.ApplicationId} failed validation: " +
                validation.AllErrors
            );
            return Result<LoanApproval>.Failure(
                "Application validation failed: " + validation.AllErrors,
                validation.Context
            );
        }

        // Step 2: Calculate DTI
        return BankingFormulas.TryCalcDebtToIncomeRatio(
                app.MonthlyDebts, app.GrossMonthlyIncome
            )
            .Bind(dti => {
                _logger.LogInformation($"DTI calculated: {dti:P1}");

                if (dti > 0.43m)
                {
                    return Result<LoanApproval>.Failure(
                        $"DTI ratio {dti:P1} exceeds maximum 43%",
                        new ErrorContext
                        {
                            ParameterName = "DebtToIncomeRatio",
                            ParameterValue = dti,
                            ConstraintViolated = "DTI must be <= 43%",
                            FormulaName = "Loan Qualification"
                        }
                    );
                }

                // Step 3: Calculate LTV
                return BankingFormulas.TryCalcLoanToValueRatio(
                        app.LoanAmount, app.PropertyValue
                    )
                    .Bind(ltv => {
                        _logger.LogInformation($"LTV calculated: {ltv:P1}");

                        if (ltv > 0.95m)
                        {
                            return Result<LoanApproval>.Failure(
                                $"LTV ratio {ltv:P1} exceeds maximum 95%"
                            );
                        }

                        // Step 4: Calculate monthly payment
                        return BankingFormulas.TryCalcLoanPayment(
                                app.LoanAmount,
                                app.InterestRate / 12m,
                                app.LoanTermYears * 12m
                            )
                            .Map(payment => {
                                _logger.LogInformation(
                                    $"Monthly payment: {payment:C}"
                                );

                                bool pmiRequired = ltv > 0.80m;
                                decimal pmiPayment = pmiRequired ?
                                    app.LoanAmount * 0.005m / 12m : 0m;

                                return new LoanApproval
                                {
                                    ApplicationId = app.ApplicationId,
                                    Approved = true,
                                    LoanAmount = app.LoanAmount,
                                    MonthlyPayment = payment,
                                    PMIPayment = pmiPayment,
                                    TotalMonthlyPayment = payment + pmiPayment,
                                    InterestRate = app.InterestRate,
                                    LTV = ltv,
                                    DTI = dti,
                                    ApprovalDate = DateTime.UtcNow
                                };
                            });
                    });
            })
            .Match(
                onSuccess: approval => {
                    _logger.LogInformation(
                        $"Application {app.ApplicationId} APPROVED"
                    );
                    return Result<LoanApproval>.Success(approval);
                },
                onFailure: error => {
                    _logger.LogWarning(
                        $"Application {app.ApplicationId} DENIED: {error}"
                    );

                    return Result<LoanApproval>.Success(new LoanApproval
                    {
                        ApplicationId = app.ApplicationId,
                        Approved = false,
                        DenialReason = error
                    });
                }
            );
    }

    private ValidationResult ValidateApplication(LoanApplication app)
    {
        var result = ParameterValidator.ValidatePositive(
            app.LoanAmount, nameof(app.LoanAmount)
        );
        result = result.Combine(ParameterValidator.ValidatePositive(
            app.PropertyValue, nameof(app.PropertyValue)
        ));
        result = result.Combine(ParameterValidator.ValidateRange(
            app.InterestRate, 0m, 0.20m, nameof(app.InterestRate)
        ));
        result = result.Combine(ParameterValidator.ValidateRange(
            app.LoanTermYears, 1m, 30m, nameof(app.LoanTermYears)
        ));
        result = result.Combine(ParameterValidator.ValidatePositive(
            app.GrossMonthlyIncome, nameof(app.GrossMonthlyIncome)
        ));
        result = result.Combine(ParameterValidator.ValidateNonNegative(
            app.MonthlyDebts, nameof(app.MonthlyDebts)
        ));

        return result;
    }
}

public class LoanApplication
{
    public string ApplicationId { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal PropertyValue { get; set; }
    public decimal InterestRate { get; set; }
    public int LoanTermYears { get; set; }
    public decimal GrossMonthlyIncome { get; set; }
    public decimal MonthlyDebts { get; set; }
}

public class LoanApproval
{
    public string ApplicationId { get; set; }
    public bool Approved { get; set; }
    public string DenialReason { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal PMIPayment { get; set; }
    public decimal TotalMonthlyPayment { get; set; }
    public decimal InterestRate { get; set; }
    public decimal LTV { get; set; }
    public decimal DTI { get; set; }
    public DateTime ApprovalDate { get; set; }
}

// Usage with comprehensive error handling
var processor = new RobustLoanProcessor(logger);
var application = new LoanApplication
{
    ApplicationId = "APP-2024-001",
    LoanAmount = 300000m,
    PropertyValue = 350000m,
    InterestRate = 0.065m,
    LoanTermYears = 30,
    GrossMonthlyIncome = 8000m,
    MonthlyDebts = 1500m
};

var result = processor.ProcessLoanApplication(application);
result.Match(
    onSuccess: approval => {
        if (approval.Approved)
        {
            Console.WriteLine($"Loan APPROVED");
            Console.WriteLine($"Monthly Payment: {approval.MonthlyPayment:C}");
            Console.WriteLine($"PMI: {approval.PMIPayment:C}");
            Console.WriteLine($"Total Payment: {approval.TotalMonthlyPayment:C}");
        }
        else
        {
            Console.WriteLine($"Loan DENIED");
            Console.WriteLine($"Reason: {approval.DenialReason}");
        }
    },
    onFailure: error => {
        Console.WriteLine($"Processing ERROR: {error}");
    }
);
```

---

## Additional Resources

### GitHub Repository
[https://github.com/srbrettle/Financial-Formulas-Library-.NET-Standard](https://github.com/srbrettle/Financial-Formulas-Library-.NET-Standard)

### NuGet Package
[https://www.nuget.org/packages/FinancialFormulas](https://www.nuget.org/packages/FinancialFormulas)

### Report Issues
[https://github.com/srbrettle/Financial-Formulas-Library-.NET-Standard/issues](https://github.com/srbrettle/Financial-Formulas-Library-.NET-Standard/issues)

### License
MIT License - See [LICENSE](https://github.com/srbrettle/Financial-Formulas-Library-.NET-Standard/blob/master/LICENSE) file

---

## Appendix A: Quick Reference

### Namespaces
```csharp
using srbrettle.FinancialFormulas;
using srbrettle.FinancialFormulas.Core;
using srbrettle.FinancialFormulas.Validation;
using srbrettle.FinancialFormulas.Mathematics;
```

### Common Patterns

**Exception-based:**
```csharp
decimal result = BankingFormulas.CalcLoanPayment(amount, rate, periods);
```

**Result-based:**
```csharp
Result<decimal> result = BankingFormulas.TryCalcLoanPayment(amount, rate, periods);
```

**Validation:**
```csharp
var validation = ParameterValidator.ValidatePositive(value, "value");
validation.ThrowIfInvalid();
```

**Pattern Matching:**
```csharp
result.Match(
    onSuccess: value => Console.WriteLine(value),
    onFailure: error => Console.WriteLine(error)
);
```

**Chaining:**
```csharp
var result = Step1()
    .Bind(r1 => Step2(r1))
    .Map(r2 => Transform(r2));
```

---

**End of API Documentation**

*Generated for Financial Formulas Library v2.0*
*Last Updated: 2024*
