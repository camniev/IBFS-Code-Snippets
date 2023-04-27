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
    public partial class frmBudgetRepresentation : ChildWindow
    {
        public String DivisionId { get; set; }
        public String AccountableID { get; set; }
        public String MOOE_ID { get; set; }
        public String MOOE_INDEX { get; set; }
        public String ActivityID { get; set; }
        public String _Year { get; set; }
        public String _Month { get; set; }
        public String FundSource { get; set; }

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

        private String SelectedArea = "";
        private String SelectedMealType = "";
        private MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private clsBudgetRepresentation c_representation = new clsBudgetRepresentation();
        private List<RepresentationArea> ListArea = new List<RepresentationArea>();
        private List<MealServiceType> ListMealServiceType = new List<MealServiceType>();
        private List<RespresentationValuesData> ListRepresentationValues = new List<RespresentationValuesData>();
        private List<GridData> ListGridData= new List<GridData>();
        private List<DescData> ListDescData = new List<DescData>();
        private Double _Breakfast_Rate = 0.00;
        private Double _AMSnacks_Rate = 0.00;
        private Double _Lunch_Rate = 0.00;
        private Double _PMSnacks_Rate = 0.00;
        private Double _Dinner_Rate = 0.00;
        private Double _Venue_Daily_Rate = 0.00;
        private Double _Accom_Rate = 0.00;
        private Double _Token_Rate = 0.00;

        private Double _TotalBreakFast = 0.00;
        private Double _TotalSnacksAM = 0.00;
        private Double _TotalLunch = 0.00;
        private Double _TotalSnacksPM = 0.00;
        private Double _TotalDinner = 0.00;
        private Double _TotalVenue = 0.00;
        private Double _TotalHotelAccom = 0.00;
        private Double _TotalToken = 0.00;
        private Double _Totals = 0.00;

        private Double  _numParticipants = 0;
        private Double  _numDays = 0;
        private Double  _numAccomParticipants = 0;
        private Double  _numTokens = 0;
        private Double _numDaysAccom = 0;

        private Double _total_catering_services = 0.00;
        private Double _total_food_and_venue = 0.00;
        private Double _total_food_venue_accom = 0.00;
     
        public frmBudgetRepresentation()
        {
            InitializeComponent();
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;
            c_representation.SQLOperation += c_representation_SQLOperation;
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
        private BudgetRunningBalance _budget_bal { get; set; }
        private void LoadBudgetBalance()
        {
            BudgetRunningBalance _budget_bal = new BudgetRunningBalance(ComputeTotalExpenditure(), "Representation Expenses", MOOE_ID,MOOE_INDEX);
            _budget_bal._DivisionID = DivisionId;
            _budget_bal._Year = this._Year;
            _budget_bal.WorkingYear = this._Year;
            _budget_bal._FundSource = this.FundSource;
            grdBR.Children.Clear();
            grdBR.Children.Add(_budget_bal);
        }
        void c_representation_SQLOperation(object sender, EventArgs e)
        {
            switch (c_representation.Process)
            {
                case "SaveData":
                    GetGridData();
                    break;
                case "Suspend":
                    GetGridData();
                    break; 
            }
        }
        
        void svc_mindaf_ExecuteSQLCompleted(object sender, ExecuteSQLCompletedEventArgs e)
        {
             var _results = e.Result.ToString();
            switch (c_representation.Process)
            {
                case "GetAreas":
                     XDocument oDocKeyResults = XDocument.Parse(_results);
                    var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                     select new RepresentationArea
                                     {
                                         Area = Convert.ToString(info.Element("Area").Value)
                                   
                                     };

                    ListArea.Clear();

                   
                    foreach (var item in _dataLists)
                    {
                       
                            RepresentationArea _varDetails = new RepresentationArea();


                            _varDetails.Area = item.Area;


                            ListArea.Add(_varDetails);
                     
                    }
                    cmbArea.ItemsSource = ListArea;
                    this.Cursor = Cursors.Arrow;
                    GetMealServiceType();
                    break;
                case "GetMealServiceType":
                    XDocument oDocKeyResultsGetMealServiceType = XDocument.Parse(_results);
                    var _dataListsGetMealServiceType = from info in oDocKeyResultsGetMealServiceType.Descendants("Table")
                                     select new MealServiceType
                                     {
                                         Service_Type = Convert.ToString(info.Element("Service_Type").Value)

                                     };

                    ListMealServiceType.Clear();


                    foreach (var item in _dataListsGetMealServiceType)
                    {

                        MealServiceType _varDetails = new MealServiceType();


                        _varDetails.Service_Type = item.Service_Type;


                        ListMealServiceType.Add(_varDetails);

                    }
                    cmbServiceType.ItemsSource = ListMealServiceType;
                    this.Cursor = Cursors.Arrow;
                    GetRepresentationValues();
                    break;
                case "GetRepresentationValues":
                    XDocument oDocKeyResultsGetRepresentationValues = XDocument.Parse(_results);
                    var _dataListsGetRepresentationValues = from info in oDocKeyResultsGetRepresentationValues.Descendants("Table")
                                     select new RespresentationValuesData
                                     {
                                            item_code = Convert.ToString(info.Element("item_code").Value),
                                            item_name  = Convert.ToString(info.Element("item_name").Value),
                                            library_type  = Convert.ToString(info.Element("library_type").Value),
                                            rate = Convert.ToString(info.Element("rate").Value),
                                            rate_year = Convert.ToString(info.Element("rate_year").Value)

                                     };

                    ListRepresentationValues.Clear();


                    foreach (var item in _dataListsGetRepresentationValues)
                    {

                        RespresentationValuesData _varDetails = new RespresentationValuesData();


                        _varDetails.item_code = item.item_code;
                        _varDetails.item_name = item.item_name;
                        _varDetails.library_type = item.library_type;
                        _varDetails.rate = item.rate;
                        _varDetails.rate_year = item.rate_year;

                        ListRepresentationValues.Add(_varDetails);

                    }
                   
                    this.Cursor = Cursors.Arrow;
                    GetGridData();
                    GetDescData();
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
                    grdData.Columns["DateStart"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["DateEnd"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Fare_Rate"].Visibility = System.Windows.Visibility.Visible;
                    grdData.Columns["Travel_Allowance"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Remarks"].Visibility = System.Windows.Visibility.Collapsed;
                    LoadBudgetBalance();
                    this.Cursor = Cursors.Arrow;


                    break;
                case "FetchDescData":
                    XDocument oDocKeyFetchDescData = XDocument.Parse(_results);
                    var _dataListsFetchDescData = from info in oDocKeyFetchDescData.Descendants("Table")
                                                  select new DescData
                                                  {
                                                      remarks = Convert.ToString(info.Element("remarks").Value)
                                                  };

                    ListDescData.Clear();

                    foreach (var item in _dataListsFetchDescData)
                    {
                        DescData _var_Details = new DescData();

                        _var_Details.remarks = item.remarks;
                        ListDescData.Add(_var_Details);

                    }

                    txtRemark2.ItemsSource = null;
                    txtRemark2.ItemsSource = ListDescData;
                    this.Cursor = Cursors.Arrow;

                    break;
            }
        }
        private void SaveData()
        {
            String _breakfast =cmbBreakfast.SelectedItem.ToString();
            String _am_snacks = cmbAMSnacks.SelectedItem.ToString();
            String _lunch = cmbLunch.SelectedItem.ToString();
            String _pm_snacks = cmbpmsnacks.SelectedItem.ToString();
            String _dinner = cmbdinner.SelectedItem.ToString();
            String isRev = "0";
            String procureChoice = "0";
            String general_desc = "";
            if (IsRevision)
            {
                isRev = "1";
            }

            if (procureRadioBtn.IsChecked == true)
            {
                procureChoice = "1";
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
                        c_representation.Process = "SaveData";
                        c_representation.SQLOperation += c_representation_SQLOperation;
                        c_representation.SaveProjectRepresentation(item.activity_id.ToString(), this.AccountableID, txtRemark2.Text,
                            SelectedArea, _numParticipants.ToString(), _total_catering_services, 0, Convert.ToDouble(txtTotal.Value), item.month.ToString(), this._Year,
                            this.MOOE_INDEX, SelectedMealType, _Breakfast_Rate.ToString(), _AMSnacks_Rate.ToString(), _Lunch_Rate.ToString(), _PMSnacks_Rate.ToString(), _Dinner_Rate.ToString(), _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                    }
                    //For Food and  Venue
                    else if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == true && chkHotelAccom.IsChecked == false)
                    {
                        general_desc = "Food and Venue";
                        SelectedMealType = "Food/Catering Services";

                        //Catering Services
                        c_representation.Process = "SaveData";
                        c_representation.SQLOperation += c_representation_SQLOperation;
                        c_representation.SaveProjectRepresentation(item.activity_id.ToString(), this.AccountableID, txtRemark2.Text,
                            SelectedArea, _numParticipants.ToString(), _total_catering_services, 0, _total_catering_services, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, SelectedMealType, _Breakfast_Rate.ToString(), _AMSnacks_Rate.ToString(), _Lunch_Rate.ToString(), _PMSnacks_Rate.ToString(), _Dinner_Rate.ToString(), _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);

                        //Venue
                        c_representation.Process = "SaveData";
                        c_representation.SQLOperation += c_representation_SQLOperation;
                        c_representation.SaveProjectRepresentation(item.activity_id.ToString(), this.AccountableID, txtRemark2.Text,
                            SelectedArea, _numParticipants.ToString(), Convert.ToDouble(txtVenueDailyRate.Text), 0, _TotalVenue, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, "Venue - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                    }
                    //For Food & Venue with Accommodation
                    else if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == true && chkHotelAccom.IsChecked == true)
                    {
                        general_desc = "Food and Venue with Accommodation";
                        SelectedMealType = "Food/Catering Services";

                        //Food
                        c_representation.Process = "SaveData";
                        c_representation.SQLOperation += c_representation_SQLOperation;
                        c_representation.SaveProjectRepresentation(item.activity_id.ToString(), this.AccountableID, txtRemark2.Text,
                            SelectedArea, _numParticipants.ToString(), _total_catering_services, 0, _total_catering_services, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, SelectedMealType, _Breakfast_Rate.ToString(), _AMSnacks_Rate.ToString(), _Lunch_Rate.ToString(), _PMSnacks_Rate.ToString(), _Dinner_Rate.ToString(), _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);

                        //Venue
                        c_representation.Process = "SaveData";
                        c_representation.SQLOperation += c_representation_SQLOperation;
                        c_representation.SaveProjectRepresentation(item.activity_id.ToString(), this.AccountableID, txtRemark2.Text,
                            SelectedArea, _numParticipants.ToString(), Convert.ToDouble(txtVenueDailyRate.Text), 0, _TotalVenue, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, "Venue - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);

                        //Hotel
                        c_representation.Process = "SaveData";
                        c_representation.SQLOperation += c_representation_SQLOperation;
                        c_representation.SaveProjectRepresentation(item.activity_id.ToString(), this.AccountableID, txtRemark2.Text,
                            SelectedArea, _numAccomParticipants.ToString(), Convert.ToDouble(txtAccomRate.Text), 0, _TotalHotelAccom, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, "Hotel Accommodation - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                    }
                    else if (chkCateringServices.IsChecked == false && chkVenue.IsChecked == false && chkHotelAccom.IsChecked == true)
                    {
                        general_desc = "Hotel Accommodation";
                        SelectedMealType = "Hotel Accommodation";
                        c_representation.Process = "SaveData";
                        c_representation.SQLOperation += c_representation_SQLOperation;
                        c_representation.SaveProjectRepresentation(item.activity_id.ToString(), this.AccountableID, txtRemark2.Text,
                            SelectedArea, _numAccomParticipants.ToString().ToString(), _total_catering_services, 0, Convert.ToDouble(txtTotal.Value), item.month.ToString(), this._Year,
                            this.MOOE_INDEX, SelectedMealType, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                    }
                    else
                    {
                        general_desc = "";
                    }


                    //IF TOKEN IS ONLY CHECKED
                    if (chkToken.IsChecked == true)
                    {
                        general_desc = "Token";
                        SelectedMealType = "Token";
                        c_representation.Process = "SaveData";
                        c_representation.SQLOperation += c_representation_SQLOperation;
                        c_representation.SaveTokenRepresentation(item.activity_id.ToString(), this.AccountableID, txtRemark2.Text,
                            SelectedArea, _numTokens.ToString(), _Token_Rate, 0, _TotalToken, item.month.ToString(), this._Year,
                            this.MOOE_INDEX, SelectedMealType, "-", "-", "-", "-", "-", "-", _numTokens.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                    }
                }
            }
            else
            {
                //For Catering Services
                if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == false && chkHotelAccom.IsChecked == false)
                {
                    general_desc = "Catering Services";
                    SelectedMealType = "Food/Catering Services";
                    c_representation.Process = "SaveData";
                    c_representation.SQLOperation += c_representation_SQLOperation;
                    c_representation.SaveProjectRepresentation(this.ActivityID, this.AccountableID, txtRemark2.Text,
                        SelectedArea, _numParticipants.ToString(), _total_catering_services, 0, Convert.ToDouble(txtTotal.Value), this._Month, this._Year,
                        this.MOOE_INDEX, SelectedMealType, _Breakfast_Rate.ToString(), _AMSnacks_Rate.ToString(), _Lunch_Rate.ToString(), _PMSnacks_Rate.ToString(), _Dinner_Rate.ToString(), _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
                //For Food and  Venue
                else if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == true && chkHotelAccom.IsChecked == false)
                {
                    general_desc = "Food and Venue";
                    SelectedMealType = "Food/Catering Services";

                    //Catering Services
                    c_representation.Process = "SaveData";
                    c_representation.SQLOperation += c_representation_SQLOperation;
                    c_representation.SaveProjectRepresentation(this.ActivityID, this.AccountableID, txtRemark2.Text,
                        SelectedArea, _numParticipants.ToString(), _total_catering_services, 0, _total_catering_services, this._Month, this._Year,
                        this.MOOE_INDEX, SelectedMealType, _Breakfast_Rate.ToString(), _AMSnacks_Rate.ToString(), _Lunch_Rate.ToString(), _PMSnacks_Rate.ToString(), _Dinner_Rate.ToString(), _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);

                    //Venue
                    c_representation.Process = "SaveData";
                    c_representation.SQLOperation += c_representation_SQLOperation;
                    c_representation.SaveProjectRepresentation(this.ActivityID, this.AccountableID, txtRemark2.Text,
                        SelectedArea, _numParticipants.ToString(), Convert.ToDouble(txtVenueDailyRate.Text), 0, _TotalVenue, this._Month, this._Year,
                        this.MOOE_INDEX, "Venue - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
                //For Food & Venue with Accommodation
                else if (chkCateringServices.IsChecked == true && chkVenue.IsChecked == true && chkHotelAccom.IsChecked == true)
                {
                    general_desc = "Food and Venue with Accommodation";
                    SelectedMealType = "Food/Catering Services";

                    //Food
                    c_representation.Process = "SaveData";
                    c_representation.SQLOperation += c_representation_SQLOperation;
                    c_representation.SaveProjectRepresentation(this.ActivityID, this.AccountableID, txtRemark2.Text,
                        SelectedArea, _numParticipants.ToString(), _total_catering_services, 0, _total_catering_services, this._Month, this._Year,
                        this.MOOE_INDEX, SelectedMealType, _Breakfast_Rate.ToString(), _AMSnacks_Rate.ToString(), _Lunch_Rate.ToString(), _PMSnacks_Rate.ToString(), _Dinner_Rate.ToString(), _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);

                    //Venue
                    c_representation.Process = "SaveData";
                    c_representation.SQLOperation += c_representation_SQLOperation;
                    c_representation.SaveProjectRepresentation(this.ActivityID, this.AccountableID, txtRemark2.Text,
                        SelectedArea, _numParticipants.ToString(), Convert.ToDouble(txtVenueDailyRate.Text), 0, _TotalVenue, this._Month, this._Year,
                        this.MOOE_INDEX, "Venue - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);

                    //Hotel
                    c_representation.Process = "SaveData";
                    c_representation.SQLOperation += c_representation_SQLOperation;
                    c_representation.SaveProjectRepresentation(this.ActivityID, this.AccountableID, txtRemark2.Text,
                        SelectedArea, _numAccomParticipants.ToString(), Convert.ToDouble(txtAccomRate.Text), 0, _TotalHotelAccom, this._Month, this._Year,
                        this.MOOE_INDEX, "Hotel Accommodation - " + general_desc, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
                else if (chkCateringServices.IsChecked == false && chkVenue.IsChecked == false && chkHotelAccom.IsChecked == true)
                {
                    general_desc = "Hotel Accommodation";
                    SelectedMealType = "Hotel Accommodation";
                    c_representation.Process = "SaveData";
                    c_representation.SQLOperation += c_representation_SQLOperation;
                    c_representation.SaveProjectRepresentation(this.ActivityID, this.AccountableID, txtRemark2.Text,
                        SelectedArea, _numAccomParticipants.ToString().ToString(), _total_catering_services, 0, Convert.ToDouble(txtTotal.Value), this._Month, this._Year,
                        this.MOOE_INDEX, SelectedMealType, "", "", "", "", "", _numDays.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
                else
                {
                    general_desc = "";
                }


                //IF TOKEN IS ONLY CHECKED
                if (chkToken.IsChecked == true)
                {
                    general_desc = "Token";
                    SelectedMealType = "Token";
                    c_representation.Process = "SaveData";
                    c_representation.SQLOperation += c_representation_SQLOperation;
                    c_representation.SaveTokenRepresentation(this.ActivityID, this.AccountableID, txtRemark2.Text,
                        SelectedArea, _numTokens.ToString(), _Token_Rate, 0, _TotalToken, this._Month, this._Year,
                        this.MOOE_INDEX, SelectedMealType, "-", "-", "-", "-", "-", "-", _numTokens.ToString(), this.FundSource, isRev, procureChoice, general_desc);
                }
            }

        }
        private void GetAreas()
        {
                c_representation.Process = "GetAreas";
                svc_mindaf.ExecuteSQLAsync(c_representation.GetRepresentationArea());
        }
       private void GetMealServiceType() 
       {
           c_representation.Process = "GetMealServiceType";
           svc_mindaf.ExecuteSQLAsync(c_representation.GetMealServiceType());
       }
       private void GetRepresentationValues()
       {
           c_representation.Process = "GetRepresentationValues";
           svc_mindaf.ExecuteSQLAsync(c_representation.GetRepresentationValues());
       }
       private void GetGridData()
       {
           c_representation.Process = "FetchGridData";
           svc_mindaf.ExecuteSQLAsync(c_representation.FetchLocalData(this.ActivityID,this._Month,this._Year,this.MOOE_INDEX,this.FundSource));
       }

       private void GetDescData()
       {
           //c_representation.Process = "FetchDescData";
           //svc_mindaf.ExecuteSQLAsync(c_representation.FetchDescData(this.ActivityID, this._Year, this.MOOE_INDEX, this.FundSource));
       }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //if (_budget_bal.BalanceOff < _Totals)
            //{
            //    frmNotifyBalance fBa = new frmNotifyBalance();

            //    fBa.Show();
            //}
            //else
            //{
                SaveData();
            //}
        }
        
        private void SetMealCoverageData() 
        {
            cmbAMSnacks.Items.Clear();
            cmbBreakfast.Items.Clear();
            cmbdinner.Items.Clear();
            cmbLunch.Items.Clear();
            cmbpmsnacks.Items.Clear();

            cmbAMSnacks.Items.Add("Yes");
            cmbAMSnacks.Items.Add("No");

            cmbBreakfast.Items.Add("Yes");
            cmbBreakfast.Items.Add("No");

            cmbdinner.Items.Add("Yes");
            cmbdinner.Items.Add("No");

            cmbLunch.Items.Add("Yes");
            cmbLunch.Items.Add("No");

            cmbpmsnacks.Items.Add("Yes");
            cmbpmsnacks.Items.Add("No");

            cmbAMSnacks.SelectedIndex = 1;
            cmbBreakfast.SelectedIndex = 1;
            cmbdinner.SelectedIndex = 1;
            cmbLunch.SelectedIndex = 1;
            cmbpmsnacks.SelectedIndex = 1;

        }

        private void frmbudget_representation_Loaded(object sender, RoutedEventArgs e)
        {
            SetMealCoverageData();
            GetAreas();
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
            txtTokenRate.Text = ".00";

            txtBreakfastAmnt.IsEnabled = false;
            txtAMSnacksAmnt.IsEnabled = false;
            txtLunchAmnt.IsEnabled = false;
            txtPMSnacksAmnt.IsEnabled = false;
            txtDinnerAmnt.IsEnabled = false;
            nudCateringNo.IsEnabled = false;
            txtVenueDailyRate.IsEnabled = false;
            txtAccomRate.IsEnabled = false;
            nudAccomNo.IsEnabled = false;
            txtTokenRate.IsEnabled = false;
            nudNoOfTokens.IsEnabled = false;
        }

        private void cmbArea_DropDownClosed(object sender, EventArgs e)
        {
            var selectedItem = cmbArea.SelectedItem as RepresentationArea;

            if (selectedItem != null)
            {
                List<RepresentationArea> x = ListArea.Where(item => item.Area == selectedItem.Area).ToList();
                if (x.Count != 0)
                {
                    SelectedArea = x[0].Area;
                }
            }
            //UpdateValues();
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
                _numTokens = nudNoOfTokens.Value;
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

            //Token
            _TotalToken = _Token_Rate * _numTokens;

            _Totals = _TotalBreakFast + _TotalSnacksAM + _TotalSnacksPM + _TotalLunch + _TotalDinner + _TotalVenue + _TotalHotelAccom + _TotalToken;
            try
            {
                txtTotal.Value = _Totals;
            }
            catch (Exception)
            {
            }
         
        }

        private void cmbServiceType_DropDownClosed(object sender, EventArgs e)
        {
            var selectedItem = cmbServiceType.SelectedItem as MealServiceType;

            if (selectedItem != null)
            {
                List<MealServiceType> x = ListMealServiceType.Where(item => item.Service_Type == selectedItem.Service_Type).ToList();
                if (x.Count != 0)
                {
                    SelectedMealType = x[0].Service_Type;
                }
            }
            UpdateValues();
        }

        private void cmbBreakfast_DropDownClosed(object sender, EventArgs e)
        {
            switch (cmbBreakfast.SelectedItem.ToString())
            {
                case "Yes":
                    List<RespresentationValuesData> x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Breakfast")).ToList();
                    foreach (var item in x_data)
                    {
                        _Breakfast_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _Breakfast_Rate = 0;
                    break;
                default:
                    break;
            }
            ComputeTotals();
        }

        private void cmbAMSnacks_DropDownClosed(object sender, EventArgs e)
        {
            switch (cmbAMSnacks.SelectedItem.ToString())
            {
                case "Yes":
                    List<RespresentationValuesData> x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Snacks")).ToList();
                    foreach (var item in x_data)
                    {
                        _AMSnacks_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _AMSnacks_Rate = 0;
                    break;
                default:
                    break;
            }
            ComputeTotals();
        }

        private void UpdateValues() 
        {
              List<RespresentationValuesData> x_data  = new List<RespresentationValuesData>();
            switch (cmbBreakfast.SelectedItem.ToString())
            {
                case "Yes":
                    x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Breakfast")).ToList();
                    foreach (var item in x_data)
                    {
                        _Breakfast_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _Breakfast_Rate = 0;
                    break;
                default:
                    break;
            }

            switch (cmbAMSnacks.SelectedItem.ToString())
            {
                case "Yes":
                    x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Snacks")).ToList();
                    foreach (var item in x_data)
                    {
                        _AMSnacks_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _AMSnacks_Rate = 0;
                    break;
                default:
                    break;
            }
            switch (cmbLunch.SelectedItem.ToString())
            {
                case "Yes":
                     x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Lunch")).ToList();
                    foreach (var item in x_data)
                    {
                        _Lunch_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _Lunch_Rate = 0;
                    break;
                default:
                    break;
            }
            switch (cmbpmsnacks.SelectedItem.ToString())
            {
                case "Yes":
                     x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Snacks")).ToList();
                    foreach (var item in x_data)
                    {
                        _PMSnacks_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _PMSnacks_Rate = 0;
                    break;
                default:
                    break;
            }
            switch (cmbdinner.SelectedItem.ToString())
            {
                case "Yes":
                     x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Dinner")).ToList();
                    foreach (var item in x_data)
                    {
                        _Dinner_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _Dinner_Rate = 0;
                    break;
                default:
                    break;
            }
            ComputeTotals();
        }

        private void cmbLunch_DropDownClosed(object sender, EventArgs e)
        {
            switch (cmbLunch.SelectedItem.ToString())
            {
                case "Yes":
                    List<RespresentationValuesData> x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Lunch")).ToList();
                    foreach (var item in x_data)
                    {
                        _Lunch_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _Lunch_Rate = 0;
                    break;
                default:
                    break;
            }
            ComputeTotals();
        }

        private void cmbpmsnacks_DropDownClosed(object sender, EventArgs e)
        {
            switch (cmbpmsnacks.SelectedItem.ToString())
            {
                case "Yes":
                    List<RespresentationValuesData> x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Snacks")).ToList();
                    foreach (var item in x_data)
                    {
                        _PMSnacks_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _PMSnacks_Rate = 0;
                    break;
                default:
                    break;
            }
            ComputeTotals();
        }

        private void cmbdinner_DropDownClosed(object sender, EventArgs e)
        {
            switch (cmbdinner.SelectedItem.ToString())
            {
                case "Yes":
                    List<RespresentationValuesData> x_data = ListRepresentationValues.Where(item => item.item_name.Contains(SelectedArea) && item.library_type.Contains(SelectedMealType) && item.library_type.Contains("Dinner")).ToList();
                    foreach (var item in x_data)
                    {
                        _Dinner_Rate = Convert.ToDouble(item.rate);
                    }
                    break;
                case "No":
                    _Dinner_Rate = 0;
                    break;
                default:
                    break;
            }
            ComputeTotals();
        }

        private void nudQuantity_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void nudDays_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void cmbArea_SelectionChanged(object sender, Infragistics.Controls.Editors.SelectionChangedEventArgs e)
        {
         
        }

        private void cmbServiceType_SelectionChanged(object sender, Infragistics.Controls.Editors.SelectionChangedEventArgs e)
        {
          
        }

        private void frmbudget_representation_Closed(object sender, EventArgs e)
        {
            if (ReloadData!=null)
            {
                ReloadData(this, new EventArgs());
            }
        }


        private void SuspendActivity()
        {
            String _id = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["ActId"].Value.ToString();
            c_representation.Process = "Suspend";
            c_representation.SQLOperation+=c_representation_SQLOperation;
            c_representation.UpdateSuspend(_id, "1");

        }

        private void btnSuspend_Click(object sender, RoutedEventArgs e)
        {
            SuspendActivity();
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

        private void chkToken_Checked(object sender, RoutedEventArgs e)
        {
            //if (chkCateringServices.IsChecked == false)
            //{
            //    MessageBox.Show("Can't add TOKEN if CATERING SERVICES or CATERING SERVICES + VENUE is unchecked. Please tick the CATERING SERVICES checkbox first.");
            //    chkToken.IsChecked = false;
            //}
            //else
            //{
                txtTokenRate.IsEnabled = true;
                nudNoOfTokens.IsEnabled = true;
            //}
        }

        private void chkToken_Unchecked(object sender, RoutedEventArgs e)
        {
            txtTokenRate.IsEnabled = false;
            nudNoOfTokens.IsEnabled = false;
            txtTokenRate.Text = "0.00";
            nudNoOfTokens.Value = 1;
            ComputeTotals();
        }

        private void nudCateringNo_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
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

        private void txtVenueDailyRate_TextChanged(object sender, EventArgs e)
        {
            _Venue_Daily_Rate = Convert.ToDouble(txtVenueDailyRate.Text);
            ComputeTotals();
        }

        private void txtAccomRate_TextChanged(object sender, EventArgs e)
        {
            _Accom_Rate = Convert.ToDouble(txtAccomRate.Text);
            ComputeTotals();
        }

        private void txtTokenRate_TextChanged(object sender, EventArgs e)
        {
            _Token_Rate = Convert.ToDouble(txtTokenRate.Text);
            ComputeTotals();
        }

        private void nudAccomNo_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void nudNoOfTokens_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }

        private void txtAMSnacksAmnt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAMSnacksAmnt.Text == "0.00")
            {
                txtAMSnacksAmnt.Text = "";
            }
            //txtAMSnacksAmnt.Text = "";
        }

        private void txtAMSnacksAmnt_LostFocus(object sender, RoutedEventArgs e)
        {
            //txtAMSnacksAmnt.Text = "0.00";
        }

        private void txtBreakfastAmnt_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtBreakfastAmnt_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtLunchAmnt_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtLunchAmnt_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtDinnerAmnt_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtDinnerAmnt_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtPMSnacksAmnt_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtPMSnacksAmnt_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void numDaysAccom_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            ComputeTotals();
        }
    }

    public class Area_Data
    {
        public String _name { get; set; }
    }
    public class Meal_Type_Data
    {
        public String _name { get; set; }
    }
}

