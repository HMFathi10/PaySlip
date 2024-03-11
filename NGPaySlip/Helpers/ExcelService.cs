using System.Data;
using System.Data.OleDb;

namespace NGPaySlip.Helpers
{
    public class ExcelService : IExcelService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ExcelService(IWebHostEnvironment environment , IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;

        }
        public DataTable CustomerDataTable(string path)
        {
            var constr = _configuration.GetConnectionString("excelconnection");
            DataTable datatable = new DataTable();

            constr = string.Format(constr, path);

            using (OleDbConnection excelconn = new OleDbConnection(constr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
                    {

                        excelconn.Open();
                        cmd.Connection = excelconn;
                        DataTable excelschema;
                        excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        var sheetname = excelschema.Rows[0]["Table_Name"].ToString();
                        excelconn.Close();

                        excelconn.Open();
                        cmd.CommandText = "SELECT * From [" + sheetname + "] Where EmployeeID Is Not NULL";
                        adapterexcel.SelectCommand = cmd;
                        adapterexcel.Fill(datatable);
                        excelconn.Close();

                    }
                }

            }

            return datatable;
        }

        public string Documentupload(IFormFile fromFiles)
        {
            string uploadpath = _environment.WebRootPath;
            string dest_path = Path.Combine(uploadpath, "uploaded_doc");

            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }
            string sourcefile = Path.GetFileName(fromFiles.FileName);
            string path = Path.Combine(dest_path, DateTime.Now.ToString("dd-mm-yyyy").ToString() + sourcefile );

            using (FileStream filestream = new FileStream(path, FileMode.Create))
            {
                fromFiles.CopyTo(filestream);
            }
            return path;
        }
    }

}
