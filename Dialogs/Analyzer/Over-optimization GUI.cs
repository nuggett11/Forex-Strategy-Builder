// Strategy Analyzer - OverOptimization GUI
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    public partial class OverOptimization : Fancy_Panel
    {
        Label lblIntro;
        Label lblDeviation;
        Label lblParams;
        NumericUpDown nudDeviation;
        NumericUpDown nudParams;
        ProgressBar progressBar;
        Button btnStart;
        Button btnViewCharts;
        Button btnOpenFolder;
        Button btnOpenReport;

        BackgroundWorker bgWorker;
        bool isRunning = false;
        public bool IsRunning { get { return isRunning; } }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public OverOptimization(string caption) : base(caption)
        {
            lblIntro      = new Label();
            lblDeviation  = new Label();
            lblParams     = new Label();
            nudDeviation  = new NumericUpDown();
            nudParams     = new NumericUpDown();
            btnStart      = new Button();
            btnViewCharts = new Button();
            btnOpenFolder = new Button();
            btnOpenReport = new Button();

            Font font = this.Font;
            Color colorText = LayoutColors.ColorControlText;

            // Label Intro
            lblIntro.Parent    = this;
            lblIntro.ForeColor = colorText;
            lblIntro.BackColor = Color.Transparent;
            lblIntro.AutoSize  = false;
            lblIntro.Text      = Language.T("The over-optimization report shows how the stats results of back test are changing with changing of the numerical parameters of the strategy by given percent.");

            // Label Deviation
            lblDeviation.Parent    = this;
            lblDeviation.ForeColor = colorText;
            lblDeviation.BackColor = Color.Transparent;
            lblDeviation.AutoSize  = true;
            lblDeviation.Text      = Language.T("Parameters deviation % [recommended 20]");

            // Label Parameters
            lblParams.Parent    = this;
            lblParams.ForeColor = colorText;
            lblParams.BackColor = Color.Transparent;
            lblParams.AutoSize  = true;
            lblParams.Text      = Language.T("Parameters number [recommended 20]");

            // NumericUpDown Deviation
            nudDeviation.BeginInit();
            nudDeviation.Parent    = this;
            nudDeviation.Name      = "Deviation";
            nudDeviation.TextAlign = HorizontalAlignment.Center;
            nudDeviation.Minimum   = 1;
            nudDeviation.Maximum   = 100;
            nudDeviation.Value     = 20;
            nudDeviation.EndInit();

            // NumericUpDown Swap Long
            nudParams.BeginInit();
            nudParams.Parent    = this;
            nudParams.Name      = "Parameters";
            nudParams.TextAlign = HorizontalAlignment.Center;
            nudParams.Minimum   = 1;
            nudParams.Maximum   = 100;
            nudParams.Value     = 20;
            nudParams.EndInit();

            // Button View Charts
            btnViewCharts.Parent     = this;
            btnViewCharts.Name       = "btnViewCharts";
            btnViewCharts.Text       = Language.T("View Charts");
            btnViewCharts.ImageAlign = ContentAlignment.MiddleLeft;
            btnViewCharts.Image      = Properties.Resources.overoptimization_chart;
            btnViewCharts.Enabled    = false;
            btnViewCharts.Click     += new EventHandler(ViewCharts_Click);
            btnViewCharts.UseVisualStyleBackColor = true;

            // Button Open report folder
            btnOpenFolder.Parent     = this;
            btnOpenFolder.Name       = "btnOpenFolder";
            btnOpenFolder.Text       = Language.T("Open Folder");
            btnOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenFolder.Image      = Properties.Resources.folder_open;
            btnOpenFolder.Enabled    = false;
            btnOpenFolder.Click     += new EventHandler(OpenFolder_Click);
            btnOpenFolder.UseVisualStyleBackColor = true;

            // Button Open Report
            btnOpenReport.Parent     = this;
            btnOpenReport.Name       = "btnOpenReport";
            btnOpenReport.Text       = Language.T("Open Report");
            btnOpenReport.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenReport.Image      = Properties.Resources.export;
            btnOpenReport.Enabled    = false;
            btnOpenReport.Click     += new EventHandler(OpenReport_Click);
            btnOpenReport.UseVisualStyleBackColor = true;

            // Button Run
            btnStart.Parent = this;
            btnStart.Text   = Language.T("Start");
            btnStart.Click += new EventHandler(BtnStart_Click);
            btnStart.UseVisualStyleBackColor = true;

            // ProgressBar
            progressBar = new ProgressBar();
            progressBar.Parent  = this;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;
            progressBar.Step    = 1;

            // BackgroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress      = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork             += new DoWorkEventHandler(BgWorker_DoWork);
            bgWorker.ProgressChanged    += new ProgressChangedEventHandler(BgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);

            this.Resize += new EventHandler(PnlOverOptimization_Resize);
        }

        /// <summary>
        /// Calculates controls positions on resizing.
        /// </summary>
        void PnlOverOptimization_Resize(object sender, EventArgs e)
        {
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int space  = btnHrzSpace;
            int border = 2;

            // Labels
            lblIntro.Width    = this.Width - 2 * (btnHrzSpace + border);
            lblIntro.Height   = 2 * buttonHeight;
            lblIntro.Location = new Point(btnHrzSpace + border, 2 * space + 18);

            lblDeviation.Location = new Point(btnHrzSpace + border, 3 * buttonHeight + 2 * space + 18);
            lblParams.Location    = new Point(btnHrzSpace + border, 4 * buttonHeight + 3 * space + 18);

            // NUD Parameters
            int maxLabelRight = lblDeviation.Right;
            if (lblParams.Right > maxLabelRight) maxLabelRight = lblParams.Right;
            int nudLeft = maxLabelRight + btnHrzSpace;
            int nudWidth = 70;
            nudDeviation.Size = new Size(nudWidth, buttonHeight);
            nudDeviation.Location = new Point(nudLeft, 3 * buttonHeight + 2 * space + 16);
            nudParams.Size = new Size(nudWidth, buttonHeight);
            nudParams.Location = new Point(nudLeft, 4 * buttonHeight + 3 * space + 16);

            int btnWideWidth = (int)((this.ClientSize.Width - 2 * border - 4 * btnHrzSpace) / 3);
            btnViewCharts.Size = new Size(btnWideWidth, buttonHeight);
            btnOpenFolder.Size = btnViewCharts.Size;
            btnOpenReport.Size = btnViewCharts.Size;

            btnViewCharts.Location = new Point(border + btnHrzSpace, this.Height - buttonHeight - btnVertSpace);
            btnOpenFolder.Location = new Point(btnViewCharts.Right + btnHrzSpace, btnViewCharts.Top);
            btnOpenReport.Location = new Point(btnOpenFolder.Right + btnHrzSpace, btnViewCharts.Top);

            // Progress Bar
            progressBar.Size = new Size(this.ClientSize.Width - 2 * border - 2 * btnHrzSpace, (int)(Data.VerticalDLU * 9));
            progressBar.Location = new Point(border + btnHrzSpace, btnOpenFolder.Top - progressBar.Height - btnVertSpace);

            // Button Run
            btnStart.Size = btnViewCharts.Size;
            btnStart.Location = new Point(border + btnHrzSpace, progressBar.Top - buttonHeight - btnVertSpace);
        }

        /// <summary>
        /// Button Run clicked.
        /// </summary>
        void BtnStart_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {   // Cancel the asynchronous operation
                bgWorker.CancelAsync();
            }
            else
            {   // Start the bgWorker
                Cursor = Cursors.WaitCursor;
                btnStart.Text = Language.T("Stop");
                isRunning = true;

                nudDeviation.Enabled = false;
                nudParams.Enabled    = false;

                btnViewCharts.Enabled = false;
                btnOpenFolder.Enabled = false;
                btnOpenReport.Enabled = false;

                progressBar.Value = 1;
                progressPercent   = 0;
                computedCycles    = 0;

                bgWorker.RunWorkerAsync();
            }

            return;
        }

        /// <summary>
        /// Does the job.
        /// </summary>
        void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event
            BackgroundWorker worker = sender as BackgroundWorker;
            SetParametersValues((int)nudDeviation.Value, (int)nudParams.Value);
            CalculateStatsTables((int)nudDeviation.Value, (int)nudParams.Value);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation
        /// </summary>
        void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                System.Media.SystemSounds.Exclamation.Play();

            Backtester.Calculate();
            Backtester.CalculateAccountStats();

            string report = GenerateReport();
            SaveReport(report);

            foreach (Control control in this.Controls)
                control.Enabled = true;

            btnStart.Enabled = true;

            btnStart.Text = Language.T("Start");
            Cursor = Cursors.Default;
            isRunning = false;

            return;
        }

        /// <summary>
        /// Opens charts screan.
        /// </summary>
        void ViewCharts_Click(object sender, EventArgs e)
        {
            Over_optimization_Charts_Form chartForm = new Over_optimization_Charts_Form(tableReport);
            chartForm.ShowDialog();
        }

        /// <summary>
        /// Opens the report folder.
        /// </summary>
        void OpenFolder_Click(object sender, EventArgs e)
        {
            try { System.Diagnostics.Process.Start(Data.StrategyDir); }
            catch (System.Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Opens the report.
        /// </summary>
        void OpenReport_Click(object sender, EventArgs e)
        {
            try { System.Diagnostics.Process.Start(pathReportFile); }
            catch (System.Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
