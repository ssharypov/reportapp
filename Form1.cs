using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Reports
{
    public partial class Form1 : Form
    {
        string connStr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\_dev\\csharp\\solution1\\Reports\\Reports\\Database1.mdf;Integrated Security=True;Connect Timeout=5";
        public SqlConnection sqlConnection;
        public Dictionary<int, string> storeTypes = new Dictionary<int, string>();

        private Dictionary<string, string> reportParams = new Dictionary<string, string>();
        public DateTime startDate;
        public DateTime endDate;
        public string storeList = "";
        public string productList = "";
        


        public Form1()
        {
            InitializeComponent();
        }
        
        private async void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(endDate.ToString("yyyy-dd-MM"), "Начало периода", MessageBoxButtons.OK, MessageBoxIcon.Error);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                sqlConnection = new SqlConnection(connStr);
                await sqlConnection.OpenAsync();
                toolStripStatusLabel1.Text = "Подключение к БД установлено";
                storeTypes = await Queries.getStoreTypes(sqlConnection);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Нет подключения к БД";
                Thread blinkThread = new Thread(BlinkText);
                blinkThread.Start();
                blinkThread = null;
                //sqlConnection = null;
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BlinkText()
        {
            for(int i = 0; i < 10; i++) {
                System.Threading.Thread.Sleep(100);
                if (toolStripStatusLabel1.ForeColor != Color.Red)
                {
                    toolStripStatusLabel1.ForeColor = Color.Red;
                }
                else
                { 
                    toolStripStatusLabel1.ForeColor = Color.Black;
                }
                
            }
            
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 selectStores = new Form2(this);
            selectStores.LoadData(sqlConnection);
            selectStores.ShowDialog();
        }

        private async void сформироватьОтчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (startDate.ToString("yyyy-MM-dd")!="0001-01-01") reportParams.Add("startDate", startDate.ToString("yyyy-MM-dd"));
            if (endDate.ToString("yyyy-MM-dd") != "0001-01-01") reportParams.Add("endDate", endDate.ToString("yyyy-MM-dd"));
            if (storeList != "") reportParams.Add("storeList", storeList);
            if (productList != "") reportParams.Add("productList", productList);
            //MessageBox.Show(endDate.ToString("yyyy-dd-MM"), "test", MessageBoxButtons.OK, MessageBoxIcon.Error);
            dataGridView1.DataSource = await Queries.mainReport(sqlConnection, reportParams);
            reportParams.Clear();
        }
    }
}
