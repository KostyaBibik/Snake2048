namespace GameUtilities.UI
{
    public class PermanentUIElementException : System.Exception
    {
        public PermanentUIElementException() : base("Permanent UI element cann't be showed or hided.")
        {

        }
    }
}