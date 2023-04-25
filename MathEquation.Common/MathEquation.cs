using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEquation.Common
{
    public class MathEquation
    {
        public int? id { get; set; }

        public string equation { get; set; }

        public double x_value { get; set; }

        public double y_value { get; set; }

        public DateTime date_completed { get; set; }
    }
}
