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
    [SerializeField] private Player _player;
    [SerializeField] private UI _uI;
    [SerializeField] private GameState GameState;
    [SerializeField] private ExceptionPanel _exceptionPanel;
    [SerializeField] private SceneManagementSc _sceneManagment;
    [SerializeField] private InfoPanel _infoPanel;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private EnemySpawnerController _enemySpawnerController;
    public override void InstallBindings()
    {
        var rounds = JsonUtility.FromJson<RoundsData>(roundsJsonText.text);
        Container.Bind<RoundsData>().FromInstance(rounds).AsSingle();

        var weapons = JsonUtility.FromJson<WeaponList>(weaponJsonData.text);
        Container.Bind<WeaponList>().FromInstance(weapons).AsSingle();

        Container.Bind<Canvas>().FromInstance(_canvas).AsSingle();
        Container.BindFactory<WeaponData, InfoPanel, InfoPanelFactory>().FromMethod((container, data) =>
        {
            var panel = container.InstantiatePrefab(_infoPanel);
            var info = panel.GetComponent<InfoPanel>();

            info.SetData(data);
            container.Inject(info);
            return info;
        });
        Container.Bind<SceneManagementSc>().FromInstance(_sceneManagment).AsSingle();
        Container.Bind<ExceptionPanel>().FromInstance(_exceptionPanel).AsSingle();
        Container.Bind<GameState>().FromInstance(GameState).AsSingle();
        Container.Bind<Transform>().FromInstance(_player.transform).AsSingle();
        Container.Bind<Player>().FromInstance(_player).AsSingle();
        Container.Bind<UI>().FromInstance(_uI).AsSingle();
        Container.Bind<EnemySpawnerController>().FromInstance(_enemySpawnerController).AsSingle();
    }
}
