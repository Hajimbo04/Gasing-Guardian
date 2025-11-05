using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneLoader : MonoBehaviour
{
    [Tooltip("Drag the Scene Reference ScriptableObject here.")]
    public SceneAssetReference targetScene;

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

        SceneManager.LoadScene(sceneName);
        Debug.Log("Loading scene: " + sceneName);
    }

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadTargetScene);
        }
    }
}