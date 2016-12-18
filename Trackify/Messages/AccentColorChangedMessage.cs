namespace Trackify.Messages
{
    public class AccentColorChangedMessage
    {
        public AccentColorChangedMessage(string newAccentColor)
        {
            NewAccentColor = newAccentColor;
        }

        public string NewAccentColor { get; }
    }
}