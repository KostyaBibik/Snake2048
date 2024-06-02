using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Input.Context;
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
        private readonly SignalBus _signalBus;
        
        private Dictionary<int, AccelerationTeamModel> _boostTimers;
        private ButtonInputContext _mouseMovementContext;
        private float _timeCasting;
        private float _delayBeforeHideUiContainer;

        public AccelerationBoxSystem(BoxService boxService, GameInputManager inputManager, SignalBus signalBus)
        {
            _boxService = boxService;
            _inputManager = inputManager;
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
        }

        private void OnRegisterNewTeam(RegisterTeamSignal signal)
        {
            var team = _boxService.GetTeamById(signal.id);
            if (team.Leader == null || team.Leader.isIdle) return;

            AddBoostTeamTimer(signal.id, team.Leader.isPlayer);
        }

        private void OnAddBoxOnTeam(AddBoxToTeamSignal signal)
        {
            var idTeam = signal.idTeam;
            SetAccelerationStatus(idTeam, _boostTimers[idTeam].isForced, _boostTimers[idTeam].isForced);
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
            var playerTeam = _boostTimers.Values.FirstOrDefault(x => x.isPlayer);
            if (playerTeam == null) return;

            playerTeam.tweenTimer?.Dispose();
            
            playerTeam.tweenTimer = Observable.FromCoroutine(() => AccelerateTeam(playerTeam))
                .Subscribe();
        }

        private void OnReleaseMouse()
        {
            var playerTeam = _boostTimers.Values.FirstOrDefault(x => x.isPlayer);
            if (playerTeam == null) return;

            SetAccelerationStatus(playerTeam.id, false, true);
            playerTeam.tweenTimer?.Dispose();
            
            playerTeam.tweenTimer = Observable.FromCoroutine(() => RecoverAbility(playerTeam))
                .Subscribe();
        }

        private IEnumerator AccelerateTeam(AccelerationTeamModel teamModel)
        {
            SetAccelerationStatus(teamModel.id, true, true);

            while (teamModel.abilityCapacity > 0)
            {
                teamModel.abilityCapacity -= Time.deltaTime / _timeCasting;
                UpdateUIForPlayer(teamModel);
                yield return null;
            }

            teamModel.abilityCapacity = 0f;
            SetAccelerationStatus(teamModel.id, false, true);
            yield return RecoverAbility(teamModel);
        }

        private IEnumerator RecoverAbility(AccelerationTeamModel teamModel)
        {
            var timeSinceUpdateUi = 0f;

            while (teamModel.abilityCapacity < 1)
            {
                teamModel.abilityCapacity += Time.deltaTime / _timeCasting;
                timeSinceUpdateUi += Time.deltaTime;

                UpdateUIForPlayer(teamModel);

                if (timeSinceUpdateUi >= _delayBeforeHideUiContainer)
                {
                    _boxService.GetHighestBoxInTeam(teamModel.id)?.UpdateAccelerationSliderStatus(teamModel.abilityCapacity, false);
                }

                yield return null;
            }
            
            _boxService.GetHighestBoxInTeam(teamModel.id)?.UpdateAccelerationSliderStatus(teamModel.abilityCapacity, false);
        }

        private void UpdateUIForPlayer(AccelerationTeamModel teamModel)
        {
            if (!teamModel.isPlayer) return;

            var leader = _boxService.GetHighestBoxInTeam(teamModel.id);
            if (leader != null)
            {
                leader.UpdateAccelerationSliderStatus(teamModel.abilityCapacity);
            }
        }

        private void SetAccelerationStatus(int idTeam, bool enabled, bool sliderFlag)
        {
            var team = _boxService.GetTeamById(idTeam);
            if (team == null || team.Members.Count <= 0) return;

            _boostTimers[idTeam].isForced = enabled;
            foreach (var box in team.Members)
            {
                box.IsAccelerationActive = enabled;
            }
            
            if (team.Leader.isPlayer)
            {
                team.Leader.UpdateAccelerationSliderStatus(_boostTimers[idTeam].abilityCapacity, sliderFlag);
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
