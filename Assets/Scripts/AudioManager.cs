using UnityEngine;

/// <summary>
/// Manages all audio in the game including background music and sound effects
/// Integrates with BeautifulMenuSystem volume controls
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Music Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    
    [Header("Settings")]
    [SerializeField] private bool playMusicOnStart = true;
    [SerializeField] private bool loopMusic = true;
    
    private static AudioManager instance;
    
    public static AudioManager Instance
    {
        get { return instance; }
    }
    
    void Awake()
    {
        // Singleton pattern - persist across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("🎵 AudioManager initialized");
            
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Load saved volume settings
        LoadVolumeSettings();
        
        // Start playing background music
        if (playMusicOnStart && backgroundMusic != null)
        {
            PlayMusic();
        }
    }
    
    void InitializeAudioSources()
    {
        // Create music audio source if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = loopMusic;
            musicSource.playOnAwake = false;
            Debug.Log("🎵 Music AudioSource created");
        }
        
        // Create SFX audio source if not assigned
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            Debug.Log("🔊 SFX AudioSource created");
        }
    }
    
    void LoadVolumeSettings()
    {
        // Load saved volume settings from PlayerPrefs
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        
        Debug.Log($"🎵 Volume settings loaded - Master: {masterVolume}, Music: {musicVolume}, SFX: {sfxVolume}");
    }
    
    public void PlayMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = loopMusic;
            musicSource.Play();
            Debug.Log("🎵 Background music started");
        }
        else
        {
            if (backgroundMusic == null)
                Debug.LogWarning("⚠️ No background music clip assigned!");
            if (musicSource == null)
                Debug.LogWarning("⚠️ Music source not initialized!");
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            Debug.Log("🎵 Background music stopped");
        }
    }
    
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }
    
    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySFX(AudioClip clip, float volumeMultiplier)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volumeMultiplier);
        }
    }
    
    // Volume control methods called by BeautifulMenuSystem
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
    
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
    
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    
    public float GetMasterVolume()
    {
        return AudioListener.volume;
    }
    
    public float GetMusicVolume()
    {
        return musicSource != null ? musicSource.volume : 0f;
    }
    
    public float GetSFXVolume()
    {
        return sfxSource != null ? sfxSource.volume : 0f;
    }
    
    // Useful for debugging
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}

