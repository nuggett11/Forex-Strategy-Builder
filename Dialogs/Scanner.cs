// Forex Strategy Builder - Scanner
// Last changed on: 2011-01-21
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// The Scanner
    /// </summary>
    public class Scanner : Form
    {
        Panel pnlInfo;
        Small_Balance_Chart smallBalanceChart;
        ProgressBar progressBar;
        Label lblProgress;
        CheckBox chbAutoscan;
        CheckBox chbTickScan;
        Button btnClose;
        ToolTip toolTip = new ToolTip();
        string warningMessage;
        bool isCompactMode = false;
        bool isTickDataFile;

        BackgroundWorker bgWorker;
        int  progressPercent;
        bool isLoadingNow;

        Font fontInfo;
        int  infoRowHeight;

        Font  font;
        Color colorText;

        /// <summary>
        /// Sets scanner compact mode.
        /// </summary>
        public bool CompactMode { set { isCompactMode = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Scanner()
        {
            pnlInfo           = new Panel();
            smallBalanceChart = new Small_Balance_Chart();
            progressBar       = new ProgressBar();
            lblProgress       = new Label();
            chbAutoscan       = new CheckBox();
            chbTickScan       = new CheckBox();
            btnClose          = new Button();

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnClose;
            Text            = Language.T("Intrabar Scanner");
            FormClosing    += new FormClosingEventHandler(Scanner_FormClosing);

            font            = this.Font;
            colorText       = LayoutColors.ColorControlText;
            fontInfo        = new Font(Font.FontFamily, 9);
            infoRowHeight   = (int)Math.Max(fontInfo.Height, 18);
            isTickDataFile  = CheckTickDataFile();

            // pnlInfo
            pnlInfo.Parent    = this;
            pnlInfo.BackColor = LayoutColors.ColorControlBack;
            pnlInfo.Paint    += new PaintEventHandler(PnlInfo_Paint);

            // Small Balance Chart
            smallBalanceChart.Parent = this;
            smallBalanceChart.SetChartData();

            // ProgressBar
            progressBar.Parent = this;

            // Label Progress
            lblProgress.Parent    = this;
            lblProgress.ForeColor = LayoutColors.ColorControlText;
            lblProgress.AutoSize = true;

            // Automatic Scan checkbox.
            chbAutoscan.Parent    = this;
            chbAutoscan.ForeColor = colorText;
            chbAutoscan.BackColor = Color.Transparent;
            chbAutoscan.Text      = Language.T("Automatic Scan");
            chbAutoscan.Checked   = Configs.Autoscan;
            chbAutoscan.AutoSize  = true;
            chbAutoscan.CheckedChanged += new EventHandler(ChbAutoscan_CheckedChanged);

            // Tick Scan checkbox.
            chbTickScan.Parent    = this;
            chbTickScan.ForeColor = colorText;
            chbTickScan.BackColor = Color.Transparent;
            chbTickScan.Text      = Language.T("Use Ticks");
            chbTickScan.Checked   = Configs.UseTickData && isTickDataFile;
            chbTickScan.AutoSize  = true;
            chbTickScan.Visible   = isTickDataFile;
            chbTickScan.CheckedChanged += new EventHandler(ChbTickScan_CheckedChanged);

            //Button Close
            btnClose.Parent       = this;
            btnClose.Name         = "Close";
            btnClose.Text         = Language.T("Close");
            btnClose.DialogResult = DialogResult.OK;
            btnClose.UseVisualStyleBackColor = true;

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress      = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork             += new DoWorkEventHandler(BgWorker_DoWork);
            bgWorker.ProgressChanged    += new ProgressChangedEventHandler(BgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);

            isLoadingNow = false;

            if (!isTickDataFile)
                Configs.UseTickData = false;

            return;
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (isCompactMode)
            {
                pnlInfo.Visible = false;
                smallBalanceChart.Visible = false;
                lblProgress.Visible = true;
                chbAutoscan.Visible = false;

                Width  = 300;
                Height = 95;

                StartLoading();
            }
            else
            {
                lblProgress.Visible = false;
                chbAutoscan.Visible = true;
                Width  = 460;
                Height = 540;
                if (!isTickDataFile)
                    Height -= infoRowHeight;
            }

            return;
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
            int space = btnHrzSpace;

            if (isCompactMode)
            {
                //Button Close
                btnClose.Size     = new Size (buttonWidth, buttonHeight);
                btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

                // ProgressBar
                progressBar.Size     = new Size(ClientSize.Width - 2 * space, (int)(Data.VerticalDLU * 9));
                progressBar.Location = new Point(space, btnVertSpace);

                // Label Progress
                lblProgress.Location = new Point(space, btnClose.Top + 5);
            }
            else
            {
                //Button Close
                btnClose.Size     = new Size (buttonWidth, buttonHeight);
                btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

                // ProgressBar
                progressBar.Size     = new Size(ClientSize.Width - 2 * space, (int)(Data.VerticalDLU * 9));
                progressBar.Location = new Point(space, btnClose.Top - progressBar.Height - btnVertSpace);

                // Panel Info
                int pnlInfoHeight = isTickDataFile ? infoRowHeight * 11 + 2 : infoRowHeight * 10 + 2;
                pnlInfo.Size     = new Size(ClientSize.Width - 2 * space, pnlInfoHeight);
                pnlInfo.Location = new Point(space, space);

                // Panel balance chart
                smallBalanceChart.Size = new Size(ClientSize.Width - 2 * space, progressBar.Top - pnlInfo.Bottom - 2 * space);
                smallBalanceChart.Location = new Point(space, pnlInfo.Bottom + space);

                // Label Progress
                lblProgress.Location = new Point(space, btnClose.Top + 5);

                // Autoscan checkbox
                chbAutoscan.Location = new Point(space, btnClose.Top + 5);

                // TickScan checkbox
                chbTickScan.Location = new Point(chbAutoscan.Right + space, btnClose.Top + 5);
            }

            return;
        }

        /// <summary>
        /// Loads data and recalculates.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (isCompactMode)
                return;

            if (!Data.IsIntrabarData)
            {
                StartLoading();
            }
            else
            {
                Backtester.Scan();
                ShowScanningResult();
                progressBar.Value = 100;
                btnClose.Focus();
            }

            return;
        }

        /// <summary>
        /// Stops the background worker.
        /// </summary>
        void Scanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isLoadingNow)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                e.Cancel = true;
            }

            return;
        }

        /// <summary>
        /// Repaint the panel Info
        /// </summary>
        void PnlInfo_Paint(object sender, PaintEventArgs e)
        {
            // +------------------------------------------------------+
            // |                   Data                               |
            // |------------------- ----------------------------------+
            // | Period  | Bars  | From | Until | Cover |  %  | Label |
            // |------------------------------------------------------+
            //xp0       xp1     xp2    xp3     xp4     xp5   xp6     xp7

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (!Data.IsData || !Data.IsResult) return;

            Panel  pnl = (Panel)sender;
            string FF  = Data.FF; // Format modifier to print the numbers
            int border = 2;
            int xp0 = border;
            int xp1 = 80;
            int xp2 = 140;
            int xp3 = 200;
            int xp4 = 260;
            int xp5 = 320;
            int xp6 = 370;
            int xp7 = pnl.ClientSize.Width - border;

            Size size = new Size(xp7 - xp0, infoRowHeight);

            StringFormat sf  = new StringFormat();
            sf.Alignment     = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;

            // Caption background
            PointF pntStart   = new PointF(0, 0);
            SizeF  szfCaption = new Size(pnl.ClientSize.Width - 0, 2 * infoRowHeight);
            RectangleF rectfCaption = new RectangleF(pntStart, szfCaption);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            StringFormat stringFormatCaption  = new StringFormat();
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;
            stringFormatCaption.Alignment     = StringAlignment.Near;
            string stringCaptionText = Language.T("Intrabar Data");
            float  captionWidth      = Math.Min(pnlInfo.ClientSize.Width, xp7 - xp0);
            float  captionTextWidth  = g.MeasureString(stringCaptionText, fontInfo).Width;
            float  captionTextX      = Math.Max((captionWidth - captionTextWidth) / 2f, 0);
            PointF pfCaptionText     = new PointF(captionTextX, 0);
            SizeF  sfCaptionText     = new SizeF(captionWidth - captionTextX, infoRowHeight);
            rectfCaption = new RectangleF(pfCaptionText, sfCaptionText);

            Brush brush = new SolidBrush(LayoutColors.ColorCaptionText);
            // First caption row
            g.DrawString(stringCaptionText, fontInfo, brush, rectfCaption, stringFormatCaption);

            // Second title row
            g.DrawString(Language.T("Period"),   fontInfo, brush, (xp1 + xp0) / 2, infoRowHeight, sf);
            g.DrawString(Language.T("Bars"),     fontInfo, brush, (xp2 + xp1) / 2, infoRowHeight, sf);
            g.DrawString(Language.T("From"),     fontInfo, brush, (xp3 + xp2) / 2, infoRowHeight, sf);
            g.DrawString(Language.T("Until"),    fontInfo, brush, (xp4 + xp3) / 2, infoRowHeight, sf);
            g.DrawString(Language.T("Coverage"), fontInfo, brush, (xp5 + xp4) / 2, infoRowHeight, sf);
            g.DrawString("%",                    fontInfo, brush, (xp6 + xp5) / 2, infoRowHeight, sf);
            g.DrawString(Language.T("Label"),    fontInfo, brush, (xp7 + xp6) / 2, infoRowHeight, sf);

            brush = new SolidBrush(LayoutColors.ColorControlText);
            int allPeriods = Enum.GetValues(typeof(DataPeriods)).Length;
            for (int iPeriod = 0; iPeriod <= allPeriods; iPeriod++)
            {
                int y = (iPeriod + 2) * infoRowHeight;
                Point point = new Point(xp0, y);

                if (iPeriod % 2f != 0)
                    g.FillRectangle(new SolidBrush(LayoutColors.ColorEvenRowBack), new Rectangle(point, size));
            }

            // Tick statistics
            if (isTickDataFile)
            {
                g.DrawString(Language.T("Tick"), fontInfo, brush, (xp1 + xp0) / 2, 2 * infoRowHeight, sf);
                if (Data.IsTickData && Configs.UseTickData)
                {
                    int firstBarWithTicks = -1;
                    int lastBarWithTicks = -1;
                    int tickBars = 0;
                    for (int b = 0; b < Data.Bars; b++)
                    {
                        if (firstBarWithTicks == -1 && Data.TickData[b] != null)
                            firstBarWithTicks = b;
                        if (Data.TickData[b] != null)
                        {
                            lastBarWithTicks = b;
                            tickBars++;
                        }
                    }
                    double percentage = 100d * tickBars / Data.Bars;

                    int y = 2 * infoRowHeight;
                    string ticks = (Data.Ticks > 999999) ? (Data.Ticks / 1000).ToString() + "K" : Data.Ticks.ToString();
                    g.DrawString(ticks, fontInfo, brush, (xp2 + xp1) / 2, y, sf);
                    g.DrawString((firstBarWithTicks + 1).ToString(), fontInfo, brush, (xp3 + xp2) / 2, y, sf);
                    g.DrawString((lastBarWithTicks + 1).ToString(), fontInfo, brush, (xp4 + xp3) / 2, y, sf);
                    g.DrawString(tickBars.ToString(), fontInfo, brush, (xp5 + xp4) / 2, y, sf);
                    g.DrawString(percentage.ToString("F2"), fontInfo, brush, (xp6 + xp5) / 2, y, sf);

                    RectangleF rectf = new RectangleF(xp6 + 10, y + 4, xp7 - xp6 - 20, 9);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.min1], 60);
                    rectf = new RectangleF(xp6 + 10, y + 7, xp7 - xp6 - 20, 3);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.day], 60);
                }
            }

            for (int iPeriod = 0; iPeriod < allPeriods; iPeriod++)
            {
                int startY = isTickDataFile ? 3 : 2;
                int y = (iPeriod + startY) * infoRowHeight;
                Point point = new Point(xp0, y);

                DataPeriods period = (DataPeriods)Enum.GetValues(typeof(DataPeriods)).GetValue(iPeriod);
                int intraBars   = Data.IntraBars == null || !Data.IsIntrabarData ? 0 : Data.IntraBars[iPeriod];
                int fromBar     = 0;
                int untilBar    = 0;
                int coveredBars = 0;
                double percentage = 0;

                bool isMultyAreas = false;
                if (intraBars > 0)
                {
                    bool isFromBarFound = false;
                    bool isUntilBarFound = false;
                    untilBar = Data.Bars;
                    for (int bar = 0; bar < Data.Bars; bar++)
                    {
                        if (!isFromBarFound && Data.IntraBarsPeriods[bar] == period)
                        {
                            fromBar = bar;
                            isFromBarFound = true;
                        }
                        if (isFromBarFound && !isUntilBarFound &&
                            (Data.IntraBarsPeriods[bar] != period || bar == Data.Bars - 1))
                        {
                            if (bar < Data.Bars - 1)
                            {
                                isUntilBarFound = true;
                                untilBar = bar;
                            }
                            else
                            {
                                untilBar = Data.Bars;
                            }
                            coveredBars = untilBar - fromBar;
                        }
                        if (isFromBarFound && isUntilBarFound && Data.IntraBarsPeriods[bar] == period)
                        {
                            isMultyAreas = true;
                            coveredBars++;
                        }
                    }
                    if (isFromBarFound)
                    {
                        percentage = 100d * coveredBars / Data.Bars;
                        fromBar++;
                    }
                    else
                    {
                        fromBar     = 0;
                        untilBar    = 0;
                        coveredBars = 0;
                        percentage  = 0;
                    }
                }
                else if (period == Data.Period)
                {
                    intraBars   = Data.Bars;
                    fromBar     = 1;
                    untilBar    = Data.Bars;
                    coveredBars = Data.Bars;
                    percentage  = 100;
                }

                g.DrawString(Data.DataPeriodToString(period), fontInfo, brush, (xp1 + xp0) / 2, y, sf);

                if (coveredBars > 0 || period == Data.Period)
                {
                    g.DrawString(intraBars.ToString(), fontInfo, brush, (xp2 + xp1) / 2, y, sf);
                    g.DrawString(fromBar.ToString(),   fontInfo, brush, (xp3 + xp2) / 2, y, sf);
                    g.DrawString(untilBar.ToString(),  fontInfo, brush, (xp4 + xp3) / 2, y, sf);
                    g.DrawString(coveredBars.ToString() + (isMultyAreas ? "*" : ""), fontInfo, brush, (xp5 + xp4) / 2, y, sf);
                    g.DrawString(percentage.ToString("F2"), fontInfo, brush, (xp6 + xp5) / 2, y, sf);

                    RectangleF rectf = new RectangleF(xp6 + 10, y + 4, xp7 - xp6 - 20, 9);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[period], 60);
                }
            }

            Pen penLine = new Pen(LayoutColors.ColorJournalLines);
            g.DrawLine(penLine, xp1, 2 * infoRowHeight, xp1, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp2, 2 * infoRowHeight, xp2, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp3, 2 * infoRowHeight, xp3, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp4, 2 * infoRowHeight, xp4, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp5, 2 * infoRowHeight, xp5, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp6, 2 * infoRowHeight, xp6, pnl.ClientSize.Height);

            // Border
            Pen penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);
            g.DrawLine(penBorder, 1, 2 * infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, 2 * infoRowHeight, pnl.ClientSize.Width - border + 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width, pnl.ClientSize.Height - border + 1);

            return;
        }

        /// <summary>
        /// Starts intrabar data loading.
        /// </summary>
        void StartLoading()
        {
            if (isLoadingNow)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            Cursor              = Cursors.WaitCursor;
            progressBar.Value   = 0;
            warningMessage      = string.Empty;
            isLoadingNow        = true;
            progressPercent     = 0;
            lblProgress.Visible = true;
            chbAutoscan.Visible = false;
            chbTickScan.Visible = false;

            btnClose.Text = Language.T("Cancel");

            // Start the bgWorker
            bgWorker.RunWorkerAsync();

            return;
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            LoadData(worker, e);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 200)
                progressBar.Style = ProgressBarStyle.Marquee;
            else
                progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (Data.IsIntrabarData ||
                Configs.UseTickData && Data.IsTickData ||
                Data.Period == DataPeriods.min1)
                Backtester.Scan();

            ShowScanningResult();

            if (warningMessage != string.Empty && Configs.CheckData)
                MessageBox.Show(warningMessage + Environment.NewLine + Environment.NewLine +
                    Language.T("Probably the data is incomplete and the scanning may not be reliable!") + Environment.NewLine +
                    Language.T("You can try also \"Cut Off Bad Data\"."),
                    Language.T("Scanner"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            progressBar.Style = ProgressBarStyle.Blocks;

            if (isCompactMode)
                Close();

            return;
        }

        /// <summary>
        /// Updates the Generator interface.
        /// </summary>
        void ShowScanningResult()
        {
            smallBalanceChart.SetChartData();
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();
            pnlInfo.Invalidate();

            lblProgress.Text    = string.Empty;
            lblProgress.Visible = false;
            chbAutoscan.Visible = true;
            chbTickScan.Visible = Configs.UseTickData || isTickDataFile;

            btnClose.Text = Language.T("Close");
            Cursor        = Cursors.Default;
            isLoadingNow  = false;
            btnClose.Focus();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        void LoadData(BackgroundWorker worker, DoWorkEventArgs e)
        {
            int  periodsToLoad    = 0;
            int  allPeriods       = Enum.GetValues(typeof(DataPeriods)).Length;
            Data.IntraBars        = new int[allPeriods];
            Data.IntraBarData     = new Bar[Data.Bars][];
            Data.IntraBarBars     = new int[Data.Bars];
            Data.IntraBarsPeriods = new DataPeriods[Data.Bars];
            Data.LoadedIntraBarPeriods = 0;

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                Data.IntraBarsPeriods[bar] = Data.Period;
                Data.IntraBarBars[bar] = 0;
            }

            // Counts how many periods to load
            for (int prd = 0; prd < allPeriods; prd++)
            {
                DataPeriods period = (DataPeriods)Enum.GetValues(typeof(DataPeriods)).GetValue(prd);
                if (period < Data.Period)
                {
                    periodsToLoad++;
                }
            }

            // Load the intrabar data (Starts from 1 Min)
            for (int prd = 0; prd < allPeriods && isLoadingNow; prd++)
            {
                if (worker.CancellationPending) break;

                int loadedBars = 0;
                DataPeriods period = (DataPeriods)Enum.GetValues(typeof(DataPeriods)).GetValue(prd);

                SetLabelProgressText(Language.T("Loading:") + " " + Data.DataPeriodToString(period) + "...");

                if (period < Data.Period)
                {
                    loadedBars = LoadIntrabarData(period);
                    if (loadedBars > 0)
                    {
                        Data.IsIntrabarData = true;
                        Data.LoadedIntraBarPeriods++;
                    }
                }
                else if (period == Data.Period)
                {
                    loadedBars = Data.Bars;
                    Data.LoadedIntraBarPeriods++;
                }

                Data.IntraBars[prd] = loadedBars;

                // Report progress as a percentage of the total task.
                int percentComplete = periodsToLoad > 0 ? 100 * (prd + 1) / periodsToLoad : 100 ;
                percentComplete = percentComplete > 100 ? 100 : percentComplete;
                if (percentComplete > progressPercent)
                {
                    progressPercent = percentComplete;
                    worker.ReportProgress(percentComplete);
                }
            }

            CheckIntrabarData();
            RepairIntrabarData();

            if (Configs.UseTickData)
            {
                SetLabelProgressText(Language.T("Loading:") + " " + Language.T("Ticks") + "...");
                worker.ReportProgress(200);
                try
                {
                    LoadTickData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return;
        }

        /// <summary>
        /// Loads the Intrabar data.
        /// </summary>
        int LoadIntrabarData(DataPeriods period)
        {
            Instrument instrument = new Instrument(Data.InstrProperties.Clone(), (int)period);

            instrument.DataDir    = Data.OfflineDataDir;
            instrument.FormatDate = DateFormat.Unknown;
            instrument.MaxBars    = Configs.MAX_INTRA_BARS;

            // Loads the data
            int loadingResult   = instrument.LoadData();
            int loadedIntrabars = instrument.Bars;

            if (loadingResult == 0 && loadedIntrabars > 0)
            {
                if (Data.Period != DataPeriods.week)
                {
                    if (instrument.DaysOff > 5)
                        warningMessage += Environment.NewLine + Language.T("Data for:") + " " + Data.Symbol + " " +
                            Data.DataPeriodToString(period) + " - " + Language.T("Maximum days off:") + " " + instrument.DaysOff;
                    if (Data.Update - instrument.Update > new TimeSpan(24, 0, 0))
                        warningMessage += Environment.NewLine + Language.T("Data for:") + " " + Data.Symbol + " " +
                            Data.DataPeriodToString(period) + " - " + Language.T("Updated on:") + " " + instrument.Update.ToString();
                }

                int iStartBigBar = 0;
                for (iStartBigBar = 0; iStartBigBar < Data.Bars; iStartBigBar++)
                    if (Data.Time[iStartBigBar] >= instrument.Time(0))
                        break;

                int iStopBigBar = 0;
                for (iStopBigBar = iStartBigBar; iStopBigBar < Data.Bars; iStopBigBar++)
                    if (Data.IntraBarsPeriods[iStopBigBar] != Data.Period)
                        break;

                // Seek for the place
                int iReachedBar  = 0;
                for (int bar = iStartBigBar; bar < iStopBigBar; bar++)
                {
                    Data.IntraBarData[bar] = new Bar[(int)Data.Period/(int)period];
                    DateTime endTime = Data.Time[bar] + new TimeSpan(0, (int)Data.Period, 0);
                    int iCurrentBar = 0;
                    for (int intrabar = iReachedBar; intrabar < loadedIntrabars && instrument.Time(intrabar) < endTime; intrabar++)
                    {
                        if (instrument.Time(intrabar) >= Data.Time[bar])
                        {
                            Data.IntraBarData[bar][iCurrentBar].Time  = instrument.Time(intrabar);
                            Data.IntraBarData[bar][iCurrentBar].Open  = instrument.Open(intrabar);
                            Data.IntraBarData[bar][iCurrentBar].High  = instrument.High(intrabar);
                            Data.IntraBarData[bar][iCurrentBar].Low   = instrument.Low(intrabar);
                            Data.IntraBarData[bar][iCurrentBar].Close = instrument.Close(intrabar);
                            Data.IntraBarsPeriods[bar] = period;
                            Data.IntraBarBars[bar]++;
                            iCurrentBar++;
                            iReachedBar = intrabar;
                        }
                    }
                }
            }

            return loadedIntrabars;
        }

        /// <summary>
        /// Checks the intrabar data.
        /// </summary>
        void CheckIntrabarData()
        {
            int inraBarDataStarts = 0;
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                if (inraBarDataStarts == 0 && Data.IntraBarsPeriods[bar] != Data.Period)
                    inraBarDataStarts = bar;

                if (inraBarDataStarts > 0 && Data.IntraBarsPeriods[bar] == Data.Period)
                {
                    inraBarDataStarts = 0;
                    warningMessage += Environment.NewLine +
                        Language.T("There is no intrabar data from bar No:") + " " +
                        (bar + 1) + " - " + Data.Time[bar].ToString();
                }
            }

            return;
        }

        /// <summary>
        /// Repairs the intrabar data.
        /// </summary>
        void RepairIntrabarData()
        {
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                if (Data.IntraBarsPeriods[bar] != Data.Period)
                {   // We have intrabar data here

                    // Repair the Opening prices
                    double price = Data.Open[bar];
                    int b = 0;
                    Data.IntraBarData[bar][b].Open = Data.Open[bar];
                    if (price > Data.IntraBarData[bar][b].High &&
                        price > Data.IntraBarData[bar][b].Low)
                    {   // Adjust the High price
                        Data.IntraBarData[bar][b].High = price;
                    }
                    else if (price < Data.IntraBarData[bar][b].High &&
                             price < Data.IntraBarData[bar][b].Low)
                    {   // Adjust the Low price
                        Data.IntraBarData[bar][b].Low = price;
                    }

                    // Repair the Closing prices
                    price = Data.Close[bar];
                    b = Data.IntraBarBars[bar] - 1;
                    Data.IntraBarData[bar][b].Close = Data.Close[bar];
                    if (price > Data.IntraBarData[bar][b].High &&
                        price > Data.IntraBarData[bar][b].Low)
                    {   // Adjust the High price
                        Data.IntraBarData[bar][b].High = price;
                    }
                    else if (price < Data.IntraBarData[bar][b].High &&
                        price < Data.IntraBarData[bar][b].Low)
                    {   // Adjust the Low price
                        Data.IntraBarData[bar][b].Low = price;
                    }

                    int iMinIntrabar = -1; // Contains the min price
                    int iMaxIntrabar = -1; // Contains the max price
                    double dMinPrice = double.MaxValue;
                    double dMaxPrice = double.MinValue;

                    for (b = 0; b < Data.IntraBarBars[bar]; b++)
                    {   // Find min and max
                        if (Data.IntraBarData[bar][b].Low < dMinPrice)
                        {   // Min found
                            dMinPrice = Data.IntraBarData[bar][b].Low;
                            iMinIntrabar = b;
                        }
                        if (Data.IntraBarData[bar][b].High > dMaxPrice)
                        {   // Max found
                            dMaxPrice = Data.IntraBarData[bar][b].High;
                            iMaxIntrabar = b;
                        }
                        if (b > 0)
                        {   // Repair the Opening prices
                            price = Data.IntraBarData[bar][b - 1].Close;
                            Data.IntraBarData[bar][b].Open = price;
                            if (price > Data.IntraBarData[bar][b].High &&
                                price > Data.IntraBarData[bar][b].Low)
                            {   // Adjust the High price
                                Data.IntraBarData[bar][b].High = price;
                            }
                            else if (price < Data.IntraBarData[bar][b].High &&
                                     price < Data.IntraBarData[bar][b].Low)
                            {   // Adjust the Low price
                                Data.IntraBarData[bar][b].Low = price;
                            }
                        }
                    }

                    if (dMinPrice > Data.Low[bar]) // Repair the Bottom
                        Data.IntraBarData[bar][iMinIntrabar].Low = Data.Low[bar];
                    if (dMaxPrice < Data.High[bar]) // Repair the Top
                        Data.IntraBarData[bar][iMaxIntrabar].High = Data.High[bar];
                }
            }

            return;
        }

        /// <summary>
        /// Loads available tick data.
        /// </summary>
        void LoadTickData()
        {
            FileStream   fileStream   = new FileStream(Data.OfflineDataDir + Data.Symbol + "0.bin", FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            Data.TickData = new double[Data.Bars][];
            int bar = 0;

            long totalVolume = 0;
            int  min1Bars    = 0;

            long pos    = 0;
            long length = binaryReader.BaseStream.Length;
            while (pos < length)
            {
                DateTime time = DateTime.FromBinary(binaryReader.ReadInt64());
                pos += sizeof(Int64);

                int volume = binaryReader.ReadInt32();
                pos += sizeof(Int32);

                int count = binaryReader.ReadInt32();
                pos += sizeof(Int32);

                double[] bid = new double[count];
                for (int i = 0; i < count; i++)
                    bid[i] = binaryReader.ReadDouble();
                pos += count * sizeof(Double);

                while (bar < Data.Bars - 1 && Data.Time[bar] < time)
                {
                    if (time < Data.Time[bar + 1])
                        break;
                    bar++;
                }

                if (time == Data.Time[bar])
                {
                    Data.TickData[bar] = bid;
                }
                else if ((bar < Data.Bars - 1 && time > Data.Time[bar] && time < Data.Time[bar + 1]) || bar == Data.Bars - 1)
                {
                    if (Data.TickData[bar] == null && (Math.Abs(Data.Open[bar] - bid[0]) < 10 * Data.InstrProperties.Pip))
                        Data.TickData[bar] = bid;
                    else
                        AddTickData(bar, bid);
                }

                totalVolume += volume;
                min1Bars++;
            }

            binaryReader.Close();
            fileStream.Close();

            Data.IsTickData = false;
            int barsWithTicks = 0;
            for (int b = 0; b < Data.Bars; b++)
                if (Data.TickData[b] != null)
                    barsWithTicks++;

            if (barsWithTicks > 0)
            {
                Data.Ticks = totalVolume;
                Data.IsTickData = true;
            }
        }

        /// <summary>
        /// Determines whether a tick data file exists.
        /// </summary>
        bool CheckTickDataFile()
        {
            return File.Exists(Data.OfflineDataDir + Data.Symbol + "0.bin");
        }

        /// <summary>
        /// Adds tick data to Data
        /// </summary>
        void AddTickData(int bar, double[] bid)
        {
            if (Data.TickData[bar] != null)
            {
                int oldLenght = Data.TickData[bar].Length;
                int ticksAdd = bid.Length;
                Array.Resize<double>(ref Data.TickData[bar], oldLenght + ticksAdd);
                Array.Copy(bid, 0, Data.TickData[bar], oldLenght, ticksAdd);
            }
        }

        /// <summary>
        /// Export tick data to a .CSV file.
        /// </summary>
        void ExportTickToCSV()
        {
            bool showEmpty = true;
            StreamWriter sw = new StreamWriter(Data.OfflineDataDir + Data.Symbol + "0.csv");
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                if (Data.TickData[bar] != null)
                {
                    sw.Write((bar + 1).ToString() + "\t" + Data.Time[bar].ToString("yyyy-MM-dd HH:mm") + "\t");
                    foreach (double t in Data.TickData[bar])
                        sw.Write(t.ToString("F5") + "\t");
                    sw.WriteLine();
                }
                else if (showEmpty)
                {
                    sw.WriteLine((bar + 1).ToString() + "\t" + Data.Time[bar].ToString("yyyy-MM-dd HH:mm") + "\t" + Data.Time[bar].DayOfWeek.ToString());
                }
            }
            sw.Close();
        }

        delegate void SetLabelProgressCallback(string text);
        /// <summary>
        /// Sets the lblProgress.Text.
        /// </summary>
        void SetLabelProgressText(string text)
        {
            if (lblProgress.InvokeRequired)
            {
                Invoke(new SetLabelProgressCallback(SetLabelProgressText), new object[] { text });
            }
            else
            {
                lblProgress.Text = text;
            }
        }

        /// <summary>
        /// Autoscan checkbox.
        /// </summary>
        void ChbAutoscan_CheckedChanged(object sender, EventArgs e)
        {
            Configs.Autoscan = chbAutoscan.Checked;

            return;
        }

        /// <summary>
        /// Tick scan checkbox.
        /// </summary>
        void ChbTickScan_CheckedChanged(object sender, EventArgs e)
        {
            Configs.UseTickData = chbTickScan.Checked;
            StartLoading();

            return;
        }
    }
}
