using SQLite;

[Table("scores")]
public class ScoreRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string TraineeName { get; set; }
    public string TraineeId { get; set; }
    public string EquipmentType { get; set; }
    public string Mode { get; set; }          // "training" or "assessment"
    public int Score { get; set; }
    public int DurationSeconds { get; set; }
    public int ErrorCount { get; set; }
    public string Date { get; set; }          // "YYYY-MM-DD"
    public string ErrorStepIds { get; set; }  // JSON array string, e.g. "[2,5]"
}
