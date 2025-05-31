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

    // source for button clicks on UI, clip is always same
    [SerializeField] private AudioSource clickAudio;

    // bool for right distribution of duties between music audio sources
    private bool _firstSontActive = false;

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
            StartCoroutine(FadeIn(current));
            _firstSontActive = !_firstSontActive; //changing bool to oposite to make another audio source became current in next code
        }
        if(move == "replace")
        {
            StartCoroutine(ReplaceSong(clip));
            _firstSontActive = !_firstSontActive; //changing bool to oposite to make another audio source became current in next code
        }
    }
    // coroutine => replace current song by other one slowly fade out one by another
    private IEnumerator ReplaceSong(AudioClip clip)
    {
        // initializing which source play(current) to change it by one that not(next)
        AudioSource current = _firstSontActive ? musicAudio : musicAudio1;
        AudioSource next = !_firstSontActive ? musicAudio : musicAudio1;

        yield return StartCoroutine(FadeOut(current)); // slowly cut current song
        next.clip = clip;
        next.volume = 0f;
        next.Play();
        yield return StartCoroutine(FadeIn(next)); //slowly let new one reveal
    }
    // function to slowly quiet dawn song
    private IEnumerator FadeOut(AudioSource source)
    {
        for (float t = 0; t < 1.5f; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(soundsMaxValue, 0f, t / 1.5f);
            yield return null;
        }
        source.Stop();
    }
    // function to slowly make louder song
    private IEnumerator FadeIn(AudioSource source)
    {
        for (float t = 0; t < 1.5f; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, soundsMaxValue, t / 1.5f);
            yield return null;
        }
    }
    // let any class(monobehaviour) to make click sound on time of need
    public void PlayClick()
    {
        clickAudio.Play();
    }
    // function to quiet dawn or make song louder(back) for pause or any other moments
    public void QuietLouderSong(bool quiet)
    {
        AudioSource current = _firstSontActive ? musicAudio : musicAudio1;
        if (quiet)
            QuietDawn(current);
        else
            LouderUp(current);
    }
    // makes song quiter
    private IEnumerator QuietDawn(AudioSource source)
    {
        for (float t = 0; t < 1.5f; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(soundsMaxValue, soundsMinValue, t / 1.5f);
            yield return null;
        }
    }
    // makes song louder
    private IEnumerator LouderUp(AudioSource source)
    {
        for (float t = 0; t < 1.5f; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(soundsMinValue, soundsMaxValue, t / 1.5f);
            yield return null;
        }
    }
}
