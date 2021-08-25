using System;
using System.Collections.Generic;
using System.Text;
/*
// *Requerimiento 1:Asignar una expresion matematica a la hora de declarare una variable.
// *Requerimiento 2: En la condición debe de ir Expresion operadorRelacional Expresion.
// *Requerimiento 3: Implementar el For
// *Requerimiento 4: Implementar el while
// *Requerimiento 5: Implementar el Do-While
*/
namespace AutomatasII
{
    class Lenguaje : Sintaxis
    {
        public Lenguaje()
        {
            Console.WriteLine("Iniciando analisis gramatical");
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            Console.WriteLine("Iniciando analisis gramatical");
        }

        //Programa --> Libreria Main
        public void Program()
        {
            Libreria();
            Main();
        }

        //Libreria --> ¿ #include < identificador (.h)? > Libreria() ?
        private void Libreria()
        {
            if (GetContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Token.Clasificaciones.identificador);

                if (GetContenido() == ".")
                {
                    match(".");
                    match("h");
                }

                match(">");

                Libreria();
            }
        }

        // Main -> void main(){ (Variables)? Instrucciones }
        private void Main()
        {
            match(Clasificaciones.TipoDato);
            match("main");
            match("(");
            match(")");

            BloquedeInstrucciones();
        }

        // Bloque Instrucciones -> {intrucciones]
        private void BloquedeInstrucciones() 
        {
            match(Clasificaciones.IniciodeBloque);

            Instrucciones();

            match(Clasificaciones.FindeBloque);
        }

        //Lista_IDs -> identificador (,Lista_IDs)?
        private void Lista_IDs()
        {
            match(Clasificaciones.identificador);

            if (GetClasificacion() == Clasificaciones.asignacion)
            {
                match(Clasificaciones.asignacion);
                Expresion();
            }

            if (GetContenido() == ",")
            {
                match(",");
                Lista_IDs();
            }
        }

        //Variables -> tipoDato List_IDs; 
        private void Variables()
        {
            match(Clasificaciones.TipoDato);
            Lista_IDs();
            match(Clasificaciones.FinSentencia);

        }

        //Instruccion -> (inicializacion | printf(identificador | cadena | numero)) ;

        private void Instruccion()
        {
            if (GetContenido() == "if")
            {
                IF();
            }
            else if (GetContenido() == "while")
            {
                WHILE();
            }
            else if (GetContenido() == "for")
            {
                FOR();
            }
            else if (GetContenido() == "do")
            {
                DOWHILE();
            }
            else if (GetContenido() == "cin")
            {
                match("cin");
                match(Clasificaciones.FlujoEntrada);
                match(Clasificaciones.identificador);
                match(Clasificaciones.FinSentencia);
            }
            else if (GetContenido() == "cout")
            {
                match("cout");
                ListaFlujoSalida();
                match(Clasificaciones.FinSentencia);
            }
            else if (GetContenido() == "const")
            {
                Constante();
            }

            else if (GetClasificacion() == Clasificaciones.TipoDato) 
            {
                Variables();
            }
            else
            {
                match(Clasificaciones.identificador);
                match(Clasificaciones.asignacion);

                
                if (GetClasificacion() == Clasificaciones.Cadena)
                {
                    match(Clasificaciones.Cadena);
                }
                else
                {
                    Expresion();
                }
                match(Clasificaciones.FinSentencia); 
            }
        }

        //Instrucciones -> Instruccion Instrucciones?
        private void Instrucciones()
        {
            Instruccion();

            if (GetClasificacion() != Clasificaciones.FindeBloque)
            {
                Instrucciones();
            }
        }

        //Constante -> const TipoDato identificador = numero | cadena;
        private void Constante()
        {
            match("const");
            match(Clasificaciones.TipoDato);
            match(Clasificaciones.identificador);
            match(Clasificaciones.asignacion);

            if (GetClasificacion() == Clasificaciones.numero)
            {
                match(Clasificaciones.numero);
            }
            else
            {
                match(Clasificaciones.Cadena);
            }

            match(Clasificaciones.FinSentencia);

        }

        //ListaFlujoSalida -> << cadena | identificador | numero (ListaFlujoSalida)?
        private void ListaFlujoSalida()
        {
            match(Clasificaciones.FlujoSalida);
            if (GetClasificacion() == Clasificaciones.numero)
            {
                match(Clasificaciones.numero);
            }
            else if (GetClasificacion() == Clasificaciones.Cadena)
            {
                match(Clasificaciones.Cadena);
            }
            else
            {
                match(Clasificaciones.identificador);
            }
            
            if (GetClasificacion() == Clasificaciones.FlujoSalida)
            {
                ListaFlujoSalida();
            }
            
        }

        //Condicion -> identificador opoeraddorrelacional identificador
        private void Condicion() 
        {
            Expresion();
            match(Clasificaciones.OperadorRelacional);
            Expresion();
        }

        //Expresion -> Termino MasTermino
        //x26 = (3+5)*8-(10-4)/2;
        private void Expresion()
        {
            Termino();
            MasTermino();
        }

        //MasTermino -> (operadorTermino Termino)?
        private void MasTermino()
        {
            if (GetClasificacion() == Clasificaciones.OperadorTermino)
            {
                match(Clasificaciones.OperadorTermino);
                Termino();
            }
        }

        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }

        //PorFactor -> (operadorFactor Factor)?
        private void PorFactor()
        {
            if (GetClasificacion() == Clasificaciones.OperadorFactor)
            {
                match(Clasificaciones.OperadorFactor);
                Factor();
            }
        }

        //Factor -> identificard | numero | ( Expresion )
        private void Factor()
        {
            if (GetClasificacion() == Clasificaciones.identificador)
            {
                match(Clasificaciones.identificador);
            }
            else if (GetClasificacion() == Clasificaciones.numero)
            {
                match(Clasificaciones.numero);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }


        //if -> ( coindicion ) bloquedeInstrucciones (else bloquedeInstrucciones)?
        private void IF()
        {
            match("if");
            match("(");
            Condicion();
            match(")");

            BloquedeInstrucciones();

            if (GetContenido() == "else")
            {
                match("else");
                BloquedeInstrucciones();
            }
        }
        //For -> for(identificador = Expresion; Condicion; identificador incrementoTermino) BloqueInstrucciones
        private void FOR()
        {
            match("for");
            match("(");
            match(Clasificaciones.identificador);
            match(Clasificaciones.asignacion);
            Expresion();
            match(Clasificaciones.FinSentencia);
            Condicion();
            match(Clasificaciones.FinSentencia);
            match(Clasificaciones.identificador);
            match(Clasificaciones.IncrementoTermino);
            match(")");
            BloquedeInstrucciones();
        }

        //While -> while (Condicion) BloqueInstrucciones
        private void WHILE()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            BloquedeInstrucciones();
        }

        //DoWhile -> do BloqueInstrucciones while(Condicion);
        private void DOWHILE()
        {
            match("do");
            BloquedeInstrucciones();
            match("while");
            match("(");
            Condicion();
            match(")");
            match(Clasificaciones.FinSentencia);

        }
    }
}