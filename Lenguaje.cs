using System;
using System.Collections.Generic;
using System.Text;


// :))) Requerimiento 1: Implementar el not en el if.
// Requerimiento 2: Validar la asignacion de strings en instrucción.
// Requerimiento 3: Implementar la comparacion de tiposdedatos en ListaIDs.
// Requerimiento 4: Validar los tipos de datos en la asignacion de cin.
// Requerimiento 5: Implementar el cast.

namespace AutomatasII
{
    class Lenguaje : Sintaxis
    {
        Stack s;
        ListaVariables l;
        Variable.tipo maxBytes;
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

            BloqueInstrucciones(true);
        }

        // BloqueInstrucciones -> { Instrucciones }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match(clasificaciones.inicioBloque);
            Instrucciones(ejecuta);
            match(clasificaciones.finBloque);
        }

        // Lista_IDs -> identificador (= Expresion)? (,Lista_IDs)? 
        private void Lista_IDs(Variable.tipo Tipo, bool ejecuta)
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
                
                if(getClasificacion() == clasificaciones.cadena)
                {
                    if (Tipo == Variable.tipo.STRING)
                    {
                        match(clasificaciones.cadena);
                        if (ejecuta)
                        {
                            l.setValor(nombre,getContenido());
                        }
                    }
                    else
                    {
                        throw new Error(bitacora, "Error de semantico: No se puede Asignar un STRING a un  (" + Tipo + "). " + " (" + linea + ", " + caracter + ")");
                    }
                }
                else
                {
                    //Requerimiento 3
                    maxBytes = Variable.tipo.CHAR;
                    Expresion();
                    if (ejecuta)
                    {
                        l.setValor(nombre,s.Pop(bitacora,linea,caracter).ToString());   
                    }
                }
            }

            if (getContenido() == ",")
            {
                match(",");
                Lista_IDs(Tipo, ejecuta);
            }
        }

        // Variables -> tipoDato Lista_IDs; 
        private void Variables(bool ejecuta)
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

            Lista_IDs(TipodeVariable, ejecuta);
            match(clasificaciones.finSentencia);
        }

        // Instruccion -> (If | cin | cout | const | Variables | asignacion) ;
        private void Instruccion(bool ejecuta)
        {
            if (getContenido() == "do")
            {
                DoWhile(ejecuta);
            }
            else if (getContenido() == "while")
            {
                While(ejecuta);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta);
            }
            else if (getContenido() == "if")
            {
                If(ejecuta);
            }
            else if (getContenido() == "cin")
            {
                //Requerimiento 4
                match("cin");
                match(clasificaciones.flujoEntrada);

                string nombre = getContenido(); 
                if(l.Existe(nombre))
                {
                    if (ejecuta)
                    {
                        match(clasificaciones.identificador); //Validar existencia
                        string entrada = Console.ReadLine();
                        l.setValor(nombre,entrada); 
                    }
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
                ListaFlujoSalida(ejecuta);
                match(clasificaciones.finSentencia);
            }
            else if (getContenido() == "const")
            {
                Constante(ejecuta);
            }
            else if (getClasificacion() == clasificaciones.tipoDato)
            {
                Variables(ejecuta);
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

                //requerimiento 2
                string valor;
                
                if (getClasificacion() == clasificaciones.cadena)
                {
                    valor = getContenido();

                    if (l.getTipoDato(nombre) == Variable.tipo.STRING)
                    {
                        match(clasificaciones.cadena);
                        if (ejecuta)
                        {
                            l.setValor(nombre,getContenido());
                        }
                    }
                    else
                    {
                        throw new Error(bitacora, "Error de semantico: No se puede Asignar un STRING a un (" + l.getTipoDato(nombre) + "). " + " (" + linea + ", " + caracter + ")");
                    }
                }
                else
                {
                    //requerimiento 3
                    maxBytes = Variable.tipo.CHAR;
                    Expresion();
                    valor = s.Pop(bitacora,linea,caracter).ToString();

                    if (TipodatoExpresion(float.Parse(valor)) > maxBytes)
                    {
                        maxBytes = TipodatoExpresion(float.Parse(valor));
                    }

                    if (maxBytes > l.getTipoDato(nombre))
                    {
                        throw new Error(bitacora, "Error de semantico: No se puede Asignar un ("+maxBytes+") a un (" + l.getTipoDato(nombre)+ "). " + " (" + linea + ", " + caracter + ")");    
                    }
                }

                if (ejecuta)
                {
                    l.setValor(nombre, valor);
                    match(clasificaciones.finSentencia);
                }
            }
        }

        // Instrucciones -> Instruccion Instrucciones?
        private void Instrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);

            if (getClasificacion() != clasificaciones.finBloque)
            {
                Instrucciones(ejecuta);
            }
        }

        // Constante -> const tipoDato identificador = numero | cadena;
        private void Constante(bool ejecuta)
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
            if(!l.Existe(nombre) && ejecuta)
            {
                match(clasificaciones.identificador); //Validar duplicidad
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: La constante (" + nombre + ") esta duplicada" + "(" + linea + ", " + caracter + ")");
            }
            
            l.Inserta(nombre,TipodeVariable,true);
            match(clasificaciones.asignacion);  

                
            if (getClasificacion() == clasificaciones.numero)
            {
                if (ejecuta)
                {
                    l.setValor(nombre,getContenido());  //Se le hace un Set al valor en la lista
                }
                match(clasificaciones.numero);
            }
            else
            {
                if (ejecuta)
                {
                    l.setValor(nombre,getContenido());  //Se le hace un Set al valor en la lista
                }
                match(clasificaciones.cadena);
            }

            match(clasificaciones.finSentencia);
        }

        // ListaFlujoSalida -> << cadena | identificador | numero (ListaFlujoSalida)?
        private void ListaFlujoSalida(bool ejecuta)
        {
            match(clasificaciones.flujoSalida);

            if (getClasificacion() == clasificaciones.numero)
            {
                if (ejecuta)
                {
                    Console.Write(getContenido());
                    match(clasificaciones.numero);  
                }
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

                if (ejecuta)
                {
                    Console.Write(secuencias);
                }

                match(clasificaciones.cadena);
            }
            else
            {
                string nombre = getContenido();

                if(l.Existe(nombre))
                {
                    if (ejecuta)
                    {
                        Console.Write(l.getValor(nombre));
                    }
                    match(clasificaciones.identificador); //Validar existencia
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable (" + nombre + ") no declarada " + "(" + linea + ", " + caracter + ")");
                }
            }

            if (getClasificacion() == clasificaciones.flujoSalida)
            {
                ListaFlujoSalida(ejecuta);
            }
        }

        // If -> if (Condicion) { BloqueInstrucciones } (else BloqueInstrucciones)?
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");
            bool ejecuta;

            if (getContenido() == "!")
            {
                match(clasificaciones.operadorLogico);
                match("(");
                ejecuta = !Condicion();
                match(")");
            }
            else
            {
                ejecuta = Condicion();
            }
        
            match(")");
            BloqueInstrucciones(ejecuta && ejecuta2);
            //console.writeLine(ejecuta + " "+ ejecuta2);
            if (getContenido() == "else")
            {
                match("else");
                BloqueInstrucciones(!ejecuta && ejecuta2);
            }
        }

        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            maxBytes = Variable.tipo.CHAR;
            Expresion();
            float n1 = s.Pop(bitacora,linea,caracter);
            string operador = getContenido(); 
            match(clasificaciones.operadorRelacional);
            maxBytes = Variable.tipo.CHAR;
            Expresion();
            float n2 = s.Pop(bitacora,linea,caracter);

            switch (operador)
            {
                case ">":
                    return n1 > n2;

                case ">=":
                    return n1 >= n2;

                case "<":
                    return n1 < n2;

                case "<=":
                    return n1 <= n2;

                case "==":
                    return n1 == n2;
                     
                default:
                    return n1 != n2;              
            } 
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

                    if (l.getTipoDato(nombre) > maxBytes)
                    {
                        maxBytes = l.getTipoDato(nombre);
                    }
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

                if (TipodatoExpresion(float.Parse(getContenido())) > maxBytes)
                {
                    maxBytes = TipodatoExpresion(float.Parse(getContenido()));
                }

                match(clasificaciones.numero);
            }
            else
            {
                match("(");

                Variable.tipo TipoDato = Variable.tipo.CHAR;
                bool HuboCast = false;

                if (getClasificacion() == clasificaciones.tipoDato)
                {
                    HuboCast = true;
                    TipoDato = determinarTipoDato(getContenido());
                    match(clasificaciones.tipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");

                if (HuboCast) 
                {
                    //Hacer un pop y convertir ese numero al tipo dato y meterlo al stack.
                    float n1 = s.Pop(bitacora,linea,caracter);
                    //Para convertir un int a char se divide /256 y el residuo es el resultado del cast 256 = 0, 257 = 1,...  
                    //Para convertir un float a int se divide /65536 y el residuo es el resultado del cast 
                    //Para convertir un float a otro redondear el numero para eliminar la parte fraccional 
                    //Para convertir un float a char se divide /65535 /256  y el residuo es el resultado del cast 256 = 0, 257 = 1,...
                    //Para convertir a float n1 = n1 
                    //n1 = cast(n1, TipoDato);
                    s.Push(n1,bitacora,linea,caracter);
                    maxBytes = TipoDato;
                }
            }
        }

        // For -> for (identificador = Expresion; Condicion; identificador incrementoTermino) BloqueInstrucciones
        private void For(bool ejecuta)
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

            BloqueInstrucciones(ejecuta);
        }

        // While -> while (Condicion) BloqueInstrucciones
        private void While(bool ejecuta)
        {
            match("while");

            match("(");
            Condicion();
            match(")");

            BloqueInstrucciones(ejecuta);
        }

        // DoWhile -> do BloqueInstrucciones while (Condicion);
        private void DoWhile(bool ejecuta)
        {
            match("do");

            BloqueInstrucciones(ejecuta);

            match("while");

            match("(");
            Condicion();
            match(")");
            match(clasificaciones.finSentencia);
        }

        private Variable.tipo TipodatoExpresion (float valor)
        {
            if (valor % 1 != 0)
            {
                return Variable.tipo.FLOAT;
            }
            else if (valor < 256)
            {
                return Variable.tipo.CHAR;
            }
            else if (valor < 65535)
            {
                return Variable.tipo.INT;   
            }
            return Variable.tipo.FLOAT;   
        }
        private Variable.tipo determinarTipoDato(string tipoDato)
        {
            Variable.tipo tipoVar;

            switch(tipoDato)
            {
                case "int":
                    tipoVar = Variable.tipo.INT;
                    break;
                
                case "float":
                    tipoVar = Variable.tipo.FLOAT;
                    break;

                case "string":
                    tipoVar = Variable.tipo.STRING;
                    break;

                default:
                    tipoVar = Variable.tipo.CHAR;  
                    break;                  
            }

            return tipoVar;
        }
    }
}