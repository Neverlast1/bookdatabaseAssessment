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
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;

namespace BookDatabase
{

    public partial class BookList : Window
    {
        private OleDbConnection Connection;
        private OleDbCommand command;
        private OleDbDataAdapter daBooks; //daBooks
        private DataSet dsPublisher;

        public BookList(string publisherCode)
        {
            InitializeComponent();
            //Connexts to the database
            string strConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Books.mdb";

            try
            {
                Connection = new OleDbConnection(strConnection);
                Connection.Open();
                dsPublisher = new DataSet();
                //SQL Query that Displays Books published by comparing against the variable publisherCode
                //which is the currently selected publisher.
                string queryString = "SELECT Publisher.PublisherCode, Book.Title FROM Publisher, Book " +
                 "WHERE Publisher.PublisherCode = Book.PublisherCode " +
                 "AND Publisher.PublisherCode LIKE " + "'" + publisherCode + "'" + ";";

                command = new OleDbCommand(queryString, Connection);
                daBooks = new OleDbDataAdapter(queryString, Connection);
                daBooks.Fill(dsPublisher, "Publisher");

                dgBookList.DataContext = dsPublisher.Tables["Publisher"];
                dgBookList.ItemsSource = dsPublisher.Tables["Publisher"].AsDataView();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
