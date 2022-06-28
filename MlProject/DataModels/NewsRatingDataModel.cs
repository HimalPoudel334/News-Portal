using Microsoft.ML.Data;

namespace MlProject.DataModels;

public class NewsRatingDataModel
{
    [LoadColumn(0)]
    [ColumnName(@"UserId")]
    public float UserId;
    
    [LoadColumn(1)]
    [ColumnName(@"NewsId")]
    public float NewsId;
    
    [LoadColumn(2)]
    [ColumnName(@"Rating")]
    public float Rating;

    [LoadColumn(3)]
    [ColumnName(@"CategoryId")]
    public float CategoryId { get; set; }
    
    [LoadColumn(4)]
    [ColumnName(@"UserLikes")]
    public float UserLikes { get; set; }
}