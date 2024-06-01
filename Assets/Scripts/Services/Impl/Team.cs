using System;
using System.Collections.Generic;
using Components.Boxes.Views.Impl;

namespace Services.Impl
{
    public class Team
    {
        public string Nickname { get; }
        public List<BoxView> Members { get; } = new List<BoxView>();
        public BoxView Leader;
        public Guid Id { get; private set; }
        
        public Team(string nickname)
        {
            Nickname = nickname;
            Id = Guid.NewGuid();
        }

        public void AddMember(BoxView member)
        {
            if (Members.Contains(member))
                return;

            if (Members.Count <= 0)
                Leader = member;
                
            Members.Add(member);
        }

        public void RemoveMember(BoxView member)
        {
            if (Members.Contains(member))
            {
                Members.Remove(member);
            }
        }

        public int GetScore()
        {
            var score = 0;
            for (var index = 0; index < Members.Count; index++)
            {
                var member = Members[index];
                score += (int) member.Grade;
            }

            return score;
        }
        
        public int GetId()
        {
            return Id.GetHashCode();
        }
    }
}