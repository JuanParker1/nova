using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EchangeDGD_SOCOMAR
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1- navire ; 2- armateur ; 3 Acconier ; 4. Requests; 6.SAGE ");
            string ans = Console.ReadLine();
            switch (ans)
            {
                case "1":
                    Navire();
                    break;
                case "2" :
                    Armateur();
                    break;
                case "3" :
                    Acconier();
                    break;
                case "4":
                    Request();
                    break;
                case "5":
                    Douane();
                    break;
                case "6":
                    Sage();
                    break;
            }
        }
        private static void Sage()
        {
            VesselStopOverData.VsomSAGE v = new VesselStopOverData.VsomSAGE();
            //v.NOVAReport();
            v.NOVA_TRANSACTIONS();
        }
        private static void Navire()
        {
            VesselStopOverData.GuichetUnique gu = new VesselStopOverData.GuichetUnique();
            string path = gu.Navire();
            Console.WriteLine(path);
            Console.ReadKey();
        }

        private static void Armateur()
        {
            VesselStopOverData.GuichetUnique gu = new VesselStopOverData.GuichetUnique();
            string path = gu.Armateur();
            Console.WriteLine(path);
            Console.ReadKey();
        }

        private static void Acconier()
        {
            VesselStopOverData.GuichetUnique gu = new VesselStopOverData.GuichetUnique();
            string path = gu.Acconier();
            Console.WriteLine(path);
            Console.ReadKey();
        }

        private static void Request()
        {
            VesselStopOverData.GuichetUnique gu = new VesselStopOverData.GuichetUnique();
            gu.BuildRequest();
            Console.WriteLine();
            Console.ReadKey();
        }

        private static void Douane()
        {
            VesselStopOverData.GuichetUnique gu = new VesselStopOverData.GuichetUnique();
            gu.ReadRequest(@"G:\\gu_files\VM_CMDLP_2016_324_201607181330.txt");
        }

    }
}
