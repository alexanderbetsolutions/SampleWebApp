using Microsoft.ML;
using SampleWebApp.Data;
using SampleWebApp.Models;
using SampleWebApp.Services.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleWebApp.Services
{
    public class ModelTrainService : IModelTrainService
    {
        private readonly IDataStore dataStore;

        public ModelTrainService(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public async Task Retrain()
        {
            var context = new MLContext(seed: 1);

            var data = await GetData();

            var trainingDataView = context.Data.LoadFromEnumerable(data);

            var pipeline = ProcessData(context);

            ITransformer model = BuildAndTrainModel(context, pipeline, trainingDataView);

            context.Model.Save(model, trainingDataView.Schema, "t.zip");
        }

        private Task<IList<SentimentData>> GetData()
        {
            return dataStore.GetSentimentData();
        }

        private ITransformer BuildAndTrainModel(MLContext context, IEstimator<ITransformer> pipeline, IDataView trainingDataView)
        {
            var trainingPipeline = pipeline.Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(SentimentData.Sentiment),
                featureColumnName: "SentimentTextFeaturized"));

            var trainedModel = trainingPipeline.Fit(trainingDataView);

            return trainedModel;
        }

        private static IEstimator<ITransformer> ProcessData(MLContext context)
        {
            var pipeline = context.Transforms.Text.FeaturizeText(inputColumnName: nameof(SentimentData.SentimentText), outputColumnName: "SentimentTextFeaturized");

            return pipeline;
        }
    }
}