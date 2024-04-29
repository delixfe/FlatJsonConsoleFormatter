namespace Unit.Infrastructure;

public static class ExceptionExtensions
{
    public static void EnsureStackTrace(params Exception[] exceptions)
    {
        foreach (var exception in exceptions)
        {
            EnsureStackTrace(exception);
        }
    }

    public static Exception EnsureStackTrace(this Exception exception)
    {
        if (string.IsNullOrEmpty(exception.StackTrace))
        {
            try
            {
                throw exception;
            }
            catch
            {
            }
        }

        Assert.False(string.IsNullOrEmpty(exception.StackTrace));
        return exception;
    }
}
