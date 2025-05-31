using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/AudioRecorder")]
public class AudioRecorder : ScriptableObject
{
    public AudioClip ClipForRound;
    public AudioClip ClipForMenu;
    public AudioClip ClipForShop;
}
