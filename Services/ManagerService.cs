using TradingSolutions.Models;
using Sportbet.TradingSolution.Models;

#pragma warning disable
namespace TradingSolutions.Services
{
    public class ManagerService : IManagerService
    {
        public List<DepthChartData> depthCharts;

        public ManagerService()
        {
            depthCharts = new List<DepthChartData>();
        }

        public void AddPlayerToDepthChart(string position, string Name, IEnumerable<Player> players, int? positionDepth = null)
        {
            try
            {
                Player player = players?.FirstOrDefault(x => x.Name == Name);

                if (positionDepth == null || depthCharts.Count == 0)
                {
                    depthCharts.Add(new DepthChartData() { Position = position, Player = player, PositionDepth = positionDepth });
                }
                else
                {
                    DepthChartData existingData = depthCharts.FirstOrDefault(t => t.Position == position & t.PositionDepth == positionDepth);
                    if (existingData != null)
                    {
                        UpdateDepthChartAfterAdd(position, positionDepth);
                        depthCharts.Add(new DepthChartData() { Position = position, Player = player, PositionDepth = positionDepth });
                    }
                    else
                    {
                        depthCharts.Add(new DepthChartData() { Position = position, Player = player, PositionDepth = positionDepth });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateDepthChartAfterAdd(string position, int? positionDepth)
        {
            try
            {
                List<DepthChartData> depthChartDataByPosition = depthCharts.FindAll(t => t.Position == position && t.PositionDepth >= positionDepth);
                foreach (DepthChartData depthChartData in depthChartDataByPosition)
                {
                    depthChartData.PositionDepth += 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateDepthChartAfterRemove(string position, int? positionDepth)
        {
            try
            {
                List<DepthChartData> depthChartDataByPosition = depthCharts.FindAll(t => t.Position == position && t.PositionDepth > positionDepth);
                foreach (DepthChartData depthChartData in depthChartDataByPosition)
                {
                    depthChartData.PositionDepth -= 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Player RemovePlayerFromDepthChart(string position, Player player)
        {
            var existingPlayer = new Player();
            try
            {
                var depthChartData = depthCharts.FirstOrDefault(t => t.Position == position & t.Player.Number == player.Number);
                if(depthChartData != null)
                {
                    existingPlayer = depthChartData?.Player;
                    depthCharts.Remove(depthChartData);
                    UpdateDepthChartAfterRemove(position, depthChartData.PositionDepth);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return existingPlayer;
        }

        public List<Player> GetBackups(string position, string Name, IEnumerable<Player> players)
        {
            Player player = players?.FirstOrDefault(x => x.Name == Name);

            List<Player> backups = new List<Player>();
            try
            {
                List<DepthChartData> depthChartDataByPosition = depthCharts.FindAll(t => t.Position == position);
                DepthChartData depthChartPlayerData = depthChartDataByPosition.FirstOrDefault(t => t.Position == position && t.Player.Number == player.Number);
                if (depthChartDataByPosition != null && depthChartPlayerData != null)
                {
                    int? positionDepth = depthChartPlayerData.PositionDepth;
                    foreach (var depthChartData in depthChartDataByPosition)
                    {
                        positionDepth = positionDepth + 1;
                        DepthChartData backupPlayerData = depthChartDataByPosition.FirstOrDefault(t => t.PositionDepth == positionDepth);
                        if (backupPlayerData != null && backupPlayerData.Player != null)
                        {
                            backups.Add(backupPlayerData.Player);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return backups;
        }

        public void GetFullDepthChart()
        {
            try
            {
                var depthChartDataByPosition = depthCharts.GroupBy(t => t.Position);
                foreach (var depthChartDataGroup in depthChartDataByPosition)
                {
                    Console.Write(depthChartDataGroup.Key);
                    Console.Write("   ");
                    foreach (DepthChartData depthChartData in depthChartDataGroup)
                    {
                        Console.Write("  ");
                        Console.Write(depthChartData.Player?.Number);
                        Console.Write("  ");
                        Console.Write(depthChartData.Player?.Name);
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<DepthChartData> GetDepthCharts()
        {
            return depthCharts;
        }
    }

    public interface IManagerService
    {
        void AddPlayerToDepthChart(string position, string Name, IEnumerable<Player> players, int? positionDepth = null);
        Player RemovePlayerFromDepthChart(string position, Player player);
        List<Player> GetBackups(string position, string Name, IEnumerable<Player> players);
        void GetFullDepthChart();
        List<DepthChartData> GetDepthCharts();
    }
}
