using SampleWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleWebApp.Data
{
    public interface IDataStore
    {
        Task AddSentimentData(SentimentData sentimentData);
        Task<IList<SentimentData>> GetSentimentData();
    }
}
