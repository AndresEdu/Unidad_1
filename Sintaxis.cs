using System;
using System.Collections.Generic;
using System.Text;

namespace AutomatasII
{
    class Sintaxis : Lexico
    {
        public Sintaxis()
        {
            Console.WriteLine("Iniciando analisis Sintatico");
            NextToken();
        }
        public Sintaxis(String nombre) : base(nombre)

        {
            Console.WriteLine("Iniciando analisis Sintatico");
            NextToken();
        }
        protected void match(string espera)
        {

            if (espera == GetContenido())
            {
                NextToken();
            }
            else
            {
                bitacora.WriteLine("Error de sintaxis: se espera un " + espera + ". Linea: " + linea +" Caracter: "+ caracter);
                throw new Exception("Error de sintaxis: se espera un " + espera+". Linea: "+ linea + " Caracter: " + caracter);
            }
        }

        protected void match(Clasificaciones espera)
        {
            if (espera == GetClasificacion())
            {
                NextToken();
            }
            else
            {
                bitacora.WriteLine("Error de sintaxis: se espera un " + espera + ". Linea: " + linea + " Caracter: " + caracter);
                throw new Exception("Error de sintaxis: se espera un " + espera + ". Linea: " + linea + " Caracter: " + caracter);
            }
        }

    }
}