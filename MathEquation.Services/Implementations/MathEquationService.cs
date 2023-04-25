using Dapper;
using MathEquation.Services.Interfaces;
using org.matheval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEquation.Services.Implementations
{
    public class MathEquationService: IMathEquationService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MathEquationService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> SaveEquationAsync(Common.MathEquation mathEquation)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var result = await connection.ExecuteAsync(
                @"INSERT INTO maths_equations (id, equation, x_value, y_value, date_completed) 
            VALUES (@id, @equation, @x_value, @y_value, @date_completed)",
                mathEquation);

            return result > 0;
        }

        public object CalculateEquation(string equation, double x_value)
        {
            var exp = equation.ToLower();
            
            if (equation.Contains('='))
            {
                exp = equation.Substring(equation.IndexOf('=') + 1).ToLower();
            }

            if (exp.Contains("\\sqrt"))
            {
                exp = exp.Replace("\\sqrt", "");
                exp = $"SQRT({exp})";
            }

            Expression expression = new Expression(exp);
            expression.Bind("x", x_value);

            var errors = expression.GetError();

            if (errors.Count != 0)
            {
                return errors;
            }

            Double.TryParse(expression.Eval().ToString(), out var y_value);

            return new Common.MathEquation
            {
                x_value = x_value,
                date_completed = DateTime.Now,
                equation = equation,
                y_value = y_value
            };
        }

        public async Task<IEnumerable<Common.MathEquation>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            return await connection.QueryAsync<Common.MathEquation>("SELECT * FROM maths_equations");
        }
    }
}
