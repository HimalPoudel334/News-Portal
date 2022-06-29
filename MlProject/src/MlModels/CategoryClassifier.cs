using Microsoft.ML;
using Microsoft.ML.Trainers;
using MlProject.DataModels;

namespace MlProject.MlModels;

public class CategoryClassifier
{
    private static string MLNetModelPath = Path.GetFullPath("../TrainedModels/CategoryPredictionModel.zip");
    
    public static readonly Lazy<PredictionEngine<NewsModel, CategoryPrediction>> PredictEngine = new Lazy<PredictionEngine<NewsModel, CategoryPrediction>>(() => CreatePredictEngine(), true);

    public static ITransformer RetrainPipeline(MLContext mlContext, IDataView trainData)
    {
        var pipeline = BuildPipeline(mlContext);
        var model = pipeline.Fit(trainData);

        return model;
    }
    
    public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
    {
        // Data process configuration with pipeline data transformations
        var pipeline = mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Heading",outputColumnName:@"Heading")      
            .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Content",outputColumnName:@"Content"))      
            .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"Heading",@"Content"}))      
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"Category",inputColumnName:@"Category"))      
            .Append(mlContext.Transforms.NormalizeMinMax(@"Features", @"Features"))      
            .Append(mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(new LbfgsLogisticRegressionBinaryTrainer.Options(){L1Regularization=1F,L2Regularization=1F,LabelColumnName=@"Category",FeatureColumnName=@"Features"}), labelColumnName:@"Category"))      
            .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

        return pipeline;
    }

    public static CategoryPrediction Predict(NewsModel input)
    {
        var predictionEngine = PredictEngine.Value;
        return predictionEngine.Predict(input);

    }
    
    private static PredictionEngine<NewsModel, CategoryPrediction> CreatePredictEngine()
    {
        var mlContext = new MLContext();
        ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
        return mlContext.Model.CreatePredictionEngine<NewsModel, CategoryPrediction>(mlModel);
    }
}

/*
var context = new MLContext(seed: 1);

//Loading data from file
var data = context.Data.LoadFromTextFile<NewsModel>("../../Datasets/category_refined.csv", hasHeader: true, separatorChar: ',');
        
//splitting data to train and test set
var split = context.Data.TrainTestSplit(data, 0.2);
        
//creating features
//features are the columns that we give our ml algo to use to make the model
var features = split.TrainSet.Schema
    .Select(col => col.Name)
    .Where(colName => colName != nameof(NewsModel.Category) && colName != "Date")
    .ToArray();
        
//creating pipeline. This is where we create our model
var pipeline = context.Transforms.Text.FeaturizeText("Category", "Features")
    .Append(context.MulticlassClassification.Trainers.NaiveBayes("Category", "Features"));
    */
