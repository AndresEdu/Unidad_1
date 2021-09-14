using System;
using System.Collections.Generic;
using System.Text;

// :)) Requerimiento 1: Implementar las secuencias de escape: \n, \t cuando se imprime una cadena y 
//                      eliminar las dobles comillas.
// :)) Requerimiento 2: Levantar excepciones en la clase Stack.
// :)) Requerimiento 3: Agregar el tipo de dato en el Inserta de ListaVariables.
// :)) Requerimiento 4: Validar existencia o duplicidad de variables. Mensaje de error: 
//                      "Error de sintaxis: La variable (x26) no ha sido declarada."
//                      "Error de sintaxis: La variables (x26) está duplicada." 
// :)) Requerimiento 5: Modificar el valor de la variable o constante al momento de su declaracion.

namespace AutomatasII
{
    class Lenguaje : Sintaxis
    {
        Stack s;
        ListaVariables l;
        public Lenguaje()
        {
            s = new Stack(5);
            l = new ListaVariables();
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        public Lenguaje(string nombre) : base(nombre)
        {   
            s = new Stack(5);
            l = new ListaVariables();
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        // Programa -> Libreria Main
        public void Programa()
        {
            Libreria();
            Main();
            l.imprime(bitacora);
        }

        // Libreria -> (#include <identificador(.h)?> Libreria) ?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(clasificaciones.identificador);

                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }

                match(">");

                Libreria();
            }
        }

        // Main -> tipoDato main() BloqueInstrucciones 
        private void Main()
        {
            match(clasificaciones.tipoDato);
            match("main");
            match("(");
            match(")");

            BloqueInstrucciones();
        }

        // BloqueInstrucciones -> { Instrucciones }
        private void BloqueInstrucciones()
        {
            match(clasificaciones.inicioBloque);
            Instrucciones();
            match(clasificaciones.finBloque);
        }

        // Lista_IDs -> identificador (= Expresion)? (,Lista_IDs)? 
        private void Lista_IDs(Variable.tipo Tipo)
        {
            string nombre = getContenido();
            
            if(!l.Existe(nombre))
            {
                match(clasificaciones.identificador); //Validar duplicidad
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis:La variable  (" + nombre + ") esta Duplicada. " + "(" + linea + ", " + caracter + ")");
            }

            l.Inserta(nombre, Tipo);

            if (getClasificacion() == clasificaciones.asignacion)
            {   
                match(clasificaciones.asignacion);
                Expresion();
                l.setValor(nombre,s.Pop(bitacora,linea,caracter).ToString());
            }

            if (getContenido() == ",")
            {
                match(",");
                Lista_IDs(Tipo);
            }
        }

        // Variables -> tipoDato Lista_IDs; 
        private void Variables()
        {
            string TipoDato = getContenido();
            match(clasificaciones.tipoDato);
            Variable.tipo TipodeVariable;

            switch (TipoDato)
            {
                case "int":
                    TipodeVariable = Variable.tipo.INT;
                    break;
                case "float":
                    TipodeVariable = Variable.tipo.FLOAT;
                    break;
                case "char":
                    TipodeVariable = Variable.tipo.CHAR;
                    break;
                case "string":
                    TipodeVariable = Variable.tipo.STRING;
                    break;
                default:
                    TipodeVariable = Variable.tipo.INT;
                    break;
            }

            Lista_IDs(TipodeVariable);
            match(clasificaciones.finSentencia);
        }

        // Instruccion -> (If | cin | cout | const | Variables | asignacion) ;
        private void Instruccion()
        {
            if (getContenido() == "do")
            {
                DoWhile();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "cin")
            {
                match("cin");
                match(clasificaciones.flujoEntrada);

                string nombre = getContenido(); 
                if(l.Existe(nombre))
                {
                    string entrada = Console.ReadLine();
                    match(clasificaciones.identificador); //Validar existencia
                    l.setValor(nombre,entrada);
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable (" + nombre + ") no declarada " + "(" + linea + ", " + caracter + ")");
                }

                match(clasificaciones.finSentencia);
            }
            else if (getContenido() == "cout")
            {
                match("cout");
                ListaFlujoSalida();
                match(clasificaciones.finSentencia);
            }
            else if (getContenido() == "const")
            {
                Constante();
            }
            else if (getClasificacion() == clasificaciones.tipoDato)
            {
                Variables();
            }
            else
            {
                string nombre = getContenido();

                if(l.Existe(nombre))
                {
                    match(clasificaciones.identificador); //Validar existencia
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable (" + nombre + ") no declarada " + "(" + linea + ", " + caracter + ")");
                }

                match(clasificaciones.asignacion);
                
                string valor;

                if (getClasificacion() == clasificaciones.cadena)
                {
                    valor = getContenido();
                    match(clasificaciones.cadena);
                }

                else
                {
                    Expresion();
                    valor = s.Pop(bitacora,linea,caracter).ToString();
                }

                l.setValor(nombre, valor);
                match(clasificaciones.finSentencia);
            }
        }

        // Instrucciones -> Instruccion Instrucciones?
        private void Instrucciones()
        {
            Instruccion();

            if (getClasificacion() != clasificaciones.finBloque)
            {
                Instrucciones();
            }
        }

        // Constante -> const tipoDato identificador = numero | cadena;
        private void Constante()
        {
            match("const");
            string TipoDato = getContenido();
            match(clasificaciones.tipoDato);
            Variable.tipo TipodeVariable;

            switch (TipoDato)
            {
                case "int":
                    TipodeVariable = Variable.tipo.INT;
                    break;
                case "float":
                    TipodeVariable = Variable.tipo.FLOAT;
                    break;
                case "char":
                    TipodeVariable = Variable.tipo.CHAR;
                    break;
                case "string":
                    TipodeVariable = Variable.tipo.STRING;
                    break;
                default:
                    TipodeVariable = Variable.tipo.CHAR;
                    break;
            }

            string nombre = getContenido();
            if(!l.Existe(nombre))
            {
                match(clasificaciones.identificador); //Validar duplicidad
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: La constante (" + nombre + ") esta duplicada" + "(" + linea + ", " + caracter + ")");
            }
            
            l.Inserta(nombre,TipodeVariable);
            match(clasificaciones.asignacion);  

            
            if (getClasificacion() == clasificaciones.numero)
            {
                l.setValor(nombre,getContenido());  //Se le hace un Set al valor en la lista
                match(clasificaciones.numero);
            }
            else
            {
                l.setValor(nombre,getContenido());  //Se le hace un Set al valor en la lista
                match(clasificaciones.cadena);
            }

            match(clasificaciones.finSentencia);
        }

        // ListaFlujoSalida -> << cadena | identificador | numero (ListaFlujoSalida)?
        private void ListaFlujoSalida()
        {
            match(clasificaciones.flujoSalida);

            if (getClasificacion() == clasificaciones.numero)
            {
                Console.Write(getContenido());
                match(clasificaciones.numero);  
            }
            else if (getClasificacion() == clasificaciones.cadena)
            {
                string secuencias = getContenido();

                if (secuencias.Contains("\""))
                {
                    secuencias = secuencias.Replace("\""," ");
                }
                if (secuencias.Contains("\\t"))
                {
                    secuencias = secuencias.Replace("\\t","\t");
                }
                if (secuencias.Contains("\\n"))
                {
                    secuencias = secuencias.Replace("\\n","\n");
                }
                Console.Write(secuencias);
                match(clasificaciones.cadena);
            }
            else
            {
                string nombre = getContenido();

                if(l.Existe(nombre))
                {
                    Console.Write(l.getValor(nombre));
                    match(clasificaciones.identificador); //Validar existencia
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable (" + nombre + ") no declarada " + "(" + linea + ", " + caracter + ")");
                }
            }

            if (getClasificacion() == clasificaciones.flujoSalida)
            {
                ListaFlujoSalida();
            }
        }

        // If -> if (Condicion) { BloqueInstrucciones } (else BloqueInstrucciones)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            BloqueInstrucciones();

            if (getContenido() == "else")
            {
                match("else");
                BloqueInstrucciones();
            }
        }

        // Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(clasificaciones.operadorRelacional);
            Expresion();
        }

        // x26 = (3+5)*8-(10-4)/2;
        // Expresion -> Termino MasTermino 
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (operadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == clasificaciones.operadorTermino)
            {
                string operador = getContenido();
                match(clasificaciones.operadorTermino);
                Termino();
                float e1 = s.Pop(bitacora, linea, caracter), e2 = s.Pop(bitacora, linea, caracter);                

                switch (operador)
                {
                    case "+":
                        s.Push(e2+e1, bitacora, linea, caracter);
                        break;
                    case "-":
                        s.Push(e2-e1, bitacora, linea, caracter);
                        break;      
                }

                s.Display(bitacora);
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (operadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == clasificaciones.operadorFactor)
            {
                string operador = getContenido();
                match(clasificaciones.operadorFactor);
                Factor();
                float e1 = s.Pop(bitacora, linea, caracter), e2 = s.Pop(bitacora, linea, caracter);   

                switch (operador)
                {
                    case "*":
                        s.Push(e2*e1, bitacora, linea, caracter);
                        break;
                    case "/":
                        s.Push(e2/e1, bitacora, linea, caracter);
                        break;      
                }
                s.Display(bitacora);
            }
        }
        // Factor -> identificador | numero | ( Expresion )
        private void Factor()
        {
            if (getClasificacion() == clasificaciones.identificador)
            {
                string nombre = getContenido();

                if(l.Existe(nombre))
                {
                    s.Push(float.Parse(l.getValor(getContenido())), bitacora, linea, caracter);
                    s.Display(bitacora);
                    match(clasificaciones.identificador); //Validar existencia
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable (" + nombre + ") no declarada " + "(" + linea + ", " + caracter + ")");
                }
            }
            else if (getClasificacion() == clasificaciones.numero)
            {
                s.Push(float.Parse(getContenido()), bitacora, linea, caracter);
                s.Display(bitacora);
                match(clasificaciones.numero);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }

        // For -> for (identificador = Expresion; Condicion; identificador incrementoTermino) BloqueInstrucciones
        private void For()
        {
            match("for");
            match("(");

            string nombre1 = getContenido();
            if(!l.Existe(nombre1))
            {
                match(clasificaciones.identificador); //Validar existencia
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: Variable (" + nombre1 + ") no declarada " + "(" + linea + ", " + caracter + ")");
            }

            match(clasificaciones.asignacion);
            Expresion();
            match(clasificaciones.finSentencia);

            Condicion();
            match(clasificaciones.finSentencia);

            string nombre2 = getContenido();
            if(l.Existe(nombre2))
            {
                match(clasificaciones.identificador); //Validar existencia
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: Variable (" + nombre2 + ") no declarada " + "(" + linea + ", " + caracter + ")");
            }
            match(clasificaciones.incrementoTermino);

            match(")");

            BloqueInstrucciones();
        }

        // While -> while (Condicion) BloqueInstrucciones
        private void While()
        {
            match("while");

            match("(");
            Condicion();
            match(")");

            BloqueInstrucciones();
        }

        // DoWhile -> do BloqueInstrucciones while (Condicion);
        private void DoWhile()
        {
            match("do");

            BloqueInstrucciones();

            match("while");

            match("(");
            Condicion();
            match(")");
            match(clasificaciones.finSentencia);
        }
        // x26 = (3 + 5) * 8 - (10 - 4) / 2
        // x26 = 3 + 5 * 8 - 10 - 4 / 2
        // x26 = 3 5 + 8 * 10 4 - 2 / -
    }
}