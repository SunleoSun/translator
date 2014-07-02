using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Volumer;
using System.Net;
using System.IO;
namespace TranslatorMainApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        HookKeys hk = new HookKeys();
        NotifyIcon nf = new NotifyIcon();
        private void MenuClickExit(object sender, EventArgs e)
        {
            this.Close();
        }
        private void hk_KeyHooked(object sender, KeyHooksEventArgs e)
        {
            if (e.control)
            {
                nf.BalloonTipText = Translate(Clipboard.GetText(TextDataFormat.Text), "en", "ru");
                nf.ShowBalloonTip(1000);
            }
        }

        private static string Translate(string InputText, string FromLang, string ToLang)
        {
            string OutputText = "";
            WebRequest req = WebRequest.Create(@"http://api.microsofttranslator.com/V2/Http.svc/Translate?appId=FD9BE24FE82B92F679959684D22AEB0B14752498&text=" + InputText + "&from=" + FromLang + "&to=" + ToLang);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            OutputText = sr.ReadToEnd();
            OutputText = OutputText.Remove(OutputText.Length - 9);
            OutputText = OutputText.Remove(0, 68);
            return OutputText;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            Bitmap bitmap = TranslatorMainApplication.Properties.Resources._1;
            nf.Icon = new System.Drawing.Icon(Icon.FromHandle(bitmap.GetHicon()), new Size(32, 31));
            nf.BalloonTipIcon = ToolTipIcon.Info;
            nf.Visible = true;
            ContextMenu.MenuItemCollection menu = new ContextMenu.MenuItemCollection(nf.ContextMenu = new ContextMenu());
            menu.Add("Exit");
            menu[0].Click += new EventHandler(MenuClickExit);
            hk.KeyHooked += new KeyHooksEventHandler(hk_KeyHooked);
            hk.Start();

        }

        private void Closing(object sender, FormClosingEventArgs e)
        {
            hk.Stop();
        }

        private void Showned(object sender, EventArgs e)
        {
            this.Hide();
        }


    }
}
