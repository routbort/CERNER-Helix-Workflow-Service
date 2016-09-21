using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

 
    public class FlowTube
    {

        static Bitmap bmpCheckImage = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HelixFlowTubeChecker.Resources.check.png"));
        static Bitmap bmpBlankImage = new Bitmap(1, 1);


        public string MRN { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public int SeqNbr { get; set; }
        public string TubeLabel { get; set; }
        public bool Confirmed { get; set; }
        public Bitmap ConfirmationIcon { get { return (this.Confirmed) ? bmpCheckImage : bmpBlankImage; } }
        public string AccessionNumberUnformatted
        {
            get
            {
                if (AccessionNumber == null) return null;
                return this.AccessionNumber.Replace("-", "");
            }
        }

    }
  