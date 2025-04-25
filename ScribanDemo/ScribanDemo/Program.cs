using Scriban;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XmlToClassGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputXml = "model.xml";
            string templatePath = "ClassTemplate.sbn";
            string outputDir = "Generated";

            Directory.CreateDirectory(outputDir);

            var doc = XDocument.Load(inputXml);

            var classes = doc.Root.Elements("class").Select(cls => new
            {
                Name = (string)cls.Attribute("name"),
                Properties = cls.Elements("property").Select(prop => new
                {
                    Type = (string)prop.Attribute("type"),
                    Name = (string)prop.Attribute("name")
                }).ToList()
            }).ToList();

            string templateText = File.ReadAllText(templatePath);
            var template = Template.Parse(templateText);

            foreach (var cls in classes)
            {
                Console.WriteLine(cls.Name);
                var result = template.Render(new { model = cls });
                File.WriteAllText(Path.Combine(outputDir, $"{cls.Name}.cs"), result);
                Console.WriteLine($"Generated {cls.Name}.cs");
            }
        }
    }
}