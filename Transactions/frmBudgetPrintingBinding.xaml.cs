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
    public partial class frmBudgetPrintingBinding : ChildWindow
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
        public Boolean IsRevision { get; set; }
        private Double SelectedRate = 0;
        private String SelectedMealType = "";
        private List<GridData> ListGridData = new List<GridData>();
        private MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private clsBudgetPrintingBinding c_pbind = new clsBudgetPrintingBinding();

        public frmBudgetPrintingBinding()
        {
            InitializeComponent();
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;

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
            _budget_bal = new BudgetRunningBalance(ComputeTotalExpenditure(), "Printing and Binding Expense", MOOE_ID,MOOE_INDEX);
            _budget_bal._DivisionID = DivisionId;
            _budget_bal._Year = this._Year;
            _budget_bal._FundSource = this.FundSource;
            _budget_bal.BalanceOff = 0.00;
            _budget_bal.WorkingYear = this._Year;
            grdBR.Children.Clear();
            grdBR.Children.Add(_budget_bal);
        }
        void svc_mindaf_ExecuteSQLCompleted(object sender, ExecuteSQLCompletedEventArgs e)
        {
            var _results = e.Result.ToString();
            switch (c_pbind.Process)
            {
                case "GetPBType":
                    XDocument oDocKeyResults = XDocument.Parse(_results);
                    var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                     select new PBType
                                     {
                               
                                         item_name = Convert.ToString(info.Element("item_name").Value)

                                     };

          
                    List<ProfData> _ComboList = new List<ProfData>();

                    foreach (var item in _dataLists)
                    {
                        ProfData _varProf = new ProfData();

     
                        _varProf._Name = item.item_name;

                        _ComboList.Add(_varProf);

                    }
                    cmbPBType.ItemsSource = _ComboList;
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
                                                      Quantity = Convert.ToString(info.Element("Quantity").Value)

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
                        _varDetails.Quantity = item.Quantity;
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
                    //grdData.Columns["Service_Type"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Breakfast"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["AM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Lunch"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Dinner"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["PM_Snacks"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["No_Days"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["No_Staff"].Visibility = System.Windows.Visibility.Collapsed;
                    grdData.Columns["Fare_Rate"].HeaderText = "Rate";
                    LoadBudgetBalance();
                    //grdData.Columns["No_Staff"].HeaderText = "No. Professional Service";
                    this.Cursor = Cursors.Arrow;
                    break;
            }
        }
        private void GetPBTypeData()
        {
            c_pbind.Process = "GetPBType";
            svc_mindaf.ExecuteSQLAsync(c_pbind.FetchPrintingBindingRefference());
        }
        private void GetGridData()
        {
            c_pbind.Process = "FetchGridData";
            svc_mindaf.ExecuteSQLAsync(c_pbind.FetchLocalData(this.ActivityID, this._Month, this._Year, this.MOOE_INDEX,this.FundSource));
        }
        private void SaveData() 
        {
            var selectedItem = cmbPBType.SelectedItem as ProfData;

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

            //if (cmbPBType.SelectedItem.ToString() == "Printing (Photocopy)" || cmbPBType.SelectedItem.ToString() == "Printing (Commercial)")
            //{
            //    general_desc = "Printing Services";
            //}
            //else if (cmbPBType.SelectedItem.ToString() == "Print and Bind (Photocopy)" || cmbPBType.SelectedItem.ToString() == "Print and Bind (Commercial)")
            //{
            //    general_desc = "Printing and Binding Services";
            //}
            //else if (cmbPBType.SelectedItem.ToString() == "Binding")
            //{
            //    general_desc = "Binding Services";
            //}
            //else
            //{
            //    general_desc = "";
            //}

            switch (selectedItem._Name)
            {
                case "Printing (Photocopy)": general_desc = "Printing Services";
                    break;
                case "Printing (Commercial)": general_desc = "Printing Services";
                    break;
                case "Print and Bind (Photocopy)": general_desc = "Printing and Binding Services";
                    break;
                case "Print and Bind (Commercial)": general_desc = "Printing and Binding Services";
                    break;
                case "Binding": general_desc = "Binding Services";
                    break;
                default: general_desc = "";
                    break;
            }

            if (is_mult == true)
            {
                foreach (var item in _subExList)
                {
                    c_pbind.Process = "SaveData";
                    c_pbind.SQLOperation += c_pbind_SQLOperation;
                    c_pbind.SaveProjectPrintingBinding(item.activity_id.ToString(), this.AccountableID, txtRemark.Text, nudPieces.Value.ToString(), Convert.ToDouble(txtCostPerPiece.Value), Convert.ToDouble(txtTotal.Value), item.month.ToString(), _Year, MOOE_INDEX, "", selectedItem._Name, this.FundSource, isRev, procureChoice, general_desc);
                }
                GetGridData();
            }
            else
            {
                c_pbind.Process = "SaveData";
                c_pbind.SQLOperation += c_pbind_SQLOperation;
                c_pbind.SaveProjectPrintingBinding(this.ActivityID, this.AccountableID, txtRemark.Text, nudPieces.Value.ToString(), Convert.ToDouble(txtCostPerPiece.Value), Convert.ToDouble(txtTotal.Value), _Month, _Year, MOOE_INDEX, "", selectedItem._Name, this.FundSource, isRev, procureChoice, general_desc);
            }
            

        }

        void c_pbind_SQLOperation(object sender, EventArgs e)
        {
            switch (c_pbind.Process)
            {
                case "SaveData":
                    GetGridData();
                    break;
                case "Suspend":
                    GetGridData();
                    break;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                
              MessageBox.Show(ex.Message.ToString());
            }

           
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

        private void frm_b_printbind_Closed(object sender, EventArgs e)
        {
            if (ReloadData!=null)
            {
                ReloadData(this, new EventArgs());
            }
        }

        private void frm_b_printbind_Loaded(object sender, RoutedEventArgs e)
        {
            GetPBTypeData();
        }

        private void cmbPBType_DropDownClosed(object sender, EventArgs e)
        {

        }
        private void ComputeTotals()
        {
            Double _numPieces = 0;

            Double _rate = 0;
            try
            {
                _rate = Convert.ToDouble(txtCostPerPiece.Value);
            }
            catch (Exception)
            {


            }

            try
            {
                _numPieces = nudPieces.Value;
            }
            catch (Exception)
            {
            }

           

            Double Totals =_rate* _numPieces;
            try
            {
                txtTotal.Value = Totals;
            }
            catch (Exception)
            {
            }

        }
        private void SuspendActivity()
        {
            String _id = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["ActId"].Value.ToString();
            c_pbind.Process = "Suspend";
            c_pbind.SQLOperation+=c_pbind_SQLOperation;
            c_pbind.UpdateSuspend(_id, "1");

        }
        private void btnSuspend_Click(object sender, RoutedEventArgs e)
        {
            SuspendActivity();
        }
        
        private void nudPieces_ValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            try
            {
                ComputeTotals();
            }
            catch (Exception)
            {

            }
           
        }

        private void txtCostPerPiece_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                ComputeTotals();
            }
            catch (Exception)
            {
                
                
            }
        }
    }
}

