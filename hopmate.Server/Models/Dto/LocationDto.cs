using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Dto
{
    public class LocationDto
    {
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Código postal inválido.")]
        public string PostalCode { get; set; } = string.Empty;
    }
}
