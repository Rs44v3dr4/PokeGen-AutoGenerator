namespace PokeGenBot
{
    public class BotConfig
    {
        public string Token { get; set; }
        public ulong CanalId { get; set; }
        public ulong RolId { get; set; }
        public string GeminiApiKey { get; set; } // Nueva propiedad para la IA
    }
}