using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Infrastructure.Factories.Impl;
using Signals;
using UnityEngine;
using Views.Impl;
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
        private SignalBus _signalBus;

        [Inject]
        public void Construct(
            BoxStateFactory stateFactory,
            BoxPool boxPool,
            SignalBus signalBus
        )
        {
            _stateFactory = stateFactory;
            _boxPool = boxPool;
            _signalBus = signalBus;
        }

        public void RegisterNewTeam(BoxView leader, string nickName)
        {
            if (_boxToTeamMap.ContainsKey(leader))
            {
                return;
            }

            if (!Boxes.Contains(leader))
            {
                AddEntityOnService(leader);
            }

            var newTeam = new Team(nickName);
            newTeam.AddMember(leader);
            _teams.Add(newTeam);
            _boxToTeamMap[leader] = newTeam;
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
            return _boxToTeamMap.TryGetValue(box1, out var team1) && _boxToTeamMap.TryGetValue(box2, out var team2) &&
                   team1 == team2;
        }

        public void AddBoxToTeam(BoxView teamViewer, BoxView box)
        {
            if (_boxToTeamMap.TryGetValue(teamViewer, out var team) && !_boxToTeamMap.ContainsKey(box))
            {
                team.AddMember(box);
                AddEntityOnService(box);
                _boxToTeamMap[box] = team;
            }
            else
            {
                throw new ArgumentException($"Can't add box to team");
            }
        }

        public void UpdateTeamStates(BoxView viewFromTeam)
        {
            var team = GetTeam(viewFromTeam);
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

                        var state = _stateFactory.CreateFollowState(members[i - 1].transform);
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
        }

        public List<BoxView> GetAllBoxes()
        {
            return Boxes;
        }

        public Team GetTeam(BoxView boxView)
        {
            _boxToTeamMap.TryGetValue(boxView, out var team);
            return team;
        }

        public EBoxGrade GetHighGradeInTeam(BoxView boxMember)
        {
            var team = GetTeam(boxMember);
            return team?.Members.Max(grade => grade.Grade) ?? default;
        }

        public int GetBotTeamsCount()
        {
            return _teams.Count(team => team.Members.Any(box => box.isBot));
        }

        public BoxView GetHighestBoxInTeam(BoxView boxView)
        {
            var team = GetTeam(boxView);
            return team?.Leader;
        }
    }
}