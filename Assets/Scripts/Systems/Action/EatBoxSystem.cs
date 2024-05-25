using Database;
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
        private readonly BotService _botService;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly BoxPool _boxPool;

        public EatBoxSystem(
            SignalBus signalBus,
            BoxService boxService,
            BoxEntityFactory boxEntityFactory,
            BotService botService,
            GameSettingsConfig settingsConfig,
            BoxPool boxPool
        )
        {
            _signalBus = signalBus;
            _boxService = boxService;
            _boxEntityFactory = boxEntityFactory;
            _botService = botService;
            _gameSettingsConfig = settingsConfig;
            _boxPool = boxPool;
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

            if(owner.isIdle)
                return;
                
            if(!eatenBox.isIdle && eatenBox.Grade >= owner.Grade)
                return;
            
            if(eatenBox.isIdle && eatenBox.Grade > owner.Grade)
                return;

            var ownerTransform = owner.transform;
            var ownerPos = ownerTransform.position;

            var newBox = _boxPool.GetBox(eatenBox.Grade);
            //var newBox = _boxEntityFactory.Create(eatenBox.Grade);
            var directionSpawn = (ownerPos - eatenBox.transform.position).normalized;
            directionSpawn.y = 0;
            
            newBox.transform.position = ownerPos + directionSpawn * _gameSettingsConfig.BoxFollowDistance;
            
            if (owner.isBot)
            {
                _botService.AddEntityOnService(newBox);
            }
            
            _boxService.AddBoxToTeam(owner, newBox);
            _boxService.RemoveEntity(eatBoxSignal.eatenBox);
            _boxService.UpdateTeamStates(owner);
        }
    }
}