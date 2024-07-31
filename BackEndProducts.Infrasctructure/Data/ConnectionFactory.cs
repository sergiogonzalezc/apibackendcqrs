using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Infraestructure.Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public ConnectionFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IDbConnection GetConnection
        {
            get
            {
                SqlConnection sqlConnection = new SqlConnection();

                if (sqlConnection == null)
                {
                    return null;
                }

                sqlConnection.ConnectionString = this._configuration.GetConnectionString("stringConnection");

                sqlConnection.Open();

                return sqlConnection;
            }
        }
    }
}