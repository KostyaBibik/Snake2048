namespace Infrastructure.Factories
{
    public interface IEntityFactory<T, TResult>
    {
        TResult Create(T param);
    }
}