using System;
using System.IO;


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

        public void Push(float element, StreamWriter bitacora, int linea, int caracter)
        {   
            if (ultimo < maxElementos)
            {
                bitacora.WriteLine("Push = " + element);
                elementos[ultimo++] = element; 
            }
            else
            {
                throw new Error(bitacora,"Error: Stack Overflow " + "(" + linea + ", " + caracter + ")");
            }
        }

        public float Pop(StreamWriter bitacora,int linea, int caracter)
        {
            if (ultimo > 0)
            {
                bitacora.WriteLine("Pop = " + elementos[ultimo-1]);
                return elementos[--ultimo];
            }
            else
            {
                throw new Error(bitacora,"Error: Stack Underflow " + "(" + linea + ", " + caracter + ")");
            }
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