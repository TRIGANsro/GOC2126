using System;

namespace NotifyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AutoNotifyAttribute : Attribute
    {
        public string ExtraInfo { get; set; }

    }
}
