using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using SampleWebApp.Data;
using SampleWebApp.Models;
using SampleWebApp.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace SampleWebApp.Controllers
{
    public class SentimentDataController : Controller
    {
        private readonly IDataStore dataStore;
        private readonly IModelTrainService modelTrainService;
        private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;

        public SentimentDataController(IDataStore dataStore, PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool, IModelTrainService modelTrainService)
        {
            this.modelTrainService = modelTrainService;
            this._predictionEnginePool = _predictionEnginePool;
            this.dataStore = dataStore;
        }

        public async Task<IActionResult> Index()
        {
            var data = await dataStore.GetSentimentData();

            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("SentimentText, Sentiment")] SentimentData data)
        {
            data = new SentimentData
            {
                SentimentText = Request.Form[nameof(data.SentimentText)],
                Sentiment = Convert.ToBoolean(Request.Form[nameof(data.Sentiment)].ToString().Split(',')[0])
            };

            await dataStore.AddSentimentData(data);

            await modelTrainService.Retrain();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SentimentCheck(string text)
        {
            var output = _predictionEnginePool.Predict("t", new SentimentData { SentimentText = text });

            return View(output);
        }
    }
}