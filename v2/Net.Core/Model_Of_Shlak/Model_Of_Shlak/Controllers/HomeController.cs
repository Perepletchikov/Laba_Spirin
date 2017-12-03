using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model_Of_Shlak.Models;
using Microsoft.SolverFoundation;
using Microsoft.SolverFoundation.Services;
using System.Xml;

namespace Model_Of_Shlak.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            RashetShlak cls = new RashetShlak();

            ViewBag.rashet = cls;



            return View();
        }

        public IActionResult Result(double TempVhod, double FeOVhod, double TempExit, double Osnovnoct, double FeOExit, int dsfsdfdsfdsf)
        {
            ViewData["Message"] = "Your application description page.";

            RashetShlak cls = new RashetShlak(TempVhod, FeOVhod, TempExit, Osnovnoct, FeOExit);

            ViewBag.rashet = cls;            

            // Расчеты

            #region расчеты СЛАУ
            string[] str = new string[4];
            int p = 0;
            XmlReader reader = XmlReader.Create(@"C:\Users\1\Desktop\v2\Net.Core\Model_Of_Shlak\Model_Of_Shlak\bin\Debug\netcoreapp2.0\phones_.xml");
            XmlDocument doc = new XmlDocument();
            
            XmlReaderSettings settings = new XmlReaderSettings();
            //settings.
            //doc = new XmlReader();
            {
                //doc.Load("phones_.xml");
                doc.Load(reader);
                foreach (XmlNode node in doc.SelectNodes("phones"))
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        str[p] = (string.Format(child.InnerText)).ToString();
                        p++;
                    }
                }
            }
            Char delimiter = '/';
            double[] x = new double[6];

            double[,] y = new double[3, 6];
            for (int u = 0; u < 4; u++)
            {
                String[] subn = str[u].Split(delimiter);
                if (u == 0)
                    for (int j = 0; j < 6; j++)
                        x[j] = double.Parse(subn[j]);
                else
                    for (int j = 0; j < 6; j++)
                        y[u - 1, j] = double.Parse(subn[j]);
            }

            double[,] results = new double[3, 6];

            SolverContext context = SolverContext.GetContext();

            for (int i = 0; i < y.GetLength(0); i++)
            {
                Decision a = new Decision(Domain.Real, "a");
                Decision b = new Decision(Domain.Real, "b");
                Decision c = new Decision(Domain.Real, "c");
                Decision d = new Decision(Domain.Real, "d");
                Decision e = new Decision(Domain.Real, "e");
                Decision f = new Decision(Domain.Real, "f");

                Model model = context.CreateModel();

                model.AddDecisions(a, b, c, d, e, f);

                model.AddConstraint("eqA1", y[i, 0] == ((Math.Pow(x[0], 1)) * a + (Math.Pow(x[0], 2)) * b + (Math.Pow(x[0], 3)) * c + (Math.Pow(x[0], 4)) * d + (Math.Pow(x[0], 5)) * e + f));
                model.AddConstraint("eqA2", y[i, 1] == ((Math.Pow(x[1], 1)) * a + (Math.Pow(x[1], 2)) * b + (Math.Pow(x[1], 3)) * c + (Math.Pow(x[1], 4)) * d + (Math.Pow(x[1], 5)) * e + f));
                model.AddConstraint("eqA3", y[i, 2] == ((Math.Pow(x[2], 1)) * a + (Math.Pow(x[2], 2)) * b + (Math.Pow(x[2], 3)) * c + (Math.Pow(x[2], 4)) * d + (Math.Pow(x[2], 5)) * e + f));
                model.AddConstraint("eqA4", y[i, 3] == ((Math.Pow(x[3], 1)) * a + (Math.Pow(x[3], 2)) * b + (Math.Pow(x[3], 3)) * c + (Math.Pow(x[3], 4)) * d + (Math.Pow(x[3], 5)) * e + f));
                model.AddConstraint("eqA5", y[i, 4] == ((Math.Pow(x[4], 1)) * a + (Math.Pow(x[4], 2)) * b + (Math.Pow(x[4], 3)) * c + (Math.Pow(x[4], 4)) * d + (Math.Pow(x[4], 5)) * e + f));
                model.AddConstraint("eqA6", y[i, 5] == ((Math.Pow(x[5], 1)) * a + (Math.Pow(x[5], 2)) * b + (Math.Pow(x[5], 3)) * c + (Math.Pow(x[5], 4)) * d + (Math.Pow(x[5], 5)) * e + f));

                Solution solution = context.Solve();
                string result = solution.GetReport().ToString();

                results[i, 0] = a.ToDouble();
                results[i, 1] = b.ToDouble();
                results[i, 2] = c.ToDouble();
                results[i, 3] = d.ToDouble();
                results[i, 4] = e.ToDouble();
                results[i, 5] = f.ToDouble();

                context.ClearModel();
            }

            double[] temps = { cls.TempVhod, cls.TempExit };
            double[,] koefs = new double[2, 3];

            for (int i = 0; i < temps.GetLength(0); i++)
            {
                Decision a = new Decision(Domain.Real, "a");
                Decision b = new Decision(Domain.Real, "b");
                Decision c = new Decision(Domain.Real, "c");

                Model model = context.CreateModel();

                model.AddDecisions(a, b, c);

                double ta = results[0, 5] +
                    results[0, 0] * Math.Pow(temps[i], 1) +
                    results[0, 1] * Math.Pow(temps[i], 2) +
                    results[0, 2] * Math.Pow(temps[i], 3) +
                    results[0, 3] * Math.Pow(temps[i], 4) +
                    results[0, 4] * Math.Pow(temps[i], 5);
                double tb = results[1, 5] +
                    results[1, 0] * Math.Pow(temps[i], 1) +
                    results[1, 1] * Math.Pow(temps[i], 2) +
                    results[1, 2] * Math.Pow(temps[i], 3) +
                    results[1, 3] * Math.Pow(temps[i], 4) +
                    results[1, 4] * Math.Pow(temps[i], 5);
                double tc = results[2, 5] +
                    results[2, 0] * Math.Pow(temps[i], 1) +
                    results[2, 1] * Math.Pow(temps[i], 2) +
                    results[2, 2] * Math.Pow(temps[i], 3) +
                    results[2, 3] * Math.Pow(temps[i], 4) +
                    results[2, 4] * Math.Pow(temps[i], 5);

                //MessageBox.Show(ta.ToString() + "..." + tb.ToString() + "..." + tc.ToString());

                model.AddConstraint("eqA1", ta == 0 * a + 0 * b + c);
                model.AddConstraint("eqA2", tb == 6.0 * a + 36.0 * b + c);
                model.AddConstraint("eqA3", tc == 12.0 * a + 144.0 * b + c);

                Solution solution = context.Solve();
                // string result = solution.GetReport().ToString();

                koefs[i, 0] = a.ToDouble();
                koefs[i, 1] = b.ToDouble();
                koefs[i, 2] = c.ToDouble();

                context.ClearModel();
            }
            #endregion 

            cls.VisVhod = koefs[0, 2] + koefs[0, 0] * Math.Pow(cls.FeOVhod, 1) + koefs[0, 1] * cls.FeOVhod * cls.FeOVhod;
            cls.VisExit = koefs[1, 2] + koefs[1, 0] * Math.Pow(cls.FeOExit, 1) + koefs[1, 1] * cls.FeOExit * cls.FeOExit;
            cls.VisOcnVhod = cls.VisVhod + 196*(cls.Osnovnoct -1);
            cls.VisOsnExit = cls.VisExit + 4 * (cls.Osnovnoct - 1);
            cls.KoffB = (Math.Log10(Math.Log10(cls.VisOsnExit)) - Math.Log10(Math.Log10(cls.VisOcnVhod))) / (cls.TempExit - cls.TempVhod);
            cls.KoffA = Math.Log10(Math.Log10(cls.VisOcnVhod)) - cls.TempVhod * cls.KoffB;
            cls.Vis1180 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * cls.TempVhod));
            cls.Vis1200 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * 1200));
            cls.Vis1250 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * 1250));
            cls.Vis1275 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * 1275));
            cls.Vis1300 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * 1300));
            cls.TempKristal = (cls.KoffA - Math.Log10(Math.Log10(25))) / -cls.KoffB;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
