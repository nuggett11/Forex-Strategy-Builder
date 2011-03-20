// Forex Strategy Builder - Check Update class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace Forex_Strategy_Builder
{
    public class Check_Update
    {
        string updateFileURL = "http://forexsb.com/products/fsb-update.xml";
        string pathUpdateFile;
        string pathSystem;
        Dictionary<string, string> dictBrokers;
        ToolStripMenuItem miLiveContent;
        ToolStripMenuItem miForex;
        BackgroundWorker  bgWorker;

        XmlDocument xmlUpdateFile;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Check_Update(string pathSystem, ToolStripMenuItem miLiveContent, ToolStripMenuItem miForex)
        {
            this.pathSystem    = pathSystem;
            this.miLiveContent = miLiveContent;
            this.miForex       = miForex;
            pathUpdateFile     = Path.Combine(pathSystem, "fsb-update.xml");

            dictBrokers = new Dictionary<string, string>();
            xmlUpdateFile = new XmlDocument();

            LoadUpdateFile();
            ReadBrokers();
            SetBrokers();

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(DoWork);
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateTheUpdateFile();
            CheckProgramsVersionNumber();
        }

        /// <summary>
        /// Update the config file if it is necessary
        /// </summary>
        void UpdateTheUpdateFile()
        {
            Uri url = new Uri(updateFileURL);
            WebClient webClient = new WebClient();
            try
            {
                xmlUpdateFile.LoadXml(webClient.DownloadString(url));
                SaveUpdateFile();
            }
            catch { }
        }

        /// <summary>
        /// Load config file
        /// </summary>
        void LoadUpdateFile()
        {
            try
            {
                xmlUpdateFile.Load(pathUpdateFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Config");
            }

            return;
        }

        /// <summary>
        /// Save config file
        /// </summary>
        void SaveUpdateFile()
        {
            try
            {
                xmlUpdateFile.Save(pathUpdateFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Check for Updates");
            }

            return;
        }

        /// <summary>
        /// Checks the program version
        /// </summary>
        void CheckProgramsVersionNumber()
        {
            string text = "";
            try
            {
                int iProgramVersion = int.Parse(xmlUpdateFile.SelectSingleNode("update/versions/release").InnerText);
                if (Configs.CheckForUpdates && iProgramVersion > Data.ProgramID)
                {   // A newer release version was published
                    text = Language.T("New Version");
                }
                else
                {
                    int iBetaVersion = int.Parse(xmlUpdateFile.SelectSingleNode("update/versions/beta").InnerText);
                    if (Configs.CheckForNewBeta && iBetaVersion > Data.ProgramID)
                    {   // A newer beta version was published
                        text = Language.T("New Beta");
                    }
                }

                if (text != "")
                {
                    miLiveContent.Text    = text;
                    miLiveContent.Visible = true;
                    miLiveContent.Click  += new EventHandler(MenuLiveContentOnClick);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Check for Updates");
            }

        }

        /// <summary>
        /// Opens the Live Content browser
        /// </summary>
        protected void MenuLiveContentOnClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/");
                HideMenuItemLiveContent();
            }
            catch { }
        }

        /// <summary>
        /// Hides the menu news
        /// </summary>
        void HideMenuItemLiveContent()
        {
            miLiveContent.Visible = false;
            miLiveContent.Click  -= new EventHandler(MenuLiveContentOnClick);
        }

        /// <summary>
        /// Reads the brokers
        /// </summary>
        void ReadBrokers()
        {
            try
            {
                XmlNodeList xmlListBrokers = xmlUpdateFile.GetElementsByTagName("broker");

                foreach (XmlNode nodeBroker in xmlListBrokers)
                {
                    string title = nodeBroker.SelectSingleNode("title").InnerText;
                    string link = nodeBroker.SelectSingleNode("link").InnerText;

                    dictBrokers.Add(title, link);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Check for Updates");
            }

            return;
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        void SetBrokers()
        {
            foreach (KeyValuePair<string, string> kvpBroker in dictBrokers)
            {
                ToolStripMenuItem miBroker = new ToolStripMenuItem();
                miBroker.Text   = kvpBroker.Key + "...";
                miBroker.Image  = Properties.Resources.globe;
                miBroker.Tag    = kvpBroker.Value;
                miBroker.Click += new EventHandler(MenuForexContentsOnClick);

                miForex.DropDownItems.Add(miBroker);
            }

            return;
        }

        /// <summary>
        /// Opens the forex news
        /// </summary>
        void MenuForexContentsOnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            try
            {
                System.Diagnostics.Process.Start((string)mi.Tag);
            }
            catch { }

            return;
        }
    }
}
