using System;

namespace REITs.Domain.Models
{
    public abstract class BaseSystemUser
    {
        public Guid Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string FullName
        {
            get { return string.Format("{0} {1}", Forename, Surname); }
        }
        public string FullNameReversed
        {
            get { return string.Format("{0}, {1}", Surname, Forename); }
        }
    }
}
