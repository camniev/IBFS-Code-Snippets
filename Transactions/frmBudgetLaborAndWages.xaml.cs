using MinDAF.Class;
using MinDAF.MinDAFS;
using MinDAF.Usercontrol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MinDAF.Forms
{
    public partial class frmBudgetLaborAndWages : ChildWindow
    {
        public String DivisionId { get; set; }
        public String AccountableID { get; set; }
        public String MOOE_ID { get; set; }
        public String MOOE_INDEX { get; set; }
        public String ActivityID { get; set; }
        public String _Year { get; set; }
        public String _Month { get; set; }
        public String FundSource { get; set; }
        public event EventHandler ReloadData;

        private Double SelectedRate = 0;
        private String ProfType = "";
        private String SelectedMealType = "";
        private List<GridData> ListGridData = new List<GridData>();
        private MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private clsBudgetProfessionalFees c_prof = new clsBudgetProfessionalFees();
        private List<ProfessionalRate> ListProfessionalRate = new List<ProfessionalRate>();
        public Boolean IsRevision { get; set; }
        public frmBudgetLaborAndWages()
        {
            InitializeComponent();
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;
            c_prof.SQLOperation += c_prof_SQLOperation;
        }
        private Double ComputeTotalExpenditure()
        {
            double _Total = 0.00;
            foreach (var item in ListGridData)
            {
                _Total += Convert.ToDouble(item.Total.ToString());
            }

            return _Total;
        }
        private BudgetRunningBalance _budget_bal;
        private void LoadBudgetBalance()
        {
            _budget_bal = new BudgetRunningBalance(ComputeTotalExpenditure(), "Labor and Wages", MOOE_ID,MOOE_INDEX);
            _budget_bal._DivisionID = DivisionId;
            _budget_bal._Year = this._Year;
            _budget_bal.WorkingYear = this._Year;
            _budget_bal._FundSource = this.FundSource;
            grdBR.Children.Clear();
            grdBR.Children.Add(_budget_bal);
        }
        void svc_mindaf_ExecuteSQLCompleted(object sender, ExecuteSQLCompletedEventArgs e)
        {
            var _results = e.Result.ToString();
            switch (c_prof.Process)
            {
                case "GetProfRate":
                    XDocument oDocKeyResults = XDocument.Parse(_results);
                    var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                     select new ProfessionalRate
                                     {
                                         item_code = Convert.ToString(info.Element("item_code").Value),
                                         item_name = Convert.ToString(info.Element("item_name").Value),
                                         rate = Convert.ToDecimal(info.Element("rate").Value),
                                         rate_year = Convert.ToDecimal(info.Element("rate_year").Value)

                                     };

                    ListProfessionalRate.Clear();
                    List<ProfData> _ComboList = new List<ProfData>();

                    foreach (var item in _dataLists)
                    {

                        ProfessionalRate _varDetails = new ProfessionalRate();
                        ProfData _varProf = new ProfData();

                        _varDetails.item_code = item.item_code;
                        _varDetails.item_name = item.item_name;
                        _varDetails.rate = item.rate;
                        _varDetails.rate_year = item.rate_year;
            

                        _varProf._Name = item.item_name;

                        ListProfessionalRate.Add(_varDetails);
                        _ComboList.Add(_varProf);

                    }
                    cmbProfType.ItemsSource = _ComboList;
                    this.Cursor = Cursors.Arrow;
                    GetGridData();
                    break;
                case "FetchGridData":
                    XDocument oDocKeyFetchGridData = XDocument.Parse(_results);
                    var _dataListsFetchGridData = from info in oDocKeyFetchGridData.Descendants("Table")
                                                  select new GridData
                                                  {
                                                      ActId = Convert.ToString(info.Element("ActId").Value),
                                                      Activity = Convert.ToString(info.Element("Activity").Value),
                                                      Assigned = Convert.ToString(info.Element("Assigned").Value),
                                                      Destination = Convert.ToString(info.Element("Destination").Value),
                                                      DateEnd = Convert.ToString(info.Element("DateEnd").Value),
                                                      Fare_Rate = Convert.ToString(info.Element("Fare_Rate").Value),
                                                      No_Staff = Convert.ToString(info.Element("No_Staff").Value),
                                                      Remarks = Convert.ToString(info.Element("Remarks").Value),
                                                      DateStart = Convert.ToString(info.Element("DateStart").Value),
                                                      Total = Convert.ToString(info.Element("Total").Value),
                                                      Travel_Allowance = Convert.ToString(info.Element("Travel_Allowance").Value),
                                                      Service_Type = Convert.ToString(info.Element("Service_Type").Value),
                                                      Breakfast = Convert.ToString(info.Element("Breakfast").Value),
                                                      AM_Snacks = Convert.ToString(info.Element("AM_Snacks").Value),
                                                      Lunch = Convert.ToString(info.Element("Lunch").Value),
                                                      PM_Snacks = Convert.ToString(info.Element("PM_Snacks").Value),
                                                      Dinner = Convert.ToString(info.Element("Dinner").Value),
                                                      No_Days = Convert.ToString(info.Element("No_Days").Value)

                                                  };

                    ListGridData.Clear();

                    foreach (var item in _dataListsFetchGridData)
                    {
                        GridData _varDetails = new GridData();

                        _varDetails.ActId = item.ActId;
                        _varDetails.Activity = item.Activity;
                        _varDetails.Assigned = item.Assigned;
                        _varDetails.Destination = item.Destination;
                        _varDetails.DateEnd = item.DateEnd;
                        _varDetails.Fare_Rate = item.Fare_Rate;
                        _varDetails.No_Staff = item.No_Staff;
                        _varDetails.Remarks = item.Remarks;
                        _varDetails.DateStart = item.DateStart;
                        _varDetails.Total = item.Total;
                        _varDetails.Travel_Allowance = item.Travel_Allowance;
                        _varDetails.Service_Type = item.Service_Type;
                        _varDetails.Breakfast = item.Breakfast;
                        _varDetails.AM_Snacks = item.AM_Snacks;
                        _varDetails.Lunch = item.Lunch;
                        _varDetails.PM_Snacks = item.PM_Snacks;
                        _varDetails.Dinner = item.Dinner;
                        _varDetails.No_Days = item.No_Days;
                        ListGridData.Add(_varDetails);

                    }
                    grdData.ItemsSource = null;
                    grdData.ItemsSource = ListGridData;
                    grdData.Columns["ActId"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["DateStart"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["DateEnd"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Travel_Allowance"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Destination"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["DateEnd"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["DateStart"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Travel_Allowance"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Service_Type"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Breakfast"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["AM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Lunch"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Dinner"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["PM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Quantity"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Fare_Rate"].HeaderText = "Rate";
                    grdData.Columns["No_Staff"].HeaderText = "No. of Persons";
                    LoadBudgetBalance();
                    this.Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void GetProfRate()
        {
            c_prof.Process = "GetProfRate";
            svc_mindaf.ExecuteSQLAsync(c_prof.FetchRateProfessionalExpense(MOOE_INDEX, _Year));
        }
        private void GetGridData()
        {
            c_prof.Process = "FetchGridData";
            svc_mindaf.ExecuteSQLAsync(c_prof.FetchLocalData(this.ActivityID, this._Month, this._Year, this.MOOE_INDEX,this.FundSource));
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_budget_bal.BalanceOff < Convert.ToDouble(txtTotal.Value))
            {
                frmNotifyBalance fBa = new frmNotifyBalance();

                fBa.Show();
            }
            else
            {
                SaveData();
            }
        }

       
        private void nudProfService_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void nudDays_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void cmbProfType_DropDownClosed(object sender, EventArgs e)
        {
            var selectedItem = cmbProfType.SelectedItem as ProfData;

            if (selectedItem != null)
            {
                List<ProfessionalRate> x = ListProfessionalRate.Where(item => item.item_name == selectedItem._Name).ToList();
                if (x.Count != 0)
                {
                    ProfType = Convert.ToString(x[0].item_name);
                    SelectedRate = Convert.ToDouble(x[0].rate);
                    txtDailyRate.Value = SelectedRate;
                }
                ComputeTotals();
            }
        }
        private void SaveData()
        {
            String isRev = "0";
            String procureChoice = "0";
            if (IsRevision)
            {
                isRev = "1";
            }

            if (procureRadioBtn.IsChecked == true)
            {
                procureChoice = "1";
            }
            
            double _noProfService = nudProfService.Value;
            double _noDays = nudDays.Value;

            c_prof.Process = "SaveData";
            c_prof.SQLOperation += c_prof_SQLOperation;
            c_prof.SaveProjectProfessionalFee(this.ActivityID, this.AccountableID, txtRemark.Text,
                _noProfService.ToString(), Convert.ToDouble(txtDailyRate.Value), Convert.ToDouble(txtTotal.Value), this._Month, this._Year,
                this.MOOE_INDEX, _noDays.ToString(), this.FundSource,isRev, procureChoice, ProfType, "");


        }

        void c_prof_SQLOperation(object sender, EventArgs e)
        {
            switch (c_prof.Process)
            {
                case "SaveData":
                    GetGridData();
                    break;
                case "Suspend":
                    GetGridData();
                    break;
            }
        }
        private void ComputeTotals()
        {
            Double _numProfService = 0;

            Double _numDays = 0;
            Double _rate = 0;
            try
            {
                _rate = Convert.ToDouble(txtDailyRate.Value);
            }
            catch (Exception)
            {


            }

            try
            {
                _numProfService = nudProfService.Value;
            }
            catch (Exception)
            {
            }

            try
            {
                _numDays = nudDays.Value;
            }
            catch (Exception)
            {
            }

            Double Totals = (_numProfService * _rate) * _numDays;
            try
            {
                txtTotal.Value = Totals;
            }
            catch (Exception)
            {
            }

        }

        private void txtDailyRate_ValueChanged(object sender, EventArgs e)
        {
            ComputeTotals();
        }

        private void frm_b_wages_Loaded(object sender, RoutedEventArgs e)
        {
            GetProfRate();
        }

        private void frm_b_wages_Closed(object sender, EventArgs e)
        {
            if (ReloadData!=null)
            {
                ReloadData(this, new EventArgs());
            }
        }
        private void SuspendActivity()
        {
            String _id = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["ActId"].Value.ToString();
            c_prof.Process = "Suspend";
            c_prof.SQLOperation+=c_prof_SQLOperation;
            c_prof.UpdateSuspend(_id, "1");

        }

        private void btnSuspend_Click(object sender, RoutedEventArgs e)
        {
            SuspendActivity();
        }

       

       
    }
}

