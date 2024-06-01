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

        public AccelerationBoxSystem(
            BoxService boxService,
            GameInputManager inputManager,
            SignalBus signalBus
            )
        {
            _boxService = boxService;
            _inputManager = inputManager;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _boostTimers = new Dictionary<int, AccelerationTeamModel>();
            
            _mouseMovementContext = _inputManager.GetContext<MousePressContext>();
            
            _mouseMovementContext.RegisterButtonPressCallback(OnPressMouse, GameSystemType.PlayerHighPriority);
            _mouseMovementContext.RegisterButtonReleaseCallback(OnReleaseMouse, GameSystemType.PlayerHighPriority);
            
            _signalBus.Subscribe<RegisterTeamSignal>(OnRegisterNewTeam);
            _signalBus.Subscribe<AddBoxToTeamSignal>(OnAddBoxOnTeam);
            
            InitTeams();
        }

        private void OnRegisterNewTeam(RegisterTeamSignal signal)
        {
            var team = _boxService.GetTeamById(signal.id);
            if(team.Leader == null || team.Leader.isIdle)
                return;

            var isPlayerTeam = team.Leader.isPlayer;
            AddBoostTeamTimer(signal.id, isPlayerTeam);
        }

        private void OnAddBoxOnTeam(AddBoxToTeamSignal signal)
        {
            var idTeam = signal.idTeam;
            var isForced = _boostTimers[idTeam].isForced;
            SetAccelerationStatus(idTeam, isForced);
        }
        
        private void AddBoostTeamTimer(int id, bool isPlayer = false)
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
            var teams = _boxService
                .GetAllTeams()
                .Where(t => t.Members.Count > 0 && !t.Leader.isIdle);
            
            foreach (var team in teams)
            {
                var id = team.GetId();
                var isPlayerTeam = _boxService.GetTeamById(id).Leader.isPlayer;
                AddBoostTeamTimer(id, isPlayerTeam);
            }
        }

        private void OnPressMouse()
        {
            var playerTeam = _boostTimers.Values.FirstOrDefault(x => x.isPlayer);
            if (playerTeam == null)
                return;
            
            playerTeam.tweenTimer?.Dispose();
            
            playerTeam.tweenTimer = Observable.FromCoroutine(() => AccelerateTeam(playerTeam))
                .Subscribe();
        }
        
        private void OnReleaseMouse()
        {
            var playerTeam = _boostTimers.Values.FirstOrDefault(x => x.isPlayer);
            if (playerTeam == null)
                return;
            
            SetAccelerationStatus(playerTeam.id, false);
            
            playerTeam.tweenTimer?.Dispose();
            
            playerTeam.tweenTimer = Observable.FromCoroutine(() => RecoverAbility(playerTeam))
                .Subscribe();
        }

         private IEnumerator AccelerateTeam(AccelerationTeamModel teamModel)
         {
             SetAccelerationStatus(teamModel.id, true);
             
             while (teamModel.abilityCapacity > 0)
             {
                 teamModel.abilityCapacity -= Time.deltaTime / 4f;
                 var playerLeader = _boxService.GetHighestBoxInTeam(teamModel.id);
                 if(playerLeader != null)
                 {
                     playerLeader.UpdateAccelerationSlider(teamModel.abilityCapacity, true);
                 }
                 
                 yield return null;
             }

             teamModel.abilityCapacity = 0f;
             SetAccelerationStatus(teamModel.id, false);
             
             var player = _boxService.GetHighestBoxInTeam(teamModel.id);
             player.UpdateAccelerationSlider(teamModel.abilityCapacity, false);

             while (teamModel.abilityCapacity < 1)
             {
                 teamModel.abilityCapacity += Time.deltaTime / 4f;

                 yield return null;
             }
         }

         private IEnumerator RecoverAbility(AccelerationTeamModel teamModel)
         {
             while (teamModel.abilityCapacity < 1)
             {
                 teamModel.abilityCapacity += Time.deltaTime / 4f;

                 yield return null;
             }
         }
         
        private void SetAccelerationStatus(int idTeam, bool enabled)
        {
            var team = _boxService.GetTeamById(idTeam);
            
            if(team == null || team.Members.Count <= 0)
                return;

            _boostTimers[idTeam].isForced = enabled;
            foreach (var box in team.Members)
            {
                box.IsAccelerationActive = enabled;
            }

            if (team.Leader.isPlayer)
            {
                var playerLeader = team.Leader;
                playerLeader.UpdateAccelerationSlider(_boostTimers[idTeam].abilityCapacity, enabled);
            }
        }

        public void Dispose()
        {
            foreach (var boostTimer in _boostTimers)
            {
                boostTimer.Value.tweenTimer?.Dispose();
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