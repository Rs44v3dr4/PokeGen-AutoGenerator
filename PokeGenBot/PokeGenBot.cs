namespace PokeGenBot
{
    public class BotConfig
    {
        public string Token { get; set; }
        public ulong CanalId { get; set; }
        public ulong RolId { get; set; }

        // CAMBIO: Antes era string GeminiApiKey, ahora es una lista
        public List<string> ApiKeys { get; set; } = new List<string>();
    }
}