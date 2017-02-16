using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui
{
    class CamProgramm
    {
        public string OperationName { get; set; }
        public string OperationParams { get; set; }

        public static CamProgramm[] GetPersons()
        {
            return new []
            {
                new CamProgramm{OperationParams = "Sidorov", OperationName = "Sidor"},
                new CamProgramm{OperationParams = "Petrov", OperationName = "Peter"},
                new CamProgramm{OperationParams = "Victorov",OperationName = "Victor"}
            };
        }
    }
}
