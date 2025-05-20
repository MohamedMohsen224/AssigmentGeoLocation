using System.ComponentModel.DataAnnotations;

namespace GeolocationProject.Dtos
{
    public class BlockCountryRequest
    {

        

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; }

        public string CountryName { get; set; }

       
    }
}
