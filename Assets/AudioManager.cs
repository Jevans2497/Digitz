using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    public AudioSource audioSource;
    public AudioClip menuMusic;

    private bool isMenuMusicPlaying;

    private List<AudioSource> audioSources = new List<AudioSource>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void playMenuMusic(bool withFadeIn) {
        if (!isMenuMusicPlaying) {
            playSound(menuMusic, withFadeIn, 0.8f);
            isMenuMusicPlaying = true;
        }        
    }

    public void stopMenuMusic() {
        if (isMenuMusicPlaying) {
            AudioSource menuMusicAudioSource = audioSources.Find(audioSource => audioSource.clip == menuMusic);
            if (menuMusicAudioSource == null) return;
            StartCoroutine(SharedResources.fadeOutAudio(1.5f, menuMusicAudioSource));
            isMenuMusicPlaying = false;
        }
    }

    public void stopAudioClip(AudioClip audioClip) {
        foreach (var source in audioSources) {
            if (source.clip == audioClip) {
                source.Stop();
                return;
            }
        }
    }

    public void playSound(AudioClip clip, bool withFadeIn = false, float volume = 1f) {
        AudioSource source = getAvailableAudioSource();
        source.clip = clip;
        source.volume = volume;

        if (withFadeIn) {
            StartCoroutine(SharedResources.fadeInAudio(2.0f, source));
        } else {
            source.Play();
        }        
    }

    private AudioSource getAvailableAudioSource() {
        foreach (var source in audioSources) {
            if (!source.isPlaying) return source;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        audioSources.Add(newSource);
        return newSource;
    }
}
