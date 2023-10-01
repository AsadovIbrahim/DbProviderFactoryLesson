using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace DbProviderFactoryLesson
{
    public partial class Form1 : Form
    {
        DbConnection? connection = null;
        DbProviderFactory? providerFactory = null;
        IConfigurationRoot? configuration = null;
        string providerName = string.Empty;

        public Form1()
        {
            InitializeComponent();
            Configuration();
        }
        private void Configuration()
        {

            DbProviderFactories.RegisterFactory("System.Data.SqlClient", typeof(SqlClientFactory));
            DbProviderFactories.RegisterFactory("System.Data.OleDb", typeof(OleDbFactory));

            configuration = new ConfigurationBuilder().AddJsonFile("AppSettings.json").Build();

        }

        private void btn_getAllProviders_Click(object sender, EventArgs e)
        {
            DataTable table = DbProviderFactories.GetFactoryClasses();
            //dataGridView1.DataSource = table;
            cmboxProviders.Items.Clear();
            foreach (DataRow row in table.Rows)
            {
                cmboxProviders.Items.Add(row["InvariantName"]).ToString();
            }
        }

        private void cmboxProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            providerName = cmboxProviders.SelectedItem.ToString()!;
            var connectionString = configuration.GetConnectionString(providerName);
            textConStr.Text = connectionString;
            providerFactory = DbProviderFactories.GetFactory(providerName);
            connection = providerFactory.CreateConnection();
            connection!.ConnectionString = connectionString;
        }

        private void btn_execute_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textRequest.Text) || (string.IsNullOrWhiteSpace(textConStr.Text)))
            {
                return;
            }
            using var command=connection!.CreateCommand();
            command.CommandText= textRequest.Text;

            var adapter = providerFactory!.CreateDataAdapter();
            adapter!.SelectCommand= command;

            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;
        }
    }
}