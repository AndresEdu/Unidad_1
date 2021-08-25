using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AutomatasII
{
    class Lexico : Token, IDisposable
    {
        protected StreamReader archivo;
        protected StreamWriter bitacora;
        protected int linea, caracter;
        String NombreArch;
        const int F = -1;
        const int E = -2;

        int[,] trand6x = { // WS,EF, L, D, ., +, -, E, =, :, ;, &, |, !, >, <, *, /, %, ", ', ?,La, {, }, #10   :D
                            {  0, F, 1, 2,29,17,18, 1, 8, 9,11,12,13,15,26,27,20,32,20,22,24,28,29,30,31, 0},//0
                            {  F, F, 1, 1, F, F, F, 1, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//1
                            {  F, F, F, 2, 3, F, F, 5, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//2
                            {  E, E, E, 4, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E},//3
                            {  F, F, F, 4, F, F, F, 5, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//4
                            {  E, E, E, 7, E, 6, 6, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E},//5
                            {  E, E, E, 7, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E},//6
                            {  F, F, F, 7, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//7
                            {  F, F, F, F, F, F, F, F,16, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//8
                            {  F, F, F, F, F, F, F, F,10, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//9
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//10
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//11
                            {  F, F, F, F, F, F, F, F, F, F, F,14, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//12
                            {  F, F, F, F, F, F, F, F, F, F, F, F,14, F, F, F, F, F, F, F, F, F, F, F, F, F},//13
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//14
                            {  F, F, F, F, F, F, F, F,16, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//15
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//16
                            {  F, F, F, F, F,19, F, F,19, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//17
                            {  F, F, F, F, F, F,19, F,19, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//18
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//19
                            {  F, F, F, F, F, F, F, F,21, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//20
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//21
                            { 22, E,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,23,22,22,22,22,22,22},//22
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//23
                            { 24, E,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,25,24,24,24,24,24},//24
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//25
                            {  F, F, F, F, F, F, F, F,16, F, F, F, F, F,36, F, F, F, F, F, F, F, F, F, F, F},//26
                            {  F, F, F, F, F, F, F, F,16, F, F, F, F, F,16,37, F, F, F, F, F, F, F, F, F, F},//27
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//28
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//29
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//30
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//31
                            {  F, F, F, F, F, F, F, F,21, F, F, F, F, F, F, F,34,33, F, F, F, F, F, F, F, F},//32
                            { 33, 0,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33, 0},//33
                            { 34, E,34,34,34,34,34,34,34,34,34,34,34,34,34,34,35,34,34,34,34,34,34,34,34,34},//34
                            { 34, E,34,34,34,34,34,34,34,34,34,34,34,34,34,34,35, 0,34,34,34,34,34,34,34,34},//35
                            {  F, E, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//36
                            {  F, E, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//37
                            //WS,EF, L, D, ., +, -, E, =, :, ;, &, |, !, >, <, *, /, %, ", ', ?,La {, }, #10
                            };

        public Lexico()
        {
            linea = caracter = 1;

            Console.WriteLine("Compilando prueba.cpp");
            Console.WriteLine("Iniciando analisis Lexico");

            if (File.Exists("C:\\Archivos\\prueba.cpp"))
            {
                archivo = new StreamReader("C:\\Archivos\\prueba.cpp");
                bitacora = new StreamWriter("C:\\Archivos\\prueba.log");
                bitacora.AutoFlush = true;

                bitacora.WriteLine("Archivo: prueba.cpp");
                bitacora.WriteLine("Directorio: C:\\Archivos");
                bitacora.WriteLine("Fecha: 19 de abril de 2021 12:21");
            }
            else
            {
                throw new Exception("El archivo prueba.cpp no existe.");
            }

        }
        public Lexico(String nombre)
        {
            linea = caracter = 1;

            Console.WriteLine("Compilando " + nombre);
            Console.WriteLine("Iniciando analisis Lexico");

            if (Path.GetExtension(nombre) == ".cpp")  
            {
                NombreArch = Path.GetFileName(nombre);

                if (File.Exists(nombre))
                {
                    archivo = new StreamReader(nombre);
                    String log = Path.ChangeExtension(nombre, "log");
                    bitacora = new StreamWriter(log);
                    bitacora.AutoFlush = true;

                    String fecha = DateTime.Now.ToLongDateString();
                    DateTime Minseg = DateTime.Now;

                    bitacora.WriteLine("Archivo: " + NombreArch);
                    bitacora.WriteLine("Directorio: " + Path.GetDirectoryName(nombre));
                    bitacora.WriteLine("Fecha: " + fecha + " " + Minseg.ToString("T"));
                }
                else
                {
                    throw new Exception("No se puede leer debido a que el archivo: "+ NombreArch +" no existe");
                }
            }
            else
            {
                throw new Exception("El archivo "+ Path.GetFileName(nombre) +" no es un archivo .cpp");
            }

        }
        public void Dispose()
        {
            Console.WriteLine("Finaliza compilacion de " + NombreArch);
            CerrarArchivos();
        }

        private void CerrarArchivos()
        {
            archivo.Close();
            bitacora.Close();
        }

        protected void NextToken()
        {
            char transicion;
            string palabra = "";
            int estado = 0;
            int edoanterior = 0;

            while (estado >= 0)
            {
                edoanterior = estado;
                transicion = (char)archivo.Peek();

                estado = trand6x[estado, columna(transicion)];
                clasificar(estado);

                if (estado >= 0)
                {
                    archivo.Read();
                    caracter++;

                    if (transicion == 10)
                    {
                        linea++;
                        caracter = 1;
                    }

                    if (estado > 0)
                    {
                        palabra += transicion;
                    }
                    else
                    {
                        palabra = "";
                    }
                }
            }

            if (estado == E)
            {
                Clasificaciones ultimo = GetClasificacion();

                if (ultimo == Clasificaciones.numero)
                {
                    bitacora.WriteLine("Error lexico en la linea: {0} caracter: {1} se esperaba un digito", linea, caracter);
                    throw new Exception("Error lexico en la linea: " + linea + " caracter: " + caracter + " se esperaba un digito");
                }

                else if (ultimo == Clasificaciones.Cadena)
                {
                    if (edoanterior == 22)
                    {
                        bitacora.WriteLine("Error lexico en la linea: {0} caracter: {1} se esperaba comillas dobles", linea, caracter);
                        throw new Exception("Error lexico en la linea: " + linea + " caracter: " + caracter + " se esperaban comillas DOBLES");
                    }
                    else if (edoanterior == 24)
                    {
                        bitacora.WriteLine("Error lexico en la linea: {0} caracter: {1} se esperaba comillas simples", linea, caracter);
                        throw new Exception("Error lexico en la linea: " + linea + " caracter: " + caracter + " se esperaban comillas SIMPLES");
                    }
                }

                else //ELSE que representa SIN CLASIFICACION
                {
                    if (edoanterior == 34)
                    {
                        bitacora.WriteLine("Error lexico en la linea: {0} caracter: {1} se esperaba cierre de comentario MULTI-LINEA", linea, caracter);
                        throw new Exception("Error lexico en la linea: " + linea + " caracter: " + caracter + " se esperaban cierre de comentario MULTI-LINEA");
                    }
                    else if (edoanterior == 35)
                    {
                        bitacora.WriteLine("Error lexico en la linea: {0} caracter: {1} se esperaba cierre de comentario", linea, caracter);
                        throw new Exception("Error lexico en la linea: " + linea + " caracter: " + caracter + " se esperaban cierre de comentario");
                    }
                }

            }

            else if (palabra != "")
            {
                SetContenido(palabra);
                switch (palabra)
                {
                    case "char":
                    case "int":
                    case "float":
                    case "string":
                        SetClasificacion(Clasificaciones.TipoDato);
                        break;

                    case "private":
                    case "public":
                    case "protected":
                        SetClasificacion(Clasificaciones.Zona);
                        break;

                    case "if":
                    case "else":
                    case "switch":
                        SetClasificacion(Clasificaciones.Condicion);
                        break;

                    case "for":
                    case "while":
                    case "do":
                        SetClasificacion(Clasificaciones.ciclo);
                        break;
                }

                bitacora.WriteLine("Token = " + GetContenido());
                bitacora.WriteLine("Clasificacion = " + GetClasificacion());

            }
        }

        private void clasificar(int estado)
        {
            switch (estado)
            {
                case 1:
                    SetClasificacion(Clasificaciones.identificador);
                    break;
                case 2:
                    SetClasificacion(Clasificaciones.numero);
                    break;
                case 8:
                    SetClasificacion(Clasificaciones.asignacion);
                    break;
                case 9:
                case 12:
                case 13:
                case 29:
                    SetClasificacion(Clasificaciones.caracter);
                    break;
                case 10:
                    SetClasificacion(Clasificaciones.inicializacion);
                    break;
                case 11:
                    SetClasificacion(Clasificaciones.FinSentencia);
                    break;
                case 14:
                case 15:
                    SetClasificacion(Clasificaciones.OperadorLogico);
                    break;
                case 16:
                case 26:
                case 27:
                    SetClasificacion(Clasificaciones.OperadorRelacional);
                    break;
                case 17:
                case 18:
                    SetClasificacion(Clasificaciones.OperadorTermino);
                    break;
                case 19:
                    SetClasificacion(Clasificaciones.IncrementoTermino);
                    break;
                case 20:
                case 32:
                    SetClasificacion(Clasificaciones.OperadorFactor);
                    break;
                case 21:
                    SetClasificacion(Clasificaciones.IncrementoFactor);
                    break;
                case 22:
                case 24:
                    SetClasificacion(Clasificaciones.Cadena);
                    break;
                case 28:
                    SetClasificacion(Clasificaciones.OperadorTernario);
                    break;
                case 30:
                    SetClasificacion(Clasificaciones.IniciodeBloque);
                    break;
                case 31:
                    SetClasificacion(Clasificaciones.FindeBloque);
                    break;
                case 36:
                    SetClasificacion(Clasificaciones.FlujoEntrada);
                    break;
                case 37:
                    SetClasificacion(Clasificaciones.FlujoSalida);
                    break;
            }
        }

        private int columna(char t)
        {
            // WS,EF, L, D, ., +, -, E, =, :, ;, &, |, !, >, <, *, /, %, ", ', ?,La {, }, #10
            if (FinDeArchivo())
            {
                return 1;
            }
            else if (t == 10)
            {
                return 25;
            }
            else if (char.IsWhiteSpace(t))
            {
                return 0;
            }
            else if (char.ToLower(t) == 'e')
            {
                return 7;
            }
            else if (char.IsLetter(t))
            {
                return 2;
            }
            else if (char.IsDigit(t))
            {
                return 3;
            }
            else if (t == '.')
            {
                return 4;
            }
            else if (t == '+')
            {
                return 5;
            }
            else if (t == '-')
            {
                return 6;
            }
            else if (t == '=')
            {
                return 8;
            }
            else if (t == ':')
            {
                return 9;
            }
            else if (t == ';')
            {
                return 10;
            }
            else if (t == '&')
            {
                return 11;
            }
            else if (t == '|')
            {
                return 12;
            }
            else if (t == '!')
            {
                return 13;
            }
            else if (t == '>')
            {
                return 14;
            }
            else if (t == '<')
            {
                return 15;
            }
            else if (t == '*')
            {
                return 16;
            }
            else if (t == '/')
            {
                return 17;
            }
            else if (t == '%')
            {
                return 18;
            }
            else if (t == '"')
            {
                return 19;
            }
            else if (t == '\'')
            {
                return 20;
            }
            else if (t == '?')
            {
                return 21;
            }
            else if (t == '{')
            {
                return 23;
            }
            else if (t == '}')
            {
                return 24;
            }
            else
            {
                return 22;
            }
            // WS,EF, L, D, ., +, -, E, =, :, ;, &, |, !, >, <, *, /, %, ", ', ?,La {, }, #10
        }

        public bool FinDeArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}