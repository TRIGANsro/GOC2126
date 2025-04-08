namespace ClassInfoGenerated;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class MyInfoAttribute : Attribute
{
    public string ExtraInfo { get; set; }
}