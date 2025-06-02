using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using GS = GameState.State;

public class ChooseWeaponPanel : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    [SerializeField] private Button[] icons; // all selectable weapon buttons
    [SerializeField] private Button leftHand; // left hand weapon icon slot
    [SerializeField] private Button rightHand; // right hand weapon icon slot
    [SerializeField] private Sprite empty; // default sprite for empty
    [SerializeField] private GameObject panel; // panel to destroy on submit

    private string[] ChoosenWeapons = new string[2]; // selected weapon names

    // dependencies
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
        _sceneAudioController.StartStopSong("play", _audioRecorder.ClipForShop); // starting music
        submitButton.onClick.AddListener(delegate { SubmitButton(); });
        submitButton.enabled = false;

        // initializing empty player hands, till they choose any
        leftHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(false, noImage: empty);
        rightHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(false, noImage: empty);

        // initializing weapons for choose
        int i = 0;
        foreach (Button button in icons)
        {
            var weapon = button.GetComponent<WeaponIcon>();
            weapon.ChoosenItemInitialization(true,weapons[i]); //initializing icon
            weapon.weaponData.currentLevel = 0;  // set weapon level to 1
            button.onClick.AddListener(delegate { OnChoosenItemClick(weapon.weaponData,button); });
            i++;
        }
    }
    // called when weapon is selected
    private void OnChoosenItemClick(WeaponData data,Button butt)
    {
        if (ChoosenWeapons[0] == null)
        {
            leftHand.onClick.RemoveAllListeners();
            ChoosenWeapons[0] = data.name;
            leftHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(true,Weapondata:data);
            leftHand.onClick.AddListener(delegate { RemoveChoosenItem(0); }); 
        }
        else if (ChoosenWeapons[1] == null)
        {
            rightHand.onClick.RemoveAllListeners();
            ChoosenWeapons[1] = data.name; 
            rightHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(true,Weapondata:data);
            rightHand.onClick.AddListener(delegate { RemoveChoosenItem(1); });
        }
        BothWeaponsChoosen();
    }
    // removes a selected weapon
    private void RemoveChoosenItem(int index)
    {
        ChoosenWeapons[index] = null; 
        if (index == 0)
            leftHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(false,noImage:empty);
        else if (index == 1)
            rightHand.GetComponent<WeaponIcon>().ChoosenItemInitialization(false,noImage:empty);
        BothWeaponsChoosen();
    }
    // for submit button listener
    private void SubmitButton()
    {
        _player.InitializeChoozenWeapon(ChoosenWeapons);    // send chosen weapons to player
        _gameState.state = GS.playing;         //change state from pause to play
        _sceneAudioController.PlayClick();
        _sceneAudioController.StartStopSong("replace", _audioRecorder.ClipForRound);
        Destroy(panel);
    }
    // enables/disables icons and submit button based on selection state
    private void BothWeaponsChoosen()
    {
        bool bothSelected = ChoosenWeapons[0] != null && ChoosenWeapons[1] != null;

        foreach(var butt in icons)
            butt.enabled = !bothSelected;

        submitButton.enabled = bothSelected;
    }
}
