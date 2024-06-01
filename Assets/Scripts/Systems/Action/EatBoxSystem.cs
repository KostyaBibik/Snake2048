using System.Linq;
using Components.Boxes.Views.Impl;
using Database;
using Enums;
using Services.Impl;
using Signals;
using UnityEngine;
using Zenject;

namespace Systems.Action
{
    public class EatBoxSystem : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly BoxService _boxService;
        private readonly BotService _botService;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly BoxPool _boxPool;

        public EatBoxSystem(
            SignalBus signalBus,
            BoxService boxService,
            BotService botService,
            GameSettingsConfig settingsConfig,
            BoxPool boxPool
        )
        {
            _signalBus = signalBus;
            _boxService = boxService;
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
            
            var eatenTeam = _boxService.GetTeamByMember(eatenBox);
            var eatenBoxes = eatenTeam.Members
                .Where(box => box.Grade <= eatenBox.Grade || box.isIdle).ToArray();
            
            AddBoxesForTeam(eatenBoxes, owner);
        }

        private void AddBoxesForTeam(BoxView[] eatenBoxes, BoxView newOwner)
        {
            var newBoxes = new BoxView[eatenBoxes.Length];
            var ownerTeam = _boxService.GetTeamByMember(newOwner);
            
            for (var i = 0; i < eatenBoxes.Length; i++)
            {
                var eatenBox = eatenBoxes[i];
                
                var newBox = _boxPool.GetBox(eatenBox.Grade);
                newBox.isPlayer = newOwner.isPlayer;
                newBox.isBot = newOwner.isBot;
                newBox.isIdle = newOwner.isIdle;

                var sortedMembers = ownerTeam.Members.OrderBy(b => b.Grade).ToArray();

                Vector3 directionSpawn;

                if (sortedMembers.Length > 1)
                {
                    directionSpawn = (sortedMembers[0].transform.position - sortedMembers[1].transform.position)
                        .normalized;
                }
                else
                {
                    var ownerTransform = newOwner.transform;
                    var ownerPos = ownerTransform.position;
                    
                    directionSpawn = (ownerPos - eatenBox.transform.position).normalized;
                }

                directionSpawn.y = 0;
                newBox.transform.position = sortedMembers[0].transform.position + directionSpawn * _gameSettingsConfig.BoxFollowDistance;
                
                if (newOwner.isBot)
                {
                    _botService.AddEntityOnService(newBox);
                }

                newBoxes[i] = newBox;
            }

            var delay = 0f;
            var delayInterval = .2f;

            foreach (var boxInTeam in ownerTeam.Members.ToArray())
            {
                boxInTeam.AnimateUpscale(delay);
                delay += delayInterval;
            }

            for (var i = 0; i < newBoxes.Length; i++)
            {
                var newBox = newBoxes[i];
                var eatenBox = eatenBoxes[i];
                
                _boxService.AddBoxToTeam(newOwner, newBox);
                _boxService.RemoveEntity(eatenBox);
                _botService.RemoveEntity(eatenBox);
            }

            _boxService.UpdateTeamStates(newOwner);
            
            if (newOwner.isPlayer)
            {
                _signalBus.Fire(new PlaySoundSignal
                {
                    Type = ESoundType.Eat
                });
            }
        }
    }
}