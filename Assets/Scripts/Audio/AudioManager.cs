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
        if (Instance != null) { Destroy(gameObject); return; }
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
        return StartCoroutine(LoadAndPlayVoice(relativePath, onComplete));
    }

    private IEnumerator LoadAndPlayVoice(string relativePath, System.Action onComplete)
    {
        string fullPath = "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
        using var req = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.MPEG);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[AudioManager] Voice load failed: {req.error}");
            onComplete?.Invoke();
            yield break;
        }

        var clip = DownloadHandlerAudioClip.GetContent(req);
        _voiceSource.clip = clip;
        _voiceSource.Play();
        yield return new WaitForSeconds(clip.length);
        onComplete?.Invoke();
    }

    public void StopVoice() => _voiceSource.Stop();
}
