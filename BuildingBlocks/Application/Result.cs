namespace BuildingBlocks.Application;

public class Result
{
    public bool IsSuccess { get; }
    public List<Error> Errors { get; }

    protected Result(bool isSuccess, List<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new();
    }

    public static Result Success() => new(true);

    public static Result Failure(params Error[] errors)
        => new(false, errors.ToList());
}