namespace HelixFlowTubeChecker
{
    using Infragistics.Win;
    using Infragistics.Win.UltraWinGrid;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    public class UltraGridMDL : UltraGrid
    {
        private int _Band = 0;
        private Image _ColumnChooserImage = null;
        private string _ColumnVisibleInitString = null;
        private string _ColumnWidthInitString = null;
        private List<string> ReadOnlyColumns = new List<string>();

        public UltraGridMDL()
        {
            this.InitializeComponent();
            base.DisplayLayout.Override.ColumnAutoSizeMode = ColumnAutoSizeMode.AllRowsInBand;
            base.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            base.DisplayLayout.Override.SelectTypeCol = SelectType.None;
            base.DisplayLayout.Override.SelectTypeGroupByRow = SelectType.None;
            base.DisplayLayout.Override.RowSelectors = DefaultableBoolean.False;
            base.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
            base.DisplayLayout.Override.AllowColMoving = AllowColMoving.WithinBand;
            base.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortSingle;
            base.AfterRowActivate += new EventHandler(this.UltraGridMDL_AfterRowActivate);
        }

        public void CopyRowsToClipboard(bool IncludeColumnList = true)
        {
            int num = 0;
            Dictionary<int, UltraGridColumn> dictionary = new Dictionary<int, UltraGridColumn>();
            foreach (UltraGridColumn column in base.DisplayLayout.Bands[this._Band].Columns)
            {
                if (!column.Hidden)
                {
                    num++;
                    dictionary.Add(column.Header.VisiblePosition, column);
                }
            }
            List<int> list = dictionary.Keys.ToList<int>();
            list.Sort();
            StringBuilder builder = new StringBuilder();
            if (IncludeColumnList)
            {
                foreach (int num2 in list)
                {
                    builder.Append(dictionary[num2].Header.Caption);
                    builder.Append("\t");
                }
                builder.Append(Environment.NewLine);
            }
            foreach (UltraGridRow row in base.Selected.Rows)
            {
                foreach (int num2 in list)
                {
                    builder.Append(row.Cells[dictionary[num2]].Value.ToString());
                    builder.Append("\t");
                }
                builder.Append(Environment.NewLine);
            }
            Clipboard.SetDataObject(builder.ToString());
        }

        ~UltraGridMDL()
        {
        }

        private void InitializeComponent()
        {
            ((ISupportInitialize) this).BeginInit();
            base.SuspendLayout();
            base.InitializeLayout += new InitializeLayoutEventHandler(this.UltraGridMDL_InitializeLayout);
            base.KeyDown += new KeyEventHandler(this.UltraGridMDL_KeyDown);
            base.MouseClick += new MouseEventHandler(this.UltraGridMDL_MouseClick);
            base.MouseDoubleClick += new MouseEventHandler(this.UltraGridMDL_MouseDoubleClick);
            ((ISupportInitialize) this).EndInit();
            base.ResumeLayout(false);
        }

        public void LoadDisplayLayoutFromXml(string ConfigXML)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(ConfigXML)))
            {
                base.DisplayLayout.LoadFromXml(stream);
            }
        }

        public void ResizeColumnsFull()
        {

            foreach (var column in base.DisplayLayout.Bands[this._Band].Columns)
            { column.MinWidth = 50; }


        
            if ((this._ColumnWidthInitString != null) && (this._ColumnWidthInitString != ""))
            {
                string[] strArray = this._ColumnWidthInitString.Split(new char[] { ';' });
                foreach (string str in strArray)
                {
                    string[] strArray2 = str.Split(new char[] { '=' });
                    if (base.DisplayLayout.Bands[this._Band].Columns.Exists(strArray2[0]))
                    {
                        base.DisplayLayout.Bands[this._Band].Columns[strArray2[0]].MinWidth = int.Parse(strArray2[1]);
                    }
                }
            }
        }

        public void ResizeColumnsShort()
        {
        }

        public string SaveDisplayLayoutAsXml()
        {
            string str;
            using (MemoryStream stream = new MemoryStream())
            {
                base.DisplayLayout.SaveAsXml(stream);
                stream.Position = 0L;
                using (StreamReader reader = new StreamReader(stream))
                {
                    str = reader.ReadToEnd();
                }
            }
            return str;
        }

        public void SetMinColumnWidths(string ColumnInitString, int Band = 0)
        {
            this._ColumnWidthInitString = ColumnInitString;
            this._Band = Band;
        }

        public void SetOnlyVisibleColumns(string ColumnInitString, int Band = 0)
        {
            this._ColumnVisibleInitString = ColumnInitString;
            this._Band = Band;
        }

        public void SetReadOnlyColumns(string ColumnList)
        {
            this.ReadOnlyColumns.Clear();
            foreach (string str in ColumnList.Split(new char[] { ';' }))
            {
                this.ReadOnlyColumns.Add(str);
            }
        }

        private void ShowGridColumnChooser()
        {
            ColumnChooserDialog dialog = new ColumnChooserDialog {
                Owner = base.FindForm()
            };
            UltraGridColumnChooser columnChooserControl = dialog.ColumnChooserControl;
            columnChooserControl.SourceGrid = this;
            columnChooserControl.CurrentBand = base.DisplayLayout.Bands[0];
            columnChooserControl.Style = ColumnChooserStyle.AllColumnsAndChildBandsWithCheckBoxes;
            columnChooserControl.MultipleBandSupport = MultipleBandSupport.SingleBandOnly;
            dialog.Size = new Size(150, 300);
            dialog.Show();
        }

        private void UltraGridMDL_AfterRowActivate(object sender, EventArgs e)
        {
            UltraGridMDL dmdl = sender as UltraGridMDL;
            if (dmdl.ActiveRow.IsDataRow)
            {
                foreach (string str in this.ReadOnlyColumns)
                {
                    dmdl.ActiveRow.Band.Columns[str].TabStop = false;
                    dmdl.ActiveRow.Band.Columns[str].CellActivation = Activation.NoEdit;
                }
            }
        }

        private void UltraGridMDL_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (this._ColumnChooserImage == null)
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
            }
            e.Layout.CaptionAppearance.Image = this._ColumnChooserImage;
            if ((this._ColumnVisibleInitString != null) && (this._ColumnVisibleInitString != ""))
            {
                string[] strArray = this._ColumnVisibleInitString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                int num = -1;
                foreach (var column in e.Layout.Bands[this._Band].Columns)
                {
                    column.Hidden = true;
                }
                
                foreach (string str in strArray)
                {
                    num++;
                    string[] strArray2 = str.Split(new char[] { '=' });
                    string key = strArray2[0];
                    if (e.Layout.Bands[this._Band].Columns.Exists(key))
                    {
                        e.Layout.Bands[this._Band].Columns[key].Hidden = false;
                        e.Layout.Bands[this._Band].Columns[key].AllowRowFiltering = DefaultableBoolean.False;
                        e.Layout.Bands[this._Band].Columns[key].Header.VisiblePosition = num;
                        if (strArray2.Length > 1)
                        {
                            e.Layout.Bands[this._Band].Columns[strArray2[0]].Header.Caption = strArray2[1];
                        }
                    }
                }
            }
            this.ResizeColumnsFull();
        }

        private void UltraGridMDL_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.C) && e.Control)
            {
                this.CopyRowsToClipboard(true);
            }
        }

        private void UltraGridMDL_MouseClick(object sender, MouseEventArgs e)
        {
           // Debug.WriteLine(e.Location.ToString());
            UIElement element = base.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
           // Debug.WriteLine(element.ToString());
            if ((element.ToString() == "Infragistics.Win.ImageUIElement") && (e.Y < 20))
            {
                base.ShowColumnChooser();
            }
        }

        private void UltraGridMDL_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UIElement element = base.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
            if ((((element.ToString() == "Infragistics.Win.TextUIElement") && (element.Parent.ToString() == "Infragistics.Win.UltraWinGrid.CaptionAreaUIElement")) && (element.Parent.Parent.ToString() == "Infragistics.Win.UltraWinGrid.UltraGridUIElement")) && (((Control.ModifierKeys & Keys.Shift) != Keys.None) && ((Control.ModifierKeys & Keys.Control) != Keys.None)))
            {
                Clipboard.SetText(this.SaveDisplayLayoutAsXml());
                MessageBox.Show("Grid configuration copied to clipboard");
            }
        }
    }
}

