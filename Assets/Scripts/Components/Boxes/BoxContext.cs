using Components.Boxes.States;
using Views.Impl;

namespace Components.Boxes
{
    public class BoxContext
    {
        private readonly BoxView _boxView;
        private IBoxState _currentState;

        public BoxView BoxView => _boxView;

        public BoxContext(BoxView view)
        {
            _boxView = view;
        }
        
        public void SetState(IBoxState newState)
        {
            _currentState?.ExitState(this);
            _currentState = newState;
            _currentState?.EnterState(this);
        }

        public void Update()
        {
            _currentState?.UpdateState(this);
        }
    }
}