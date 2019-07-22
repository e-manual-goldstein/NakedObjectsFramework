using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedFunctions;

namespace AdventureWorksModel
{
    public class EmailTemplate : IViewModelEdit
    {

        public EmailTemplate(
            string to,
            string from,
            string subject,
            string message,
            EmailStatus status)
        {
            To = to;
            From = from;
            Subject = subject;
            Message = message;
            Status = status;
        }

        public EmailTemplate() { }

        public string[] DeriveKeys()
        {
            return new[] {
                To,
                From,
                Subject,
                Message,
                Status.ToString() };
        }

        public void PopulateUsingKeys(string[] keys)
        {
            this.To = keys[0];
            this.From = keys[1];
            this.Subject = keys[2];
            this.Message = keys[3];
            this.Status = (EmailStatus)Enum.Parse(typeof(EmailStatus), keys[4]);
        }

        [MemberOrder(10), Optionally]
        public virtual string To { get; set; }

        [MemberOrder(20), Optionally]
        public virtual string From { get; set; }

        [MemberOrder(30), Optionally]
        public virtual string Subject { get; set; }

        [MemberOrder(40), Optionally]
        public virtual string Message { get; set; }

        [Disabled]
        public virtual EmailStatus Status { get; set; }

    }

    public static class EmailTemplateFunctions
    {
        public static string Title(this EmailTemplate et)
        {
            return et.CreateTitle($"{((EmailStatus)et.Status).ToString()} email");
        }

        public static (EmailTemplate, EmailTemplate) Send(EmailTemplate et)
        {
            return (null, et.With(x => x.Status, EmailStatus.Sent));
 
        }

        public static IQueryable<string> AutoCompleteSubject(EmailTemplate et, [MinLength(2)] string value)
        {
            var matchingNames = new List<string> { "Subject1", "Subject2", "Subject3" };
            return from p in matchingNames.AsQueryable() select p.Trim();
        }

    }
    public enum EmailStatus
    {
        New, Sent, Failed
    }

    public enum EmailPromotion
    {
        NoPromotions = 0,
        AdventureworksOnly = 1,
        AdventureworksAndPartners = 2
    }
}
