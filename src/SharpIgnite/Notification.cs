using System;

namespace SharpIgnite
{
    public interface INotifiable
    {
        void Notify(Notification notification);
    }

    public class Notification
    {
        protected INotifiable notifiable;

        public virtual string[] Via()
        {
            return new string[] { };
        }

        public void Send(INotifiable notifiable)
        {
            this.notifiable = notifiable;
            var type = GetType();
            foreach (var viaMethod in Via()) {
                var methodInfo = type.GetMethod("To" + viaMethod);
                if (methodInfo != null) {
                    // Assuming the method takes no arguments
                    methodInfo.Invoke(this, null);
                } else {
                    Console.WriteLine($"Method To{viaMethod} not found.");
                }
            }
        }
    }
}
