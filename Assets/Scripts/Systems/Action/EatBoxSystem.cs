using Infrastructure.Factories.Impl;
using Services.Impl;
using Signals;
using Zenject;

namespace Systems.Action
{
    public class EatBoxSystem : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly BoxService _boxService;
        private readonly BoxEntityFactory _boxEntityFactory;
        private readonly BoxStateFactory _boxStateFactory;
        
        public EatBoxSystem(
            SignalBus signalBus,
            BoxService boxService,
            BoxEntityFactory boxEntityFactory,
            BoxStateFactory boxStateFactory
        )
        {
            _signalBus = signalBus;
            _boxService = boxService;
            _boxEntityFactory = boxEntityFactory;
            _boxStateFactory = boxStateFactory;
        }   
        
        public void Initialize()
        {
            _signalBus.Subscribe<EatBoxSignal>(OnEatBox);
        }

        private void OnEatBox(EatBoxSignal eatBoxSignal)
        {
            var owner = eatBoxSignal.newOwner;
            var eatenBox = eatBoxSignal.eatenBox;
            
            if (_boxService.AreInSameTeam(eatenBox, owner))
                return;
            
            if(eatenBox.Grade >= owner.Grade)
                return;

            var ownerTransform = owner.transform;
            var ownerPos = ownerTransform.position;
            
            var newBox = _boxEntityFactory.Create(eatenBox.Grade);
            var directionSpawn = (ownerPos - eatenBox.transform.position).normalized;
            directionSpawn.y = 0;
            
            var ownerTeam = _boxService.GetTeam(owner);
            var lastBoxInTeam = ownerTeam[^1];

            newBox.transform.position = ownerPos + directionSpawn * ownerTeam.Count * 2;
            var state = _boxStateFactory.CreateFollowState(lastBoxInTeam.transform);
            newBox.stateContext.SetState(state);
            
            _boxService.AddBoxToTeam(owner, newBox);
            _boxService.RemoveEntity(eatBoxSignal.eatenBox);
            
            _signalBus.Fire(new MergeBoxSignal
            {
                mergingBox = newBox
            });
        }
    }
}