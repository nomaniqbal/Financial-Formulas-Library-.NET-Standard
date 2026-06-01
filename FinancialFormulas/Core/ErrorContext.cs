using System;

namespace srbrettle.FinancialFormulas.Core
{
    /// <summary>
    /// Provides detailed context information about an error that occurred during calculation or validation
    /// </summary>
    public class ErrorContext
    {
        /// <summary>
        /// Gets or sets the name of the parameter that caused the error
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the value of the parameter that caused the error
        /// </summary>
        public object ParameterValue { get; set; }

        /// <summary>
        /// Gets or sets a description of the constraint that was violated
        /// </summary>
        public string ConstraintViolated { get; set; }

        /// <summary>
        /// Gets or sets the valid range or values for the parameter
        /// </summary>
        public string ValidRange { get; set; }

        /// <summary>
        /// Gets or sets the formula or operation that failed
        /// </summary>
        public string FormulaName { get; set; }

        /// <summary>
        /// Gets or sets any inner exception that occurred
        /// </summary>
        public Exception InnerException { get; set; }

        /// <summary>
        /// Gets or sets additional diagnostic information
        /// </summary>
        public string AdditionalInfo { get; set; }

        /// <summary>
        /// Creates a formatted error message from the context
        /// </summary>
        public string ToDetailedMessage()
        {
            var message = string.Empty;

            if (!string.IsNullOrEmpty(FormulaName))
                message += $"Formula: {FormulaName}\n";

            if (!string.IsNullOrEmpty(ParameterName))
                message += $"Parameter: {ParameterName}\n";

            if (ParameterValue != null)
                message += $"Value: {ParameterValue}\n";

            if (!string.IsNullOrEmpty(ConstraintViolated))
                message += $"Constraint: {ConstraintViolated}\n";

            if (!string.IsNullOrEmpty(ValidRange))
                message += $"Valid Range: {ValidRange}\n";

            if (!string.IsNullOrEmpty(AdditionalInfo))
                message += $"Additional Info: {AdditionalInfo}\n";

            if (InnerException != null)
                message += $"Inner Exception: {InnerException.Message}\n";

            return message.TrimEnd('\n');
        }

        public override string ToString()
        {
            return ToDetailedMessage();
        }
    }
}
