using NUnit.Framework;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class MailTests
    {
        [Test]
        public void TestMailer()
        {
            string host = Config.Get("SmtpServer");
            int? port = Config.Get<int?>("SmtpPort");
            string username = Config.Get("SmtpUsername");
            string password = Config.Get("SmtpPassword");
            string from = Config.Get("EmailFrom");

            var mailer = new SmtpMailer()
                .Settings(host, port, username, password)
                .EnableSsl(true);
            var message = new MailMessage()
                .SetHeader("X-Auto-Response-Suppress", "DR, RN, NRN, OOF, AutoReply")
                .SetTextOnly(true)
                .SetFrom(from)
                .SetTo("ian.escarro@gmail.com")
                .SetSubject("Test subject")
                .SetBody("This is mail body!");
            mailer.Send(message);
        }

        [Test]
        public void TestWithSettings()
        {
            int userID = 42;
            string healthWatchURL = "https://test.healthwatch.se";
            string encryptedUserID = Crypt.Encrypt(userID.ToString());
            string emailUnsubscribe = Config.Get("EmailUnsubscribe");

            Mail.To("ian.escarro@gmail.com")
                .SetHeader("X-Auto-Response-Suppress", "DR, RN, NRN, OOF, AutoReply")
                .SetHeader("Auto-Submitted", "auto-generated")
                .SetHeaderIf(userID > 0, "List-Unsubscribe", $"<${healthWatchURL}/unsubscribe.aspx?id=${encryptedUserID}>, <mailto:${emailUnsubscribe}?Subject=Unsubscribe-${encryptedUserID}>")
                .SetFrom(Config.Get("EmailFrom"))
                .Settings(Config.Get("SmtpServer"),
                    Config.Get<int?>("SmtpPort"),
                    Config.Get("SmtpUsername"),
                    Config.Get("SmtpPassword"))
                .Send(new LoginReminder());
        }

        [Test]
        public void TestText()
        {
            Mail.To("ian.escarro@gmail.com")
              .SetTextOnly(true)
              .Send(new LoginReminder());
        }

        [Test]
        public void TestHtml()
        {
            Mail.To("ian.escarro@gmail.com")
                .Send(new LoginReminder());
        }

        public class LoginReminder : MailMessage
        {
            public override Mailable Build()
            {
                return SetSubject("Login reminder")
                    .With(@"Hello, {name}

<p>This is a reminder to login to the system.</p>

<a href={url}'>{url}</a>

<p>Best regards,<br>
{company}
</p>",
                    new string[] { "{name}", "{url}", "{company}" },
                    new string[] { "Bob", "https://gmail.com", "Google" });
            }
        }
    }
}