using UnityEngine.SceneManagement;
using Zenject;

namespace Helpers
{
    public class SceneLoader
    {
        private readonly ZenjectSceneLoader _sceneLoader;

        public SceneLoader(ZenjectSceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void RestartScene()
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            
            _sceneLoader.LoadScene(currentSceneName, LoadSceneMode.Single);
        }
    }
}