public class ConnectionFailureException : System.ApplicationException
{
    public ConnectionFailureException() : base() { }
    public ConnectionFailureException(string message) : base(message) { }
    public ConnectionFailureException(string message, System.Exception inner) : base(message, inner) { }
}

public class ApplicationParametersException : System.ApplicationException
{
    public ApplicationParametersException() : base() { }
    public ApplicationParametersException(string message) : base(message) { }
    public ApplicationParametersException(string message, System.Exception inner) : base(message, inner) { }
}

public class UnavailableViewException : System.ApplicationException
{
    public UnavailableViewException() : base() { }
    public UnavailableViewException(string message) : base(message) { }
    public UnavailableViewException(string message, System.Exception inner) : base(message, inner) { }
}