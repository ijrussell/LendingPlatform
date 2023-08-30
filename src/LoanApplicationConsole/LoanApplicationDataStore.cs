using System.Collections.Immutable;
using System.Collections.ObjectModel;
using LoanApplicationConsole.LoanApplication;

namespace LoanApplicationConsole;

public class LoanApplicationDataStore : Collection<LoanApplicationData>
{
    public void Insert(LoanApplicationResponse response)
    {
        switch (response)
        {
            case LoanApplicationResponse.Processed p:
                Add(new LoanApplicationData(
                    p.LoanAmount,
                    p.AssetValue,
                    p.CreditScore,
                    p.LoanToValueRate,
                    p.Status
                ));
                break;
        }
    }

    public ImmutableList<LoanApplicationData> LoanApplications => Items.ToImmutableList();
}

public record struct LoanApplicationData(
    int LoanAmount, 
    int AssetValue, 
    short CreditScore, 
    byte LoanToValueRate, 
    string Status
);    
