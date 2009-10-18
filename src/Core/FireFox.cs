#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.Native.Mozilla;
using WatiN.Core.Native;
using WatiN.Core.UtilityClasses;
using WatiN.Core.Native.Windows;
using System.Collections.Generic;
using WatiN.Core.Native.Mozilla.Dialogs;
using WatiN.Core.WatchableObjects;

// https://developer.mozilla.org/en/Gecko_DOM_Reference

namespace WatiN.Core
{
    public class FireFox : Browser 
    {
        private FFBrowser ffBrowser;

        #region Public constructors / destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FireFox"/> class.
        /// </summary>
        public FireFox() : this(true) { }

        public FireFox(bool createNew)
        {
            if (createNew)
                CreateFireFoxInstance("about:blank");
            else
                PartialInitFireFox();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FireFox"/> class.
        /// </summary>
        /// <param name="url">The url to go to</param>
        public FireFox(string url)
        {
            CreateFireFoxInstance(url);
            WaitForComplete();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FireFox"/> class.
        /// </summary>
        /// <param name="uri">The url to go to</param>
        public FireFox(Uri uri)
        {
            CreateFireFoxInstance(uri.AbsoluteUri);
            WaitForComplete();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FireFox"/> is reclaimed by garbage collection.
        /// </summary>
        ~FireFox()
        {
            Dispose(false);
        }

        #endregion

        #region Public instance properties

        public override INativeBrowser NativeBrowser
        {
            get { return ffBrowser; }
        }

        #endregion Public instance properties

        #region Public instance methods

        /// <summary>
        /// Gets the number of running FireFox processes.
        /// </summary>
        /// <value>The number of running FireFox processes.</value>
        public static int CurrentProcessCount
        {
            get
            {
                var ffCount = 0;

                foreach (var process in Process.GetProcesses())
                {
                    if (process.ProcessName.Equals("firefox", StringComparison.OrdinalIgnoreCase))
                    {
                        ffCount++;
                    }
                }

                return ffCount;
            }
        }

        /// <summary>
        /// Gets the current FireFox process.
        /// </summary>
        /// <value>The current FireFox process or null if none is found.</value>
        internal static Process CurrentProcess
        {
            get
            {
                Process ffProcess = null;

                foreach (var process in Process.GetProcesses())
                {
                    if (process.ProcessName.Equals("firefox", StringComparison.OrdinalIgnoreCase))
                    {
                        ffProcess = process;
                    }
                }

                return ffProcess;
            }
        }

        private static string pathToExe;
        /// <summary>
        /// Gets the path to FireFox executable.
        /// </summary>
        /// <value>The path to exe.</value>
        public static string PathToExe
        {
            get
            {
                if (pathToExe == null)
                {
                    pathToExe = GetExecutablePath();
                }

                return pathToExe;
            }
        }

        internal static Process CreateProcess(string arguments, bool waitForMainWindow)
        {
            var ffProcess = new Process {StartInfo = {FileName = PathToExe, Arguments = arguments}};
            
            ffProcess.Start();
            ffProcess.WaitForInputIdle(5000);
            ffProcess.Refresh();

            if (waitForMainWindow)
            {
                var action = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(Settings.WaitForCompleteTimeOut))
                {
                    SleepTime = TimeSpan.FromMilliseconds(200)
                };

                var result = action.Try(() =>
                                            {
                                                ffProcess.Refresh();
                                                if (!ffProcess.HasExited && ffProcess.MainWindowHandle != IntPtr.Zero)
                                                {
                                                    Logger.LogAction("Waited for FireFox, main window handle found.");
                                                    return true;
                                                }
                                                return false;
                                            });

                if (!result)
                {
                    Debug.WriteLine("Timer elapsed waiting for FireFox to start.");
                }
            }

            return ffProcess;
        }


        /// <summary>
        /// Initalizes the executable path.
        /// </summary>
        private static string GetExecutablePath()
        {
            string path;
            var mozillaKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Mozilla\Mozilla Firefox");
            if (mozillaKey != null)
            {
                path = GetExecutablePathUsingRegistry(mozillaKey);
            }
            else
            {
                // We try and guess common locations where FireFox might be installed
                var tempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Mozilla FireFox\FireFox.exe");
                if (File.Exists(tempPath))
                {
                    path = tempPath;
                }
                else
                {
                    tempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + " (x86)", @"Mozilla FireFox\FireFox.exe");
                    if (File.Exists(tempPath))
                    {
                        path = tempPath;
                    }
                    else
                    {
                        throw new FireFoxException("Unable to determine the current version of FireFox tried looking in the registry and the common locations on disk, please make sure you have installed FireFox and Jssh correctly");
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// Initializes the executable path to FireFox using the registry.
        /// </summary>
        /// <param name="mozillaKey">The mozilla key.</param>
        private static string GetExecutablePathUsingRegistry(RegistryKey mozillaKey)
        {
            var currentVersion = (string)mozillaKey.GetValue("CurrentVersion");
            if (string.IsNullOrEmpty(currentVersion))
            {
                throw new FireFoxException("Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox and Jssh correctly");
            }

            var currentMain = mozillaKey.OpenSubKey(string.Format(@"{0}\Main", currentVersion));
            if (currentMain == null)
            {
                throw new FireFoxException(
                    "Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox and Jssh correctly");
            }

            var path = (string)currentMain.GetValue("PathToExe");
            if (!File.Exists(path))
            {
                throw new FireFoxException(
                    "FireFox executable listed in the registry does not exist, please make sure you have installed FireFox and Jssh correctly");
            }

            return path;
        }

        /// <summary>
        /// Waits until the page is completely loaded
        /// </summary>
        public override void WaitForComplete(int waitForCompleteTimeOut)
        {
            WaitForComplete(new JSWaitForComplete(ffBrowser, waitForCompleteTimeOut));
        }

        /// <summary>
        /// Closes the browser.
        /// </summary>
        public override void Close()
        {
            ffBrowser.Close();
        }

        public static bool Exists(Constraint findBy)
        {
            return new FireFox(false).FindFireFox(findBy);
        }

        #endregion Public instance methods

        #region Protected instance methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (IsDisposed) return;
            
            // If disposing equals true, dispose all managed 
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                ffBrowser.ClientPort.Dispose();
            }

            // Call the appropriate methods to clean up 
            // unmanaged resources here.
            base.Dispose(disposing);
        }

        #endregion Protected instance methods

        #region Private static methods

        private void CreateFireFoxInstance(string url)
        {
            Logger.LogAction("Creating FireFox instance");

            UtilityClass.MoveMousePoinerToTopLeft(Settings.AutoMoveMousePointerToTopLeft);

            var clientPort = new FireFoxClientPort();
            clientPort.LaunchHostProcess(url);
            ffBrowser = new FFBrowser(clientPort);
            clientPort.Connect(url);
            WaitForComplete();
        }

        public static FireFox AttachToFireFox(Constraint findBy)
        {
            return AttachToFireFox(findBy, Settings.AttachToIETimeOut);
        }

        public static FireFox AttachToFireFox(Constraint findBy, int timeout)
        {
            FireFox result = new FireFox(false);
            result.AttachToExisting(findBy, timeout, true);
            return result;
        }

        public static FireFox AttachToFireFoxNoWait(Constraint findBy)
        {
            return AttachToFireFoxNoWait(findBy, Settings.AttachToIETimeOut);
        }

        public static FireFox AttachToFireFoxNoWait(Constraint findBy, int timeout)
        {
            FireFox result = new FireFox(false);
            result.AttachToExisting(findBy, timeout, false);
            return result;
        }

        private void PartialInitFireFox()
        {
            var clientPort = new FireFoxClientPort();
            clientPort.ConnectToExisting();
            ffBrowser = new FFBrowser(clientPort);
        }

        private void AttachToExisting(Constraint findBy, int timeout, bool waitForComplete)
        {
            var action = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeout)) { SleepTime = TimeSpan.FromMilliseconds(500) };
            bool found = action.Try(() => FindFireFox(findBy));
            if (found)
            {
                if (waitForComplete)
                    WaitForComplete();
                return;
            }

            throw new FireFoxNotFoundException(findBy.ToString(), timeout);
        }

        private bool FindFireFox(Constraint findBy)
        {
            int windowCount = ffBrowser.WindowCount;

            for(int i = 0; i < windowCount; i++)
            {
                ((FireFoxClientPort)ffBrowser.ClientPort).DefineDefaultJSVariablesForWindow(i);
                if (Matches(findBy)) return true;
            }

            return false;
        }

        #endregion
    }
}