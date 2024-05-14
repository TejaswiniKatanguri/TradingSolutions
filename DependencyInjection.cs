using Microsoft.Extensions.DependencyInjection;
using TradingSolutions.Services;

namespace TradingSolutions
{
    public static class DependencyInjection
    {
        public static ServiceProvider AddApplication(this IServiceCollection services)
        {
            return new ServiceCollection()
                            .AddScoped<IManagerService, ManagerService>()
                         .BuildServiceProvider();
        }
    }
}
