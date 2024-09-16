using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SharpIgnite
{
    public interface IMailable
    {
        string To { get; set; }

        string Subject { get; set; }
        string Body { get; set; }

        IMailer Mailer { get; set; }
    }

    public class Mailable : IMailable
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public IMailer Mailer { get; set; }

        public Mailable() { }

        public virtual IMailer SetMailer(string mailMailer)
        {
            if (mailMailer == "Smtp") {
                Mailer = new SmtpMailer();
            } else {
                throw new NotSupportedException();
            }
            return Mailer;
        }

        public Mailable SetMailer(IMailer mailer)
        {
            this.Mailer = mailer;
            return this;
        }

        public Mailable SetTo(string to)
        {
            this.To = to;
            return this;
        }

        public Mailable SetSubject(string subject)
        {
            this.Subject = subject;
            return this;
        }

        public Mailable With(string content, string[] oldValues, string[] newValues)
        {
            for (int i = 0; i < oldValues.Length; i++) {
                content = content.Replace(oldValues[i], newValues[i]);
            }
            this.Body = content;
            return this;
        }

        public Mailable SetBody(string body)
        {
            this.Body = body;
            return this;
        }

        public virtual Mailable Build()
        {
            throw new NotImplementedException();
        }

        public virtual void Send(Mailable mailable)
        {
            mailable.SetTo(To ?? mailable.To);
            mailable.Build();
            Mailer.Send(mailable);
        }
    }

    public interface IMailMessage : IMailable
    {
        string From { get; set; }
        bool TextOnly { get; set; }
        NameValueCollection Headers { get; set; }
    }

    public class MailMessage : Mailable, IMailMessage
    {
        public string From { get; set; }
        public bool TextOnly { get; set; }
        public NameValueCollection Headers { get; set; }

        public MailMessage SetFrom(string from)
        {
            this.From = from;
            return this;
        }

        public MailMessage SetTextOnly(bool textOnly)
        {
            this.TextOnly = textOnly;
            return this;
        }

        public bool EnableSsl { get; set; }

        public MailMessage SetHeaderIf(bool condition, string key, string value)
        {
            if (condition) {
                SetHeader(key, value);
            }
            return this;
        }

        public MailMessage SetHeader(string key, string value)
        {
            if (Headers == null) {
                Headers = new NameValueCollection();
            }
            Headers.Add(key, value);
            return this;
        }

        public MailMessage SetHeaders(NameValueCollection headers)
        {
            this.Headers = headers;
            return this;
        }

        public MailMessage Settings(string host, int? port, string username, string password)
        {
            var mailer = Mailer as SmtpMailer;
            mailer.Settings(host ?? mailer.Host,
                port ?? mailer.Port,
                username ?? mailer.Username,
                password ?? mailer.Password);
            return this;
        }

        public override void Send(Mailable mailable)
        {
            var message = mailable as MailMessage;
            if (message == null) {
                throw new ArgumentException();
            }

            message.SetHeaders(Headers ?? message.Headers);
            message.SetFrom(From ?? message.From);
            message.SetTextOnly(TextOnly);

            base.Send(mailable);
        }
    }

    public interface IMailer
    {
        void Send(IMailable mailable);
    }

    public class Mailer : IMailer
    {
        public virtual void Send(IMailable mailable)
        {
        }

        public static Mailer GetMailer(string mailer)
        {
            if (mailer.ToLower().Equals("mail")) {
                return new SmtpMailer();
            }
            throw new NotSupportedException();
        }
    }

    public class SmtpMailer : Mailer // SMTP Mailer
    {
        public string Host { get; set; }
        public int? Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        bool enableSsl;

        public SmtpMailer Settings(string host, int? port, string username, string password)
        {
            this.Host = host;
            this.Port = port;
            this.Username = username;
            this.Password = password;
            return this;
        }

        public SmtpMailer EnableSsl(bool enableSsl)
        {
            this.enableSsl = enableSsl;
            return this;
        }

        public override void Send(IMailable mailable)
        {
            var message = mailable as IMailMessage;
            if (message == null) {
                throw new ArgumentException();
            }
            var mail = new System.Net.Mail.MailMessage(message.From, message.To, message.Subject, message.Body);
            mail.IsBodyHtml = !message.TextOnly;

            if (message.Headers != null) {
                foreach (var headerKey in message.Headers?.Keys) {
                    mail.Headers.Add(headerKey.ToString(), message.Headers[headerKey.ToString()]);
                }
            }

            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            var client = new System.Net.Mail.SmtpClient(Host, Port ?? 25);
            client.Credentials = new NetworkCredential(Username, Password);
            client.EnableSsl = enableSsl;
            client.Send(mail);
        }
    }

    public class Mail
    {
        // Facade

        public static MailMessage Settings(string host, int? port, string username, string password)
        {
            var mailer = new SmtpMailer()
                .Settings(host, port, username, password);
            return new MailMessage()
                .SetMailer(mailer) as MailMessage;
        }

        public static MailMessage To(string to)
        {
            var mailer = new SmtpMailer()
                .Settings(Config.Get("MAIL_HOST"),
                    Config.Get<int?>("MAIL_PORT"),
                    Config.Get("MAIL_USERNAME"),
                    Config.Get("MAIL_PASSWORD"));
            return new MailMessage()
                .SetFrom(Config.Get("MAIL_FROM_ADDRESS"))
                .SetTo(to)
                .SetMailer(mailer) as MailMessage;
        }
    }
}