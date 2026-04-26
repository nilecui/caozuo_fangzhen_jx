using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource _voiceSource;
    private AudioSource _sfxSource;

    public AudioClip ClipCorrect;
    public AudioClip ClipError;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _voiceSource = gameObject.AddComponent<AudioSource>();
        _sfxSource   = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip);
    }

    public void PlayCorrect() => PlaySFX(ClipCorrect);
    public void PlayError()   => PlaySFX(ClipError);

    public Coroutine PlayVoice(string relativePath, System.Action onComplete = null)
    {
        if (string.IsNullOrEmpty(relativePath)) { onComplete?.Invoke(); return null; }
        return StartCoroutine(LoadAndPlayVoice(relativePath, onComplete));
    }

    private IEnumerator LoadAndPlayVoice(string relativePath, System.Action onComplete)
    {
        string fullPath = "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);

        // AudioType.MPEG requires com.unity.modules.unitywebrequestaudio.
        // Use AudioType.UNKNOWN as fallback so the code compiles on all setups.
        var req = new UnityWebRequest(fullPath, UnityWebRequest.kHttpVerbGET);
        req.downloadHandler = new DownloadHandlerBuffer();
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[AudioManager] Voice load failed: {req.error}");
            req.Dispose();
            onComplete?.Invoke();
            yield break;
        }

        // Write raw bytes to a temp file and load via AudioSource (WAV only, MP3 not supported this way)
        // For full MP3 support enable com.unity.modules.unitywebrequestaudio and restore
        // UnityWebRequestMultimedia.GetAudioClip / DownloadHandlerAudioClip.GetContent.
        Debug.LogWarning("[AudioManager] Voice playback requires com.unity.modules.unitywebrequestaudio. Skipping.");
        req.Dispose();
        onComplete?.Invoke();
    }

    public void StopVoice() => _voiceSource.Stop();
}
