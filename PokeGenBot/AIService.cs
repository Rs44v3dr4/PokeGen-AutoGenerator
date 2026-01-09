using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PokeGenBot
{
    public static class AIService
    {
        // LISTA DE MODELOS A PROBAR (Basado en tu reporte forense)
        // El código intentará con el primero, si falla, salta al siguiente.
        private static readonly string[] Modelos = new[]
        {
            "gemini-2.5-flash",      // El más nuevo (visto en tu cuenta)
            "gemini-2.0-flash-exp",  // El experimental rápido
            "gemini-1.5-flash"       // El estándar (por seguridad)
        };

        // Nota el cambio en el parámetro: ahora recibe una LISTA de strings
        public static async Task<string> GenerarShowdown(string userQuery, List<string> apiKeys)
        {
            // Validamos que haya al menos una llave
            if (apiKeys == null || apiKeys.Count == 0) return "⚠️ Error: No hay API Keys configuradas.";

            using var client = new HttpClient();

            // --- PROMPT MAESTRO ---
            string systemPrompt =
               "Actúa como un experto en mecánica Pokémon para PKHeX.\n" +
               "REGLAS DE ORO (INNEGOCIABLES):\n" +
               "1. LEGALIDAD DE EVs (SUMA 508): Usa 6 stats obligatoriamente. Suma total MAX 508.\n" +
               "   - Ejemplo: 'EVs: 0 HP / 252 Atk / 0 Def / 0 SpA / 4 SpD / 252 Spe'.\n" +
               "2. TRADUCCIÓN DE BAYAS Y OBJETOS (ESTRICTO):\n" +
               "   A) SI EL USUARIO PIDE 'DIMENSIONAL' O 'HÍPER':\n" +
               "      Usa el prefijo 'Hyper ' + el nombre en Inglés de la baya base. USA ESTA TABLA DE REFERENCIA:\n" +
               "      [Zreza->Cheri] [Atania->Chesto] [Meloc->Pecha] [Safre->Rawst] [Perasi->Aspear]\n" +
               "      [Aranja->Oran] [Caquic->Persim] [Ziuela->Lum] [Zidra->Sitrus] [Grana->Pomeg]\n" +
               "      [Algama->Kelpsy] [Ispero->Qualot] [Meluce->Hondew] [Uvav->Grepa] [Tamate->Tamato]\n" +
               "      [Caoca->Occa] [Pasio->Passho] [Gualot->Wacan] [Rind->Rindo] [Hibis->Yache]\n" +
               "      [Pomaro->Chople] [Kouba->Kebia] [Caudol->Shuca] [Cobag->Coba] [Payapa->Payapa]\n" +
               "      [Yapati->Tanga] [Aloca->Charti] [Drasi->Kasib] [Anjiro->Haban] [Baribá->Colbur]\n" +
               "      [Babiri->Babiri] [Chilan->Chilan] [Roseli->Roseli]\n" +
               "      [Yecana->Tanga]\n" +
               "      - Ejemplo: Si piden 'Baya Zidra Híper', escribe: 'Hyper Sitrus Berry'.\n" +
               "   B) BAYAS NORMALES (SIN HÍPER):\n" +
               "      - Traduce por EFECTO (ej: Anti-Hada -> Roseli Berry, Recupera-Vida -> Sitrus Berry).\n" +
               "3. BALL OBLIGATORIA (JERARQUÍA ESTRICTA):\n" +
               "   A) SI EL USUARIO PIDE UNA BALL ESPECÍFICA: ¡OBEDÉCELE!\n" +
               "      - Traduce CUALQUIER Ball solicitada a su nombre oficial en inglés.\n" +
               "      - EJEMPLOS: 'Ente Ball' -> 'Beast Ball', 'Ocaso Ball' -> 'Dusk Ball'.\n" +
               "   B) SOLO SI EL USUARIO NO PIDE BALL: Elige una que combine con los colores del Shiny.\n" +
               "   - FORMATO FINAL: Escribe siempre 'Ball: [Nombre en Inglés]' al final.\n" +
               "4. FORMATO: Showdown puro en Inglés. Línea 1 con @ Objeto.\n" +
               "5. ALPHA: Si piden Alfa, pon 'Alpha: Yes'.\n" +
               "6. SIN MOVIMIENTOS: NO escribas líneas de ataques (las que empiezan con '- '). Omítelos por completo.\n" +
               "7. VARIANTES: Separa con '-----'.\n\n" +
               "EJEMPLO DE SALIDA:\n" +
               "Annihilape (M) @ Hyper Roseli Berry\n" +
               "Ability: Defiant\n" +
               "Level: 100\n" +
               "Shiny: Yes\n" +
               "Alpha: Yes\n" +
               "EVs: 0 HP / 252 Atk / 0 Def / 0 SpA / 4 SpD / 252 Spe\n" +
               "Adamant Nature\n" +
               "Ball: Moon Ball\n\n" +
               $"PETICIÓN DEL USUARIO: {userQuery}";

            // Preparamos el cuerpo del mensaje
            var payload = new { contents = new[] { new { parts = new[] { new { text = systemPrompt } } } } };
            string jsonPayload = JsonConvert.SerializeObject(payload);

            // --- BUCLE NIVEL 1: INTENTAR CADA API KEY ---
            // Esto asegura el ORDEN: Siempre intenta la Key 1, si falla, va a la Key 2, etc.
            foreach (var currentKey in apiKeys)
            {
                string cleanKey = currentKey.Trim();
                if (string.IsNullOrEmpty(cleanKey)) continue; // Si una key está vacía, saltamos a la siguiente

                // --- BUCLE NIVEL 2: INTENTAR CADA MODELO ---
                foreach (var modelo in Modelos)
                {
                    string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelo}:generateContent?key={cleanKey}";

                    try
                    {
                        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(url, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        // 1. ÉXITO
                        if (response.IsSuccessStatusCode)
                        {
                            dynamic dynamicResponse = JsonConvert.DeserializeObject(responseBody);
                            if (dynamicResponse.candidates == null || dynamicResponse.candidates.Count == 0)
                                continue; // Modelo falló, intentar siguiente modelo (misma key)

                            string aiText = dynamicResponse.candidates[0].content.parts[0].text;
                            return aiText.Replace("```yaml", "").Replace("```", "").Trim();
                        }

                        // 2. ERROR 429: SATURACIÓN (Aquí está la magia)
                        if ((int)response.StatusCode == 429)
                        {
                            // Si esta KEY dio 429, es probable que esté quemada para todos los modelos.
                            // Rompemos el bucle de modelos (break) para volver al bucle de keys
                            // y probar la siguiente API KEY inmediatamente.
                            break;
                        }

                        // 3. ERROR 404 O OTROS:
                        // Si el modelo no existe (404), intentamos el siguiente modelo con la MISMA key.
                        if ((int)response.StatusCode == 404)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        // Error de red, probamos siguiente intento
                        continue;
                    }
                }
                // Si el "break" se ejecutó arriba por error 429, el código cae aquí y sube al siguiente ciclo del 'foreach(apiKey)'
            }

            // Si salimos de todos los bucles, fallaron TODAS las keys
            return "⚠️ **Error Total:** Todas las API Keys están saturadas o los servicios de Google no responden.";
        }
    }
}