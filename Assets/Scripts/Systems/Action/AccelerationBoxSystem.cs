using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components.Boxes.Views.Impl;
using Enums;
using Input.Context;
using Services;
using Services.Impl;
using Signals;
using UniRx;
using UnityEngine;
using Zenject;

namespace Systems.Action
{
    public class AccelerationBoxSystem : IInitializable, IDisposable
    {
        private readonly BoxService _boxService;
        private readonly GameInputManager _inputManager;
        private readonly GameMatchService _gameMatchService;
        private readonly SignalBus _signalBus;
        
        private Dictionary<int, AccelerationTeamModel> _boostTimers;
        private ButtonInputContext _mouseMovementContext;
        private float _timeCasting;
        private float _delayBeforeHideUiContainer;

        public AccelerationBoxSystem(
            BoxService boxService,
            GameInputManager inputManager,
            GameMatchService gameMatchService,
            SignalBus signalBus)
        {
            _boxService = boxService;
            _inputManager = inputManager;
            _gameMatchService = gameMatchService;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _boostTimers = new Dictionary<int, AccelerationTeamModel>();
            _mouseMovementContext = _inputManager.GetContext<MousePressContext>();

            _timeCasting = 4f;
            _delayBeforeHideUiContainer = .5f;

            RegisterCallbacks();
            SubscribeToSignals();
            InitTeams();
        }

        private void RegisterCallbacks()
        {
            _mouseMovementContext.RegisterButtonPressCallback(OnPressMouse, GameSystemType.PlayerHighPriority);
            _mouseMovementContext.RegisterButtonReleaseCallback(OnReleaseMouse, GameSystemType.PlayerHighPriority);
        }

        private void SubscribeToSignals()
        {
            _signalBus.Subscribe<RegisterTeamSignal>(OnRegisterNewTeam);
            _signalBus.Subscribe<AddBoxToTeamSignal>(OnAddBoxOnTeam);
            _signalBus.Subscribe<ChangeGameModeSignal>(OnLoseGameSignal);
        }

        private void OnRegisterNewTeam(RegisterTeamSignal signal)
        {
            var team = _boxService.GetTeamById(signal.id);
            if (team.Leader == null || team.Leader.isIdle) 
                return;

            AddBoostTeamTimer(signal.id, team.Leader.isPlayer);
        }

        private void OnAddBoxOnTeam(AddBoxToTeamSignal signal)
        {
            var idTeam = signal.idTeam;
            SetAccelerationStatus(idTeam, _boostTimers[idTeam].isForced, _boostTimers[idTeam].isForced);
        }

        private void OnLoseGameSignal(ChangeGameModeSignal signal)
        {
            if(signal.status != EGameModeStatus.Lose)
                return;

            var playerTimer = _boostTimers.Values.FirstOrDefault(t => t.isPlayer);

            if (playerTimer != null)
            {
                var keyToRemove = _boostTimers.FirstOrDefault(x => x.Value.Equals(playerTimer)).Key;

                _boostTimers.Remove(keyToRemove);
            }
        }
        
        private void AddBoostTeamTimer(int id, bool isPlayer)
        {
            var model = new AccelerationTeamModel
            {
                abilityCapacity = 1f,
                isPlayer = isPlayer,
                id = id
            };

            _boostTimers.Add(id, model);
        }

        private void InitTeams()
        {
            var teams = _boxService.GetAllTeams()
                                   .Where(t => t.Members.Count > 0 && !t.Leader.isIdle);

            foreach (var team in teams)
            {
                AddBoostTeamTimer(team.GetId(), team.Leader.isPlayer);
            }
        }

        private void OnPressMouse()
        {
            if(!_gameMatchService.IsGameRunning())
                return;
            
            var playerTeam = _boostTimers.Values.FirstOrDefault(x => x.isPlayer);
            if (playerTeam == null) 
                return;

            playerTeam.tweenTimer?.Dispose();
            
            playerTeam.tweenTimer = Observable.FromCoroutine(() => AccelerateTeam(playerTeam))
                .Subscribe();
        }

        private void OnReleaseMouse()
        {
            if(!_gameMatchService.IsGameRunning())
                return;
            
            var playerTeam = _boostTimers.Values.FirstOrDefault(x => x.isPlayer);
            if (playerTeam == null)
                return;

            SetAccelerationStatus(playerTeam.id, false, true);
            playerTeam.tweenTimer?.Dispose();
            
            playerTeam.tweenTimer = Observable.FromCoroutine(() => RecoverAbility(playerTeam))
                .Subscribe();
        }

        public void ChangeAccelerationStatusForBotTeam(BoxView bot, bool forceStatus)
        {
            if(!_gameMatchService.IsGameRunning())
                return;

            var botTeamId = _boxService.GetTeamByMember(bot).GetId();
            var botTimer = _boostTimers.Values.FirstOrDefault(x => x.id == botTeamId);
            if (botTimer == null)
                return;
            
            if(botTimer.abilityCapacity < .75f)
                return;

            botTimer.tweenTimer?.Dispose();
            
            if(forceStatus)
            {
                botTimer.tweenTimer = Observable.FromCoroutine(() => AccelerateTeam(botTimer))
                    .Subscribe();
            }
            else
            {
                botTimer.tweenTimer = Observable.FromCoroutine(() => RecoverAbility(botTimer))
                    .Subscribe();
            }
        }
        
        private IEnumerator AccelerateTeam(AccelerationTeamModel teamModel)
        {
            var sliderEnabled = teamModel.isPlayer;
            SetAccelerationStatus(teamModel.id, true, sliderEnabled);

            while (teamModel.abilityCapacity > 0)
            {
                teamModel.abilityCapacity -= Time.deltaTime / _timeCasting;
                
                if(teamModel.isPlayer)
                    UpdateUIForPlayer(teamModel);
                
                yield return null;
            }

            teamModel.abilityCapacity = 0f;
            teamModel.tweenTimer = Observable.FromCoroutine(() => RecoverAbility(teamModel))
                .Subscribe();
        }

        private IEnumerator RecoverAbility(AccelerationTeamModel teamModel)
        {
            var sliderEnabled = teamModel.isPlayer;
            
            SetAccelerationStatus(teamModel.id, false, sliderEnabled);
            
            var timeSinceUpdateUi = 0f;

            while (teamModel.abilityCapacity < 1)
            {
                teamModel.abilityCapacity += Time.deltaTime / _timeCasting;
                timeSinceUpdateUi += Time.deltaTime;

                if(teamModel.isPlayer)
                    UpdateUIForPlayer(teamModel);

                if (timeSinceUpdateUi >= _delayBeforeHideUiContainer && teamModel.isPlayer)
                {
                    var leader = _boxService.GetHighestBoxInTeam(teamModel.id);
                    if(leader != null && !leader.isDestroyed)
                    {
                        leader.UpdateAccelerationSliderStatus(teamModel.abilityCapacity, false);
                        leader.UpdateAccelerationFxStatus(false);
                    }
                }

                yield return null;
            }
            
            if(teamModel.isPlayer)
            {
                var leader = _boxService.GetHighestBoxInTeam(teamModel.id);
                if(leader != null && !leader.isDestroyed)
                {
                    leader.UpdateAccelerationSliderStatus(teamModel.abilityCapacity, false);
                    leader.UpdateAccelerationFxStatus(false);
                }
            }
        }

        private void UpdateUIForPlayer(AccelerationTeamModel teamModel)
        {
            if (!teamModel.isPlayer) return;

            var leader = _boxService.GetHighestBoxInTeam(teamModel.id);
            if (leader != null)
            {
                leader.UpdateAccelerationSliderStatus(teamModel.abilityCapacity, true);
                leader.UpdateAccelerationFxStatus(true);
            }
        }

        private void SetAccelerationStatus(int idTeam, bool forceEnabled, bool sliderFlag)
        {
            var team = _boxService.GetTeamById(idTeam);
            if (team == null || team.Members.Count <= 0)
                return;

            _boostTimers[idTeam].isForced = forceEnabled;
            foreach (var box in team.Members)
            {
                box.IsAccelerationActive = forceEnabled;
            }

            var teamLeader = team.Leader;
            teamLeader.UpdateAccelerationFxStatus(forceEnabled);
            
            if (teamLeader.isPlayer)
            {
                teamLeader.UpdateAccelerationSliderStatus(_boostTimers[idTeam].abilityCapacity, sliderFlag);

                if (teamLeader && forceEnabled)
                {
                    _signalBus.Fire(new PlaySoundSignal {type = ESoundType.AccelerateMoving});
                }
            }
        }

        public void Dispose()
        {
            foreach (var boostTimer in _boostTimers.Values)
            {
                boostTimer.tweenTimer?.Dispose();
            }
            _boostTimers.Clear();
        }
    }

    public class AccelerationTeamModel
    {
        public IDisposable tweenTimer;
        public float abilityCapacity;
        public int id;
        public bool isPlayer;
        public bool isForced;
    }
}
