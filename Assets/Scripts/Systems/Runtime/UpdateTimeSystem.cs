using UI.Top;
using UISystem;
using UnityEngine;
using Zenject;

namespace Systems.Runtime
{
    public class UpdateTimeSystem : ITickable, IInitializable
    {
        private TopWindow _topWindow;
        private float _time;
        
        public void Initialize()
        {
            _topWindow = UIManager.Instance.GetUIElement<TopWindow>();
        }

        public void Tick()
        {
            _time += Time.deltaTime;
            var topWindowModel = new TopWindowModel();
            topWindowModel.CurrentTime = (int)_time;
            _topWindow.InvokeUpdateView(topWindowModel);
        }
    }
}