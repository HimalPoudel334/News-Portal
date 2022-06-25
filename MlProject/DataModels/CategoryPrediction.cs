using Microsoft.ML.Data;

namespace MlProject.DataModels;

public class CategoryPrediction : NewsModel
{
    [ColumnName(@"PredictedLabel")]
    public bool PredictionLabel { get; set; }

    public float Probability { get; set; }

    [ColumnName(@"Score")]
    public float Score { get; set; }
}