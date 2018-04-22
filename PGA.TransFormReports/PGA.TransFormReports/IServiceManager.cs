using System.Threading.Tasks;

namespace PGA.TransFormReports
{
    internal interface IServiceManager
    {
        Task<int> StartService();
        bool StopService();
        bool SendReport(object info);
    }
}