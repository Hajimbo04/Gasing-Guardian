using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneLoader : MonoBehaviour
{
    // This is the drag-and-drop custom asset!
    [Tooltip("Drag the Scene Reference ScriptableObject here.")]
    public SceneAssetReference targetScene;

    // The method to call on button click
    public void LoadTargetScene()
    {
        if (targetScene == null)
        {
            Debug.LogError("ButtonSceneLoader: Target Scene Reference is not assigned!");
            return;
        }

        string sceneName = targetScene.GetSceneName();

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("ButtonSceneLoader: Scene name is empty in the Scene Reference asset!");
            return;
        }

        // Load the scene using the name from the ScriptableObject
        SceneManager.LoadScene(sceneName);
        Debug.Log("Loading scene: " + sceneName);
    }

    // Optional: Auto-attach the click event in code (assuming this script is on the Button)
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadTargetScene);
        }
    }
}