// Forex Strategy Builder - JForex_Import
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class JForex_Import : Form
    {
        Label   lblIntro;
        TextBox txbDataDirectory;
        Button  btnBrowse;
        Label   lblMarketClose;
        Label   lblMarketOpen;
        NumericUpDown nudMarketClose;
        NumericUpDown nudMarketOpen;

        Fancy_Panel pnlSettings;
        Fancy_Panel pnlInfoBase;
        ProgressBar progressBar;
        TextBox tbxInfo;
        Button  btnHelp;
        Button  btnImport;
        Button  btnClose;
        ToolTip toolTip = new ToolTip();

        Font   font;
        Color  colorText;

        BackgroundWorker bgWorker;
        bool isImporting;
        List<JForex_Data_Files> files = new List<JForex_Data_Files>();

        /// <summary>
        /// Constructor
        /// </summary>
        public JForex_Import()
        {
            lblIntro         = new Label();
            txbDataDirectory = new TextBox();
            btnBrowse        = new Button();
            pnlSettings      = new Fancy_Panel();
            pnlInfoBase      = new Fancy_Panel(Language.T("Imported Files"));
            tbxInfo          = new TextBox();
            btnHelp          = new Button();
            btnClose         = new Button();
            btnImport        = new Button();
            progressBar      = new ProgressBar();

            lblMarketClose  = new Label();
            lblMarketOpen   = new Label();
            nudMarketClose  = new NumericUpDown();
            nudMarketOpen   = new NumericUpDown();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnImport;
            CancelButton    = btnClose;
            Text            = Language.T("JForex Import");

            // Label Intro
            lblIntro.Parent    = pnlSettings;
            lblIntro.ForeColor = colorText;
            lblIntro.BackColor = Color.Transparent;
            lblIntro.AutoSize  = true;
            lblIntro.Text      = Language.T("Directory containing JForex data files:");

            // Data Directory
            txbDataDirectory.Parent    = pnlSettings;
            txbDataDirectory.BackColor = LayoutColors.ColorControlBack;
            txbDataDirectory.ForeColor = colorText;
            txbDataDirectory.Text      = Configs.JForexDataPath;

            // Button Browse
            btnBrowse.Parent = pnlSettings;
            btnBrowse.Name   = "Browse";
            btnBrowse.Text   = Language.T("Browse");
            btnBrowse.Click += new EventHandler(BtnBrowse_Click);
            btnBrowse.UseVisualStyleBackColor = true;

            // Label Market Close
            lblMarketClose.Parent    = pnlSettings;
            lblMarketClose.ForeColor = colorText;
            lblMarketClose.BackColor = Color.Transparent;
            lblMarketClose.AutoSize  = true;
            lblMarketClose.Text      = Language.T("Market closing hour on Friday:");

            // Label Market Open
            lblMarketOpen.Parent    = pnlSettings;
            lblMarketOpen.ForeColor = colorText;
            lblMarketOpen.BackColor = Color.Transparent;
            lblMarketOpen.AutoSize  = true;
            lblMarketOpen.Text      = Language.T("Market opening hour on Sunday:");

            // nudMarketClose
            nudMarketClose.BeginInit();
            nudMarketClose.Parent    = pnlSettings;
            nudMarketClose.TextAlign = HorizontalAlignment.Center;
            nudMarketClose.Minimum   = 0;
            nudMarketClose.Maximum   = 24;
            nudMarketClose.Increment = 1;
            nudMarketClose.Value     = Configs.MarketClosingHour;
            nudMarketClose.EndInit();

            // nudMarketOpen
            nudMarketOpen.BeginInit();
            nudMarketOpen.Parent    = pnlSettings;
            nudMarketOpen.TextAlign = HorizontalAlignment.Center;
            nudMarketOpen.Minimum   = 0;
            nudMarketOpen.Maximum   = 24;
            nudMarketOpen.Increment = 1;
            nudMarketOpen.Value     = Configs.MarketOpeningHour;
            nudMarketOpen.EndInit();

            // pnlSettings
            pnlSettings.Parent = this;

            // pnlInfoBase
            pnlInfoBase.Parent  = this;
            pnlInfoBase.Padding = new Padding(4, (int)pnlInfoBase.CaptionHeight, 2, 2);

            // tbxInfo
            tbxInfo.Parent        = pnlInfoBase;
            tbxInfo.BorderStyle   = BorderStyle.None;
            tbxInfo.Dock          = DockStyle.Fill;
            tbxInfo.BackColor     = LayoutColors.ColorControlBack;
            tbxInfo.ForeColor     = LayoutColors.ColorControlText;
            tbxInfo.Multiline     = true;
            tbxInfo.AcceptsReturn = true;
            tbxInfo.AcceptsTab    = true;
            tbxInfo.ScrollBars    = ScrollBars.Vertical;

            // ProgressBar
            progressBar.Parent = this;

            // Button Help
            btnHelp.Parent = this;
            btnHelp.Name   = "Help";
            btnHelp.Text   = Language.T("Help");
            btnHelp.Click += new EventHandler(BtnHelp_Click);
            btnHelp.UseVisualStyleBackColor = true;

            // Button Close
            btnClose.Parent       = this;
            btnClose.Text         = Language.T("Close");
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.UseVisualStyleBackColor = true;

            // Button Import
            btnImport.Parent = this;
            btnImport.Name   = "Import";
            btnImport.Text   = Language.T("Import");
            btnImport.Click += new EventHandler(BtnImport_Click);
            btnImport.UseVisualStyleBackColor = true;

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(BgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            ClientSize = new Size(3 * buttonWidth + 4 * btnHrzSpace, 400);

            btnImport.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int border       = btnHrzSpace;
            int textHeight   = Font.Height;
            int nudWidth     = 70;

            //Button Cancel
            btnClose.Size     = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Help
            btnHelp.Size     = new Size(buttonWidth, buttonHeight);
            btnHelp.Location = new Point(btnClose.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Import
            btnImport.Size     = new Size(buttonWidth, buttonHeight);
            btnImport.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            progressBar.Size = new Size(ClientSize.Width - 2 * border, (int)(Data.VerticalDLU * 9));
            progressBar.Location = new Point(border, btnClose.Top - progressBar.Height - btnVertSpace);

            pnlSettings.Size = new Size(ClientSize.Width - 2 * btnHrzSpace, 110);
            pnlSettings.Location = new Point(btnHrzSpace, border);

            pnlInfoBase.Size = new Size(ClientSize.Width - 2 * btnHrzSpace, progressBar.Top - pnlSettings.Bottom - 2 * border);
            pnlInfoBase.Location = new Point(btnHrzSpace, pnlSettings.Bottom + border);

            // Label Intro
            lblIntro.Location = new Point(btnHrzSpace + border, btnVertSpace);

            //Button Browse
            btnBrowse.Size     = new Size(buttonWidth, buttonHeight);
            btnBrowse.Location = new Point(pnlSettings.Width - buttonWidth - btnHrzSpace, lblIntro.Bottom + border);

            //TextBox txbDataDirectory
            txbDataDirectory.Width    = btnBrowse.Left - 2 * btnHrzSpace - border;
            txbDataDirectory.Location = new Point(btnHrzSpace + border, btnBrowse.Top + (buttonHeight - txbDataDirectory.Height) / 2);

            int nudLeft = pnlSettings.ClientSize.Width - nudWidth - btnHrzSpace - border;
            nudMarketClose.Size = new Size(nudWidth, buttonHeight);
            nudMarketClose.Location = new Point(nudLeft, btnBrowse.Bottom + border);
            nudMarketOpen.Size = new Size(nudWidth, buttonHeight);
            nudMarketOpen.Location = new Point(nudLeft, nudMarketClose.Bottom + border);

            // Labels
            lblMarketClose.Location = new Point(btnHrzSpace + border, nudMarketClose.Top + 2);
            lblMarketOpen.Location  = new Point(btnHrzSpace + border, nudMarketOpen.Top + 2);

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        delegate void SetInfoTextCallback(string text);
        void SetInfoText(string text)
        {
            if (tbxInfo.InvokeRequired)
            {
                BeginInvoke(new SetInfoTextCallback(SetInfoText), new object[] { text });
            }
            else
            {
                tbxInfo.AppendText(text);
            }
        }

        /// <summary>
        /// Button Browse Click
        /// <summary>
        void BtnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                Configs.JForexDataPath = fd.SelectedPath;
                txbDataDirectory.Text  = fd.SelectedPath;
            }
        }

        /// <summary>
        /// Button Help Click
        /// <summary>
        void BtnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/wiki/fsb/manual/jforex_data");
            }
            catch { }
        }

        /// <summary>
        /// Button Import Click
        /// <summary>
        void BtnImport_Click(object sender, EventArgs e)
        {
            if (isImporting)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            Configs.JForexDataPath = txbDataDirectory.Text;
            Cursor = Cursors.WaitCursor;
            progressBar.Style = ProgressBarStyle.Marquee;
            isImporting = true;
            btnImport.Text = Language.T("Stop");
            Configs.MarketClosingHour = (int)nudMarketClose.Value;
            Configs.MarketOpeningHour   = (int)nudMarketOpen.Value;

            // Start the bgWorker
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            files.Clear();
            ReadJForexFiles();
            foreach (JForex_Data_Files file in files)
                if (!worker.CancellationPending)
                {
                    if (file.Period > 0)
                        ImportBarFile(file);
                    if (file.Period == 0)
                        ImportTicks(file);
                }
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                System.Media.SystemSounds.Exclamation.Play();

            progressBar.Style = ProgressBarStyle.Blocks;
            isImporting = false;
            btnImport.Text = Language.T("Import");
            Cursor = Cursors.Default;

            return;
        }

        void ReadJForexFiles()
        {
            if (!Directory.Exists(txbDataDirectory.Text))
                return;

            string[] dataFiles = Directory.GetFiles(txbDataDirectory.Text);
            foreach (string filePath in dataFiles)
            {
                if(Path.GetExtension(filePath) == ".csv")
                {
                    JForex_Data_Files file = new JForex_Data_Files(filePath);
                    if (file.Period > -1)
                        files.Add(file);
                }
            }
        }

        void ImportBarFile(JForex_Data_Files file)
        {
            StreamReader streamReader = new StreamReader(file.FilePath);
            StreamWriter streamWriter = new StreamWriter(file.FileTargetPath);

            string dateFormat;
            dateFormat = "yyyy.MM.dd HH:mm:ss";

            char fieldSplitter = ',';
            IFormatProvider formatProvider = System.Globalization.CultureInfo.InvariantCulture;
            string line = "";
            int bars = 0;

            try
            {
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine();
                    if (line.StartsWith("Time"))
                        continue; // Skips annotation line. 

                    string[] data = line.Split(new char[] { fieldSplitter });

                    DateTime time   = DateTime.ParseExact(data[0], dateFormat, formatProvider);
                    double   open   = StringToDouble(data[1]);
                    double   high   = StringToDouble(data[2]);
                    double   low    = StringToDouble(data[3]);
                    double   close  = StringToDouble(data[4]);
                    int      volume = (int)StringToDouble(data[5]);

                    if (volume > 0 && !(open == high && open == low && open == close))
                    {
                        streamWriter.WriteLine(
                            time.ToString("yyyy-MM-dd\tHH:mm") + "\t" +
                            open.ToString()  + "\t" +
                            high.ToString()  + "\t" +
                            low.ToString()   + "\t" +
                            close.ToString() + "\t" +
                            volume.ToString()
                        );
                        bars++;
                    }
                }
            }
            catch (Exception excaption)
            {
                MessageBox.Show(excaption.Message);
            }

            streamWriter.Close();
            streamReader.Close();
            SetInfoText(file.Symbol + " " + Data.DataPeriodToString((DataPeriods)file.Period) + " - " +
                (Language.T("Bars")).ToLower() + ": " + bars.ToString() + Environment.NewLine);
        }

        void ImportTicks(JForex_Data_Files file)
        {
            StreamReader streamReader = new StreamReader(file.FilePath);
            FileStream   outStream    = new FileStream(file.FileTargetPath, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(outStream);

            DateTime time = DateTime.MinValue;
            int volume = 0;
            List<double> reccord = new List<double>();
            int count1MinBars = 1;
            int totalVolume   = 0;

            string dateFormat = "yyyy.MM.dd HH:mm:ss";

            char fieldSplitter = ',';

            IFormatProvider formatProvider = System.Globalization.CultureInfo.InvariantCulture;
            string line = "";

            try
            {
                DateTime tickTime;
                double bid;
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine();
                    if (line.StartsWith("Time"))
                        continue; // Skips annotation line. 
                    string[] data = line.Split(new char[] { fieldSplitter });
                    DateTime t = DateTime.ParseExact(data[0], dateFormat, formatProvider);
                    tickTime = new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0);
                    bid = StringToDouble(data[2]);

                    if (tickTime.Minute != time.Minute || volume == 0)
                    {

                        if (volume > 0 && !IsWeekend(time))
                        {
                            FilterReccord(reccord);
                            SaveReccord(binaryWriter, time, volume, reccord);
                            count1MinBars++;
                        }

                        time = tickTime;
                        volume = 0;
                        reccord.Clear();
                        reccord.Add(bid);
                    }
                    else if (reccord.Count > 0 && bid != reccord[reccord.Count - 1])
                    {
                        reccord.Add(bid);
                    }

                    volume++;
                    totalVolume++;
                }
                if (volume > 0 && !IsWeekend(time))
                {
                    FilterReccord(reccord);
                    SaveReccord(binaryWriter, time, volume, reccord);
                    count1MinBars++;
                }
            }
            catch (Exception excaption)
            {
                MessageBox.Show(excaption.Message);
            }

            streamReader.Close();
            binaryWriter.Close();
            outStream.Close();

            volume--;
            totalVolume--;
            count1MinBars--;

            SetInfoText(file.Symbol + " " + Language.T("Ticks") + " - " + (Language.T("Ticks")).ToLower() + ": " + totalVolume.ToString()
                 + " - 1M " + (Language.T("Bars")).ToLower() + ": " + count1MinBars.ToString() + Environment.NewLine);
        }

        bool IsWeekend(DateTime time)
        {
            bool isWeekend = false;

            if (time.DayOfWeek == DayOfWeek.Friday && time.Hour >= Configs.MarketClosingHour)
                isWeekend = true;
            if (time.DayOfWeek == DayOfWeek.Saturday)
                isWeekend = true;
            if (time.DayOfWeek == DayOfWeek.Sunday && time.Hour < Configs.MarketOpeningHour)
                isWeekend = true;

            return isWeekend;
        }

        static void FilterReccord(List<double> reccord)
        {
            int count = reccord.Count;
            if (count < 3)
                return;

            List<double> bids = new List<double>(count - 1);

            bids.Add(reccord[0]);
            bids.Add(reccord[1]);

            int b = 1;
            bool isChanged = false;

            for (int i = 2; i < count; i++)
            {
                double bid = reccord[i];

                if (bids[b - 1] < bids[b] && bids[b] < bid || bids[b - 1] > bids[b] && bids[b] > bid)
                {
                    bids[b] = bid;
                    isChanged = true;
                }
                else
                {
                    bids.Add(bid);
                    b++;
                }
            }

            if (isChanged)
            {
                reccord.Clear();
                foreach (double bid in bids)
                    reccord.Add(bid);
            }
        }

        static void SaveReccord(BinaryWriter binaryWriter, DateTime time, int volume, List<double> reccord)
        {
            int count = reccord.Count;
            if (count < 1)
                return;

            binaryWriter.Write(time.ToBinary());
            binaryWriter.Write(volume);
            binaryWriter.Write(count);
            foreach (double bid in reccord)
                binaryWriter.Write(bid);
        }

        /// <summary>
        /// Converts a string to a double number.
        /// </summary>
        static double StringToDouble(string input)
        {
            string sDecimalPoint = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(sDecimalPoint))
            {
                input = input.Replace(".", sDecimalPoint);
                input = input.Replace(",", sDecimalPoint);
            }

            double number = double.Parse(input);

            return number;
        }

    }

    public class JForex_Data_Files
    {
        string   filePath;
        string   fileName;
        string   fileTargetPath;
        string   symbol;
        int      period;
        DateTime timeUpdate;

        public string FilePath
        {
            get { return filePath; }
        }

        public string FileTargetPath
        {
            get { return fileTargetPath; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public string Symbol
        {
            get { return symbol; }
        }

        public int Period
        {
            get { return period; }
        }

        public DateTime TimeUpdate
        {
            get { return timeUpdate; }
        }

        public JForex_Data_Files(string filePath)
        {
            this.filePath = filePath;
            fileName = Path.GetFileNameWithoutExtension(filePath);
            string[] fields = fileName.Split(new char[] { '_' });

            switch(fields[1])
            {
                case "Ticks":
                    period = 0;
                    break;
                case "1 Min":
                    period = 1;
                    break;
                case "5 Mins":
                    period = 5;
                    break;
                case "15 Mins":
                    period = 15;
                    break;
                case "30 Mins":
                    period = 30;
                    break;
                case "Hourly":
                    period = 60;
                    break;
                case "4 Hours":
                    period = 240;
                    break;
                case "Daily":
                    period = 1440;
                    break;
                case "Weekly":
                    period = 10080;
                    break;
                default:
                    period = -1;
                    break;
            }

            symbol = fields[0];
            IFormatProvider formatProvider = System.Globalization.CultureInfo.InvariantCulture;
            timeUpdate = DateTime.ParseExact(fields[4], "yyyy.MM.dd", formatProvider);
            fileTargetPath = Data.OfflineDataDir + symbol + period.ToString() + (period == 0 ? ".bin" :  ".csv");
        }
    }
}
