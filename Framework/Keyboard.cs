using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace AutomationFramework
{
    public class Keyboard : Mouse
    {
        #region WinAPI DLL Imports
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string windowName);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
#endregion

        protected void LeftEnter(IntPtr handle)
        {
            Thread.Sleep(5000);
            Console.WriteLine("Sending Tab Key");
            PostMessage(handle, (int)WindowsMessages.WM_KEYDOWN, (int)Keys.Tab, 1);
            Thread.Sleep(33);
            PostMessage(handle, (int)WindowsMessages.WM_KEYUP, (int)Keys.Tab, 1);
            Thread.Sleep(5000);
            Console.WriteLine("Sending Enter Key");
            PostMessage(handle, (int)WindowsMessages.WM_KEYDOWN, (int)Keys.Enter, 1);
            Thread.Sleep(1000);
            PostMessage(handle, (int)WindowsMessages.WM_KEYUP, (int)Keys.Enter, 1);
        }

        protected void SendEnter(IntPtr handle)
        {
            Console.WriteLine("Sending Enter Key");
            PostMessage(handle, (int)WindowsMessages.WM_KEYDOWN, (int)Keys.Enter, 1);
            Thread.Sleep(33);
            PostMessage(handle, (int)WindowsMessages.WM_KEYUP, (int)Keys.Enter, 1);
        }

        public void SendKey(IntPtr handle, Keys Key)
        {
            PostMessage(handle, (int)WindowsMessages.WM_KEYDOWN, (int)Key, 1);
            Thread.Sleep(33);
            PostMessage(handle, (int)WindowsMessages.WM_KEYUP, (int)Key, 1);
        }


        public void SendKeys(IntPtr handle, string message)
        {
            Keys[] messageKeys = new Keys[message.Length];
            Thread.Sleep(500);
            foreach (char c in message)
            {
                Console.WriteLine("Sending key: {0}", c);
                Keys k = (Keys)System.Enum.Parse(typeof(Keys), c.ToString());
                Console.WriteLine("Key Down");
                PostMessage(handle, (int)WindowsMessages.WM_KEYDOWN, (int)k, 1);
                //Thread.Sleep(11);
                // PostMessage(handle, (int)WindowsMessages.WM_KEYUP, (int)k, 1);
                Console.WriteLine("Pause");
                Thread.Sleep(50);
            }
            Thread.Sleep(500);
        }

        public enum WindowsMessages
        {
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
        }

        public enum Keys
        {
            Enter = 0x0D,
            Left = 0x25,
            Tab = 0x09,
            H = 0x48,
            E = 0x45,
            L = 0x4C,
            O = 0x4F
        }
    }
}
