using AutoMapper;
using NodaTime;

namespace LiteWeightApi.Utils;

public class SharedMaps : Profile
{
	public SharedMaps()
	{
		CreateMap<string, LocalDate>().ConvertUsing(x => ParsingService.ParseStringToLocalDate(x));
		CreateMap<Instant, string>().ConvertUsing(x => ParsingService.ConvertInstantToString(x));
		CreateMap<LocalDate, string>().ConvertUsing(x => ParsingService.ConvertLocalDateToString(x));
	}
}