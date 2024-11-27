namespace BenchmarkTest;

public class ExceptionThrower
{
    private static readonly int RecursiveCalls = 50;

    public static async Task<Exception> GetExceptionWithStacktrace()
    {
        try
        {
            return await Nested1(0);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Dynamic Stacktrace:{Environment.NewLine}{e.StackTrace}");
            return e;
        }
    }

    public static async Task<Exception> Nested1(int i)
    {
        if (i < RecursiveCalls)
        {
            return await Nested1(++i);
        }
        
        return await Nested2(0);
    }

    public static async Task<Exception> Nested2(int i)
    {
        if (i < RecursiveCalls)
        {
            return await Nested2(++i);
        }
        
        return await Nested3(0);
    }
    
    public static async Task<Exception> Nested3(int i)
    {
        if (i < RecursiveCalls)
        {
            return await Nested3(++i);
        }
        
        return await Nested4(0);
    }
    
    public static async Task<Exception> Nested4(int i)
    {
        if (i < RecursiveCalls)
        {
            return await Nested4(++i);
        }
        
        return await Nested5();
    }

    public static async Task<Exception> Nested5()
    {
        await Task.CompletedTask;
        throw new Exception("Something went wrong!!!");
    }
}