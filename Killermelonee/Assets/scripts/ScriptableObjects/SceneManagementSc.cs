using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Scene Load Data")]
public class SceneManagementSc : ScriptableObject
{
    public string SceneToLoad;
    public bool Continue;
}
