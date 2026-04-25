using UnityEngine;
using Newtonsoft.Json;

public class TrainingFlowController : MonoBehaviour
{
    public StepProgressBarController progressBar;
    public SubtitleController subtitle;
    public FeedbackOverlayController feedback;
    public PartSelector partSelector;

    private TrainingSession _session;
    private TrainingStateMachine _stateMachine;

    public void LoadConfig(string fileName)
    {
        var config = JsonConfigLoader.LoadFromFile(fileName);
        StartTraining(config, TrainingMode.Training);
    }

    public void StartTraining(TrainingConfig config, TrainingMode mode)
    {
        _session = new TrainingSession(config, mode);
        _stateMachine = new TrainingStateMachine();
        _stateMachine.Start();

        if (partSelector != null)
            partSelector.OnPartSelected += HandlePartInteraction;

        ShowCurrentStep();
    }

    private void HandlePartInteraction(PartInfo info)
    {
        if (_session == null || _session.IsComplete) return;

        var step = _session.CurrentStep;
        bool correct = StepValidator.Validate(step, info?.ObjectName);

        if (correct)
        {
            feedback.ShowCorrect();
            AudioManager.Instance?.PlayCorrect();
            _session.AdvanceStep();

            if (_session.IsComplete)
                FinishTraining();
            else
                ShowCurrentStep();
        }
        else if (_session.Mode == TrainingMode.Assessment)
        {
            _session.RecordError(step.StepId);
            feedback.ShowError();
            AudioManager.Instance?.PlayError();
        }
        else
        {
            feedback.ShowError();
            AudioManager.Instance?.PlayError();
            subtitle.ShowSubtitle($"提示：{step.HintText}");
        }
    }

    private void ShowCurrentStep()
    {
        var step = _session.CurrentStep;
        if (step == null) return;

        int current = _session.CurrentStepIndex + 1;
        int total   = _session.Config.Steps.Count;

        progressBar.UpdateProgress(current, total, step.StepName);

        if (_session.Mode == TrainingMode.Training)
        {
            subtitle.ShowSubtitle(step.HintText);
            AudioManager.Instance?.PlayVoice(step.VoiceFile);
        }
        else
        {
            subtitle.ClearSubtitle();
        }
    }

    private void FinishTraining()
    {
        _stateMachine.Complete();
        int score = _session.GetFinalScore();
        int duration = (int)_session.GetDurationSeconds();

        if (_session.Mode == TrainingMode.Assessment)
        {
            var record = new ScoreRecord
            {
                TraineeName = AppManager.Instance.Session.TraineeName,
                TraineeId   = AppManager.Instance.Session.TraineeId,
                EquipmentType = _session.Config.EquipmentType,
                Mode = "assessment",
                Score = score,
                DurationSeconds = duration,
                ErrorCount = _session.ErrorStepIds.Count,
                Date = System.DateTime.Now.ToString("yyyy-MM-dd"),
                ErrorStepIds = JsonConvert.SerializeObject(_session.ErrorStepIds)
            };
            record.PassScore = _session.Config.PassScore;
            int insertedId = AppManager.Instance.Database.InsertScore(record);
            AppManager.Instance.Session.CurrentScoreRecordId = insertedId;
        }

        SceneLoader.Instance?.LoadScene("ScoreManagement");
    }

    private void OnDisable()
    {
        if (partSelector != null)
            partSelector.OnPartSelected -= HandlePartInteraction;
    }
}
