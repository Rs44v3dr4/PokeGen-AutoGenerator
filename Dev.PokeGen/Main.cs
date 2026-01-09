using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokeGenBot; // <--- Importante: Usamos la librería compilada

namespace Dev.PokeGen
{
    public partial class Main : Form
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private const string ConfigFile = "config.json";
        private bool _isRunning = false;
        private PokeGenBot.BotConfig _currentConfig;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (File.Exists(ConfigFile))
            {
                try
                {
                    var json = File.ReadAllText(ConfigFile);
                    var config = JsonSerializer.Deserialize<PokeGenBot.BotConfig>(json);

                    if (config != null)
                    {
                        txtToken.Text = config.Token;
                        txtCanal.Text = config.CanalId.ToString();
                        txtRol.Text = config.RolId.ToString();

                        // --- NUEVA LÓGICA DE CARGA ---
                        // Si la lista no es nula, unimos todas las keys con un salto de línea
                        if (config.ApiKeys != null && config.ApiKeys.Count > 0)
                        {
                            txtApiKeys.Text = string.Join(Environment.NewLine, config.ApiKeys);
                        }
                        // -----------------------------

                        LogToConsole("Configuración cargada correctamente.", System.Drawing.Color.Cyan);
                    }
                }
                catch { LogToConsole("Error al cargar la configuración.", System.Drawing.Color.Red); }
            }
        }

        private async void btnIniciar_Click(object sender, EventArgs e)
        {
            string comand = "";
            if (_isRunning) return;

            if (!ulong.TryParse(txtCanal.Text, out ulong canalId)) return;
            if (!ulong.TryParse(txtRol.Text, out ulong rolId)) return;

            // 1. Recoger las Keys del TextBox Multilínea
            List<string> listaKeys = txtApiKeys.Text
                .Split(new[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim()) // Quitamos espacios en blanco accidentales
                .Where(k => !string.IsNullOrEmpty(k)) // Aseguramos que no haya líneas vacías
                .ToList();

            if (listaKeys.Count == 0)
            {
                MessageBox.Show("Debes ingresar al menos una API Key de Gemini.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Creamos el objeto de configuración de la librería
            var config = new PokeGenBot.BotConfig
            {
                Token = txtToken.Text,
                CanalId = canalId,
                RolId = rolId,
                ApiKeys = listaKeys // Guardamos la lista completa
            };

            File.WriteAllText(ConfigFile, JsonSerializer.Serialize(config));

            btnIniciar.Enabled = false;
            LogToConsole("Conectando...", System.Drawing.Color.White);

            try
            {
                await StartBotAsync(config);
                _isRunning = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnIniciar.Enabled = true;
            }
        }

        private async Task BotListoParaTrabajar()
        {
            // 1. Obtenemos el ID del canal desde tu TextBox o Configuración
            // (Asegúrate de que txtCanal.Text tenga un número válido)
            if (ulong.TryParse(txtCanal.Text, out ulong canalId))
            {
                var canal = _client.GetChannel(canalId) as IMessageChannel;

                if (canal != null)
                {
                    // 2. Construimos el "Embed" (La tarjeta bonita)
                    var embed = new EmbedBuilder()
                        .WithTitle("🟢 SISTEMA EN LÍNEA") // Título
                        .WithDescription($"**{_client.CurrentUser.Username}** se ha iniciado correctamente y está listo para recibir pedidos.")
                        .AddField("📅 Fecha de Inicio", DateTime.Now.ToString("dd/MM/yyyy HH:mm tt"), true) // Campo 1 (Lado Izquierdo)
                        .AddField("📶 Latencia", $"{_client.Latency} ms", true) // Campo 2 (Lado Derecho)
                        .WithColor(Discord.Color.Green) // Borde Verde
                        .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl()) // Foto del bot
                        .WithFooter("PokeGen Bot System • v1.0", "https://i.imgur.com/A6j5yD3.png") // Un pie de página opcional
                        .WithCurrentTimestamp()
                        .Build();

                    // 3. Enviamos el mensaje
                    await canal.SendMessageAsync(embed: embed);

                    LogToConsole("Mensaje de inicio enviado al canal.", System.Drawing.Color.Lime);
                }
                else
                {
                    LogToConsole("No se encontró el canal para enviar el saludo.", System.Drawing.Color.Orange);
                }
            }

            // Importante: Desuscribirse para que no lo mande doble si se reconecta solo
            _client.Ready -= BotListoParaTrabajar;
        }

        private async Task StartBotAsync(PokeGenBot.BotConfig config)
        {
            // 1. Guardamos la config globalmente para usarla en el mensaje de bienvenida
            _currentConfig = config;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                // MessageContent es CRÍTICO para leer comandos, ¡bien hecho!
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });

            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(config) // Inyección de dependencia correcta
                .BuildServiceProvider();

            _client.Log += LogAsync;
            _client.MessageReceived += HandleCommandAsync;

            // --- EVENTO 1: Actualizar la Interfaz Gráfica (Tu código original) ---
            _client.Ready += () =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lblEstado.Text = "Conectado: " + _client.CurrentUser.Username;
                    lblEstado.ForeColor = System.Drawing.Color.Green; // Un toque visual extra
                    LogToConsole("¡Bot Online y listo!", System.Drawing.Color.Lime);
                });
                return Task.CompletedTask;
            };

            // --- EVENTO 2: Mandar la Tarjeta de Bienvenida al Canal ---
            _client.Ready += BotListoParaTrabajar;

            // Cargamos el módulo desde la DLL
            await _commands.AddModulesAsync(typeof(PokeGenBot.PokeModule).Assembly, _services);

            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();
        }

        // ... (El resto de funciones HandleCommandAsync, LogAsync y LogToConsole son iguales) ...
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || message.Author.IsBot) return;

            if (!ulong.TryParse(txtCanal.Text, out ulong canalId)) return;

            ulong canalPermitidoId = canalId; // <--- TU ID AQUÍ

            // Si el mensaje NO viene de ese canal, cancelamos la ejecución inmediatamente.
            if (message.Channel.Id != canalPermitidoId)
            {
                return;
            }
            // -------------------------------------

            int argPos = 0;
            if (message.HasCharPrefix('!', ref argPos))
            {
                // Opcional: Si quieres evitar llenar la consola con comandos de otros bots,
                // puedes mover este Log DEBAJO de la ejecución o filtrarlo también.
                LogToConsole($"[PETICIÓN] {message.Author.Username}: {message.Content}", System.Drawing.Color.Yellow);

                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    // --- AQUÍ ESTÁ EL CAMBIO ---
                    if (result.Error == CommandError.UnknownCommand)
                        return;

                    // Si es otro tipo de error (ej. faltan parámetros, error de código), entonces sí avísanos.
                    LogToConsole($"[ERROR] {result.ErrorReason}", System.Drawing.Color.Red);
                }
                else
                {
                    LogToConsole($"[ÉXITO] Pokémon enviado a {message.Author.Username}", System.Drawing.Color.Lime);
                }
            }
        }

        private Task LogAsync(LogMessage log)
        {
            if (log.Exception != null) LogToConsole(log.Message, System.Drawing.Color.Red);
            return Task.CompletedTask;
        }

        private void LogToConsole(string text, System.Drawing.Color color)
        {
            if (rtbLog.InvokeRequired) rtbLog.Invoke(new Action<string, System.Drawing.Color>(LogToConsole), text, color);
            else
            {
                rtbLog.SelectionColor = color;
                rtbLog.AppendText(text + "\n");
                rtbLog.ScrollToCaret();
            }
        }

    }
}
