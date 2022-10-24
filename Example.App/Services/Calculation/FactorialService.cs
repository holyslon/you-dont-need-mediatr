namespace Example.App.Services.Calculation;

public class FactorialService
{
    public int GetFactorial(int number)
    {
        return Enumerable.Range(1, number).Aggregate(1, (i, i1) => i * i1);
    }
}