using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    [Header("Transition Settings")]
    public float transitionDuration = 1f;

    private Coroutine musicTransitionCoroutine;
    private float originalMusicVolume;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        originalMusicVolume = musicSource.volume;
    }

    private void Start()
    {
        PlayMusic("MainMenu-BG");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
            
        if(musicTransitionCoroutine != null)
        {
            StopCoroutine(musicTransitionCoroutine);
        }

        musicTransitionCoroutine = StartCoroutine(TransitionToNewMusic(s.clip));
    }

    private IEnumerator TransitionToNewMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / transitionDuration);
            yield return null;
        }

        musicSource.volume = 0f;


        musicSource.clip = newClip;
        musicSource.Play();

        timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, originalMusicVolume, timer / transitionDuration);
            yield return null;
        }

        musicSource.volume = originalMusicVolume;

        musicTransitionCoroutine = null;

    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        originalMusicVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
