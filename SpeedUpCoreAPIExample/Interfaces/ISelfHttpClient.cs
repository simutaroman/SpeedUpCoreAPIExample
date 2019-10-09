using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Interfaces
{
    public interface ISelfHttpClient
    {
        Task PostIdAsync(string apiRoute, string id);
    }
}