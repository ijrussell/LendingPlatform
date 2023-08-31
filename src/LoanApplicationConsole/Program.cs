using System.Collections.Immutable;
using LoanApplicationConsole;
using LoanApplicationConsole.LoanApplication;
using Spectre.Console;

var processor = new LoanApplicationProcessor(
    new LoanApplicationRiskDecider()
);

var dataStore = new LoanApplicationDataStore();
var loanApplicationMetrics = new LoanApplicationMetrics(dataStore);

bool shouldContinue;

do
{
    var loanAmountInput = LoanAmountInput();
    var assetValueInput = AssetValueInput();
    var creditScoreInput = CreditScoreInput();
    
    var request = new LoanApplicationRequest(loanAmountInput, assetValueInput, creditScoreInput);
    
    shouldContinue = AskWantTheyWantToDo() switch 
    {
        ConsoleKey.P => ProcessLoanApplication(processor, dataStore, request),
        ConsoleKey.X => false, // Exit
        _ => true // Continue
    };
} while (shouldContinue);

Console.WriteLine("Finished");
return;

bool ProcessLoanApplication(
    LoanApplicationProcessor loanApplicationProcessor, 
    LoanApplicationDataStore loanApplicationDataStore,
    LoanApplicationRequest loanApplicationRequest)
{
    switch (loanApplicationProcessor.Process(loanApplicationRequest))
    {
        case LoanApplicationResponse.Processed response:
            loanApplicationDataStore.Insert(response);

            DisplayLoanApplicationStatus(response);
            DisplayMetrics(loanApplicationMetrics);
            break;
        case LoanApplicationResponse.UnableToProcess response:
            Console.WriteLine($"{response.Reason}");
            break;
    }

    return AskIfWeShouldContinue();
}

int LoanAmountInput()
{
    return AnsiConsole.Prompt(
        new TextPrompt<int>("Enter Loan Amount in GBP:")
            .ValidationErrorMessage("You must enter a value in GBP")
            .Validate(age =>
            {
                return age switch
                {
                    < 0 => ValidationResult.Error("Loan amount cannot be negative"),
                    _ => ValidationResult.Success(),
                };
            }));
}

int AssetValueInput()
{
    return AnsiConsole.Prompt(
        new TextPrompt<int>("Enter Asset Value in GBP:")
            .ValidationErrorMessage("You must enter a value in GBP")
            .Validate(value =>
            {
                return value switch
                {
                    < 0 => ValidationResult.Error("Asset value cannot be negative"),
                    _ => ValidationResult.Success(),
                };
            }));
}

short CreditScoreInput()
{
    return AnsiConsole.Prompt(
        new TextPrompt<short>("Enter Credit Score:")
            .ValidationErrorMessage("You must enter a value between 1 and 999")
            .Validate(value =>
            {
                return value switch
                {
                    < 1 => ValidationResult.Error("Credit score cannot be less than 1"),
                    > 999 => ValidationResult.Error("Credit score cannot be more than 999"),
                    _ => ValidationResult.Success(),
                };
            }));
}

void DisplayLoanApplicationStatus(LoanApplicationResponse.Processed response)
{
    Console.WriteLine($"Your loan application was {response.Status}");
}

void DisplayMetrics(LoanApplicationMetrics metrics)
{
    DisplayCountByStatusSummary(metrics.SummaryCountByStatus());
    DisplayTotalLoanValue(metrics.ApprovedLoanTotalValue());
    DisplayMeanLoanToValue(metrics.MeanLoanToValueRate());
    return;

    void DisplayCountByStatusSummary(ImmutableList<LoanApplicationStatusSummaryData> summary)
    {
        summary.ForEach(item =>
            Console.WriteLine($"There are {item.Count} {item.Status} applications.")
        );
    }

    void DisplayTotalLoanValue(int totalValue)
    {
        Console.WriteLine($"The total value of approved loans is {totalValue}.");
    }

    void DisplayMeanLoanToValue(int meanValue)
    {
        Console.WriteLine($"The mean LTV rate of all processed loans is {meanValue}%.");
    }
}

ConsoleKey AskWantTheyWantToDo()
{
    Console.WriteLine("Press p to process your loan application, x to exit, and any other key to cancel this application.");

    var key = Console.ReadKey().Key;
    
    Console.WriteLine("");

    return key;
}

bool AskIfWeShouldContinue()
{
    Console.WriteLine("Press x to exit, and any other key to make another loan application.");

    var key = Console.ReadKey().Key;

    Console.WriteLine("");

    return key != ConsoleKey.X;
}
