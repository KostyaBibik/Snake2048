using Components.Boxes.Views.Impl;
using Enums;
using Helpers;
using Infrastructure.Pools.Impl;
using Services.Impl;
using Signals;
using Zenject;

namespace Systems.Action.BoostSystems
{
    public class AddGradesBoostSystem : IInitializable
    {
        private const int stepGrades = 2;
        
        private readonly BoxService _boxService;
        private readonly BoxPool _boxPool;
        private readonly SignalBus _signalBus;

        public AddGradesBoostSystem(
            BoxService boxService,
            BoxPool boxPool,
            SignalBus signalBus
        )
        {
            _boxService = boxService;
            _boxPool = boxPool;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<BoxBoostSignal>(OnGetBoostSignal);
        }

        private void OnGetBoostSignal(BoxBoostSignal signal)
        {
            if(signal.boostType != EBoxBoost.AddGrades)
                return;

            ApplyBoostToTeam(signal.box);
        }

        private void ApplyBoostToTeam(BoxView signalBox)
        {
            var boxTeam = _boxService.GetTeamByMember(signalBox);
            var teamMembers = boxTeam.Members;

            for (var i = 0; i < teamMembers.Count; i++)
            {
                var currentBox = teamMembers[i];
                var newBox = _boxPool.GetBox(currentBox.Grade.NextSteps(stepGrades));

                newBox.isPlayer = currentBox.isPlayer;
                newBox.isBot = currentBox.isBot;
                newBox.IsAccelerationActive = currentBox.IsAccelerationActive;
                newBox.transform.position = currentBox.transform.position;
                newBox.gameObject.SetActive(true);
                
                _boxService.AddBoxToTeam(currentBox, newBox);
                _boxService.RemoveEntity(currentBox);
            }
            
            _boxService.UpdateTeamStates(boxTeam.Leader);
        }
    }
}