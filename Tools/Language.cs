// Language class
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Manages the language translations of the program
    /// </summary>
    public static class Language
    {
        static Dictionary<String, String> languageFiles; // Language files <Language><FileName>
        static Dictionary<String, String> dictLanguage;      // Language dictionarry
        static string[] languageList; // List of the languages
        static string translatedBy;
        static string website;
        static string contacts;
        static List<string> listMissingPhrases = new List<string>();

        /// <summary>
        /// Gets the language dictionary.
        /// </summary>
        public static Dictionary<String, String> Translation { get { return dictLanguage; } }

        /// <summary>
        /// Gets the language files list.
        /// </summary>
        public static string[] LanguageList { get { return languageList; } }

        /// <summary>
        /// Gets language file name.
        /// </summary>
        public static string LanguageFileName { get { return Path.GetFileName(languageFiles[Configs.Language]); } }

        /// <summary>
        /// Gets the author name.
        /// </summary>
        public static string Author { get { return translatedBy; } }

        /// <summary>
        /// Gets the author's website.
        /// </summary>
        public static string AuthorsWebsite { get { return website; } }

        /// <summary>
        /// Gets the author's email.
        /// </summary>
        public static string AuthorsEmail { get { return contacts; } }

        /// <summary>
        /// Gets the list of missing phrases.
        /// </summary>
        public static List<string> MissingPhrases { get { return listMissingPhrases; } }

        /// <summary>
        /// Language Translation.
        /// </summary>
        public static string T(string phrase)
        {
            if (Configs.Language == "English" || string.IsNullOrEmpty(phrase))
                return phrase;

            string translated = phrase;

            if (dictLanguage.ContainsKey(phrase))
                translated = dictLanguage[phrase];
            else if (!listMissingPhrases.Contains(phrase))
                listMissingPhrases.Add(phrase);

            return translated;
        }

        /// <summary>
        /// Inits the languages.
        /// </summary>
        public static void InitLanguages()
        {
            languageFiles = new Dictionary<string, string>();
            bool bIsLanguageSet = false;

            if (Directory.Exists(Data.LanguageDir) && Directory.GetFiles(Data.LanguageDir).Length > 0)
            {
                string[] asLangFiles = Directory.GetFiles(Data.LanguageDir);

                foreach (string sLangFile in asLangFiles)
                {
                    if (sLangFile.EndsWith(".xml", true, null))
                    {
                        try
                        {
                            XmlDocument xmlLanguage = new XmlDocument();
                            xmlLanguage.Load(sLangFile);
                            XmlNode node = xmlLanguage.SelectSingleNode("lang//language");

                            if (node == null)
                            {   // There is no language specified it the lang file
                                string sMessageText = "Language file: " + sLangFile + Environment.NewLine + Environment.NewLine +
                                    "The language is not specified!";
                                MessageBox.Show(sMessageText, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else if (languageFiles.ContainsKey(node.InnerText))
                            {   // This language has been already loaded
                                string sMessageText = "Language file: " + sLangFile + Environment.NewLine + Environment.NewLine +
                                    "Duplicated language!";
                                MessageBox.Show(sMessageText, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else
                            {   // It looks OK
                                string sLanguage = node.InnerText;
                                languageFiles.Add(sLanguage, sLangFile);

                                if (sLanguage == Configs.Language)
                                {
                                    LoadLanguageFile(sLangFile);
                                    bIsLanguageSet = true;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            string sMessageText = "Language file: " + sLangFile + Environment.NewLine + Environment.NewLine +
                                "Error in the language file!" + Environment.NewLine + Environment.NewLine + e.Message;
                            MessageBox.Show(sMessageText, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
            }

            if (!languageFiles.ContainsKey("English"))
                languageFiles.Add("English", "System");

            if (!languageFiles.ContainsKey("Български"))
                languageFiles.Add("Български", "System");

            if (!bIsLanguageSet)
            {
                if (Configs.Language == "Български")
                    LoadLanguageFile("Български");
                else
                {
                    LoadLanguageFile("English");
                    Configs.Language = "English";
                }
            }

            CheckLangFile();

            languageList = new string[languageFiles.Count];
            languageFiles.Keys.CopyTo(languageList, 0);
            System.Array.Sort(languageList);
        }

        /// <summary>
        /// Loads a language dictionary.
        /// </summary>
        static void LoadLanguageFile(string sLangFile)
        {
            XmlDocument xmlLanguage = new XmlDocument();

            if (sLangFile == "Български" || sLangFile == "English")
                xmlLanguage.InnerXml = Properties.Resources.Bulgarian;
            else
                xmlLanguage.Load(sLangFile);

            translatedBy = xmlLanguage.SelectSingleNode("lang//translatedby").InnerText;
            website      = xmlLanguage.SelectSingleNode("lang//website").InnerText;
            contacts  = xmlLanguage.SelectSingleNode("lang//corrections").InnerText;

            XmlNodeList xmlStringList = xmlLanguage.GetElementsByTagName("str");

            int iStrings = xmlStringList.Count;
            dictLanguage = new Dictionary<string, string>(iStrings);

            foreach (XmlNode nodeString in xmlStringList)
            {
                string sMain = nodeString.SelectSingleNode("main").InnerText;
                string sAlt  = nodeString.SelectSingleNode("alt").InnerText;

                if (Data.Debug && dictLanguage.ContainsValue(sAlt))
                {
                    string sMessage = "The string" + ": " + sAlt + Environment.NewLine + "appears more than once in the laguage file";
                    System.Windows.Forms.MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (dictLanguage.ContainsKey(sMain))
                {
                    string sMessage = "The string" + ": " + sMain + Environment.NewLine + "appears more than once in the laguage file";
                    System.Windows.Forms.MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    dictLanguage.Add(sMain, sAlt);
                }
            }

            return;
        }

        /// <summary>
        /// Generates English.xml and Bulgarian.xml.
        /// </summary>
        public static void GenerateLangFiles()
        {
            // Generate Bulgarian.xml
            string sFilePath = Path.Combine(Data.LanguageDir, "Bulgarian.xml");
            string sContent  = Properties.Resources.Bulgarian;
            SaveTextFile(sFilePath, sContent);

            // Generate English.xml
            string patternMain = @"<main>(?<main>.*)</main>";
            string patternAlt  = @"<alt>(?<alt>.*)</alt>";
            Regex expression = new Regex(".*" + patternMain + patternAlt + ".*", RegexOptions.Compiled);
            sFilePath = Path.Combine(Data.LanguageDir, "English.xml");
            StringBuilder sb = new StringBuilder();
            foreach (string sLine in sContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                Match match = expression.Match(sLine);
                if (match.Success)
                {
                    string main = match.Groups["main"].Value;
                    string alt  = match.Groups["alt"].Value;
                    sb.AppendLine(sLine.Replace(alt, main));
                }
                else
                    sb.AppendLine(sLine);
            }
            sContent = sb.ToString();
            sContent = sContent.Replace("Български", "English");
            SaveTextFile(sFilePath, sContent);

            return;
        }

        /// <summary>
        /// Generates a new language file.
        /// </summary>
        public static bool GenerateNewLangFile(string sFileName, string sLang, string sAuthor, string sWebsite, string sEmail)
        {
            string sContent    = Properties.Resources.Bulgarian;
            string patternMain = @"<main>(?<main>.*)</main>";
            string patternAlt  = @"<alt>(?<alt>.*)</alt>";
            Regex  expression  = new Regex(".*" + patternMain + patternAlt + ".*", RegexOptions.Compiled);
            string sFilePath   = Path.Combine(Data.LanguageDir, sFileName);
            StringBuilder sb   = new StringBuilder();
            foreach (string sLine in sContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                Match match = expression.Match(sLine);
                if (match.Success)
                {
                    string main = match.Groups["main"].Value;
                    string alt  = match.Groups["alt"].Value;
                    sb.AppendLine(sLine.Replace(alt, main));
                }
                else
                    sb.AppendLine(sLine);
            }
            sContent = sb.ToString();
            sContent = sContent.Replace("Български",           sLang);
            sContent = sContent.Replace("Forex Software Ltd.", sAuthor);
            sContent = sContent.Replace(@"http://forexsb.com", sWebsite);
            sContent = sContent.Replace(@"info@forexsb.com",   sEmail);

            return SaveTextFile(sFilePath, sContent);
        }

        /// <summary>
        /// Generates a new language file.
        /// </summary>
        public static void SaveLangFile(Dictionary<string,string> dict , string sAuthor, string sWebsite, string sEmail)
        {
            string path = languageFiles[Configs.Language];
            XmlDocument xmlLanguage = new XmlDocument();
            xmlLanguage.Load(path);

            xmlLanguage.SelectSingleNode("lang//translatedby").InnerText = sAuthor;
            xmlLanguage.SelectSingleNode("lang//website"     ).InnerText = sWebsite;
            xmlLanguage.SelectSingleNode("lang//corrections" ).InnerText = sEmail;

            XmlNodeList xmlStringList = xmlLanguage.GetElementsByTagName("str");

            foreach (XmlNode nodeString in xmlStringList)
            {
                nodeString.SelectSingleNode("alt").InnerText = dict[nodeString.SelectSingleNode("main").InnerText];
            }

            try
            {
                xmlLanguage.Save(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return;
        }

        /// <summary>
        /// Repairs all the language files.
        /// </summary>
        public static string RapairAllLangFiles()
        {
            string[] langFiles = Directory.GetFiles(Data.LanguageDir);
            string report = "";

            foreach (string langFile in langFiles)
            {
                if (langFile.EndsWith(".xml", true, null))
                {
                    XmlDocument xmlBaseLanguage = new XmlDocument();
                    xmlBaseLanguage.InnerXml = Properties.Resources.Bulgarian;

                    XmlDocument xmlLanguage = new XmlDocument();
                    xmlLanguage.Load(langFile);

                    try
                    {
                        xmlBaseLanguage.SelectSingleNode("lang//language").InnerText     = xmlLanguage.SelectSingleNode("lang//language").InnerText;
                        xmlBaseLanguage.SelectSingleNode("lang//translatedby").InnerText = xmlLanguage.SelectSingleNode("lang//translatedby").InnerText;
                        xmlBaseLanguage.SelectSingleNode("lang//website").InnerText      = xmlLanguage.SelectSingleNode("lang//website").InnerText;
                        xmlBaseLanguage.SelectSingleNode("lang//corrections").InnerText  = xmlLanguage.SelectSingleNode("lang//corrections").InnerText;

                        XmlNodeList xmlBaseStringList = xmlBaseLanguage.GetElementsByTagName("str");
                        XmlNodeList xmlStringList     = xmlLanguage.GetElementsByTagName("str");
                        foreach (XmlNode nodeBaseString in xmlBaseStringList)
                        {
                            string main = nodeBaseString.SelectSingleNode("main").InnerText;
                            nodeBaseString.SelectSingleNode("alt").InnerText = main;

                            foreach (XmlNode nodeString in xmlStringList)
                                if(nodeString.SelectSingleNode("main").InnerText == main)
                                    nodeBaseString.SelectSingleNode("alt").InnerText = nodeString.SelectSingleNode("alt").InnerText;
                        }

                        report += xmlLanguage.SelectSingleNode("lang//language").InnerText + " - OK" + Environment.NewLine;
                    }
                    catch (Exception e)
                    {
                        string message = "Language file: " + langFile + Environment.NewLine + Environment.NewLine +
                            "Error in the language file!" + Environment.NewLine + Environment.NewLine + e.Message;
                        MessageBox.Show(message, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    xmlBaseLanguage.Save(langFile);
                }
            }

            return report;
        }

        /// <summary>
        /// Saves a text file
        /// </summary>
        static bool SaveTextFile(string sFilePath, string sContent)
        {
            bool bSuccess = false;

            try
            {
                // Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(sFilePath);

                // Write the text
                sw.Write(sContent);

                // Close the file
                sw.Close();

                bSuccess = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return bSuccess;
        }

        /// <summary>
        /// Checks the language file.
        /// </summary>
        static void CheckLangFile()
        {
            XmlDocument xmlLanguage = new XmlDocument();
            xmlLanguage.InnerXml = Properties.Resources.Bulgarian;
            XmlNodeList xmlStringList = xmlLanguage.GetElementsByTagName("str");

            int iStrings = xmlStringList.Count;
            List<string> listPhrases= new List<string>(iStrings);

            foreach (XmlNode nodeString in xmlStringList)
            {
                string sMain = nodeString.SelectSingleNode("main").InnerText;

                if (listPhrases.Contains(sMain))
                {
                    string sMessage = "The string" + ": " + sMain + Environment.NewLine + "appears more than once in the base laguage file";
                    System.Windows.Forms.MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    listPhrases.Add(sMain);
                }
            }

            string sErrors = "";

            foreach (KeyValuePair<string, string> kvp in dictLanguage)
                if (!listPhrases.Contains(kvp.Key))
                    sErrors += kvp.Key + Environment.NewLine;

            if (sErrors != "")
            {
                string sMessage = "Unused phrases:" + Environment.NewLine + Environment.NewLine + sErrors;
                System.Windows.Forms.MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            sErrors = "";

            foreach (string sPhrase in listPhrases)
                if (!dictLanguage.ContainsKey(sPhrase))
                    sErrors += sPhrase + Environment.NewLine;

            if (sErrors != "")
            {
                string sMessage = "The language file does not contain the phrases:" + Environment.NewLine + Environment.NewLine + sErrors;
                System.Windows.Forms.MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return;
        }

        /// <summary>
        /// Shows the phrases in a webbrowser.
        /// </summary>
        /// <param name="iWhatToshow">1 - English, 2 - Alt, 3 - Both, 4 - Wiki</param>
        public static void ShowPhrases(int iWhatToShow)
        {
            StringBuilder sb = new StringBuilder();

            // Header
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">");
            sb.AppendLine("<head><meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\" />");
            sb.AppendLine("<title>" + Configs.Language + "</title>");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {padding: 0 10px 10px 10px; margin: 0px; font-family: Verdana, Helvetica, Arial, Sans-Serif; font-size: 62.5%; background-color: #fffffe; color: #000033}");
            sb.AppendLine(".content h1 {font-size: 1.9em; text-align: center;}");
            sb.AppendLine(".content h2 {font-size: 1.6em;}");
            sb.AppendLine(".content p {color: #000033; font-size: 1.3em; text-align: left}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"content\" id=\"header\">");

            sb.AppendLine("<h1>" + T("Language Phrases") + "</h1>");

            string[] asEnglishPhrases = new string[dictLanguage.Count];
            string[] asAltPhrases = new string[dictLanguage.Count];
            dictLanguage.Keys.CopyTo(asEnglishPhrases, 0);
            dictLanguage.Values.CopyTo(asAltPhrases, 0);

            string sTranslating = "<p>" +
                T("Translated by") + ": " + translatedBy + "<br />" +
                T("Website")       + ": <a href=\"" + website + "\" target=\"_blanc\">" + website + "</a>" + "<br />" +
                T("Contacts")      + ": " + contacts + "</p><hr />";

            if (iWhatToShow == 1)
            {
                sb.AppendLine("<h2>" + T("Useful for Automatic Translation") + "</h2>");
                sb.AppendLine(sTranslating);
                sb.AppendLine("<p>");
                foreach (string sPhrase in asEnglishPhrases)
                {
                    sb.AppendLine(sPhrase + "<br/>");
                }
                sb.AppendLine("</p>");
            }
            else if (iWhatToShow == 2)
            {
                sb.AppendLine("<h2>" + T("Useful for Spell Check") + "</h2>");
                sb.AppendLine(sTranslating);
                sb.AppendLine("<p>");
                foreach (string sPhrase in asAltPhrases)
                {
                    sb.AppendLine(sPhrase + "<br/>");
                }
                sb.AppendLine("</p>");
            }
            else if (iWhatToShow == 3)
            {
                sb.AppendLine("<h2>" + T("Useful for Translation Check") + "</h2>");
                sb.AppendLine(sTranslating);
                sb.AppendLine("<p>");
                foreach (string sPhrase in asEnglishPhrases)
                {
                    sb.AppendLine(sPhrase + " - " + dictLanguage[sPhrase] + "<br/>");
                }
                sb.AppendLine("</p>");
            }
            else if (iWhatToShow == 4)
            {
                sb.AppendLine("<h2>" + T("Wiki Format") + "</h2>");
                sb.AppendLine(sTranslating);
                sb.AppendLine("<p>");
                sb.AppendLine("====== " + Configs.Language + " ======" + "<br/><br/>");
                sb.AppendLine("Please edit the right column only!<br/><br/>");
                sb.AppendLine("^ English ^" + Configs.Language + "^" + "<br/>");
                foreach (string sPhrase in asEnglishPhrases)
                {
                    sb.AppendLine("| " + sPhrase + " | " + dictLanguage[sPhrase] + " |" + "<br/>");
                }
                sb.AppendLine("</p>");
            }

            // Footer
            sb.AppendLine("</div></body></html>");

            Browser brwsr = new Browser(T("Translation"), sb.ToString());
            brwsr.Show();

            return;
        }

        /// <summary>
        /// Imports a language file.
        /// </summary>
        public static void ImportLanguageFile(string sLangFile)
        {
            string patternMain = @"<main>(?<main>.*)</main>";
            string patternAlt  = @"<alt>(?<alt>.*)</alt>";
            Regex  expression  = new Regex(".*" + patternMain + patternAlt + ".*", RegexOptions.Compiled);

            string sContent = Properties.Resources.Bulgarian;
            string sFilePath = Path.Combine(Data.LanguageDir, "Imported.xml");
            string[] asPhrases = sLangFile.Split(Environment.NewLine.ToCharArray()[0]);

            StringBuilder sb = new StringBuilder();
            int iPhraseNumber = 0;
            foreach (string sLine in sContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                Match match = expression.Match(sLine);
                if (match.Success)
                {
                    if (iPhraseNumber >= asPhrases.Length)
                        break;

                    int index  = match.Groups["alt"].Index;
                    int lenght = match.Groups["alt"].Length;

                    string translation = asPhrases[iPhraseNumber].Trim();
                    sb.AppendLine(sLine.Remove(index, lenght).Insert(index, translation));
                    iPhraseNumber++;
                }
                else
                    sb.AppendLine(sLine);
            }
            sContent = sb.ToString();
            sContent = sContent.Replace("Български", "Imported");
            SaveTextFile(sFilePath, sContent);

            return;

        }
    }
}
