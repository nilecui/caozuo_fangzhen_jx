using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class TrainingConfig
{
    [JsonProperty("training_name")]
    public string TrainingName { get; set; }

    [JsonProperty("equipment_type")]
    public string EquipmentType { get; set; }

    [JsonProperty("personnel_total")]
    public int PersonnelTotal { get; set; }

    [JsonProperty("pass_score")]
    public int PassScore { get; set; }

    [JsonProperty("time_limit_seconds")]
    public int TimeLimitSeconds { get; set; }

    [JsonProperty("steps")]
    public List<TrainingStep> Steps { get; set; } = new();

    public static bool ValidateTotalWeight(TrainingConfig config)
    {
        int total = 0;
        foreach (var step in config.Steps)
            total += step.ScoreWeight;
        return total == 100;
    }
}

[System.Serializable]
public class TrainingStep
{
    [JsonProperty("step_id")]
    public int StepId { get; set; }

    [JsonProperty("step_name")]
    public string StepName { get; set; }

    [JsonProperty("personnel_count")]
    public int PersonnelCount { get; set; }

    [JsonProperty("hint_text")]
    public string HintText { get; set; }

    [JsonProperty("content_info")]
    public string ContentInfo { get; set; }

    [JsonProperty("voice_file")]
    public string VoiceFile { get; set; }

    [JsonProperty("target_object")]
    public string TargetObject { get; set; }

    [JsonProperty("trigger_action")]
    public string TriggerAction { get; set; }

    [JsonProperty("trigger_params")]
    public Dictionary<string, object> TriggerParams { get; set; } = new();

    [JsonProperty("score_weight")]
    public int ScoreWeight { get; set; }

    [JsonProperty("timeout_seconds")]
    public int TimeoutSeconds { get; set; }
}
