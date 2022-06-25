using Microsoft.ML.Data;

namespace MlProject.DataModels;

public class NewsModel
{
    [LoadColumn(0)]
    [ColumnName(@"Category")]
    public string Category;

    [LoadColumn(1)]
    [ColumnName(@"Heading")]
    public string Heading;
    
    [LoadColumn(2)]
    [ColumnName(@"Content")]
    public string Content;
    
}