using HW.Core.Util;
using NUnit.Framework;
using SharpIgnite;
using System.Collections.Generic;

namespace HW.Core.Tests.Util.SharpIgnite
{
    [TestFixture]
    public class NotificationTests
    {
        [Test]
        public void TestNotification()
        {
            var u = new MyUser {
                Email = "ian.escarro@gmail.com",
                FirebaseTokens = new string[] {
                    "fvwlmPh3QEmEsOMRsxAr2J:APA91bFgchRxH564rP1AEVVJVJ8nnG7OoR3mCBwfGSKx7h9oySKW5Jc8nE9QzAdApEUOcuwOstHhBChmzfYsjNbeebQPbFe_Z7dUI1SgrB_QqfFCo-pVfiIEqgdbyaolCwiDMvlzzmWI",
                }
            };
            u.Notify(new LoginNotification(
                "Email subject",
                "Email body",
                "Notification title",
                "Notification body"));
        }

        public class MyUser : INotifiable
        {
            public string Email { get; set; }
            public IEnumerable<string> FirebaseTokens { get; set; }

            public void Notify(Notification notification)
            {
                notification.Send(this);
            }
        }

        public class LoginNotification : Notification
        {
            string emailSubject;
            string emailBody;
            string notificationTitle;
            string notificationBody;

            public LoginNotification(string emailSubject, string emailBody, string notificationTitle, string notificationBody)
            {
                this.emailSubject = emailSubject;
                this.emailBody = emailBody;
                this.notificationTitle = notificationTitle;
                this.notificationBody = notificationBody;
            }

            public override string[] Via()
            {
                return new string[] { "Mail", "Fcm" };
            }

            public void ToMail()
            {
                //Console.WriteLine("ToMail");
                var email = (notifiable as MyUser).Email;
                new MailMessage()
                    .SetTo(email)
                    .SetSubject(emailSubject)
                    .SetBody(emailBody);
            }

            public async void ToFcm()
            {
                //Console.WriteLine("ToFcm");
                var user = (notifiable as MyUser);
                var tokens = (notifiable as MyUser).FirebaseTokens;
                foreach (var token in tokens) {
                    await Fcm.Send(token, notificationTitle, notificationBody);
                }
            }
        }
    }
}
