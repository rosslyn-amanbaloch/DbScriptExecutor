using System;
using System.Windows.Forms;
using RapidCommon.Sql;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseScriptExecutor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtConnectionString.Text = ConfigurationManager.ConnectionStrings["RAMaster"].ConnectionString;
            txtSql.Text = ConfigurationManager.AppSettings["sqlString"];
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            var connectionString = txtConnectionString.Text;
            var dbs = "SELECT name FROM sys.databases where name NOT IN('master', 'model', 'msdb', 'tempdb') AND sys.databases.state = 0".Query<string>(connectionString).ToList();

            if (!dbs.Any()) return;

            //foreach (var dbName in 
            Parallel.ForEach(dbs, dbName =>
            {
                try
                {
                    string.Format(txtSql.Text, dbName).Execute(connectionString);
                }
                catch (Exception ex)
                {
                    if (lblResult.InvokeRequired)
                    {
                        var invokeAction = new Action(() => { lblResult.Text += "\r\n\r\n" + ex; });
                        Invoke(invokeAction);
                    }
                    else
                    {
                        lblResult.Text += "\r\n\r\n" + ex;
                    }

                }
            });

            lblResult.Text += "\r\n\r\nFinished";
        }
    }
}
