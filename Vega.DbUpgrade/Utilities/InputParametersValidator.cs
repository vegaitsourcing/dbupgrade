using System;

namespace Vega.DbUpgrade.Utilities
{
    /// <summary>
    /// This class validates input parameters of protected and public methods.
    /// </summary>
    public static class InputParametersValidator
    {
        /// <summary>
        /// Error message if parameter is not initialized.
        /// </summary>
        private const string NotInitializedParameterMessage = "Parameter {0} must be initialized (not null)!";

        /// <summary>
        /// Error message if string parameter is null.
        /// </summary>
        private const string NullStringParameterMessage = "String parameter {0} must be initialized (not null)!";

        /// <summary>
        /// Error Message if string parameter is empty.
        /// </summary>
        private const string EmptyStringParameterMessage = "String parameter {0} must not be empty!";

        /// <summary>
        /// Checks if the input parameter (<paramref name="objectToValidate"/>is initialized (not null)
        /// and throws an ArgumentNullException if it is not initialized.
        /// </summary>
        /// <param name="objectToValidate">The parameter that will be validated.</param>
        /// <param name="paramName">The name of parameter that will be validated.</param>
        /// <seealso cref="ArgumentNullException"/>
        public static void ValidateObjectParameter(object objectToValidate, string paramName)
        {
            ValidateStringNotEmpty(paramName, "paramName");

            if (objectToValidate == null)
            {
                string errorMessage = String.Format(NotInitializedParameterMessage, paramName);
                throw new ArgumentNullException(errorMessage);
            }
        }

        /// <summary>
        /// Checks if <paramref name="stringToValidate"/> is null or empty and throw an
        /// ArgumentNullException in case the string is null or ArgumeException in case the string is empty.
        /// </summary>
        /// <param name="stringToValidate">The string parameter that will be validated.</param>
        /// <param name="paramName">The name of parameter that will be validated.</param>
        /// <seealso cref="ArgumentNullException"/>
        /// <seealso cref="ArgumentException"/>
        public static void ValidateStringNotEmpty(string stringToValidate, string paramName)
        {
            if (String.IsNullOrEmpty(paramName))
            {
                throw new ArgumentException("ValidateStringNotEmpty method requires name of parameter to be set!");
            }

            if (String.IsNullOrEmpty(stringToValidate))
            {
                string errorMessage;
                if (stringToValidate == null)
                {
                    errorMessage = String.Format(NullStringParameterMessage, paramName);
                    throw new ArgumentNullException(errorMessage);
                }

                errorMessage = String.Format(EmptyStringParameterMessage, paramName);
                throw new ArgumentException(errorMessage);
            }
        }
    }
}