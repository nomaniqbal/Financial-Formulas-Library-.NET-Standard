using System;

namespace srbrettle.FinancialFormulas.Core
{
    /// <summary>
    /// Base exception for financial calculation errors
    /// </summary>
    public class FinancialException : Exception
    {
        /// <summary>
        /// Gets the error context associated with this exception
        /// </summary>
        public ErrorContext Context { get; }

        public FinancialException(string message)
            : base(message)
        {
        }

        public FinancialException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public FinancialException(string message, ErrorContext context)
            : base(message)
        {
            Context = context;
        }

        public FinancialException(string message, ErrorContext context, Exception innerException)
            : base(message, innerException)
        {
            Context = context;
        }
    }

    /// <summary>
    /// Exception thrown when a financial calculation fails
    /// </summary>
    public class FinancialCalculationException : FinancialException
    {
        public FinancialCalculationException(string message)
            : base(message)
        {
        }

        public FinancialCalculationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public FinancialCalculationException(string message, ErrorContext context)
            : base(message, context)
        {
        }

        public FinancialCalculationException(string message, ErrorContext context, Exception innerException)
            : base(message, context, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when parameter validation fails
    /// </summary>
    public class ValidationException : FinancialException
    {
        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, ErrorContext context)
            : base(message, context)
        {
        }

        public ValidationException(string message, string parameterName, object parameterValue)
            : base(message, new ErrorContext
            {
                ParameterName = parameterName,
                ParameterValue = parameterValue
            })
        {
        }
    }
}
