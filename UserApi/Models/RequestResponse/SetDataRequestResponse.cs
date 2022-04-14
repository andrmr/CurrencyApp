using System.ComponentModel.DataAnnotations;

namespace UserApi.Models.RequestResponse
{
    public record SetDataRequest
    (
        [Required(AllowEmptyStrings = false)]
        string Username,

        [Required]
        UserData Data
    );

    public record SetDataResponse : ResponseBase;
}
