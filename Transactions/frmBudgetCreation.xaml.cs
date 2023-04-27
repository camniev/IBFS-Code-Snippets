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
    public partial class frmBudgetCreation : ChildWindow
    {
        public Boolean IsRealignment { get; set; }

        public String DivisionId { get; set; }
        public String AccountableID { get; set; }
        public String MOOE_ID { get; set; }
        public String MOOE_Index { get; set; }
        public String ActivityID { get; set; }
        public String _Year { get; set; }
        public String _Month { get; set; }
        public String FundSource { get; set; }

        public String TravelType { get; set; }
        public Double AmountRealigned { get; set; }

        double _valueTravel = 0.00;
        double _valueAllowance = 0.00;
        double _valueNoPersonnel = 0.00;
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
        public event EventHandler CloseRealignment;
        
        private MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private MinDAFSVCClient svc_mindaf_foreign = new MinDAFSVCClient();
        private clsExpenditures c_expenditures = new clsExpenditures();
        private List<LocalTravel> ListLocalTravel  = new List<LocalTravel>();
        private List<LocalAllowance> ListLocalAllowance = new List<LocalAllowance>();
        private List<ForeignTravel> ListForeignTravel = new List<ForeignTravel>();
        private List<ForeignAllowance> ListForeignAllowance = new List<ForeignAllowance>();
        private List<GridData> ListTravelData = new List<GridData>();

        public frmBudgetCreation()
        {
            InitializeComponent();
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;
            svc_mindaf_foreign.ExecuteSQLCompleted += svc_mindaf_foreign_ExecuteSQLCompleted;

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
            _budget_bal = new BudgetRunningBalance(ComputeTotalExpenditure(), this.TravelType, MOOE_ID, MOOE_Index);
            _budget_bal.LockControls += _budget_bal_LockControls;
            _budget_bal._DivisionID = DivisionId;
            _budget_bal._Year = this._Year;
            _budget_bal.WorkingYear = this._Year;
            _budget_bal._FundSource = this.FundSource;
            grdBR.Children.Clear();
            grdBR.Children.Add(_budget_bal);
        }

        void _budget_bal_LockControls(object sender, EventArgs e)
        {
            btnAdd.IsEnabled = false;
            txtRemark.IsEnabled = false;
            txtAllowanceRate.IsEnabled = false;
            txtDays.IsEnabled = false;
            txtPlaneFareRate.IsEnabled = false;
            txtStaff.IsEnabled = false;
            txtTotal.IsEnabled = false;
            cmbAccomodationType.IsEnabled = false;
            cmbDestination.IsEnabled = false;
            //grdData.IsEnabled = false;
        }
        private void SetMonth() 
        {
            DateTime value = new DateTime(DateTime.Now.Year, 1, 1);
            int _yearValue = Convert.ToInt32(_Year);
            switch (_Month)
	        {
                case "Jan":
                    value = new DateTime(_yearValue, 1, 1);
                    break;
                case "Feb":
                    value = new DateTime(_yearValue, 2, 1);
                    break;
                case "Mar":
                    value = new DateTime(_yearValue, 3, 1);
                    break;
                case "Apr":
                    value = new DateTime(_yearValue, 4, 1);
                    break;
                case "May":
                    value = new DateTime(_yearValue, 5,1);
                    break;
                case "Jun":
                    value = new DateTime(_yearValue, 6, 1);
                    break;
                case "Jul":
                    value = new DateTime(_yearValue, 7, 1);
                    break;
                case "Aug":
                    value = new DateTime(_yearValue, 8, 1);
                    break;
                case "Sep":
                    value = new DateTime(_yearValue, 9,1);
                    break;
                case "Oct":
                    value = new DateTime(_yearValue, 10, 1);
                    break;
                case "Nov":
                    value = new DateTime(_yearValue, 11, 1);
                    break;
                case "Dec":
                    value = new DateTime(_yearValue, 12, 1);
                    break;
		
	        }
            dte_To.DisplayDate = value;
            dteFrom.DisplayDate = value;

        }
        void svc_mindaf_foreign_ExecuteSQLCompleted(object sender, ExecuteSQLCompletedEventArgs e)
        {
            var _results = e.Result.ToString();
            switch (c_expenditures.Process)
            {
                case "FetchForeignTravel":
                    XDocument oDocKeyResults = XDocument.Parse(_results);
                    var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                     select new ForeignTravel
                                     {
                                         item_code = Convert.ToString(info.Element("item_code").Value),
                                         item_name = Convert.ToString(info.Element("item_name").Value),
                                         rate = Convert.ToDecimal(info.Element("rate").Value),
                                         rate_year = Convert.ToDecimal(info.Element("rate_year").Value)
                                     };

                    ListForeignTravel.Clear();
                    cmbDestination.Items.Clear();

                    foreach (var item in _dataLists)
                    {
                        ForeignTravel _varDetails = new ForeignTravel();


                        _varDetails.item_code = item.item_code;
                        _varDetails.item_name = item.item_name;
                        _varDetails.rate = item.rate;
                        _varDetails.rate_year = item.rate_year;
                    
                        ListForeignTravel.Add(_varDetails);
                        cmbDestination.Items.Add(item.item_name);
                    }




                    this.Cursor = Cursors.Arrow;

                    GetForeignAllowanceRate();
                    break;
                case "FetchForeignAllowanceRate":
                    XDocument oDocKeyForeignAllowanceRate = XDocument.Parse(_results);
                    var _dataListsForeignAllowanceRatee = from info in oDocKeyForeignAllowanceRate.Descendants("Table")
                                                            select new ForeignAllowance
                                                            {
                                                                item_code = Convert.ToString(info.Element("item_code").Value),
                                                                item_name = Convert.ToString(info.Element("item_name").Value),
                                                                rate = Convert.ToDecimal(info.Element("rate").Value),
                                                                rate_year = Convert.ToDecimal(info.Element("rate_year").Value),
                                                                peso_rate = Convert.ToDecimal(info.Element("peso_rate").Value)
                                                            };

                    ListForeignAllowance.Clear();
                    cmbAccomodationType.Items.Clear();

                    foreach (var item in _dataListsForeignAllowanceRatee)
                    {
                        ForeignAllowance _varDetails = new ForeignAllowance();

                        _varDetails.item_code = item.item_code;
                        _varDetails.item_name = item.item_name;
                        _varDetails.rate = item.rate;
                        _varDetails.rate_year = item.rate_year;
                        _varDetails.peso_rate = item.peso_rate;

                        ListForeignAllowance.Add(_varDetails);
                        cmbAccomodationType.Items.Add(item.item_name);
                    }




                    this.Cursor = Cursors.Arrow;

                    LoadGridData();
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
                                                      No_Days = Convert.ToString(info.Element("No_Days").Value)



                                                  };

                    ListTravelData.Clear();

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
                        _varDetails.No_Days = item.No_Days;

                        ListTravelData.Add(_varDetails);

                    }

                    grdData.ItemsSource = null;
                    grdData.ItemsSource = ListTravelData;

                    grdData.Columns["Service_Type"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Breakfast"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["AM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Lunch"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["PM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Dinner"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Quantity"].Visibility = System.Windows.Visibility.Collapsed;
                    this.Cursor = Cursors.Arrow;

                    LoadBudgetBalance();
                    break;
            }
              
        }

        void svc_mindaf_ExecuteSQLCompleted(object sender, ExecuteSQLCompletedEventArgs e)
        {
              var _results = e.Result.ToString();
              switch (c_expenditures.Process)
              {
                  case "FetchLocalTravel":
                      XDocument oDocKeyResults = XDocument.Parse(_results);
                      var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                       select new LocalTravel
                                       {
                                           item_code = Convert.ToString(info.Element("item_code").Value),
                                           item_name = Convert.ToString(info.Element("item_name").Value),
                                           rate = Convert.ToDecimal(info.Element("rate").Value),
                                           rate_year = Convert.ToDecimal(info.Element("rate_year").Value)
                                       };

                      ListLocalTravel.Clear();
                      cmbDestination.Items.Clear();

                      foreach (var item in _dataLists)
                      {
                          LocalTravel _varDetails = new LocalTravel();


                          _varDetails.item_code = item.item_code;
                          _varDetails.item_name = item.item_name;
                          _varDetails.rate = item.rate;
                          _varDetails.rate_year = item.rate_year;


                          ListLocalTravel.Add(_varDetails);
                          cmbDestination.Items.Add(item.item_name);
                      }

                    


                      this.Cursor = Cursors.Arrow;

                      GetLocalAllowanceRate();
                      break;
                  case "FetchLocalAllowanceRate":
                      XDocument oDocKeyFetchLocalAllowanceRate = XDocument.Parse(_results);
                      var _dataListsFetchLocalAllowanceRate = from info in oDocKeyFetchLocalAllowanceRate.Descendants("Table")
                                       select new LocalAllowance
                                       {
                                           item_code = Convert.ToString(info.Element("item_code").Value),
                                           item_name = Convert.ToString(info.Element("item_name").Value),
                                           rate = Convert.ToDecimal(info.Element("rate").Value),
                                           rate_year = Convert.ToDecimal(info.Element("rate_year").Value)
                                       };

                      ListLocalAllowance.Clear();
                      cmbAccomodationType.Items.Clear();

                      foreach (var item in _dataListsFetchLocalAllowanceRate)
                      {
                          LocalAllowance _varDetails = new LocalAllowance();

                          _varDetails.item_code = item.item_code;
                          _varDetails.item_name = item.item_name;
                          _varDetails.rate = item.rate;
                          _varDetails.rate_year = item.rate_year;


                          ListLocalAllowance.Add(_varDetails);
                          cmbAccomodationType.Items.Add(item.item_name);
                      }

                    


                      this.Cursor = Cursors.Arrow;

                      LoadGridData();
                      break;
                  case "FetchGridData":
                      XDocument oDocKeyFetchGridData = XDocument.Parse(_results);
                      var _dataListsFetchGridData = from info in oDocKeyFetchGridData.Descendants("Table")
                                                              select new GridData
                                                              {
                                                                  ActId = Convert.ToString(info.Element("act_id").Value),
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
                                                                  No_Days = Convert.ToString(info.Element("No_Days").Value)

                                                              };

                      ListTravelData.Clear();
                  
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
                          _varDetails.No_Days = item.No_Days;
                          _varDetails.Remarks = item.Remarks;
                          _varDetails.DateStart = item.DateStart;  
                          _varDetails.Total = item.Total;
                          _varDetails.Travel_Allowance = item.Travel_Allowance;
                         
                          ListTravelData.Add(_varDetails);
                        
                      }

                      grdData.ItemsSource = null;
                      grdData.ItemsSource = ListTravelData;

                      grdData.Columns["ActId"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Service_Type"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Breakfast"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["AM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Lunch"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["PM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Dinner"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["Quantity"].Visibility = System.Windows.Visibility.Collapsed;

                      this.Cursor = Cursors.Arrow;

                      LoadBudgetBalance();
                      break;
              }
              
        }

      

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void frmlocaltravel_Loaded(object sender, RoutedEventArgs e)
        {
            switch (this.TravelType)
            {
                case "Local Travel":
                    GetLocalTravelRate();
                  //  this.Title = "Budget Creation - Local Travel";
                    break;
                case "Foreign Travel":
                    GetForeignTravelRate();
                  //  this.Title = "Budget Creation - International Travel";
                    break;
            }
            SetMonth();
        }

        private void SaveData() 
        {
            String isRev = "0";
            String procureChoice = "0";
            String general_desc = "";
            String _TypeService = "";
            if (IsRevision)
            {
                isRev = "1";
            }

            if (procureRadioBtn.IsChecked == true)
            {
                procureChoice = "1";
            }

            if(this.TravelType == "Local Travel") {
                if (cmbDestination.SelectedItem.ToString() == "PAL Ticket")
                {
                    _TypeService = "Airline Ticket - Philippine Airlines (Local)";
                    general_desc = "Airline Ticket - Local";
                }
                else if (cmbDestination.SelectedItem.ToString() == "Cebu Pacific Ticket")
                {
                    _TypeService = "Airline Ticket - Cebu Pacific (Local)";
                    general_desc = "Airline Ticket - Local";
                } else {
                    _TypeService = "Local Travel - Other Airline";
                    general_desc = "Airline Ticket - Local";
                }

                if (is_mult == true)
                {
                    foreach (var item in _subExList)
                    {
                        c_expenditures.Process = "SaveData";
                        c_expenditures.SQLOperation += c_expenditures_SQLOperation;
                        c_expenditures.SaveProjectBudget(item.activity_id.ToString(), this.AccountableID, txtRemark.Text, Convert.ToDateTime(dteFrom.SelectedDate), Convert.ToDateTime(dte_To.SelectedDate),
                        cmbDestination.SelectedItem.ToString(), txtStaff.Text, Convert.ToDouble(txtPlaneFareRate.Text), Convert.ToDouble(txtAllowanceRate.Text), Convert.ToDouble(txtTotal.Text), item.month.ToString(), this._Year, this.MOOE_Index, _TypeService, txtDays.Text, this.FundSource, isRev, procureChoice, general_desc);
                    }
                }
                else
                {
                    c_expenditures.Process = "SaveData";
                    c_expenditures.SQLOperation += c_expenditures_SQLOperation;
                    c_expenditures.SaveProjectBudget(this.ActivityID, this.AccountableID, txtRemark.Text, Convert.ToDateTime(dteFrom.SelectedDate), Convert.ToDateTime(dte_To.SelectedDate),
                    cmbDestination.SelectedItem.ToString(), txtStaff.Text, Convert.ToDouble(txtPlaneFareRate.Text), Convert.ToDouble(txtAllowanceRate.Text), Convert.ToDouble(txtTotal.Text), this._Month, this._Year, this.MOOE_Index, _TypeService, txtDays.Text, this.FundSource, isRev, procureChoice, general_desc);
                }
            }
            else
            {
                _TypeService = "International Travel - Other Airline";
                general_desc = "Airline Ticket - International";

                if (is_mult == true)
                {
                    foreach (var item in _subExList)
                    {
                        c_expenditures.Process = "SaveData";
                        c_expenditures.SQLOperation += c_expenditures_SQLOperation;
                        c_expenditures.SaveProjectBudget(item.activity_id.ToString(), this.AccountableID, txtRemark.Text, Convert.ToDateTime(dteFrom.SelectedDate), Convert.ToDateTime(dte_To.SelectedDate),
                        cmbDestination.SelectedItem.ToString(), txtStaff.Text, Convert.ToDouble(txtPlaneFareRate.Text), Convert.ToDouble(txtAllowanceRate.Text), Convert.ToDouble(txtTotal.Text), item.month.ToString(), this._Year, this.MOOE_Index, _TypeService, txtDays.Text, this.FundSource, isRev, procureChoice, general_desc);
                    }
                }
                else
                {
                    c_expenditures.Process = "SaveData";
                    c_expenditures.SQLOperation += c_expenditures_SQLOperation;
                    c_expenditures.SaveProjectBudget(this.ActivityID, this.AccountableID, txtRemark.Text, Convert.ToDateTime(dteFrom.SelectedDate), Convert.ToDateTime(dte_To.SelectedDate),
                    cmbDestination.SelectedItem.ToString(), txtStaff.Text, Convert.ToDouble(txtPlaneFareRate.Text), Convert.ToDouble(txtAllowanceRate.Text), Convert.ToDouble(txtTotal.Text), this._Month, this._Year, this.MOOE_Index, _TypeService, txtDays.Text, this.FundSource, isRev, procureChoice, general_desc);
                }
            }
        }

        void c_expenditures_SQLOperation(object sender, EventArgs e)
        {
            switch (c_expenditures.Process.ToString())
            {
                case "SaveData":
                    LoadGridData();
                    
                    if (IsRealignment)
                    {
                        if (CloseRealignment!=null)
                        {
                            this.AmountRealigned =Convert.ToDouble(txtTotal.Value);
                            CloseRealignment(this, new EventArgs());
                        }
                    }
                    ClearData();
                    break;
                case "Suspend":
                    LoadGridData();
                    break;
                default:
                    break;
            }
        }

       

        private void ClearData() 
        {
            txtAllowanceRate.Text = "";
            txtDays.Text ="";
            txtPlaneFareRate.Value = 0;
            txtRemark.Text = "";
            txtStaff.Text = "";
            txtTotal.Value = 0;
            cmbAccomodationType.SelectedIndex = -1;
            cmbDestination.SelectedItem = -1;

        }


        private void GetLocalTravelRate() 
        {
            c_expenditures.Process = "FetchLocalTravel";
            svc_mindaf.ExecuteSQLAsync(c_expenditures.FetchLocalTravel());
        }
        private void GetForeignTravelRate()
        {
            c_expenditures.Process = "FetchForeignTravel";
            svc_mindaf_foreign.ExecuteSQLAsync(c_expenditures.FetchForeignTravel());
        }
        private void GetLocalAllowanceRate()
        {
            c_expenditures.Process = "FetchLocalAllowanceRate";
            svc_mindaf.ExecuteSQLAsync(c_expenditures.FetchLocalAllowance());
        }
        private void GetForeignAllowanceRate()
        {
            c_expenditures.Process = "FetchForeignAllowanceRate";
            svc_mindaf_foreign.ExecuteSQLAsync(c_expenditures.FetchForeignAllowance());
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if ( _budget_bal.BegBalance !=0)
            {
                if ( Convert.ToDouble(txtTotal.Value) > _budget_bal.BalanceOff  )
                {
                     frmNotifyBalance fBa = new frmNotifyBalance();

                     fBa.Show();
                }
                else
                {
                    SaveData();
                }
               
            }
            else
            {
                if (IsRealignment)
                {
                    SaveData();
                }
                else
                {
                    SaveData();
                }
            }
           
           
        }

        private void ComputeDetailsLocal() 
        {
            try
            {
                _valueNoPersonnel = Convert.ToDouble(txtStaff.Text);
            }
            catch (Exception)
            {
                
        
            }

            double _valueDays = 0.00;
            try
            {
                _valueDays =  Convert.ToDouble(txtDays.Text);
            }
            catch (Exception)
            {
    
            }
            //try
            //{
            //    List<LocalTravel> y = ListLocalTravel.Where(item => item.item_name == cmbDestination.SelectedItem.ToString()).ToList();
            //    foreach (var item in y)
            //    {
            //        _valueTravel = Convert.ToDouble(item.rate);
            //        break;
            //    }

            //}
            //catch (Exception)
            //{
            //    _valueTravel = 0;
            //}

            _valueTravel = Convert.ToDouble(txtPlaneFareRate.Text);

            try
            {

                List<LocalAllowance> x = ListLocalAllowance.Where(item => item.item_name == cmbAccomodationType.SelectedItem.ToString()).ToList();
                foreach (var item in x)
                {
                    _valueAllowance = Convert.ToDouble(item.rate);
                    break;
                }

            }
            catch (Exception)
            {
                _valueAllowance = 0;
            }

          
            double _totalTravel = _valueTravel * _valueNoPersonnel;
            double _totalAllowance = (_valueAllowance * _valueDays) * _valueNoPersonnel;
            txtTotal.Value = _totalTravel + _totalAllowance;
        }
        private void ComputeDetailsForeign()
        {
            double _valueTravel = 0.00;
            double _valueAllowance = 0.00;
            double _valueNoPersonnel = 0.00;
            try
            {
                _valueNoPersonnel = Convert.ToDouble(txtStaff.Text);
            }
            catch (Exception)
            {


            }

            double _valueDays = 0.00;
            try
            {
                _valueDays = Convert.ToDouble(txtDays.Text);
            }
            catch (Exception)
            {

            }
            try
            {
                List<ForeignTravel> y = ListForeignTravel.Where(item => item.item_name == cmbDestination.SelectedItem.ToString()).ToList();
                foreach (var item in y)
                {
                    _valueTravel = Convert.ToDouble(item.rate);
                    break;
                }

            }
            catch (Exception)
            {
                _valueTravel = 0;
            }

            try
            {

                List<ForeignAllowance> x = ListForeignAllowance.Where(item => item.item_name == cmbAccomodationType.SelectedItem.ToString()).ToList();
                foreach (var item in x)
                {
                    _valueAllowance = (Convert.ToDouble(item.rate) * Convert.ToDouble(item.peso_rate));
                    break;
                }

            }
            catch (Exception)
            {
                _valueAllowance = 0;
            }


            double _totalTravel = _valueTravel * _valueNoPersonnel;
            double _totalAllowance = _valueAllowance * _valueDays;
            txtTotal.Value = _totalTravel + _totalAllowance;
        }
        public void AddToGrid() 
        {
            List<LocalTravel> y = ListLocalTravel.Where(item => item.item_name == cmbDestination.SelectedItem.ToString()).ToList();
            double _valueTravel = 0.00;
            double _valueAllowance = 0.00;
            double _valueNoPersonnel = Convert.ToDouble(txtStaff.Text);
            double _valueDays = Convert.ToDouble(txtDays.Text);

            foreach (var item in y)
            {
                _valueTravel = Convert.ToDouble(item.rate);
                break;
            }

            List<LocalAllowance> x = ListLocalAllowance.Where(item => item.item_name == cmbAccomodationType.SelectedItem.ToString()).ToList();
            foreach (var item in x)
            {
                _valueAllowance = Convert.ToDouble(item.rate);
                break;
            }

            double _totalTravel = _valueTravel * _valueNoPersonnel;
            double _totalAllowance = _valueAllowance * _valueDays;
            List<GridData> x_data_file = new List<GridData>();

            GridData x_data = new GridData();

            x_data.Activity = "";
            x_data.Assigned = "";

            x_data.Remarks = txtRemark.Text;
            x_data.DateStart = dteFrom.SelectedDate.Value.ToShortDateString();
            x_data.DateEnd = dte_To.SelectedDate.Value.ToShortDateString();
            x_data.Destination = cmbDestination.SelectedItem.ToString();
            x_data.No_Staff = txtStaff.Text;
            x_data.Fare_Rate = _totalTravel.ToString();
            x_data.Travel_Allowance = _totalAllowance.ToString();
            x_data.Total = (_totalTravel + _totalAllowance).ToString();
            x_data_file.Add(x_data);

            ListTravelData = x_data_file;
            grdData.ItemsSource = null;
            grdData.ItemsSource = ListTravelData;
        }

        private void cmbAccomodationType_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                double _valueAllowance = 0.00;

                switch (this.TravelType)
                {
                    case "Local Travel":
                        if (IsRealignment==false) 
                        {

                            List<LocalAllowance> x = ListLocalAllowance.Where(item => item.item_name == cmbAccomodationType.SelectedItem.ToString()).ToList();
                            foreach (var item in x)
                            {
                                _valueAllowance = Convert.ToDouble(item.rate);
                                break;
                            }
                            txtAllowanceRate.Value = _valueAllowance;
                            ComputeDetailsLocal();  
                        }
                   
                        
                      
                        break;
                    case "Foreign Travel":
                        if (IsRealignment == false)
                        {
                            List<ForeignAllowance> _x = ListForeignAllowance.Where(item => item.item_name == cmbAccomodationType.SelectedItem.ToString()).ToList();
                            foreach (var item in _x)
                            {
                                _valueAllowance = Convert.ToDouble(item.rate) * Convert.ToDouble(item.peso_rate);
                                break;
                            }
                            txtAllowanceRate.Value = _valueAllowance;
                            ComputeDetailsForeign();  
                        }
                    
                        break;
                }
               
            }
            catch (Exception)
            {

            }
           
        }

        private void cmbDestination_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                switch (this.TravelType)
                {
                    case "Local Travel":
                        if (IsRealignment ==false)
                        {
                                List<LocalTravel> y = ListLocalTravel.Where(item => item.item_name == cmbDestination.SelectedItem.ToString()).ToList();
                            double _valueTravel = 0.00;

                            foreach (var item in y)
                            {
                                _valueTravel = Convert.ToDouble(item.rate);
                                break;
                            }
                            txtPlaneFareRate.Value = _valueTravel;
                            ComputeDetailsLocal(); 
                        }
                     
                        break;
                    case "Foreign Travel":
                        if (IsRealignment == false)
                        {
                            List<ForeignTravel> _y = ListForeignTravel.Where(item => item.item_name == cmbDestination.SelectedItem.ToString()).ToList();
                            double __valueTravel = 0.00;

                            foreach (var item in _y)
                            {
                                __valueTravel = Convert.ToDouble(item.rate);
                                break;
                            }
                            txtPlaneFareRate.Value = __valueTravel;
                            ComputeDetailsForeign();
                        }
                        
                        break;
                }

                //if (cmbDestination.SelectedItem.ToString() == "Other Airline")
                //{
                //    txtPlaneFareRate.IsReadOnly = false;
                //}
                //else
                //{
                //    txtPlaneFareRate.IsReadOnly = true;
                //}
                
            }
            catch (Exception)
            {

            }
          
        }

        private void txtStaff_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (this.TravelType)
            {
                case "Local Travel":
                    if (IsRealignment==false)
                    {
                        ComputeDetailsLocal();
                    }
                   
                    break;
                case "Foreign Travel":
                    if (IsRealignment == false)
                    {
                        ComputeDetailsForeign();
                    }
              
                    break;
            }
          
        }

        private void txtDays_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (this.TravelType)
            {
                case "Local Travel":
                    if (IsRealignment == false)
                    {
                        ComputeDetailsLocal();
                    }                   
                    break;
                case "Foreign Travel":
                    if (IsRealignment == false)
                    {
                        ComputeDetailsForeign();
                    }
                  
                    break;
            }
          
        }

        private void LoadGridData() 
        {
            c_expenditures.Process = "FetchGridData";
            svc_mindaf.ExecuteSQLAsync(c_expenditures.FetchLocalData(this.ActivityID,this._Month,this._Year,this.MOOE_Index,this.FundSource));
        }

        private void frmlocaltravel_Closed(object sender, EventArgs e)
        {
            if (ReloadData!=null)
            {
                ReloadData(this, new EventArgs());
            }
        }

        private void dte_To_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtDays.Text = ComputeDays(Convert.ToDateTime(dteFrom.SelectedDate), Convert.ToDateTime(dte_To.SelectedDate)).ToString();
            }
            catch (Exception)
            {

            }
            
        }

        private Int32 ComputeDays(DateTime d1, DateTime d2) 
        {
            int _count = 0;
            while  ( d1 != d2)
            {
                _count += 1;
                d1 = d1.AddDays(1);
            }
            _count += 1;
            return _count;
        }

        private void SuspendActivity() 
        {
            String _id = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["ActId"].Value.ToString();
            c_expenditures.Process = "Suspend";
            c_expenditures.SQLOperation += c_expenditures_SQLOperation;
            c_expenditures.UpdateSuspend(_id, "1");
            
        }

        private void btnSuspend_Click(object sender, RoutedEventArgs e)
        {
            SuspendActivity();
        }

        private void txtPlaneFareRate_TextChanged(object sender, EventArgs e)
        {
            switch (this.TravelType)
            {
                case "Local Travel":
                    if (IsRealignment == false)
                    {
                        ComputeDetailsLocal();
                    }

                    break;
                case "Foreign Travel":
                    if (IsRealignment == false)
                    {
                        ComputeDetailsForeign();
                    }

                    break;
            }
        }
    }

    
}

