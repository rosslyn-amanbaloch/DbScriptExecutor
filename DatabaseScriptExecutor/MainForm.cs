using System;
using System.Windows.Forms;
using System.Configuration;
using System.Linq;

namespace DatabaseScriptExecutor
{
    public partial class MainForm : Form
    {
        private readonly IFormService _formService;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(IFormService formService) : this()
        {
            _formService = formService;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtConnectionString.Text = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            txtSql.Text = ConfigurationManager.AppSettings["DefaultSqlQuery"];
        }

        private async void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkDbNames.SelectedItems.Count == 0)
                {
                    MessageBox.Show("No db selected");
                }
                btnExecute.Enabled = false;

                UpdateInfo($"Script execution triggered {DateTime.Now}");

                var connectionString = txtConnectionString.Text;
                var sql = txtSql.Text;

                var dbs = chkDbNames.CheckedItems.Cast<string>().ToList();

                if (!dbs.Any()) return;

                //TODO: add parallelization
                //var taskList = new List<Task>();

                var dbCount = 1;
                dbs.ForEach(async dbName =>
                {
                    try
                    {
                        UpdateInfo($"({dbCount++}/{dbs.Count}) Executing script on {dbName} {DateTime.Now}");
                        await _formService.ExecuteSqlAsync(connectionString, dbName, sql);
                    }
                    catch (Exception ex)
                    {
                        UpdateInfo(ex.ToString());
                    }
                });

                //await Task.WhenAll(taskList);

            }
            finally
            {
                UpdateInfo($"Script execution finished {DateTime.Now}");
                btnExecute.Enabled = true;
            }
        }

        private void UpdateInfo(string message)
        {
            if (lblResult.InvokeRequired)
            {
                var invokeAction = new Action(() => { lblResult.Text += "\r\n\r\n" + message; });
                Invoke(invokeAction);
            }
            else
            {
                lblResult.Text += "\r\n\r\n" + message;
            }
        }

        private async void btnGetDbNames_Click(object sender, EventArgs e)
        {
            chkDbNames.Items.Clear();
            var connectionString = txtConnectionString.Text;
            UpdateInfo($"Querying server for databases...");
            var dbs = (await _formService.GetDbNamesAsync(connectionString)).ToList();
            UpdateInfo($"{dbs.Count} database(s) found.");
            dbs.ForEach(d => chkDbNames.Items.Add(d));
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            ChangeChecks(true);
        }

        private void ChangeChecks(bool isChecked)
        {
            for (var i = 0; i < chkDbNames.Items.Count; i++)
            {
                chkDbNames.SetItemChecked(i, isChecked);
            }
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            ChangeChecks(false);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lblResult.Text = string.Empty;
        }
    }
}