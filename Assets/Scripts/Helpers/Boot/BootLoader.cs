using System.Collections;
using System.Text;
using Kimicu.YandexGames;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Helpers.Boot
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text progressText;

        private AsyncOperation levelLoader;
        
        private const string SceneName = "GameScene";

        private void Start()
        {
            levelLoader = SceneManager.LoadSceneAsync(SceneName);
            levelLoader.allowSceneActivation = false;
            
            StartCoroutine(nameof(LevelLoadSync));
            StartCoroutine(nameof(InitYaServices));
        }

        private IEnumerator InitYaServices()
        {
            // Обязательно к вызову. Вызывать в 1 очередь!!!
            yield return YandexGamesSdk.Initialize();

// При необходимости.
            yield return Cloud.Initialize();

// При необходимости.
            Advertisement.Initialize();

// При необходимости.
                // yield return Billing.Initialize();

// При необходимости.
            WebApplication.Initialize();
            
            yield return null;
            
            levelLoader.allowSceneActivation = true;
        }
        
        private IEnumerator LevelLoadSync()
        {
            var textBuilder = new StringBuilder
            {
                Capacity = 18
            };

            levelLoader.allowSceneActivation = true;
            while (!levelLoader.isDone)
            {
                var progressValue = levelLoader.progress;
                progressSlider.value = progressValue;
                textBuilder.Clear();
                textBuilder.Append("Loading... ");
                textBuilder.Insert(10, (int) (progressSlider.value * 100));
                textBuilder.Append("%");
                progressText.text = textBuilder.ToString();
                yield return null;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}