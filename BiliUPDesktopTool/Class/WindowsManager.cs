﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace BiliUPDesktopTool
{
    public class WindowsManager
    {
        #region Private Fields

        private static WindowsManager instance;

        private readonly List<Window> Windows;

        #endregion Private Fields

        #region Private Constructors

        private WindowsManager()
        {
            Windows = new List<Window>();
        }

        #endregion Private Constructors

        #region Public Properties

        public static WindowsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WindowsManager();
                }
                return instance;
            }
        }

        #endregion Public Properties
        public class WindowExistedException : Exception
        {
            public WindowExistedException(string message, Window window) : base(message)
            {
                Window = window;
            }
            public Window Window;
        }

        #region Public Methods

        public void AddWindow<T>(T window) where T : Window
        {
            var win = Windows.FirstOrDefault(i => i.GetType() == typeof(T));
            if (win == null)
            {
                Windows.Add(window);
            }
            else
            {
                throw new WindowExistedException($"{typeof(T).Name} 已存在", win);
            }
        }

        public T GetWindow<T>() where T : Window, new()
        {
            var win = Windows.FirstOrDefault(i => i.GetType() == typeof(T));
            if (win == null)
            {
                win = new T();
                win.Closed += Win_Closed;
                Windows.Add(win);
            }
            return (T)win;
        }

        public bool HasWindow<T>() where T : Window
        {
            return Windows.Exists(i => i.GetType() == typeof(T));
        }

        #endregion Public Methods

        #region Private Methods

        private void Win_Closed(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Closed -= Win_Closed;
            Windows.Remove(window);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                WinAPIHelper.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        #endregion Private Methods
    }
}