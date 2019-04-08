using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using static Symbol.XamarinEMDK.Barcode.ScannerConfig;
using System;
using System.Collections.Generic;

namespace App1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, EMDKManager.IEMDKListener
    {
        private BarcodeManager barcodeManager = null;
        private Scanner zebraScanner = null;
        private EMDKManager emdkManager = null;
        private TextView textView1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            EMDKResults results = EMDKManager.GetEMDKManager(Application.Context, this);
            textView1 = FindViewById<TextView>(Resource.Id.textView1);
        }

        protected override void OnResume()
        {
            base.OnResume();
            InitScanner();
        }

        protected override void OnPause()
        {
            base.OnPause();
            DeinitScanner();
        }

        private void InitScanner()
        {
            if (emdkManager != null)
            {

                if (barcodeManager == null)
                {
                    ScannerConfig config;

                    try
                    {
                        barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);
                        zebraScanner = barcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);

                        if (zebraScanner != null)
                        {
                            zebraScanner.Data += scanner_Data;
                            zebraScanner.Status += scanner_Status;
                            zebraScanner.Enable();

                            config = zebraScanner.GetConfig();

                            config.DecoderParams.Aztec.Enabled = false;
                            config.DecoderParams.CodaBar.Enabled = false;
                            config.DecoderParams.Code128.Enabled = false;
                            config.DecoderParams.Code39.Enabled = false;
                            config.DecoderParams.DataMatrix.Enabled = false;
                            config.DecoderParams.Ean8.Enabled = false;
                            config.DecoderParams.Gs1Databar.Enabled = false;
                            config.DecoderParams.Gs1DatabarExp.Enabled = false;
                            config.DecoderParams.MailMark.Enabled = false;
                            config.DecoderParams.MaxiCode.Enabled = false;
                            config.DecoderParams.Pdf417.Enabled = false;
                            config.DecoderParams.QrCode.Enabled = false;
                            config.DecoderParams.Upca.Enabled = false;
                            config.DecoderParams.Upce1.Enabled = false;

                            config.DecoderParams.Ean13.Enabled = true;
                            config.DecoderParams.UpcEanParams.Supplemental5 = true;
                            config.DecoderParams.UpcEanParams.SupplementalMode = SupplementalMode.Always;

                            // pour éviter la ScannerException ci-dessous (?)
                            if (zebraScanner.IsReadPending)
                                zebraScanner.CancelRead();

                            zebraScanner.SetConfig(config);
                        }
                    }
                    catch (ScannerException e)
                    {

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        void DeinitScanner()
        {
            if (emdkManager != null)
            {
                if (zebraScanner != null)
                {
                    try
                    {
                        zebraScanner.Data -= scanner_Data;
                        zebraScanner.Status -= scanner_Status;

                        zebraScanner.Disable();
                    }
                    catch (ScannerException e)
                    {

                    }
                }

                if (barcodeManager != null)
                {
                    emdkManager.Release(EMDKManager.FEATURE_TYPE.Barcode);
                }

                barcodeManager = null;
                zebraScanner = null;
            }
        }

        void EMDKManager.IEMDKListener.OnOpened(EMDKManager p0)
        {
            emdkManager = p0;
            InitScanner();
        }

        void EMDKManager.IEMDKListener.OnClosed()
        {
            if (emdkManager != null)
            {
                emdkManager.Release();
                emdkManager = null;
            }
        }

        void scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            ScanDataCollection scanDataCollection = e.P0;

            if ((scanDataCollection != null) && (scanDataCollection.Result == ScannerResults.Success))
            {
                IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();

                foreach (ScanDataCollection.ScanData data in scanData)
                {
                    RunOnUiThread(() => textView1.Text = data.Data);
                }
            }
        }

        void scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            StatusData.ScannerStates state = e.P0.State;

            if (state == StatusData.ScannerStates.Idle)
            {
                try
                {
                    if (zebraScanner.IsEnabled && !zebraScanner.IsReadPending)
                    {
                        zebraScanner.Read();
                    }
                }
                catch (ScannerException e1)
                {

                }
            }
        }
    }
}