using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NanoBuilder
{
    public class Crystal
    {
        public string Name;
        public string ShortName;
        public int AOID;
        public int QL;

        public Crystal(string name, string shortname, int aoid, int ql)
        {
            Name = name;
            AOID = aoid;
            ShortName = shortname;
            QL = ql;
        }
    }
}