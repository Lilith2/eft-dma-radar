﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace eft_dma_radar
{
    internal static class Program
    {
        private static Mutex _mutex;
        private delegate bool EventHandler(int sig);
        private static EventHandler _handler;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode; // allow russian chars
            try
            {
                _mutex = new Mutex(true, "9A19103F-16F7-4668-BE54-9A1E7A4F7556", out bool singleton);
                if (singleton)
                {
                    _handler += new EventHandler(ShutdownHandler);
                    SetConsoleCtrlHandler(_handler, true); // Handle Ctrl-C exit
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit; // Handle application exit
                    ApplicationConfiguration.Initialize();
					Application.Run(new MainForm());
                }
                else
                {
                    throw new Exception("The Application Is Already Running!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "EFT Radar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool ShutdownHandler(int sig) // Handle ctrl-c
        {
            Memory.Shutdown();
            return false;
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e) // handle normal exit
        {
            Memory.Shutdown();
        }

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
    }
}
