using UnityEngine;

[CreateAssetMenu(fileName = "SceneRef_", menuName = "Custom/Scene Reference")]
public class SceneAssetReference : ScriptableObject
{
    [Tooltip("MainMenu'")]
    public string SceneName;

    public string GetSceneName()
    {
        return SceneName;
    }
}