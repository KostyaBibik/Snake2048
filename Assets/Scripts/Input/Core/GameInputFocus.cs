using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: REWORK
public enum GameSystemType
{
    UI = 0,
    PlayerHighPriority = 1,    // SLOTS
    PlayerMiddlePriority = 2,      
    Camera = 3,
    PlayerLowPriority = 4,      // DEFAULT
}

public static class GameInputFocus
{
    private class LockItem
    {
        public GameSystemType type;
        public List<object> senders = new List<object>();
    }
    private static List<LockItem> lockItems = new List<LockItem>();

    public static bool IsActive(GameSystemType type)
    {
        return !lockItems
            .Any(x => (int)x.type < (int)type && x.senders.Count > 0); ;
    }

    public static void Unfocus(object sender)
    {
        var item = lockItems.Find(x => x.senders.Contains(sender));

        if (item != null)
        {
            item.senders.Remove(sender);
        }
    }

    public static void Focus(GameSystemType type, object sender)
    {
        var item = lockItems.Find(x => x.type == type);

        if (item == null)
        {
            item = new LockItem()
            {
                type = type
            };

            lockItems.Add(item);
        }

        if (!item.senders.Contains(sender))
        {
            item.senders.Add(sender);
        }
    }
}