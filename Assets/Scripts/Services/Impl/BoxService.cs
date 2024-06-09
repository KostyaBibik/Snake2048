using System;
using System.Collections.Generic;
using System.Linq;
using Components.Boxes.Views.Impl;
using Enums;
using Helpers.Game;
using Infrastructure.Factories.Impl;
using Infrastructure.Pools.Impl;
using Signals;
using UI.Leaderboard;
using UniRx;
using Zenject;

namespace Services.Impl
{
    public class BoxService : IEntityService<BoxView>
    {
        private readonly List<BoxView> Boxes = new List<BoxView>();
        private readonly Dictionary<BoxView, Team> _boxToTeamMap = new Dictionary<BoxView, Team>();
        private readonly List<Team> _teams = new List<Team>();

        private BoxStateFactory _stateFactory;
        private BoxPool _boxPool;
        private GameMatchService _matchService;
        private SignalBus _signalBus;
        private PlayerDataService _playerDataService;
        private PlayerTeamSavedData _playerTeamSavedData = new PlayerTeamSavedData();
        
        [Inject]
        public void Construct(
            BoxStateFactory stateFactory,
            BoxPool boxPool,
            GameMatchService matchService,
            PlayerDataService playerDataService,
            SignalBus signalBus
        )
        {
            _stateFactory = stateFactory;
            _boxPool = boxPool;
            _matchService = matchService;
            _playerDataService = playerDataService;
            _signalBus = signalBus;

            _matchService.playerNickname.Subscribe(HandleUpdatePlayerNickname);
        }

        public void RegisterNewTeam(BoxView leader, string nickName)
        {
            if (_boxToTeamMap.ContainsKey(leader))
            {
                throw new Exception("_boxToTeamMap.ContainsKey");
            }

            if (!Boxes.Contains(leader))
            {
                AddEntityOnService(leader);
            }

            var newTeam = new Team(nickName);
            newTeam.AddMember(leader);
            _teams.Add(newTeam);
            _boxToTeamMap[leader] = newTeam;

            if (leader.isPlayer)
            {
                _playerTeamSavedData.maxGrade = leader.Grade;
                _playerTeamSavedData.maxScore = newTeam.GetScore();
            }
            
            _signalBus.Fire(new RegisterTeamSignal { id = newTeam.GetId() });
        }

        public void AddEntityOnService(BoxView entityView)
        {
            if (Boxes.Contains(entityView))
                return;
            
            Boxes.Add(entityView);
        }

        public void RemoveEntity(BoxView entityView)
        {
            if (!Boxes.Contains(entityView))
                return;

            if (entityView.isPlayer && GetHighestBoxInTeam(entityView) == entityView)
            {
                _signalBus.Fire(new CameraUpdateSignal
                {
                    followBox = null
                });
            }
            
            if (_boxToTeamMap.TryGetValue(entityView, out var team))
            {
                team.RemoveMember(entityView);
                _boxToTeamMap.Remove(entityView);

                if (team.Members.Count == 0)
                {
                    if (entityView.isPlayer)
                    {
                        var progress = _playerDataService.PlayerProgress;
                        
                        var resultScore = ResultUtility.CalcResultScore(
                            progress.CurrentLeaderTime,
                            progress.CurrentTotalKills,
                            _playerTeamSavedData.maxGrade,
                            _playerTeamSavedData.maxScore
                        );
                        
                        _playerDataService.UpdateTotalScore(resultScore);
                        
                        _signalBus.Fire(new ChangeGameModeSignal()
                        {
                            status = EGameModeStatus.Lose
                        });
                    }
                    
                    _teams.Remove(team);
                }
            }

            Boxes.Remove(entityView);
            _boxPool.ReturnToPool(entityView, entityView.Grade);
        }

        public bool AreInSameTeam(BoxView box1, BoxView box2)
        {
            return _boxToTeamMap.TryGetValue(box1, out var team1)
                   && _boxToTeamMap.TryGetValue(box2, out var team2)
                   && team1 == team2;
        }

        public void AddBoxToTeam(BoxView teamViewer, BoxView box)
        {
            if (_boxToTeamMap.TryGetValue(teamViewer, out var team) && !_boxToTeamMap.ContainsKey(box))
            {
                team.AddMember(box);
                AddEntityOnService(box);
                _boxToTeamMap[box] = team;
                
                _signalBus.Fire(new AddBoxToTeamSignal
                {
                    idTeam = team.GetId(),
                    newBox = box
                });

                if (box.isPlayer)
                {
                    if (team.Leader.Grade > _playerTeamSavedData.maxGrade)
                        _playerTeamSavedData.maxGrade = team.Leader.Grade;
                    if (team.GetScore() > _playerTeamSavedData.maxScore)
                        _playerTeamSavedData.maxScore = team.GetScore();
                }
                
                InvokeUpdateLeaderboard();
            }
            else
            {
                throw new ArgumentException($"Can't add box to team");
            }
        }

        public void UpdateTeamStates(BoxView viewFromTeam)
        {
            var team = GetTeamByMember(viewFromTeam);
            if (team == null)
                return;

            var members = team.Members.OrderByDescending(box => box.Grade).ToList();

            var teamLeader = members.FirstOrDefault();
            team.Leader = teamLeader;

            if (teamLeader != null)
            {
                if (teamLeader.isPlayer)
                {
                    _signalBus.Fire(new CameraUpdateSignal
                    {
                        followBox = teamLeader
                    });
                }
                
                teamLeader.SetNickname(team.Nickname);

                for (var i = 0; i < members.Count; i++)
                {
                    var currentBox = members[i];
                    
                    if (i == 0)
                    {
                        var state = currentBox.isPlayer
                            ? _stateFactory.CreateMoveState()
                            : _stateFactory.CreateBotMoveState();
                        currentBox.stateContext.SetState(state);
                    }
                    else
                    {
                        if (currentBox.isMerging)
                            continue;

                        var targetMember = members[i - 1];
                        var state = _stateFactory.CreateFollowState(targetMember.transform, targetMember.meshOffset);
                        currentBox.stateContext.SetState(state);
                    }
                }

                for (var i = 0; i < members.Count - 1; i++)
                {
                    for (var j = i + 1; j < members.Count; j++)
                    {
                        if (members[i].Grade == members[j].Grade
                            && members[i] != members[j]
                            && !members[i].isMerging
                            && !members[j].isMerging
                        )
                        {
                            members[i].isMerging = true;
                            members[j].isMerging = true;

                            var state = _stateFactory.CreateMergeState(members[i], members[j].Grade);
                            members[j].stateContext.SetState(state);
                        }
                    }
                }
            }

            InvokeUpdateLeaderboard();
        }

        private void InvokeUpdateLeaderboard()
        {
            var handledTeams = _teams
                .Where(x => !x.Leader.isIdle)
                .OrderByDescending(x => x.GetScore())
                .Take(5)
                .ToArray();
            
            var leaderboardElementsModel = new LeaderboardModel(handledTeams.Length);
            for (var i = 0; i < handledTeams.Length; i++)
            {
                var handleTeam = handledTeams[i];
                leaderboardElementsModel.Items[i] = new LeaderboardElementModel
                {
                    nickname = handleTeam.Nickname,
                    score = handleTeam.GetScore(),
                    isPlayer = handleTeam.Leader.isPlayer
                };
            }
            
            _signalBus.Fire(new LeaderboardUpdateSignal
            {
                elementModels = leaderboardElementsModel.Items
            });
        }

        public List<BoxView> GetAllBoxes()
        {
            return Boxes;
        }

        public List<Team> GetAllTeams()
        {
            return _teams;
        } 
        
        public Team GetTeamByMember(BoxView boxView)
        {
            _boxToTeamMap.TryGetValue(boxView, out var team);
            return team;
        }

        public Team GetTeamById(int id)
        {
            return _teams.FirstOrDefault(team => team.GetId() == id);
        }

        public EBoxGrade GetHighGradeInTeam(BoxView boxMember)
        {
            var team = GetTeamByMember(boxMember);
            return team?.Members.Max(grade => grade.Grade) ?? default;
        }
        
        public EBoxGrade GetHighGradeInTeam(int idTeam)
        {
            var team = GetTeamById(idTeam);
            return team?.Members.Max(grade => grade.Grade) ?? default;
        }

        public int GetBotTeamsCount()
        {
            return _teams.Count(team => team.Members.Any(box => box.isBot));
        }

        public BoxView GetHighestBoxInTeam(int idTeam)
        {
            var team = GetTeamById(idTeam);
            return team?.Leader;
        }
        
        public BoxView GetHighestBoxInTeam(BoxView boxView)
        {
            var team = GetTeamByMember(boxView);
            return team?.Leader;
        }

        public EBoxGrade GetHighestPlayerGrade()
        {
            return _playerTeamSavedData.maxGrade;
        }

        private void HandleUpdatePlayerNickname(string nick)
        {
            var playerTeam = _teams
                .FirstOrDefault(t => t.Members.Count > 0 && t.Leader != null && t.Leader.isPlayer);
            playerTeam?.UpdateNickname(nick);
        }
    }

    public class PlayerTeamSavedData
    {
        public EBoxGrade maxGrade;
        public int maxScore;
    }
}