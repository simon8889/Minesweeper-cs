using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace JuegoBuscaminas
{
    internal class TableroBuscaminas
    {
        const string MINA = "☄️";
        const string VACIO = "";
        const string BANDERA = "🚀";
        int tamano { get; set; }
        string[,] contenido;
        bool[,] visibilidad;
        bool[,] banderas;
        int tiempo = 0;
        bool haPerdido = false;
        
        public TableroBuscaminas(int tamano)
        {
            this.tamano = tamano;
            CargarEstadoInicialTablero();
        }
        
        public void AumentarSegundo()
        {
            tiempo += 1000;
        }
       
        public int ObtenerTiempo()
        {
            return tiempo;
        }
                
        public bool EsCasillaVisible(int px, int py)
        {
            return visibilidad[px, py];
        }
        
        public string ObtenerContenido(int px, int py)
        {
            return contenido[px, py];
        }
        
        public bool HayBandera(int px, int py)
        {
            return banderas[px, py];
        }
        
        public bool HaPerdido()
        {
            return haPerdido;
        }
        
        public string SimboloBandera()
        {
            return BANDERA;
        }
        public string SimboloMina()
        {
            return MINA;
        }
        
        public int ContarBanderas()
        {
            int contador = 0;
            for (int i = 0; i < tamano; i++)
            {
                for (int j = 0; j < tamano; j++)
                {
                    if (banderas[i, j]) contador++;
                }
            }
            return contador;
        }

        private void CargarVisibilidadYBanderas()
        {
            for (int i = 0; i < tamano; i++)
            {
                for (int j = 0; j < tamano; j++)
                {
                    visibilidad[i, j] = false;
                    banderas[i, j] = false;
                }
            }
        }
        
        public int ObtenerNumeroMinas()
        {
            int numMinas = tamano * 2;
            if (tamano == 20) numMinas += 10;
            return numMinas;
        }
        
        private void CargarMinas()
        {
            List<int[]> posicionesMinas = new List<int[]>();
            while(posicionesMinas.Count < ObtenerNumeroMinas())
            {
                int[] posicion = [new Random().Next(tamano - 1), new Random().Next(tamano - 1)];
                if (posicionesMinas.Contains(posicion)) continue;
                posicionesMinas.Add(posicion);
                contenido[posicion[0], posicion[1]] = MINA;
            }
        }
        
        private void CargarContenido()
        {
            for (int i = 0; i < tamano; i++)
            {
                for (int j = 0; j < tamano; j++)
                {
                    if (contenido[i, j] == MINA) continue;
                    int cantidadMinas = ContarMinasAdyacentes(i, j);
                    contenido[i, j] = cantidadMinas.ToString();
                    if (cantidadMinas == 0) contenido[i, j] = VACIO;
                }
            }
        }

        private bool ValidarIndiceFueraDeRango(int[] posicion)
        {
            bool indiceFueraDeRango = !(0 <= posicion[0] && posicion[0] < tamano && 0 <= posicion[1] && posicion[1] < tamano);
            return indiceFueraDeRango;
        }
        private int ContarMinasAdyacentes(int px, int py)
        {
            int cantidadMinas = 0;
            List<int[]> posicionesAdyacentes = new List<int[]>([[px - 1, py - 1], [px - 1, py + 1], [px + 1, py - 1], [px + 1, py + 1],
                                                                [px + 1, py], [px - 1, py], [px, py + 1], [px, py - 1]]);
            foreach (int[] posicion in posicionesAdyacentes)
            {
                if (ValidarIndiceFueraDeRango(posicion)) continue;
                if (contenido[posicion[0], posicion[1]] == MINA) cantidadMinas++;
            }
            return cantidadMinas;
        }
        
        public void SeleccionarCasilla(int px, int py)
        {
            bool visibilidadCelda = visibilidad[px, py];
            if (visibilidadCelda) return;
            string contenidoCelda = contenido[px, py];
            if (contenidoCelda == VACIO)
            {
                LlenarArea(px, py, []);
            }
            
            if (contenidoCelda != VACIO && contenidoCelda != MINA)
            {
                visibilidad[px, py] = true;
            }
            
            if (contenidoCelda == MINA)
            {
                haPerdido = true;
                HacerVisibleElTablero();
            }

            banderas[px, py] = false;
        }
         
        public void BanderaEnCasilla(int px, int py)
        {
            if (!visibilidad[px, py]) banderas[px, py] = !banderas[px, py];
        }
        
        void HacerVisibleElTablero()
        {
            for (int i = 0; i < tamano; i++)
            {
                for (int j = 0; j < tamano; j++)
                {
                    visibilidad[i, j] = true;
                }
            }
        }
        
        private void CargarEstadoInicialTablero()
        {
            visibilidad = new bool[tamano, tamano];
            banderas = new bool[tamano, tamano];
            contenido = new string[tamano, tamano];
            CargarVisibilidadYBanderas();
            CargarMinas();
            CargarContenido();
        }
        
        public void ReiniciarTablero()
        {
            haPerdido = false;
            tiempo = 0;
            CargarEstadoInicialTablero();
        }

        private void LlenarArea(int px, int py, List<int[]> memo)
        {
            if (ValidarIndiceFueraDeRango([px, py])) return;
            if (memo.Contains([px, py])) return;
            string contenidoCelda = contenido[px, py];
            bool visibilidadCelda = visibilidad[px, py];
            if (contenidoCelda == VACIO && !visibilidadCelda)
            {
                memo.Add([px, py]);
                visibilidad[px, py] = true;
                LlenarArea(px + 1, py, memo);
                LlenarArea(px, py + 1, memo);
                LlenarArea(px - 1, py, memo);
                LlenarArea(px, py - 1, memo);
                LlenarArea(px - 1, py - 1, memo);
                LlenarArea(px - 1, py + 1, memo);
                LlenarArea(px + 1, py - 1, memo);
                LlenarArea(px + 1, py + 1, memo);
            }
            
            if(contenidoCelda != VACIO && contenidoCelda != MINA)
            {
                visibilidad[px, py] = true;
            }
        }
    }
}
