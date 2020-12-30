using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MagStoreRoom
{
    public partial class Home : Form
    {
        // creating connection to sql database
        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\MagStoreRoom\MagStoreRoom\Equipment.mdf;Integrated Security=True");
        public Home()
        {
            InitializeComponent();
            this.ActiveControl = SearchTxtBox;
        }
        public void display()
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            //sql query command show data in Equipment table
            cmd.CommandText = "select * from Equipment";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView.DataSource = dt;
        }

        private void Home_Load(object sender, EventArgs e)
        {
            // sql server connection test 
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            connection.Open();

            // Display all data in table on load
            display();

            // Only allow user to enter 1 charcter for each text boxes
            addShelfTxtBox.MaxLength = 1;
            addRowTxtBox.MaxLength = 1;
            addShelf2TxtBox.MaxLength = 1;
            addRow2TxtBox.MaxLength = 1;
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            int i = 0;
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            //sql query command to ensure item does not exist in database
            cmd.CommandText = "SELECT * FROM Equipment WHERE Item LIKE '%" + SearchTxtBox.Text + "%'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            i = Convert.ToInt32(dt.Rows.Count.ToString());
            if (i == 0)
            {
                MessageBox.Show("Item Not Found In Database");
                //display();
            }
            else
            {
                SqlCommand cmd1 = connection.CreateCommand();
                cmd1.CommandType = CommandType.Text;

                //sql query command show data in Equipment table
                cmd1.CommandText = "SELECT * FROM Equipment WHERE Item LIKE '%" + SearchTxtBox.Text + "%'";
                cmd1.ExecuteNonQuery();
                DataTable dt1 = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                da1.Fill(dt1);
                dataGridView.DataSource = dt1;
            }
        }

        //When clear search button is pressed
        private void SearchClearBtn_Click(object sender, EventArgs e)
        {
            // reset search box
            SearchTxtBox.Text = ("search...");
            this.ActiveControl = SearchTxtBox;
            updateBtn.Visible = false;
            display();
        }

        //add item to database
        private void addBtn_Click(object sender, EventArgs e)
        {
            int i = 0;
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            //sql query command to ensure item does not exist in database
            cmd.CommandText = "select * from Equipment where Item='" + addItemTxtBox.Text + "'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            i = Convert.ToInt32(dt.Rows.Count.ToString());
            if (i == 0)
            {
                // attempt to parse row text string to int
                int row, row2;
                bool canConvertRow = int.TryParse(addRowTxtBox.Text, out row);
                bool canConvertRow2 = int.TryParse(addRow2TxtBox.Text, out row2);

                if (canConvertRow == true && (canConvertRow2 == true || addRow2TxtBox.Text == ""))
                {
                    SqlCommand cmd1 = connection.CreateCommand();
                    cmd1.CommandType = CommandType.Text;

                    // insert textbox values into database
                    cmd1.CommandText = "insert into Equipment values('" + addItemTxtBox.Text.ToUpper() + "','" + addShelfTxtBox.Text.ToUpper() + "','"
                    + addRowTxtBox.Text + "','" + addShelf2TxtBox.Text.ToUpper() + "','" + addRow2TxtBox.Text + "')";
                    cmd1.ExecuteNonQuery();

                    //clear all text boxes
                    clearAddItemFields();
                    display();
                    // inform user item has been successfully entered
                    MessageBox.Show("Item Has Been Entered Into Database");
                }

                else
                {
                    // notify user about incorrect row input
                    MessageBox.Show("Row Input Must Be An Integar");
                    addRow2TxtBox.Clear();
                    addRowTxtBox.Clear();
                }
            }   
            else
            {
                //inform user that Item is not unique and already inside DB
                addItemTxtBox.Clear();
                MessageBox.Show("This Item Already Exist In The Database.");
            }
        }       

        private void addClearBtn_Click(object sender, EventArgs e)
        {
            // reset text fields
            clearAddItemFields();
            display();
        }
        private void clearAddItemFields()
        {
            // Reset add equipment text fields to default
            addItemTxtBox.Text = ("");
            addShelfTxtBox.Text = ("");
            addShelf2TxtBox.Text = ("");
            addRowTxtBox.Text = ("");
            addRow2TxtBox.Text = ("");
            //Focus text box
            this.ActiveControl = addItemTxtBox;
            updateBtn.Visible = false;
        }

        // populate add item text fields when cell is selected (for updating purpose)
        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // make update btn visible
            updateBtn.Visible = true;
            // get selected cell
            int i = Convert.ToInt32(dataGridView.SelectedCells[0].Value.ToString());

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;


            // get selected row in data table
            cmd.CommandText = "SELECT * FROM Equipment WHERE Id = '" + i + "'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            // add data from table to add item text fields
            foreach (DataRow dr in dt.Rows)
            {
                addItemTxtBox.Text = dr["Item"].ToString();
                addShelfTxtBox.Text = dr["Shelf"].ToString();
                addShelf2TxtBox.Text = dr["Shelf2"].ToString();
                addRowTxtBox.Text = dr["Row"].ToString();
                addRow2TxtBox.Text = dr["Row2"].ToString();
            }
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            int i = Convert.ToInt32(dataGridView.SelectedCells[0].Value.ToString());
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            // update sql statemnt to take all current add item tetx field text to update selected id 
            cmd.CommandText = "UPDATE Equipment set Item = '" + addItemTxtBox.Text + 
                "', Shelf = '" + addShelfTxtBox.Text + "', Row = '" + addRowTxtBox.Text + 
                "', Shelf2 = '" + addShelf2TxtBox.Text + "', Row2 = '" + addRow2TxtBox.Text + "' WHERE Id = '" + i + "'";
            cmd.ExecuteNonQuery();

            // update data grid and notify user that it was updated successfully
            display();
            MessageBox.Show("Item " + i + " Has Been Updated");
            clearAddItemFields();
        }

        private void deleteBtn_Click(object sender, EventArgs e)
		{
            // Ask user if they want to delete specifed item
            DialogResult dr =  MessageBox.Show("Are You Sure You Want To Delete This Item?", "Delete Item", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dataGridView.SelectedCells[0].Value.ToString());

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;

                // Sql query command to delete a row from database
                cmd.CommandText = "delete from Equipment where Id='" + id + "'";
                cmd.ExecuteNonQuery();

                MessageBox.Show("Item " + id + " Has Been Deleted!");

                // refresh UI with current tabel status
                display();
            }
        }

        // only allow numeric characters in text box
		private void addRowTxtBox_KeyPress(object sender, KeyPressEventArgs e)
		{
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }

        // only allow numeric characters in text box
        private void addRow2TxtBox_KeyPress(object sender, KeyPressEventArgs e)
		{
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }

        // only allow alphabatic charcaters in text box
		private void addShelfTxtBox_KeyPress(object sender, KeyPressEventArgs e)
		{
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        // only allow alphabatic charcaters in text box
        private void addShelf2TxtBox_KeyPress(object sender, KeyPressEventArgs e)
		{
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        //when search txt box is active and enter is pressed it will run the search query of text box 
		private void SearchTxtBox_KeyDown(object sender, KeyEventArgs e)
		{
            if (e.KeyCode == Keys.Enter)
            {
                SearchBtn_Click(this, new EventArgs());
            }
        }
    }
}
