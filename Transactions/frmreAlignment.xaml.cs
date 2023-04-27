using MinDAF.Class;
using MinDAF.MinDAFS;
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
    public partial class frmreAlignment : ChildWindow
    {
        public String DivisionId { get; set; }
        public String DivisionPaP { get; set; }
       
        public event EventHandler ReloadData;
        private frmListExpenditures f_expenditures = new frmListExpenditures();

        private List<rel_ProcuredItems> ListBudgetProcured = new List<rel_ProcuredItems>();
        private List<rel_MOOELIST> ListMOEEList = new List<rel_MOOELIST>();
        private MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private List<rel_FundSource> ListFundSource = new List<rel_FundSource>();
        private List<rel_Activity> ListActivity = new List<rel_Activity>();
        private List<ReportTypeService> ListTypeService = new List<ReportTypeService>();
        private List<rel_ForAlignment> ListForAlignment = new List<rel_ForAlignment>();
        private clsreAlignment c_realign = new clsreAlignment();

        private String SelectedYear = "";
        private String SelectedFundSource = "";
        public frmreAlignment()
        {
            InitializeComponent();
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;
        }

        void svc_mindaf_ExecuteSQLCompleted(object sender, ExecuteSQLCompletedEventArgs e)
        {
               var _results = e.Result.ToString();
               switch (c_realign.Process)
               {
                   case "FetchProcured":
                       XDocument oDocKeyResults = XDocument.Parse(_results);
                       var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                        select new rel_ProcuredItems
                                        {
                                            activity_id = Convert.ToString(info.Element("activity_id").Value),
                                            uacs_code = Convert.ToString(info.Element("uacs_code").Value),
                                            name = Convert.ToString(info.Element("name").Value),
                                            total = Convert.ToString(info.Element("total").Value),
                                            fiscal_year = Convert.ToString(info.Element("fiscal_year").Value),
                                            User_Fullname = Convert.ToString(info.Element("User_Fullname").Value)
                                        };


                    ListBudgetProcured.Clear();

               

                       foreach (var item in _dataLists)
                       {
                           rel_ProcuredItems _varProf = new rel_ProcuredItems();

                             _varProf.User_Fullname = item.User_Fullname;
                           _varProf.fiscal_year = item.fiscal_year;
                           _varProf.activity_id = item.activity_id;
                           _varProf.uacs_code = item.uacs_code;
                           _varProf.name = item.name;
                           _varProf.total = item.total;

                           ListBudgetProcured.Add(_varProf);

                       }
                       List<rel_ProcuredItems> FinalData = new List<rel_ProcuredItems>();

                       foreach (var item in ListMOEEList)
                       {
                           rel_ProcuredItems _varProf = new rel_ProcuredItems();
                           double _total = 0.00;

                           List<rel_ProcuredItems> x_data = _dataLists.Where(items => items.uacs_code == item.uacs_code).ToList();
                           if (x_data.Count!=0)
                           {
                               _varProf.name = item.name;
                               _varProf.uacs_code = item.uacs_code;
                               foreach (var itemdata in x_data)
                               {
                                   _total += Convert.ToDouble(itemdata.total);
                               }
                               _varProf.total = _total.ToString();
                               FinalData.Add(_varProf);
                           }
                          
                       }

                       foreach (var item in FinalData)
                       {
                           List<ReportDataMBA> x_data = ListReportData.Where(items => items.uacs == item.uacs_code).ToList();
                           double _total = Convert.ToDouble(item.total);

                           foreach (var item_data in x_data)
                           {
                               _total -= Convert.ToDouble(item_data.total);


                           }
                           item.total = _total.ToString();
                       }
                       ListForAlignment.Clear();
                       List<rel_ForAlignment> x_align = new List<rel_ForAlignment>();
                       foreach (var item in ListReportData)
                       {
                         
                           List<rel_ProcuredItems> x_filter = FinalData.Where(item_data => item_data.uacs_code == item.uacs).ToList();
                           if (x_filter.Count==0)
                           {
                               rel_ForAlignment x_data = new rel_ForAlignment();
                               x_data.Name = item.name;
                               x_data.Uacs = item.uacs;
                               x_data.Total = item.total;
                               x_data.Months = item.month;
                               x_align.Add(x_data);
                           }
                       }
                       var results = from p in x_align
                                     group p by p.Uacs into g
                                     select new
                                     {
                                         Id = g.Key,
                                         ExpenseName = g.Select(m => m.Name),
                                         UACS = g.Select(m => m.Uacs),
                                         Months = g.Select(m => m.Months)
                                     };
                       foreach (var item in results)
                       {
                           rel_ForAlignment x_final = new rel_ForAlignment();
                           List<rel_ForAlignment> x_data = x_align.Where(items => items.Uacs == item.Id).ToList();
                           foreach (var itemname in item.ExpenseName)
                           {
                               x_final.Name = itemname.ToString();
                               break;
                           }
                           foreach (var itemuacs in item.UACS)
	                        {
                                x_final.Uacs = itemuacs.ToString();
		                            break;
	                        }
                           foreach (var itemmonths in item.Months)
                           {
                               x_final.Months = itemmonths.ToString();
                               break;
                           }

                           double _total = 0.00;
                           foreach (var itemresult in x_data)
                           {
                               _total += Convert.ToDouble(itemresult.Total);
                           }
                           x_final.Total = _total.ToString();
                           ListForAlignment.Add(x_final);
                       }
                       cmbData.Items.Clear();
                       foreach (var item in ListMOEEList)
                       {
                           cmbData.Items.Add(item.name);
                       }
                       foreach (var item in FinalData)
                       {
                           List<rel_AlignmentData> x_data = ListAlignmentData.Where(items => items.from_uacs == item.uacs_code).ToList();
                           double _total = 0.00;

                           foreach (var itemdata in x_data)
                           {
                               _total += Convert.ToDouble(itemdata.total_alignment);
                           }
                           item.total = (Convert.ToDouble(item.total) - _total).ToString();

                       }
                   
                       grdData.ItemsSource = null;
                       grdData.ItemsSource = FinalData;
                       grdData.Columns["activity_id"].Visibility = System.Windows.Visibility.Collapsed;
                       grdData.Columns["uacs_code"].Visibility = System.Windows.Visibility.Collapsed;
                       grdData.Columns["total"].Visibility = System.Windows.Visibility.Collapsed;
                       grdData.Columns["User_Fullname"].Visibility = System.Windows.Visibility.Collapsed;
                       grdData.Columns["fiscal_year"].Visibility = System.Windows.Visibility.Collapsed;


                       FetchActivity();
                       break;
                   case "FetchMOELists":
                       XDocument oDocKeyResultsFetchMOELists = XDocument.Parse(_results);
                       var _dataListsFetchMOELists = from info in oDocKeyResultsFetchMOELists.Descendants("Table")
                                        select new rel_MOOELIST
                                        {
                                            id = Convert.ToString(info.Element("id").Value),
                                            is_active = Convert.ToString(info.Element("is_active").Value),
                                            is_dynamic = Convert.ToString(info.Element("is_dynamic").Value),
                                            mooe_id = Convert.ToString(info.Element("mooe_id").Value),
                                            name = Convert.ToString(info.Element("name").Value),
                                            uacs_code = Convert.ToString(info.Element("uacs_code").Value)
                                            
                                        };


                       ListMOEEList.Clear();



                       foreach (var item in _dataListsFetchMOELists)
                       {
                           rel_MOOELIST _varProf = new rel_MOOELIST();

                           _varProf.id = item.id;
                           _varProf.is_active = item.is_active;
                           _varProf.is_dynamic = item.is_dynamic;
                           _varProf.mooe_id = item.mooe_id;
                           _varProf.name = item.name;
                           _varProf.uacs_code = item.uacs_code;

                           ListMOEEList.Add(_varProf);

                       }
                      
                       FetchProcured();
                       break;
                   case "FetchFundSource":
                       XDocument oDocKeyResultsFetchFundSource = XDocument.Parse(_results);
                       var _dataListsFetchFundSource = from info in oDocKeyResultsFetchFundSource.Descendants("Table")
                                                     select new rel_FundSource
                                                     {
                                                         Fund_Name = Convert.ToString(info.Element("Fund_Name").Value),
                                                        Fund_Source_Id = Convert.ToString(info.Element("Fund_Source_Id").Value),

                                                     };


                       ListFundSource.Clear();
                       cmbFundSource.Items.Clear();


                       foreach (var item in _dataListsFetchFundSource)
                       {
                           rel_FundSource _varProf = new rel_FundSource();

                           _varProf.Fund_Name= item.Fund_Name;
                           _varProf.Fund_Source_Id = item.Fund_Source_Id;
                           ListFundSource.Add(_varProf);
                           cmbFundSource.Items.Add(item.Fund_Name);
                       }
                       FetchActivity();
                       break;
                   case "FetchActivity":
                       XDocument oDocKeyResultsFetchActivity = XDocument.Parse(_results);
                       var _dataListsFetchActivity = from info in oDocKeyResultsFetchActivity.Descendants("Table")
                                                       select new rel_Activity
                                                       {
                                                           Id = Convert.ToString(info.Element("id").Value),
                                                           Name = Convert.ToString(info.Element("description").Value),
                                                           User = Convert.ToString(info.Element("User_Fullname").Value)

                                                       };


                       ListActivity.Clear();
                       cmbActivity.Items.Clear();


                       foreach (var item in _dataListsFetchActivity)
                       {
                           rel_Activity _varProf = new rel_Activity();

                           _varProf.Id = item.Id;
                           _varProf.Name = item.Name;
                           _varProf.User = item.User;
                           ListActivity.Add(_varProf);
                           cmbActivity.Items.Add(item.Name);
                       }
                 
                       break;
                   case "FetchType":
                       XDocument oDocKeyResultsFetchFundType = XDocument.Parse(_results);
                       var _dataListsFetchFundType = from info in oDocKeyResultsFetchFundType.Descendants("Table")
                                                     select new ReportTypeService
                                                     {
                                                         code  ="",
                                                         name = Convert.ToString(info.Element("name").Value),
                                                         uacs = Convert.ToString(info.Element("uacs").Value)

                                                     };


                       ListTypeService.Clear();
                       cmbFundSource.Items.Clear();


                       foreach (var item in _dataListsFetchFundType)
                       {
                           ReportTypeService _varProf = new ReportTypeService();

                           _varProf.code = item.uacs.Substring(0,3);
                           _varProf.name = item.name;
                           _varProf.uacs = item.uacs;
                           ListTypeService.Add(_varProf);
                           cmbFundSource.Items.Add(item.name);
                       }

                       break;
                  case "FetchSelected":
                    XDocument oDocKeyResultsFetchSelected = XDocument.Parse(_results);
                    var _dataListsFetchSelected = from info in oDocKeyResultsFetchSelected.Descendants("Table")
                                                  select new rel_AlignmentData
                                              {
                                                  id = Convert.ToString(info.Element("id").Value),
                                                  division_pap = Convert.ToString(info.Element("division_pap").Value),
                                                  division_year = Convert.ToString(info.Element("division_year").Value),
                                                  from_total = Convert.ToString(info.Element("from_total").Value),
                                                  from_uacs = Convert.ToString(info.Element("from_uacs").Value),
                                                  is_approved = Convert.ToString(info.Element("is_approved").Value),
                                                  to_uacs = Convert.ToString(info.Element("to_uacs").Value),
                                                  total_alignment = Convert.ToString(info.Element("total_alignment").Value),
                                                  name_from = Convert.ToString(info.Element("name_from").Value),
                                                  name_to = Convert.ToString(info.Element("name_to").Value),


                                              };


                    ListAlignmentData.Clear();

                    foreach (var item in _dataListsFetchSelected)
                    {
                        rel_AlignmentData _varProf = new rel_AlignmentData();

                        _varProf.id = item.id;
                        _varProf.name_to = item.name_to;
                        _varProf.name_from = item.name_from;
                        _varProf.division_pap = item.division_pap;
                        _varProf.division_year = item.division_year;
                        _varProf.from_total = Convert.ToDouble(item.from_total).ToString("#,##0.00");
                         _varProf.from_uacs = item.from_uacs;
                         _varProf.is_approved = item.is_approved;
                         _varProf.to_uacs = item.to_uacs;
                         _varProf.total_alignment = Convert.ToDouble(item.total_alignment).ToString("#,##0.00");
                   

                        ListAlignmentData.Add(_varProf);

                    }
                    FetchFundSource();
                    this.Cursor = Cursors.Arrow;
                   
                    //  FetchAllocation();
                    break;
               }
        }
        private void RemoveAlignment()         
        {
            String x_id = grdAlignment.Rows[grdAlignment.ActiveCell.Row.Index].Cells["id"].Value.ToString();
            c_realign.Process = "RemoveAlignment";
            c_realign.RemoveAlignment(x_id);
            c_realign.SQLOperation+=c_realign_SQLOperation;
        }

        private void LoadData() 
        {
            List<rel_AlignmentData> _xdata = ListAlignmentData.Where(items => items.from_uacs == grdData.Rows[grdData.ActiveCell.Row.Index].Cells["uacs_code"].Value.ToString()).ToList();

            grdAlignment.ItemsSource = null;
            grdAlignment.ItemsSource = _xdata;
            grdAlignment.Columns["from_uacs"].HeaderText = "UACS";
            grdAlignment.Columns["to_uacs"].HeaderText = "UACS";
            grdAlignment.Columns["name_to"].HeaderText = "Expenditure From";
            grdAlignment.Columns["name_from"].HeaderText = "Expenditure To";
            grdAlignment.Columns["total_alignment"].HeaderText = "Total Alignment";
            grdAlignment.Columns["from_total"].HeaderText = "Original Amount";

            grdAlignment.Columns["id"].Visibility = System.Windows.Visibility.Collapsed;
            grdAlignment.Columns["division_pap"].Visibility = System.Windows.Visibility.Collapsed;
            grdAlignment.Columns["division_year"].Visibility = System.Windows.Visibility.Collapsed;
            grdAlignment.Columns["is_approved"].Visibility = System.Windows.Visibility.Collapsed;
        }
        private void SaveAlignment() 
        {
           
           String from_uacs = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["uacs_code"].Value.ToString();
           String from_total = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["total"].Value.ToString();
           String to_uacs = ListMOEEList.Where(items => items.name == cmbData.SelectedItem.ToString()).ToList()[0].uacs_code.ToString();
            String total_alignment= txtRelAmount.Value.ToString();
            String division_pap = DivisionId;
            String division_year = cmbYear.SelectedItem.ToString();
            String months = cmbMonths.SelectedItem.ToString();
            String f_source = ListFundSource.Where(items => items.Fund_Name == cmbFundSource.SelectedItem.ToString()).ToList()[0].Fund_Source_Id;
            c_realign.Process = "SaveAlignment";
            c_realign.SaveAlignment(from_uacs, from_total, to_uacs, total_alignment, division_pap, division_year, months, f_source);
            c_realign.SQLOperation += c_realign_SQLOperation;
        }

        private void ClearData() 
        {
            grdData.ItemsSource = null;
            grdAlignment.ItemsSource = null;
            cmbActivity.Items.Clear();
            cmbData.Items.Clear();
            txtProcAmount.Value = 0.00;
            txtRelAmount.Value = 0.00;
            cmbFundSource.Focus();
        }

        void c_realign_SQLOperation(object sender, EventArgs e)
        {
            switch (c_realign.Process)
            {
                case "SaveAlignment":
                    FetchSelected();
                    break;
                case "RemoveAlignment":
                    FetchSelected();
                    ClearData();
                    break;
            }
        }
        private void FetchProcured() 
        {
            string FundId = "";
            c_realign.Process = "FetchProcured";
            List<rel_FundSource> x_source = ListFundSource.Where(items => items.Fund_Name == cmbFundSource.SelectedItem.ToString()).ToList();
            if (x_source.Count!=0)
            {
                FundId = x_source[0].Fund_Source_Id;
                SelectedFundSource = FundId;
            }
            svc_mindaf.ExecuteSQLAsync(c_realign.FetchProcuredBudget(this.DivisionId,FundId));
        }
        private void FetchMOELists()
        {
            c_realign.Process = "FetchMOELists";
            svc_mindaf.ExecuteSQLAsync(c_realign.FetchMooeList());
        }
        public String PaP { get; set; }
     
        private void FetchDataSummary()
        {
            try
            {
                c_realign.Process = "FetchData";


                svc_mindaf.ExecuteImportDataSQLAsync(c_realign.FetchDataMBASummary(this.cmbYear.SelectedItem.ToString(), this.PaP, cmbFundSource.SelectedItem.ToString()));
                svc_mindaf.ExecuteImportDataSQLCompleted += svc_mindaf_ExecuteImportDataSQLCompleted;
            }
            catch (Exception)
            {

            }
        
            //  svc_mindaf.ExecuteSQLAsync(c_rep_mba.FetchDataDB(this.SelectedYear, this.DivisionPAP,this.FundSource));
        }
        private List<ReportDataMBA> ListReportData = new List<ReportDataMBA>();
        private List<rel_AlignmentData> ListAlignmentData = new List<rel_AlignmentData>();
        void svc_mindaf_ExecuteImportDataSQLCompleted(object sender, ExecuteImportDataSQLCompletedEventArgs e)
        {
            var _results = e.Result.ToString();
            switch (c_realign.Process)
            {
                case "FetchData":
                    XDocument oDocKeyResultsFetchData = XDocument.Parse(_results);
                    var _dataListsFetchData = from info in oDocKeyResultsFetchData.Descendants("Table")
                                              select new ReportDataMBA
                                              {
                                                  type_service = "",
                                                  month = Convert.ToString(info.Element("_month").Value),
                                                  total = Convert.ToString(info.Element("Total").Value),
                                                  name = Convert.ToString(info.Element("name").Value),
                                                  fund_source = Convert.ToString(info.Element("FundSource").Value),
                                                  uacs = Convert.ToString(info.Element("uacs").Value)
                                              };


                    ListReportData.Clear();

                    foreach (var item in _dataListsFetchData)
                    {
                        ReportDataMBA _varProf = new ReportDataMBA();

                        _varProf.id = item.id;

                        switch (item.month)
                        {
                            case "1":
                                _varProf.month = "Jan";
                                break;
                            case "2":
                                _varProf.month = "Feb";
                                break;
                            case "3":
                                _varProf.month = "Mar";
                                break;
                            case "4":
                                _varProf.month = "Apr";
                                break;
                            case "5":
                                _varProf.month = "May";
                                break;
                            case "6":
                                _varProf.month = "Jun";
                                break;
                            case "7":
                                _varProf.month = "Jul";
                                break;
                            case "8":
                                _varProf.month = "Aug";
                                break;
                            case "9":
                                _varProf.month = "Sep";
                                break;
                            case "10":
                                _varProf.month = "Oct";
                                break;
                            case "11":
                                _varProf.month = "Nov";
                                break;
                            case "12":
                                _varProf.month = "Dec";
                                break;
                        }
                        _varProf.total = item.total;
                        _varProf.year = item.year;
                        _varProf.name = item.name;
                        _varProf.fund_source = item.fund_source;
                        _varProf.mooe = item.mooe;
                        _varProf.uacs = item.uacs;
                        if (_varProf.uacs.Substring(0,3)=="502")
                        {
                            ListReportData.Add(_varProf);
                        }
                   

                    }
                    FetchMOELists();
                    this.Cursor = Cursors.Arrow;
                  //  FetchAllocation();
                    break;
             
            }

        }
        private void FetchFundSource()
        {
            c_realign.Process = "FetchFundSource";
            svc_mindaf.ExecuteSQLAsync(c_realign.FetchFundSource(this.DivisionId, cmbYear.SelectedItem.ToString()));
        }

        private void FetchType()
        {
            c_realign.Process = "FetchType";
            svc_mindaf.ExecuteSQLAsync(c_realign.FetchFundType());
        }
        private void FetchActivity()
        {
            c_realign.Process = "FetchActivity";
            svc_mindaf.ExecuteSQLAsync(c_realign.FetchActivities(this.DivisionId));
        }
        private void FetchSelected()
        {
            c_realign.Process = "FetchSelected";
            svc_mindaf.ExecuteSQLAsync(c_realign.FetchRealigned(this.DivisionId, cmbYear.SelectedItem.ToString()));
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SaveAlignment();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReloadData!=null)
            {
                ReloadData(this, new EventArgs());
            }
            this.DialogResult = false;
        }

        private void frmReAlignment_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateMonths();
            GenerateYear();
            FetchSelected();
         
          //  FetchMOELists();
        }
        private void GenerateYear()
        {
            int _year = DateTime.Now.Year;
            int _limit = _year + 20;
            _year -= 10;
            for (int i = _year; i != _limit; i++)
            {
                cmbYear.Items.Add(_year);
                _year += 1;
            }
            cmbYear.SelectedIndex = 11;
            this.SelectedYear = cmbYear.SelectedItem.ToString();
        }
        private void grdData_CellClicked(object sender, Infragistics.Controls.Grids.CellClickedEventArgs e)
        {
            try
            {
                txtProcAmount.Value = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["total"].Value;
                LoadData();
            }
            catch (Exception)
            {
  
            }
        }

        private void GenerateMonths()         
        {
            cmbMonths.Items.Clear();
            cmbMonths.Items.Add("Jan");
            cmbMonths.Items.Add("Feb");
            cmbMonths.Items.Add("Mar");
            cmbMonths.Items.Add("Apr");
            cmbMonths.Items.Add("Jun");
            cmbMonths.Items.Add("Jul");
            cmbMonths.Items.Add("Aug");
            cmbMonths.Items.Add("Sep");
            cmbMonths.Items.Add("Oct");
            cmbMonths.Items.Add("Nov");
            cmbMonths.Items.Add("Dec");
        }

        private void btnAlignment_Click(object sender, RoutedEventArgs e)
        {

            f_expenditures = new frmListExpenditures();
            f_expenditures._Month = cmbMonths.SelectedItem.ToString();
            f_expenditures.DivisionId = this.DivisionId;
            f_expenditures.FundName = cmbFundSource.SelectedItem.ToString();
            f_expenditures.SelectedExpenditure += f_expenditures_SelectedExpenditure;
            f_expenditures.Show();
        }
        private frmBudgetCreation frm_local_travel;
        void f_expenditures_SelectedExpenditure(object sender, EventArgs e)
        {
            String _act_id = ListActivity.Where(items => items.Name == cmbActivity.SelectedItem.ToString()).ToList()[0].Id.ToString();
            String _act_user = ListActivity.Where(items => items.Name == cmbActivity.SelectedItem.ToString()).ToList()[0].User.ToString();
            String ActivityId = "";
            String Assigned = "";
            switch (f_expenditures.SelectedCode)
            {
                case "Local Travel":
                    frm_local_travel = new frmBudgetCreation();
                    ActivityId =_act_id;//grdData.Rows[grdData.ActiveCell.Row.Index].Cells["activity_id"].Value.ToString();
                    Assigned =_act_user; //grdData.Rows[grdData.ActiveCell.Row.Index].Cells["User_Fullname"].Value.ToString();
                    string _Month = f_expenditures._Month;
                    string _Year = cmbYear.SelectedItem.ToString();

                    frm_local_travel.ActivityID = ActivityId;
                    frm_local_travel.AccountableID = Assigned;
                    frm_local_travel._Month = _Month;
                    frm_local_travel._Year = _Year;
                    frm_local_travel.MOOE_ID = f_expenditures.MOOE_ID;
                  
                    frm_local_travel.TravelType = f_expenditures.SelectedCode;
                    frm_local_travel.FundSource = f_expenditures.FundSourceId;
                    frm_local_travel.DivisionId = this.DivisionId;
                    frm_local_travel.IsRealignment = true;
                    frm_local_travel.txtTotal.Value = txtRelAmount.Value;
                    frm_local_travel.txtAllowanceRate.Value = 0;
                    frm_local_travel.txtPlaneFareRate.Value = 0;
        
                    frm_local_travel.CloseRealignment += frm_local_travel_CloseRealignment;
                    frm_local_travel.Show();
                    break;
                case "International Travel":
                    frmBudgetCreation frm_foreign_travel = new frmBudgetCreation();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_foreign_travel.ActivityID = ActivityId;
                    frm_foreign_travel.AccountableID = Assigned;
                    frm_foreign_travel._Month = _Month;
                    frm_foreign_travel._Year = _Year;
                    frm_foreign_travel.MOOE_ID = f_expenditures.MOOE_ID;
                 //   frm_foreign_travel.ReloadData += frm_foreign_travel_ReloadData;
                    frm_foreign_travel.TravelType = f_expenditures.SelectedCode;
                    frm_foreign_travel.FundSource = f_expenditures.FundSourceId;
                    frm_foreign_travel.DivisionId = this.DivisionId;

                    frm_foreign_travel.IsRealignment = true;
                    frm_foreign_travel.txtTotal.Value = txtRelAmount.Value;
                    frm_foreign_travel.txtAllowanceRate.Value = 0;
                    frm_foreign_travel.txtPlaneFareRate.Value = 0;
                    frm_foreign_travel.Show();
                    break;
                case "Gasoline, Oil and Lubricants Expenses":
                    frmBudgetGasoline frm_gasoline = new frmBudgetGasoline();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_gasoline.ActivityID = ActivityId;
                    frm_gasoline.AccountableID = Assigned;
                    frm_gasoline._Month = _Month;
                    frm_gasoline._Year = _Year;
                    frm_gasoline.MOOE_ID = f_expenditures.MOOE_ID;
                  //  frm_gasoline.ReloadData += frm_gasoline_ReloadData;
                    frm_gasoline.TravelType = f_expenditures.SelectedCode;
                    frm_gasoline.FundSource = f_expenditures.FundSourceId;
                    frm_gasoline.DivisionId = this.DivisionId;
                    frm_gasoline.Show();
                    break;
                case "Supplies":
                    frmBudgetSupplies frm_budget_supplies = new frmBudgetSupplies();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_budget_supplies.ActivityID = ActivityId;
                    frm_budget_supplies.AccountableID = Assigned;
                    frm_budget_supplies._Month = _Month;
                    frm_budget_supplies._Year = _Year;
                    frm_budget_supplies.MOOE_ID = f_expenditures.MOOE_ID;
                  //  frm_budget_supplies.ReloadData += frm_budget_supplies_ReloadData;
                    frm_budget_supplies.FundSource = f_expenditures.FundSourceId;
                    frm_budget_supplies.DivisionId = this.DivisionId;
                    frm_budget_supplies.Show();
                    break;
                case "Representation":
                    frmBudgetRepresentation frm_budget_representation = new frmBudgetRepresentation();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_budget_representation.ActivityID = ActivityId;
                    frm_budget_representation.AccountableID = Assigned;
                    frm_budget_representation._Month = _Month;
                    frm_budget_representation._Year = _Year;
                    frm_budget_representation.MOOE_ID = f_expenditures.MOOE_ID;
                //    frm_budget_representation.ReloadData += frm_budget_representation_ReloadData;
                    frm_budget_representation.FundSource = f_expenditures.FundSourceId;
                    frm_budget_representation.DivisionId = this.DivisionId;
                    frm_budget_representation.Show();
                    break;
                case "Professional Fee":
                    frmBudgetProfessionalFees frm_prof = new frmBudgetProfessionalFees();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_prof.ActivityID = ActivityId;
                    frm_prof.AccountableID = Assigned;
                    frm_prof._Month = _Month;
                    frm_prof._Year = _Year;
                    frm_prof.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_prof.FundSource = f_expenditures.FundSourceId;
                    frm_prof.DivisionId = this.DivisionId;
                 //   frm_prof.ReloadData += frm_prof_ReloadData;

                    frm_prof.Show();
                    break;
                case "Printing and Binding":
                    frmBudgetPrintingBinding frm_printbind = new frmBudgetPrintingBinding();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_printbind.ActivityID = ActivityId;
                    frm_printbind.AccountableID = Assigned;
                    frm_printbind._Month = _Month;
                    frm_printbind._Year = _Year;
                    frm_printbind.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_printbind.FundSource = f_expenditures.FundSourceId;
                    frm_printbind.DivisionId = this.DivisionId;
                 //   frm_printbind.ReloadData += frm_printbind_ReloadData;

                    frm_printbind.Show();
                    break;
                case "Dues":
                    frmBudgetDues frm_due = new frmBudgetDues();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_due.ActivityID = ActivityId;
                    frm_due.AccountableID = Assigned;
                    frm_due._Month = _Month;
                    frm_due._Year = _Year;
                    frm_due.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_due.FundSource = f_expenditures.FundSourceId;
                 //   frm_due.ReloadData += frm_due_ReloadData;
                    frm_due.DivisionId = this.DivisionId;
                    frm_due.Show();
                    break;
                case "Subscription":
                    frmBudgetSubscription frm_subs = new frmBudgetSubscription();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_subs.ActivityID = ActivityId;
                    frm_subs.AccountableID = Assigned;
                    frm_subs._Month = _Month;
                    frm_subs._Year = _Year;
                    frm_subs.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_subs.DivisionId = this.DivisionId;
                    frm_subs.FundSource = f_expenditures.FundSourceId;
                //    frm_subs.ReloadData += frm_subs_ReloadData;

                    frm_subs.Show();
                    break;
                case "Advertising":
                    frmBudgetAdvertising frm_ads = new frmBudgetAdvertising();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_ads.ActivityID = ActivityId;
                    frm_ads.AccountableID = Assigned;
                    frm_ads._Month = _Month;
                    frm_ads._Year = _Year;
                    frm_ads.DivisionId = this.DivisionId;
                    frm_ads.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_ads.FundSource = f_expenditures.FundSourceId;
                //    frm_ads.ReloadData += frm_ads_ReloadData;

                    frm_ads.Show();
                    break;
                case "Training Expenses":
                    frmBudgetTraining frm_bud = new frmBudgetTraining();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_bud.ActivityID = ActivityId;
                    frm_bud.AccountableID = Assigned;
                    frm_bud._Month = _Month;
                    frm_bud._Year = _Year;
                    frm_bud.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_bud.FundSource = f_expenditures.FundSourceId;
                    frm_bud.DivisionId = this.DivisionId;
                    frm_bud._Title = f_expenditures.SelectedCode;
                //    frm_bud.ReloadData += frm_ads_ReloadData;

                    frm_bud.Show();
                    break;
                case "ICT Training Expenses":
                    frmBudgetTraining frm_bud2 = new frmBudgetTraining();
                    ActivityId = _act_id;
                    Assigned = _act_user;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_bud2.ActivityID = ActivityId;
                    frm_bud2.AccountableID = Assigned;
                    frm_bud2._Month = _Month;
                    frm_bud2._Year = _Year;
                    frm_bud2.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_bud2.FundSource = f_expenditures.FundSourceId;
                    frm_bud2.DivisionId = this.DivisionId;
                    //    frm_bud.ReloadData += frm_ads_ReloadData;

                    frm_bud2.Show();
                    break;
                case "Rental Fee":
                    frmBudgetRental frm_rent = new frmBudgetRental();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_rent.ActivityID = ActivityId;
                    frm_rent.AccountableID = Assigned;
                    frm_rent._Month = _Month;
                    frm_rent._Year = _Year;
                    frm_rent.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_rent.FundSource = f_expenditures.FundSourceId;
                    frm_rent.DivisionId = this.DivisionId;
                //    frm_rent.ReloadData += frm_rent_ReloadData;

                    frm_rent.Show();
                    break;
                case "Labor and Wages":
                    frmBudgetLaborAndWages frm_law = new frmBudgetLaborAndWages();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_law.ActivityID = ActivityId;
                    frm_law.AccountableID = Assigned;
                    frm_law._Month = _Month;
                    frm_law._Year = _Year;
                    frm_law.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_law.DivisionId = this.DivisionId;
                    frm_law.FundSource = f_expenditures.FundSourceId;
                 //   frm_law.ReloadData += frm_law_ReloadData;

                    frm_law.Show();
                    break;
                case "Other MOOE":
                    frmBudgetOtherMooe frm_other = new frmBudgetOtherMooe();
                    ActivityId =_act_id;
                    Assigned =_act_user ;
                    _Month = f_expenditures._Month;
                    _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                    frm_other.ActivityID = ActivityId;
                    frm_other.AccountableID = Assigned;
                    frm_other._Month = _Month;
                    frm_other._Year = _Year;
                    frm_other.MOOE_ID = f_expenditures.MOOE_ID;
                    frm_other.FundSource = f_expenditures.FundSourceId;
                    frm_other.DivisionId = this.DivisionId;
                 //   frm_other.ReloadData += frm_other_ReloadData;

                    frm_other.Show();
                    break;
                default:
                    if (f_expenditures.IsDynamic)
                    {
                        frmGenericExpenditureCharging f_dynamic = new frmGenericExpenditureCharging();
                        ActivityId =_act_id;
                        Assigned =_act_user ;
                        _Month = f_expenditures._Month;
                        _Year = grdData.Rows[grdData.ActiveCell.Row.Index].Cells["fiscal_year"].Value.ToString();

                        f_dynamic.ActivityID = ActivityId;
                        f_dynamic.AccountableID = Assigned;
                        f_dynamic._Month = _Month;
                        f_dynamic._Year = _Year;
                        f_dynamic.MOOE_ID = f_expenditures.MOOE_ID;
                        f_dynamic.FundSource = f_expenditures.FundSourceId;
                        f_dynamic.DivisionId = this.DivisionId;
                     //   f_dynamic.ReloadData += f_dynamic_ReloadData;

                        f_dynamic.Show();
                    }
                    break;
            }
        }

        void frm_local_travel_CloseRealignment(object sender, EventArgs e)
        {
            try
            {
                txtRelAmount.Value = frm_local_travel.AmountRealigned;
            }
            catch (Exception)
            {

                txtRelAmount.Value = 0; ;
            }
         
        }

        private void txtRelAmount_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Double _val1 = Convert.ToDouble(grdData.Rows[grdData.ActiveCell.Row.Index].Cells["total"].Value);
                Double _val2 = Convert.ToDouble(txtRelAmount.Value);
                Double Total = _val1 - _val2;
                txtProcAmount.Value = Total;
            }
            catch (Exception)
            {

                txtProcAmount.Value = 0;
            }
          
        }

        private void cmbFundSource_DropDownClosed(object sender, EventArgs e)
        {
            FetchDataSummary();
         
        }

        private void cmbActivity_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void bntRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveAlignment();
        }
    }
}

