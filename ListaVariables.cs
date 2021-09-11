using System;
using System.IO;
using System.Collections.Generic;

namespace AutomatasII
{
    public class ListaVariables
    {
        List<Variable>Lista;


    public ListaVariables()
    {
        Lista = new List<Variable>();
    }

    public void Inserta(string nombre, Variable.tipo tipoDato, bool esConstante = false)
    {
        Lista.Add(new Variable(nombre, tipoDato, esConstante));
    }

    public bool Existe(string nombre)
    {
        return Lista.Exists(x =>  x.getNombre() == nombre);
    }

    public void setValor(string nombre, string valor)
    {
        foreach (Variable x in Lista)
        {
            if (x.getNombre() == nombre)
            {
                x.setValor(valor);
                break;
            }
        }
    }

    public string getValor(string nombre)
    {
        foreach (Variable x in Lista)
        {
            if (x.getNombre() == nombre)
            {
                return x.getValor();
            }
        }
        return "";
    }
        public Variable.tipo getTipoDato(string nombre)
    {
        foreach (Variable x in Lista)
        {
            if (x.getNombre() == nombre)
            {
                return x.getTipoDato();
            }
        }
        return Variable.tipo.CHAR;
    }

    public void imprime(StreamWriter bitacora)
    {
        bitacora.WriteLine("Lista de variables");
        foreach (Variable x in Lista)
        {
            bitacora.WriteLine(x.getNombre()+ " " + x.getValor()+ " " + x.getTipoDato() + " " + (x.getEsConstante() ? "Constante" : "Variable"));
        }

    }
    }
}

