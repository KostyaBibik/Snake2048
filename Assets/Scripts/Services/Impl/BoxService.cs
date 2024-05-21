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
        private BoxStateFactory _stateFactory;

        [Inject]
        public void Construct(BoxStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }
        
        public void RegisterNewTeam(BoxView leader)
        {
            if (_teams.Any(team => team.Contains(leader)))
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

            for (var i = 0; i < _teams.Count; i++)
            {
                if (_teams[i].Contains(entityView))
                {
                    var updatedTeam = new Queue<BoxView>(_teams[i].Where(box => box != entityView));
                    _teams[i] = updatedTeam;
                }
            }

            Boxes.Remove(entityView);
            entityView.isDestroyed = true;
            Object.Destroy(entityView.gameObject);
            _teams.RemoveAll(team => team.Count == 0);
        }

        public bool AreInSameTeam(BoxView box1, BoxView box2)
        {
            return _teams.Any(team => team.Contains(box1) && team.Contains(box2));
        }

        public void AddBoxToTeam(BoxView leader, BoxView box)
        {
            var team = _teams.FirstOrDefault(t => t.Peek() == leader);
            if (team != null && !team.Contains(box))
            {
                team.Enqueue(box);
                AddEntityOnService(box);
            }
        }
        
        public void UpdateTeamStates(BoxView viewFromTeam)
        {
            var team = GetTeam(viewFromTeam).OrderByDescending(box => box.Grade).ToList();
            
            team[0].SetNickname(team[0].isPlayer
                ? "player"
                : "Bot"
            );
            
            for (var i = 0; i < team.Count; i++)
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
                    var state = _stateFactory.CreateFollowState(team[i - 1].transform);
                    currentBox.stateContext.SetState(state);
                }
            }

            for (var i = 0; i < team.Count - 1; i++)
            {
                for (var j = i + 1; j < team.Count; j++)
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
            var team = _teams.FirstOrDefault(t => t.Contains(boxView));
            return team != null ? team.ToList() : new List<BoxView>();
        }

        public EBoxGrade GetHighGradeInTeam(BoxView boxMember)
        {
            var team = GetTeam(boxMember);
            var highGrade = team.Max(grade => grade.Grade);
            
            return highGrade;
        }
    }
}
