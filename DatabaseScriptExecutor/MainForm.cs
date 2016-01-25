using System;
using System.Windows.Forms;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseScriptExecutor
{
    public partial class MainForm : Form
    {
        private readonly IFormService _formService;
        private CancellationTokenSource _tokenSource;

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
                if (chkDbNames.CheckedItems.Count == 0)
                {
                    MessageBox.Show("No db selected");
                }
                btnExecute.Enabled = false;

                UpdateInfo($"Script execution triggered {DateTime.Now}");

                var connectionString = txtConnectionString.Text;
                var sql = txtSql.Text;

                var dbs = chkDbNames.CheckedItems.Cast<string>().ToList();

                if (!dbs.Any()) return;

                var dbCount = 1;
                _tokenSource = new CancellationTokenSource();
                btnCancel.Enabled = true;

                await Task.Run(() =>
                {
                    try
                    {
                        dbs.AsParallel().AsOrdered().WithCancellation(_tokenSource.Token).ForAll(dbName =>
                        {
                            try
                            {
                                UpdateInfo($"({dbCount++}/{dbs.Count}) Executing script on {dbName} {DateTime.Now}");
                                _formService.ExecuteSqlAsync(connectionString, dbName, sql).Wait();
                            }
                            catch (Exception ex)
                            {
                                UpdateInfo(ex.ToString());
                            }
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        UpdateInfo($"Script execution cancelled {DateTime.Now}");
                    }
                });
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _tokenSource?.Cancel();
            UpdateInfo($"Script cancellation requested {DateTime.Now}");
            btnCancel.Enabled = false;
        }
    }
}