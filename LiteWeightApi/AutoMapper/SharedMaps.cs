using AutoMapper;
using LiteWeightAPI.Utils;
using NodaTime;

namespace LiteWeightAPI.AutoMapper;

public class SharedMaps : Profile
{
	public SharedMaps()
	{
		CreateMap<string, LocalDate>().ConvertUsing(x => ParsingService.ParseStringToLocalDate(x));
		CreateMap<Instant, string>().ConvertUsing(x => ParsingService.ConvertInstantToString(x));
		CreateMap<LocalDate, string>().ConvertUsing(x => ParsingService.ConvertLocalDateToString(x));
	}
}