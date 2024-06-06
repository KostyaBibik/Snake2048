using System.Collections;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UISystem;
using UnityEngine;

namespace UI.Loose
{
    public class StatsContainerModel
    {
        public int totalKills;
        public int totalScore;
        public int leaderTime;
        public bool isAnimating;
    }
    
    public class StatsContainerView : UIElementView<StatsContainerModel>
    {
        [AutoSetupField] private TextMeshProUGUI _totalScore;
        [AutoSetupField] private TextMeshProUGUI _totalKills;
        [AutoSetupField] private TextMeshProUGUI _leaderTime;
        
        private StatsContainerModel _savedModel;
        
        protected override void UpdateView(StatsContainerModel model)
        {
            if(!model.isAnimating)
            {
                _totalScore.text = model.totalScore.ToString(CultureInfo.InvariantCulture);
                _totalKills.text = model.totalKills.ToString(CultureInfo.InvariantCulture);
                _leaderTime.text = FormatTime(model.leaderTime);
            }
            else
            {
                _savedModel = model;
            }
        }

        protected override void OnShowEnd()
        {
            if (_savedModel != null)
            {
                StartCoroutine(AnimateContainer(_savedModel));
            }

            base.OnShowEnd();
        }

        private IEnumerator AnimateContainer(StatsContainerModel model)
        {
            yield return StartCoroutine(AnimateValueView(_leaderTime, model.leaderTime, 3f, true));
            
            yield return StartCoroutine(AnimateValueView(_totalKills, model.totalKills, 3f));
            
            yield return StartCoroutine(AnimateValueView(_totalScore, model.totalScore, 5f));
        }

        private IEnumerator AnimateValueView(TextMeshProUGUI view, int finalValue, float animationDuration, bool isTimer = false)
        {
            var elapsedTime = 0f;

            if (finalValue == 0)
                elapsedTime = animationDuration;
            
            while (elapsedTime < animationDuration)
            {
                var progress = elapsedTime / animationDuration;

                var currentValue = Mathf.Lerp(0, finalValue, progress);

                var showedValue = isTimer
                    ? FormatTime(Mathf.RoundToInt(currentValue))
                    : Mathf.RoundToInt(currentValue).ToString();
                
                view.text = showedValue;

                var delay = Mathf.Lerp(0.01f, 0.15f, progress * progress); 
                yield return new WaitForSeconds(delay);

                elapsedTime += delay;
            }

            view.text = isTimer
                ? FormatTime(Mathf.RoundToInt(finalValue))
                : Mathf.RoundToInt(finalValue).ToString();
            
            var originalScale = view.transform.localScale;
            var targetScale = originalScale * 1.4f; 

            view.transform.DOScale(targetScale, 0.3f).OnComplete(() =>
            {
                view.transform.DOScale(originalScale, 0.3f);
            });
        }
        
        private string FormatTime(int value)
        {
            var minutes = Mathf.FloorToInt(value / 60);
            var seconds = Mathf.FloorToInt(value % 60);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}