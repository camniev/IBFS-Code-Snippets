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
    public partial class frmBudgetTraining : ChildWindow
    {
        public String DivisionId { get; set; }
        public String AccountableID { get; set; }
        public String MOOE_ID { get; set; }
        public String ActivityID { get; set; }
        public String _Year { get; set; }
        public String _Month { get; set; }
        public String FundSource { get; set; }
        public String MOOE_INDEX { get; set; }

        public String _Title { get; set; }

        public Boolean IsRevision { get; set; }

        public event EventHandler ReloadData;
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

        private List<GridData> ListGridData = new List<GridData>();
        private MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private clsBudgetTraining c_train = new clsBudgetTraining();
        private Double _Breakfast_Rate = 0.00;
        private Double _AMSnacks_Rate = 0.00;
        private Double _Lunch_Rate = 0.00;
        private Double _PMSnacks_Rate = 0.00;
        private Double _Dinner_Rate = 0.00;
        private Double _Venue_Daily_Rate = 0.00;
        private Double _Accom_Rate = 0.00;

        private Double _TotalBreakFast = 0.00;
        private Double _TotalSnacksAM = 0.00;
        private Double _TotalLunch = 0.00;
        private Double _TotalSnacksPM = 0.00;
        private Double _TotalDinner = 0.00;
        private Double _TotalVenue = 0.00;
        private Double _TotalHotelAccom = 0.00;
        private Double _Totals = 0.00;

        private Double _numParticipants = 0;
        private Double _numDays = 0;
        private Double _numAccomParticipants = 0;
        private Double _numDaysAccom = 0;

        private Double _total_catering_services = 0.00;
        private Double _total_food_and_venue = 0.00;
        private Double _total_food_venue_accom = 0.00;

        public frmBudgetTraining()
        {
            InitializeComponent();
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;
        }
        private Double ComputeTotal() 
        {
            double _Total = 0.00;
            foreach (var item in ListGridData)
            {
                _Total += Convert.ToDouble(item.Total.ToString());
            }

            return _Total;
        }
        private BudgetRunningBalance _budget_bal { get; set; }
        private void LoadBudgetBalance()
        {
            _budget_bal = new BudgetRunningBalance(ComputeTotal(), _Title, MOOE_ID, MOOE_INDEX);
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
            switch (c_train.Process)
            {
                case "FetchTypeTraining":
                    XDocument oDocKeyResults = XDocument.Parse(_results);
                    var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                     select new TypeTraining
                                     {
                                         item_code = Convert.ToString(info.Element("item_code").Value),
                                         item_name = Convert.ToString(info.Element("item_name").Value),
                                         rate = Convert.ToString(info.Element("rate").Value),
                                         rate_year = Convert.ToString(info.Element("rate_year").Value)

                                     };


                    List<ProfData> _ComboList = new List<ProfData>();

                    foreach (var item in _dataLists)
                    {
                        ProfData _varProf = new ProfData();

                        _varProf._Name = item.item_name;


                        _ComboList.Add(_varProf);

                    }
                    cmbTrainingLocation.ItemsSource = _ComboList;
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
                                                      No_Days = Convert.ToString(info.Element("No_Days").Value),

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
                   // grdData.Columns["Service_Type"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Breakfast"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["AM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Lunch"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Dinner"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["PM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Fare_Rate"].HeaderText = "Rate";
                    grdData.Columns["No_Staff"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Quantity"].Visibility = System.Windows.Visibility.Collapsed;
                    LoadBudgetBalance();
                    this.Cursor = Cursors.Arrow;
                    break;
            }
        }
        void c_train_SQLOperation(object sender, EventArgs e)
        {
            GetGridData();
        }
        private void SaveData()
        {
            String isRev = "0";
            String procureChoice = "0";
            String general_desc = "";
            String SelectedMealType = "";
            String mealLoc = "";
            if (IsRevision)
            {
                isRev = "1";
            }

            if (procureRadioBtn.IsChecked == true)
            {
                procureChoice = "1";
            }

            try {
                mealLoc = cmbTrainingLocation.SelectedItem.ToString();
            }
            catch
            {

            }

            if (is_mult == true)
            {
                foreach (var item in _subExList)
                {
                    //For Catering Services
                    if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == false && chkHotelAccom.IsChecked == false)
                    {
                        general_desc = "Catering Services";
                        SelectedMealType = "Food/Catering Services";
                    }
                    //For Food and  Venue
                    else if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == true && chkHotelAccom.IsChecked == false)
                    {
                        general_desc = "Food and Venue";
                        c_train.Process = "SaveData";
                        c_train.SQLOperation += c_train_SQLOperation;
                        c_train.SaveProjectAdvertising(item.activity_id.ToString(), this.AccountableID, txtRemark.Text,
                            mealLoc, _numParticipants.ToString(), Convert.ToDouble(txtVenueDailyRate.Text), 0, _TotalVenue, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, "Venue - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                    }
                    //For Food & Venue with Accommodation
                    else if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == true && chkHotelAccom.IsChecked == true)
                    {
                        general_desc = "Food and Venue with Accommodation";

                        //FOR VENUE
                        c_train.Process = "SaveData";
                        c_train.SQLOperation += c_train_SQLOperation;
                        c_train.SaveProjectAdvertising(item.activity_id.ToString(), this.AccountableID, txtRemark.Text,
                            mealLoc, _numParticipants.ToString(), Convert.ToDouble(txtVenueDailyRate.Text), 0, _TotalVenue, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, "Venue - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);

                        //FOR HOTEL ACCOMMODATION
                        c_train.Process = "SaveData";
                        c_train.SQLOperation += c_train_SQLOperation;
                        c_train.SaveProjectAdvertising(item.activity_id.ToString(), this.AccountableID, txtRemark.Text,
                            mealLoc, _numAccomParticipants.ToString(), Convert.ToDouble(txtAccomRate.Text), 0, _TotalHotelAccom, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, "Hotel Accommodation - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                    }
                    else if (chkCateringServices.IsChecked == false && chkVenue.IsChecked == false && chkHotelAccom.IsChecked == true)
                    {
                        general_desc = "Hotel Accommodation";
                        c_train.SaveProjectAdvertising(item.activity_id.ToString(), this.AccountableID, txtRemark.Text,
                           mealLoc, _numAccomParticipants.ToString(), Convert.ToDouble(txtAccomRate.Text), 0, _TotalHotelAccom, item.month.ToString(), this._Year,
                           this.MOOE_INDEX, "Hotel Accommodation - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                    }
                    else
                    {
                        general_desc = "";
                    }

                    c_train.Process = "SaveData";
                    c_train.SQLOperation += c_train_SQLOperation;
                    c_train.SaveProjectAdvertising(item.activity_id.ToString(), this.AccountableID, txtRemark.Text,
                        mealLoc, _numParticipants.ToString(), _total_catering_services, 0, _total_catering_services, item.month.ToString(), this._Year,
                        this.MOOE_INDEX, SelectedMealType, _Breakfast_Rate.ToString(), _AMSnacks_Rate.ToString(), _Lunch_Rate.ToString(), _PMSnacks_Rate.ToString(), _Dinner_Rate.ToString(), _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
            }
            else
            {
                //For Catering Services
                if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == false && chkHotelAccom.IsChecked == false)
                {
                    general_desc = "Catering Services";
                    SelectedMealType = "Food/Catering Services";
                }
                //For Food and  Venue
                else if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == true && chkHotelAccom.IsChecked == false)
                {
                    general_desc = "Food and Venue";
                    c_train.Process = "SaveData";
                    c_train.SQLOperation += c_train_SQLOperation;
                    c_train.SaveProjectAdvertising(this.ActivityID, this.AccountableID, txtRemark.Text,
                        mealLoc, _numParticipants.ToString(), Convert.ToDouble(txtVenueDailyRate.Text), 0, _TotalVenue, this._Month, this._Year,
                        this.MOOE_INDEX, "Venue - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
                //For Food & Venue with Accommodation
                else if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == true && chkHotelAccom.IsChecked == true)
                {
                    general_desc = "Food and Venue with Accommodation";

                    //FOR VENUE
                    c_train.Process = "SaveData";
                    c_train.SQLOperation += c_train_SQLOperation;
                    c_train.SaveProjectAdvertising(this.ActivityID, this.AccountableID, txtRemark.Text,
                        mealLoc, _numParticipants.ToString(), Convert.ToDouble(txtVenueDailyRate.Text), 0, _TotalVenue, this._Month, this._Year,
                        this.MOOE_INDEX, "Venue - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);

                    //FOR HOTEL ACCOMMODATION
                    c_train.Process = "SaveData";
                    c_train.SQLOperation += c_train_SQLOperation;
                    c_train.SaveProjectAdvertising(this.ActivityID, this.AccountableID, txtRemark.Text,
                        mealLoc, _numAccomParticipants.ToString(), Convert.ToDouble(txtAccomRate.Text), 0, _TotalHotelAccom, this._Month, this._Year,
                        this.MOOE_INDEX, "Hotel Accommodation - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
                else if (chkCateringServices.IsChecked == false && chkVenue.IsChecked == false && chkHotelAccom.IsChecked == true)
                {
                    general_desc = "Hotel Accommodation";
                    c_train.SaveProjectAdvertising(this.ActivityID, this.AccountableID, txtRemark.Text,
                       mealLoc, _numAccomParticipants.ToString(), Convert.ToDouble(txtAccomRate.Text), 0, _TotalHotelAccom, this._Month, this._Year,
                       this.MOOE_INDEX, "Hotel Accommodation - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
                else
                {
                    general_desc = "";
                }

                c_train.Process = "SaveData";
                c_train.SQLOperation += c_train_SQLOperation;
                c_train.SaveProjectAdvertising(this.ActivityID, this.AccountableID, txtRemark.Text,
                    mealLoc, _numParticipants.ToString(), _total_catering_services, 0, _total_catering_services, this._Month, this._Year,
                    this.MOOE_INDEX, SelectedMealType, _Breakfast_Rate.ToString(), _AMSnacks_Rate.ToString(), _Lunch_Rate.ToString(), _PMSnacks_Rate.ToString(), _Dinner_Rate.ToString(), _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
            }

            //c_train.Process = "SaveData";
            //c_train.SQLOperation += c_train_SQLOperation;
            //c_train.SaveProjectAdvertising(this.ActivityID, this.AccountableID, txtRemark.Text,
            //   "", _rate, ((_rate * _noParticipants) * _noDay), this._Month, this._Year,
            //    this.MOOE_INDEX, _noDay.ToString(), _selectedData, this.FundSource,isRev,procureChoice);


        }
       
        private void GetGridData()
        {
            c_train.Process = "FetchGridData";
            svc_mindaf.ExecuteSQLAsync(c_train.FetchLocalData(this.ActivityID, this._Month, this._Year, this.MOOE_INDEX,this.FundSource));
        }
        private void FetchTypeTraining()
        {
            c_train.Process = "FetchTypeTraining";
            svc_mindaf.ExecuteSQLAsync(c_train.FetchTypeTrainingFees());
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void frm_b_training_Closed(object sender, EventArgs e)
        {
            if (ReloadData!=null)
            {
                ReloadData(this, new EventArgs());
            }
        }

        private void frm_b_training_Loaded(object sender, RoutedEventArgs e)
        {
            FetchTypeTraining();
            CheckChkBoxStatus();
        }

        private void CheckChkBoxStatus()
        {
            txtBreakfastAmnt.Text = ".00";
            txtAMSnacksAmnt.Text = ".00";
            txtLunchAmnt.Text = ".00";
            txtPMSnacksAmnt.Text = ".00";
            txtDinnerAmnt.Text = ".00";
            txtVenueDailyRate.Text = ".00";
            txtAccomRate.Text = ".00";

            txtBreakfastAmnt.IsEnabled = false;
            txtAMSnacksAmnt.IsEnabled = false;
            txtLunchAmnt.IsEnabled = false;
            txtPMSnacksAmnt.IsEnabled = false;
            txtDinnerAmnt.IsEnabled = false;
            nudCateringNo.IsEnabled = false;
            txtVenueDailyRate.IsEnabled = false;
            txtAccomRate.IsEnabled = false;
            nudAccomNo.IsEnabled = false;
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
        private void ComputeTotals()
        {
            try
            {
                _numParticipants = nudCateringNo.Value;
            }
            catch (Exception)
            {
            }

            try
            {
                _numAccomParticipants = nudAccomNo.Value;
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

            try
            {
                _numDaysAccom = numDaysAccom.Value;
            }
            catch (Exception)
            {
            }

            //MessageBox.Show(_AMSnacks_Rate.ToString() +" - "+ _numParticipants.ToString() +" - "+_numDays.ToString());
            //Catering Services
            _TotalBreakFast = (_Breakfast_Rate * _numParticipants) * _numDays;
            _TotalSnacksAM = (_AMSnacks_Rate * _numParticipants) * _numDays;
            _TotalLunch = (_Lunch_Rate * _numParticipants) * _numDays;
            _TotalSnacksPM = (_PMSnacks_Rate * _numParticipants) * _numDays;
            _TotalDinner = (_Dinner_Rate * _numParticipants) * _numDays;
            _total_catering_services = _TotalBreakFast + _TotalSnacksAM + _TotalLunch + _TotalSnacksPM + _TotalDinner;

            //Food and Venue
            _TotalVenue = _Venue_Daily_Rate * _numDays;
            _total_food_and_venue = _total_catering_services + _TotalVenue;

            //Food and Venue with Hotel Accommodation
            _TotalHotelAccom = (_Accom_Rate * _numAccomParticipants) * _numDaysAccom;
            _total_food_venue_accom = _total_food_and_venue + _TotalHotelAccom;

            _Totals = _TotalBreakFast + _TotalSnacksAM + _TotalSnacksPM + _TotalLunch + _TotalDinner + _TotalVenue + _TotalHotelAccom;
            try
            {
                txtTotal.Value = _Totals;
            }
            catch (Exception)
            {
            }

        }
        private void nudParticipants_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void nudDays_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void txtRate_ValueChanged(object sender, EventArgs e)
        {
            ComputeTotals();
        }

        private void SuspendActivity()
        {
            String _id = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["ActId"].Value.ToString();
            c_train.Process = "Suspend";
            c_train.SQLOperation +=c_train_SQLOperation;
            c_train.UpdateSuspend(_id, "1");

        }
        private void btnSuspend_Click(object sender, RoutedEventArgs e)
        {
            SuspendActivity();
        }

        private void chkCateringServices_Checked(object sender, RoutedEventArgs e)
        {
            txtBreakfastAmnt.IsEnabled = true;
            txtAMSnacksAmnt.IsEnabled = true;
            txtLunchAmnt.IsEnabled = true;
            txtPMSnacksAmnt.IsEnabled = true;
            txtDinnerAmnt.IsEnabled = true;
            nudCateringNo.IsEnabled = true;
        }

        private void chkCateringServices_Unchecked(object sender, RoutedEventArgs e)
        {
            txtBreakfastAmnt.IsEnabled = false;
            txtAMSnacksAmnt.IsEnabled = false;
            txtLunchAmnt.IsEnabled = false;
            txtPMSnacksAmnt.IsEnabled = false;
            txtDinnerAmnt.IsEnabled = false;
            nudCateringNo.IsEnabled = false;
            txtBreakfastAmnt.Text = "0.00";
            txtAMSnacksAmnt.Text = "0.00";
            txtLunchAmnt.Text = "0.00";
            txtPMSnacksAmnt.Text = "0.00";
            txtDinnerAmnt.Text = "0.00";
            nudCateringNo.Value = 1;
            ComputeTotals();
        }

        private void txtBreakfastAmnt_TextChanged(object sender, EventArgs e)
        {
            _Breakfast_Rate = Convert.ToDouble(txtBreakfastAmnt.Text);
        }

        private void txtAMSnacksAmnt_TextChanged(object sender, EventArgs e)
        {
            _AMSnacks_Rate = Convert.ToDouble(txtAMSnacksAmnt.Text);
        }

        private void txtLunchAmnt_TextChanged(object sender, EventArgs e)
        {
            _Lunch_Rate = Convert.ToDouble(txtLunchAmnt.Text);
        }

        private void txtPMSnacksAmnt_TextChanged(object sender, EventArgs e)
        {
            _PMSnacks_Rate = Convert.ToDouble(txtPMSnacksAmnt.Text);
        }

        private void txtDinnerAmnt_TextChanged(object sender, EventArgs e)
        {
            _Dinner_Rate = Convert.ToDouble(txtDinnerAmnt.Text);
        }

        private void nudCateringNo_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void chkVenue_Checked(object sender, RoutedEventArgs e)
        {
            if (chkCateringServices.IsChecked == false)
            {
                MessageBox.Show("Can't add venue rate if CATERING SERVICES is unchecked. Please tick the CATERING SERVICES checkbox first.");
                chkVenue.IsChecked = false;
            }
            else
            {
                txtVenueDailyRate.IsEnabled = true;
            }
        }

        private void chkVenue_Unchecked(object sender, RoutedEventArgs e)
        {
            txtVenueDailyRate.IsEnabled = false;
            txtVenueDailyRate.Text = "0.00";
        }

        private void chkHotelAccom_Checked(object sender, RoutedEventArgs e)
        {
            txtAccomRate.IsEnabled = true;
            nudAccomNo.IsEnabled = true;
            numDaysAccom.IsEnabled = true;
        }

        private void chkHotelAccom_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAccomRate.IsEnabled = false;
            nudAccomNo.IsEnabled = false;
            numDaysAccom.IsEnabled = false;
            txtAccomRate.Text = "0.00";
            nudAccomNo.Value = 1;
            ComputeTotals();
        }

        private void txtAccomRate_TextChanged(object sender, EventArgs e)
        {
            _Accom_Rate = Convert.ToDouble(txtAccomRate.Text);
            ComputeTotals();
        }

        private void nudAccomNo_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void txtVenueDailyRate_TextChanged(object sender, EventArgs e)
        {
            _Venue_Daily_Rate = Convert.ToDouble(txtVenueDailyRate.Text);
            ComputeTotals();
        }

        private void numDaysAccom_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }
    }
}

