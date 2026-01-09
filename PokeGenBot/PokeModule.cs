using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace PokeGenBot
{
    public class PokeModule : ModuleBase<SocketCommandContext>
    {
        private readonly BotConfig _config;

        public PokeModule(BotConfig config)
        {
            _config = config;
        }

        [Command("gen")]
        [Alias("crear", "spawn", "pokemon", "make", "peticion")]
        public async Task GenPokemon([Remainder] string query)
        {
            // 1. Validar que sea el canal correcto
            if (Context.Channel.Id != _config.CanalId) return;

            // 2. Poner al bot en modo "Escribiendo..."
            using (Context.Channel.EnterTypingState())
            {
                // 3. Llamada a la IA (usando tu nueva configuración de Gemini 2.5)
                string resultado = await AIService.GenerarShowdown(query, _config.ApiKeys);

                // 4. Verificar si hubo error (Rojo)
                if (resultado.StartsWith("Error") || resultado.Contains("⚠️") || resultado.Contains("❌"))
                {
                    await ReplyAsync(resultado);
                }
                else
                {
                    // 5. ÉXITO (Verde)
                    // Extraemos el nombre del Pokémon para el título (primera palabra antes del espacio o @)
                    string nombrePokemon = resultado.Split(' ', '@')[0];

                    // Construimos el mensaje con mención, petición y resultado
                    await ReplyAsync(
                        $"👤 **Solicitado por:** {Context.User.Mention}\n" +
                        $"📝 **Petición:** *\"{query}\"*\n" +
                        $"✨ **{nombrePokemon}** generado por IA:\n" +
                        $"```\n{resultado}\n```"
                    );

                    // Borramos el mensaje del usuario para mantener el chat limpio
                    try { await Context.Message.DeleteAsync(); } catch { }
                }
            }
        }
    }
}