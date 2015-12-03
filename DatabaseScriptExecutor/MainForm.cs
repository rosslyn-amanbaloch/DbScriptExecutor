using System;
using System.Windows.Forms;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            txtConnectionString.Text = ConfigurationManager.ConnectionStrings["RAMaster"].ConnectionString;
            txtSql.Text = ConfigurationManager.AppSettings["sqlString"];
        }

        private async void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                btnExecute.Enabled = false;

                UpdateInfo($"Script execution triggered {DateTime.Now}");

                var connectionString = txtConnectionString.Text;
                var sql = txtSql.Text;
                //var dbs = "SELECT name FROM sys.databases where name NOT IN('master', 'model', 'msdb', 'tempdb') AND sys.databases.state = 0".Query<string>(connectionString).ToList();
                var dbs = (await _formService.GetDbNamesAsync(connectionString)).ToList();

                if (!dbs.Any()) return;

                //foreach (var dbName in 
                Parallel.ForEach(dbs, async dbName =>
                {
                    try
                    {
                        await _formService.ExecuteSqlAsync(connectionString, sql);
                        //string.Format(txtSql.Text, dbName).Execute(connectionString);
                    }
                    catch (Exception ex)
                    {
                        UpdateInfo(ex.ToString());

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
    }
}
