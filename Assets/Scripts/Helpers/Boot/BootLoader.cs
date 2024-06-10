using System;
using System.Collections;
using System.Text;
using DG.Tweening;
using Kimicu.YandexGames;
using LocalizationSystem.Data.KeyGeneration;
using LocalizationSystem.Main;
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

        private void Awake()
        {
            Localization.SetLocalization("en");
        }

        public void Start()
        {
            levelLoader = SceneManager.LoadSceneAsync(SceneName);
             levelLoader.allowSceneActivation = false;

            SetGameSettings();
            StartCoroutine(nameof(UpdateLoadingSlider));
            StartCoroutine(nameof(InitYaServices));
        }

        private void SetGameSettings()
        {
            DOTween.SetTweensCapacity(500, 312);
        }

        private IEnumerator InitYaServices()
        {
            yield return YandexGamesSdk.Initialize();

            if(YandexGamesSdk.IsInitialized)
            {
                Localization.SetLocalization(YandexGamesSdk.Environment.i18n.lang);
            }
            else
            {
                Localization.SetLocalization("ru");
            }

            yield return Cloud.Initialize();

            Advertisement.Initialize();

                // yield return Billing.Initialize();

            WebApplication.Initialize();
            
            yield return null;
            
            levelLoader.allowSceneActivation = true;
        }

        private IEnumerator UpdateLoadingSlider()
        {
            var textBuilder = new StringBuilder
            {
                Capacity = 18
            };

            while (!levelLoader.isDone)
            {
                Localization.TryGetText(LocalizationKey.Loading.ToString(), out var textLoadingByLang);
                
                var progressValue = levelLoader.progress;
                progressSlider.value = progressValue;
                textBuilder.Clear();
                textBuilder.Append($"{textLoadingByLang}... ");
                textBuilder.Insert(10, (int) (progressSlider.value * 100));
                textBuilder.Append("%");
                progressText.text = textBuilder.ToString();
                yield return null;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}