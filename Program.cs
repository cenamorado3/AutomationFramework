using System;
using System.Collections.Generic;
using System.Linq;
using AutomationFramework;

namespace Program
{
    class Program
    {


        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Window window = null;

            try
            {
                window = new Window("Notepad.exe", "Untitled - Notepad");
                System.Threading.Thread.Sleep(1000);
                List<IntPtr> childWindows = window.GetChildWindows(window.WindowHandle);


                List<IntPtr> c = window.GetChildWindows(window.WindowHandle);



                window.SendKeys(childWindows[0], "HELLO");
                window.SendKey(childWindows[0], Keyboard.Keys.Enter);
                System.Threading.Thread.Sleep(1000);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                System.Threading.Thread.Sleep(5000);
            }

            finally
            {
                System.Threading.Thread.Sleep(2000);
                if (window != null)
                {
                    window.cleanUp();
                }
            }
        }

    }


}