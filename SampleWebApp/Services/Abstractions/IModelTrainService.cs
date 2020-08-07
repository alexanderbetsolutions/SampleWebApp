using System.Threading.Tasks;

namespace SampleWebApp.Services.Abstractions
{
    public interface IModelTrainService
    {
        Task Retrain();
    }
}