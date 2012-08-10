using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NanoBuilder
{
    public class PPPE
    {
        public string Name;
        public string ShortName;
        public string Type;
        public int AOID;
        public int QL;

        public PPPE(string name, string shortname, string type, int aoid, int ql)
        {
            Name = name;
            ShortName = shortname;
            Type = type;
            AOID = aoid;
            QL = ql;
        }
    }
}
