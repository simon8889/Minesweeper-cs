using System.Net.NetworkInformation;
using System.Reflection.Metadata;

namespace JuegoBuscaminas
{
    public partial class Form1 : Form
    {
        Button[,] botones = { };
        // botones
        Button facil = new Button();
        Button medio = new Button();
        Button dificil = new Button();
        Panel panel1 = new Panel();

        Label lbl_banderas = new Label();
        
        Label lbl_tiempo = new Label();
        System.Windows.Forms.Timer cronometro = new System.Windows.Forms.Timer();
        
        int tamanoBoton = 30;
        int tamanoMatriz = 10;
        TableroBuscaminas estadoTablero;

        public Form1()
        {
            InitializeComponent();
            this.Text = "ESQUIVA LOS COMETAS!";
            this.BackColor = Color.Beige;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(20 * tamanoBoton + 20, 20 * tamanoBoton + 60);
            panel1.Location = new Point(0, 20);
            this.Controls.Add(panel1);
            SeleccionarDificultad(tamanoMatriz);
            CargarBotonesInterfaz();

            cronometro.Interval = 1000;
            cronometro.Tick += new EventHandler(ActualizarTiempo);
            cronometro.Start();
        }

        public void ActualizarTiempo(object sender, EventArgs e)
        {
            estadoTablero.AumentarSegundo();
            string stringTiempo = ConvertirMilisegundosAString(estadoTablero.ObtenerTiempo());
            lbl_tiempo.Text = $"Tiempo transcurrido: {stringTiempo}";
        }
        
        public string ConvertirMilisegundosAString(int milisegundos)
        {
            int segundosTotales = milisegundos / 1000;
            int minutos = segundosTotales / 60;
            int segundos = segundosTotales % 60;
            return $"{minutos}:{segundos:D2}";
        }

        private void CargarBotonesInterfaz()
        {
            facil.Text = "Facil";
            facil.Size = new Size(100, 20);
            facil.Location = new Point(0, 0);
            facil.Click += (sender, e) => SeleccionarDificultad(10);
            this.Controls.Add(facil);

            medio.Text = "Medio";
            medio.Size = new Size(100, 20);
            medio.Location = new Point(100, 0);
            medio.Click += (sender, e) => SeleccionarDificultad(15);
            this.Controls.Add(medio);

            dificil.Text = "Dificil";
            dificil.Size = new Size(100, 20);
            dificil.Location = new Point(200, 0);
            dificil.Click += (sender, e) => SeleccionarDificultad(20);
            this.Controls.Add(dificil);
            
            lbl_tiempo.Text = $"Tiempo transcurrido: 00:00";
            lbl_tiempo.Size = new Size(150, 20);
            lbl_tiempo.Location = new Point(350, 0);
            this.Controls.Add(lbl_tiempo);

            ActualizarTextoBanderas();
            lbl_banderas.Size = new Size(200, 20);
            lbl_banderas.Location = new Point(500, 0);
            this.Controls.Add(lbl_banderas);
        }
        
        private void ActualizarTextoBanderas()
        {
            lbl_banderas.Text = $"{estadoTablero.SimboloBandera()}: {estadoTablero.ContarBanderas()} | {estadoTablero.ObtenerNumeroMinas()} {estadoTablero.SimboloMina()}";

        }

        private void SeleccionarDificultad(int num)
        {
            tamanoMatriz = num;
            panel1.Controls.Clear();
            estadoTablero = new TableroBuscaminas(tamanoMatriz);
            CargarMatriz();
            ActualizarTextoBanderas();
        }
            
        private void CargarMatriz()
        {
            tamanoBoton = this.ClientSize.Width / tamanoMatriz;
            panel1.Size = new Size(tamanoMatriz * tamanoBoton, tamanoMatriz * tamanoBoton);
            botones = new Button[tamanoMatriz, tamanoMatriz];
            Matriz();
            RefrescarContenidoDeMatriz();
        }
        
        private void RefrescarContenidoDeMatriz()
        {
            for (int i = 0; i < tamanoMatriz; i++)
            {
                for (int j = 0; j < tamanoMatriz; j++)
                {
                    bool esCasillaVisible = estadoTablero.EsCasillaVisible(i, j);
                    bool hayBandera = estadoTablero.HayBandera(i, j);
                    botones[i, j].BackColor = esCasillaVisible ? Color.LightPink : Color.LightGreen;
                    botones[i, j].Text = esCasillaVisible ? estadoTablero.ObtenerContenido(i, j) : "";
                    botones[i, j].Text = hayBandera && !esCasillaVisible  ? estadoTablero.SimboloBandera() : botones[i, j].Text;
                }
            }
        }

        private void Matriz()
        {
            for (int i = 0; i < tamanoMatriz; i++)
            {
                for (int j = 0; j < tamanoMatriz; j++)
                {
                    int i_actual = i;
                    int j_actual = j;
                    botones[i, j] = new Button();
                    botones[i, j].Size = new Size(tamanoBoton, tamanoBoton);
                    botones[i, j].Location = new Point(tamanoBoton * i, tamanoBoton * j);
                    float tamanoFuente = tamanoBoton * 0.4f;
                    botones[i, j].Font = new Font("Segoe UI", tamanoFuente, FontStyle.Regular);
                    botones[i, j].MouseDown += (sender, e) => ClickCasilla(e, i_actual, j_actual);
                    panel1.Controls.Add(botones[i, j]);
                }
            }
        }
        
        private void ClickCasilla(MouseEventArgs e, int px, int py)
        {
            if (e.Button == MouseButtons.Left) SeleccionarCasilla(px, py);
            if (e.Button == MouseButtons.Right) BanderaEnCasilla(px, py);
            ActualizarTextoBanderas();
        }
        
        private void SeleccionarCasilla(int posicion_x, int posicion_y)
        {
            estadoTablero.SeleccionarCasilla(posicion_x, posicion_y);
            RefrescarContenidoDeMatriz();
            if (estadoTablero.HaPerdido())
            {
                JuegoPerdido();
            }
        }
        
        private void BanderaEnCasilla(int px, int py)
        {
            estadoTablero.BanderaEnCasilla(px, py);
            RefrescarContenidoDeMatriz();
        }
        
        private void JuegoPerdido()
        {
            MessageBox.Show("Has Perdido");
            estadoTablero.ReiniciarTablero();
            RefrescarContenidoDeMatriz();
        }
    }
}
