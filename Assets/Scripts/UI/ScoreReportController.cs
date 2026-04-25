using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;

public class ScoreReportController : MonoBehaviour
{
    private ScoreRecord _record;

    private void OnEnable()
    {
        int scoreId = AppManager.Instance.Session.CurrentScoreRecordId;
        var records = AppManager.Instance.Database.QueryAll();
        _record = records.Find(r => r.Id == scoreId) ?? records.LastOrDefault();

        var root = GetComponent<UIDocument>().rootVisualElement;
        if (_record == null) return;

        root.Q<Label>("trainee-info").text   = $"学员：{_record.TraineeName}（{_record.TraineeId}）";
        root.Q<Label>("equipment-info").text = $"装备：{_record.EquipmentType}";
        root.Q<Label>("date-info").text      = $"日期：{_record.Date}";
        root.Q<Label>("score-value").text    = _record.Score.ToString();
        root.Q<Label>("duration-value").text = _record.DurationSeconds.ToString();
        root.Q<Label>("error-value").text    = _record.ErrorCount.ToString();

        int passScore = _record.PassScore > 0 ? _record.PassScore : 60;
        bool passed = _record.Score >= passScore;
        var passLabel = root.Q<Label>("pass-label");
        passLabel.text  = passed ? "✓ 合格" : "✗ 不合格";
        passLabel.style.color = passed
            ? new StyleColor(new Color(0.3f, 0.68f, 0.31f))
            : new StyleColor(new Color(0.96f, 0.26f, 0.21f));

        var errorIds = !string.IsNullOrEmpty(_record.ErrorStepIds)
            ? JsonConvert.DeserializeObject<System.Collections.Generic.List<int>>(_record.ErrorStepIds)
            : new System.Collections.Generic.List<int>();
        root.Q<Label>("error-steps-list").text =
            errorIds.Count > 0 ? string.Join("、", errorIds.Select(id => $"步骤{id}")) : "无";

        root.Q<Button>("btn-export-csv").RegisterCallback<ClickEvent>(_ => ExportCsv());
        root.Q<Button>("btn-menu").RegisterCallback<ClickEvent>(_ =>
            SceneLoader.Instance.LoadScene("MainMenu"));
        root.Q<Button>("btn-retrain").RegisterCallback<ClickEvent>(_ =>
            SceneLoader.Instance.LoadScene("OperationTraining"));
    }

    private void ExportCsv()
    {
        var all = AppManager.Instance.Database.QueryAll();
        var sb = new StringBuilder();
        sb.AppendLine("学员姓名,工号,装备类型,模式,总分,用时(秒),错误次数,日期");
        foreach (var r in all)
            sb.AppendLine($"{r.TraineeName},{r.TraineeId},{r.EquipmentType},{r.Mode},{r.Score},{r.DurationSeconds},{r.ErrorCount},{r.Date}");

        string path = Path.Combine(Application.persistentDataPath, "scores_export.csv");
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        Debug.Log($"[ScoreReport] CSV exported to: {path}");

#if UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start("explorer.exe", $"/select,{path}");
#endif
    }
}
