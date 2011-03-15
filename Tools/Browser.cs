// Browser class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public class Browser : Form
    {
        MenuStrip menu;
        ToolStripMenuItem itemSaveAs, itemPrint, itemPreview, itemProps, itemOnlineHelp, itemForum;
        WebBrowser wbBrowser;
        string webPage;

        /// <summary>
        /// Constructor
        /// </summary>
        public Browser(string caption, string webPage)
        {
            Text = caption;
            ResizeRedraw = true;
            BackColor    = SystemColors.GradientInactiveCaption;

            wbBrowser = new WebBrowser();
            wbBrowser.Parent = this;
            wbBrowser.Dock   = DockStyle.Fill;

            this.webPage = webPage;

            // Create MenuStrip
            menu = new MenuStrip();
            menu.Parent = this;
            menu.Dock   = DockStyle.Top;

            MainMenuStrip = menu;
            menu.Items.Add(FileMenu());
            menu.Items.Add(HelpMenu());
        }

        /// <summary>
        /// Overides the base method OnLoad.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = Data.Icon;
            this.Size = new Size(660, 450);
            ShowDocument();
        }

        /// <summary>
        /// Loads the help page.
        /// </summary>
        private void ShowDocument()
        {
            wbBrowser.DocumentText = webPage;
        }

        ToolStripMenuItem FileMenu()
        {
            ToolStripMenuItem item;
            ToolStripMenuItem itemFile = new ToolStripMenuItem(Language.T("File"));

            itemSaveAs = new ToolStripMenuItem(Language.T("Save As") + "...");
            itemSaveAs.Click += SaveAsOnClick;
            itemFile.DropDownItems.Add(itemSaveAs);

            item = new ToolStripMenuItem(Language.T("Page Setup") + "...");
            item.Click += PageSetupOnClick;
            itemFile.DropDownItems.Add(item);

            itemPrint = new ToolStripMenuItem(Language.T("Print") + "...");
            itemPrint.ShortcutKeys = Keys.Control | Keys.P;
            itemPrint.Click += PrintDialogOnClick;
            itemFile.DropDownItems.Add(itemPrint);

            itemPreview = new ToolStripMenuItem(Language.T("Print Preview") + "...");
            itemPreview.Click += PreviewOnClick;
            itemFile.DropDownItems.Add(itemPreview);

            itemFile.DropDownItems.Add(new ToolStripSeparator());

            itemProps = new ToolStripMenuItem(Language.T("Properties") + "...");
            itemProps.Click += PropertiesOnClick;
            itemFile.DropDownItems.Add(itemProps);

            itemFile.DropDownItems.Add(new ToolStripSeparator());

            item = new ToolStripMenuItem(Language.T("Exit"));
            item.Click += ExitOnClick;
            itemFile.DropDownItems.Add(item);

            return itemFile;
        }

        ToolStripMenuItem HelpMenu()
        {
            ToolStripMenuItem itemHelp = new ToolStripMenuItem(Language.T("Help"));

            itemOnlineHelp = new ToolStripMenuItem(Language.T("Online Help") + "...");
            itemOnlineHelp.ShortcutKeys = Keys.F1;
            itemOnlineHelp.Click += OnlineHelpOnClick;
            itemHelp.DropDownItems.Add(itemOnlineHelp);

            itemForum = new ToolStripMenuItem(Language.T("Forum") + "...");
            itemForum.Click += ForumOnClick;
            itemHelp.DropDownItems.Add(itemForum);

            return itemHelp;
        }

        void SaveAsOnClick(object objSrc, EventArgs args)
        {
            try
            {
                string tempFile = Path.GetTempFileName();
                tempFile = Path.Combine(Path.GetDirectoryName(tempFile), Path.GetFileNameWithoutExtension(tempFile) + ".html");
                StreamWriter writer = File.CreateText(tempFile);
                writer.Write(wbBrowser.DocumentText);
                writer.Flush();
                writer.Close();
                wbBrowser.Navigate(tempFile);
                wbBrowser.ShowSaveAsDialog();
                wbBrowser.DocumentText = webPage;
                File.Delete(tempFile);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        void PageSetupOnClick(object objSrc, EventArgs args)
        {
            wbBrowser.ShowPageSetupDialog();
        }

        void PrintDialogOnClick(object objSrc, EventArgs args)
        {
            wbBrowser.ShowPrintDialog();
        }

        void PreviewOnClick(object objSrc, EventArgs args)
        {
            wbBrowser.ShowPrintPreviewDialog();
        }

        void PropertiesOnClick(object objSrc, EventArgs args)
        {
            wbBrowser.ShowPropertiesDialog();
        }

        void ExitOnClick(object objSrc, EventArgs args)
        {
            Close();
        }

        void OnlineHelpOnClick(object objSrc, EventArgs args)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/wiki/fsb/manual/start");
            }
            catch { }
        }

        void ForumOnClick(object objSrc, EventArgs args)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/forum/");
            }
            catch { }
        }

        void ChatOnClick(object objSrc, EventArgs args)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/chat/");
            }
            catch { }
        }

    }
}