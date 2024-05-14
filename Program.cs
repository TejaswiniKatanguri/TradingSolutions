using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sportbet.TradingSolution.Models;
using TradingSolutions.Models;
using TradingSolutions.Services;

#pragma warning disable
namespace TradingSolutions
{
    class Program
    {
        static void Main()
        {
            var func = new Function();
            func.ProcessAsync();
        }

    }

    public class Function
    {
        private readonly IManagerService managerService;
        public Function()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddApplication();
            serviceCollection.AddTransient<IManagerService, ManagerService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            managerService = serviceProvider.GetRequiredService<IManagerService>();
        }

        public async Task ProcessAsync()
        {
            Console.WriteLine("Hello World!");
            // Load player data from JSON file
            var players = LoadPlayersFromFile();

            // Add players to the depth chart
            AddPlayersToDepthChart(players);

            // Print the full depth chart
            PrintFullDepthChart();

            // Get backups for selected players
            await GetAndPrintBackups(players);

            // Print the full depth chart
            PrintFullDepthChart();

            // Remove a player from the depth chart
            await RemoveAndPrintPlayerFromDepthChart(players);

            // Print the updated depth chart
            PrintFullDepthChart();
        }

        public static List<Player> LoadPlayersFromFile()
        {
            var players = new List<Player>();

            foreach (var type in Enum.GetNames(typeof(GameTypes)))
            {
                // Load player data from a JSON file
                var inputFile = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName, $"{type}.json");

                if (File.Exists(inputFile))
                {
                    var inputJson = File.ReadAllText(inputFile);

                    var resp = JsonConvert.DeserializeObject<List<Player>>(inputJson);

                    if (resp?.Any() ?? false)
                        players.AddRange(resp);
                }                
            }            

            return players;
        }

        public void AddPlayersToDepthChart(List<Player> players)
        {
            // Add players to the depth chart
            AddPlayerToDepthChart("QB", "Tom Brady", players, 0);
            AddPlayerToDepthChart("QB", "Blaine Gabbert", players, 1);
            AddPlayerToDepthChart("QB", "Kyle Trask", players, 2);

            AddPlayerToDepthChart("LWR", "Mike Evans", players, 0);
            AddPlayerToDepthChart("LWR", "Jaelon Darden", players, 1);
            AddPlayerToDepthChart("LWR", "Scott Miller", players, 2);
        }

        public void PrintFullDepthChart()
        {
            // Print the full depth chart
            Console.WriteLine("Full Depth Chart:");
            GetFullDepthChart();
        }

        public async Task GetAndPrintBackups(List<Player> players)
        {
            // Get backups for selected players and print them
            var backupTasks = new List<Task<List<Player>>>
            {
                GetBackups("QB", "Tom Brady", players),
                GetBackups("LWR", "Jaelon Darden", players),
                GetBackups("QB", "Mike Evans", players),
                GetBackups("QB", "Blaine Gabbert", players),
                GetBackups("QB", "Kyle Trask", players)
            };

            var backups = await Task.WhenAll(backupTasks);

            foreach (var backup in backups)
            {
                PrintPlayers(backup);
            }
        }

        public async Task RemoveAndPrintPlayerFromDepthChart(List<Player> players)
        {
            // Remove a player from the depth chart and print the removed player
            var playerToRemove = players.FirstOrDefault(x => x.Name == "Mike Evans");
            if (playerToRemove != null)
            {
                var removedPlayer = await RemovePlayerFromDepthChart("LWR", playerToRemove);
                if (removedPlayer != null)
                {
                    Console.WriteLine("Removed Player:");
                    PrintPlayers(new List<Player> { removedPlayer });
                }
            }
        }

        public async Task AddPlayerToDepthChart(string position, string Name, IEnumerable<Player> players, int? positionDepth = null)
        {         
            managerService.AddPlayerToDepthChart(position, Name, players, positionDepth);
        }

        public async void GetFullDepthChart()
        {
            managerService.GetFullDepthChart();
        }

        public async Task<List<Player>> GetBackups(string position, string Name, IEnumerable<Player> players)
        {
            var result = managerService.GetBackups(position, Name, players);
            return result;
        }

        public async Task<Player> RemovePlayerFromDepthChart(string position, Player player)
        {
            var result = managerService.RemovePlayerFromDepthChart(position, player);
            return result;
        }

        public async Task<List<DepthChartData>> GetDepthCharts()
        {
            return managerService.GetDepthCharts();
        }

        public static void PrintPlayers(List<Player> players)
        {
            if (players.Count == 0)
            {
                Console.WriteLine("No Backup");
            }
            else
            {
                foreach (Player player in players)
                {
                    Console.Write("  ");
                    Console.Write(player.Number);
                    Console.Write("  ");
                    Console.Write(player.Name);
                    Console.WriteLine();
                }
            }
        }
    }
}