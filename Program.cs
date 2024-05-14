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
            var inputFile = $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName}\\Player.json ";
            var inputJson = File.ReadAllText(inputFile);
            var players = JsonConvert.DeserializeObject<List<Player>>(inputJson);

            AddPlayerToDepthChart("QB", "Tom Brady", players, 0);
            AddPlayerToDepthChart("QB", "Blaine Gabbert", players, 1);
            AddPlayerToDepthChart("QB", "Kyle Trask", players, 2);

            AddPlayerToDepthChart("LWR", "Mike Evans", players, 0);
            AddPlayerToDepthChart("LWR", "Jaelon Darden", players, 1);
            AddPlayerToDepthChart("LWR", "Scott Miller", players, 2);

            GetFullDepthChart();

            Console.WriteLine("Back up TomBrady");
            List<Player> backUpTomBrady = await GetBackups("QB", "Tom Brady", players);
            PrintPlayers(backUpTomBrady);

            Console.WriteLine();
            Console.WriteLine("Back up JaelonDarden");

            List<Player> backUpJaelonDarden = await GetBackups("LWR", "Jaelon Darden", players);
            PrintPlayers(backUpJaelonDarden);

            Console.WriteLine();
            Console.WriteLine("Back up MikeEvans");
            List<Player> backUpMikeEvans = await GetBackups("QB", "Mike Evans", players);
            PrintPlayers(backUpMikeEvans);

            Console.WriteLine();
            Console.WriteLine("Back up BlaineGabbert");

            List<Player> backUpBlaineGabbert = await GetBackups("QB", "Blaine Gabbert", players);
            PrintPlayers(backUpBlaineGabbert);

            Console.WriteLine();
            Console.WriteLine("Back up KyleTrask");

            List<Player> backUpKyleTrask = await GetBackups("QB", "Kyle Trask", players);
            PrintPlayers(backUpKyleTrask);

            Console.WriteLine();
            Console.WriteLine("Full Depth Chart");
            GetFullDepthChart();

            Console.WriteLine();
            Console.WriteLine("Remove player from Chart");
            Player removedplayer = await RemovePlayerFromDepthChart("LWR", players.FirstOrDefault(x => x.Name == "Mike Evans"));

            if (removedplayer != null)
            {
                Console.WriteLine("Removed Player");
                PrintPlayers(new List<Player> { removedplayer });
            }

            Console.WriteLine();
            Console.WriteLine("Full Depth Chart");
            GetFullDepthChart();
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