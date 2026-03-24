using ooceBot.Functionality;
using Xunit;

namespace ooceBot.Tests;

public class QuoteCommandMethodsTests : IDisposable
{
    private readonly string _tmpFile;

    public QuoteCommandMethodsTests()
    {
        _tmpFile = Path.GetTempFileName();
        QuoteCommandMethods.FilePath = _tmpFile;
    }

    public void Dispose()
    {
        File.Delete(_tmpFile);
    }

    [Fact]
    public void SelectQuote_CanReturnLastQuote()
    {
        File.WriteAllLines(_tmpFile, new[] { "#1: first quote", "#2: second quote" });
        QuoteCommandMethods.random = new Random(42);

        var results = Enumerable.Range(0, 100)
            .Select(_ => QuoteCommandMethods.SelectQuote())
            .ToHashSet();

        // With the bug, Next(0, 2-1) = Next(0, 1) always returns 0.
        // The second quote (index 1) is never reachable, so this fails.
        Assert.Contains("#2: second quote", results);
    }
}
