using System.Collections.Immutable;
namespace LoanApplicationConsoleTests;

public class LoanApplicationDataStoreTests
{
    private static readonly string Approved = 
        new LoanApplicationStatus.Approved().ToString();
    private static readonly string Declined = 
        new LoanApplicationStatus.Declined("").ToString();

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

    [Fact]
    public void CountByStatus()
    {
        PopulateStoreWithTestData();

        var actual = _store.SummaryCountByStatus();
        
        Assert.Equal(8, GetCount(actual, Approved));
        Assert.Equal(1, GetCount(actual, Declined));
    }

    [Fact]
    public void TotalValueOfApprovedLoans()
    {
        PopulateStoreWithTestData();

        var actual = _store.ApprovedLoanTotalValue();
        
        Assert.Equal(3_700_000, actual);
    }

    [Fact]
    public void Mean_LoanToValueRate_ForAllApplications()
    {
        PopulateStoreWithTestData();

        var actual = _store.MeanLoanToValueRate();
        
        Assert.Equal(50, actual);
    }

    private static short GetCount(ImmutableList<LoanApplicationStatusSummaryData> items, string status)
    {
        var data = items.FirstOrDefault(item => 
            item.Status.Equals(status, StringComparison.OrdinalIgnoreCase)
        );
        
        return data.Count;
    }
    
    private void PopulateStoreWithTestData() =>
        _responses.ForEach(_store.Insert);
    
    // 8 approved, 1 declined
    // Total approved loans: 3_700_000
    // Mean LTV of all valid applications: 50%
    private readonly List<LoanApplicationResponse.Processed> _responses = new()
    {
         new LoanApplicationResponse.Processed(
             100_000,
             1_000_000,
             900,
             10,
             Approved),
         new LoanApplicationResponse.Processed(
             200_000,
             1_000_000,
             900,
             20,
             Approved),
         new LoanApplicationResponse.Processed(
             300_000,
             1_000_000,
             900,
             30,
             Approved),
         new LoanApplicationResponse.Processed(
             400_000,
             1_000_000,
             900,
             40,
             Approved),
         new LoanApplicationResponse.Processed(
             500_000,
             1_000_000,
             900,
             50,
             Approved),
         new LoanApplicationResponse.Processed(
             600_000,
             1_000_000,
             900,
             60,
             Approved),
         new LoanApplicationResponse.Processed(
             700_000,
             1_000_000,
             900,
             70,
             Approved),
         new LoanApplicationResponse.Processed(
             800_000,
             1_000_000,
             400,
             80,
             Declined),
         new LoanApplicationResponse.Processed(
             900_000,
             1_000_000,
             900,
             90,
             Approved)
   };
}