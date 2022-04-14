using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserApi.Models.RequestResponse
{
    public record AuthenticationRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Username { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string Password { get; init; } = default!;
    }

    public record AuthenticationResponse : ResponseBase
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Token { get; init; }
    }
}
