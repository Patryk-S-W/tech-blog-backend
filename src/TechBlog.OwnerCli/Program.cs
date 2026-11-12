using Microsoft.Extensions.Configuration;

namespace TechBlog.OwnerCli;

internal static class OwnerCliProgram
{
    public static async Task<int> Main(string[] args)
    {
        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancellation.Cancel(); };
        return await OwnerProvisioning.RunAsync(args, Console.In, Console.Out,
            ReadHiddenPassword, new ConfigurationBuilder().AddEnvironmentVariables().Build(), cancellation.Token);
    }

    private static string ReadHiddenPassword()
    {
        if (Console.IsInputRedirected)
            throw new InvalidOperationException();
        var previous = Console.TreatControlCAsInput;
        try
        {
            Console.TreatControlCAsInput = true;
            var password = new System.Text.StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    throw new OperationCanceledException();
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return password.ToString();
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0) password.Length--;
                }
                else if (!char.IsControl(key.KeyChar)) password.Append(key.KeyChar);
            }
        }
        finally { Console.TreatControlCAsInput = previous; }
    }
}
