using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class gameInstaller : MonoInstaller
{
    [SerializeField] private TextAsset roundsJsonText;
    [SerializeField] private TextAsset weaponJsonData;
    [SerializeField] private TextAsset PlayerData;
    [SerializeField] private Player _player;
    [SerializeField] private UI _uI;
    [SerializeField] private GameState GameState;
    [SerializeField] private ExceptionPanel _exceptionPanel;
    [SerializeField] private SceneManagementSc _sceneManagment;
    [SerializeField] private InfoPanel _infoPanel;
    [SerializeField] private DescriptionPanel _descriptionPanel;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private EnemySpawnerController _enemySpawnerController;
    [SerializeField] private AudioRecorder _audioRecorder;
    [SerializeField] private SceneAudioController _audioController;
    public override void InstallBindings()
    {
        var rounds = JsonUtility.FromJson<RoundsData>(roundsJsonText.text);
        Container.Bind<RoundsData>().FromInstance(rounds).AsSingle();

        var weapons = JsonUtility.FromJson<WeaponList>(weaponJsonData.text);
        Container.Bind<WeaponList>().FromInstance(weapons).AsSingle();

        Container.BindFactory<string, DescriptionPanel,DescriptionPanelFactory>().FromMethod((container, text) =>
        {
            var panel = container.InstantiatePrefab(_descriptionPanel);
            var descr = panel.GetComponent<DescriptionPanel>();

            descr.SetDescription(text);
            container.Inject(descr);
            return descr;
        });

        Container.BindFactory<WeaponData, InfoPanel, InfoPanelFactory>().FromMethod((container, data) =>
        {
            var panel = container.InstantiatePrefab(_infoPanel);
            var info = panel.GetComponent<InfoPanel>();

            info.SetData(data);
            container.Inject(info);
            return info;
        });
        Container.Bind<Canvas>().FromInstance(_canvas).AsSingle();
        Container.Bind<SceneManagementSc>().FromInstance(_sceneManagment).AsSingle();
        Container.Bind<ExceptionPanel>().FromInstance(_exceptionPanel).AsSingle();
        Container.Bind<GameState>().FromInstance(GameState).AsSingle();
        Container.Bind<Transform>().FromInstance(_player.transform).AsSingle();
        Container.Bind<Player>().FromInstance(_player).AsSingle();
        Container.Bind<UI>().FromInstance(_uI).AsSingle();
        Container.Bind<EnemySpawnerController>().FromInstance(_enemySpawnerController).AsSingle();
        Container.Bind<AudioRecorder>().FromInstance(_audioRecorder).AsSingle();
        Container.Bind<SceneAudioController>().FromInstance(_audioController).AsSingle();
        var Heroes = JsonUtility.FromJson<Heroes>(PlayerData.text);
        Container.Bind<Heroes>().FromInstance(Heroes).AsSingle();
    }
}
