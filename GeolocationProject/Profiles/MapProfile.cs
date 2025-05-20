using AutoMapper;
using Geolocation.Core.GeoLocationConfig;
using Geolocation.Core.Models;
using GeolocationProject.Dtos;

namespace GeolocationProject.Profiles
{
    public class MapProfile :Profile
    {
        public MapProfile()
        {
            CreateMap<TemporalBlockRequest, TemporalBlock>().ForMember(src => src.CountryName, src => src.MapFrom(x => x.CountryName))
                  .ForMember(src => src.CountryCode, src => src.MapFrom(x => x.CountryCode));


            CreateMap<BlockCountryRequest, BlockedCountries>()
                .ForMember(src => src.CountryName, src => src.MapFrom(x => x.CountryName))
                .ForMember(src => src.CountryCode, src => src.MapFrom(x => x.CountryCode));

           


        }
    }
}
