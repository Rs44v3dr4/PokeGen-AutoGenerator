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

                        // Carga la API Key guardada para que no tengas que pegarla siempre
                        txtGeminiKey.Text = config.GeminiApiKey;

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

            // Creamos el objeto de configuración de la librería
            var config = new PokeGenBot.BotConfig
            {
                Token = txtToken.Text,
                CanalId = canalId,
                RolId = rolId,
                GeminiApiKey = txtGeminiKey.Text // <--- AQUÍ SE ASIGNA EL VALOR
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

        private async Task StartBotAsync(PokeGenBot.BotConfig config)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });

            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(config) // Inyectamos la config de PokeGenBot
                .BuildServiceProvider();

            _client.Log += LogAsync;
            _client.MessageReceived += HandleCommandAsync;

            _client.Ready += () =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lblEstado.Text = "Conectado: " + _client.CurrentUser.Username;
                    LogToConsole("¡Bot Online!", System.Drawing.Color.Lime);
                });
                return Task.CompletedTask;
            };

            // Cargamos el módulo desde la DLL referenciada
            await _commands.AddModulesAsync(typeof(PokeGenBot.PokeModule).Assembly, _services);

            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();
        }

        // ... (El resto de funciones HandleCommandAsync, LogAsync y LogToConsole son iguales) ...
        // Copia las que te pasé en el mensaje anterior si te faltan.

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasCharPrefix('!', ref argPos))
            {
                // Esto permite "vigilar" desde el front-end
                LogToConsole($"[PETICIÓN] {message.Author.Username}: {message.Content}", System.Drawing.Color.Yellow);

                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    LogToConsole($"[ERROR] {result.ErrorReason}", System.Drawing.Color.Red);
                else
                    LogToConsole($"[ÉXITO] Pokémon enviado a {message.Author.Username}", System.Drawing.Color.Lime);
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
