using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;

namespace ProjOb
{
    public interface IReportable
    {
        public string Reported(IVisitor visitor);
    }

}
