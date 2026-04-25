using NUnit.Framework;

public class JsonConfigLoaderTests
{
    private const string SampleJson = @"{
      ""training_name"": ""测试训练"",
      ""equipment_type"": ""ZhuangKaChe_TypeA"",
      ""personnel_total"": 3,
      ""pass_score"": 60,
      ""time_limit_seconds"": 600,
      ""steps"": [
        {
          ""step_id"": 1,
          ""step_name"": ""展开支腿"",
          ""personnel_count"": 2,
          ""hint_text"": ""提示文字"",
          ""content_info"": ""内容信息"",
          ""voice_file"": ""audio/step_01.mp3"",
          ""target_object"": ""HydraulicLeg_L"",
          ""trigger_action"": ""click"",
          ""trigger_params"": {},
          ""score_weight"": 10,
          ""timeout_seconds"": 60
        }
      ]
    }";

    [Test]
    public void ParseConfig_ValidJson_ReturnsCorrectConfig()
    {
        var config = JsonConfigLoader.ParseConfig(SampleJson);

        Assert.IsNotNull(config);
        Assert.AreEqual("测试训练", config.TrainingName);
        Assert.AreEqual("ZhuangKaChe_TypeA", config.EquipmentType);
        Assert.AreEqual(60, config.PassScore);
        Assert.AreEqual(1, config.Steps.Count);
    }

    [Test]
    public void ParseConfig_Step_HasCorrectFields()
    {
        var config = JsonConfigLoader.ParseConfig(SampleJson);
        var step = config.Steps[0];

        Assert.AreEqual(1, step.StepId);
        Assert.AreEqual("展开支腿", step.StepName);
        Assert.AreEqual(2, step.PersonnelCount);
        Assert.AreEqual("HydraulicLeg_L", step.TargetObject);
        Assert.AreEqual("click", step.TriggerAction);
        Assert.AreEqual(10, step.ScoreWeight);
    }

    [Test]
    public void ParseConfig_NullInput_ReturnsNull()
    {
        var config = JsonConfigLoader.ParseConfig(null);
        Assert.IsNull(config);
    }

    [Test]
    public void ParseConfig_EmptyString_ReturnsNull()
    {
        var config = JsonConfigLoader.ParseConfig("");
        Assert.IsNull(config);
    }

    [Test]
    public void ParseConfig_MalformedJson_ReturnsNull()
    {
        var config = JsonConfigLoader.ParseConfig("{not valid json}");
        Assert.IsNull(config);
    }

    [Test]
    public void ValidateTotalWeight_SingleStep10_ReturnsFalse()
    {
        var config = JsonConfigLoader.ParseConfig(SampleJson);
        bool result = TrainingConfig.ValidateTotalWeight(config);
        Assert.IsFalse(result);
    }

    [Test]
    public void ValidateTotalWeight_StepsSumTo100_ReturnsTrue()
    {
        var config = new TrainingConfig
        {
            Steps = new System.Collections.Generic.List<TrainingStep>
            {
                new TrainingStep { ScoreWeight = 60 },
                new TrainingStep { ScoreWeight = 40 }
            }
        };
        Assert.IsTrue(TrainingConfig.ValidateTotalWeight(config));
    }

    [Test]
    public void ParseConfig_ValidJsonString_ReturnsConfig()
    {
        var config = JsonConfigLoader.ParseConfig(SampleJson);
        Assert.IsNotNull(config);
        Assert.AreEqual("测试训练", config.TrainingName);
    }
}
