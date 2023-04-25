using Dapper;
using MathEquation.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEquation.Business.Data
{
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DatabaseInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(
                @"CREATE TABLE IF NOT EXISTS maths_equations (
            id INTEGER PRIMARY KEY,
            equation TEXT NOT NULL,
            x_value REAL NOT NULL,
            y_value REAL NOT NULL,
            date_completed DATETIME NOT NULL)"
                );
        }
    }
}
