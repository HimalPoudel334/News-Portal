using Microsoft.ML.Data;

namespace MlProject.DataModels;

public class CategoryPrediction
{
    [ColumnName(@"Category")]
    public uint Category { get; set; }

    [ColumnName(@"Heading")]
    public float[] Heading { get; set; }

    [ColumnName(@"Content")]
    public float[] Content { get; set; }

    [ColumnName(@"Features")]
    public float[] Features { get; set; }

    [ColumnName(@"PredictedLabel")]
    public string PredictedLabel { get; set; }

    [ColumnName(@"Score")]
    public float[] Score { get; set; }


}