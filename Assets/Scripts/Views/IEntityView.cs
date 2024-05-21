namespace Views
{
    public interface IEntityView
    {
        public bool isDestroyed { get; set; }
        public bool isPlayer { get; set; }
        public bool isIdle { get; set; }
        public bool isBot { get; set; }
    }
}