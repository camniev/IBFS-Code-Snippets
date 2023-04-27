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
using System.Collections.ObjectModel;

namespace MinDAF.Forms
{
    public partial class frmBudgetGasoline : ChildWindow
    {
        public String DivisionId { get; set; }
        public String AccountableID { get; set; }
        public String ActivityID { get; set; }
        public String _Year { get; set; }
        public String _Month { get; set; }
        public String FundSource { get; set; }
        public String TravelType { get; set; }
        public String MOOE_ID { get; set; }
        public String MOOE_INDEX { get; set; }

        public Boolean IsRevision { get; set; }

        public Boolean is_mult { get; set; }

        public ObservableCollection<MultActivities> _subExList;
        public ObservableCollection<MultActivities> subExList
        {
            get { return _subExList; }
            set
            {
                _subExList = value;
            }
        }

        public event EventHandler ReloadData;

        private MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private clsExpenditures c_expenditures = new clsExpenditures();
        private List<GasolineData> ListDestinationGasolineALL = new List<GasolineData>();
        private List<GasolineData> ListDestinationGasoline1D = new List<GasolineData>();
        private List<GasolineData> ListDestinationGasoline2_3D = new List<GasolineData>();
        private List<GasolineData> ListDestinationGasoline4D_MORE = new List<GasolineData>();
        private List<GridData> ListTravelData = new List<GridData>();

        public frmBudgetGasoline()
        {
            InitializeComponent();
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;
        }
        private Double ComputeTotalExpenditure()
        {
            double _Total = 0.00;
            foreach (var item in ListTravelData)
            {
                _Total += Convert.ToDouble(item.Total.ToString());
            }

            return _Total;
        }
        private BudgetRunningBalance _budget_bal { get; set; }
        private void LoadBudgetBalance()
        {
             _budget_bal = new BudgetRunningBalance(ComputeTotalExpenditure(), "Gasoline Expense", MOOE_ID,MOOE_INDEX);
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
            switch (c_expenditures.Process)
            {
                case "FetchGasolineData":
                    XDocument oDocKeyResults = XDocument.Parse(_results);
                    var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                     select new GasolineData
                                     {
                                         item_code = Convert.ToString(info.Element("item_code").Value),
                                         item_name = Convert.ToString(info.Element("item_name").Value),
                                         day = Convert.ToString(info.Element("day").Value),
                                         rate = Convert.ToDecimal(info.Element("rate").Value),
                                         rate_year = Convert.ToDecimal(info.Element("rate_year").Value)
                                     };

                    ListDestinationGasolineALL.Clear();
                    ListDestinationGasoline1D.Clear();
                    ListDestinationGasoline2_3D.Clear();
                    ListDestinationGasoline4D_MORE.Clear();

                    cmbDestination.Items.Clear();

                    foreach (var item in _dataLists)
                    {
                        GasolineData _varDetails = new GasolineData();


                        _varDetails.item_code = item.item_code;
                        _varDetails.item_name = item.item_name;
                        _varDetails.day = item.day;
                        _varDetails.rate = item.rate;
                        _varDetails.rate_year = item.rate_year;


                        ListDestinationGasolineALL.Add(_varDetails);
                        
                    }

                    List<GasolineData> _1D = ListDestinationGasolineALL.Where(item => item.day =="1D").ToList();
                    List<GasolineData> _23D = ListDestinationGasolineALL.Where(item => item.day == "2-3D").ToList();
                    List<GasolineData> _4D = ListDestinationGasolineALL.Where(item => item.day == "4D-MORE").ToList();
                    
                    ListDestinationGasoline1D = _1D;
                    ListDestinationGasoline2_3D = _23D;
                    ListDestinationGasoline4D_MORE = _4D;

                    foreach (var item in _1D)
                    {
                        cmbDestination.Items.Add(item.item_name);
                    }

                    this.Cursor = Cursors.Arrow;
                    GetGasolineType();

                    break;
                case "FetchGasolineType":
                    XDocument oDocKeyFetchGasolineType = XDocument.Parse(_results);
                    var _dataListsFetchGasolineType = from info in oDocKeyFetchGasolineType.Descendants("Table")
                                     select new GasolineTypeData
                                     {
                                         item_code = Convert.ToString(info.Element("item_code").Value),
                                         item_name = Convert.ToString(info.Element("item_name").Value),
                                         rate = Convert.ToDecimal(info.Element("rate").Value),
                                         rate_year = Convert.ToDecimal(info.Element("rate_year").Value)
                                     };

                    cmbActivity.Items.Clear();

                    foreach (var item in _dataListsFetchGasolineType)
                    {
                        GasolineTypeData _varDetails = new GasolineTypeData();


                        _varDetails.item_code = item.item_code;
                        _varDetails.item_name = item.item_name;
                        _varDetails.rate = item.rate;
                        _varDetails.rate_year = item.rate_year;

                        cmbActivity.Items.Add(item.item_name);
                    }

                    LoadGridData();
                    break;
                case "FetchGridData":
                    XDocument oDocKeyFetchGridData = XDocument.Parse(_results);
                    var _dataListsFetchGridData = from info in oDocKeyFetchGridData.Descendants("Table")
                                                  select new GridData
                                                  {
                                                      Service_Type = Convert.ToString(info.Element("TypeService").Value),
                                                      ActId = Convert.ToString(info.Element("ActId").Value),
                                                      Activity = Convert.ToString(info.Element("Activity").Value),
                                                      Assigned = Convert.ToString(info.Element("Assigned").Value),
                                                      Destination = Convert.ToString(info.Element("Destination").Value),
                                                      DateEnd = Convert.ToString(info.Element("DateEnd").Value),
                                                      Fare_Rate = Convert.ToString(info.Element("Fare_Rate").Value),
                                                      No_Staff = Convert.ToString(info.Element("No_Staff").Value),
                                                      No_Days = Convert.ToString(info.Element("No_Days").Value),
                                                      Remarks = Convert.ToString(info.Element("Remarks").Value),
                                                      DateStart = Convert.ToString(info.Element("DateStart").Value),
                                                      Total = Convert.ToString(info.Element("Total").Value),
                                                      Travel_Allowance = Convert.ToString(info.Element("Travel_Allowance").Value)


                                                  };

                    ListTravelData.Clear();

                    foreach (var item in _dataListsFetchGridData)
                    {
                        GridData _varDetails = new GridData();
                        _varDetails.ActId = item.ActId;
                        _varDetails.Service_Type = item.Service_Type;
                        _varDetails.Activity = item.Activity;
                        _varDetails.Assigned = item.Assigned;
                        _varDetails.Destination = item.Destination;
                        _varDetails.DateEnd = item.DateEnd;
                        _varDetails.Fare_Rate = item.Fare_Rate;
                        _varDetails.No_Staff = item.No_Staff;
                        _varDetails.No_Days = item.No_Days;
                        _varDetails.Remarks = item.Remarks;
                        _varDetails.DateStart = item.DateStart;
                        _varDetails.Total = item.Total;
                        _varDetails.Travel_Allowance = item.Travel_Allowance;

                        ListTravelData.Add(_varDetails);

                    }

                    grdData.ItemsSource = null;
                    grdData.ItemsSource = ListTravelData;
                    try 
	                {
                        grdData.Columns["ActId"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["AM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Breakfast"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Dinner"].Visibility = System.Windows.Visibility.Collapsed;

                        grdData.Columns["Lunch"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["No_Days"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["No_Staff"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["PM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Quantity"].Visibility = System.Windows.Visibility.Collapsed;
                       grdData.Columns["No_Days"].Visibility = System.Windows.Visibility.Visible;

                        grdData.Columns["DateStart"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["DateEnd"].Visibility = System.Windows.Visibility.Collapsed;
        
                        grdData.Columns["Fare_Rate"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Travel_Allowance"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Assigned"].Visibility = System.Windows.Visibility.Collapsed;
	                }
	                catch (Exception)
	                {

	                }
                   
                    LoadBudgetBalance();
                    this.Cursor = Cursors.Arrow;


                    break;
            }
        }

        private void GetDestinationGasoline() 
        {
            c_expenditures.Process = "FetchGasolineData";
            svc_mindaf.ExecuteSQLAsync(c_expenditures.FetchGasolineExpense());
        }

        private void GetGasolineType()
        {
            c_expenditures.Process = "FetchGasolineType";
            svc_mindaf.ExecuteSQLAsync(c_expenditures.FetchGasolineTypes());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void frmbudgtgas_Loaded(object sender, RoutedEventArgs e)
        {
            GetDestinationGasoline();
        }

        private void cmbDestination_DropDownClosed(object sender, EventArgs e)
        {
            ComputeData();
        }

        private void SaveData()
        {
            String isRev = "0";
            String procureChoice = "0";
            String general_desc = "";
            String type_service = "";
            if (IsRevision)
            {
                isRev = "1";
            }

            if (procureRadioBtn.IsChecked == true)
           { 
                procureChoice = "1";
            }

            general_desc = "Gasoline, Oil and Lubricants";
            type_service = "Gasoline, Oil and Lubricants Expenses";

            if (is_mult == true)
            {
                foreach (var item in _subExList)
                {
                    c_expenditures.Process = "SaveData";
                    c_expenditures.SQLOperation += c_expenditures_SQLOperation;
                    c_expenditures.SaveProjectBudget(item.activity_id.ToString(), this.AccountableID, txtRemark.Text, DateTime.Now, DateTime.Now,
                    cmbDestination.SelectedItem.ToString(), "0", Convert.ToDouble(txtTotal.Text), 0.00, Convert.ToDouble(txtTotal.Text), item.month.ToString(), this._Year, this.MOOE_INDEX, type_service, txtDays.Text, this.FundSource, isRev, procureChoice, general_desc);
                }
            }
            else
            {
                c_expenditures.Process = "SaveData";
                c_expenditures.SQLOperation += c_expenditures_SQLOperation;
                c_expenditures.SaveProjectBudget(this.ActivityID, this.AccountableID, txtRemark.Text, DateTime.Now, DateTime.Now,
                cmbDestination.SelectedItem.ToString(), "0", Convert.ToDouble(txtTotal.Text), 0.00, Convert.ToDouble(txtTotal.Text), this._Month, this._Year, this.MOOE_INDEX, type_service, txtDays.Text, this.FundSource, isRev, procureChoice, general_desc);
            }
        }
        private void LoadGridData()
        {
            c_expenditures.Process = "FetchGridData";
            svc_mindaf.ExecuteSQLAsync(c_expenditures.FetchLocalDataGas(this.ActivityID, this._Month, this._Year, this.MOOE_INDEX,this.FundSource));
        }

        void c_expenditures_SQLOperation(object sender, EventArgs e)
        {
            ClearData();
            LoadGridData();
        }
        private void ComputeData() 
        {
            int _days = 0;
            double _totals = 0.00;

            try 
	        {
                _days = Convert.ToInt32(txtDays.Text);
	        }
	        catch (Exception)
	        {
	        }
            try
            {
                List<GasolineData> _1D = ListDestinationGasolineALL.Where(item => item.day == "1D" && item.item_name == cmbDestination.SelectedItem.ToString()).ToList();
                List<GasolineData> _23D = ListDestinationGasolineALL.Where(item => item.day == "2-3D" && item.item_name == cmbDestination.SelectedItem.ToString()).ToList();
                List<GasolineData> _4D = ListDestinationGasolineALL.Where(item => item.day == "4D-MORE" && item.item_name == cmbDestination.SelectedItem.ToString()).ToList();


                if (_days == 1)
                {
                    foreach (var item in _1D)
                    {
                        _totals += Convert.ToDouble(item.rate);
                    }
                }
                else if (_days == 2 || _days == 3)
                {
                    foreach (var item in _23D)
                    {
                        _totals += Convert.ToDouble(item.rate);
                    }
                }
                else if (_days >= 4)
                {
                    foreach (var item in _4D)
                    {
                        _totals += Convert.ToDouble(item.rate);
                    }

                }
            }
            catch (Exception)
            {
                
        
            }
          
            txtTotal.Value = _totals;
        }
        private void cmbAccomodationType_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void txtDays_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComputeData();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (_budget_bal.BalanceOff < Convert.ToDouble(txtTotal.Value))
            {
                frmNotifyBalance fBa = new frmNotifyBalance();

                fBa.Show();
            }
            else
            {
                SaveData();
                ClearData();
                LoadGridData();
            }
           
        }

        private void ClearData() 
        {
            txtDays.Text = "0";
            txtRemark.Text = "";
            txtTotal.Value = 0.00;
            cmbActivity.SelectedIndex = -1;
            cmbDestination.SelectedIndex = -1;
            
        }
        private void SuspendActivity()
        {
            String _id = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["ActId"].Value.ToString();
            c_expenditures.Process = "Suspend";
            c_expenditures.SQLOperation += c_expenditures_SQLOperation;
            c_expenditures.UpdateSuspend(_id, "1");

        }
        private void frmbudgtgas_Closed(object sender, EventArgs e)
        {
            if (ReloadData!=null)
            {
                ReloadData(this, new EventArgs());
            }
        }

        private void btnSuspend_Click(object sender, RoutedEventArgs e)
        {
            SuspendActivity();
        }

        
    }
}

