namespace INEOSTestProject
{
    internal class INEOSTestProjectException : Exception
    {
        public INEOSTestProjectException() : base() { }
        public INEOSTestProjectException(string? message) : base(message) { }
        public INEOSTestProjectException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
