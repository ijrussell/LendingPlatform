namespace LoanApplicationConsoleTests;

public class LoanApplicationDataStoreTests
{
    private static readonly string Approved = 
        new LoanApplicationStatus.Approved().ToString();

    private readonly LoanApplicationDataStore _store = new();

    [Fact]
    public void AddingAnItemIsSuccessful()
    {
        var response = new LoanApplicationResponse.Processed(
            100_000,
            200_000,
            700, 
            50, 
            Approved
        );
        
        _store.Insert(response);
        
        Assert.Single(_store, item => 
            string.Equals(item.Status, Approved, StringComparison.OrdinalIgnoreCase)
        );
    }
}