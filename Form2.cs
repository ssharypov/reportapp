using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Reports
{
    
    public partial class Form2 : Form
    {
        SqlConnection conn;
        private Dictionary<int, string> storeTypes;
        private Dictionary<int, string> stores;
        private Form1 parentForm;
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(Form1 baseForm)
        {
            InitializeComponent();
            this.parentForm = baseForm;
        }
        public void LoadData(SqlConnection sqlConnection)
        {
            conn = sqlConnection;
        }
        private async void Form2_Load(object sender, EventArgs e)
        {   
            try
            {
                storeTypes = await Queries.getStoreTypes(conn);
                comboBox1.Items.Insert(0, "Все");
                foreach (var typeOfStore in storeTypes)
                {
                    comboBox1.Items.Add(typeOfStore.Value);
                }
                try
                {
                    stores = await Queries.getStoreList(conn, "Все");
                    var bindSource = new BindingSource
                    {
                        DataSource = stores.ToList(),
                    };
                    dataGridView1.DataSource = bindSource;
                    dataGridView1.Columns["Key"].Visible = false;
                    dataGridView1.ColumnHeadersVisible = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = comboBox1.SelectedItem;
            //MessageBox.Show("Значение измененео на " + item.ToString(), "кликнуто", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (item != null)
            {
                try
                {
                    stores = await Queries.getStoreList(conn, item.ToString());
                    var bindSource = new BindingSource
                    {
                        DataSource = stores.ToList(),
                    };
                    dataGridView1.DataSource = bindSource;
                    dataGridView1.Columns["Key"].Visible = false;
                    dataGridView1.ColumnHeadersVisible = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(dateTimePicker1.Value.Date.ToString("yyyy-dd-MM"), "Начало периода", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //MessageBox.Show("test", "test", MessageBoxButtons.OK, MessageBoxIcon.Error);
            parentForm.startDate = dateTimePicker1.Value;
            
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            parentForm.endDate = dateTimePicker2.Value;
        }

        private async void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            var item = comboBox1.SelectedItem;
            if (item != null)
            {
                try
                {
                    stores = await Queries.getStoreList(conn, item.ToString());
                    var bindSource = new BindingSource
                    {
                        DataSource = stores.ToList(),
                    };
                    dataGridView1.DataSource = bindSource;
                    dataGridView1.Columns["Key"].Visible = false;
                    dataGridView1.ColumnHeadersVisible = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.Clicks == 2 && e.Button == MouseButtons.Left)
            {
                if(e.RowIndex > 0)
                {
                    DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                    DataGridViewRow newRow = (DataGridViewRow)selectedRow.Clone();
                    for(int i=0;i<selectedRow.Cells.Count;i++)
                    {
                        newRow.Cells[i].Value = selectedRow.Cells[i].Value;
                    }
                    if (dataGridView2.ColumnCount == 0)
                    {
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            dataGridView2.Columns.Add((DataGridViewColumn)column.Clone());
                        }
                    }
                    dataGridView2.Rows.Add(newRow);
                }
            }
        }
    }
}
