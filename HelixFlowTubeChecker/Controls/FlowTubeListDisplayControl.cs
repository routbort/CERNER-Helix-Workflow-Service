using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;

namespace HelixFlowTubeChecker
{
    public partial class FlowTubeListDisplayControl : UserControl
    {
        public FlowTubeListDisplayControl()
        {
            InitializeComponent();
        }
        private DataInterface _DataInterface;

        public void  Bind(DataInterface di)
        {
            _DataInterface = di;
        }

        public void  RefreshData()
        {
            this.gridFlowLists.SetOnlyVisibleColumns("filename=Filename;containing_directory=Directory;date_created=File Created;date_verified=Verified On;verified_by=Verified By");
            this.gridFlowLists.DataSource = _DataInterface.GetRecentTubeLists();
            this.gridFlowLists.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.gridFlowLists.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
            this.gridFlowLists.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
            this.gridFlowLists.DisplayLayout.Bands[0].Columns["date_created"].Format = "MM/dd/yy hh:mm tt";
            gridFlowLists.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            gridFlowLists.ActiveRow = null;
            this.gridFlowLists.Selected.Rows.Clear();
        }

    }
}
