using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace MySqlCRUD
{
    public partial class Form1 : Form
    {
        string connectionString = @"Server=localhost;Database=bookdb;Uid=root;Pwd=00000000;";
        int bookID = 0;
        Dictionary<int, string> dic;

        public Form1()
        {
            InitializeComponent();
            dgvBook.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBook.RowTemplate.Height = 30;
            dgvBook.RowHeadersVisible = false;
            dgvBook.Columns[0].Visible = false;

            dic = new Dictionary<int, string>
            {
                { 1, "Big" },
                { 2, "Small" }
            };
            comboBox1.DataSource = new BindingSource(dic, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.SelectedIndex = -1;

            dgvBook.CellClick += dataGridViewSoftware_CellClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();

            GridFill();
        }

        void GridFill()
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
            {
                mysqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("BookViewAll", mysqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtblBook = new DataTable();
                sqlDa.Fill(dtblBook);
                dgvBook.DataSource = dtblBook;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
            {
                mysqlCon.Open();
                MySqlCommand mySqlCmd = new MySqlCommand("BookAddOrEdit", mysqlCon);
                mySqlCmd.CommandType = CommandType.StoredProcedure;
                mySqlCmd.Parameters.AddWithValue("_BookID", bookID);
                mySqlCmd.Parameters.AddWithValue("_BookName", txtBookName.Text.Trim());
                mySqlCmd.Parameters.AddWithValue("_Author", txtAuthor.Text.Trim());
                mySqlCmd.Parameters.AddWithValue("_DateBook", dateBook.Value);
                mySqlCmd.Parameters.AddWithValue("_TypeBook", comboBox1.SelectedValue);
                mySqlCmd.Parameters.AddWithValue("_Free", checkBox1.Checked);
                mySqlCmd.Parameters.AddWithValue("_Description", txtDescripiton.Text.Trim());
                mySqlCmd.ExecuteNonQuery();
                //MessageBox.Show("Submitted Successfully");
                Clear();
                GridFill();
            }
        }

        void Clear()
        {
            txtBookName.Text = txtAuthor.Text = txtDescripiton.Text = tbSearch.Text = "";
            bookID = 0;
            btSave.Text = "Save";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void dataGridViewSoftware_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int cell = 2;

                //update           
                if (e.ColumnIndex == 0)
                {
                    if (dgvBook.CurrentRow.Index != -1)
                    {
                        txtBookName.Text = dgvBook.CurrentRow.Cells[cell + 1].Value.ToString();
                        txtAuthor.Text = dgvBook.CurrentRow.Cells[cell + 2].Value.ToString();
                        dateBook.Text = dgvBook.CurrentRow.Cells[cell + 3].Value.ToString();
                        comboBox1.SelectedValue = dgvBook.CurrentRow.Cells[cell + 4].Value;
                        checkBox1.Checked = Convert.ToInt32(dgvBook.CurrentRow.Cells[cell + 5].Value) == 1 ? true : false;
                        txtDescripiton.Text = dgvBook.CurrentRow.Cells[cell + 6].Value.ToString();
                        bookID = Convert.ToInt32(dgvBook.CurrentRow.Cells[cell + 0].Value.ToString());
                        btSave.Text = "Update";
                    }
                }
                //delete
                else if (e.ColumnIndex == 1)
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to delete " + dgvBook.CurrentRow.Cells[cell + 1].Value + "book?", "Delete book", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
                        {
                            bookID = Convert.ToInt32(dgvBook.CurrentRow.Cells[cell + 0].Value.ToString());

                            mysqlCon.Open();
                            MySqlCommand mySqlCmd = new MySqlCommand("BookDeleteByID", mysqlCon);
                            mySqlCmd.CommandType = CommandType.StoredProcedure;
                            mySqlCmd.Parameters.AddWithValue("_BookID", bookID);
                            mySqlCmd.ExecuteNonQuery();
                            //MessageBox.Show("Deleted Successfully");

                            Clear();
                            GridFill();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btSearsh_Click(object sender, EventArgs e)
        {
            SearshFilter();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                SearshFilter();
            //else if(string.IsNullOrWhiteSpace(tbSearch.Text))
            //    GridFill();
        }

        private void SearshFilter()
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
            {
                mysqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("BookSearchByValue", mysqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("_SearchValue", tbSearch.Text);
                DataTable dtblBook = new DataTable();
                sqlDa.Fill(dtblBook);
                dgvBook.DataSource = dtblBook;
            }
        }

        private void dgvBook_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                string name = dgvBook.Columns[e.ColumnIndex].DataPropertyName;
                switch (name)
                {
                    case "DateBook":
                        if (e.Value != null && DateTime.TryParse(e.Value.ToString(), out DateTime dateTime))
                            e.Value = dateTime.ToShortDateString();
                        else
                            e.Value = "";

                        break;

                    case "TypeBook":
                        if (e.Value != null && Convert.ToInt32(e.Value) > 0)
                            e.Value = dic[Convert.ToInt32(e.Value)];
                        else
                            e.Value = "";

                        break;

                    case "Free":

                        if (e.Value == null || Convert.ToInt32(e.Value) == 0)
                            e.Value = "Not free";
                        else if (e.Value != null && Convert.ToInt32(e.Value) == 1)
                            e.Value = "Free";

                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
    }
}
