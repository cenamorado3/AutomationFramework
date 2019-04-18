using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using System.Diagnostics;


namespace AutomationFramework
{
    public class Window : Keyboard
    {
        Process proc = null;


        public Window(string appPath, string windowName, int sleep = 500)
        {
            Init(appPath);
            Thread.Sleep(sleep);
            WindowHandle = GetWindow(windowName);
        }



        public IntPtr WindowHandle { get; set; }

        #region WinAPI Imports
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();    

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern string GetDlgItemText(IntPtr hDlg);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr gcListHandle);

        #endregion

        private void Init(string appPath)
        {
            
            proc = new Process();
            proc.StartInfo.FileName = appPath;
            proc.Start();
        }

        public void cleanUp()
        {
            if(proc != null)
            {
                Console.WriteLine("Exiting Application");
                proc.CloseMainWindow();
                proc.Close();
            }
        }

        public IntPtr GetWindow(string windowName)
        {
            IntPtr windowHandle = IntPtr.Zero;
            int timeOut = 1;
            while (windowHandle == IntPtr.Zero)
            {
                if (windowHandle == IntPtr.Zero)
                {
                    Console.WriteLine("Attempt " + timeOut + " to get " + windowName + " handle");
                }

                if (timeOut <= 20)
                {
                    timeOut++;
                    Thread.Sleep(250);
                    windowHandle = Window.FindWindow(null, windowName);
                }

                if (timeOut > 20)
                {
                    throw new NullReferenceException("Timed out getting handle");
                }
            }

            Console.WriteLine("Parent handle: " + windowHandle);
            return windowHandle;

        }
        private IntPtr GetModalWindow(string className, string windowName)
        {
            try
            {
                IntPtr windowHandle = IntPtr.Zero;
                int timeOut = 1;
                while (windowHandle == IntPtr.Zero)
                {
                    if (windowHandle == IntPtr.Zero)
                    {
                        Console.WriteLine("Attempt " + timeOut + " to get " + className + " handle");
                    }

                    if (timeOut <= 20)
                    {
                        timeOut++;
                        Thread.Sleep(250);
                        windowHandle = FindWindow(className, windowName);
                    }

                    if (timeOut > 20)
                    {
                        throw new NullReferenceException("Timed out getting handle");
                    }
                }

                Console.WriteLine("Current handle: " + windowHandle);
                return windowHandle;
            }

            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);

                IntPtr windowHandle = IntPtr.Zero;
                int timeOut = 21;
                while (windowHandle == IntPtr.Zero)
                {
                    if (windowHandle == IntPtr.Zero)
                    {
                        Console.WriteLine("Attempt " + timeOut + " to get " + className + " handle");
                    }

                    if (timeOut >= 21 && timeOut < 100)
                    {
                        timeOut++;
                        Thread.Sleep(250);
                        windowHandle = FindWindow(className, windowName);
                    }

                    if (timeOut > 100)
                    {
                        throw new NullReferenceException("Failed to get handle of " + windowName + " modal window");
                    }
                }

                Console.WriteLine("Current handle: " + windowHandle);
                return windowHandle;

            }
        }

        public string GetDialogText()
        {
            IntPtr window = GetModalWindow("#32770", "HPA Volumes Fetcher");
            string dialogText = GetDlgItemText(window);
            return dialogText;
        }

        public void OKModalWindows(string windowName)
        {
                IntPtr modalWindow = IntPtr.Zero;
                modalWindow = GetModalWindow("#32770", windowName);
                if(modalWindow != IntPtr.Zero)
                {
                    LeftEnter(modalWindow);
                }
        }

        public void SendEnterToModalWindow(string windowName)
        {
            IntPtr modalWindow = IntPtr.Zero;
            modalWindow = GetModalWindow("#32770", windowName);
            if (modalWindow != IntPtr.Zero)
            {
                SendEnter(modalWindow);
            }
        }

        public AutomationElement[] GetChildren(IntPtr handle, ControlType controlType, string localizedControlType, string automationId)
        {
            var window = AutomationElement.FromHandle(handle);

            Condition conditions = new AndCondition
                (
                new PropertyCondition(AutomationElement.ControlTypeProperty, controlType),
                new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, localizedControlType),
                new PropertyCondition(AutomationElement.AutomationIdProperty, automationId)
                );

            List<AutomationElement> children = new List<AutomationElement>();
            AutomationElementCollection elementCollection = window.FindAll(TreeScope.Descendants, conditions == null ? System.Windows.Automation.Condition.TrueCondition : conditions);
            Thread.Sleep(2000);
            foreach(AutomationElement element in elementCollection)
            {
                children.Add(element);
            }
            return children.ToArray();
        }

        public AutomationElement[] GetChildren(IntPtr handle, ControlType controlType, string localizedControlType)
        {
            var window = AutomationElement.FromHandle(handle);

            Condition conditions = new AndCondition
                (
                new PropertyCondition(AutomationElement.ControlTypeProperty, controlType),
                new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, localizedControlType)
                );

            List<AutomationElement> children = new List<AutomationElement>();
            AutomationElementCollection elementCollection = window.FindAll(TreeScope.Descendants, conditions == null ? System.Windows.Automation.Condition.TrueCondition : conditions);
            Thread.Sleep(2000);
            foreach (AutomationElement element in elementCollection)
            {
                children.Add(element);
            }
            return children.ToArray();
        }

        /// <summary>
        /// Returns a list of child windows
        /// </summary>
        /// <param name="parent">Parent of the windows to return</param>
        /// <returns>List of child windows</returns>
        public List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }


        /// <summary>
        /// Callback method to be used when enumerating windows.
        /// </summary>
        /// <param name="handle">Handle of the next window</param>
        /// <param name="pointer">Pointer to a GCHandle that holds a reference to the list to fill</param>
        /// <returns>True to continue the enumeration, false to bail</returns>
        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }

            list.Add(handle);

            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }

        /// <summary>
        /// Delegate for the EnumChildWindows method
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
        /// <returns>True to continue enumerating, false to exit.</returns>
        private delegate bool EnumWindowProc(IntPtr handle, IntPtr pointer);

    }
}
