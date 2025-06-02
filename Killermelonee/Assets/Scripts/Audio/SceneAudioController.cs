using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;
using static Unity.VisualScripting.Member;

public class SceneAudioController : MonoBehaviour
{
    // max value for songs and min value when they quiet dawn
    [SerializeField] private float soundsMaxValue;
    [SerializeField] private float soundsMinValue;

    // two audio Sources for changing(replacing) one song by another
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private AudioSource musicAudio1;

    // source for button clicks on UI
    [SerializeField] private AudioSource clickAudio;

    // bool for right distribution of duties between music audio sources
    private bool _firstSontActive = false;

    // dependencies
    private AudioRecorder _audioRecorder; //scriptable with sounds
    [Inject]
    public void Constructor(AudioRecorder audioRecorder)
    {
        _audioRecorder = audioRecorder;
    }
    // function for start or replace for songs
    // "play" move is only for first time, after only replace used cause music is always on
    public void StartStopSong(string move, AudioClip clip)
    {
        if(move == "play")
        {
            AudioSource current = _firstSontActive ? musicAudio : musicAudio1; // choosing one of audio sources
            current.clip = clip;
            current.volume = 0f;
            current.Play();
            StartCoroutine(FadeIn(current,0,soundsMaxValue));
        }
        if(move == "replace")
        {
            StartCoroutine(ReplaceSong(clip));
            _firstSontActive = !_firstSontActive; //changing bool to oposite to make another audio source became current in next code
        }
    }
    // coroutine => replace current song by other 
    private IEnumerator ReplaceSong(AudioClip clip)
    {
        // initializing which source play(current) to change it by one that not(next)
        AudioSource current = _firstSontActive ? musicAudio : musicAudio1;
        AudioSource next = !_firstSontActive ? musicAudio : musicAudio1;

        yield return StartCoroutine(FadeOut(current,soundsMaxValue,0)); 
        current.Stop();
        next.clip = clip;
        next.volume = 0f;
        next.Play();
        yield return StartCoroutine(FadeIn(next,0,soundsMaxValue)); 
    }
    // function to slowly quiet dawn song
    private IEnumerator FadeOut(AudioSource source,float soundMax,float soundMin)
    {
        for (float t = 0; t < 1.5f; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(soundMax, soundMin, t / 1.5f);
            yield return null;
        }
    }
    // function to slowly make louder song
    private IEnumerator FadeIn(AudioSource source,float soundMin,float soundMax)
    {
        for (float t = 0; t < 1.5f; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(soundMin, soundMax, t / 1.5f);
            yield return null;
        }
    }
    //make click sound on time of need
    public void PlayClick()
    {
        clickAudio.clip = _audioRecorder.ClipForButtonClick;
        clickAudio.Play();
    }
    //make sound for click on weapon icon
    public void PlayWeaponIconClick()
    {
        clickAudio.clip = _audioRecorder.ClipForWeapoIconClick;
        clickAudio.Play();
    }
    // function to quiet dawn or make song louder(back)
    public void QuietLouderSong(bool quiet)
    {
        AudioSource current = _firstSontActive ? musicAudio : musicAudio1;
        if (quiet)
            StartCoroutine( FadeOut(current,soundsMaxValue,soundsMinValue));
        else
            StartCoroutine( FadeIn(current,soundsMinValue,soundsMaxValue));
    }
}
