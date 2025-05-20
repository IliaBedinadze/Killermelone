using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text pressAnyKey;

    private SceneManagementSc _scene;
    [Inject]
    public void Constructor(SceneManagementSc scene)
    {
        _scene = scene;
    }
    private void Start()
    {
        StartCoroutine(LoadSceneAsync(_scene.SceneToLoad));
    }
    private IEnumerator LoadSceneAsync(string scene)
    {
        var operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress/0.9f);
            progressBar.value = progress;
            yield return null;
        }
        progressBar.value = 1f;
        pressAnyKey.gameObject.SetActive(true);
        yield return new WaitUntil(() => Input.anyKeyDown);
        operation.allowSceneActivation = true;
    }
}
