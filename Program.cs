using System;

namespace AutomatasII
{
    class Program
    {
        static void Main(string[] args)
        {            
            try
            {
                using (Lenguaje l = new Lenguaje("C:\\Archivos\\suma.cpp"))                
                {
                    /*while (!l.FinDeArchivo())
                    {
                        l.NextToken();
                    }*/
                    l.Programa();

                }
            }
            catch (Error e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
