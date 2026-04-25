using NUnit.Framework;
using System.IO;
using UnityEngine;

public class DatabaseManagerTests
{
    private string _testDbPath;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _testDbPath = Path.Combine(Application.temporaryCachePath, "test_scores.db");
        if (File.Exists(_testDbPath)) File.Delete(_testDbPath);
        _db = new DatabaseManager(_testDbPath);
        _db.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        _db.Close();
        if (File.Exists(_testDbPath)) File.Delete(_testDbPath);
    }

    [Test]
    public void InsertScore_ValidRecord_ReturnsPositiveId()
    {
        var record = new ScoreRecord
        {
            TraineeName = "张三",
            TraineeId = "001",
            EquipmentType = "ZhuangKaChe_TypeA",
            Mode = "assessment",
            Score = 85,
            DurationSeconds = 272,
            ErrorCount = 1,
            Date = "2026-04-25"
        };
        int id = _db.InsertScore(record);
        Assert.Greater(id, 0);
    }

    [Test]
    public void QueryScores_ByTraineeId_ReturnsMatchingRecords()
    {
        var record = new ScoreRecord
        {
            TraineeName = "李四", TraineeId = "002",
            EquipmentType = "ZhuangKaChe_TypeA", Mode = "assessment",
            Score = 90, DurationSeconds = 240, ErrorCount = 0, Date = "2026-04-25"
        };
        _db.InsertScore(record);

        var results = _db.QueryByTrainee("002");
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("李四", results[0].TraineeName);
    }

    [Test]
    public void QueryScores_AllRecords_ReturnsAll()
    {
        _db.InsertScore(new ScoreRecord { TraineeName = "A", TraineeId = "1",
            EquipmentType = "TypeA", Mode = "assessment",
            Score = 70, DurationSeconds = 300, ErrorCount = 2, Date = "2026-04-25" });
        _db.InsertScore(new ScoreRecord { TraineeName = "B", TraineeId = "2",
            EquipmentType = "TypeB", Mode = "assessment",
            Score = 80, DurationSeconds = 280, ErrorCount = 1, Date = "2026-04-25" });

        var all = _db.QueryAll();
        Assert.AreEqual(2, all.Count);
    }
}
