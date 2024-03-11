using System.Data;

namespace NGPaySlip.Helpers
{
    public interface IExcelService
    {
        string Documentupload(IFormFile formFile);
        DataTable CustomerDataTable(string path);
    }
}
