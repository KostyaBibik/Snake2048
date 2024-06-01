using System.Collections.Generic;
using Components.Boxes.Views.Impl;
using Services;

public class BotService : IEntityService<BoxView>
{
    private List<BoxView> _bots = new List<BoxView>();

    public void AddEntityOnService(BoxView bot)
    {
        if (!_bots.Contains(bot))
        {
            _bots.Add(bot);
        }
    }

    public void RemoveEntity(BoxView bot)
    {
        if (_bots.Contains(bot))
        {
            _bots.Remove(bot);
        }
    }

    public List<BoxView> GetAllBots()
    {
        return _bots;
    }

    public int GetBotCount()
    {
        return _bots.Count;
    }
}