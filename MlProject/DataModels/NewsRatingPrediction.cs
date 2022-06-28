using Microsoft.ML.Data;

namespace MlProject.DataModels;

public class NewsRatingPrediction : NewsRatingDataModel
{
    [ColumnName(@"Score")]
    public float Score { get; set; }
}