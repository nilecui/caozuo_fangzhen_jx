using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }
    public UserSession Session { get; private set; } = new UserSession();
    public DatabaseManager Database { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Database = new DatabaseManager();
        Database.Initialize();
    }

    private void OnApplicationQuit()
    {
        Database?.Close();
    }
}
