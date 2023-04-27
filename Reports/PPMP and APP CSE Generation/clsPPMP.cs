using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MinDAF.Class
{
    public class clsPPMP
    {
        public event EventHandler SQLOperation;
        public String Process { get; set; }
        public String ReturnCode { get; set; }
        private clsServiceOperation c_ops = new clsServiceOperation();

        public String FetchData(string _div_id,string _year) 
        {
            var sb = new System.Text.StringBuilder(873);
            sb.AppendLine(@"SELECT");
            sb.AppendLine(@"  mpe.acountable_division_code,");
            sb.AppendLine(@"  mfs.Fund_Name,");
            sb.AppendLine(@"  div.Division_Desc,");
            sb.AppendLine(@"  div.Division_Code,");
            sb.AppendLine(@"  mooe.uacs_code,");
            sb.AppendLine(@"  mooe.name,");
            sb.AppendLine(@"  mad.total as rate,");
            sb.AppendLine(@"  mad.quantity,mad.type_service,ISNULL(mad.id,0) as act_id,");
            sb.AppendLine(@"  mad.month,");
            sb.AppendLine(@"  mad.year,");
            sb.AppendLine(@"  ISNULL(mad.entry_date,'') as entry_date,");
            sb.AppendLine(@"  ISNULL(mad.start,'') as s_date,");
            sb.AppendLine(@"  ISNULL(mad.[end],'') as e_date");
            sb.AppendLine(@"FROM mnda_approved_projects_division madp");
            sb.AppendLine(@"LEFT JOIN mnda_activity_data mad on mad.id = madp.activity_id");
            sb.AppendLine(@"LEFT JOIN mnda_mooe_sub_expenditures mooe on mooe.id = mad.mooe_sub_expenditure_id");
            sb.AppendLine(@"LEFT JOIN mnda_activity ma on ma.id = madp.main_activity_id");
            sb.AppendLine(@"LEFT JOIN mnda_project_output mpo on mpo.id = ma.output_id	");
            sb.AppendLine(@"LEFT JOIN mnda_program_encoded mpe on mpe.id = mpo.program_code");
            sb.AppendLine(@"LEFT JOIN mnda_fund_source mfs on mfs.code = mad.fund_source_id");
            sb.AppendLine(@"LEFT JOIN Division div on div.Division_Id = mpe.acountable_division_code");
            sb.AppendLine(@"WHERE mpe.acountable_division_code = " + _div_id + " AND mad.year = " + _year + " and mad.is_suspended =0  and madp.is_submitted =0 and mfs.service_type = 1 and madp.isapproved = 1");

            return sb.ToString();
        }
        public String FetchDataAPP(string _year)
        {

            StringBuilder sb = new StringBuilder(402);
            sb.AppendLine(@"SELECT");
            sb.AppendLine(@"mfs.Fund_Name,");
            sb.AppendLine(@"div.Division_Desc,");
            sb.AppendLine(@"div.Division_Code,");
            sb.AppendLine(@"mooe.uacs_code,");
            sb.AppendLine(@"mooe.name,");
            sb.AppendLine(@"mad.total as rate,");
            sb.AppendLine(@"mad.quantity,mad.type_service,ISNULL(mad.id,0) as act_id,");
            sb.AppendLine(@"mad.month,");
            sb.AppendLine(@"mad.year,");
            sb.AppendLine(@"ISNULL(mad.entry_date,'') as entry_date,");
            sb.AppendLine(@"ISNULL(mad.start,'') as s_date,");
            sb.AppendLine(@"ISNULL(mad.[end],'') as e_date");
            sb.AppendLine(@"FROM mnda_approved_projects_division madp");
            sb.AppendLine(@"LEFT JOIN mnda_activity_data mad on mad.id = madp.activity_id");
            sb.AppendLine(@"LEFT JOIN mnda_mooe_sub_expenditures mooe on mooe.id = mad.mooe_sub_expenditure_id");
            sb.AppendLine(@"LEFT JOIN mnda_program_encoded mpe on mpe.id = madp.program_id");
            sb.AppendLine(@"LEFT JOIN mnda_fund_source mfs on mfs.code = mad.fund_source_id");
            sb.AppendLine(@"LEFT JOIN Division div on div.Division_Id = mpe.acountable_division_code");
            sb.AppendLine(@"WHERE  mad.year = " + _year + " and madp.is_submitted =0 and mfs.service_type = 1");
            sb.AppendLine(@" and div.Division_Desc= 'Project Development Division (PDD)' ");
            sb.AppendLine(@"and div.Division_Code = '301010003'");
            sb.AppendLine(@"and mooe.name NOT in ('ICT Office Supplies','Other Supplies and Materials Expenses','Office Supplies Expenses')");

            return sb.ToString();
        }
        public String FetchRevision(string _pap,String _year)
        {
            var sb = new System.Text.StringBuilder(149);
            sb.AppendLine(@"SELECT ");
            sb.AppendLine(@"COUNT(ppmp.revision) as _rev");
            sb.AppendLine(@"FROM ");
            sb.AppendLine(@"  dbo.mnda_report_data_ppmp ppmp");
            sb.AppendLine(@"WHERE ppmp.PAP ='"+ _pap +"' and ppmp.Yearssss = '" + _year + "'");
            sb.AppendLine(@"GROUP BY ppmp.revision");
            return sb.ToString();
        }

        public String FetchGeneralCategory()
        {
            var sb = new System.Text.StringBuilder(149);
            sb.AppendLine(@"SELECT ");
            sb.AppendLine(@"DISTINCT general_category");
            sb.AppendLine(@"FROM ");
            sb.AppendLine(@"  dbo.mnda_procurement_items_updated");
            return sb.ToString();
        }

        public String FetchSubCategory()
        {
            var sb = new System.Text.StringBuilder(149);
            sb.AppendLine(@"SELECT ");
            sb.AppendLine(@"DISTINCT sub_category");
            sb.AppendLine(@"FROM ");
            sb.AppendLine(@"  dbo.mnda_procurement_items_updated");
            return sb.ToString();
        }

        public String FetchSupplies(String year, String fund_source_id)
        {
            var sb = new System.Text.StringBuilder(149);
            sb.AppendLine(@"SELECT");
            sb.AppendLine(@" DISTINCT mad.type_service,");
            sb.AppendLine(@" mooe.name,");
            sb.AppendLine(@" mooe.uacs_code,");
            sb.AppendLine(@" mpin.sub_category");
            sb.AppendLine(@" FROM mnda_activity_data mad");
            sb.AppendLine(@" LEFT JOIN mnda_procurement_items_updated mpin on mpin.item_specifications = mad.type_service");
            sb.AppendLine(@" LEFT JOIN mnda_mooe_sub_expenditures mooe on mooe.id = mad.mooe_sub_expenditure_id");
            sb.AppendLine(@" LEFT JOIN mnda_fund_source mfs on mfs.code = mad.fund_source_id");
            sb.AppendLine(@" WHERE mad.year = " + year + "and mfs.service_type = 1 and mad.total !=0 and mad.status ='FINANCE APPROVED'");
            sb.AppendLine(@" AND mad.fund_source_id ='" + fund_source_id + "'  AND mad.is_approved = 1");
            sb.AppendLine(@" GROUP BY");
            sb.AppendLine(@" mfs.Fund_Name,");
            sb.AppendLine(@" mooe.uacs_code,");
            sb.AppendLine(@" mpin.sub_category,");
            sb.AppendLine(@" mooe.name,");
            sb.AppendLine(@" mad.total,");
            sb.AppendLine(@" mad.quantity,");
            sb.AppendLine(@" mad.type_service,");
            sb.AppendLine(@" mad.id,");
            sb.AppendLine(@" mad.month,");
            sb.AppendLine(@" mad.year ,mad.entry_date,");
            sb.AppendLine(@" mad.start,");
            sb.AppendLine(@" mad.[end],mad.status");
            return sb.ToString();
        }

        public String FetchApprovals(String _pap) 
            {
                var sb = new System.Text.StringBuilder(263);
                sb.AppendLine(@"SELECT ");
                sb.AppendLine(@"  Uacs,");
                sb.AppendLine(@"  Description,");
                sb.AppendLine(@"  Quantity,");
                sb.AppendLine(@"  EstimateBudget,");
                sb.AppendLine(@"  ModeOfProcurement,");
                sb.AppendLine(@"  Jan,");
                sb.AppendLine(@"  Feb,");
                sb.AppendLine(@"  Mar,");
                sb.AppendLine(@"  Apr,");
                sb.AppendLine(@"  May,");
                sb.AppendLine(@"  Jun,");
                sb.AppendLine(@"  Jul,");
                sb.AppendLine(@"  Aug,");
                sb.AppendLine(@"  Sep,");
                sb.AppendLine(@"  Octs,");
                sb.AppendLine(@"  Nov,");
                sb.AppendLine(@"  Dec,");
                sb.AppendLine(@"  Total,");
                sb.AppendLine(@"  Division,");
                sb.AppendLine(@"  Yearssss,");
                sb.AppendLine(@"  PAP");
                sb.AppendLine(@"FROM ");
                sb.AppendLine(@"  dbo.mnda_report_data_ppmp WHERE approved = 0 AND PAP ='"+ _pap +"';");


                return sb.ToString();
            }
        public String FetchFundSource(String div)
          {
              var sb = new System.Text.StringBuilder(101);
              sb.AppendLine(@"SELECT ");
              sb.AppendLine(@"  Fund_Source_Id,");
              sb.AppendLine(@"  Fund_Name");
              sb.AppendLine(@"FROM ");
              sb.AppendLine(@"  dbo.mnda_fund_source mfs");
              sb.AppendLine(@"WHERE mfs.division_id='"+ div+"';");



              return sb.ToString();
          }
        public String FetchReview(String _pap,String _year)
          {
              var sb = new System.Text.StringBuilder(263);
              sb.AppendLine(@"SELECT ");
              sb.AppendLine(@"  Uacs,");
              sb.AppendLine(@"  Description,");
              sb.AppendLine(@"  Quantity,");
              sb.AppendLine(@"  EstimateBudget,");
              sb.AppendLine(@"  ModeOfProcurement,");
              sb.AppendLine(@"  Jan,");
              sb.AppendLine(@"  Feb,");
              sb.AppendLine(@"  Mar,");
              sb.AppendLine(@"  Apr,");
              sb.AppendLine(@"  May,");
              sb.AppendLine(@"  Jun,");
              sb.AppendLine(@"  Jul,");
              sb.AppendLine(@"  Aug,");
              sb.AppendLine(@"  Sep,");
              sb.AppendLine(@"  Octs,");
              sb.AppendLine(@"  Nov,");
              sb.AppendLine(@"  Dec,");
              sb.AppendLine(@"  Total,");
              sb.AppendLine(@"  Division,");
              sb.AppendLine(@"  Yearssss,");
              sb.AppendLine(@"  PAP");
              sb.AppendLine(@"FROM ");
              sb.AppendLine(@"  dbo.mnda_report_data_ppmp WHERE approved = 1 AND PAP ='" + _pap + "' AND Yearssss ='"+ _year +"';");


              return sb.ToString();
          }
        public String FetchView(String _pap, String _year)
          {
              var sb = new System.Text.StringBuilder(263);
              sb.AppendLine(@"SELECT ");
              sb.AppendLine(@"  Uacs,");
              sb.AppendLine(@"  Description,");
              sb.AppendLine(@"  Quantity,");
              sb.AppendLine(@"  EstimateBudget,");
              sb.AppendLine(@"  ModeOfProcurement,");
              sb.AppendLine(@"  Jan,");
              sb.AppendLine(@"  Feb,");
              sb.AppendLine(@"  Mar,");
              sb.AppendLine(@"  Apr,");
              sb.AppendLine(@"  May,");
              sb.AppendLine(@"  Jun,");
              sb.AppendLine(@"  Jul,");
              sb.AppendLine(@"  Aug,");
              sb.AppendLine(@"  Sep,");
              sb.AppendLine(@"  Octs,");
              sb.AppendLine(@"  Nov,");
              sb.AppendLine(@"  Dec,");
              sb.AppendLine(@"  Total,");
              sb.AppendLine(@"  Division,");
              sb.AppendLine(@"  Yearssss,");
              sb.AppendLine(@"  PAP");
              sb.AppendLine(@"FROM ");
              sb.AppendLine(@"  dbo.mnda_report_data_ppmp WHERE approved = 1 AND PAP ='" + _pap + "' AND Yearssss ='" + _year + "';");

              return sb.ToString();
          }

        public String FetchDivisionWithPap()
          {
              var sb = new System.Text.StringBuilder(66);
              sb.AppendLine(@"SELECT ");
              sb.AppendLine(@"  Division_Id as DivisionId,");
              sb.AppendLine(@"  Division_Code,");
              sb.AppendLine(@"  Division_Desc");
              sb.AppendLine(@"FROM ");
              sb.AppendLine(@"  dbo.Division;");


              return sb.ToString();
          }

        public void ApprovePPMP(String _pap,String _approval)
          {
              String _sqlString = "";
              var sb = new System.Text.StringBuilder(74);
              sb.AppendLine(@"UPDATE ");
              sb.AppendLine(@"  dbo.mnda_report_data_ppmp  ");
              sb.AppendLine(@"SET ");
              sb.AppendLine(@"  approved = "+ _approval+"");
              if (_approval == "1")
              {
                  sb.AppendLine(@"WHERE pap =" + _pap + " AND approved = '0';");
              }
              else
              {
                  sb.AppendLine(@"WHERE pap =" + _pap + " AND approved = '1';");
              }
           
              
              _sqlString = sb.ToString();

              c_ops.InstantiateService();
              c_ops.ExecuteSQL(_sqlString);
              c_ops.DataReturn += c_ops_DataReturn;


          }

        public String GetSubExpenditures()
        {
            var sb = new System.Text.StringBuilder(85);
            sb.AppendLine(@"SELECT");
            sb.AppendLine(@"id, name, is_for_ppmp");
            sb.AppendLine(@"FROM mnda_mooe_sub_expenditures WHERE is_active = 1 ORDER BY name ASC");

            return sb.ToString();
        }

        public void UpdatePPMPExpenseItems(int id, Boolean is_for_ppmp)
        {
            String _sqlString = "";
            var sb = new System.Text.StringBuilder(300);

            sb.AppendLine(@"UPDATE ");
            sb.AppendLine(@"dbo.mnda_mooe_sub_expenditures ");
            sb.AppendLine(@"SET is_for_ppmp = '" + is_for_ppmp + "' ");
            sb.AppendLine(@"WHERE id = " + id + ";");

            _sqlString = sb.ToString();

            c_ops.InstantiateService();
            c_ops.ExecuteSQL(_sqlString);
            c_ops.DataReturn += c_ops_DataReturn;
            //MessageBox.Show(id + ": " + is_for_ppmp);
        }

        public Boolean SubmitPPMP(List<PPMPFormat> _data,String _Rev,String OverAll)
          {
              String _sqlString = "";
              StringBuilder sb = new StringBuilder(300);
              String _header = "";

              if (_Rev =="0")
              {
                  _header = "PROJECT PROCUREMENT MANAGEMENT PLAN (PPMP)";
              }
              else
              {
                  _header = "REVISED PROJECT PROCUREMENT MANAGEMENT PLAN (PPMP)";
              }
         String _sqlHead   ="INSERT INTO  dbo.mnda_report_data_ppmp(" +
                    "  Uacs,  Description,  Quantity,  EstimateBudget,  ModeOfProcurement," + Environment.NewLine + 
                    "  Jan,  Feb,  Mar,  Apr,  May,  Jun,  Jul,  Aug,  Sep,  Octs,  Nov," + Environment.NewLine + 
                    "  Dec,  Total,  Division,  Yearssss,  PAP,  approved,header,revision,overall) VALUES "; 
           //   sb.AppendLine(@"DELETE FROM dbo.mnda_report_data_ppmp;");
         var count = _data.Count;

              foreach (var item in _data)
              {
                 
                  if (item.Description.Contains("'"))
                  {
                      item.Description = item.Description.Replace("'", "");
                  }
                  if (item.Description.Contains("\""))
                  {
                      item.Description = item.Description.Replace("\"", "");
                  }
                  if (--count > 0)
                  {
                      _sqlString += "('" + item.UACS + "','"+ item.Description +"','" + item.Quantity_Size + "','" + item.EstimatedBudget + "','" + item.ModeOfProcurement + "','" + item.Jan + "','" + item.Feb + "'," + Environment.NewLine +
                  "   '" + item.Mar + "','" + item.Apr + "','" + item.May + "','" + item.Jun + "', '" + item.Jul + "','" + item.Aug + "','" + item.Sep + "','" + item.Oct + "','" + item.Nov + "','" + item.Dec + "','" + item.EstimatedBudget + "','" + item._Division + "','" + item._Year + "', '" + item._Pap + "','0','" + _header + "','" + _Rev + "','" + OverAll + "'),";             
                
                  }
                  else
                  {
                      _sqlString += "('" + item.UACS + "','"+ item.Description +"','" + item.Quantity_Size + "','" + item.EstimatedBudget + "','" + item.ModeOfProcurement + "','" + item.Jan + "','" + item.Feb + "'," + Environment.NewLine +
                  "   '" + item.Mar + "','" + item.Apr + "','" + item.May + "','" + item.Jun + "', '" + item.Jul + "','" + item.Aug + "','" + item.Sep + "','" + item.Oct + "','" + item.Nov + "','" + item.Dec + "','" + item.EstimatedBudget + "','" + item._Division + "','" + item._Year + "', '" + item._Pap + "','0','" + _header + "','" + _Rev + "','" + OverAll + "')";             
                
                  }
                  
              }



              c_ops.InstantiateService();
              c_ops.ExecuteSQL(_sqlHead + _sqlString);
              c_ops.DataReturn += c_ops_DataReturn;
              return true;

          }

        public void SaveReportPrintOut(List<PPMPFormat> _data)
        {
            String _sqlString = "";
            StringBuilder sb = new StringBuilder(1000);
            sb.AppendLine(@"DELETE FROM dbo.mnda_report_data_ppmp;");
            foreach (var item in _data)
            {

                sb.AppendLine(@" INSERT INTO ");
                sb.AppendLine(@"  dbo.mnda_report_data_ppmp ");
                sb.AppendLine(@"( ");
                sb.AppendLine(@"  Uacs, ");
                sb.AppendLine(@"  Description, ");
                sb.AppendLine(@"  Quantity, ");
                sb.AppendLine(@"  EstimateBudget, ");
                sb.AppendLine(@"  ModeOfProcurement, ");
                sb.AppendLine(@"  Jan, ");
                sb.AppendLine(@"  Feb, ");
                sb.AppendLine(@"  Mar, ");
                sb.AppendLine(@"  Apr, ");
                sb.AppendLine(@"  May, ");
                sb.AppendLine(@"  Jun, ");
                sb.AppendLine(@"  Jul, ");
                sb.AppendLine(@"  Aug, ");
                sb.AppendLine(@"  Sep, ");
                sb.AppendLine(@"  Octs, ");
                sb.AppendLine(@"  Nov, ");
                sb.AppendLine(@"  Dec, ");
                sb.AppendLine(@"  Total, ");
                sb.AppendLine(@"  Division, ");
                sb.AppendLine(@"  Yearssss, ");
                sb.AppendLine(@"  PAP ");
                sb.AppendLine(@")  ");
                sb.AppendLine(@"VALUES ( ");
                sb.AppendLine(@"  '" + item.UACS +"', ");
                sb.AppendLine(@"  '" + item.Description +"', ");
                sb.AppendLine(@"  '" + item.Quantity_Size +"', ");
                sb.AppendLine(@"  '" + item.EstimatedBudget +"', ");
                sb.AppendLine(@"  '" + item.ModeOfProcurement +"', ");
                sb.AppendLine(@"  '" + item.Jan +"', ");
                sb.AppendLine(@"  '" + item.Feb +"', ");
                sb.AppendLine(@"  '" + item.Mar +"', ");
                sb.AppendLine(@"  '" + item.Apr +"', ");
                sb.AppendLine(@"  '" + item.May +"', ");
                sb.AppendLine(@"  '" + item.Jun +"', ");
                sb.AppendLine(@"  '" + item.Jul +"', ");
                sb.AppendLine(@"  '" + item.Aug +"', ");
                sb.AppendLine(@"  '" + item.Sep +"', ");
                sb.AppendLine(@"  '" + item.Oct +"', ");
                sb.AppendLine(@"  '" + item.Nov +"', ");
                sb.AppendLine(@"  '" + item.Dec +"', ");
                sb.AppendLine(@"  '" + item._Total +"', ");
                sb.AppendLine(@"  '" + item._Division +"', ");
                sb.AppendLine(@"  '"+ item._Year +"', ");
                sb.AppendLine(@"  '"+ item._Pap +"' ");
                sb.AppendLine(@"  ); ");
                _sqlString += sb.ToString();
            }
        

    
            c_ops.InstantiateService();
            c_ops.ExecuteSQL(_sqlString);
            c_ops.DataReturn += c_ops_DataReturn;


        }

        void c_ops_DataReturn(object sender, EventArgs e)
        {
            switch ( this.Process)
            {
                case "SaveReportPrintOut":
                     if (SQLOperation!=null)
	                {
                        SQLOperation(this, new EventArgs());
	                }
                    break;
                case "ApprovePPMP":
                    if (SQLOperation != null)
                    {
                        SQLOperation(this, new EventArgs());
                    }
                    break;
                case "RevisePPMP":
                    if (SQLOperation != null)
                    {
                        SQLOperation(this, new EventArgs());
                    }
                    break;  
            }
        }

    }

    public class PPMPFinanceFS 
    {
         public String Fund_Source_Id {get;set;}
         public String Fund_Name { get; set; }
    }
    public class PPMPViewDivision
    {
        public String PAP { get; set; }
        public String DivisionId { get; set; }
        public String DivisionName { get; set; }
    }


    public class PPMPRevision 
    {
        public String Revision { get; set; }
    }
    public class PPMPDataApproval
    {
      public String Uacs { get; set; }
      public String Description { get; set; }
      public String Quantity { get; set; }
      public String EstimateBudget { get; set; }
      public String ModeOfProcurement { get; set; }
      public String Jan { get; set; }
      public String Feb { get; set; }
      public String Mar { get; set; }
      public String Apr { get; set; }
      public String May { get; set; }
      public String Jun { get; set; }
      public String Jul { get; set; }
      public String Aug { get; set; }
      public String Sep { get; set; }
      public String Octs { get; set; }
      public String Nov { get; set; }
      public String Dec { get; set; }
      public String Total { get; set; }
      public String Division { get; set; }
      public String Yearssss { get; set; }
      public String PAP { get; set; }
    }
   
  

    public class PPMPData 
    {
        public String ACTID { get; set; }
        public String UACS { get; set; }
        public String Description { get; set; }
        public String Allocation { get; set; }
        public String Balance { get; set; }
        public String Revised { get; set; }
        public String Jan { get; set; }
        public String Feb { get; set; }
        public String Mar { get; set; }
        public String Apr { get; set; }
        public String May { get; set; }
        public String Jun { get; set; }
        public String Jul { get; set; }
        public String Aug { get; set; }
        public String Sep { get; set; }
        public String Oct { get; set; }
        public String Nov { get; set; }
        public String Dec { get; set; }      
        public String _Year { get; set; }
        public String _Pap { get; set; }
     
    }
    public class PPMPFormat 

    {
        public String ACTID { get; set; }
        public String UACS { get; set; }
        public String Description { get; set; }
        public String Quantity_Size { get; set; }
        public String EstimatedBudget { get; set; }
        public String ModeOfProcurement { get; set; }
        public String Jan { get; set; }
        public String Feb { get; set; }
        public String Mar { get; set; }
        public String Apr { get; set; }
        public String May { get; set; }
        public String Jun { get; set; }
        public String Jul { get; set; }
        public String Aug { get; set; }
        public String Sep { get; set; }
        public String Oct { get; set; }
        public String Nov { get; set; }
        public String Dec { get; set; }
        public String _Total { get; set; }
        public String _Division { get; set; }
        public String _Year { get; set; }
        public String _Pap { get; set; }
     
    }

    public class PPMPDetails

    {
        public String ACTID { get; set; }
        public String UACS { get; set; }
        public String Description { get; set; }
        public String Quantity_Size { get; set; }
        public String EstimatedBudget { get; set; }
        public String ModeOfProcurement { get; set; }
        public String Jan { get; set; }
        public String Feb { get; set; }
        public String Mar { get; set; }
        public String Apr { get; set; }
        public String May { get; set; }
        public String Jun { get; set; }
        public String Jul { get; set; }
        public String Aug { get; set; }
        public String Sep { get; set; }
        public String Oct { get; set; }
        public String Nov { get; set; }
        public String Dec { get; set; }
        public String _Total { get; set; }
        public String _Division { get; set; }
        public String _Year { get; set; }
        public String _Pap { get; set; }
    }
    public class PPMPAlignment
    {
        public String fundsource { get; set; }
        public String mooe { get; set; }
        public String name { get; set; }
        public String from_uacs { get; set; }
        public String from_total { get; set; }
        public String to_uacs { get; set; }
        public String total_alignment { get; set; }
        public String months { get; set; }
    }

    public class SubExpendituresList
    {
        public int id { get; set; }
        public String name { get; set; }
        public Boolean is_for_ppmp { get; set; }
    }
}
