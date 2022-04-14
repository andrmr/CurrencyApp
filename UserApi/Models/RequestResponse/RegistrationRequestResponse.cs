using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserApi.Models.RequestResponse
{
    public record RegistrationRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Username { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string Password { get; init; } = default!;
    }

    public record RegistrationResponse : ResponseBase
    {
        [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Token { get; init; }
    }
}
