using NGPaySlip.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using NGPaySlip.Models;
using Microsoft.AspNetCore.Authorization;

namespace NGPaySlip.Controllers
{
    [Authorize]
    public class PaySlipController : Controller
    {

        private readonly IExcelService _excelService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PaySlipController( IExcelService excelService, IWebHostEnvironment hostingEnvironment)
        {
            _excelService = excelService;
            _hostingEnvironment = hostingEnvironment;

        }
        public IActionResult Send()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(IFormFile fromFiles)
        {
            try
            {
                string path = _excelService.Documentupload(fromFiles);
                DataTable dt = _excelService.CustomerDataTable(path);
                if (dt.Rows.Count > 0)
                {
                    IList<PaySlipModel> paySlips = new List<PaySlipModel>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        paySlips.Add(new PaySlipModel()
                        {
                            EmployeeId = dr["EmployeeID"].ToString()!,
                            EmployeeName = dr["EmployeeName"].ToString()!,
                            EmployeeEmail = dr["EmployeeEmail"].ToString()!,
                            OrganizationName = dr["OrganizationName"].ToString()!,
                            EmployeeJob = dr["EmployeeJob"].ToString()!,
                            FixedSalaryValue = dr["FixedSalaryValue"].ToString()!,
                            AchievedVariableValue = dr["AchievedVariableValue"].ToString()!,
                            SettlementValue = dr["SettlementValue"].ToString()!,
                            Bonus_Taxable_Value = dr["Bonus_Taxable_Value"].ToString()!,
                            OverTime = dr["Over Time"].ToString()!,
                            Total_Earning_Value = dr["Total_Earning_Value"].ToString()!,
                            Employee_Pension_Value = dr["Employee_Pension_Value"].ToString()!,
                            Tax_Payable_Value = dr["Tax_Payable_Value"].ToString()!,
                            Deduction_Value1 = dr["Deduction_Value1"].ToString()!,
                            Deduction_Reason = dr["Deduction_Reason"].ToString()!,
                            Total_Deduction_Value = dr["Total_Deduction_Value"].ToString()!,
                            Net_Payroll_Value = dr["Net_Payroll_Value"].ToString()!,
                            Bank_Account_No_Value = dr["Bank_Account_No_Value"].ToString()!,
                            Bank = dr["Bank"].ToString()!,

                        });
                    }
                    await SendEmail(paySlips);
                }
                else
                {
                    TempData["Error"] += "There is no data in table";
                }
                return View();

            }
            catch (Exception ex)
            {
                TempData["Error"] += ex.Message;
                return View();
            }
        }
        public async Task SendEmail(IList<PaySlipModel> paySlips)
        {
            foreach (var item in paySlips)
            {
                string body = MapHTMLPage(item);
                await SendEmail_General(item.EmployeeEmail, body);
            }
        }
        public string RoundTwoDicemalPoints(string input)
        {
            string output = "";
            float foutput = 0;
            try
            {
                if (float.TryParse(input, out foutput))
                    output = foutput.ToString("0.00");
                else
                    output = input;
            }
            catch { output = input; }
            return output;
        }
        public string MapHTMLPage(PaySlipModel data)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(webRootPath, "PaySlip.htm");

            string contents = string.Empty;
            string body = string.Empty;

            using (StreamReader sr = new StreamReader(filePath))
            {
                contents = sr.ReadToEnd();
                body = contents.Replace("EmployeeID", data.EmployeeId)
                       .Replace("EmployeeName", data.EmployeeName)
                       .Replace("OrganizationName", data.OrganizationName)
                       .Replace("EmployeeJob", data.EmployeeJob)
                       .Replace("FixedSalaryValue", RoundTwoDicemalPoints(data.FixedSalaryValue))
                       .Replace("AchievedVariableValue", RoundTwoDicemalPoints(data.AchievedVariableValue))
                       .Replace("SettlementValue", RoundTwoDicemalPoints(data.SettlementValue))
                       .Replace("Bonus_Taxable_Value", RoundTwoDicemalPoints(data.Bonus_Taxable_Value))
                       .Replace("Overtime_Taxable_Value", RoundTwoDicemalPoints(data.OverTime))
                       .Replace("Total_Earning_Value", RoundTwoDicemalPoints(data.Total_Earning_Value))
                       .Replace("Employee_Pension_Value", RoundTwoDicemalPoints(data.Employee_Pension_Value))
                       .Replace("Tax_Payable_Value", RoundTwoDicemalPoints(data.Tax_Payable_Value))
                       .Replace("Deduction_Value1", RoundTwoDicemalPoints(data.Deduction_Value1))
                       .Replace("Deduction_Reason", data.Deduction_Reason)
                       .Replace("Total_Deduction_Value", RoundTwoDicemalPoints(data.Total_Deduction_Value))
                       .Replace("Net_Payroll_Value", RoundTwoDicemalPoints(data.Net_Payroll_Value))
                       .Replace("Bank_Account_No_Value", data.Bank_Account_No_Value)
                       .Replace("DateTimeNow", DateTime.Now.ToShortDateString());
            }
            return body;
        }
        public async Task SendEmail_General(string to, string body)
        {

            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://www.rayatrade.com/ProgrammmingUtilities/api/EmailNotification/SendManualEmail`");
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(to), "ToList");
                List<string> ccList = new List<string>()
                {
                    "hamdy_smohamed@rayacorp.com",
                    "mohamed_elhoteibi@rayacorp.com"
                };
                string CCUsers = string.Empty;
                foreach (string item in ccList)
                {
                    CCUsers = CCUsers + item + ";";
                }
                CCUsers = CCUsers.TrimEnd(';');
                content.Add(new StringContent(CCUsers), "CCList");
                content.Add(new StringContent("Employee Payslip"), "Subject");
                content.Add(new StringContent(body), "Body");
                content.Add(new StringContent("RD Best service NG HR Notification"), "Title");
                content.Add(new StringContent("naigeria_hrnotifi@rayacorp.com"), "From");
                content.Add(new StringContent("R@ya2017"), "Password");
                content.Add(new StringContent("true"), "IsBodyHtml");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
        // hamdy_smohamed@rayacorp.com
    }
}
