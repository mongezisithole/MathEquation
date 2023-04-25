using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   

namespace MathEquation.Services.Interfaces
{
    public interface IMathEquationService
    {
        Task<bool> SaveEquationAsync(Common.MathEquation mathEquation);

        object CalculateEquation(string equation, double x_value);

        Task<IEnumerable<Common.MathEquation>> GetAllAsync();
    }
}
