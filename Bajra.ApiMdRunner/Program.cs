using Bajra.ApiMdGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            path = Directory.GetParent(path).FullName;

            path = Path.Combine(path + "\\AB.SampleWithApi\\bin\\Debug");
            string asm = "AB.SampleWithApi.dll";

            string dllPath = Path.Combine(path, asm);
            string xmlPath = Path.Combine(path, "AB.SampleWithApi.XML");

            MdGeneratorCore mdCore = new MdGeneratorCore(dllPath, xmlPath, AppSettings.MdOutputPath);

            mdCore.GenerateMDFilesForAssembly();

        }


    }
}
