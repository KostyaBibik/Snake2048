using Views;

namespace Services
{
    public interface IEntityService <T>  where T : IEntityView
    {
        void AddEntityOnService(T entityView);
        void RemoveEntity(T entityView);
    }
}