using System;
using System.Collections;
using System.Collections.Generic;
using Components.Boxes.Views.Impl;
using Database;
using Enums;
using Services.Impl;
using Signals;
using UI.Top;
using UISystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace Systems.Action
{
    public class BoxBoostSystem : IInitializable, IDisposable
    {
        private readonly BoxService _boxService;
        private readonly GameSettingsConfig _settingsConfig;
        private readonly SignalBus _signalBus;
        
        private Dictionary<int, BoostTeamModel> _boostTimers;
        private ContainerBoostView _containerBoostView;
        private float _speedBoostDuration;

        public BoxBoostSystem(
            BoxService boxService,
            GameSettingsConfig gameSettingsConfig,
            SignalBus signalBus
        )
        {
            _boxService = boxService;
            _settingsConfig = gameSettingsConfig;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _boostTimers = new Dictionary<int, BoostTeamModel>();
            _containerBoostView = UIManager.Instance.GetUIElement<ContainerBoostView>();

            _speedBoostDuration = _settingsConfig.BoostSpeedDuration;

            _signalBus.Subscribe<BoxBoostSignal>(OnGetBoostSignal);
            _signalBus.Subscribe<AddBoxToTeamSignal>(OnAddBoxToTeam);
        }

        private void OnGetBoostSignal(BoxBoostSignal signal)
        {
            switch (signal.boostType)
            {
                case EBoxBoost.Speed:
                    OnGetBoostSpeedSignal(signal.box);
                    break;
                default:
                    break;
            }
        }

        private void OnGetBoostSpeedSignal(BoxView box)
        {
            var boxTeam = _boxService.GetTeamByMember(box);
            var idTeam = boxTeam.GetId();

            if (_boostTimers.TryGetValue(idTeam, out var existingTimer))
            {
                existingTimer.tweenTimer?.Dispose();
                existingTimer.tweenTimer = Observable.FromCoroutine(() => ApplyBoostForTeam(existingTimer))
                    .Subscribe();
            }
            else
            {
                var boostTeamModel = new BoostTeamModel();
                boostTeamModel.id = idTeam;
                boostTeamModel.abilityCapacity = 1f;
                boostTeamModel.isPlayer = boxTeam.Leader.isPlayer;
                boostTeamModel.tweenTimer = Observable.FromCoroutine(() => ApplyBoostForTeam(boostTeamModel))
                    .Subscribe();
                
                _boostTimers.Add(idTeam, boostTeamModel);
            }
        }

        private void OnAddBoxToTeam(AddBoxToTeamSignal signal)
        {
            var idTeam = signal.idTeam;
            var team = _boxService.GetTeamById(idTeam);

            if (_boostTimers.ContainsKey(idTeam))
            {
                var teamLeader = team.Leader;
                foreach (var box in team.Members)
                {
                    box.IsSpeedBoosted = true;
                    box.UpdateBoostVFXStatus(box == teamLeader);
                }
            }
        }

        private IEnumerator ApplyBoostForTeam(BoostTeamModel teamModel)
        {
            SetBoostStatus(teamModel.id, true);
            var containerBoostModel = new ContainerBoostModel();
            
            if(teamModel.isPlayer)
                _containerBoostView.BeginShow();

            teamModel.abilityCapacity = 1f;

            while (teamModel.abilityCapacity > 0)
            {
                teamModel.abilityCapacity -= Time.deltaTime / _speedBoostDuration;
                if (teamModel.isPlayer)
                {
                    UpdateUIForPlayer(teamModel, containerBoostModel);
                }
       
                yield return null;
            }

            SetBoostStatus(teamModel.id, false);
            
            if (teamModel.isPlayer)
                _containerBoostView.BeginHide();

            _boostTimers.Remove(teamModel.id);
        }

        private void SetBoostStatus(int idTeam, bool enabled)
        {
            var team = _boxService.GetTeamById(idTeam);
            if (team == null || team.Members.Count <= 0) 
                return;

            var teamLeader = team.Leader;
            foreach (var box in team.Members)
            {
                box.IsSpeedBoosted = enabled;
                box.UpdateBoostVFXStatus(enabled && box == teamLeader);
            }
        }
        
        private void UpdateUIForPlayer(BoostTeamModel teamModel, ContainerBoostModel containerBoostModel)
        {
            if (!teamModel.isPlayer) 
                return;

            var leader = _boxService.GetHighestBoxInTeam(teamModel.id);
            if (leader != null)
            {
                containerBoostModel.value = teamModel.abilityCapacity;
                _containerBoostView.InvokeUpdateView(containerBoostModel);
            }
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<BoxBoostSignal>(OnGetBoostSignal);

            foreach (var timer in _boostTimers.Values)
            {
                timer.tweenTimer?.Dispose();
            }
            _boostTimers.Clear();
        }
    }
    
    public class BoostTeamModel
    {
        public IDisposable tweenTimer;
        public float abilityCapacity;
        public int id;
        public bool isPlayer;
    }
}
