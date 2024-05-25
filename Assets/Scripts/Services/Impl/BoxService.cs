using System.Collections.Generic;
using System.Linq;
using Enums;
using Infrastructure.Factories.Impl;
using UnityEngine;
using Views.Impl;
using Zenject;

namespace Services.Impl
{
    public class BoxService : IEntityService<BoxView>
    {
        private readonly List<BoxView> Boxes = new List<BoxView>();
        private readonly List<Queue<BoxView>> _teams = new List<Queue<BoxView>>();
        private readonly Dictionary<BoxView, Queue<BoxView>> _boxToTeamMap = new Dictionary<BoxView, Queue<BoxView>>();

        private BoxStateFactory _stateFactory;
        private BoxPool _boxPool;

        [Inject]
        public void Construct(
            BoxStateFactory stateFactory,
            BoxPool boxPool
        )
        {
            _stateFactory = stateFactory;
            _boxPool = boxPool;
        }

        public void RegisterNewTeam(BoxView leader)
        {
            if (_boxToTeamMap.ContainsKey(leader))
            {
                return;
            }

            if (!Boxes.Contains(leader))
            {
                AddEntityOnService(leader);
            }

            var newTeam = new Queue<BoxView>();
            newTeam.Enqueue(leader);
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

            if (_boxToTeamMap.TryGetValue(entityView, out var team))
            {
                var updatedTeam = new Queue<BoxView>(team.Where(box => box != entityView));
                _teams[_teams.IndexOf(team)] = updatedTeam;
        
                _boxToTeamMap.Remove(entityView);

                if (updatedTeam.Count == 0)
                {
                    _teams.Remove(updatedTeam);
                }
                else
                {
                    foreach (var box in updatedTeam)
                    {
                        _boxToTeamMap[box] = updatedTeam;
                    }
                }
            }

            Boxes.Remove(entityView);
            _boxPool.ReturnToPool(entityView, entityView.Grade);
        }

        public bool AreInSameTeam(BoxView box1, BoxView box2)
        {
            if (_boxToTeamMap.TryGetValue(box1, out var team1) && _boxToTeamMap.TryGetValue(box2, out var team2))
            {
                return team1 == team2;
            }

            return false;
        }

        public void AddBoxToTeam(BoxView teamViewer, BoxView box)
        {
            if (_boxToTeamMap.TryGetValue(teamViewer, out var team) && !team.Contains(box))
            {
                team.Enqueue(box);
                AddEntityOnService(box);
                _boxToTeamMap[box] = team;
            }
        }

        public void UpdateTeamStates(BoxView viewFromTeam)
        {
            var team = GetTeam(viewFromTeam).OrderByDescending(box => box.Grade).ToArray();

            team[0].SetNickname(team[0].isPlayer
                ? "player"
                : "Bot"
            );

            for (var i = 0; i < team.Length; i++)
            {
                var currentBox = team[i];
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

                    var state = _stateFactory.CreateFollowState(team[i - 1].transform);
                    currentBox.stateContext.SetState(state);
                }
            }

            for (var i = 0; i < team.Length - 1; i++)
            {
                for (var j = i + 1; j < team.Length; j++)
                {
                    if (team[i].Grade == team[j].Grade
                        && team[i] != team[j]
                        && !team[i].isMerging
                        && !team[j].isMerging
                    )
                    {
                        team[i].isMerging = true;
                        team[j].isMerging = true;

                        var state = _stateFactory.CreateMergeState(team[i], team[j].Grade);
                        team[j].stateContext.SetState(state);
                    }
                }
            }
        }

        public List<BoxView> GetAllBoxes()
        {
            return Boxes;
        }

        public List<BoxView> GetTeam(BoxView boxView)
        {
            if (_boxToTeamMap.TryGetValue(boxView, out var team))
            {
                return team.ToList();
            }
            return new List<BoxView>();
        }

        public EBoxGrade GetHighGradeInTeam(BoxView boxMember)
        {
            var team = GetTeam(boxMember);
            var highGrade = team.Max(grade => grade.Grade);

            return highGrade;
        }
    }
}
