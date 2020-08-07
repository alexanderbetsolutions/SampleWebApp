using Newtonsoft.Json;
using SampleWebApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SampleWebApp.Data
{
    public class DataStore : IDataStore
    {
        private readonly string filePath = "data.json";

        public async Task AddSentimentData(SentimentData sentimentData)
        {
            var data = await GetSentimentData();

            data.Add(sentimentData);

            await SaveSentimentData(data);
        }

        private Task SaveSentimentData(IEnumerable<SentimentData> data)
        {
            return File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        public async Task<IList<SentimentData>> GetSentimentData()
        {
            if (!File.Exists(filePath))
            {
                await SaveSentimentData(new List<SentimentData>()
                {
                    new SentimentData
                    {
                        SentimentText = "works great",
                        Sentiment = true
                    }
                });
            }

            var content = await File.ReadAllTextAsync(filePath);

            var data = JsonConvert.DeserializeObject<IList<SentimentData>>(content);

            return data;
        }
    }
}