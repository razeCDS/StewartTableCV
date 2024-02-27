using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StewartTableCV
{
    public partial class StewartTableCV : Form
    {
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        VideoCapture capture;
        
        public StewartTableCV()
        {
            InitializeComponent();
        }

        private void IniciarWebCamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(capture != null)
            {
                MessageBox.Show("A webcam já está iniciada!");
                return;
            }

            capture = new VideoCapture(0);
            capture.ImageGrabbed += Capture_ImageGrabbed;
            capture.Start();
        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();
                capture.Retrieve(m);
                ProcessarImagem(m.Bitmap);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PararWebCamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capture == null)
            {
                MessageBox.Show("A webcam já está parada!");
                return;
            }

            capture.ImageGrabbed -= Capture_ImageGrabbed;
            capture.Stop();
            capture.Dispose();
            capture = null;
            picImagem.Image = null;
        }

        private void ProcessarImagem(Bitmap btm)
        {
            Bitmap bitmap = (Bitmap)btm.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
            foreach (var rectangle in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Blue, 4))
                    {
                        graphics.DrawRectangle(pen, rectangle);
                        var testeCoordenada = rectangle.Location;
                        Console.WriteLine($"Coordenada X:{testeCoordenada.X}, Coordenada Y:{testeCoordenada.Y}, {DateTime.Now.ToString("hh: mm:ss.fff tt")}");
                    }

                }
            }
            picImagem.Image = bitmap;
        }
    }
}
