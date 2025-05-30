// Services/ExoticMetadataService.cs
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.Services
{
    /// <summary>
    /// JSON root shape in exotics-metadata.json
    /// </summary>
    internal class ExoticMetadataRoot
    {
        public List<ExoticMetadata> Armor   { get; set; } = new();
        public List<ExoticMetadata> Weapons { get; set; } = new();
    }

    /// <summary>
    /// Flattened, de-duplicated list of all exotic armor + weapons.
    /// </summary>
    public interface IExoticMetadataService
    {
        IReadOnlyList<ExoticMetadata> All { get; }
    }

    /// <summary>
    /// Loads Data/exotics-metadata.json and merges Armor+Weapons, removing duplicate hashes.
    /// </summary>
    public class ExoticMetadataService : IExoticMetadataService
    {
        public IReadOnlyList<ExoticMetadata> All { get; }

        public ExoticMetadataService(IConfiguration config)
        {
            var path = config["ExoticMetadataPath"];
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new InvalidOperationException($"Cannot find exotic metadata at '{path}'.");

            var json = File.ReadAllText(path);
            var root = JsonSerializer.Deserialize<ExoticMetadataRoot>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidOperationException("Failed to deserialize exotic metadata.");

            // merge armor + weapons, drop duplicates
            All = root.Armor
                      .Concat(root.Weapons)
                      .GroupBy(x => x.Hash)
                      .Select(g => g.First())
                      .ToList();
        }
    }
}
