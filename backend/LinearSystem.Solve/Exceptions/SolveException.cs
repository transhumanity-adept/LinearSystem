using System.Runtime.Serialization;

namespace LinearSystem.Solve.Exceptions;

public class SolveException: Exception
{
    public SolveException()
    {
    }

    protected SolveException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SolveException(string? message) : base(message)
    {
    }

    public SolveException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    
    public static void ThrowIf(bool condition, string message)
    {
        if(condition) throw new SolveException(message);
    }
}