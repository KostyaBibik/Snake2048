using System.Linq;
using UISystem;
using UnityEngine;

namespace UI.Leaderboard
{
    public class LeaderboardModel
    {
        public LeaderboardElementModel[] Items { get; set; }
    
        public LeaderboardModel(LeaderboardElementModel[] items)
        {
            Items = items;
        }
        
        public LeaderboardModel(int count)
        {
            Items = new LeaderboardElementModel[count];
        }
    }

    public class LeaderboardWindow : UIWindow<LeaderboardModel>
    {
        private LeaderboardElementView[] _lines;

        public override void Initialization()
        {
            UpdateItemsCount(5);
        }

        protected override void UpdateView(LeaderboardModel model)
        {
            var sortedItems = model.Items
                .OrderByDescending(s => s.score)
                .Take(5)
                .ToArray();
            
            UpdateContent(sortedItems);
        }

        private void UpdateItemsCount(int count)
        {
            if (_lines == null || !_lines.Any())
            {
                var template = gameObject.GetElement<LeaderboardElementView>(UIConstantDictionary.Names.DefaultTemplateTag);
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
                var isActive = i < count;
                _lines[i].gameObject.SetActive(isActive);
                _lines[i].transform.SetSiblingIndex(i);
            }
        }

        private void UpdateContent(LeaderboardElementModel[] Items)
        {
            for (var i = Items.Length - 1; i >= 0; i--)
            {
                var line = _lines[i];
                var lineModel = Items[i];
                lineModel.nickname = $"{i + 1}. {lineModel.nickname}";
                
                line.InvokeUpdateView(lineModel);
            }
        }
    }
}