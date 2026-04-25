using System.Collections.Generic;
using System.IO;
using SQLite;
using UnityEngine;

public class DatabaseManager
{
    private SQLiteConnection _connection;
    private readonly string _dbPath;

    public DatabaseManager(string dbPath = null)
    {
        _dbPath = dbPath ?? Path.Combine(Application.persistentDataPath, "scores.db");
    }

    public void Initialize()
    {
        _connection = new SQLiteConnection(_dbPath);
        _connection.CreateTable<ScoreRecord>();
    }

    public void Close()
    {
        _connection?.Close();
    }

    public int InsertScore(ScoreRecord record)
    {
        _connection.Insert(record);
        return record.Id;
    }

    public List<ScoreRecord> QueryAll()
    {
        return _connection.Table<ScoreRecord>().ToList();
    }

    public List<ScoreRecord> QueryByTrainee(string traineeId)
    {
        return _connection.Table<ScoreRecord>()
            .Where(r => r.TraineeId == traineeId)
            .ToList();
    }

    public List<ScoreRecord> QueryByEquipment(string equipmentType)
    {
        return _connection.Table<ScoreRecord>()
            .Where(r => r.EquipmentType == equipmentType)
            .ToList();
    }
}
