using Microsoft.ML.Data;

namespace MlProject.DataModels;

public class NewsRatingPrediction
{
    [ColumnName(@"UserId")]
    public uint UserId { get; set; }

    [ColumnName(@"NewsId")]
    public uint NewsId { get; set; }

    [ColumnName(@"Rating")]
    public float Rating { get; set; }

    [ColumnName(@"Timestamp")]
    public float Timestamp { get; set; }

    [ColumnName(@"Score")]
    public float Score { get; set; }


}