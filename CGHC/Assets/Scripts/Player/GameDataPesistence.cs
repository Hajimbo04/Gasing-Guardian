    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class GameData : MonoBehaviour
    {
        public static GameData Instance { get; private set; }

        [Header("Persistent Data")]
        public int? playerLives = null; 

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void TransitionToNextLevel(string nextSceneName)
        {
           
           UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);

        }
    }
