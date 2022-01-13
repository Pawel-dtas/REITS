using System;


namespace REITs.Infrastructure.CustomAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ViewName : Attribute
    {
        public string Name { get; private set; }

        public ViewName(string name)
        {
            Name = name;
        }
    }
}