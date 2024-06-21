using System;
using TMPro;
using UISystem;
using UniRx;

namespace UI.KillTeamsWindow
{
    public class KillTeamElementModel
    {
        public string killTeamer;
        public string killDestroyed;
        public IDisposable observer;
    }
    
    public class KillTeamElementView : UIElementView<KillTeamElementModel>
    {
        [AutoSetupField] private TextMeshProUGUI _nickKiller;
        [AutoSetupField] private TextMeshProUGUI _nickDestroyed;
        
        protected override void UpdateView(KillTeamElementModel model)
        {
            _nickKiller.text = model.killTeamer;
            _nickDestroyed.text = model.killDestroyed;
        }
    }
}