using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

// installer for main menu
public class MainMenuInstaller : MonoInstaller
{
    [SerializeField] private SceneManagementSc sceneManagementSc;
    [SerializeField] private AudioRecorder audioRecorder;
    [SerializeField] private SceneAudioController sceneAudioController;
    public override void InstallBindings()
    {
        Container.Bind<SceneManagementSc>().FromInstance(sceneManagementSc).AsSingle();
        Container.Bind<AudioRecorder>().FromInstance(audioRecorder).AsSingle();
        Container.Bind<SceneAudioController>().FromInstance(sceneAudioController).AsSingle();
    }
}
