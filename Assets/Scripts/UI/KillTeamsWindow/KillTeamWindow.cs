using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UISystem;
using UniRx;
using UnityEngine;

namespace UI.KillTeamsWindow
{
    public class KillTeamsModel
    {
        public string nickKiller;
        public string nickDestroyed;
    }

    public class KillTeamWindow : UIWindow<KillTeamsModel>
    {
        private KillTeamElementView[] _lines;
        private Queue<KillTeamElementModel> _queueTeamsShow;

        private const int showElementsCount = 5;
        private const float showElementTime = 3f;
        
        public override void Initialization()
        {
            _queueTeamsShow = new Queue<KillTeamElementModel>();
            UpdateItemsCount(showElementsCount);
        }
        
        protected override void UpdateView(KillTeamsModel model)
        {
            _queueTeamsShow.Enqueue(new KillTeamElementModel
            {
                killDestroyed = model.nickDestroyed,
                killTeamer = model.nickKiller,
                observer = null
            });
            UpdateContent();
        }
        
        private void UpdateItemsCount(int count)
        {
            if (_lines == null || !_lines.Any())
            {
                var template = gameObject.GetElement<KillTeamElementView>(UIConstantDictionary.Names.DefaultTemplateTag);
                _lines = new[] { template };
            }
            
            if (count > _lines.Length)
            {
                var template = _lines.First();
                var createCount = count - _lines.Length;
                _lines = _lines.Concat(Enumerable.Range(0, createCount)
                        .Select(_ => UIManager.InstantiateElement(template)))
                    .ToArray();
            }
            
            for (var i = 0; i < _lines.Length; i++)
            {
                _lines[i].transform.SetSiblingIndex(i);
                _lines[i].BeginHide();
            }
        }

        private void UpdateContent()
        {
            var iterator = 0;
            foreach (var lineModel in _queueTeamsShow)
            {
                iterator++;
                if(iterator >= showElementsCount)
                    break;
                
                var line = _lines[iterator];
                line.InvokeUpdateView(lineModel);
                line.BeginShow();

                if (lineModel.observer == null) 
                {
                    lineModel.observer = Observable.FromCoroutine(ShowWithTimer)
                        .DoOnCompleted(() =>
                        {
                            lineModel.observer = null;
                            UpdateContent();
                        })
                        .Subscribe();
                }
            }

            for (iterator++ ; iterator < showElementsCount; iterator++)
            {
                var line = _lines[iterator];
                line.InvokeUpdateView(new KillTeamElementModel());
                line.BeginHide();
            }
        }

        private IEnumerator ShowWithTimer()
        {
            yield return new WaitForSeconds(showElementTime);

            _queueTeamsShow.Dequeue();
        }
    }
}