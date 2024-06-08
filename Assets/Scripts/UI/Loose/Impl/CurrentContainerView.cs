using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Loose.Impl
{
    public class CurrentContainerView : StatsContainer
    {
        [AutoSetupField] private RecordView _recordTime;
        [AutoSetupField] private RecordView _recordKills;
        [AutoSetupField] private RecordView _recordScore;
        
        private StatsContainerModel _savedModel;
        
        protected override void UpdateView(StatsContainerModel model)
        {
            _savedModel = model;
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
            yield return StartCoroutine(AnimateValueView(leaderTime, model.leaderTime, 3f, true));

            if (model.showRecordTime)
            {
                _recordTime.BeginShow();
            }
            
            yield return StartCoroutine(AnimateValueView(totalKills, model.totalKills, 3f));
            
            if (model.showRecordKills)
            {
                _recordKills.BeginShow();
            }
            
            yield return StartCoroutine(AnimateValueView(totalScore, model.totalScore, 5f));
            
            if (model.showRecordScore)
            {
                _recordScore.BeginShow();
            }
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
    }
}