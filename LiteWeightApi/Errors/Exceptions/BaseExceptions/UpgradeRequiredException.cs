﻿namespace LiteWeightAPI.Errors.Exceptions.BaseExceptions;

public class UpgradeRequiredException : Exception
{
	public UpgradeRequiredException(string message) : base(message)
	{
	}
}