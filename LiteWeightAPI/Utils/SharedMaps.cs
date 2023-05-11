using AutoMapper;
using NodaTime;

namespace LiteWeightAPI.Utils;

public class SharedMaps : Profile
{
	public SharedMaps()
	{
		CreateMap<string, LocalDate>().ConvertUsing(x => ParsingService.ParseDateToNodatime(x));
		CreateMap<Instant, string>().ConvertUsing(x => ParsingService.ConvertInstantToString(x));
		CreateMap<LocalDate, string>().ConvertUsing(x => ParsingService.ConvertLocalDateToString(x));
	}
}