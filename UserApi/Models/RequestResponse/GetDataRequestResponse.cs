using System.ComponentModel.DataAnnotations;

namespace UserApi.Models.RequestResponse
{
    public record GetDataRequest
    (
        [Required(AllowEmptyStrings = false)]
        string Username
    );

    public record GetDataResponse : ResponseBase
    {
        public UserData? Data { get; init; }
    }
}
