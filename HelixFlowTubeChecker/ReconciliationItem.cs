using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelixFlowTubeChecker
{
    public class ReconciliationItem
    {
        public string MRN { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public int SeqNbr { get; set; }
        public string TubeLabel { get; set; }
        public bool Confirmed { get; set; }
    }
}
