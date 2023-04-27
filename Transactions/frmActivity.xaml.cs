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
    public partial class frmActivity : ChildWindow
    {
        MinDAFSVCClient svc_mindaf = new MinDAFSVCClient();
        private clsActivity c_activity = new clsActivity();
        private List<ActivityUserData> ListActivityUser = new List<ActivityUserData>();
        private List<Categories> ListCategory = new List<Categories>();
        private List<ActivityDetails> ListActivity = new List<ActivityDetails>();

        public String OutputID { get; set; }
        public String DivisionID { get; set; }
        public frmActivity()
        {
            InitializeComponent();
            svc_mindaf.ExecuteSQLCompleted += svc_mindaf_ExecuteSQLCompleted;
        }

        void svc_mindaf_ExecuteSQLCompleted(object sender, ExecuteSQLCompletedEventArgs e)
        {
            var _results = e.Result.ToString();
            switch (c_activity.Process)
            {
                case "FetchAssignedEmployee":
                      XDocument oDocKeyResults = XDocument.Parse(_results);
                   var _dataLists = from info in oDocKeyResults.Descendants("Table")
                                    select new ActivityUserData
                                    {
                                        Id = Convert.ToString(info.Element("Id").Value),
                                        Fullname = Convert.ToString(info.Element("Fullname").Value)
                                    };

                   ListActivityUser.Clear();
                   cmbAccountableMember.Items.Clear();

                   foreach (var item in _dataLists)
                   {
                       ActivityUserData _varDetails = new ActivityUserData();


                       _varDetails.Id = item.Id;
                       _varDetails.Fullname = item.Fullname;


                       ListActivityUser.Add(_varDetails);
                       cmbAccountableMember.Items.Add(item.Fullname);
                   }
               
                   this.Cursor = Cursors.Arrow;

                   LoadCategories();
                    break;
                case "FetchCategory":
                    XDocument oDocFetchCategory = XDocument.Parse(_results);
                    var _dataListsFetchCategory = from info in oDocFetchCategory.Descendants("Table")
                                    select new Categories
                                    {
                                        id = Convert.ToString(info.Element("id").Value),
                                        name = Convert.ToString(info.Element("name").Value),
                                        weight = Convert.ToString(info.Element("weight").Value),
                                    };

                    ListCategory.Clear();
                   cmbMemberCategory.Items.Clear();

                   foreach (var item in _dataListsFetchCategory)
                   {
                       Categories _varDetails = new Categories();


                       _varDetails.id = item.id;
                       _varDetails.name = item.name;
                       _varDetails.weight = item.weight;

                       ListCategory.Add(_varDetails);
                       cmbMemberCategory.Items.Add(item.name);
                   }
               
                   this.Cursor = Cursors.Arrow;

                   LoadActivity();
                    break;
                case "FetchActivity":
                    XDocument oDocFetchActivity = XDocument.Parse(_results);
                    var _dataListsFetchActivity = from info in oDocFetchActivity.Descendants("Table")
                                                  select new ActivityDetails
                                                  {
                                                      accountable_member = Convert.ToString(info.Element("accountable_member").Value),
                                                      completion_rate = Convert.ToInt32(info.Element("completion_rate").Value),
                                                      date_end = Convert.ToDateTime(info.Element("date_end").Value),
                                                      date_start = Convert.ToDateTime(info.Element("date_start").Value),
                                                      description = Convert.ToString(info.Element("description").Value),
                                                      id = Convert.ToString(info.Element("id").Value),
                                                      is_approved = Convert.ToBoolean(info.Element("is_approved").Value),
                                                      member_category = Convert.ToString(info.Element("member_category").Value),
                                                      output_id = Convert.ToString(info.Element("output_id").Value),
                                                      support_needed = Convert.ToString(info.Element("support_needed").Value),
                                                      weight = Convert.ToDecimal(info.Element("weight").Value)
                                                  };

                    ListActivity.Clear();


                    foreach (var item in _dataListsFetchActivity)
                    {
                        ActivityDetails _varDetails = new ActivityDetails();


                        _varDetails.accountable_member = item.accountable_member;
                        _varDetails.completion_rate = item.completion_rate;
                        _varDetails.weight = item.weight;
                        _varDetails.date_end = item.date_end;
                        _varDetails.date_start = item.date_start;
                        _varDetails.description = item.description;
                        _varDetails.id = item.id;
                        _varDetails.is_approved = item.is_approved;
                        _varDetails.member_category = item.member_category;
                        _varDetails.output_id = item.output_id;
                        _varDetails.support_needed = item.support_needed;


                        ListActivity.Add(_varDetails);
                    }
                    if (ListActivity.Count!=0)
                    {
                        grdData.ItemsSource = null;
                        grdData.ItemsSource = ListActivity;
                        grdData.Columns["id"].Visibility = System.Windows.Visibility.Collapsed;
                        grdData.Columns["output_id"].Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        grdData.ItemsSource = null;
                    }
                    this.Cursor = Cursors.Arrow;


                    break;
            }   
        }

       
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        
        private void EnableControls(bool _val) 
        {
            txtCompletionRate.IsEnabled = _val;
            txtDescription.IsEnabled = _val;
            txtSupportNeeded.IsEnabled = _val;
            txtWeight.IsEnabled = _val;
            cmbAccountableMember.IsEnabled = _val;
            cmbMemberCategory.IsEnabled = _val;
            cmbStatus.IsEnabled = _val;
            btnCancel.IsEnabled = _val;
            dteDateEnd.IsEnabled = _val;
            dteStartDate.IsEnabled = _val;
        }

        private void LoadStatus() 
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Completed");
            cmbStatus.Items.Add("Deffered");
            cmbStatus.Items.Add("In Progress");
            cmbStatus.Items.Add("Not Started");
        }
        private void ResetForm()
        {
            txtCompletionRate.Value = 0;
            txtDescription.Text = "";
            txtSupportNeeded.Text = "";
            txtWeight.Value = 0;

            cmbAccountableMember.SelectedIndex = -1;
            cmbMemberCategory.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            dteDateEnd.SelectedDate = null;
            dteStartDate.SelectedDate = null;

        }
        private void frmactivity_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStatus();
            LoadEmployeeAssigned();
        }
        private void LoadEmployeeAssigned() 
        {
            c_activity.Process = "FetchAssignedEmployee";
            svc_mindaf.ExecuteSQLAsync(c_activity.FetchEmployees(this.DivisionID));
        }
        private void LoadCategories()
        {
            c_activity.Process = "FetchCategory";
            svc_mindaf.ExecuteSQLAsync(c_activity.FetchCategory());
        }

        private void LoadActivity() 
        {
            c_activity.Process = "FetchActivity";
            svc_mindaf.ExecuteSQLAsync(c_activity.FetchActivity(this.OutputID));
        }

        private void SaveActivity() 
        {
            String  output_id =OutputID;
            String  description = txtDescription.Text;
            String  accountable_member_id ="";
            String  member_category_id = "";
            String  weight = txtWeight.Value.ToString();
            DateTime  date_start = Convert.ToDateTime(dteStartDate.SelectedDate);
            DateTime  date_end = Convert.ToDateTime(dteDateEnd.SelectedDate);
            String  completion_rate = txtCompletionRate.Value.ToString();
            String  support_needed= txtSupportNeeded.Text;

             List<ActivityUserData> y = ListActivityUser.Where(item => item.Fullname == cmbAccountableMember.SelectedItem.ToString()).ToList();
              foreach (var item in y)
               {
                           accountable_member_id = item.Id;
               }
             List<Categories> x = ListCategory.Where(item => item.name == cmbMemberCategory.SelectedItem.ToString()).ToList();
              foreach (var item in y)
               {
                           member_category_id = item.Id;
               }
              c_activity.Process = "SaveActivity";
              c_activity.SQLOperation += c_activity_SQLOperation;
              c_activity.SaveOutputActivity(output_id, description, accountable_member_id, member_category_id, weight, date_start, date_end, completion_rate, support_needed);

        }


        void c_activity_SQLOperation(object sender, EventArgs e)
        {
            switch (c_activity.Process)
            {
                case "SaveActivity":
                    MessageBox.Show("Data Save");
                    LoadActivity();
                    ResetForm();
                    EnableControls(false);
                    break;
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            switch (btnSave.Content.ToString())
            {
                case "New":
                    btnSave.Content = "Save";
                    
                    EnableControls(true);
                    break;
                case "Save":
                    btnSave.Content = "New";
                    SaveActivity();
                    EnableControls(false);
                    break;
                default:
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnSave.Content = "New";
            EnableControls(false);
        }

    }
}

