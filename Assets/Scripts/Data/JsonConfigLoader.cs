using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonConfigLoader
{
    public static TrainingConfig LoadFromFile(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "configs", fileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"[JsonConfigLoader] Config file not found: {path}");
            return null;
        }
        string json = File.ReadAllText(path);
        return ParseConfig(json);
    }

    public static TrainingConfig ParseConfig(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;
        try
        {
            return JsonConvert.DeserializeObject<TrainingConfig>(json);
        }
        catch (JsonException ex)
        {
            Debug.LogError($"[JsonConfigLoader] JSON parse error: {ex.Message}");
            return null;
        }
    }
}
