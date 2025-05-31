using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using GS = GameState.State;

public class ChooseWeaponPanel : MonoBehaviour
{
    [SerializeField] private Button[] icons;
    private string[] ChoosenWeapons = new string[2];
    [SerializeField] private Button leftHand;
    [SerializeField] private Button rightHand;
    [SerializeField] private Sprite empty;
    [SerializeField] private Button submitButton;
    [SerializeField] private GameObject panel;

    [Inject]private DiContainer _container;
    private ExceptionPanel _exceptionPanel;
    private List<WeaponData> weapons;
    private Player _player;
    private UI _ui;
    private GameState _gameState;
    private AudioRecorder _audioRecorder;
    private SceneAudioController _sceneAudioController;
    [Inject]
    public void Construct(WeaponList list,Player player,ExceptionPanel panel,UI ui,GameState state,AudioRecorder recorder,SceneAudioController audioController)
    {
        weapons = list.Weapons;
        _player = player;
        _exceptionPanel = panel;
        _ui = ui;
        _gameState = state;
        _audioRecorder = recorder;
        _sceneAudioController = audioController;
    }
    private void Start()
    {
        _sceneAudioController.StartStopSong("play", _audioRecorder.ClipForShop);
        submitButton.onClick.AddListener(delegate { SubmitButton(); });
        submitButton.enabled = false;
        leftHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(false, noImage: empty);
        rightHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(false, noImage: empty);
        icons = GetComponentsInChildren<Button>();
        int i = 0;
        foreach (Button button in icons)
        {
            var weapon = button.GetComponent<WeaponIcon>();
            weapon.ChoosenItemInitialization(true,weapons[i]);
            weapon.weaponData.currentLevel = 0;
            button.onClick.AddListener(delegate { OnChoosenItemClick(weapon.weaponData,button); });
            i++;
        }
    }
    private void OnChoosenItemClick(WeaponData data,Button butt)
    {
        if (ChoosenWeapons[0] == null)
        {
            leftHand.onClick.RemoveAllListeners();
            ChoosenWeapons[0] = data.name;
            leftHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(true,Weapondata:data);
            leftHand.onClick.AddListener(delegate { RemoveChoosenItem(butt,0); });
            butt.enabled = false;
        }
        else if (ChoosenWeapons[1] == null)
        {
            rightHand.onClick.RemoveAllListeners();
            ChoosenWeapons[1] = data.name;
            rightHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(true,Weapondata:data);
            rightHand.onClick.AddListener(delegate { RemoveChoosenItem(butt,1); });
            butt.enabled = true;
        }
        else
        {
            var panel = _container.InstantiatePrefab(_exceptionPanel);
            panel.GetComponent<ExceptionPanel>().SendMessage("you have both hand full!");
            panel.transform.SetParent(_ui.transform, false);
        }
        if(ChoosenWeapons[0] != null && ChoosenWeapons[1] != null)
        {
            submitButton.enabled = true;
        }
    }
    private void RemoveChoosenItem(Button butt,int index)
    {
        butt.enabled = true;
        ChoosenWeapons[index] = null;
        if (index == 0)
            leftHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(false,noImage:empty);
        else if (index == 1)
            rightHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(false,noImage:empty);
        submitButton.enabled = false;
    }
    private void SubmitButton()
    {
        _player.InitializeChoozenWeapon(ChoosenWeapons);
        _gameState.state = GS.playing;
        _sceneAudioController.PlayClick();
        _sceneAudioController.StartStopSong("replace", _audioRecorder.ClipForRound);
        Destroy(panel);
    }
}
