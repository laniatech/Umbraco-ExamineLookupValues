using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotSee.ExamineLookupValues
{
    public class ElvRule
    {
        public string DocTypeAlias { get; set; }
        public string PropertyAlias { get; set; }

        public ElvRule(string docTypeAlias, string propertyAlias)
        {
            DocTypeAlias = docTypeAlias;
            PropertyAlias = propertyAlias;
        }
    }
}
