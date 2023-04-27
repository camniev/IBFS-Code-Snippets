using Infragistics.Windows.DataPresenter;
using Procurement_Module.Class;

using ReportTool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Procurement_Module.Forms
{
    /// <summary>
    /// Interaction logic for frmPPMP.xaml
    /// </summary>
    public partial class frmPPMP : Window
    {

        private DataTable dtDivisions = new DataTable();
        private clsPPMP c_ppmp = new clsPPMP();
        public String DivID { get; set; }
        public String PAPCode { get; set; }
        public String PAPId { get; set; }
        public frmPPMP()
        {
            InitializeComponent();
            frmppmp.Loaded += frmppmp_Loaded;
        }

        void frmppmp_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDivision(DivID);  
        }

        private void LoadDivision(String DivId) 
        {
            dtDivisions = c_ppmp.GetDivisionList(DivID).Tables[0].Copy();
            cmbDivisions.Items.Clear();
            foreach (DataRow item in dtDivisions.Rows)
            {
                cmbDivisions.Items.Add(item[1].ToString());
            }
            cmbDivisions.SelectedIndex = 0;
        }
        private Boolean isRevised = false;
        private String PAP = "";
        private void LoadFundSource() 
        {
            dtDivisions.DefaultView.RowFilter = "div_name ='"+ cmbDivisions.SelectedItem.ToString() +"'";
            String DivId = dtDivisions.DefaultView[0][0].ToString();
            String FundSource = "";
            dtDivisions.DefaultView.RowFilter = "";
            frmPPMPLoad p_data = new frmPPMPLoad();
            isRevised = false;
            p_data.DivId = DivId;
            p_data.ShowDialog();
            if (p_data.DialogResult== true)
            {
                isRevised = p_data.IsRevised;
            }
            else
            {
                isRevised = p_data.IsRevised;
            }
             
            PAP = "";
            DataTable dtFundSource = c_ppmp.LoadFundSource(DivId, "2017", isRevised, p_data.FundSourceId,p_data.FundType).Tables[0].Copy();
            if (dtFundSource.Rows.Count!=0)
            {
                foreach (DataRow item in dtFundSource.Rows)
                {
                    if (item["respo_name"].ToString().Contains("CONT"))
                    {
                        continue;
                    }
                    else
                    {
                        FundSource= item[2].ToString();
                        break;
                    }
                }
                
            }
            if (FundSource=="")
            {
                FundSource = p_data.FundSourceId;
            }
            DataTable dtPPMPData = c_ppmp.FetchData(DivId, "2017", isRevised, FundSource).Tables[0].Copy();

            if (chkCSE.IsChecked == true)
            {
                dtPPMPData.DefaultView.RowFilter = "uacs_code ='5020301000' or uacs_code='5020399000' or uacs_code='5023990001' or uacs_code ='50203011002'";
                dtFundSource.DefaultView.RowFilter = "mooe_id ='5020301000' or mooe_id='5020399000' or mooe_id = '5023990001' or mooe_id ='50203011002'";
                dtFundSource = dtFundSource.DefaultView.ToTable().Copy();

                dtPPMPData = dtPPMPData.DefaultView.ToTable().Copy();
                PAP = dtFundSource.Rows[0]["pap"].ToString();
            }
            else
            {
                PAP = dtFundSource.Rows[0]["pap"].ToString();
                dtPPMPData.DefaultView.RowFilter = "NOT uacs_code ='5020301000'";
                dtPPMPData = dtPPMPData.DefaultView.ToTable().Copy();
                dtPPMPData.DefaultView.RowFilter = "NOT uacs_code ='5020399000'";
                dtPPMPData = dtPPMPData.DefaultView.ToTable().Copy();
                dtPPMPData.DefaultView.RowFilter = "NOT uacs_code ='5023990001'";
                dtPPMPData = dtPPMPData.DefaultView.ToTable().Copy();
                dtPPMPData.DefaultView.RowFilter = "NOT uacs_code ='50203011002'";
                dtPPMPData = dtPPMPData.DefaultView.ToTable().Copy();

                dtFundSource.DefaultView.RowFilter = "NOT mooe_id ='5020301000'";
                dtFundSource = dtFundSource.DefaultView.ToTable().Copy();
                dtFundSource.DefaultView.RowFilter = "NOT mooe_id ='5020399000'";
                dtFundSource = dtFundSource.DefaultView.ToTable().Copy();
                dtFundSource.DefaultView.RowFilter = "NOT mooe_id ='5023990001'";
                dtFundSource = dtFundSource.DefaultView.ToTable().Copy();
                dtFundSource.DefaultView.RowFilter = "NOT mooe_id ='50203011002'";
                dtFundSource = dtFundSource.DefaultView.ToTable().Copy();
            }
            //dtPPMPData = dtPPMPData.DefaultView.ToTable().Copy();
            DataTable dtData = new DataTable();
            DataTable dtRevisions = new DataTable();
            if (p_data.IsCONT==false)
            {
                dtFundSource.DefaultView.RowFilter = "NOT respo_name LIKE '*CONT*'";
                 dtData = dtFundSource.DefaultView.ToTable().Copy();
                dtData.DefaultView.RowFilter = "is_revision=1";

                 dtRevisions = dtData.DefaultView.ToTable().Copy();
                dtData.DefaultView.RowFilter = "is_revision=0";
                dtData = dtData.DefaultView.ToTable().Copy();
            }
            else
            {

                dtFundSource.DefaultView.RowFilter = "respo_name LIKE '*CONT*'";
                dtData = dtFundSource.DefaultView.ToTable().Copy();
                dtData.DefaultView.RowFilter = "is_revision=1";

                dtRevisions = dtData.DefaultView.ToTable().Copy();
                dtData.DefaultView.RowFilter = "is_revision=0";
                dtData = dtData.DefaultView.ToTable().Copy();
            }
          

            foreach (DataRow item in dtData.Rows)
            {
                double _total = 0.00;
                dtRevisions.DefaultView.RowFilter = "mooe_id ='"+ item["mooe_id"] +"'";
                if (dtRevisions.DefaultView.ToTable().Rows.Count!=0)
	            {
                    double _rowAmount = 0.00;
                    double _origAmount = 0.00;
                  
                    foreach (DataRow itemRow in dtRevisions.DefaultView.ToTable().Rows)
	                {
                        _origAmount = Convert.ToDouble(item["Amount"].ToString());
                        _rowAmount = Convert.ToDouble(itemRow["Amount"].ToString());
                        if (itemRow["plusminus"].ToString()=="+")
                        {
                            
                           
                            item["Amount"] = (_rowAmount + _origAmount).ToString();
                        }
                        else
                        {
                            item["Amount"] = (_rowAmount - _origAmount).ToString();
                        }
                       
	                }

                }
              
               
            }
            foreach (DataRow itemRev in dtRevisions.Rows)
            {
                dtData.DefaultView.RowFilter = "mooe_id = '"+ itemRev["mooe_id"].ToString() +"'";
                if (dtData.DefaultView.ToTable().Rows.Count==0)
                {
                    DataRow dr = dtData.NewRow();
                    dr[0] = itemRev.ItemArray[0].ToString();
                    dr[1] = itemRev.ItemArray[1].ToString();
                    dr[2] = itemRev.ItemArray[2].ToString();
                    dr[3] = itemRev.ItemArray[3].ToString();
                    dr[4] = itemRev.ItemArray[4].ToString();
                    dr[5] = itemRev.ItemArray[5].ToString();
                    dr[6] = itemRev.ItemArray[6].ToString();
                    dr[7] = itemRev.ItemArray[7].ToString();
                    dr[8] = itemRev.ItemArray[8].ToString();
                    dr[9] = itemRev.ItemArray[9].ToString();
                    dr[10] = itemRev.ItemArray[10].ToString();
                    dtData.Rows.Add(dr);
                    continue;
                }
            }
            this.IsCONT = p_data.IsCONT;
            GenerateData(dtData, dtPPMPData,p_data.IsCONT);
        }
        private Boolean IsCONT = false;
        private DataTable dtFinal = new DataTable();
        private DataTable dtReport = new DataTable();
        private void GenerateData(DataTable _FundSource, DataTable _PPMPData,Boolean IsCONT)
        {

            double _Totals = 0.00;
            double _OverAll = 0.00;

             dtFinal = new DataTable();

             dtFinal.Columns.Add("ACTID");
             dtFinal.Columns.Add("UACS");
             dtFinal.Columns.Add("Description");
             dtFinal.Columns.Add("Quantity_Size");
             dtFinal.Columns.Add("EstimatedBudget");
             dtFinal.Columns.Add("ModeOfProcurement");
             
             dtFinal.Columns.Add("_Pap");
             dtFinal.Columns.Add("Jan");
             dtFinal.Columns.Add("Feb");
             dtFinal.Columns.Add("Mar");
             dtFinal.Columns.Add("Apr");
             dtFinal.Columns.Add("May");
             dtFinal.Columns.Add("Jun");
             dtFinal.Columns.Add("Jul");
             dtFinal.Columns.Add("Aug");
             dtFinal.Columns.Add("Sep");
             dtFinal.Columns.Add("Oct");
             dtFinal.Columns.Add("Nov");
             dtFinal.Columns.Add("Dec");
             
             dtFinal.Columns.Add("_Total");
             dtFinal.Columns.Add("_Year");
             dtFinal.Columns.Add("_Division");
             dtFinal.TableName = "PPMP";
            String _PAPCode ="";
             foreach (DataRow item in _FundSource.Rows)
             {
                 double _Allocation = Convert.ToDouble(item["Amount"].ToString());
                 DataRow dr = dtFinal.NewRow();

                 _PPMPData.DefaultView.RowFilter = "uacs_code ='" + item["mooe_id"].ToString() + "'";
                 try
                 {
                     _PAPCode = _PPMPData.DefaultView.ToTable().Rows[0]["Division_Code"].ToString();
                 }
                 catch (Exception)
                 {

                 }
              
                dr["ACTID"] = "" ;
                dr["UACS"] = item["mooe_id"].ToString() ;
                dr["Description"] = item["name"].ToString();
                dr["ModeOfProcurement"] ="" ;
                dr["Quantity_Size"] = "";
                dr["_Year"] = "2017";
               
                dr["_Division"] =cmbDivisions.SelectedItem.ToString();
                dr["_Pap"] =_PAPCode;
                dr["Jan"] ="";
                dr["Feb"] ="";
                dr["Mar"] ="" ;
                dr["Apr"] ="";
                dr["May"] ="";
                dr["Jun"] ="";
                dr["Jul"] ="";
                dr["Aug"] ="";
                dr["Sep"] ="";
                dr["Oct"] ="";
                dr["Nov"] ="";
                dr["Dec"] ="";
                dr["EstimatedBudget"] ="";
                dr["_Total"] = "0.00";

                dtFinal.Rows.Add(dr);
                _Totals = 0.00;
                double totals = 0.00;
                double total_for_contigency = 0.00;

             
                if (_PPMPData.DefaultView.ToTable().Rows.Count!=0)
                {
                    

                        DataTable x_data = _PPMPData.DefaultView.ToTable().Copy();
                        foreach (DataRow itemRows in x_data.Rows)
                        {
                            
                            _PAPCode = itemRows["Division_Code"].ToString();
                            dr = dtFinal.NewRow();

                            dr["ACTID"] = "";
                            dr["UACS"] = "";
                            dr["Description"] = itemRows["type_service"].ToString();
                            dr["ModeOfProcurement"] = "";
                            dr["Quantity_Size"] = itemRows["quantity"].ToString();
                            dr["_Year"] = "2017";
                            dr["_Division"] = cmbDivisions.SelectedItem.ToString();
                            dr["_Pap"] = _PAPCode;


                            switch (itemRows["month"].ToString())
                            {
                                case "Jan":
                                    dr["Jan"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Feb":
                                    dr["Feb"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Mar":
                                    dr["Mar"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Apr":
                                    dr["Apr"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "May":
                                    dr["May"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Jun":
                                    dr["Jun"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Jul":
                                    dr["Jul"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;

                                case "Aug":
                                    dr["Aug"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Sep":
                                    dr["Sep"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Oct":
                                    dr["Oct"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Nov":
                                    dr["Nov"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;
                                case "Dec":
                                    dr["Dec"] = "X";
                                    _Totals += Convert.ToDouble(itemRows["rate"].ToString());
                                    break;

                            }



                            dr["EstimatedBudget"] = Convert.ToDouble(itemRows["rate"].ToString()).ToString("#,##0.00");
                            dr["_Total"] =0.ToString("#,##0.00");
                            total_for_contigency += Convert.ToDouble(itemRows["rate"].ToString());

                            dtFinal.Rows.Add(dr);
                        }

                       

                        dr = dtFinal.NewRow();

                        dr["ACTID"] = "";
                        dr["UACS"] = "";
                        dr["Description"] = "Contingency";
                        dr["ModeOfProcurement"] = "";
                        dr["Quantity_Size"] = "";
                        dr["_Year"] = "2017";
                        dr["_Division"] = cmbDivisions.SelectedItem.ToString();
                        dr["_Pap"] = _PAPCode;
                        dr["Jan"] = "";
                        dr["Feb"] = "";
                        dr["Mar"] = "";
                        dr["Apr"] = "";
                        dr["May"] = "";
                        dr["Jun"] = "";
                        dr["Jul"] = "";
                        dr["Aug"] = "";
                        dr["Sep"] = "";
                        dr["Oct"] = "";
                        dr["Nov"] = "";
                        dr["Dec"] = "";
                        dr["EstimatedBudget"] = (_Allocation - total_for_contigency).ToString("#,##0.00");
                        dr["_Total"] = "0.00";

                        dtFinal.Rows.Add(dr);
                        dr = dtFinal.NewRow();

                        dr["ACTID"] = "";
                        dr["UACS"] = "";
                        dr["Description"] = "Total :" + (total_for_contigency + (_Allocation - total_for_contigency)).ToString("#,##0.00"); ;
                        dr["ModeOfProcurement"] = "";
                        dr["Quantity_Size"] = "";
                        dr["_Year"] = "2017";
                        dr["_Division"] = cmbDivisions.SelectedItem.ToString();
                        dr["_Pap"] = _PAPCode;
                        dr["Jan"] = "";
                        dr["Feb"] = "";
                        dr["Mar"] = "";
                        dr["Apr"] = "";
                        dr["May"] = "";
                        dr["Jun"] = "";
                        dr["Jul"] = "";
                        dr["Aug"] = "";
                        dr["Sep"] = "";
                        dr["Oct"] = "";
                        dr["Nov"] = "";
                        dr["Dec"] = "";
                        dr["EstimatedBudget"] = "";
                        dr["_Total"] = "0.00";

                        dtFinal.Rows.Add(dr);




                }
                else
                {
                    dr = dtFinal.NewRow();

                    dr["ACTID"] = "";
                    dr["UACS"] = "";
                    dr["Description"] = "Contingency";
                    dr["ModeOfProcurement"] = "";
                    dr["Quantity_Size"] = "";
                    dr["_Year"] = "2017";
                    dr["_Division"] = cmbDivisions.SelectedItem.ToString();
                    dr["_Pap"] = _PAPCode;
                    dr["Jan"] = "";
                    dr["Feb"] = "";
                    dr["Mar"] = "";
                    dr["Apr"] = "";
                    dr["May"] = "";
                    dr["Jun"] = "";
                    dr["Jul"] = "";
                    dr["Aug"] = "";
                    dr["Sep"] = "";
                    dr["Oct"] = "";
                    dr["Nov"] = "";
                    dr["Dec"] = "";
                    dr["EstimatedBudget"] = (_Allocation).ToString("#,##0.00");
                    dr["_Total"] = (_Allocation).ToString("#,##0.00"); 

                    dtFinal.Rows.Add(dr);
                    dr = dtFinal.NewRow();

                    dr["ACTID"] = "";
                    dr["UACS"] = "";
                    dr["Description"] = "Total :" + (_Allocation).ToString("#,##0.00"); ;
                    dr["ModeOfProcurement"] = "";
                    dr["Quantity_Size"] = "";
                    dr["_Year"] = "2017";
                    dr["_Division"] = cmbDivisions.SelectedItem.ToString();
                    dr["_Pap"] = _PAPCode;
                    dr["Jan"] = "";
                    dr["Feb"] = "";
                    dr["Mar"] = "";
                    dr["Apr"] = "";
                    dr["May"] = "";
                    dr["Jun"] = "";
                    dr["Jul"] = "";
                    dr["Aug"] = "";
                    dr["Sep"] = "";
                    dr["Oct"] = "";
                    dr["Nov"] = "";
                    dr["Dec"] = "";
                    dr["EstimatedBudget"] = "";
                    dr["_Total"] = "0.00";

                    dtFinal.Rows.Add(dr);
                }

              

             }
            Boolean NotPassed = false;
             for (int i = 0; i < dtFinal.Rows.Count; i++)
             {
                 String _checkval = dtFinal.Rows[i]["EstimatedBudget"].ToString();
                 if (_checkval=="")
                 {
                     _checkval = "0";
                 }

                 double _val = Convert.ToDouble(_checkval);
                 if (_val <0)
                 {
                     NotPassed = true;
                     break;
                 }
             }
            if (NotPassed )
	        {
                MessageBox.Show("Please settle first your PPMP Data" + Environment.NewLine + "You have a negative amount on your activities");
		        btnPrintPreview.IsEnabled = false;
	        }else
	        {
                btnPrintPreview.IsEnabled = true;
	        }
            
             grdData.DataSource = null;
             grdData.DataSource = dtFinal.DefaultView;

             grdData.FieldLayouts[0].Fields["ACTID"].Visibility = Visibility.Collapsed;
             grdData.FieldLayouts[0].Fields["_Year"].Visibility = Visibility.Collapsed;
             grdData.FieldLayouts[0].Fields["_Pap"].Visibility = Visibility.Collapsed;
             grdData.FieldLayouts[0].Fields["_Division"].Visibility = Visibility.Collapsed;
             grdData.FieldLayouts[0].Fields["_Total"].Visibility = Visibility.Collapsed;
        
            
             grdData.FieldLayouts[0].Fields["UACS"].FixedLocation = Infragistics.Windows.DataPresenter.FixedFieldLocation.FixedToNearEdge;
             grdData.FieldLayouts[0].Fields["Description"].FixedLocation = Infragistics.Windows.DataPresenter.FixedFieldLocation.FixedToNearEdge;
             grdData.FieldLayouts[0].Fields["Quantity_Size"].FixedLocation = Infragistics.Windows.DataPresenter.FixedFieldLocation.FixedToNearEdge;
             grdData.FieldLayouts[0].Fields["EstimatedBudget"].FixedLocation = Infragistics.Windows.DataPresenter.FixedFieldLocation.FixedToNearEdge;
             grdData.FieldLayouts[0].Fields["ModeOfProcurement"].FixedLocation = Infragistics.Windows.DataPresenter.FixedFieldLocation.FixedToNearEdge;

             grdData.FieldLayouts[0].Fields["UACS"].Label = "UACS";
             grdData.FieldLayouts[0].Fields["Description"].Label = "Description";
             grdData.FieldLayouts[0].Fields["Quantity_Size"].Label = "Quantity";
             grdData.FieldLayouts[0].Fields["EstimatedBudget"].Label = "Estimated Budget";
             grdData.FieldLayouts[0].Fields["ModeOfProcurement"].Label = "Mode Procurement";
                
             grdData.FieldLayouts[0].Fields["UACS"].PerformAutoSize();
             grdData.FieldLayouts[0].Fields["Description"].PerformAutoSize();
             grdData.FieldLayouts[0].Fields["Quantity_Size"].PerformAutoSize();
             grdData.FieldLayouts[0].Fields["EstimatedBudget"].PerformAutoSize();
             grdData.FieldLayouts[0].Fields["ModeOfProcurement"].PerformAutoSize();

        }
        private void LoadReportPPMP(Boolean IsCONT)
        {
           
            

            dtReport = new DataTable();

            dtReport.Columns.Add("Uacs");
            dtReport.Columns.Add("Description");
            dtReport.Columns.Add("Quantity");
            dtReport.Columns.Add("EstimateBudget");
            dtReport.Columns.Add("ModeOfProcurement");
            dtReport.Columns.Add("Jan");
            dtReport.Columns.Add("Feb");
            dtReport.Columns.Add("Mar");
            dtReport.Columns.Add("Apr");
            dtReport.Columns.Add("May");
            dtReport.Columns.Add("Jun");
            dtReport.Columns.Add("Jul");
            dtReport.Columns.Add("Aug");
            dtReport.Columns.Add("Sep");
            dtReport.Columns.Add("Octs");
            dtReport.Columns.Add("Nov");
            dtReport.Columns.Add("Dec");
            dtReport.Columns.Add("Total");
            dtReport.Columns.Add("Division");
            dtReport.Columns.Add("Yearssss");
            dtReport.Columns.Add("PAP");
            dtReport.Columns.Add("approved");
            dtReport.Columns.Add("header");
            dtReport.Columns.Add("revision");
            dtReport.Columns.Add("overall");
            double Total = 0.00;
            double Contigency = 0.00;
            foreach (DataRow item in dtFinal.Rows)
            {
                
                string _debug = item["Description"].ToString();
                if (item["Description"].ToString()!= "Contingency")
                {
                    try
                    {
                        Total += Convert.ToDouble(item["EstimatedBudget"].ToString());
                    }
                    catch (Exception)
                    {

                    }
                
                }
                else
                {
                    try
                    {
                        Contigency += Convert.ToDouble(item["EstimatedBudget"].ToString());
                    }
                    catch (Exception)
                    {

                    }
                
                }
              
             
            }
            foreach (DataRow item in dtFinal.Rows)
            {
                DataRow dr = dtReport.NewRow();

                dr["Uacs"] = item["UACS"].ToString() ;
                dr["Description"] = item["Description"].ToString() ;
                dr["Quantity"] =item["Quantity_Size"].ToString()  ;
                dr["EstimateBudget"] =item["EstimatedBudget"].ToString()  ;
                dr["ModeOfProcurement"] = item["ModeOfProcurement"].ToString() ;
                dr["Jan"] = item["Jan"].ToString() ;
                dr["Feb"] =  item["Feb"].ToString()  ;
                dr["Mar"] = item["Mar"].ToString() ;
                dr["Apr"] = item["Apr"].ToString() ;
                dr["May"] = item["May"].ToString() ;
                dr["Jun"] = item["Jun"].ToString() ;
                dr["Jul"] = item["Jul"].ToString() ;
                dr["Aug"] = item["Aug"].ToString() ;
                dr["Sep"] = item["Sep"].ToString() ;
                dr["Octs"] = item["Oct"].ToString() ;
                dr["Nov"] = item["Nov"].ToString() ;
                dr["Dec"] = item["Dec"].ToString() ;
                dr["Total"] =(Total + Contigency).ToString() ;
                dr["Division"] = item["_Division"].ToString() ;
                dr["Yearssss"] = item["_Year"].ToString() ;
                dr["PAP"] = PAP;// item["_Pap"].ToString();
                dr["approved"] = "0" ;
                if (chkCSE.IsChecked== true)
                {
                    if (isRevised)
                    {
                        if (IsCONT) 
                        {
                            dr["header"] = "PROJECT PROCUREMENT MANAGEMENT PLAN(PPMP-CONT-CSE) - Revised";
                        }
                        else
                        {
                            dr["header"] = "PROJECT PROCUREMENT MANAGEMENT PLAN(PPMP-CSE) - Revised";
                        }
                       
                    }
                    else
                    {
                        if (IsCONT)                        
                        {
                            dr["header"] = "PROJECT PROCUREMENT MANAGEMENT PLAN(PPMP-CONT-CSE)";
                        }
                        else
                        {
                            dr["header"] = "PROJECT PROCUREMENT MANAGEMENT PLAN(PPMP-CSE)";
                        }
                      
                    }
                   
                }
                else
                {
                    if (isRevised)
                    {
                        if (IsCONT)
                        {
                            dr["header"] = "PROJECT PROCUREMENT MANAGEMENT PLAN(PPMP-CONT) - Revised";
                        }
                        else
                        {
                            dr["header"] = "PROJECT PROCUREMENT MANAGEMENT PLAN(PPMP) - Revised";
                        }
                       
                    }
                    else
                    {
                        if (IsCONT)
                        {
                            dr["header"] = "PROJECT PROCUREMENT MANAGEMENT PLAN(PPMP-CONT)";
                        }
                        else
                        {
                            dr["header"] = "PROJECT PROCUREMENT MANAGEMENT PLAN(PPMP)";
                        }
                       
                    }
                }
               
                dr["revision"] = "0" ;
                dr["overall"] = "0" ;

                dtReport.Rows.Add(dr);
            }
          
            
            DataSet ds = new DataSet();
            ds.Tables.Add(dtReport);


            frmGenericReport f_preview = new frmGenericReport(ds,"PPMP");


            f_preview.Show();
           

        }

        private void LoadReportAnnexA()
        {

            DataTable dtAnnexA = c_ppmp.FetchDataAnnexA(DivID, "2017").Tables[0].Copy();
            DataTable dtAnnexATitle = c_ppmp.FetchDataAnnexATitle(DivID, "2017").Tables[0].Copy();
            dtReport = new DataTable();

            dtReport.Columns.Add("mfo_pap");
            dtReport.Columns.Add("from_obect_expenditure");
            dtReport.Columns.Add("from_total");
            dtReport.Columns.Add("to_obect_expenditure");
            dtReport.Columns.Add("to_total");
        
            double Total = 0.00;
            double Contigency = 0.00;

            for (int i = 0; i < dtAnnexATitle.Rows.Count; i++)
            {

                DataRow dr = dtReport.NewRow();
                dtAnnexA.DefaultView.RowFilter = "";
                dtAnnexA.DefaultView.RowFilter = "from_obect_expenditure ='"+  dtAnnexATitle.Rows[i][1].ToString() +"'";
                DataTable dt = dtAnnexA.DefaultView.ToTable().Copy();
                if (dt.Rows.Count >1)
                {

                    dr["mfo_pap"] = "Operations " + Environment.NewLine + Environment.NewLine + "MFO 1: Integrated Policies and Programs for Mindanao " + Environment.NewLine + Environment.NewLine + Environment.NewLine + "      " + PAPCode;
                    dr["from_obect_expenditure"] = dtAnnexATitle.Rows[i][1].ToString();
                    double _total=0.00;
                    for (int c = 0; c < dt.Rows.Count; c++)
                    {
                        _total += Convert.ToDouble(dt.Rows[c][4].ToString());
                    }

                    dr["from_total"] = _total;
                    dr["to_obect_expenditure"] = dt.Rows[0][3].ToString();
                    dr["to_total"] = dt.Rows[0][4].ToString();
                    dtReport.Rows.Add(dr);
                    for (int x = 1; x < dt.Rows.Count; x++)
                    {
                        dr = dtReport.NewRow();
                        dr["mfo_pap"] = "Operations " + Environment.NewLine + Environment.NewLine + "MFO 1: Integrated Policies and Programs for Mindanao " + Environment.NewLine + Environment.NewLine + Environment.NewLine + "      " + PAPCode;
                        dr["from_obect_expenditure"] = "";
                        dr["from_total"] = "0";
                        dr["to_obect_expenditure"] = dt.Rows[x][3].ToString();
                        dr["to_total"] = dt.Rows[x][4].ToString();
                        dtReport.Rows.Add(dr);
                    }
                }
                else
                {

                    dr["mfo_pap"] = "Operations " + Environment.NewLine + Environment.NewLine + "MFO 1: Integrated Policies and Programs for Mindanao " + Environment.NewLine + Environment.NewLine + Environment.NewLine + "      " + PAPCode;
                    dr["from_obect_expenditure"] = dtAnnexATitle.Rows[i][1].ToString();
                    dr["from_total"] = dt.Rows[0][2].ToString();
                    dr["to_obect_expenditure"] = dt.Rows[0][3].ToString();
                    dr["to_total"] = dt.Rows[0][4].ToString();
                    dtReport.Rows.Add(dr);
                }
              
                
            }


            DataSet ds = new DataSet();
            ds.Tables.Add(dtReport);


            frmGenericReport f_preview = new frmGenericReport(ds, "AnnexA");


            f_preview.Show();


        }

        private void LoadReportAnnexB()
        {

            DataTable dtAnnexB = c_ppmp.FetchDataAnnexBDefficient(DivID, "2017").Tables[0].Copy();
          
            dtReport = new DataTable();

            dtReport.Columns.Add("group");
            dtReport.Columns.Add("programs_activity");
            dtReport.Columns.Add("responsibility_center");
            dtReport.Columns.Add("allotmentclass");
            dtReport.Columns.Add("obect_expenditure");
            dtReport.Columns.Add("amount");

            double Total = 0.00;
            double Contigency = 0.00;

            for (int i = 0; i < dtAnnexB.Rows.Count; i++)
            {
                DataRow dr = dtReport.NewRow();

                dr["group"] = "DEFICIENT ITEMS (TO):";
                dr["programs_activity"] = PAPCode.Split('-')[0].Trim();
                dr["responsibility_center"] = PAPCode.Replace(PAPCode.Split('-')[0].Trim(), "").Replace(" -", "").Trim() + " - " + cmbDivisions.SelectedItem.ToString(); ;
                dr["allotmentclass"] ="MOOE";
                dr["obect_expenditure"] = dtAnnexB.Rows[i][0].ToString();
                dr["amount"] = dtAnnexB.Rows[i][1].ToString();
                    dtReport.Rows.Add(dr);
             
            }
            dtAnnexB = c_ppmp.FetchDataAnnexBSource(DivID, "2017").Tables[0].Copy();

            for (int i = 0; i < dtAnnexB.Rows.Count; i++)
            {
                DataRow dr = dtReport.NewRow();

                dr["group"] = "SOURCE ITEMS (FROM):";
                dr["programs_activity"] = PAPCode.Split('-')[0].Trim();
                dr["responsibility_center"] = PAPCode.Replace(PAPCode.Split('-')[0].Trim(), "").Replace(" -", "").Trim() + " - " + cmbDivisions.SelectedItem.ToString(); ;
                dr["allotmentclass"] = "MOOE";
                dr["obect_expenditure"] = dtAnnexB.Rows[i][0].ToString();
                dr["amount"] = "-" + dtAnnexB.Rows[i][1].ToString();
                dtReport.Rows.Add(dr);

            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dtReport);


            frmGenericReport f_preview = new frmGenericReport(ds, "AnnexB");


            f_preview.Show();


        }
        private void btnPPMPPrintout_Click(object sender, RoutedEventArgs e)
        {
            LoadFundSource();
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            LoadReportPPMP(this.IsCONT);


        }

        private void btnAnnexA_Click(object sender, RoutedEventArgs e)
        {
            LoadReportAnnexA();
        }

        private void btnAnnexB_Click(object sender, RoutedEventArgs e)
        {
            LoadReportAnnexB();
        }
    }
}
