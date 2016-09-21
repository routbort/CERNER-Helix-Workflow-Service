using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace HelixFlowTubeChecker
{
    public partial class TubeValidationControl : UserControl
    {
        public TubeValidationControl()
        {
            InitializeComponent();
            VisibleColumnDefinition = "SeqNbr=Tube;AccessionNumber=Container;TubeLabel=Label;Name=Name;MRN=MRN;ConfirmationIcon=Confirmed?";
            SetBaselineMessage();
        }

        public enum Status { PendingManifest, ManifestInProgress, ManifestComplete, ErrorState, PendingManifestChoice }

        public delegate void DatabaseUpdatedEventHandler();
        public event DatabaseUpdatedEventHandler DatabaseUpdated;



        public Status CurrentStatus
        {
            get;
            private set;
        }

        public delegate void SubmitManifestEventHandler();
        public event SubmitManifestEventHandler SubmitManifest;

        bool _ConfirmationMode { get; set; }


        public bool AllSamplesConfirmed()
        {
            foreach (FlowTube sample in _FlowTubeList.FlowTubes)
                if (!sample.Confirmed) return false;
            if (_FlowTubeList.FlowTubes.Count == 0) return false;
            return true;
        }

        public UltraGridMDL Grid { get { return this.gridSamples; } }

        public void Clear()
        {

            this.gridSamples.Visible = true;
            this.gridFlowLists.Visible = false;
            if (_FlowTubeList != null)
                _FlowTubeList.FlowTubes.Clear();
            ClearError();
            CurrentStatus = Status.PendingManifest;
            this.gridSamples.Text = "";
            this.gridSamples.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
        }

        private void ConfirmTube(string barcode)
        {
            if (CurrentStatus == Status.ErrorState)
            {
                ShowError("Error state - scanning is disabled until manifest is cleared");
                return;
            }
            CurrencyManager cm = (CurrencyManager)this.BindingContext[gridSamples.DataSource];
            FlowTube currentSample = GetCurrentSample();
            if (AllSamplesConfirmed() && currentSample != null && barcode == currentSample.AccessionNumberUnformatted)
            {
                if (this.SubmitManifest != null) this.SubmitManifest();
                return;
            }

            if (AllSamplesConfirmed())
            {

                CurrentStatus = Status.ManifestComplete;
                ShowError("All samples have been confirmed.  You can scan the last sample again to complete the manifest.");
                return;
            }

            if (currentSample != null && barcode == currentSample.AccessionNumberUnformatted)
                ConfirmCurrentSample();
            else
                ShowError("Out of order scan. Review field & samples, and clear/restart manifest generation", true);
            return;

        }


        private DataInterface _DataInterface;

        public void Bind(DataInterface di)
        {
            this._DataInterface = di;
        }


        public FlowTube LastTube
        {
            get
            {
                if (_FlowTubeList == null || _FlowTubeList.FlowTubes.Count == 0)
                    return null;
                return _FlowTubeList.FlowTubes[_FlowTubeList.FlowTubes.Count - 1];
            }
        }


        public void HandleAccessionScan(string code)
        {

            if (CurrentStatus == Status.PendingManifestChoice)
            {

                ShowError("Pending manifest choice. Click on a manifest to select it for validation.");
                return;
            }


            if (CurrentStatus == Status.ErrorState)
            {
                ShowError("Error state.  You must reset the manifest and begin validation process from scratch");
                return;
            }

            if (CurrentStatus == Status.ManifestInProgress)
            {
                ConfirmTube(code);
                return;
            }

            if (CurrentStatus == Status.ManifestComplete)
            {
                if (code == this.LastTube.AccessionNumberUnformatted)
                {
                    _FlowTubeList.verified_by = Environment.UserName;
                    ShowMessage("Manifest validated and submitted by " + Global.CURRENT_USERNAME + ".  OK to proceed");
                    _DataInterface.UpdateVerficationInfo(_FlowTubeList);
                    this.Clear();
                    Sound.PlayWaveResource("ObjectiveCompleted.wav");
                    if (this.DatabaseUpdated != null)
                        this.DatabaseUpdated();
                }

                else
                    ShowError("Unexpected scan: " + code + System.Environment.NewLine + "Expected last tube: " + this.LastTube.AccessionNumberUnformatted, true);
                return;
            }


            if (CurrentStatus == Status.PendingManifest)
            {
                List<FlowTubeList> matches = _DataInterface.GetMatchingTubeLists(code);

                if (matches.Count == 0)
                {
                    ShowError("Scanned barcode " + code + " does not match first tube in any existing manifest");
                    return;
                }


                if (matches.Count == 1)
                {
                    if (ProceedWithListLoad(matches[0]))
                    {
                        this.SetFlowTubeList(matches[0]);
                        SetCurrentSampleIndex(0);
                        CurrentStatus = Status.ManifestInProgress;
                        //  ConfirmCurrentSample();
                    }
                    else
                    {
                        CurrentStatus = Status.PendingManifest;
                        SetBaselineMessage();
                    }
                    return;
                }

                if (matches.Count > 1)
                {
                    CurrentStatus = Status.PendingManifestChoice;
                    ShowMessage("Multiple matching manifests found; please select the appropriate one by clicking in the selection grid");
                    this.gridFlowLists.Visible = true;
                    this.gridSamples.Visible = false;
                    this.gridFlowLists.SetOnlyVisibleColumns("filename=Filename;directory=Path;date_created=File Created;date_verified=Verified On;verified_by=Verified By");
                    this.gridFlowLists.DataSource = matches;
                    this.gridFlowLists.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
                    this.gridFlowLists.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
                    this.gridFlowLists.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
                    this.gridFlowLists.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
                    this.gridFlowLists.DisplayLayout.Bands[0].Columns["date_created"].Format = "MM/dd/yy hh:mm tt";
                    gridFlowLists.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
                    gridFlowLists.ActiveRow = null;
                    this.gridFlowLists.Selected.Rows.Clear();
                    Sound.PlayWaveResource("StateError.wav");
                }

            }



        }


        FlowTubeList _FlowTubeList = null;

        private void SetFlowTubeList(FlowTubeList FlowTubeList)
        {

            this.lblMessage.Text = "Loaded " + FlowTubeList.filename;
            this.gridSamples.SetOnlyVisibleColumns(VisibleColumnDefinition);
            this.gridSamples.DataSource = FlowTubeList.FlowTubes;
            this.gridSamples.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.gridSamples.DisplayLayout.Override.CellClickAction = CellClickAction.CellSelect;
            this.gridSamples.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
            this.gridSamples.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.Select;
            this.gridSamples.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            UltraGridColumn imageColumn = gridSamples.DisplayLayout.Bands[0].Columns["ConfirmationIcon"];
            imageColumn.CellAppearance.ImageHAlign = HAlign.Center;
            imageColumn.CellAppearance.ImageVAlign = VAlign.Middle;
            _FlowTubeList = FlowTubeList;
            Sound.PlayWaveResource("Camera.wav");
            // _Tubes = FlowTubeList.FlowTubes;
        }



        public string VisibleColumnDefinition
        {
            get;
            set;
        }

        public BindingList<FlowTube> Tubes { get { if (_FlowTubeList == null)return null; return _FlowTubeList.FlowTubes; } }

        private FlowTube GetCurrentSample()
        {
            return (this.gridSamples.Selected.Rows[0].ListObject as FlowTube);
        }

        private FlowTube GetNextSample()
        {
            return (this.gridSamples.Rows[this.gridSamples.Selected.Rows[0].ListIndex + 1].ListObject as FlowTube);
        }

        private int GetCurrentSampleLinearIndex()
        {
            return this.gridSamples.Selected.Rows[0].Index;
        }

        private void ConfirmCurrentSample()
        {
            FlowTube currentSample = GetCurrentSample();
            currentSample.Confirmed = true;
            this.Refresh();
            int currentIndex = GetCurrentSampleLinearIndex();
            if (currentIndex < this.gridSamples.Rows.Count - 1)
            {
                SetCurrentSampleIndex(currentIndex + 1);
                ClearError();
            }
            else
                if (AllSamplesConfirmed())
                {
                    CurrentStatus = Status.ManifestComplete;
                    gridSamples.Selected.Rows.Clear();
                    ShowMessage("All tubes scanned correctly.  Rescan last tube to complete/submit manifest.");

                }
            Sound.PlayWaveResource("Match.wav");


        }

        public void SetCurrentSampleIndex(int index)
        {
            CurrencyManager cm = (CurrencyManager)this.BindingContext[gridSamples.DataSource];
            this.gridSamples.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            gridSamples.Selected.Rows.Clear();

            if (!AllSamplesConfirmed())
            {
                cm.Position = index;
                gridSamples.Rows[index].Selected = true;
                gridSamples.Rows[index].Activate();
            }

            this.gridSamples.DisplayLayout.Override.SelectTypeRow = (_ConfirmationMode) ? Infragistics.Win.UltraWinGrid.SelectType.None : Infragistics.Win.UltraWinGrid.SelectType.Single;
        }


        public void ShowError(string Message, bool Fatal = false)
        {

            lblMessage.Text = Message;
            pnlMessage.Visible = true;
            lblMessage.ForeColor = Color.Red;


            if (Fatal)
            {
                CurrentStatus = Status.ErrorState;
                Sound.PlayWaveResource("Mismatch.wav");
            }
            else
                Sound.PlayWaveResource("StateError.wav");

        }


        public void ShowMessage(string Message)
        {
            lblMessage.Text = Message;
            lblMessage.ForeColor = Color.Green;
            pnlMessage.Visible = true;
        }

        public void ClearError()
        {
            pnlMessage.Visible = false;
            this.lblMessage.Text = "";
            if (CurrentStatus == Status.ErrorState)
                CurrentStatus = Status.PendingManifest;
        }

        private void gridSamples_ClickCell(object sender, Infragistics.Win.UltraWinGrid.ClickCellEventArgs e)
        {
            Sound.PlayWaveResource("StateError.wav");
        }

        private void gridSamples_AfterRowActivate(object sender, EventArgs e)
        {
            this.gridSamples.ActiveRow = null;
        }

        private void gridSamples_AfterCellActivate(object sender, EventArgs e)
        {
            this.gridSamples.ActiveCell = null;
        }


        private bool ProceedWithListLoad(FlowTubeList ftl)
        {
            bool Load = true;
            if (ftl.verified_by != null && ftl.verified_by != "")
                Load = MessageBox.Show("This manifest was already verified on " + ftl.date_verified.Value.ToShortDateString() + " by " + ftl.verified_by + System.Environment.NewLine
                     + "Are you sure you want to start reverification of this manifest?", "Already verified", MessageBoxButtons.YesNo) == DialogResult.Yes;
            return Load;

        }

        private void SetBaselineMessage()
        {
            ShowMessage("Scan the first tube in a rack/manifest to load tube list");
        }

        private void gridFlowLists_ClickCell(object sender, ClickCellEventArgs e)
        {
            e.Cell.Row.Selected = true;
            this.Refresh();
            System.Threading.Thread.Sleep(250);
            this.gridSamples.Visible = true;
            this.gridFlowLists.Visible = false;
            FlowTubeList ftl = e.Cell.Row.ListObject as FlowTubeList;

            if (ProceedWithListLoad(ftl))
            {
                this.SetFlowTubeList(ftl);
                SetCurrentSampleIndex(0);
                CurrentStatus = Status.ManifestInProgress;
            }
            else
            {
                CurrentStatus = Status.PendingManifest;
                SetBaselineMessage();
            }

        }

        public bool LimitTubeCount
        {
            get
            {
                if (_DataInterface == null) return false;
                return _DataInterface.LimitTubeCount;
            }
            set { if (_DataInterface == null) return; _DataInterface.LimitTubeCount = value; }
        }
        public int LimitTubeMax { get { if (_DataInterface == null) return 0; return _DataInterface.LimitTubeMax; } set { if (_DataInterface == null) return; _DataInterface.LimitTubeMax = value; } }


    }
}
