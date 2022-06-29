using Microsoft.ML;
using Microsoft.ML.Trainers;
using MlProject.DataModels;

namespace MlProject.MlModels;

public class NewsRecommender
{
    private static string MLNetModelPath = Path.GetFullPath("MLModel1.zip");

    public static readonly Lazy<PredictionEngine<NewsRatingDataModel, NewsRatingPrediction>> PredictEngine = new Lazy<PredictionEngine<NewsRatingDataModel, NewsRatingPrediction>>(() => CreatePredictEngine(), true);

    /// <summary>
    /// Use this method to predict on <see cref="ModelInput"/>.
    /// </summary>
    /// <param name="input">model input.</param>
    /// <returns><seealso cref=" ModelOutput"/></returns>
    public static NewsRatingPrediction Predict(NewsRatingDataModel input)
    {
        var predEngine = PredictEngine.Value;
        return predEngine.Predict(input);
    }

    private static PredictionEngine<NewsRatingDataModel, NewsRatingPrediction> CreatePredictEngine()
    {
        var mlContext = new MLContext();
        ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
        return mlContext.Model.CreatePredictionEngine<NewsRatingDataModel, NewsRatingPrediction>(mlModel);
    }
    
    public static ITransformer RetrainPipeline(MLContext mlContext, IDataView trainData)
    {
        var pipeline = BuildPipeline(mlContext);
        var model = pipeline.Fit(trainData);

        return model;
    }

    /// <summary>
    /// build the pipeline that is used from model builder. Use this function to retrain model.
    /// </summary>
    /// <param name="mlContext"></param>
    /// <returns></returns>
    public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
    {
        // Data process configuration with pipeline data transformations
        var pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"NewsId",inputColumnName:@"NewsId")      
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"UserId",inputColumnName:@"UserId"))      
            .Append(mlContext.Recommendation().Trainers.MatrixFactorization(new MatrixFactorizationTrainer.Options(){LabelColumnName=@"Rating",MatrixColumnIndexColumnName=@"UserId",MatrixRowIndexColumnName=@"NewsId",ApproximationRank=33,LearningRate=0.0850207569853028,NumberOfIterations=25,Quiet=true}));

        return pipeline;
    }
}