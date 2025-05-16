using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LoadingSceneInstaller : MonoInstaller
{
    [SerializeField] private SceneManagementSc scene;

    public override void InstallBindings()
    {
        Container.Bind<SceneManagementSc>().FromInstance(scene).AsSingle();
    }
}
