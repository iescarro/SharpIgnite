using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SharpIgnite
{
    public interface IValidator
    {
        bool IsValid { get; }
        void Validate(object obj);
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ValidatorAttribute : Attribute
    {
        public string Message { get; set; }
        public IValidator Validator { get; set; }

        public ValidatorAttribute() { }

        public ValidatorAttribute(IValidator validator) : this(validator, "") { }

        public ValidatorAttribute(IValidator validator, string message)
        {
            this.Validator = validator;
            this.Message = message;
        }
    }

    public class RequiredAttribute : ValidatorAttribute
    {
        public RequiredAttribute() : base(new RequiredValidator()) { }

        public RequiredAttribute(string message) : base(new RequiredValidator(), message) { }
    }

    public class RequiredValidator : Validator
    {
        public RequiredValidator() { }

        public RequiredValidator(string obj)
        {
            Validate(obj);
        }

        public override void Validate(object obj)
        {
            isValid = obj != null && !string.IsNullOrEmpty(obj.ToString());
        }
    }

    public class Validator : IValidator
    {
        protected bool isValid;

        public virtual bool IsValid {
            get {
                return isValid;
            }
        }

        public virtual void Validate(object obj)
        {
        }
    }

    public class EmailValidator : Validator
    {
        bool invalid;

        public EmailValidator(string email)
        {
            Validate(email);
        }

        public override void Validate(object obj)
        {
            string email = obj.ToString();
            invalid = false;
            if (String.IsNullOrEmpty(email))
                return;

            // Use IdnMapping class to convert Unicode domain names.
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper);
            if (invalid)
                return;

            // Return true if strIn is in valid e-mail format.
            isValid = Regex.IsMatch(email,
                                 @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                 @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                                 RegexOptions.IgnoreCase);
        }

        string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try {
                domainName = idn.GetAscii(domainName);
            } catch (ArgumentException) {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}
