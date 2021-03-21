using DepsWebApp.Models;

namespace DepsWebApp.Mappings
{
    public static class ErrorMapping
    {
        /// <summary>
        /// Maps the error and sets a ErrorDetails model
        /// </summary>
        /// <param name="exceptionName">Exception name</param>
        /// <returns>ErrorDetails model with setted data</returns>
        public static ErrorDetails MapError(string exceptionName)
        {
            ErrorDetails errorDetails = new ErrorDetails();

            switch (exceptionName)
            {
                case "NotImplementedException":
                    errorDetails.StatusCode = 100;
                    errorDetails.Message = "System exception: something went wrong at the server side";
                    break;
                default:
                    errorDetails.StatusCode = 400;
                    errorDetails.Message = "Critical unhandled exception";
                    break;
            }

            return errorDetails;
        }
    }
}
