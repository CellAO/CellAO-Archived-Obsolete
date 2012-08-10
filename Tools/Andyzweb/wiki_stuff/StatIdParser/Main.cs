using System;
using System.Xml;
using System.IO;

namespace StatIdParser
{
    /// <summary>
    /// Summary description for StatParsing.
    /// </summary>
    class StatParsing
    {
        static void Main(string[] args)
        {
            XmlTextReader reader = new XmlTextReader ("Stats.xml");
                        reader.Read();
                    while (reader.Read())
                    {
                      if (reader.HasAttributes)
                      {
                                reader.MoveToAttribute("id");
                                string id = reader.Value;
                                reader.MoveToAttribute("Name");
                                string name = reader.Value;
                                Console.WriteLine("| " + id + " | " + name + " |");
                        reader.MoveToElement();
                      }
                    }

         }
    }
}
