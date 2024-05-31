using System.Collections.Generic;
using System.Linq;
using Views.Impl;

namespace Services.Impl
{
    public class Team
    {
        public string Nickname { get; }
        public List<BoxView> Members { get; } = new List<BoxView>();
        public BoxView Leader;

        public Team(string nickname)
        {
            Nickname = nickname;
        }

        public void AddMember(BoxView member)
        {
            if (!Members.Contains(member))
            {
                Members.Add(member);
            }
        }

        public void RemoveMember(BoxView member)
        {
            if (Members.Contains(member))
            {
                Members.Remove(member);
            }
        }
    }
}