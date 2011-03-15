// Info_Panel Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Info_Panel : Panel
    {
        string   caption;
        string[] asParam;
        string[] asValue;
        bool[]   abFlag;

        int border = 2;

        int   rows;
        int   visibleRows;
        float rowHeight;
        float captionHeight;
        float maxParamWidth;
        float maxValueWidth;
        float requiredHeight;
        float paddingParamData = 4;
        float paramTab;
        float valueTab;
        float width;
        float height;
        Pen   penBorder;
        Font  fontCaption;
        Font  fontData;
        Brush brushCaption;
        Brush brushParams;
        Brush brushData;
        Color colorCaptionBack;
        Color colorBackroundEvenRows;
        Color colorBackroundOddRows;
        Color colorBackroundWarningRow;
        Color colorTextWarningRow;
        StringFormat stringFormatCaption;
        StringFormat stringFormatData;
        RectangleF   rectfCaption;

        VScrollBar vScrollBar;
        HScrollBar hScrollBar;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Info_Panel()
        {
            InitializeParameters();
            SetColors();

            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Info_Panel(string[] asParams, string[] asValues, bool[] abFlags, string sCaption)
        {
            InitializeParameters();

            Update(asParam, asValues, abFlags, sCaption);

            return;
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            colorCaptionBack         = LayoutColors.ColorCaptionBack;
            colorBackroundEvenRows   = LayoutColors.ColorEvenRowBack;
            colorBackroundWarningRow = LayoutColors.ColorWarningRowBack;
            colorTextWarningRow      = LayoutColors.ColorWarningRowText;
            colorBackroundOddRows    = LayoutColors.ColorOddRowBack;

            brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            brushParams  = new SolidBrush(LayoutColors.ColorControlText);
            brushData    = new SolidBrush(LayoutColors.ColorControlText);

            penBorder    = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            return;
        }

        /// <summary>
        /// Initialize Parameters
        /// </summary>
        void InitializeParameters()
        {
            // Caption
            stringFormatCaption = new StringFormat();
            stringFormatCaption.Alignment     = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;
            fontCaption    = new Font(Font.FontFamily, 9);
            captionHeight = (float)Math.Max(fontCaption.Height, 18);
            rectfCaption   = new RectangleF(0, 0, ClientSize.Width, captionHeight);

            // Data row
            stringFormatData = new StringFormat();
            stringFormatData.Trimming = StringTrimming.EllipsisCharacter;
            fontData   = new Font(Font.FontFamily, 9);
            rowHeight = fontData.Height + 4;

            Padding = new Padding(border, (int)captionHeight, border, border);
            
            hScrollBar = new HScrollBar();
            hScrollBar.Parent      = this;
            hScrollBar.Dock        = DockStyle.Bottom;
            hScrollBar.Enabled     = false;
            hScrollBar.Visible     = false;
            hScrollBar.SmallChange = 10;
            hScrollBar.LargeChange = 30;
            hScrollBar.Scroll     += new ScrollEventHandler(ScrollBar_Scroll);

            vScrollBar = new VScrollBar();
            vScrollBar.Parent      = this;
            vScrollBar.Dock        = DockStyle.Right;
            vScrollBar.TabStop     = true;
            vScrollBar.Enabled     = false;
            vScrollBar.Visible     = false;
            vScrollBar.SmallChange = 1;
            vScrollBar.LargeChange = 3;
            vScrollBar.Maximum     = 20;
            vScrollBar.Scroll     += new ScrollEventHandler(ScrollBar_Scroll);

            MouseUp += new MouseEventHandler(Info_Panel_MouseUp);

            return;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update(string[] asParams, string[] asValues, bool[] abFlags, string caption)
        {
            this.asParam = asParams;
            this.asValue = asValues;
            this.abFlag  = abFlags;
            this.caption = caption;

            rows = asParam.Length;
            requiredHeight = captionHeight + rows * rowHeight + border;

            // Maximum width
            maxParamWidth = 0;
            maxValueWidth = 0;
            Graphics g = CreateGraphics();
            for (int i = 0; i < rows; i++)
            {
                float fWidthParam = g.MeasureString(asParam[i], fontData).Width;
                if (fWidthParam > maxParamWidth)
                    maxParamWidth = fWidthParam;

                float fValueWidth = g.MeasureString(asValue[i], fontData).Width;
                if (fValueWidth > maxValueWidth)
                    maxValueWidth = fValueWidth;
            }
            g.Dispose();

            CalculateScrollBarStatus();
            CalculateTabs();
            Invalidate();

            return;
        }

        /// <summary>
        /// On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption
            Data.GradientPaint(g, rectfCaption, colorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(caption, fontCaption, brushCaption, rectfCaption, stringFormatCaption);

            float y = captionHeight;
            for (int i = 0; i * rowHeight + captionHeight < height; i++)
            {
                float fVerticalPosition = i * rowHeight + captionHeight;
                PointF pointFParam = new PointF(paramTab + 2, fVerticalPosition);
                PointF pointFValue = new PointF(valueTab + 2, fVerticalPosition);
                RectangleF rectRow = new RectangleF(border, fVerticalPosition, width, rowHeight);

                // Row background
                if (i + vScrollBar.Value < rows && abFlag[i + vScrollBar.Value])
                    g.FillRectangle(new SolidBrush(colorBackroundWarningRow), rectRow);
                else if (i % 2f != 0)
                    g.FillRectangle(new SolidBrush(colorBackroundEvenRows), rectRow);
                else
                    g.FillRectangle(new SolidBrush(colorBackroundOddRows), rectRow);

                if (i + vScrollBar.Value >= rows)
                    continue;

                if (i + vScrollBar.Value < rows && abFlag[i + vScrollBar.Value])
                {
                    Brush brush = new SolidBrush(colorTextWarningRow);
                    Pen   pen   = new Pen(colorTextWarningRow);
                    g.DrawString(asParam[i + vScrollBar.Value], fontData, brush, pointFParam, stringFormatData);
                    g.DrawString(asValue[i + vScrollBar.Value], fontData, brush, pointFValue, stringFormatData);
                }
                else
                {
                    g.DrawString(asParam[i + vScrollBar.Value], fontData, brushParams, pointFParam, stringFormatData);
                    g.DrawString(asValue[i + vScrollBar.Value], fontData, brushData,   pointFValue, stringFormatData);
                }
            }

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, captionHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            return;
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            CalculateScrollBarStatus();
            CalculateTabs();
            Invalidate();

            return;
        }

        /// <summary>
        /// Scroll Bars status
        /// </summary>
        void CalculateScrollBarStatus()
        {
            width  = ClientSize.Width  - 2 * border;
            height = ClientSize.Height - border;

            bool needHorisontal = width  < maxParamWidth + paddingParamData + maxValueWidth - 2;
            bool needVertical   = height < requiredHeight;
            bool isHorisontal   = needHorisontal;
            bool isVertical     = needVertical;

            if (needHorisontal && needVertical)
            {
                isVertical   = true;
                isHorisontal = true;
            }
            else if (needHorisontal && !needVertical)
            {
                isHorisontal = true;
                height       = ClientSize.Height - hScrollBar.Height - border;
                isVertical   = height < requiredHeight;
            }
            else if (!needHorisontal && needVertical)
            {
                isVertical   = true;
                width        = ClientSize.Width - vScrollBar.Width - 2 * border;
                isHorisontal = width < maxParamWidth + paddingParamData + maxValueWidth - 2;
            }
            else
            {
                isHorisontal = false;
                isVertical   = false;
            }

            if (isHorisontal)
                height = ClientSize.Height - hScrollBar.Height - border;

            if (isVertical)
                width = ClientSize.Width - vScrollBar.Width - 2 * border;

            vScrollBar.Enabled = isVertical;
            vScrollBar.Visible = isVertical;
            hScrollBar.Enabled = isHorisontal;
            hScrollBar.Visible = isHorisontal;

            hScrollBar.Value = 0;
            if (isHorisontal)
            {
                int iPoinShort = (int)(maxParamWidth + paddingParamData + maxValueWidth - width);
                hScrollBar.Maximum = iPoinShort + hScrollBar.LargeChange - 2;
            }

            rectfCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            visibleRows = (int)Math.Min(((height - captionHeight) / rowHeight), rows);

            vScrollBar.Value = 0;
            vScrollBar.Maximum = rows - visibleRows + vScrollBar.LargeChange - 1;

            return;
        }

        /// <summary>
        /// Tabs
        /// </summary>
        void CalculateTabs()
        {
            if (width < maxParamWidth + paddingParamData + maxValueWidth)
            {
                paramTab = -hScrollBar.Value + border;
                valueTab = paramTab + maxParamWidth;
            }
            else
            {
                float fSpace = (width - (maxParamWidth + maxValueWidth)) / 5;
                paramTab = 2 * fSpace;
                valueTab = paramTab + maxParamWidth + fSpace;
            }

            return;
        }

        /// <summary>
        /// ScrollBar_Scroll
        /// </summary>
        void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            CalculateTabs();
            int horizontal = hScrollBar.Visible ? hScrollBar.Height : 0;
            int vertical   = vScrollBar.Visible ? vScrollBar.Width  : 0;
            Rectangle rect = new Rectangle(border, (int)captionHeight, ClientSize.Width - vertical - 2 * border, ClientSize.Height - (int)captionHeight - horizontal - border);
            Invalidate(rect);
        }

        /// <summary>
        /// Selects the vertical scrallbar
        /// </summary>
        void Info_Panel_MouseUp(object sender, MouseEventArgs e)
        {
            vScrollBar.Select();
        }
    }
}
