using System;

namespace srbrettle.FinancialFormulas.Mathematics
{
    /// <summary>
    /// High-precision mathematical operations for financial calculations using decimal type exclusively.
    /// Implements advanced mathematical functions without double conversion to maintain financial precision.
    /// </summary>
    public static class DecimalMath
    {
        /// <summary>
        /// Default precision for decimal calculations (matches decimal type precision).
        /// </summary>
        private const int DEFAULT_PRECISION = 28;

        /// <summary>
        /// Maximum number of iterations for iterative algorithms.
        /// </summary>
        private const int MAX_ITERATIONS = 100;

        /// <summary>
        /// Convergence threshold for iterative algorithms.
        /// </summary>
        private const decimal EPSILON = 0.0000000001m;

        /// <summary>
        /// Calculates the power of a decimal number using Taylor series and iteration.
        /// Implements baseValue^exponent with high precision.
        /// </summary>
        /// <param name="baseValue">The base value to be raised to a power.</param>
        /// <param name="exponent">The exponent to raise the base to.</param>
        /// <returns>The result of baseValue raised to the power of exponent.</returns>
        /// <exception cref="ArgumentException">Thrown when baseValue is negative and exponent is not an integer, or when baseValue is zero and exponent is negative or zero.</exception>
        /// <exception cref="OverflowException">Thrown when the result would exceed decimal range.</exception>
        /// <remarks>
        /// Uses the formula: base^exp = e^(exp * ln(base)) for non-integer exponents.
        /// For integer exponents, uses efficient iterative multiplication.
        /// Special cases:
        /// - base^0 = 1 (except 0^0 which throws exception)
        /// - base^1 = base
        /// - 1^exp = 1
        /// - 0^exp = 0 (for positive exp)
        /// </remarks>
        public static decimal Pow(decimal baseValue, decimal exponent)
        {
            // Handle special cases
            if (baseValue == 0)
            {
                if (exponent <= 0)
                    throw new ArgumentException("Cannot raise zero to a negative or zero power.", nameof(exponent));
                return 0;
            }

            if (baseValue < 0 && exponent != Math.Floor(exponent))
            {
                throw new ArgumentException("Cannot raise a negative number to a non-integer power.", nameof(exponent));
            }

            if (exponent == 0)
                return 1;

            if (exponent == 1)
                return baseValue;

            if (baseValue == 1)
                return 1;

            // Handle negative base with integer exponent
            if (baseValue < 0)
            {
                bool isNegativeResult = ((int)exponent % 2) != 0;
                decimal absResult = Pow(-baseValue, exponent);
                return isNegativeResult ? -absResult : absResult;
            }

            // Handle integer exponents efficiently
            if (exponent == Math.Floor(exponent) && Math.Abs(exponent) < 1000)
            {
                return PowInteger(baseValue, (int)exponent);
            }

            // For non-integer exponents: base^exp = e^(exp * ln(base))
            decimal logBase = Log(baseValue);
            decimal exponentTimesLog = exponent * logBase;

            // Check for overflow before computing
            if (exponentTimesLog > 27m) // ln(decimal.MaxValue) ≈ 27
                throw new OverflowException("Result would exceed decimal range.");
            if (exponentTimesLog < -27m)
                return 0; // Underflow to zero

            return Exp(exponentTimesLog);
        }

        /// <summary>
        /// Efficiently calculates integer powers using binary exponentiation.
        /// </summary>
        private static decimal PowInteger(decimal baseValue, int exponent)
        {
            if (exponent == 0)
                return 1;

            bool isNegativeExponent = exponent < 0;
            int absExponent = Math.Abs(exponent);
            decimal result = 1;
            decimal currentBase = baseValue;

            // Binary exponentiation
            while (absExponent > 0)
            {
                if ((absExponent & 1) == 1)
                {
                    result *= currentBase;
                }
                currentBase *= currentBase;
                absExponent >>= 1;
            }

            return isNegativeExponent ? 1 / result : result;
        }

        /// <summary>
        /// Calculates the natural logarithm (base e) of a decimal value using Newton-Raphson method.
        /// </summary>
        /// <param name="value">The positive value to calculate the natural logarithm of.</param>
        /// <returns>The natural logarithm of the specified value.</returns>
        /// <exception cref="ArgumentException">Thrown when value is less than or equal to zero.</exception>
        /// <remarks>
        /// Uses Newton-Raphson iteration: x(n+1) = x(n) + 2 * (value - e^x(n)) / (value + e^x(n))
        /// Applies range reduction for values far from 1 to improve convergence.
        /// Special cases:
        /// - ln(1) = 0
        /// - ln(e) = 1
        /// </remarks>
        public static decimal Log(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Logarithm is only defined for positive values.", nameof(value));

            if (value == 1)
                return 0;

            if (value == Constants.E)
                return 1;

            // Range reduction: if value is very large or small, use log properties
            // log(a * b^n) = log(a) + n * log(b)
            int powerOfTen = 0;
            decimal normalizedValue = value;

            // Normalize to range [0.1, 10] for better convergence
            while (normalizedValue > 10)
            {
                normalizedValue /= 10;
                powerOfTen++;
            }
            while (normalizedValue < 0.1m)
            {
                normalizedValue *= 10;
                powerOfTen--;
            }

            // Initial guess using simple approximation
            decimal x = 0;
            if (normalizedValue > 1)
                x = normalizedValue - 1;
            else
                x = -(1 - normalizedValue);

            // Newton-Raphson iteration
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                decimal expX = Exp(x);
                decimal delta = 2 * (normalizedValue - expX) / (normalizedValue + expX);

                if (Math.Abs(delta) < EPSILON)
                    break;

                x += delta;
            }

            // Add back the range reduction adjustment
            return x + powerOfTen * Constants.LN10;
        }

        /// <summary>
        /// Calculates the logarithm of a value with a specified base.
        /// </summary>
        /// <param name="value">The positive value to calculate the logarithm of.</param>
        /// <param name="baseValue">The positive base of the logarithm (must not equal 1).</param>
        /// <returns>The logarithm of value with the specified base.</returns>
        /// <exception cref="ArgumentException">Thrown when value or baseValue is invalid.</exception>
        /// <remarks>
        /// Uses the change of base formula: log_b(x) = ln(x) / ln(b)
        /// </remarks>
        public static decimal Log(decimal value, decimal baseValue)
        {
            if (value <= 0)
                throw new ArgumentException("Logarithm is only defined for positive values.", nameof(value));

            if (baseValue <= 0 || baseValue == 1)
                throw new ArgumentException("Base must be positive and not equal to 1.", nameof(baseValue));

            if (value == 1)
                return 0;

            if (value == baseValue)
                return 1;

            // Change of base formula: log_b(x) = ln(x) / ln(b)
            return Log(value) / Log(baseValue);
        }

        /// <summary>
        /// Calculates the exponential function (e^exponent) using Taylor series expansion.
        /// </summary>
        /// <param name="exponent">The exponent to raise e to.</param>
        /// <returns>The value of e raised to the specified exponent.</returns>
        /// <exception cref="OverflowException">Thrown when the result would exceed decimal range.</exception>
        /// <remarks>
        /// Uses Taylor series: e^x = 1 + x + x^2/2! + x^3/3! + x^4/4! + ...
        /// Applies range reduction for large exponents to improve accuracy and convergence.
        /// Special cases:
        /// - e^0 = 1
        /// - e^1 = e
        /// </remarks>
        public static decimal Exp(decimal exponent)
        {
            if (exponent == 0)
                return 1;

            if (exponent == 1)
                return Constants.E;

            // Check for overflow/underflow
            if (exponent > 27m) // ln(decimal.MaxValue) ≈ 27
                throw new OverflowException("Exponent too large, result would exceed decimal range.");

            if (exponent < -27m)
                return 0; // Underflow to zero

            // Range reduction: e^x = (e^(x/2))^2
            // Reduces large exponents to smaller ones for better accuracy
            bool needSquaring = false;
            decimal reducedExponent = exponent;

            while (Math.Abs(reducedExponent) > 1)
            {
                reducedExponent /= 2;
                needSquaring = true;
            }

            // Taylor series expansion: e^x = 1 + x + x^2/2! + x^3/3! + ...
            decimal result = 1;
            decimal term = 1;
            decimal x = reducedExponent;

            for (int i = 1; i < MAX_ITERATIONS; i++)
            {
                term *= x / i;

                if (Math.Abs(term) < EPSILON)
                    break;

                result += term;
            }

            // Square result if we did range reduction
            if (needSquaring)
            {
                int squaringCount = 0;
                decimal temp = exponent;
                while (Math.Abs(temp) > 1)
                {
                    temp /= 2;
                    squaringCount++;
                }

                for (int i = 0; i < squaringCount; i++)
                {
                    result *= result;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the square root of a decimal value using Newton-Raphson method.
        /// </summary>
        /// <param name="value">The non-negative value to calculate the square root of.</param>
        /// <returns>The square root of the specified value.</returns>
        /// <exception cref="ArgumentException">Thrown when value is negative.</exception>
        /// <remarks>
        /// Uses Newton-Raphson iteration: x(n+1) = (x(n) + value/x(n)) / 2
        /// Provides faster convergence than general power function for square roots.
        /// Special cases:
        /// - sqrt(0) = 0
        /// - sqrt(1) = 1
        /// </remarks>
        public static decimal Sqrt(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Cannot calculate square root of a negative number.", nameof(value));

            if (value == 0)
                return 0;

            if (value == 1)
                return 1;

            // Initial guess using a simple heuristic
            decimal guess = value / 2m;

            // For very small or very large numbers, adjust initial guess
            if (value < 1)
                guess = value;
            else if (value > 100)
                guess = value / 10m;

            // Newton-Raphson iteration: x(n+1) = (x(n) + value/x(n)) / 2
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                decimal nextGuess = (guess + value / guess) / 2m;
                decimal difference = Math.Abs(nextGuess - guess);

                if (difference < EPSILON)
                    return nextGuess;

                guess = nextGuess;
            }

            return guess;
        }

        /// <summary>
        /// Calculates the absolute value of a decimal number.
        /// </summary>
        /// <param name="value">The value to get the absolute value of.</param>
        /// <returns>The absolute value.</returns>
        public static decimal Abs(decimal value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the specified decimal number.
        /// </summary>
        /// <param name="value">A decimal number.</param>
        /// <returns>The largest integer less than or equal to value.</returns>
        public static decimal Floor(decimal value)
        {
            return Math.Floor(value);
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to the specified decimal number.
        /// </summary>
        /// <param name="value">A decimal number.</param>
        /// <returns>The smallest integer greater than or equal to value.</returns>
        public static decimal Ceiling(decimal value)
        {
            return Math.Ceiling(value);
        }

        /// <summary>
        /// Rounds a decimal value to the nearest integral value.
        /// </summary>
        /// <param name="value">A decimal number to be rounded.</param>
        /// <returns>The integer nearest to value.</returns>
        public static decimal Round(decimal value)
        {
            return Math.Round(value);
        }

        /// <summary>
        /// Rounds a decimal value to a specified number of fractional digits.
        /// </summary>
        /// <param name="value">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>The number nearest to value that contains a number of fractional digits equal to decimals.</returns>
        public static decimal Round(decimal value, int decimals)
        {
            return Math.Round(value, decimals);
        }

        /// <summary>
        /// Returns the larger of two decimal numbers.
        /// </summary>
        /// <param name="value1">The first of two decimal numbers to compare.</param>
        /// <param name="value2">The second of two decimal numbers to compare.</param>
        /// <returns>Parameter value1 or value2, whichever is larger.</returns>
        public static decimal Max(decimal value1, decimal value2)
        {
            return Math.Max(value1, value2);
        }

        /// <summary>
        /// Returns the smaller of two decimal numbers.
        /// </summary>
        /// <param name="value1">The first of two decimal numbers to compare.</param>
        /// <param name="value2">The second of two decimal numbers to compare.</param>
        /// <returns>Parameter value1 or value2, whichever is smaller.</returns>
        public static decimal Min(decimal value1, decimal value2)
        {
            return Math.Min(value1, value2);
        }

        /// <summary>
        /// Calculates the logarithm base 10 of a specified number.
        /// </summary>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <returns>The base 10 logarithm of value.</returns>
        /// <exception cref="ArgumentException">Thrown when value is less than or equal to zero.</exception>
        public static decimal Log10(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Logarithm is only defined for positive values.", nameof(value));

            if (value == 1)
                return 0;

            if (value == 10)
                return 1;

            return Log(value) / Constants.LN10;
        }

        /// <summary>
        /// Calculates the logarithm base 2 of a specified number.
        /// </summary>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <returns>The base 2 logarithm of value.</returns>
        /// <exception cref="ArgumentException">Thrown when value is less than or equal to zero.</exception>
        public static decimal Log2(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Logarithm is only defined for positive values.", nameof(value));

            if (value == 1)
                return 0;

            if (value == 2)
                return 1;

            return Log(value) / Constants.LN2;
        }

        /// <summary>
        /// Returns a specified number raised to the specified power (convenience method for integer exponents).
        /// </summary>
        /// <param name="baseValue">The number to be raised to a power.</param>
        /// <param name="exponent">The integer exponent.</param>
        /// <returns>The number baseValue raised to the power exponent.</returns>
        public static decimal Pow(decimal baseValue, int exponent)
        {
            return PowInteger(baseValue, exponent);
        }

        /// <summary>
        /// Calculates the nth root of a number.
        /// </summary>
        /// <param name="value">The number to find the root of.</param>
        /// <param name="n">The degree of the root.</param>
        /// <returns>The nth root of value.</returns>
        /// <exception cref="ArgumentException">Thrown when n is zero or when value is negative and n is even.</exception>
        public static decimal NthRoot(decimal value, int n)
        {
            if (n == 0)
                throw new ArgumentException("Root degree cannot be zero.", nameof(n));

            if (n == 1)
                return value;

            if (n == 2)
                return Sqrt(value);

            if (value == 0)
                return 0;

            if (value == 1)
                return 1;

            bool isNegative = value < 0;
            if (isNegative && n % 2 == 0)
                throw new ArgumentException("Cannot calculate even root of negative number.", nameof(value));

            decimal absValue = Math.Abs(value);

            // Use x^(1/n) = e^(ln(x)/n)
            decimal result = Exp(Log(absValue) / n);

            return isNegative ? -result : result;
        }

        /// <summary>
        /// Calculates the cube root of a number.
        /// </summary>
        /// <param name="value">The number to find the cube root of.</param>
        /// <returns>The cube root of value.</returns>
        public static decimal Cbrt(decimal value)
        {
            return NthRoot(value, 3);
        }
    }
}
