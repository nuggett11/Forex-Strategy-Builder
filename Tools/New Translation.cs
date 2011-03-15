// Forex Strategy Builder - New Translation
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// New Translation
    /// </summary>
    class New_Translation : Form
    {
        Fancy_Panel pnlInput;
        Label[]     alblInputNames;
        TextBox[]   atbxInputValues;
        Button      btnAccept;
        Button      btnCancel;

        /// <summary>
        /// Constructor
        /// </summary>
        public New_Translation()
        {
            // The form
            MaximizeBox     = false;
            MinimizeBox     = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text            = Language.T("New Translation");

            // Controls
            pnlInput        = new Fancy_Panel(Language.T("Common Parameters"));
            alblInputNames  = new Label[5];
            atbxInputValues = new TextBox[5];
            btnAccept       = new Button();
            btnCancel       = new Button();

            // Input
            pnlInput.Parent = this;

            // Input Names
            string[] asInputNames = new string[] {
                Language.T("Language"),
                Language.T("File name"),
                Language.T("Author"),
                Language.T("Website"),
                Language.T("Contacts"),
            };

            // Input Values
            string[] asInputValues = new string[] {
                "Language",
                "Language",
                "Your Name",
                "http://forexsb.com",
                "info@forexsb.com",
            };

            // Input parameters
            for (int i = 0; i < asInputNames.Length;  i++)
            {
                alblInputNames[i] = new Label();
                alblInputNames[i].Parent    = pnlInput;
                alblInputNames[i].ForeColor = LayoutColors.ColorControlText;
                alblInputNames[i].BackColor = Color.Transparent;
                alblInputNames[i].AutoSize  = true;
                alblInputNames[i].Text      = asInputNames[i];

                atbxInputValues[i] = new TextBox();
                atbxInputValues[i].Parent = pnlInput;
                atbxInputValues[i].Text   = asInputValues[i];
            }

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Click       += new EventHandler(Btn_Click);
            btnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Accept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.Click       += new EventHandler(Btn_Click);
            btnAccept.UseVisualStyleBackColor = true;

            return;
        }

        /// <summary>
        /// Perform initialising
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(335, 220);

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
            int border       = btnHrzSpace;
            int textHeight   = Font.Height;
            int width        = 195; // Right side contrlos

            // pnlInput
            pnlInput.Size     = new Size(ClientSize.Width - 2 * border, 170);
            pnlInput.Location = new Point(border, border);

            int left = pnlInput.ClientSize.Width - width - btnHrzSpace - 1;

            int shift     = 26;
            int vertSpace = 2;
            for (int i = 0; i < alblInputNames.Length; i++)
            {
                alblInputNames[i].Location = new Point(border, i * buttonHeight + (i + 1) * vertSpace + shift);
            }

            shift     = 24;
            vertSpace = 2;
            for (int i = 0; i < atbxInputValues.Length; i++)
            {
                atbxInputValues[i].Width    = width;
                atbxInputValues[i].Location = new Point(left, i * buttonHeight + (i + 1) * vertSpace + shift);
            }

            // Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            return;
        }

        /// <summary>
        /// Button click
        /// </summary>
        void Btn_Click(object sender, EventArgs e)
        {
            Button btn  = (Button)sender;
            string name = btn.Name;

            if (name == "Accept")
            {
                bool isCorrect = true;

                string language = atbxInputValues[0].Text;
                string fileName = atbxInputValues[1].Text + ".xml";
                string author   = atbxInputValues[2].Text;
                string website  = atbxInputValues[3].Text;
                string contacts = atbxInputValues[4].Text;

                // Language
                if(language.Length < 2)
                {
                    MessageBox.Show("The language name must be at least two characters in length!", "Language", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    isCorrect = false;
                }

                foreach (string lang in Language.LanguageList)
                    if (language == lang)
                    {
                        MessageBox.Show("A translation in this language exists already!" + Environment.NewLine + "Change the language name.", "Language", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        isCorrect = false;
                    }

                // Language file name
                if (fileName.Length < 2)
                {
                    MessageBox.Show("The language file name must be at least two characters in length!", "Language File Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    isCorrect = false;
                }

                if (Directory.Exists(Data.LanguageDir))
                {
                    string[] asFileNames = Directory.GetFiles(Data.LanguageDir);
                    foreach (string path in asFileNames)
                    {
                        if (fileName == Path.GetFileName(path))
                        {
                            MessageBox.Show("This file name exists already!" + Environment.NewLine + "Change the file name.", "Language File Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            isCorrect = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Could not find the language files directory!", "Language Files Directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    isCorrect = false;
                }

                if (isCorrect)
                {
                    if (Language.GenerateNewLangFile(fileName, language, author, website, contacts))
                    {
                        Configs.Language = language;
                        string sMassage = "The new language file was successfully created." + Environment.NewLine + "Restart the program and edit the translation.";
                        MessageBox.Show(sMassage, "New Translation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else
                    return;
            }

            this.Close();
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}
