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
        public Form1()
        {
            InitializeComponent();

            dgvBook.RowTemplate.Height = 30;
            dgvBook.RowHeadersVisible = false;
            dgvBook.Columns[0].Visible = false;

            dgvBook.CellClick += dataGridViewSoftware_CellClick;
            txtSearch.TextChanged += txtSearchSoftware_TextChanged;
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
                mySqlCmd.Parameters.AddWithValue("_Description", txtDescripiton.Text.Trim());
                mySqlCmd.ExecuteNonQuery();
                //MessageBox.Show("Submitted Successfully");
                Clear();
                GridFill();
            }
        }

        void Clear()
        {
            txtBookName.Text = txtAuthor.Text = txtDescripiton.Text = txtSearch.Text = "";
            bookID = 0;
            btnSave.Text = "Save";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void dataGridViewSoftware_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //update
                if (e.ColumnIndex == 0)
                {
                    //dgvBook.Columns["update"].Index
                    if (dgvBook.CurrentRow.Index != -1)
                    {
                        txtBookName.Text = dgvBook.CurrentRow.Cells[1].Value.ToString();
                        txtAuthor.Text = dgvBook.CurrentRow.Cells[2].Value.ToString();
                        txtDescripiton.Text = dgvBook.CurrentRow.Cells[3].Value.ToString();
                        bookID = Convert.ToInt32(dgvBook.CurrentRow.Cells[0].Value.ToString());
                        btnSave.Text = "Update";
                    }
                }
                //delete
                else if (e.ColumnIndex == 1)
                {
                    using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
                    {
                        bookID = Convert.ToInt32(dgvBook.CurrentRow.Cells[0].Value.ToString());

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
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void txtSearchSoftware_TextChanged(object sender, EventArgs e)
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(connectionString))
            {
                mysqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("BookSearchByValue", mysqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("_SearchValue", txtSearch.Text);
                DataTable dtblBook = new DataTable();
                sqlDa.Fill(dtblBook);
                dgvBook.DataSource = dtblBook;
                dgvBook.Columns[0].Visible = false;
            }
        }
    }
}
