public class UserSession
{
    public string TraineeName { get; set; }
    public string TraineeId { get; set; }
    public UserRole Role { get; set; } = UserRole.Trainee;
    public string SelectedEquipmentType { get; set; }
    public int CurrentScoreRecordId { get; set; }
}

public enum UserRole { Trainee, Instructor }
