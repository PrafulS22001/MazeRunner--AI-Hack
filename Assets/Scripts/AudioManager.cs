using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;
    
    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip chaseMusic;
    public AudioClip gameOverMusic;
    public AudioClip victoryMusic;
    
    [Header("Sound Effects")]
    public AudioClip playerMoveSound;
    public AudioClip spiderChaseSound;
    public AudioClip powerUpCollectSound;
    public AudioClip playerDamageSound;
    public AudioClip playerHealSound;
    public AudioClip exitReachedSound;
    public AudioClip bushHideSound;
    public AudioClip spiderSpawnSound;
    
    [Header("Ambient Sounds")]
    public AudioClip windSound;
    public AudioClip forestAmbient;
    public AudioClip spiderScuttleSound;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    [Range(0f, 1f)]
    public float ambientVolume = 0.5f;
    
    private static AudioManager instance;
    private bool isChasing = false;
    private Coroutine musicTransitionCoroutine;
    
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject audioManager = new GameObject("AudioManager");
                    instance = audioManager.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        PlayBackgroundMusic();
        PlayAmbientSound();
    }
    
    void InitializeAudioSources()
    {
        // Create audio sources if they don't exist
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        
        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.loop = true;
            ambientSource.playOnAwake = false;
        }
    }
    
    void Update()
    {
        UpdateVolumes();
    }
    
    void UpdateVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
        
        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
        
        if (ambientSource != null)
            ambientSource.volume = ambientVolume * masterVolume;
    }
    
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            if (musicSource.clip != backgroundMusic)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
                Debug.Log("Playing background music");
            }
        }
    }
    
    public void PlayChaseMusic()
    {
        if (chaseMusic != null && !isChasing)
        {
            isChasing = true;
            TransitionToMusic(chaseMusic);
            Debug.Log("Playing chase music");
        }
    }
    
    public void StopChaseMusic()
    {
        if (isChasing)
        {
            isChasing = false;
            TransitionToMusic(backgroundMusic);
            Debug.Log("Stopped chase music");
        }
    }
    
    public void PlayGameOverMusic()
    {
        if (gameOverMusic != null)
        {
            PlayMusic(gameOverMusic, false);
            Debug.Log("Playing game over music");
        }
    }
    
    public void PlayVictoryMusic()
    {
        if (victoryMusic != null)
        {
            PlayMusic(victoryMusic, false);
            Debug.Log("Playing victory music");
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    
    public void PlayPlayerMoveSound()
    {
        if (playerMoveSound != null)
        {
            PlaySFX(playerMoveSound, 0.3f);
        }
    }
    
    public void PlaySpiderChaseSound()
    {
        if (spiderChaseSound != null)
        {
            PlaySFX(spiderChaseSound, 0.5f);
        }
    }
    
    public void PlayPowerUpCollectSound()
    {
        if (powerUpCollectSound != null)
        {
            PlaySFX(powerUpCollectSound, 0.8f);
        }
    }
    
    public void PlayPlayerDamageSound()
    {
        if (playerDamageSound != null)
        {
            PlaySFX(playerDamageSound, 0.7f);
        }
    }
    
    public void PlayPlayerHealSound()
    {
        if (playerHealSound != null)
        {
            PlaySFX(playerHealSound, 0.6f);
        }
    }
    
    public void PlayExitReachedSound()
    {
        if (exitReachedSound != null)
        {
            PlaySFX(exitReachedSound, 0.9f);
        }
    }
    
    public void PlayBushHideSound()
    {
        if (bushHideSound != null)
        {
            PlaySFX(bushHideSound, 0.4f);
        }
    }
    
    public void PlaySpiderSpawnSound()
    {
        if (spiderSpawnSound != null)
        {
            PlaySFX(spiderSpawnSound, 0.6f);
        }
    }
    
    public void PlayAmbientSound()
    {
        if (forestAmbient != null && ambientSource != null)
        {
            ambientSource.clip = forestAmbient;
            ambientSource.Play();
            Debug.Log("Playing ambient forest sound");
        }
    }
    
    public void PlayWindSound()
    {
        if (windSound != null)
        {
            PlaySFX(windSound, 0.3f);
        }
    }
    
    public void PlaySpiderScuttleSound()
    {
        if (spiderScuttleSound != null)
        {
            PlaySFX(spiderScuttleSound, 0.2f);
        }
    }
    
    void PlayMusic(AudioClip clip, bool loop)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }
    
    void TransitionToMusic(AudioClip newClip)
    {
        if (musicTransitionCoroutine != null)
        {
            StopCoroutine(musicTransitionCoroutine);
        }
        
        musicTransitionCoroutine = StartCoroutine(FadeToMusic(newClip));
    }
    
    IEnumerator FadeToMusic(AudioClip newClip)
    {
        float fadeTime = 1f;
        float startVolume = musicSource.volume;
        
        // Fade out current music
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }
        
        // Change music
        musicSource.clip = newClip;
        musicSource.Play();
        
        // Fade in new music
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / fadeTime);
            yield return null;
        }
        
        musicTransitionCoroutine = null;
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
    
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
    }
    
    public void StopAllAudio()
    {
        if (musicSource != null) musicSource.Stop();
        if (sfxSource != null) sfxSource.Stop();
        if (ambientSource != null) ambientSource.Stop();
    }
}
