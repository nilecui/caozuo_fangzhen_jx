public class UserSession
{
    public string TraineeName { get; set; }
    public string TraineeId { get; set; }
    public UserRole Role { get; set; } = UserRole.Trainee;
    public string SelectedEquipmentType { get; set; } = "sample_training.json";
    public int CurrentScoreRecordId { get; set; }
    public TrainingMode CurrentMode { get; set; } = TrainingMode.Training;
}

public enum UserRole { Trainee, Instructor }
