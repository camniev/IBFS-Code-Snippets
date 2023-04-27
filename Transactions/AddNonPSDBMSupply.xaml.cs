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
    public partial class AddNonPSDBMSupply : ChildWindow
    {
        public String _Year { get; set; }

        public String Expenditure_Type { get; set; }
        
        private MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private clsBudgetSupplies c_budgetsupplies = new clsBudgetSupplies();
        private clsProcurement c_procurement = new clsProcurement();
        String exp_type;
        //frmNonPSDBMSupplies frm_non_ps = new frmNonPSDBMSupplies();
        public AddNonPSDBMSupply()
        {
            InitializeComponent();
            //txtExpenditureID.Text = ExpenditureType;
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;
            c_procurement.SQLOperation += c_procurement_SQLOperation;
        }
        void c_budgetsupplies_SQLOperation(object sender, EventArgs e)
        {
            switch (c_budgetsupplies.Process)
            {
                case "SaveActivityItems":
                    //ClearData();
                    //FetchProcurementDetails();
                    break;
                case "Suspend":
                    //FetchProcurementDetails();
                    break;

            }
        }

        void c_procurement_SQLOperation(object sender, EventArgs e)
        {
            //switch (c_procurement.Process)
            //{
            //    case "SaveData":
            //        FetchProcurementLists();
            //        break;
            //}
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveActivityItems();
        }


        void svc_mindaf_ExecuteSQLCompleted(object sender, ExecuteSQLCompletedEventArgs e)
        {
            var _results = e.Result.ToString();

            switch (c_procurement.Process)
            {
                case "FetchSubCategory":
                    XDocument oDocKeyResultsFetchSubCategory = XDocument.Parse(_results);
                    var _dataListsFetchSubCategory = from info in oDocKeyResultsFetchSubCategory.Descendants("Table")
                                                     select new ProcurementSubCategory
                                                     {
                                                         sub_category = Convert.ToString(info.Element("sub_category").Value)
                                                     };

                    //cmbSubCategory.Items.Clear();

                    foreach (var item in _dataListsFetchSubCategory)
                    {
                        ProcurementSubCategory _varDetails = new ProcurementSubCategory();


                        _varDetails.sub_category = item.sub_category;

                        //cmbSubCategory.Items.Add(item.sub_category);

                    }

                    this.Cursor = Cursors.Arrow;

                    break;
                case "FetchUnitOfMeasure":
                    XDocument oDocKeyResultsFetchUnitOfMeasure = XDocument.Parse(_results);
                    var _dataListsFetchUnitOfMeasure = from info in oDocKeyResultsFetchUnitOfMeasure.Descendants("Table")
                                                     select new ProcurementUnitOfMeasure
                                                     {
                                                         unit_of_measure = Convert.ToString(info.Element("unit_of_measure").Value)
                                                     };

                    cmbUnit.Items.Clear();

                    foreach (var item in _dataListsFetchUnitOfMeasure)
                    {
                        ProcurementUnitOfMeasure _varDetails = new ProcurementUnitOfMeasure();


                        _varDetails.unit_of_measure = item.unit_of_measure;

                        cmbUnit.Items.Add(item.unit_of_measure);

                    }

                    this.Cursor = Cursors.Arrow;

                    break;
            }
        }

        private void FetchSubCategory()
        {
            c_procurement.Process = "FetchSubCategory";
            svc_mindaf.ExecuteSQLAsync(c_procurement.FetchSubCategory(_Year));
        }

        private void FetchUnitOfMeasure()
        {
            c_procurement.Process = "FetchUnitOfMeasure";
            svc_mindaf.ExecuteSQLAsync(c_procurement.FetchUnitOfMeasure(_Year));
        }

        private void SaveActivityItems()
        {
            switch (txtExpenditureID.Text)
            {
                case "ICT Office Equipment": exp_type = "EXP-ICTOE"; break;
                case "Office Supplies Expenses": exp_type = "EXP-OS"; break;
                case "ICT Office Supplies": exp_type = "EXP-ICT"; break;
                case "Other Supplies and Materials Expenses": exp_type = "EXP-OTS"; break;
            }
            //c_budgetsupplies.SaveNonPSDBMSupply(txtItemName.Text, cmbSubCategory.SelectionBoxItem.ToString(), cmbUnit.SelectionBoxItem.ToString(), Convert.ToDouble(txtUnitPrice.Text), cmbExpenditureID.SelectionBoxItem.ToString(), this._Year);
            c_budgetsupplies.SaveNonPSDBMSupply(txtItemName.Text, cmbSubCategory.SelectionBoxItem.ToString(), cmbUnit.SelectionBoxItem.ToString(), Convert.ToDouble(txtUnitPrice.Text), exp_type, this._Year);
            if (MessageBox.Show("Supply successfully saved.", "Information", MessageBoxButton.OK) == System.Windows.MessageBoxResult.OK)
            {
                //FetchProcurementItems();
                //frm_non_ps.RefreshData();
            }
        }

        private void AddNonPSDBMSupply_MouseEnter(object sender, MouseEventArgs e)
        {
            txtExpenditureID.Text = Expenditure_Type;
        }

        private void frmAddNonPSDBMSupply_Loaded(object sender, RoutedEventArgs e)
        {
            //FetchSubCategory();
            FetchUnitOfMeasure();
        }
    }
}

