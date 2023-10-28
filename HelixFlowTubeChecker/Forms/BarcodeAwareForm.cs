using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class BarCodeAwareForm : Form
{

    const char BARCODE_PREFIX = '\\';
    const char BARCODE_SUFFIX = '\\';
    const int BARCODE_TIMEOUT = 100;


    enum WedgeState { WaitingForPrefix, WaitingForSuffix }
    bool _InTimeoutRecovery = false;
    string _BarcodeInProgress = null;

    int _TicksAtPrefix = 0;
    WedgeState _WedgeState = WedgeState.WaitingForPrefix;
    System.Windows.Forms.Timer _BarcodeTimer;


    public BarCodeAwareForm()
    {
        //InitializeComponent();
        this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BarCodeAwareForm_KeyPress);
        this.KeyPreview = true;
        _BarcodeTimer = new Timer();
        _BarcodeTimer.Interval = BARCODE_TIMEOUT;
        _BarcodeTimer.Tick += _BarcodeTimer_Tick;
        this.ResumeLayout(false);
    }


    private void _BarcodeTimer_Tick(object sender, System.EventArgs e)
    {

        if (_WedgeState == WedgeState.WaitingForSuffix)
        {
            _BarcodeTimer.Stop();
            _WedgeState = WedgeState.WaitingForPrefix;
            _InTimeoutRecovery = true;
            System.Windows.Forms.SendKeys.Send(BARCODE_PREFIX + _BarcodeInProgress);
            _BarcodeInProgress = null;
        }
    }


    private void BarCodeAwareForm_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
        switch (_WedgeState)
        {
            case WedgeState.WaitingForSuffix:
                e.Handled = true;
                if (e.KeyChar == BARCODE_SUFFIX)
                {
                    _WedgeState = WedgeState.WaitingForPrefix;
                    _BarcodeTimer.Stop();
                    System.Diagnostics.Debug.WriteLine("Complete Barcode: " + _BarcodeInProgress + " in " + (System.Environment.TickCount - _TicksAtPrefix).ToString() + " milliseconds");
                    BarcodeScanned?.Invoke(this, new BarcodeScanEventArgs(_BarcodeInProgress));
                }
                else
                {
                    _BarcodeInProgress += e.KeyChar;
                }
                break;
            case WedgeState.WaitingForPrefix:
                if (e.KeyChar == BARCODE_PREFIX)
                {
                    if (_InTimeoutRecovery)
                    {
                        _InTimeoutRecovery = false;
                    }
                    else
                    {
                        _TicksAtPrefix = System.Environment.TickCount;
                        _BarcodeInProgress = "";
                        _WedgeState = WedgeState.WaitingForSuffix;
                        _BarcodeTimer.Stop();
                        _BarcodeTimer.Start();
                        e.Handled = true;
                    }
                }
                break;
            default:
                break;
        }


    }

    public class BarcodeScanEventArgs : EventArgs
    {
        public string Code { get; private set; }

        public BarcodeScanEventArgs(string code)
        {
            Code = code;
        }
    }

    public delegate void BarcodeScanHandler(object sender, BarcodeScanEventArgs e);
    public event BarcodeScanHandler BarcodeScanned;
}
