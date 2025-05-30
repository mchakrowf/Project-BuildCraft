// Models/ApiResponse.cs
using System.Text.Json.Serialization;

namespace ProjectBuildCraft.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("Response")]
        public T Response { get; set; } = default!;
    }
}
