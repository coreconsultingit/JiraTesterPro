using System.Data;

namespace JiraTesterProService;

public interface IReconSummaryService
{
    void AddSummary(string PrefileName, string PostFileName, string description, int count);
    DataTable GetReconSummary();
    void ClearMessage();
}
public class ReconSummaryService : IReconSummaryService
{
    private DataTable dtReconMessage;
    public ReconSummaryService()
    {
        dtReconMessage = new DataTable();
        dtReconMessage.Columns.Add("PreFileName");
        dtReconMessage.Columns.Add("PostFileName");
        dtReconMessage.Columns.Add("Description");
        dtReconMessage.Columns.Add("Count");
        dtReconMessage.Columns.Add("Status");
    }


    public void AddSummary(string PrefileName, string PostFileName, string description, int count)
    {
        DataRow row = dtReconMessage.NewRow();
        row["PreFileName"] = PrefileName;
        row["PostFileName"] = PostFileName;
        row["Description"] = description;
        row["Count"] = count;
        row["Status"] = count > 0 ? "Fail" : "Pass";

        dtReconMessage.Rows.Add(row);
    }

    public DataTable GetReconSummary()
    {
        return dtReconMessage;
    }

    public void ClearMessage()
    {
        dtReconMessage.Clear();
    }
}