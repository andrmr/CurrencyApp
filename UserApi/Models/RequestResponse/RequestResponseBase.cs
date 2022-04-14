using System.Text.Json.Serialization;

namespace UserApi.Models.RequestResponse
{
    public abstract record ResponseBase
    {
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; init; }
    };

    public abstract record RequestBase;
}
