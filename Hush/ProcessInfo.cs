using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hush
{
    public class ProcessInfo
    {
        public int ID { get; private set; }
        public string name { get; private set; }

        public ProcessInfo(int ID, string name)
        {
            this.ID = ID;
            this.name = name;
        }

    }
}
