namespace LiteWeightAPI.Commands.Self.DeleteSelf;

public class DeleteSelf : ICommand<bool>
{
	public required string UserId { get; init; }
}