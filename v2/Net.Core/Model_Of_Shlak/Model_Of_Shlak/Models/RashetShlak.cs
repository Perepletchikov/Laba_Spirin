using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model_Of_Shlak.Models
{
    public class RashetShlak
    {
        public double TempVhod { get; set; } = 1180;
        public double FeOVhod { get; set; } = 9;
        public double TempExit { get; set; } = 1300;
        public double Osnovnoct { get; set; } = 1.2;
        public double FeOExit { get; set; } = 4;


        public double VisVhod { get; set; }
        public double VisExit { get; set; }
        public double VisOcnVhod { get; set; }
        public double VisOsnExit { get; set; }
        public double KoffA { get; set; }
        public double KoffB { get; set; }
        public double Vis1180 { get; set; }
        public double Vis1200 { get; set; }
        public double Vis1250 { get; set; }
        public double Vis1275 { get; set; }
        public double Vis1300 { get; set; }
        public double TempKristal { get; set; }

        public RashetShlak()
        {
        }

        public RashetShlak(double TempVhod, double FeOVhod, double TempExit, double Osnovnoct, double FeOExit)
        {
            this.TempVhod = TempVhod;
            this.FeOVhod = FeOVhod;
            this.TempExit = TempExit;
            this.Osnovnoct = Osnovnoct;
            this.FeOExit = FeOExit;

            // Osnovnost1 =ghj
        }

    }
}
