namespace Components.Boxes.Views
{
    public interface IBoxView : IEntityView
    {
        public bool isBot { get; set; }
        public bool isPlayer { get; set; }
        public bool isIdle { get; set; }
        public bool isMerging { get; set; }        
    }
}