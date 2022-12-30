using System.Data;

namespace JiraTesterProData;

public class FileResult
{
    public string FileName { get; set; }
    public DataTable Table { get; set; }

    public FileResult(string fileName, DataTable table)
    {
        FileName = fileName;
        Table = table;
    }
}