using Enums;

namespace Components.Boosts
{
    public interface IBoostView
    {
        public EBoxBoost BoostType { get; }
        public bool IsInteractable { get; }
    }
}