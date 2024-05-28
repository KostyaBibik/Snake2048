namespace GameUtilities.Extensions
{
    public static class BoolExtensions
    {
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
        
        public static bool ToBool(this int value)
        {
            return value > 0;
        }
    }
}