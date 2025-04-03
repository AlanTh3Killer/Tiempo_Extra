using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Music Clips")]
    public AudioClip backgroundMusic;
    public AudioClip menuMusic;
    public AudioClip battleMusic;

    [Header("SFX Clips")]
    public AudioClip[] attackSounds;
    public AudioClip[] hitSounds;
    public AudioClip crowdAmbience;

    [Header("Volume Settings")]
    [Range(0f, 1f)][SerializeField] private float _musicVolume = 0.5f;
    [Range(0f, 1f)][SerializeField] private float _sfxVolume = 0.5f;
    [Range(0f, 1f)][SerializeField] private float _ambientVolume = 0.5f;

    [Header("Fade Settings")]
    public float ambientFadeSpeed = 0.5f;

    // Propiedades públicas para control de volumen
    public float musicVolume
    {
        get => _musicVolume;
        set => _musicVolume = Mathf.Clamp01(value);
    }

    public float sfxVolume
    {
        get => _sfxVolume;
        set => _sfxVolume = Mathf.Clamp01(value);
    }

    public float ambientVolume
    {
        get => _ambientVolume;
        set => _ambientVolume = Mathf.Clamp01(value);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        // Configuración básica de AudioSources
        musicSource = musicSource ? musicSource : gameObject.AddComponent<AudioSource>();
        sfxSource = sfxSource ? sfxSource : gameObject.AddComponent<AudioSource>();
        ambientSource = ambientSource ? ambientSource : gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        ambientSource.loop = true;

        // Cargar configuración guardada
        LoadVolumeSettings();
        HandleSceneChange(SceneManager.GetActiveScene().name);
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.5f);
        ApplyVolumeSettings();
    }

    private void ApplyVolumeSettings()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        ambientSource.volume = ambientVolume;

        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("AmbientVolume", ambientVolume);
        PlayerPrefs.Save();
    }

    #region Scene Management
    public void HandleSceneChange(string sceneName)
    {
        if (sceneName.ToLower().Contains("menu"))
        {
            PlayMenuMusic();
        }
        else if (IsBattleScene(sceneName))
        {
            StartBattleScene();
        }
        else
        {
            PlayDefaultMusic();
        }
    }

    private bool IsBattleScene(string sceneName)
    {
        return sceneName.ToLower().Contains("level") ||
               sceneName.ToLower().Contains("battle") ||
               sceneName.ToLower().Contains("stage");
    }
    #endregion

    #region Music Control
    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
        FadeOutAmbience();
    }

    public void StartBattleScene()
    {
        PlayMusic(battleMusic);
        PlayAmbience(crowdAmbience);
    }

    public void PlayDefaultMusic()
    {
        PlayMusic(backgroundMusic);
        StopAmbience();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }
    #endregion

    #region Ambient Control
    public void PlayCrowdAmbience()
    {
        if (crowdAmbience != null && ambientSource != null)
        {
            ambientSource.clip = crowdAmbience;
            ambientSource.volume = ambientVolume;
            ambientSource.Play();
            Debug.Log("Sonido de multitud iniciado");
        }
        else
        {
            Debug.LogWarning("Falta asignar crowdAmbience o ambientSource en SoundManager");
        }
    }

    public void PlayAmbience(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null || ambientSource == null) return;

        ambientSource.clip = clip;
        ambientSource.volume = ambientVolume * Mathf.Clamp01(volumeMultiplier);
        ambientSource.Play();
    }

    public void StopAmbience()
    {
        if (ambientSource != null) ambientSource.Stop();
    }

    public void FadeOutAmbience()
    {
        StartCoroutine(FadeAmbience(0f, ambientFadeSpeed));
    }

    private IEnumerator FadeAmbience(float targetVolume, float duration)
    {
        float startVolume = ambientSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ambientSource.volume = targetVolume;
        if (targetVolume <= 0f) ambientSource.Stop();
    }
    #endregion

    #region SFX Control
    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip, sfxVolume * Mathf.Clamp01(volumeMultiplier));
    }

    public void PlayRandomSFX(AudioClip[] clips, float volumeMultiplier = 1f)
    {
        if (clips == null || clips.Length == 0 || sfxSource == null) return;

        int randomIndex = Random.Range(0, clips.Length);
        PlaySFX(clips[randomIndex], volumeMultiplier);
    }

    public void PlayUISound(AudioClip clip, float volumeMultiplier = 1f)
    {
        PlaySFX(clip, volumeMultiplier);
    }
    #endregion

    #region Editor Integration
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorApplication.isPlaying)
        {
            ApplyVolumeSettings();
        }
    }
#endif
    #endregion
}
