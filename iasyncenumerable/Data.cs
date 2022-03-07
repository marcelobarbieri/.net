namespace Exemplo;

public static class Data
{
    private static async IAsyncEnumerable<int> GetData()
    {
        for (var i = 1; i <= 1000; i++)
        {
            await Task.Delay(1000);
            yield return i;
        }
    }
}

