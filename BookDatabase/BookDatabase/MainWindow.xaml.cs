using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;

namespace BookDatabase
{
    /// This program was created by Joel Busch and was last edited on 22/08/2019 by Joel Busch
    /// This program allows a user to navigate through a Book database, and edit the database by
    /// Editing records, deleting records and creating new records.
    /// As well as open a new window listing books by Publisher code.
    public partial class MainWindow : Window
    {
        private OleDbConnection Connection;
        private OleDbCommand command;
        private OleDbDataAdapter daBooks; //daBooks
        private DataSet dsSample;
        private OleDbCommandBuilder builder;
        private int rowIndex = 0;
        private Boolean isNewRow;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                string strConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Books.mdb";
                Connection = new OleDbConnection(strConnection);
                Connection.Open();
                string strQuery = "SELECT [BookCode], [Title], [PublisherCode], [Type], [Price], [Paperback] FROM Book";
                command = new OleDbCommand(strQuery, Connection);
                daBooks = new OleDbDataAdapter(strQuery, Connection);
                builder = new OleDbCommandBuilder(daBooks);
                builder.QuotePrefix = "[";
                builder.QuoteSuffix = "]";
                dsSample = new DataSet();
                daBooks.Fill(dsSample, "Book");
                viewMode();
                DisplayRecord();





                Connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void DisplayRecord()
        {
            txtBookCode.Text = Convert.ToString(dsSample.Tables["Book"].Rows[rowIndex]["BookCode"]);
            txtTitle.Text = Convert.ToString(dsSample.Tables["Book"].Rows[rowIndex]["Title"]);
            txtPubCode.Text = Convert.ToString(dsSample.Tables["Book"].Rows[rowIndex]["PublisherCode"]);
            txtType.Text = Convert.ToString(dsSample.Tables["Book"].Rows[rowIndex]["Type"]);
            txtPrice.Text = Convert.ToString(dsSample.Tables["Book"].Rows[rowIndex]["Price"]);
            chkPaperback.IsChecked = Convert.ToBoolean(dsSample.Tables["Book"].Rows[rowIndex]["Paperback"]);
        }
        private void viewMode()
        {
            grpBooks.IsEnabled = false;
            grpNavigation.IsEnabled = true;
            grpActions.IsEnabled = true;
            grpUpdate.IsEnabled = false;
        }
        private void upDateMode()
        {
            grpBooks.IsEnabled = true;
            grpNavigation.IsEnabled = false;
            grpActions.IsEnabled = false;
            grpUpdate.IsEnabled = true;
        }



        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            {
                if (dsSample.Tables["Book"].Rows.Count > 0)
                {
                    rowIndex = 0;
                    DisplayRecord();
                }
            }

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //check that there are records in the dataset
            if (dsSample.Tables["Book"].Rows.Count > 0)
                //check that we haven’t reached the end of the table
                if (rowIndex < dsSample.Tables["Book"].Rows.Count - 1)
                {
                    rowIndex++;
                    DisplayRecord();
                }

        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (rowIndex > 0)
            {
                rowIndex--;
                DisplayRecord();

            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            rowIndex = dsSample.Tables["Book"].Rows.Count - 1;
            DisplayRecord();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtBookCode.Clear();
            txtPrice.Clear();
            txtTitle.Clear();
            txtType.Clear();
            txtPubCode.Clear();
            chkPaperback.IsChecked = false;
            upDateMode();
            txtBookCode.Focus();
            isNewRow = true;

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //set up the form to edit the currently displayed record
            upDateMode();
            //set focus to first input field
            txtBookCode.Focus();
            //flag that an edit is in progress
            isNewRow = false;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (isNewRow)
            {
                DataRow newRow = dsSample.Tables["Book"].NewRow();
                newRow["BookCode"] = txtBookCode.Text;
                newRow["Title"] = txtTitle.Text;
                newRow["PublisherCode"] = txtPubCode.Text;
                newRow["Type"] = txtType.Text;
                newRow["Price"] = Convert.ToDouble(txtPrice.Text);
                newRow["Paperback"] = chkPaperback.IsChecked;
                dsSample.Tables["Book"].Rows.Add(newRow);
                rowIndex = dsSample.Tables["Book"].Rows.Count - 1;
            }
            else
            {
                dsSample.Tables["Book"].Rows[rowIndex].BeginEdit();
                dsSample.Tables["Book"].Rows[rowIndex]["BookCode"] = txtBookCode.Text;
                dsSample.Tables["Book"].Rows[rowIndex]["Title"] = txtTitle.Text;
                dsSample.Tables["Book"].Rows[rowIndex]["PublisherCode"] = txtPubCode.Text;
                dsSample.Tables["Book"].Rows[rowIndex]["Type"] = txtType.Text;
                dsSample.Tables["Book"].Rows[rowIndex]["Price"] = Convert.ToDouble(txtPrice.Text);
                dsSample.Tables["Book"].Rows[rowIndex]["Paperback"] = chkPaperback.IsChecked; ;
                dsSample.Tables["Book"].Rows[rowIndex].EndEdit();
            }
            daBooks.UpdateCommand = builder.GetUpdateCommand();
            daBooks.Update(dsSample, "Book");
            viewMode();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //cancel all edits on current row
            dsSample.Tables["Book"].Rows[rowIndex].CancelEdit();
            //return form to view mode
            viewMode();
            //display row
            DisplayRecord();

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show("Delete this Book?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    //delete the currently displayed record

                    dsSample.Tables["Book"].Rows[rowIndex].Delete();

                    //update changes made to dataset back to database

                    daBooks.UpdateCommand = builder.GetUpdateCommand();

                    daBooks.Update(dsSample, "Book");

                    //move to displayable record

                    if (rowIndex > dsSample.Tables["Book"].Rows.Count - 1)
                        rowIndex -= 1;


                    //display row

                    DisplayRecord();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void BtnBookList_Click(object sender, RoutedEventArgs e)
        {
            //Captures and stores currently selected Publisher code and sends it to BookList
            string publisherCode = Convert.ToString(txtPubCode.Text);
            BookList winBookList = new BookList(publisherCode);
            winBookList.Show();
        }
    }
}
