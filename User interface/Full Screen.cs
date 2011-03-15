// Full Screen
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Selected Win AI Function Calls
    /// </summary>
    public class WinApi
    {
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
            int X, int Y, int width, int height, uint flags);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private static IntPtr HWND_TOP = IntPtr.Zero;
        private const int SWP_SHOWWINDOW = 64; // 0x0040

        public static int ScreenX
        {
            get { return GetSystemMetrics(SM_CXSCREEN); }
        }

        public static int ScreenY
        {
            get { return GetSystemMetrics(SM_CYSCREEN); }
        }

        public static void SetWinFullScreen(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HWND_TOP, 0, 0, ScreenX, ScreenY, SWP_SHOWWINDOW);
        }
    }

    /// <summary>
    /// Class used to preserve / restore state of the form
    /// </summary>
    public class FormState
    {
        private static FormWindowState winState;
        private static FormBorderStyle brdStyle;
        private static Rectangle bounds;
        //private static bool topMost;

        private static bool IsMaximized = false;

        public static void Maximize(Form targetForm)
        {
            if (!IsMaximized)
            {
                IsMaximized = true;
                Save(targetForm);
                targetForm.WindowState     = FormWindowState.Maximized;
                targetForm.FormBorderStyle = FormBorderStyle.None;
                //targetForm.TopMost         = true;
                WinApi.SetWinFullScreen(targetForm.Handle);
            }
        }

        public static void Save(Form targetForm)
        {
            winState = targetForm.WindowState;
            brdStyle = targetForm.FormBorderStyle;
            bounds   = targetForm.Bounds;
            //topMost  = targetForm.TopMost;
        }

        public static void Restore(Form targetForm)
        {
            targetForm.WindowState     = winState;
            targetForm.FormBorderStyle = brdStyle;
            targetForm.Bounds          = bounds;
            //targetForm.TopMost         = topMost;
            IsMaximized = false;
        }
    }
}
