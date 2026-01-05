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

        public static async Task<string> GenerarShowdown(string userQuery, string apiKey)
        {
            string cleanKey = apiKey.Trim();
            if (string.IsNullOrEmpty(cleanKey)) return "⚠️ Error: API Key vacía.";

            using var client = new HttpClient();

            // --- PROMPT MAESTRO (TU VERSIÓN CORRECTA) ---
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
            "      - Ejemplo: Si piden 'Baya Zidra Híper', escribe: 'Hyper Sitrus Berry'.\n" +
            "   B) BAYAS NORMALES (SIN HÍPER):\n" +
            "      - Traduce por EFECTO (ej: Anti-Hada -> Roseli Berry, Recupera-Vida -> Sitrus Berry).\n" +
            "3. BALL OBLIGATORIA: Escribe 'Ball: [Nombre]' al final.\n" +
            "   - Para Shinies, busca colores que combinen (ej: Annihilape Shiny = Moon Ball).\n" +
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

            // Preparamos el cuerpo del mensaje una sola vez
            var payload = new { contents = new[] { new { parts = new[] { new { text = systemPrompt } } } } };
            string jsonPayload = JsonConvert.SerializeObject(payload);

            // --- BUCLE DE INTENTOS INTELIGENTE ---
            foreach (var modelo in Modelos)
            {
                // Construimos la URL dinámica según el modelo actual
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelo}:generateContent?key={cleanKey}";

                try
                {
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // SI FUNCIONA (Código 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        dynamic dynamicResponse = JsonConvert.DeserializeObject(responseBody);

                        // Validación extra por si devuelve JSON vacío
                        if (dynamicResponse.candidates == null || dynamicResponse.candidates.Count == 0)
                            continue; // Intentar siguiente modelo

                        string aiText = dynamicResponse.candidates[0].content.parts[0].text;
                        return aiText.Replace("```yaml", "").Replace("```", "").Trim();
                    }

                    // SI FALLA (Errores conocidos)
                    // 429 = Saturado (Too Many Requests)
                    // 404 = No encontrado (Modelo no disponible en tu cuenta)
                    if ((int)response.StatusCode == 429 || (int)response.StatusCode == 404)
                    {
                        continue; // ¡No te rindas! Prueba el siguiente modelo de la lista
                    }
                }
                catch
                {
                    // Si hay error de red, probamos el siguiente sin detenernos
                    continue;
                }
            }

            // Si llegamos aquí, fallaron los 3 modelos
            return "⚠️ **Error Total:** Los servicios de Google están saturados o la API Key tiene problemas. Intenta en 1 minuto.";
        }
    }
}