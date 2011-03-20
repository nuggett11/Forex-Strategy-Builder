// Command Console
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Command_Console : Form
    {
        TextBox tbxInput;
        TextBox tbxOutput;

        /// <summary>
        /// Constructor
        /// </summary>
        public Command_Console()
        {
            // The Form
            Text        = Language.T("Command Console");
            MaximizeBox = false;
            MinimizeBox = false;
            Icon        = Data.Icon;
            BackColor   = LayoutColors.ColorFormBack;

            // Test Box Input
            tbxInput             = new TextBox();
            tbxInput.BorderStyle = BorderStyle.FixedSingle;
            tbxInput.Parent      = this;
            tbxInput.Location    = Point.Empty;
            tbxInput.KeyUp      += new KeyEventHandler(TbxInput_KeyUp);

            // Test Box Output
            tbxOutput             = new TextBox();
            tbxOutput.BorderStyle = BorderStyle.FixedSingle;
            tbxOutput.BackColor   = Color.Black;
            tbxOutput.ForeColor   = Color.GhostWhite;
            tbxOutput.Parent      = this;
            tbxOutput.Location    = Point.Empty;
            tbxOutput.Multiline   = true;
            tbxOutput.WordWrap    = false;
            tbxOutput.Font        = new Font("Courier New", 10);
            tbxOutput.ScrollBars  = ScrollBars.Vertical;
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ClientSize  = new Size(400, 505);
            ParseInput("help");
        }

        /// <summary>
        /// OnResize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int border = 5;
            tbxInput.Width     = ClientSize.Width - 2 * border;
            tbxInput.Location  = new Point(border, ClientSize.Height - border - tbxInput.Height);
            tbxOutput.Width    = ClientSize.Width - 2 * border;
            tbxOutput.Height   = tbxInput.Top - 2 * border;
            tbxOutput.Location = new Point(border, border);

        }

        /// <summary>
        /// Catches the hot keys
        /// </summary>
        private void TbxInput_KeyUp(object sender, KeyEventArgs e)
        {
            tbxInput.BackColor = Color.White;
            if (e.KeyCode == Keys.Return)
                ParseInput(tbxInput.Text);
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void ParseInput(string input)
        {
            string[] asCommands = new string[]
            {
                "help           - Shows the commands list.",
                "clr            - Clears the screen.",
                "pos #          - Shows the parameters of position #.",
                "ord #          - Shows the parameters of order #.",
                "bar #          - Shows the prices of bar #.",
                "ind #          - Shows the indicators for bar #.",
                "str            - shows the strategy.",
                "debug          - Turns on debug mode.",
                "undebug        - Turns off debug mode.",
                "loadlang       - Reloads the language file.",
                "missingphrases - Shows all phrases, which are used in the program but are not included in the language file.",
                "genlangfiles   - Regenerates English.xml.",
                "repairlang     - Repairs all the language files.",
                "importlang     - Imports a translation (Read more first).",
                "langtowiki     - Shows translation in wiki format.",
                "resetinstrum   - Resets the instruments list.",
                "speedtest      - Performs a speed test.",
                "reloadtips     - Reloads the starting tips.",
                "showalltips    - Shows all the starting tips.",
            };

            if (input.StartsWith("help") || input.StartsWith("?"))
            {
                tbxOutput.Text = "Commands" + Environment.NewLine + "-----------------" + Environment.NewLine;
                foreach (string sCommand in asCommands)
                    tbxOutput.Text += sCommand + Environment.NewLine;
            }
            else if (input.StartsWith("clr"))
            {
                tbxOutput.Text = "";
            }
            else if (input.StartsWith("debug"))
            {
                tbxOutput.Text += "Debug mode - on" + Environment.NewLine;
                Data.Debug = true;
            }
            else if (input.StartsWith("nodebug"))
            {
                tbxOutput.Text += "Debug mode - off" + Environment.NewLine;
                Data.Debug = false;
            }
            else if (input.StartsWith("loadlang"))
            {
                Language.InitLanguages();
                tbxOutput.Text += "Language file loaded." + Environment.NewLine;
            }
            else if (input.StartsWith("importlang"))
            {
                Language.ImportLanguageFile(tbxOutput.Text);
            }
            else if (input.StartsWith("langtowiki"))
            {
                Language.ShowPhrases(4);
            }
            else if (input.StartsWith("genlangfiles"))
            {
                Language.GenerateLangFiles();
                tbxOutput.Text += "Language files generated." + Environment.NewLine;
            }
            else if (input.StartsWith("repairlang"))
            {
                tbxOutput.Text += "Language files repair" + Environment.NewLine + "---------------------" + Environment.NewLine + "";
                tbxOutput.Text += Language.RapairAllLangFiles();
            }
            else if (input.StartsWith("resetinstrum"))
            {
                Instruments.ResetInstruments();
                tbxOutput.Text += "The instruments are reset." + Environment.NewLine + "Restart the program now!" + Environment.NewLine;
            }
            else if (input.StartsWith("missingphrases"))
            {
                ShowMissingPhrases();
            }
            else if (input.StartsWith("speedtest"))
            {
                SpeedTest();
            }
            else if (input.StartsWith("str"))
            {
                ShowStrategy();
            }
            else if (input.StartsWith("pos"))
            {
                ShowPosition(input);
            }
            else if (input.StartsWith("ord"))
            {
                ShowOrder(input);
            }
            else if (input.StartsWith("bar"))
            {
                ShowBar(input);
            }
            else if (input.StartsWith("ind"))
            {
                ShowIndicators(input);
            }
            else if (input.StartsWith("reloadtips"))
            {
                Starting_Tips startingTips = new Starting_Tips();
                startingTips.Show();
            }
            else if (input.StartsWith("showalltips"))
            {
                Starting_Tips startingTips = new Starting_Tips();
                startingTips.ShowAllTips = true;
                startingTips.Show();
            }

            tbxOutput.Focus();
            tbxOutput.ScrollToCaret();

            tbxInput.Focus();
            tbxInput.Text = "";
            return;
        }

        /// <summary>
        /// Speed Test
        /// </summary>
        void SpeedTest()
        {
            DateTime dtStart = DateTime.Now;
            int iRep = 1000;

            for (int i = 0; i < iRep; i++)
                Data.Strategy.Clone();

            DateTime dtStop = DateTime.Now;
            TimeSpan tsCalcTime = dtStop.Subtract(dtStart);
            tbxOutput.Text += iRep.ToString() + " times strategy clone for Sec: " +
                tsCalcTime.TotalSeconds.ToString("F4") + Environment.NewLine;
        }

        /// <summary>
        /// Show position
        /// </summary>
        void ShowPosition(string input)
        {
            string pattern = @"^pos (?<numb>\d+)$";
            Regex expression = new Regex(pattern, RegexOptions.Compiled);
            Match match = expression.Match(input);
            if (match.Success)
            {
                int pos = int.Parse(match.Groups["numb"].Value);
                if (pos < 1 || pos > Backtester.PositionsTotal)
                    return;

                Position position = Backtester.PosFromNumb(pos - 1);
                tbxOutput.Text += "Position" + Environment.NewLine + "-----------------" +
                    Environment.NewLine + position.ToString() + Environment.NewLine;
            }
        }

        /// <summary>
        /// Show position
        /// </summary>
        void ShowOrder(string input)
        {
            string pattern = @"^ord (?<numb>\d+)$";
            Regex expression = new Regex(pattern, RegexOptions.Compiled);
            Match match = expression.Match(input);
            if (match.Success)
            {
                int ord = int.Parse(match.Groups["numb"].Value);
                if (ord < 1 || ord > Backtester.OrdersTotal)
                    return;

                Order order = Backtester.OrdFromNumb(ord - 1);
                tbxOutput.Text += "Order" + Environment.NewLine + "-----------------" +
                    Environment.NewLine + order.ToString() + Environment.NewLine;
            }
        }

        /// <summary>
        /// Show bar
        /// </summary>
        void ShowBar(string input)
        {
            string pattern = @"^bar (?<numb>\d+)$";
            Regex expression = new Regex(pattern, RegexOptions.Compiled);
            Match match = expression.Match(input);
            if (match.Success)
            {
                int bar = int.Parse(match.Groups["numb"].Value);
                if (bar < 1 || bar > Data.Bars)
                    return;

                bar--;

                string sBarInfo = String.Format("Bar No " + (bar + 1).ToString() + Environment.NewLine +
                    "{0:D2}.{1:D2}.{2:D4} {3:D2}:{4:D2}" + Environment.NewLine +
                    "Open   {5:F4}" + Environment.NewLine +
                    "High   {6:F4}" + Environment.NewLine +
                    "Low    {7:F4}" + Environment.NewLine +
                    "Close  {8:F4}" + Environment.NewLine +
                    "Volume {9:D6}",
                    Data.Time[bar].Day, Data.Time[bar].Month, Data.Time[bar].Year, Data.Time[bar].Hour, Data.Time[bar].Minute,
                    Data.Open[bar], Data.High[bar], Data.Low[bar], Data.Close[bar], Data.Volume[bar]);

                tbxOutput.Text += "Bar" + Environment.NewLine + "-----------------" +
                    Environment.NewLine + sBarInfo.ToString() + Environment.NewLine;
            }
        }

        /// <summary>
        /// Shows all missing phrases.
        /// </summary>
        void ShowMissingPhrases()
        {
            tbxOutput.Text += Environment.NewLine +
                "Missing Phrases" + Environment.NewLine +
                "---------------------------" + Environment.NewLine;
            foreach (string phrase in Language.MissingPhrases)
                tbxOutput.Text += phrase + Environment.NewLine;
        }

        /// <summary>
        /// Shows the strategy
        /// </summary>
        void ShowStrategy()
        {
            tbxOutput.Text += "Strategy" + Environment.NewLine + "-----------------" +
                Environment.NewLine + Data.Strategy.ToString() + Environment.NewLine;
        }

        /// <summary>
        /// Show indicators in the selected bars.
        /// </summary>
        void ShowIndicators(string input)
        {
            string pattern = @"^ind (?<numb>\d+)$";
            Regex expression = new Regex(pattern, RegexOptions.Compiled);
            Match match = expression.Match(input);
            if (match.Success)
            {
                int bar = int.Parse(match.Groups["numb"].Value);
                if (bar < 1 || bar > Data.Bars)
                    return;

                bar--;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
                {
                    Indicator indicator = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[iSlot].IndicatorName, Data.Strategy.Slot[iSlot].SlotType);

                    sb.Append(Environment.NewLine + indicator.ToString() + Environment.NewLine + "Logic: " +
                        indicator.IndParam.ListParam[0].Text + Environment.NewLine + "-----------------" + Environment.NewLine);
                    foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                    {
                        sb.Append(indComp.CompName + "    ");
                        sb.Append(indComp.Value[bar].ToString() + Environment.NewLine);
                    }
                }

                tbxOutput.Text += Environment.NewLine + "Indicators for bar " + (bar + 1).ToString() + Environment.NewLine +
                    "-----------------" + Environment.NewLine + sb.ToString();
            }
        }
    }
}
