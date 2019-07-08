using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel
{
    public class EmailTemplate : IViewModelEdit
    {
        private EmailStatus @new;

        public EmailTemplate(EmailStatus @new)
        {
            this.@new = @new;
        }
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region 
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
        #endregion


        public override string ToString()
        {
            var t = Container.NewTitleBuilder();
            t.Append(Status).Append("email");
            return t.ToString();
        }

        [MemberOrder(10), Optionally]
        public virtual string To { get; set; }

        [MemberOrder(20), Optionally]
        public virtual string From { get; set; }

        [MemberOrder(30), Optionally]
        public virtual string Subject { get; set; }

        public virtual IQueryable<string> AutoCompleteSubject([MinLength(2)] string value)
        {
            var matchingNames = new List<string> { "Subject1", "Subject2", "Subject3" };
            return from p in matchingNames.AsQueryable() select p.Trim();
        }

        [MemberOrder(40), Optionally]
        public virtual string Message { get; set; }

        [Disabled]
        public virtual EmailStatus Status { get; set; }

        public EmailTemplate Send()
        {
            this.Status = EmailStatus.Sent;
            return this;
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
