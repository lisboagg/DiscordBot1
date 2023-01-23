using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using XAct.Users;
using static System.Windows.Forms.LinkLabel;

namespace MyDiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RunBotAsync().GetAwaiter().GetResult();
        }

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _client.MessageReceived += MessageReceived;
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string botToken = "BOT_TOKEN";

            //event subscriptions
            _client.Log += Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }

        private async Task MessageReceived(SocketMessage message) 
        {
            Dictionary<DateTime, string> schedule = new Dictionary<DateTime, string>();
            schedule.Add(new DateTime(2023, 01, 23), "23-01-2023 19:00-23:00 Língua Inglesa - CRISTINA ISABEL OLIVEIRA BATISTA ");
            schedule.Add(new DateTime(2023, 01, 24), "24-01-2023 19:00-23:00 Programação de computadores - estruturada - ANTÓNIO GUERREIRO PACHECO");
            schedule.Add(new DateTime(2023, 01, 25), "25-01-2023 19:00-23:00 Sistema operativo servidor (plataforma proprietária) - NUNO FILIPE PESQUEIRA MARTINS CARVALHO");
            schedule.Add(new DateTime(2023, 01, 26), "26-01-2023 19:00-23:00 Programação de computadores - estruturada - ANTÓNIO GUERREIRO PACHECO");
            schedule.Add(new DateTime(2023, 01, 27), "27-01-2023 19:00-23:00 Matemática - CRISTINA ISABEL OLIVEIRA BATISTA ");

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            
            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong");

            if (message.Content == "!aula hoje")
            {

                if (schedule.ContainsKey(today))
                {
                    await message.Channel.SendMessageAsync($"Verificando agenda...\n{schedule[today]}");
                }

                else
                {
                    await message.Channel.SendMessageAsync("Verificando agenda...\nNão há aula hoje. ");
                }
            }
            else if (message.Content == "!aula amanhã")
            {

                if (schedule.ContainsKey(tomorrow))
                {
                    await message.Channel.SendMessageAsync($"Verificando agenda...\n{schedule[tomorrow]}");
                }

                else
                {
                    await message.Channel.SendMessageAsync("Verificando agenda...\nNão há aula hoje. ");
                }
            }

            else if (message.Content == "!ajuda")
            {
                await message.Channel.SendMessageAsync($"Verificando comandos...\n!ping ~> Para jogar ping pong com o Bot\n\n!aula hoje ~> Para mostrar os horarios e a matéria que será apresentada no dia" +
                    $"\n\n!aula amanhã ~> Para mostrar os horários e a matéria que será apresentada no dia seguinte\n\n!ajuda ~> Mostra a lista de comando que o Bot possue.");
            }

            if (message.Content == "!agenda")
            {
                if (File.Exists(@"C:\Users\Lisboa\OneDrive - Cinel\Documentos\DiscordBot\agenda\agenda.txt"))
                {
                    StreamReader LerFicheiro = File.OpenText(@"C:\Users\Lisboa\OneDrive - Cinel\Documentos\DiscordBot\agenda.txt");
                    string line = null;
                    while ((line = LerFicheiro.ReadLine()) != null)
                    {
                        await message.Channel.SendMessageAsync(line);
                    }
                    LerFicheiro.Close();
                }
                else
                {
                    await message.Channel.SendMessageAsync("File does not exist or can't be accessed");
                }
            }
           


            //if (message.Content == "!agenda")
            //{
            //    StreamReader LerFicheiro = File.OpenText(@"C:\Users\Lisboa\OneDrive - Cinel\Documentos\DiscordBot\agenda.txt");
            //    string line = null;
            //    while ((line = LerFicheiro.ReadLine()) != null)
            //    {
            //        await message.Channel.SendMessageAsync(line);
            //    }
            //    LerFicheiro.Close();
            //}

            //else if (message.Content == "!agenda")
            //{

            //    if (schedule.ContainsKey(agenda))
            //    {
            //        await message.Channel.SendMessageAsync("Verificando agenda...");
            //        await message.Channel.SendMessageAsync(schedule);
            //    }

            //    else
            //    {
            //        await message.Channel.SendMessageAsync("Verificando agenda...");
            //        await message.Channel.SendMessageAsync("Não há aula hoje.");
            //    }
            //}
        }
    }
}
