namespace DepsWebApp.Models
{
    /// <summary>
    /// Exception wrapper model.
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// Status code of the exception that occured.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Message of the exception that occured.
        /// </summary>
        public string Message { get; set; }
    }
}
