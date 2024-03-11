namespace NGPaySlip.Models
{
    public class PaySlipModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string OrganizationName { get; set; }
        public string EmployeeJob {  get; set; }
        public string FixedSalaryValue { get; set; }
        public string AchievedVariableValue { get; set; }
        public string SettlementValue { get; set; }
        public string Bonus_Taxable_Value { get; set; }
        public string OverTime { get; set; }
        public string Total_Earning_Value {  get; set; }
        public string Employee_Pension_Value { get; set; }
        public string Tax_Payable_Value { get; set; }
        public string Deduction_Value1 { get; set; } 
        public string Deduction_Reason {  get; set; }
        public string Total_Deduction_Value {  get; set; }
        public string Net_Payroll_Value {  get; set; }
        public string Bank_Account_No_Value {  get; set; }
        public string Bank { get; set; }

    }
}
