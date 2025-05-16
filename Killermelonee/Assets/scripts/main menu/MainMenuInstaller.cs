using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    [SerializeField] private SceneManagementSc sceneManagementSc;

    public override void InstallBindings()
    {
        Container.Bind<SceneManagementSc>().FromInstance(sceneManagementSc).AsSingle();
    }
}
