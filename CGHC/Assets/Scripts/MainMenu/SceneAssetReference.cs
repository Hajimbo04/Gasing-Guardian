using UnityEngine;
// No SceneManagement needed here, just data.

// This attribute allows you to create an instance of this asset via the Project window
[CreateAssetMenu(fileName = "SceneRef_", menuName = "Custom/Scene Reference")]
public class SceneAssetReference : ScriptableObject
{
    // A string field to store the name of the scene.
    // Making it public allows the Unity Inspector to modify it.
    [Tooltip("The exact name of the Scene file in your project, e.g., 'MainMenu'")]
    public string SceneName;

    // A utility method to get the name, for clean access.
    public string GetSceneName()
    {
        return SceneName;
    }
}