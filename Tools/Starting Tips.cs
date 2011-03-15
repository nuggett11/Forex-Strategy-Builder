// Forex Strategy Builder - Starting_Tips
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Forex_Strategy_Builder
{
    class Starting_Tips : Form
    {
        Fancy_Panel pnlBase;
        Panel       pnlControl;
        WebBrowser  browser;
        Button      btnNextTip;
        Button      btnPrevTip;
        Button      btnClose;
        CheckBox    chboxShow;

        XmlDocument xmlTips;
        Random      rnd;
        int  indexTip;
        int  tipsCount = 0;
        bool showTips;

        string header;
        string currentTip;
        string footer;

        bool showAllTips = false;

        public bool ShowAllTips
        {
            set
            {
                showAllTips = value;
                browser.IsWebBrowserContextMenuEnabled = true;
                browser.WebBrowserShortcutsEnabled     = true;
            }
        }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Starting_Tips()
        {
            pnlBase    = new Fancy_Panel();
            pnlControl = new Panel();
            browser    = new WebBrowser();
            chboxShow  = new CheckBox();
            btnNextTip = new Button();
            btnPrevTip = new Button();
            btnClose   = new Button();

            xmlTips = new XmlDocument();
            rnd     = new Random();

            Text            = Language.T("Tip of the Day");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon            = Data.Icon;
            MaximizeBox     = false;
            MinimizeBox     = false;
            TopMost         = true;

            pnlBase.Parent = this;

            browser.Parent              = pnlBase;
            browser.AllowNavigation     = true;
            browser.AllowWebBrowserDrop = false;
            browser.DocumentText        = Language.T("Loading...");
            browser.Dock                = DockStyle.Fill;
            browser.TabStop             = false;
            browser.DocumentCompleted  += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            browser.IsWebBrowserContextMenuEnabled = false;
            browser.WebBrowserShortcutsEnabled = false;

            pnlControl.Parent    = this;
            pnlControl.Dock      = DockStyle.Bottom;
            pnlControl.BackColor = Color.Transparent;

            chboxShow.Parent    = pnlControl;
            chboxShow.Text      = Language.T("Show a tip");
            chboxShow.Checked   = Configs.ShowStartingTip;
            chboxShow.TextAlign = ContentAlignment.MiddleLeft;
            chboxShow.AutoSize  = true;
            chboxShow.ForeColor = LayoutColors.ColorControlText;
            chboxShow.CheckStateChanged += new EventHandler(ChboxShow_CheckStateChanged);

            btnNextTip.Parent   = pnlControl;
            btnNextTip.Text     = Language.T("Next Tip");
            btnNextTip.Name     = "Next";
            btnNextTip.Click   += new EventHandler(Navigate);
            btnNextTip.UseVisualStyleBackColor = true;

            btnPrevTip.Parent   = pnlControl;
            btnPrevTip.Text     = Language.T("Previous Tip");
            btnPrevTip.Name     = "Previous";
            btnPrevTip.Click   += new EventHandler(Navigate);
            btnPrevTip.UseVisualStyleBackColor = true;

            btnClose.Parent = pnlControl;
            btnClose.Text   = Language.T("Close");
            btnClose.Name   = "Close";
            btnClose.Click += new EventHandler(Navigate);
            btnClose.UseVisualStyleBackColor = true;

            LoadStartingTips();

            return;
        }

        /// <summary>
        /// On Load
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width  = (int)(Data.HorizontalDLU * 240);
            Height = (int)(Data.VerticalDLU   * 140);

            return;
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5 );
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int border       = btnHrzSpace;

            pnlControl.Height = buttonHeight + 2 * btnVertSpace;

            pnlBase.Size     = new Size(ClientSize.Width - 2 * border, pnlControl.Top - border);
            pnlBase.Location = new Point(border, border);

            chboxShow.Location = new Point(btnHrzSpace, btnVertSpace + 5);

            btnClose.Size     = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - btnNextTip.Width - btnHrzSpace, btnVertSpace);

            btnNextTip.Size     = new Size(buttonWidth, buttonHeight);
            btnNextTip.Location = new Point(btnClose.Left - btnNextTip.Width - btnHrzSpace, btnVertSpace);

            btnPrevTip.Size     = new Size(buttonWidth, buttonHeight);
            btnPrevTip.Location = new Point(btnNextTip.Left - btnPrevTip.Width - btnHrzSpace, btnVertSpace);

            // Resize if necessary
            if (btnPrevTip.Left - chboxShow.Right < btnVertSpace)
                Width += btnVertSpace - btnPrevTip.Left + chboxShow.Right;

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);

            return;
        }

        /// <summary>
        /// The Document is ready
        /// </summary>
        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            browser.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            indexTip--;
            ShowTip(true);

            return;
        }

        /// <summary>
        /// Change starting options
        /// </summary>
        void ChboxShow_CheckStateChanged(object sender, EventArgs e)
        {
            showTips = chboxShow.Checked;
            Configs.ShowStartingTip = showTips;
            Configs.SaveConfigs();

            return;
        }

        /// <summary>
        /// Navigate
        /// </summary>
        void Navigate(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Name == "Previous")
                ShowTip(false);
            else if (btn.Name == "Next")
                ShowTip(true);
            else if (btn.Name == "Close")
                Close();

            return;
        }

        /// <summary>
        /// Show random tip
        /// </summary>
        void ShowTip(bool bNextTip)
        {
            if (tipsCount == 0)
                return;

            if (bNextTip)
            {
                if (indexTip < tipsCount - 1)
                    indexTip++;
                else
                    indexTip = 0;
            }
            else
            {
                if (indexTip > 0)
                    indexTip--;
                else
                    indexTip = tipsCount - 1;
            }

            if (showAllTips)
            {
                StringBuilder sbTips = new StringBuilder(tipsCount);

                foreach (XmlNode node in xmlTips.SelectNodes("tips/tip"))
                    sbTips.AppendLine(node.InnerXml);

                browser.DocumentText = header + sbTips.ToString() + footer;

            }
            else
            {
                currentTip = xmlTips.SelectNodes("tips/tip").Item(indexTip).InnerXml;

                browser.DocumentText = header.Replace("###", (indexTip + 1).ToString()) + currentTip + footer;

                Configs.CurrentTipNumber = indexTip;
            }

            return;
        }

        /// <summary>
        /// Load tips config file
        /// </summary>
        void LoadStartingTips()
        {
            // Header
            header  = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">";
            header += "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">";
            header += "<head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />";
            header += "<title>Tip of the Day</title><style>";
            header += "body {margin: 0px; font-size: 14px; background-color: #fffffd}";
            header += ".number {font-size: 9px}";
            header += ".content {padding: 0 5px 5px 5px;}";
            header += ".content h1 {margin: 0; font-weight: bold; font-size: 14px; color: #000033; text-align: center;}";
            header += ".content p {margin-top: 0.5em; margin-bottom: 2px; color: #000033; text-indent: 1em;}";
            header += "</style></head>";
            header += "<body>";
            header += "<div class=\"content\">";
            header += "<div class=\"number\">(###)</div>";

            // Footer
            footer = "</div></body></html>";

            indexTip = Configs.CurrentTipNumber + 1;

            if (showAllTips) indexTip = 0;

            string sStartingTipsDir = Data.SystemDir + @"StartingTips";

            if (Directory.Exists(sStartingTipsDir) && Directory.GetFiles(sStartingTipsDir).Length > 0)
            {
                string[] asLangFiles = Directory.GetFiles(sStartingTipsDir);

                foreach (string sLangFile in asLangFiles)
                {
                    if (sLangFile.EndsWith(".xml", true, null))
                    {
                        try
                        {
                            XmlDocument xmlLanguage = new XmlDocument();
                            xmlLanguage.Load(sLangFile);
                            XmlNode node = xmlLanguage.SelectSingleNode("tips//language");

                            if (node == null)
                            {   // There is no language specified int the lang file
                                string sMessageText = "Starting tip file: " + sLangFile + Environment.NewLine + Environment.NewLine + "The language is not specified!";
                                MessageBox.Show(sMessageText, "Tips of the Day File Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else if (node.InnerText == Configs.Language)
                            {   // It looks OK
                                xmlTips.Load(sLangFile);
                                tipsCount = xmlTips.SelectNodes("tips/tip").Count;
                            }
                        }
                        catch (Exception e)
                        {
                            string sMessageText = "Starting tip file: " + sLangFile + Environment.NewLine + Environment.NewLine +
                                "Error in the starting tip file!" + Environment.NewLine + Environment.NewLine + e.Message;
                            MessageBox.Show(sMessageText, "Tips of the Day File Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
            }

            if (Configs.Language != "English" && tipsCount == 0)
            {
                try
                {   // The tips file
                    xmlTips.Load(Data.SystemDir + "StartingTips" + Path.DirectorySeparatorChar + "English.xml");
                    tipsCount = xmlTips.SelectNodes("tips/tip").Count;
                }
                catch (Exception e)
                {
                    string sMessageText = "Starting tip file \"English.xml\"" + Environment.NewLine + Environment.NewLine +
                        "Error in the starting tip file!" + Environment.NewLine + Environment.NewLine + e.Message;
                    MessageBox.Show(sMessageText, "Tips of the Day File Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            return;
        }
    }
}
