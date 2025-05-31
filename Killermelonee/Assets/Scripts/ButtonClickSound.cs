using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    private Button _button;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(_audioSource.Play);
    }
    private void PlayClick()
    {
        var click = Instantiate(_audioSource.gameObject);
    }
}
