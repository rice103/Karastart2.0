using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace KaraStart
{
    public partial class MailForm : Form
    {
        String defaultMessage = "";
        public MailForm(String defaultMessage)
        {
            InitializeComponent();
            this.defaultMessage = defaultMessage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            String from = textBox1.Text.Contains("@") ? textBox1.Text : "rice103@gmail.com";
            String to = "rice103@gmail.com";
            String subject = "Suggerimento per KARASTART";
            if (defaultMessage!="")
                subject = "CRASHREPORT per KARASTART";
            String body = textBox2.Text;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = false;
            client.Credentials = new NetworkCredential("ricei0io@gmail.com", "fricki0io");
            client.EnableSsl = true;

            try
            {
                // Create instance of message
                MailMessage message = new MailMessage();

                // Add receivers
                message.To.Add(to);

                // Set sender
                message.From = new MailAddress(from);

                // Set subject
                message.Subject = subject;

                // Set body of message
                message.Body = body + "\n\rSystem version: " + Kernel32.getOsInfo() +
                                      "\n\rFramework version: " + Kernel32.getVersionFromRegistry() +
                                      "\n\rSqlCe installed: " + Kernel32.getSqlCeCompactInstalled() +
                                      "\n\rNAudio version: " + Kernel32.getAssemblyVersion("NAudio") +
                                      "\n\rAudioSwitch version: " + Kernel32.getAssemblyVersion("AudioSwitch") +
                                      "\n\rDragNDrop version: " + Kernel32.getAssemblyVersion("DragNDrop") +
                                      "\n\rMidi version: " + Kernel32.getAssemblyVersion("Midi") +
                                      "\n\rJockerSoft.Media version: " + Kernel32.getAssemblyVersion("JockerSoft.Media") +
                                      "\n\rProfile version: " + Kernel32.getAssemblyVersion("AMS.Profile") +
                                      "";

                //validate the certificate
                ServicePointManager.ServerCertificateValidationCallback =
                delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };


                // Send the message
                client.Send(message);

                // Clean up
                message = null;
            }
            catch { }
            
            ////Istanzio la classe che rappresenta il messaggio
            //MailMessage mMailMessage = new MailMessage();
            ////Aggiungo il Mittente
            //mMailMessage.From = new MailAddress(from);
            ////Aggiungo il destinatario
            //mMailMessage.To.Add(new MailAddress(to));
            ////L'oggetto
            //mMailMessage.Subject = subject;
            ////Il corpo
            //mMailMessage.Body = body;
            ////Setto la modalità testo, per il contenuto del messaggio. Sarebbe possibile inviare anche dell'HTML mettendo true
            //mMailMessage.IsBodyHtml = false;
            ////Setto la priorità
            //mMailMessage.Priority = MailPriority.Normal;

            ////configurazione nel web.config
            //SmtpClient mSmtpClient = new SmtpClient("smtp.gmail.com", 465);
            ////Invio il messaggio
            //NetworkCredential myCreds = new NetworkCredential("ricei0io@gmail.com", "fricki0io", ""); 
            //mSmtpClient.Credentials = myCreds; 
            //mSmtpClient.Send(mMailMessage);
            Cursor.Current = Cursors.Default;
            this.Hide();
        }

        private void MailForm_Load(object sender, EventArgs e)
        {
            this.textBox2.Text = defaultMessage;
        }
    }
}
