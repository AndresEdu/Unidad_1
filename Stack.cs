using System;
using System.IO;
using overflow;


namespace AutomatasII
{
    public class Stack
    {
        int maxElementos;
        int ultimo;
        float[] elementos;

        public Stack(int maxElementos)
        {
            this.maxElementos = maxElementos;
            ultimo = 0;
            elementos = new float[maxElementos];
        }

        public void Push(float element, StreamWriter bitacora)
        {
            if (ultimo < maxElementos)
            {
                bitacora.WriteLine("Push = " + element);
                elementos[ultimo++] = element; 
            }
            // else levantar excepción de stack overflow
            else
            {
                throw new StackOverflowException("");
            }
        }

        public float Pop(StreamWriter bitacora)
        {
            if (ultimo > 0)
            {
                bitacora.WriteLine("Pop = " + elementos[ultimo-1]);
                return elementos[--ultimo];
            }
            // else levantar excepción de stack underflow
            else
            {
                throw new System.underflowException();
            }

            return 0;
        }

        public void Display(StreamWriter bitacora)
        {
            bitacora.WriteLine("Contenido del stack: ");
            
            for (int i = 0; i < ultimo; i++)
            {
                bitacora.Write(elementos[i] + " ");
            }

            bitacora.WriteLine("");
        }
    }
}