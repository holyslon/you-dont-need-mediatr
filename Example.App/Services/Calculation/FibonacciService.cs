namespace Example.App.Services.Calculation;

public class FibonacciService
{
    public int GetNumber(int number)
    {
        if (number == 0)
        {
            return 0;
        }

        if (number == 1)
        {
            return 1;
        }
        var current = (0, 1);
        for (var i = 1; i < number; i++)
        {
            current = (current.Item2, current.Item1 + current.Item2);
        }

        return current.Item2;
    }
}