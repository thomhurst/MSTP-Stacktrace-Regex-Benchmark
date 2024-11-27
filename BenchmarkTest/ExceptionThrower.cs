namespace BenchmarkTest;

public class ExceptionThrower
{
    private static readonly int RecursiveCalls = 50;

    public static Exception GetExceptionWithStacktrace()
    {
        try
        {
            return Nested1(0);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Dynamic Stacktrace:{Environment.NewLine}{e.StackTrace}");
            return e;
        }
    }

    public static Exception Nested1(int i)
    {
        if (i < RecursiveCalls)
        {
            return Nested1(++i);
        }
        
        return Nested2(0);
    }

    public static Exception Nested2(int i)
    {
        if (i < RecursiveCalls)
        {
            return Nested2(++i);
        }
        
        return Nested3(0);
    }
    
    public static Exception Nested3(int i)
    {
        if (i < RecursiveCalls)
        {
            return Nested3(++i);
        }
        
        return Nested4(0);
    }
    
    public static Exception Nested4(int i)
    {
        if (i < RecursiveCalls)
        {
            return Nested4(++i);
        }
        
        return Nested5();
    }

    public static Exception Nested5()
    {
        throw new Exception("Something went wrong!!!");
    }
}