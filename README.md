# Lending Platform

This is the code from a coding exercise written in C#. It contains a console application and a test project.

The requirements are shown below.

## Planning

### Logic Pseudocode

- ProcessLoanApplication(Request) : Response =>
    - ValidateRequest(Request) : Result<ValidatedRequest,List\<string>> =>
    - Ok validatedRequest =>
        - CalculateLoanToValueRate(LoanAmount, AssetValue) : Option<LoanToValueRate> =>
        - Some loanToValueRate =>
            - RunRiskDecider(LoanAmount, LoanToValueRate, CreditScore) : Status =>
            - Approved =>
                - Processed(data, Approved) // Approved
            - Declined reason =>
                - Processed(data, Declined) // Declined
        - None =>
            - UnableToProcess(reason) // Unable to calculate LTV
    - Error reasons =>
        - UnableToProcess(reasons) // Failed input validation

### ValueTypes

- LoanAmount [int] [ > 0 ]
- AssetValue [int] [ > 0 ]
- CreditScore [short] [ >= 1 && <= 999 ]
- LoanToValueRate [short] [ % -> 0-100?]

Types

- Status [Approved/Declined]
- Response [Processed/UnableToProcess]
- RiskDecider [ Name? ]
- LoanToValueRateCalculator [function?]

## Doing Something Similar in F#

If you're wondering why I think like I do in a functional way, this is the core of the problem in F#.

```fsharp
// Start with processLoanApplication at the bottom.
#r "nuget: FsToolkit.ErrorHandling"

open FsToolkit.ErrorHandling

type LoanAmount = LoanAmount of int
type AssetValue = AssetValue of int
type CreditScore = CreditScore of int16
type LoanToValueRate = LoanToValueRate of int16

type LoanApplicationRequest = {
    LoanAmount: option<int>
    AssetValue: option<int>
    CreditScore: option<int16>
}

type ValidatedLoanApplicationRequest = { 
    LoanAmount: LoanAmount
    AssetValue: AssetValue
    CreditScore: CreditScore
}

type LoanApplicationStatus =
    | Approved
    | Declined of Reasons:list<string>

type ProcessedLoanApplication = {
    LoanAmount: LoanAmount
    AssetValue: AssetValue
    CreditScore: CreditScore
    LoanToValueRate: LoanToValueRate 
    Status: LoanApplicationStatus
}

type LoanApplicationResponse =
    | Processed of ProcessedLoanApplication
    | UnableToProcess of Reasons:list<string>

[<RequireQualifiedAccess>]
module LoanApplicationResponse =
    let private createProcessed (request:ValidatedLoanApplicationRequest) loanToValueRate status =
        {
            LoanAmount = request.LoanAmount
            AssetValue = request.AssetValue
            CreditScore = request.CreditScore
            LoanToValueRate = loanToValueRate
            Status = status
        }
        |> Processed
     
    let isApproved request loanToValueRate = 
        LoanApplicationStatus.Approved
        |> createProcessed request loanToValueRate 

    let isDeclined request loanToValueRate reasons = 
        LoanApplicationStatus.Declined reasons
        |> createProcessed request loanToValueRate

let validateLoanAmount input =
    match input with
    | Some amount when amount > 0 -> amount |> LoanAmount |> Ok
    | _ -> Error ["LoanAmount is invalid"]

let validateAssetValue input =
    match input with
    | Some value when value > 0 -> value |> AssetValue |> Ok
    | _ -> Error ["AssetValue is invalid"]

let validateCreditScore input =
    match input with
    | Some score when score >= 1s && score <= 999s -> score |> CreditScore |> Ok
    | _ -> Error ["CreditScore is invalid"]

let createValidatedRequest loanValue assetValue creditScore =
    {
        LoanAmount = loanValue
        AssetValue = assetValue
        CreditScore = creditScore
    }

let validateRequest (request:LoanApplicationRequest) : Result<ValidatedLoanApplicationRequest,list<string>> =
    validation {
        let! loanValue = request.LoanAmount |> validateLoanAmount
        and! assetValue = request.AssetValue |> validateAssetValue
        and! creditScore = request.CreditScore |> validateCreditScore
        return createValidatedRequest loanValue assetValue creditScore
    }

let calculateLoanToValueRate (LoanAmount loanAmount) (AssetValue assetValue) =
    if assetValue = 0 || loanAmount >= assetValue then None
    else (100m * decimal loanAmount / decimal assetValue) |> int16 |> LoanToValueRate |> Some 

let (|LoanAmountIsNotAllowed|_|) (LoanAmount loanAmount) = 
    if loanAmount < 100_000 || loanAmount > 1_500_000 then Some ()
    else None

let isAcceptableRisk loanAmount loanToValueRate creditScore =
    match loanAmount, loanToValueRate with
    | amount, rate when amount >= 1_000_000 -> (rate <= 60s && creditScore >= 950s)
    | _, rate when rate < 60s -> creditScore >= 750s
    | _, rate when rate < 80s -> creditScore >= 800s
    | _, rate when rate < 90s -> creditScore >= 900s
    | _ -> false

let (|UnacceptableRisk|_|) (LoanAmount loanAmount, LoanToValueRate loanToValueRate, CreditScore creditScore) = 
    isAcceptableRisk loanAmount loanToValueRate creditScore
    |> fun isAcceptable -> 
        if not isAcceptable then Some () else None

let runRiskDecider loanAmount loanToValueRate creditScore =
    match loanAmount, loanToValueRate, creditScore with
    | LoanAmountIsNotAllowed, _, _ -> 
        Declined ["Loan amount requested is not allowed."]
    | UnacceptableRisk -> 
        Declined ["Unacceptable risk for the amount that you wish to borrow."]
    | _ -> 
        Approved

let processLoanApplication request = 
    match request |> validateRequest with
    | Ok validated -> 
        match calculateLoanToValueRate validated.LoanAmount validated.AssetValue with
        | Some loanToValueRate ->
            match runRiskDecider validated.LoanAmount loanToValueRate validated.CreditScore with
            | Approved -> 
                LoanApplicationResponse.isApproved validated loanToValueRate
            | Declined reasons ->
                LoanApplicationResponse.isDeclined validated loanToValueRate reasons
        | None ->
            LoanApplicationResponse.UnableToProcess ["Unable to calculate loan to value rate."]
    | Error errors ->
        LoanApplicationResponse.UnableToProcess errors

// Simple set of checks
let invalidLoanAmount = processLoanApplication { LoanAmount = None; AssetValue = Some 600_000; CreditScore = Some 900s }
let invalidAssetValue = processLoanApplication { LoanAmount = Some 600_000; AssetValue = None; CreditScore = Some 900s }
let invalidCreditScore = processLoanApplication { LoanAmount = Some 600_000; AssetValue = Some 800_000; CreditScore = None }
let declined = processLoanApplication { LoanAmount = Some 200_000; AssetValue = Some 400_000; CreditScore = Some 500s }
let approved = processLoanApplication { LoanAmount = Some 200_000; AssetValue = Some 300_000; CreditScore = Some 800s }

```

## Exercise Requirements

Build a simple Console application using the technology of your choice (preferably C#) that enables the writing and reporting of loans as per the requirements below. This should be approached as a way that can demonstrate your process to solving problems (any required infrastructure can simply be mocked), and does not need to be built to a production standard. Instead the exercise should be timeboxed to no longer than an hour. Notes can be taken of any assumptions made, and also any other considerations or improvements that you might make if this was a production application.

### User inputs that the application should require:

* Amount that the loan is for in GBP
* The value of the asset that the loan will be secured against
* The credit score of the applicant (between 1 and 999)

### Metrics that the application should output:

* Whether or not the applicant was successful
* The total number of applicants to date, broken down by their success status
* The total value of loans written to date
* The mean average Loan to Value of all applications received to date
    * Loan to Value (LTV) is the amount that the loan is for, expressed as a percentage of the value of the asset that the loan will be secured against.

### Business rules used to derive whether or not the applicant was successful:

* If the value of the loan is more than £1.5 million or less than £100,000 then the application must be declined
* If the value of the loan is £1 million or more then the LTV must be 60% or less and the credit score of the applicant must be 950 or more
* If the value of the loan is less than £1 million then the following rules apply:
    * If the LTV is less than 60%, the credit score of the applicant must be 750 or more
    * If the LTV is less than 80%, the credit score of the applicant must be 800 or more
    * If the LTV is less than 90%, the credit score of the applicant must be 900 or more
    * If the LTV is 90% or more, the application must be declined

