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
//using Excel = Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace Model_Of_Shlak.Controllers
{
    public class HomeController : Controller
    {

        Microsoft.Office.Interop.Excel.Application application;
        Microsoft.Office.Interop.Excel.Workbook workBook;
        Microsoft.Office.Interop.Excel.Worksheet worksheet;
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
            XmlReader reader = XmlReader.Create(@"phones_.xml");
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

                Microsoft.SolverFoundation.Services.Model model = context.CreateModel();

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

            for (int i = 0; i < 2; i++)
            {
                Decision a = new Decision(Domain.Real, "a");
                Decision b = new Decision(Domain.Real, "b");
                Decision c = new Decision(Domain.Real, "c");

                Microsoft.SolverFoundation.Services.Model model = context.CreateModel();

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

            cls.VisVhod = koefs[0, 2] + koefs[0, 0] * cls.FeOVhod + koefs[0, 1] * cls.FeOVhod * cls.FeOVhod;
            cls.VisExit = koefs[1, 2] + koefs[1, 0] * cls.FeOExit + koefs[1, 1] * cls.FeOExit * cls.FeOExit;
            cls.VisOcnVhod = cls.VisVhod + 196*(cls.Osnovnoct -1);
            cls.VisOsnExit = cls.VisExit + 4 * (cls.Osnovnoct - 1);
            cls.KoffB = (Math.Log10(Math.Log10(cls.VisOsnExit)) - Math.Log10(Math.Log10(cls.VisOcnVhod))) / (cls.TempExit - cls.TempVhod);
            cls.KoffA = Math.Log10(Math.Log10(cls.VisOcnVhod)) - cls.TempVhod * cls.KoffB;
            cls.Vis1180 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * cls.TempVhod));
            cls.Vis1200 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * 1200));
            cls.Vis1250 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * 1250));
            cls.Vis1275 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * 1275));
            cls.Vis1300 = Math.Pow(10, Math.Pow(10, cls.KoffA + cls.KoffB * 1300));
            cls.TempKristal = (cls.KoffA - Math.Log10(Math.Log10(25))) / ((-1)*cls.KoffB);


            #region Excel  
            
            // Открываем приложение
            application = new Application
            {
                DisplayAlerts = false
            };

            // Файл шаблона
            const string template = "shlak2.xlsx";

            // Открываем книгу
            workBook = application.Workbooks.Open(Path.Combine(Environment.CurrentDirectory, template));

            // Получаем активную таблицу
            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets[1];//.get_Item(2);            
            // Записываем данные
            worksheet.Range["C22"].Value = cls.TempVhod;
            worksheet.Range["C23"].Value = cls.FeOVhod;
            worksheet.Range["C24"].Value = cls.TempExit;
            worksheet.Range["C25"].Value = cls.FeOExit;
            worksheet.Range["C26"].Value = cls.Osnovnoct;
            //ВВОД данных и р-т
            double VisVhodEX = double.Parse(worksheet.Range["C27"].Value.ToString());
            double VisExitEX = double.Parse(worksheet.Range["C28"].Value.ToString());
            double VisOcnVhodEX = double.Parse(worksheet.Range["C29"].Value.ToString());
            double VisOsnExitEX = double.Parse(worksheet.Range["C30"].Value.ToString());
            double KoffAEX = double.Parse(worksheet.Range["C31"].Value.ToString());
            double KoffBEX = double.Parse(worksheet.Range["C32"].Value.ToString());
            double Vis1180EX = double.Parse(worksheet.Range["C33"].Value.ToString());
            double Vis1200EX = double.Parse(worksheet.Range["C34"].Value.ToString());
            double Vis1250EX = double.Parse(worksheet.Range["C35"].Value.ToString());
            double Vis1275EX = double.Parse(worksheet.Range["C36"].Value.ToString());
            double Vis1300EX = double.Parse(worksheet.Range["C37"].Value.ToString());
            double TempKristalEX = double.Parse(worksheet.Range["C38"].Value.ToString());
            // Показываем приложение
            
            workBook.Save();

            cls.pogr =   Math.Round((((Math.Abs(((Math.Abs(VisVhodEX) - Math.Abs(cls.VisVhod)) / Math.Abs(VisVhodEX))) +
                                    Math.Abs(((Math.Abs(VisExitEX) - Math.Abs(cls.VisExit)) / Math.Abs(VisExitEX)))+
                                    Math.Abs(((Math.Abs(VisOcnVhodEX) - Math.Abs(cls.VisOcnVhod)) / Math.Abs(VisOcnVhodEX))) +
                                    Math.Abs(((Math.Abs(VisOsnExitEX) - Math.Abs(cls.VisOsnExit)) / Math.Abs(VisOsnExitEX))) +
                                    Math.Abs(((Math.Abs(Vis1250EX) - Math.Abs(cls.Vis1250)) / Math.Abs(Vis1250EX))) +
                                    Math.Abs(((Math.Abs(Vis1275EX) - Math.Abs(cls.Vis1275)) / Math.Abs(Vis1275EX)))+
                                    Math.Abs(((Math.Abs(Vis1300EX) - Math.Abs(cls.Vis1300)) / Math.Abs(Vis1300EX))) +
                                    Math.Abs(((Math.Abs(TempKristalEX) - Math.Abs(cls.TempExit)) / Math.Abs(TempKristalEX))) +
                                    Math.Abs(((Math.Abs(KoffAEX) - Math.Abs(cls.KoffA)) / Math.Abs(KoffAEX))) +
                                    Math.Abs(((Math.Abs(KoffBEX) - Math.Abs(cls.KoffB)) / Math.Abs(KoffBEX))) +
                                    Math.Abs(((Math.Abs(Vis1180EX) - Math.Abs(cls.Vis1180)) / Math.Abs(Vis1180EX))) +
                                    Math.Abs(((Math.Abs(Vis1200EX) - Math.Abs(cls.Vis1200)) / Math.Abs(Vis1200EX))))) /12)*100,2);

            #endregion
            workBook.Close(false, Type.Missing, Type.Missing);
            application.Quit();
            return View();
        }


        
        
    }
}
