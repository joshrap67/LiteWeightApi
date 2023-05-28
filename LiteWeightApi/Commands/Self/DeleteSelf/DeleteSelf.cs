namespace LiteWeightAPI.Commands.Self.DeleteSelf;

public class DeleteSelf : ICommand<bool>
{
	public string UserId { get; set; }
}