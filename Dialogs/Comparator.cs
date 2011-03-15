// Forex Strategy Builder - Comparator
// Part of Forex Strategy Builde
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    class Comparator : Form
    {
        Panel         pnlOptions;
        CheckBox[]    achboxMethods;
        Label         lblAverageBalance;
        Panel         pnlChart;
        ProgressBar   progressBar;
        NumericUpDown numRandom;
        Label         lblRandomCycles;
        Button        btnCalculate;
        Button        btnClose;

        bool     isPaintChart;
        bool     isRandom;
        int      countMethods;
        int      checkedMethods;
        int      lines;
        float[,] afMethods;
        float[,] afRandoms;
        float[]  afBalance;
        float[]  afMinRandom;
        float[]  afMaxRandom;
        float    minimumRandom;
        float    maximumRandom;
        float    minimum;
        float    maximum;

        Pen   penOptimistic;
        Pen   penPessimistic;
        Pen   penShortest;
        Pen   penNearest;
        Pen   penRandom;
        Pen   penRandBands;
        Pen   penBalance;
        Brush brushRandArea;

        BackgroundWorker bgWorker;
        bool bIsWorking; // It is true when the comparator is running

        bool bTradeUntilMC = Configs.TradeUntilMarginCall;

        /// <summary>
        /// Initialize the form and controls
        /// </summary>
        public Comparator()
        {
            pnlOptions        = new Panel();
            pnlChart          = new Panel();
            progressBar       = new ProgressBar();
            lblAverageBalance = new Label();
            numRandom         = new NumericUpDown();
            lblRandomCycles   = new Label();
            btnCalculate      = new Button();
            btnClose          = new Button();

            Text            = Language.T("Comparator");
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon            = Data.Icon;
            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            FormClosing    += new FormClosingEventHandler(Actions_FormClosing);

            isPaintChart = false;

            //Button Calculate
            btnCalculate.Parent = this;
            btnCalculate.Name   = "btnCalculate";
            btnCalculate.Text   = Language.T("Calculate");
            btnCalculate.Click += new EventHandler(BtnCalculate_Click);
            btnCalculate.UseVisualStyleBackColor = true;

            //Button Close
            btnClose.Parent       = this;
            btnClose.Name         = "btnClose";
            btnClose.Text         = Language.T("Close");
            btnClose.DialogResult = DialogResult.OK;
            btnClose.UseVisualStyleBackColor = true;

            // ProgressBar
            progressBar.Parent  = this;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;
            progressBar.Step    = 1;

            pnlChart.Parent    = this;
            pnlChart.ForeColor = LayoutColors.ColorControlText;
            pnlChart.Paint    += new PaintEventHandler(PnlChart_Paint);

            pnlOptions.Parent    = this;
            pnlOptions.ForeColor = LayoutColors.ColorControlText;
            pnlOptions.Paint    += new PaintEventHandler(PnlOptions_Paint);

            countMethods  = Enum.GetValues(typeof (InterpolationMethod)).Length;
            achboxMethods = new CheckBox[countMethods];
            for (int i = 0; i < countMethods; i++)
            {
                achboxMethods[i] = new CheckBox();
                achboxMethods[i].Parent    = pnlOptions;
                achboxMethods[i].Text      = Language.T(Enum.GetNames(typeof(InterpolationMethod))[i]);
                achboxMethods[i].Tag       = Enum.GetValues(typeof(InterpolationMethod)).GetValue(i);
                achboxMethods[i].Checked   = true;
                achboxMethods[i].BackColor = Color.Transparent;
                achboxMethods[i].AutoSize  = true;
                achboxMethods[i].CheckedChanged += new EventHandler(Comparator_CheckedChanged);
            }

            // Label Average Balance
            lblAverageBalance.Parent    = pnlOptions;
            lblAverageBalance.AutoSize  = true;
            lblAverageBalance.Text      = Language.T("Average balance");
            lblAverageBalance.ForeColor = LayoutColors.ColorControlText;
            lblAverageBalance.BackColor = Color.Transparent;
            lblAverageBalance.TextAlign = ContentAlignment.MiddleLeft;

            // NumUpDown random cycles
            numRandom.BeginInit();
            numRandom.Parent    = this;
            numRandom.Value     = 25;
            numRandom.Minimum   = 3;
            numRandom.Maximum   = 100;
            numRandom.TextAlign = HorizontalAlignment.Center;
            numRandom.EndInit();

            // Label Random Cycles
            lblRandomCycles.Parent    = this;
            lblRandomCycles.AutoSize  = true;
            lblRandomCycles.ForeColor = LayoutColors.ColorControlText;
            lblRandomCycles.BackColor = Color.Transparent;
            lblRandomCycles.Text      = Language.T("Random iterations");
            lblRandomCycles.TextAlign = ContentAlignment.MiddleLeft;

            // Colors
            penOptimistic  = new Pen(LayoutColors.ComparatorChartOptimisticLine);
            penPessimistic = new Pen(LayoutColors.ComparatorChartPessimisticLine);
            penShortest    = new Pen(LayoutColors.ComparatorChartShortestLine);
            penNearest     = new Pen(LayoutColors.ComparatorChartNearestLine);
            penRandom      = new Pen(LayoutColors.ComparatorChartRandomLine);
            penRandBands   = new Pen(LayoutColors.ComparatorChartRandomBands);
            brushRandArea  = new SolidBrush(LayoutColors.ComparatorChartRandomArea);
            penBalance     = new Pen(LayoutColors.ComparatorChartBalanceLine);
            penBalance.Width = 2;

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress      = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork             += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.ProgressChanged    += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

            Configs.TradeUntilMarginCall = false;

            return;
        }

        /// <summary>
        /// Resizes the form
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Width  = (int)(Data.HorizontalDLU * 290);
            Height = (int)(Data.VerticalDLU   * 260);

            return;
        }

        /// <summary>
        /// Arrange the controls
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space        = btnHrzSpace;
            int controlZoneH = buttonHeight + 2 * btnVertSpace;
            int controlZoneY = ClientSize.Height - controlZoneH;
            int buttonY      = controlZoneY + btnVertSpace;

            int width = ClientSize.Width - 2 * space;

            pnlOptions.Size     = new Size(ClientSize.Width - 2 * space, 90);
            pnlOptions.Location = new Point(space, space);

            int lenght    = (int)(Data.HorizontalDLU * 60);
            int positionX = (pnlOptions.ClientSize.Width - 10) / 3;
            int positionY = 27;
            int num = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (num < countMethods)
                        achboxMethods[num].Location = new Point(j * positionX + 40, i * 30 + positionY);
                    else
                        lblAverageBalance.Location = new Point(j * positionX + 40, i * 30 + positionY + 1);

                    num++;
                }
            }

            numRandom.Size           = new Size(50, buttonHeight);
            numRandom.Location       = new Point(btnHrzSpace, controlZoneY + (controlZoneH - numRandom.Height) / 2);
            lblRandomCycles.Location = new Point(numRandom.Right + 5, controlZoneY + (controlZoneH - lblRandomCycles.Height) / 2);

            btnClose.Size     = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, buttonY);

            btnCalculate.Size     = new Size(buttonWidth, buttonHeight);
            btnCalculate.Location = new Point(btnClose.Left - buttonWidth - btnHrzSpace, buttonY);

            progressBar.Size     = new Size(ClientSize.Width - 2 * space, (int)(Data.VerticalDLU * 9));
            progressBar.Location = new Point(space, btnClose.Top - progressBar.Height - btnVertSpace);
            pnlChart.Size     = new Size(ClientSize.Width - 2 * space, progressBar.Top - pnlOptions.Bottom - 2 * space);
            pnlChart.Location = new Point(space, pnlOptions.Bottom + space);

            return;
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        void Actions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bIsWorking)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                e.Cancel = true;
            }

            Configs.TradeUntilMarginCall = bTradeUntilMC;

            return;
        }

        /// <summary>
        /// A check boxes status
        /// </summary>
        void Comparator_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbox = (CheckBox)sender;
            InterpolationMethod interpMethod = (InterpolationMethod) chbox.Tag;

            if (interpMethod == InterpolationMethod.Random)
            {
                numRandom.Enabled = chbox.Checked;
            }

            return;
        }

        /// <summary>
        /// Calculate
        /// </summary>
        void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (bIsWorking)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            Cursor = Cursors.WaitCursor;
            progressBar.Value = 1;
            bIsWorking        = true;
            btnClose.Enabled  = false;
            btnCalculate.Text = Language.T("Stop");

            for (int m = 0; m < countMethods; m++)
            {
                achboxMethods[m].Enabled = false;
            }
            numRandom.Enabled = false;

            // Start the bgWorker
            bgWorker.RunWorkerAsync();

            return;
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            progressBar.Value = 1;

            // Optimize all Parameters
            if (Calculate(worker, e) == 0)
            {
                isPaintChart = true;
                pnlChart.Invalidate();
            }

            return;
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;

            return;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bIsWorking        = false;
            btnClose.Enabled  = true;
            btnCalculate.Text = Language.T("Calculate");

            for (int m = 0; m < countMethods; m++)
            {
                achboxMethods[m].Enabled = true;
            }
            numRandom.Enabled = true;

            Cursor = Cursors.Default;
            btnClose.Focus();

            return;
        }

        /// <summary>
        /// Calculates the balance lines
        /// </summary>
        int Calculate(BackgroundWorker worker, DoWorkEventArgs e)
        {
            // Determine the number of lines
            // For each method per line
            // The random line shows the averaged values
            // Also we have two border lines for the random method
            // Plus the average balance line

            isRandom      = false;
            minimum       = float.MaxValue;
            maximum       = float.MinValue;
            minimumRandom = float.MaxValue;
            maximumRandom = float.MinValue;
            int iRandomLines = (int) numRandom.Value;

            checkedMethods = 0;
            lines = 1;
            for (int m = 0; m < countMethods; m++)
                if (achboxMethods[m].Checked)
                {
                    checkedMethods++;
                    lines++;
                    if ((InterpolationMethod)achboxMethods[m].Tag == InterpolationMethod.Random)
                        isRandom = true;
                }

            if (checkedMethods == 0 && Configs.PlaySounds)
            {
                System.Media.SystemSounds.Hand.Play();
                return -1;
            }

            afBalance = new float[Data.Bars - Data.FirstBar];
            afMethods = new float[countMethods, Data.Bars - Data.FirstBar];
            if (isRandom)
            {
                afRandoms   = new float[iRandomLines, Data.Bars - Data.FirstBar];
                afMinRandom = new float[Data.Bars - Data.FirstBar];
                afMaxRandom = new float[Data.Bars - Data.FirstBar];
            }

            // Progress params
            int computedCycles = 0;
            int cycles = lines + (isRandom ? iRandomLines : 0);
            int highestPercentageReached = 0;
            int percentComplete = 0;

            // Calculates the lines
            for (int m = 0; m < countMethods; m++)
            {
                if (worker.CancellationPending) return -1;
                if (!achboxMethods[m].Checked) continue;

                InterpolationMethod method = (InterpolationMethod)achboxMethods[m].Tag;

                if (method == InterpolationMethod.Random)
                {
                    for (int r = 0; r < iRandomLines; r++)
                    {
                        if (worker.CancellationPending) return -1;

                        Backtester.InterpolationMethod = method;
                        Backtester.Calculate();

                        if (Configs.AccountInMoney)
                            for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                                afRandoms[r, iBar] = (float)Backtester.MoneyBalance(iBar + Data.FirstBar);
                        else
                            for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                                afRandoms[r, iBar] = (float)Backtester.Balance(iBar + Data.FirstBar);


                        // Report progress as a percentage of the total task.
                        computedCycles++;
                        percentComplete = 100 * computedCycles / cycles;
                        percentComplete = percentComplete > 100 ? 100 : percentComplete;
                        if (percentComplete > highestPercentageReached)
                        {
                            highestPercentageReached = percentComplete;
                            worker.ReportProgress(percentComplete);
                        }
                    }

                    for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                    {
                        float randomSum = 0;
                        float value;
                        float minRandom = float.MaxValue;
                        float maxRandom = float.MinValue;
                        for (int r = 0; r < iRandomLines; r++)
                        {
                            value = afRandoms[r, iBar];
                            randomSum += value;
                            minRandom = value < minRandom ? value : minRandom;
                            maxRandom = value > maxRandom ? value : maxRandom;
                        }
                        afMethods[m, iBar] = randomSum / iRandomLines;
                        afMinRandom[iBar]  = minRandom;
                        afMaxRandom[iBar]  = maxRandom;
                        minimumRandom = minRandom < minimumRandom ? minRandom : minimumRandom;
                        maximumRandom = maxRandom > maximumRandom ? maxRandom : maximumRandom;
                    }

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    percentComplete = 100 * computedCycles / cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }

                }
                else
                {
                    Backtester.InterpolationMethod = method;
                    Backtester.Calculate();

                    if (Configs.AccountInMoney)
                        for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                            afMethods[m, iBar] = (float)Backtester.MoneyBalance(iBar + Data.FirstBar);
                    else
                        for (int iBar = 0; iBar < Data.Bars - Data.FirstBar; iBar++)
                            afMethods[m, iBar] = (float)Backtester.Balance(iBar + Data.FirstBar);

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    percentComplete = 100 * computedCycles / cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
            }

            // Calculates the average balance, Min and Max
            for (int bar = 0; bar < Data.Bars - Data.FirstBar; bar++)
            {
                float sum = 0;
                for (int m = 0; m < countMethods; m++)
                {
                    if (!achboxMethods[m].Checked) continue;

                    float value = afMethods[m, bar];
                    sum += value;
                    if (value < minimum)
                        minimum = value;
                    if (value > maximum)
                        maximum = value;
                }
                afBalance[bar] = sum / checkedMethods;
            }

            // Report progress as a percentage of the total task.
            computedCycles++;
            percentComplete = 100 * computedCycles / cycles;
            percentComplete = percentComplete > 100 ? 100 : percentComplete;
            if (percentComplete > highestPercentageReached)
            {
                highestPercentageReached = percentComplete;
                worker.ReportProgress(percentComplete);
            }

            return 0;
        }

        /// <summary>
        /// Paints panel pnlOptions
        /// </summary>
        void PnlOptions_Paint(object sender, PaintEventArgs e)
        {
            Panel    pnl = (Panel)sender;
            Graphics g   = e.Graphics;
            int border = 2;

            // Chart Title
            string str  = Language.T("Interpolation Methods");
            Font   font = new Font(Font.FontFamily, 9);
            float  fCaptionHeight  = (float)Math.Max(font.Height, 18);
            RectangleF   rectfCaption = new RectangleF(0, 0, pnl.ClientSize.Width, fCaptionHeight);
            StringFormat stringFormatCaption = new StringFormat();
            stringFormatCaption.Alignment = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(str, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Paint the panel backgraund
            RectangleF rectClient = new RectangleF(border, fCaptionHeight, pnl.ClientSize.Width - 2 * border, pnl.Height - fCaptionHeight - border);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorControlBack, LayoutColors.DepthControl);

            Pen penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, fCaptionHeight, pnl.ClientSize.Width - border + 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width, pnl.ClientSize.Height - border + 1);

            int positionX = (pnlOptions.ClientSize.Width - 10) / 3;
            int positionY = 35;
            int num = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (num < countMethods)
                    {
                        Point pt1 = new Point(j * positionX + 10, i * 30 + positionY);
                        Point pt2 = new Point(j * positionX + 30, i * 30 + positionY);
                        Pen pen = new Pen(Color.Red);
                        switch ((InterpolationMethod)achboxMethods[num].Tag)
                        {
                            case InterpolationMethod.Pessimistic:
                                pen = new Pen(LayoutColors.ComparatorChartPessimisticLine);
                                break;
                            case InterpolationMethod.Shortest:
                                pen = new Pen(LayoutColors.ComparatorChartShortestLine);
                                break;
                            case InterpolationMethod.Nearest:
                                pen = new Pen(LayoutColors.ComparatorChartNearestLine);
                                break;
                            case InterpolationMethod.Optimistic:
                                pen = new Pen(LayoutColors.ComparatorChartOptimisticLine);
                                break;
                            case InterpolationMethod.Random:
                                Point pntRnd1 = new Point(j * positionX + 10, i * 30 + positionY - 6);
                                Point pntRnd2 = new Point(j * positionX + 30, i * 30 + positionY - 6);
                                Point pntRnd3 = new Point(j * positionX + 10, i * 30 + positionY + 6);
                                Point pntRnd4 = new Point(j * positionX + 30, i * 30 + positionY + 6);
                                Pen   penRnd   = new Pen(LayoutColors.ComparatorChartRandomBands, 2);
                                Brush brushRnd = new SolidBrush(LayoutColors.ComparatorChartRandomArea);
                                g.FillRectangle(brushRnd, new Rectangle(pntRnd1.X, pntRnd1.Y, pntRnd2.X - pntRnd1.X, pntRnd4.Y - pntRnd2.Y));
                                g.DrawLine(penRnd, pntRnd1, pntRnd2);
                                g.DrawLine(penRnd, pntRnd3, pntRnd4);
                                pen = new Pen(LayoutColors.ComparatorChartRandomLine);
                                break;
                            default:
                                break;
                        }
                        pen.Width = 2;
                        g.DrawLine(pen, pt1, pt2);
                    }
                    else
                    {
                        Point pt1 = new Point(j * positionX + 10, i * 30 + positionY);
                        Point pt2 = new Point(j * positionX + 30, i * 30 + positionY);
                        Pen pen = new Pen(LayoutColors.ComparatorChartBalanceLine);
                        pen.Width = 3;
                        g.DrawLine(pen, pt1, pt2);
                    }

                    num++;
                }
            }
        }

        /// <summary>
        /// Paints the charts
        /// </summary>
        void PnlChart_Paint(object sender, PaintEventArgs e)
        {
            Panel    pnl = (Panel)sender;
            Graphics g   = e.Graphics;

            int space  = 5;
            int border = 2;

            // Chart Title
            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";
            string str  = Language.T("Balance Chart") + unit;
            Font   font = new Font(Font.FontFamily, 9);
            float      fCaptionHeight = (float)Math.Max(font.Height, 18);
            RectangleF rectfCaption   = new RectangleF(0, 0, pnl.ClientSize.Width, fCaptionHeight);
            StringFormat stringFormatCaption = new StringFormat();
            stringFormatCaption.Alignment = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(str, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Paint the panel backgraund
            RectangleF rectClient = new RectangleF(border, fCaptionHeight, pnl.ClientSize.Width - 2 * border, pnl.Height - fCaptionHeight - border);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            Pen penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, fCaptionHeight, pnl.ClientSize.Width - border + 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width, pnl.ClientSize.Height - border + 1);

            if (!isPaintChart)
            {
                if (Backtester.AmbiguousBars == 0)
                {
                    string sNote = Language.T("The Comparator is useful when the backtest shows ambiguous bars!");
                    RectangleF rectfNote = new RectangleF(0, 30, pnl.ClientSize.Width, Font.Height);
                    g.DrawString(sNote, Font, new SolidBrush(LayoutColors.ColorChartFore), rectfNote, stringFormatCaption);
                } return;
            }

            int bars = Data.Bars - Data.FirstBar;
            int max  = (int)Math.Max(maximum, maximumRandom) + 1;
            int min  = (int)Math.Min(minimum, minimumRandom) - 1;
            min      = (int)Math.Floor(min / 10f) * 10;
            int YTop       = (int)fCaptionHeight + 2 * space + 1;
            int YBottom    = (int)(pnl.ClientSize.Height - 2 * space - border);
            int labelWidth = (int)Math.Max(g.MeasureString(min.ToString(), Font).Width, g.MeasureString(max.ToString(), Font).Width);
            labelWidth = (int)Math.Max(labelWidth, 30);
            int XRight = pnl.ClientSize.Width - border - space - labelWidth;

            //
            // Grid
            //
            int   cntLabels = (int)Math.Max((YBottom - YTop) / 20, 1);
            float delta     = (float)Math.Max(Math.Round((max - min) / (float)cntLabels), 10);
            int   step      = (int)Math.Ceiling(delta / 10) * 10;
            cntLabels = (int)Math.Ceiling((max - min) / (float)step);
            max   = min + cntLabels * step;
            float scaleY = (YBottom - YTop) / (cntLabels * (float)step);
            Brush brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            Pen   penGrid   = new Pen(LayoutColors.ColorChartGrid);
            penGrid.DashStyle   = DashStyle.Dash;
            penGrid.DashPattern = new float[] { 4, 2 };
            // Price labels
            for (int label = min; label <= max; label += step)
            {
                int labelY = (int)(YBottom - (label - min) * scaleY);
                g.DrawString(label.ToString(), Font, brushFore, XRight, labelY - Font.Height / 2 - 1);
                g.DrawLine(penGrid, border + space, labelY, XRight, labelY);
            }

            float fScaleX = (XRight - 2 * space - border) / (float)bars;

            if (isRandom)
            {
                // Drow the random area and Min Max lines
                PointF[] apntMinRandom = new PointF[bars];
                PointF[] apntMaxRandom = new PointF[bars];
                for (int iBar = 0; iBar < bars; iBar++)
                {
                    apntMinRandom[iBar].X = border + space + iBar * fScaleX;
                    apntMinRandom[iBar].Y = YBottom - (afMinRandom[iBar] - min) * scaleY;
                    apntMaxRandom[iBar].X = border + space + iBar * fScaleX;
                    apntMaxRandom[iBar].Y = YBottom - (afMaxRandom[iBar] - min) * scaleY;
                }
                apntMinRandom[0].Y = apntMaxRandom[0].Y;
                GraphicsPath path = new GraphicsPath();
                path.AddLines(apntMinRandom);
                path.AddLine(apntMinRandom[bars - 1], apntMaxRandom[bars - 1]);
                path.AddLines(apntMaxRandom);
                System.Drawing.Region region = new Region(path);
                g.FillRegion(brushRandArea, region);
                g.DrawLines(penRandBands, apntMinRandom);
                g.DrawLines(penRandBands, apntMaxRandom);
            }

            // Drow the lines
            for (int m = 0; m < countMethods; m++)
            {
                if (!achboxMethods[m].Checked) continue;

                PointF[] apntLines = new PointF[bars];
                for (int iBar = 0; iBar < bars; iBar++)
                {
                    apntLines[iBar].X = border + space + iBar * fScaleX;
                    apntLines[iBar].Y = YBottom - (afMethods[m, iBar] - min) * scaleY;
                }

                Pen pen = new Pen(LayoutColors.ColorSignalRed);
                switch ((InterpolationMethod)achboxMethods[m].Tag)
                {
                    case InterpolationMethod.Pessimistic:
                        pen = penPessimistic;
                        break;
                    case InterpolationMethod.Shortest:
                        pen = penShortest;
                        break;
                    case InterpolationMethod.Nearest:
                        pen = penNearest;
                        break;
                    case InterpolationMethod.Optimistic:
                        pen = penOptimistic;
                        break;
                    case InterpolationMethod.Random:
                        pen = penRandom;
                        break;
                    default:
                        break;
                }
                g.DrawLines(pen, apntLines);
            }

            // Drow the average balance
            PointF[] apntBalance = new PointF[bars];
            for (int bar = 0; bar < bars; bar++)
            {
                apntBalance[bar].X = border + space  + bar * fScaleX;
                apntBalance[bar].Y = YBottom - (afBalance[bar] - min) * scaleY;
            }
            g.DrawLines(penBalance, apntBalance);

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), border + space - 1, YTop - space, border + space - 1, YBottom);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), border + space, YBottom, XRight, YBottom);

            // Balance label
            float fBalanceY = YBottom - (afBalance[bars -1] - min) * scaleY;
            g.DrawLine(new Pen(LayoutColors.ColorChartCross), border + space, fBalanceY, XRight - space, fBalanceY);

            Size      szBalance = new Size(labelWidth + space, Font.Height + 2);
            Point     point     = new Point(XRight - space + 2, (int)(fBalanceY - Font.Height / 2 - 1));
            Rectangle rec       = new Rectangle(point, szBalance);
            string    sBalance  = ((int)afBalance[bars - 1]).ToString();
            g.FillRectangle(new SolidBrush(LayoutColors.ColorLabelBack), rec);
            g.DrawRectangle(new Pen(LayoutColors.ColorChartCross), rec);
            g.DrawString(sBalance, Font, new SolidBrush(LayoutColors.ColorLabelText), rec, stringFormatCaption);

            // Scanning note
            Font fontNote = new Font(Font.FontFamily, Font.Size - 1);
            if (Configs.Autoscan && !Data.IsIntrabarData)
                    g.DrawString(Language.T("Load intrabar data"), fontNote, Brushes.Red, border + space, fCaptionHeight - 2);
            else if (Backtester.IsScanPerformed)
                g.DrawString(Language.T("Scanned") + " MQ " + Data.ModellingQuality.ToString("N2") + "%", fontNote, Brushes.LimeGreen, border + space, fCaptionHeight - 2);

            // Scanned bars
            if (Data.IntraBars != null && Data.IsIntrabarData && Backtester.IsScanPerformed)
            {
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), border + space - 1, YBottom, border + space - 1, YBottom + 8);
                DataPeriods dataPeriod = Data.Period;
                Color color    = Data.PeriodColor[Data.Period];
                int   iFromBar = Data.FirstBar;
                for (int bar = Data.FirstBar; bar < Data.Bars; bar++)
                {
                    if (Data.IntraBarsPeriods[bar] != dataPeriod || bar == Data.Bars - 1)
                    {
                        int xStart = (int)((iFromBar - Data.FirstBar) * fScaleX) + border + space;
                        int xEnd   = (int)((bar     - Data.FirstBar) * fScaleX) + border + space;
                        iFromBar   = bar;
                        dataPeriod = Data.IntraBarsPeriods[bar];
                        Data.GradientPaint(g, new RectangleF(xStart, YBottom + 3, xEnd - xStart + 2, 5), color, 60);
                        color = Data.PeriodColor[Data.IntraBarsPeriods[bar]];
                    }
                }
            }
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);

            return;
        }
    }
}
