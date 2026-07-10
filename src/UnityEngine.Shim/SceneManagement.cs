namespace UnityEngine
{
    public class AsyncOperation
    {
        public bool isDone { get; set; } = true;

        public float progress { get; set; } = 1f;

        public bool allowSceneActivation { get; set; } = true;
    }
}

namespace UnityEngine.SceneManagement
{
    using UnityEngine;

    public struct Scene
    {
        public string name { get; set; }

        public string path { get; set; }

        public int buildIndex { get; set; }

        public bool isLoaded { get; set; }

        public readonly bool IsValid()
        {
            return !string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(path) || buildIndex >= 0;
        }
    }

    public enum LoadSceneMode
    {
        Single,
        Additive
    }

    public static class SceneManager
    {
        private static Scene _activeScene = new()
        {
            name = "Main",
            path = "Main",
            buildIndex = 0,
            isLoaded = true
        };

        public static event Action<Scene, LoadSceneMode>? sceneLoaded;

        public static Scene GetActiveScene()
        {
            return _activeScene;
        }

        public static bool SetActiveScene(Scene scene)
        {
            if (!scene.IsValid())
            {
                return false;
            }

            _activeScene = scene;
            return true;
        }

        public static void LoadScene(string sceneName)
        {
            LoadScene(sceneName, LoadSceneMode.Single);
        }

        public static void LoadScene(string sceneName, LoadSceneMode mode)
        {
            _activeScene = new Scene
            {
                name = sceneName,
                path = sceneName,
                buildIndex = -1,
                isLoaded = true
            };
            sceneLoaded?.Invoke(_activeScene, mode);
        }

        public static void LoadScene(int sceneBuildIndex)
        {
            LoadScene(sceneBuildIndex, LoadSceneMode.Single);
        }

        public static void LoadScene(int sceneBuildIndex, LoadSceneMode mode)
        {
            _activeScene = new Scene
            {
                name = sceneBuildIndex.ToString(),
                path = sceneBuildIndex.ToString(),
                buildIndex = sceneBuildIndex,
                isLoaded = true
            };
            sceneLoaded?.Invoke(_activeScene, mode);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName)
        {
            LoadScene(sceneName);
            return new AsyncOperation();
        }
    }
}
