using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
    // serialize fields for main menu buttons
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button continueButton;

    // constructor for injection
    private SceneManagementSc _sceneManagementSc;
    private AudioRecorder _audioRecorder;
    private SceneAudioController _audioController;
    [Inject]
    public void Constructor(SceneManagementSc sceneManagementSc,AudioRecorder audio,SceneAudioController audioController)
    {
        _sceneManagementSc = sceneManagementSc;
        _audioRecorder = audio;
        _audioController = audioController;
    }

    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        // play menu song on start
        _audioController.StartStopSong("play", _audioRecorder.ClipForMenu);
        // adding listeners for start and quit buttons
        playButton.onClick.AddListener(delegate {StartCoroutine( PlayClick(false)); });
        exitButton.onClick.AddListener(delegate { StartCoroutine(QuitClick()); });
        // if data save.json exists unable continue button and add listener
        string path = Application.persistentDataPath + "/save.json";
        if(File.Exists(path))
        {
            continueButton.onClick.AddListener(delegate {StartCoroutine( PlayClick(true)); });
            continueButton.interactable = true;
        }
    }
    // ienumerator for play and continue button click(if no delay click sound losts)
    private IEnumerator PlayClick(bool _continue)
    {
        _audioController.PlayClick();
        yield return new WaitForSeconds(0.3f);
        _sceneManagementSc.SceneToLoad = "GameScene";
        SceneManager.LoadScene("LoadingScreen");
        _sceneManagementSc.Continue = _continue;
    }
    // ienumerator for quit click(if no delay click sound losts)
    private IEnumerator QuitClick()
    {
        _audioController.PlayClick();
        yield return new WaitForSeconds(0.3f);
        Application.Quit();
    }
}
