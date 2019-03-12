using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmtestCommunicator.Entities
{
    public class Projects
    {
        public string ProjectName { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<string> ProductA2Cs { get; set; }
    }
}
