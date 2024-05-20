namespace Components.Boxes.States
{
    public interface IBoxState
    {
        void EnterState(BoxContext context);
        void UpdateState(BoxContext context);
        void ExitState(BoxContext context);
    }
}