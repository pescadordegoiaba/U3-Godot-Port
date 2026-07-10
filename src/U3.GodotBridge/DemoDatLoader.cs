using System.Globalization;
using SDG.Unturned;
using UnityEngine;

namespace U3.GodotBridge;

public static class DemoDatLoader
{
    public static DemoSpawnDefinition LoadDefinition(string resourcesPath)
    {
        var textAsset = Resources.Load<TextAsset>(resourcesPath)
            ?? throw new InvalidOperationException($"No TextAsset registered at Resources path '{resourcesPath}'.");

        return ParseDefinition(textAsset.text);
    }

    public static DemoSpawnDefinition ParseDefinition(string datText)
    {
        var parser = new DatParser();
        var dictionary = parser.Parse(datText);
        if (parser.HasError)
        {
            throw new FormatException($"Invalid demo dat: {parser.ErrorMessage}");
        }

        var name = dictionary.GetString("Name");
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new FormatException("Demo dat requires Name.");
        }

        return new DemoSpawnDefinition
        {
            Id = dictionary.ParseInt32("Id"),
            Name = name,
            Shape = ParseShape(dictionary.GetString("Shape", "Box")),
            Color = ParseColor(dictionary.GetString("Color", "1,1,1")),
            InteractPrompt = dictionary.GetString("InteractPrompt", "Interact"),
            Position = ParseVector3(dictionary.GetString("Position", "0,0,0"))
        };
    }

    private static DemoSpawnShape ParseShape(string value)
    {
        return Enum.TryParse<DemoSpawnShape>(value, ignoreCase: true, out var shape) ? shape : DemoSpawnShape.Box;
    }

    private static Vector3 ParseVector3(string value)
    {
        var parts = value.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 3)
        {
            throw new FormatException($"Vector3 value '{value}' must have three comma-separated floats.");
        }

        return new Vector3(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseFloat(parts[2]));
    }

    private static Color ParseColor(string value)
    {
        var parts = value.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length is not (3 or 4))
        {
            throw new FormatException($"Color value '{value}' must have three or four comma-separated floats.");
        }

        return new Color(
            ParseFloat(parts[0]),
            ParseFloat(parts[1]),
            ParseFloat(parts[2]),
            parts.Length == 4 ? ParseFloat(parts[3]) : 1f);
    }

    private static float ParseFloat(string value)
    {
        return float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
    }
}
