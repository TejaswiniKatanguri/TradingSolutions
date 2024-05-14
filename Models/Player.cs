#pragma warning disable
namespace TradingSolutions.Models
{
    public class Player
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public enum GameTypes
    {
        NFL,
        MLB
    }
}
