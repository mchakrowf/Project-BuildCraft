// Models/BungieOptions.cs
namespace ProjectBuildCraft.Models
{
    public class BungieOptions
    {
        public string ApiKey       { get; set; } = null!;
        public int    ClientId     { get; set; }
        public string ClientSecret { get; set; } = null!;
    }
}
