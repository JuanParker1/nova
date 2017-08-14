using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace ExtratPaie
{
    class Program
    {
        static void Main()
        {
            XsdValidator s = new XsdValidator();
            s.AddSchema("G:\\manifeste_guce_v3.2.2-1.xsd");
            //bool re = s.IsValid("G:\\GHA0616-8567874324.xml");
            bool re = s.IsValid("G:\\GHA0616-SYTRAM.xml");

           Console.WriteLine(re+" "+s.Errors.Count+" ");
            
           Console.ReadLine();

        }

        static void booksSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
            }
        }
    }
}
