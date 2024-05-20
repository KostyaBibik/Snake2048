using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Views;
using Views.Impl;

namespace Services.Impl
{
    public class BoxService : IEntityService<BoxView>
    {
        public List<BoxView> Boxes { get; } = new List<BoxView>();
        private readonly List<Queue<BoxView>> _teams = new List<Queue<BoxView>>();

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

        public List<BoxView> GetTeam(BoxView boxView)
        {
            var team = _teams.FirstOrDefault(t => t.Contains(boxView));
            return team != null ? team.ToList() : new List<BoxView>();
        }
    }
}
