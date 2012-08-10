using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NanoBuilder
{
    public class CA
    {
        public string Name;
        public string ShortName;
        public int AOID;
        public int QL;

        public CA(string name, string shortname, int aoid, int ql)
        {
            Name = name;
            ShortName = shortname;
            AOID = aoid;
            QL = ql;
        }
    }
}
