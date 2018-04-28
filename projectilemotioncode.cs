using System;
using System.Collections.Generic;
//JORDAN MATHIS 2017
namespace projectilemotion
{
    class Program
    {
        //VARIABLES
        public class Attribute
        {
            public string name;
            public double? value;
            public string measurement;
            public bool given = false;
            public Attribute(string name, string measurement)
            {
                this.name = name;
                this.measurement = measurement;
            }
        }
        public static double GRAV_CONST = 9.8;
        public static double DEG_CONV = 180 / Math.PI;
        public static Attribute dx = new Attribute("horizontal distance","m");
        public static Attribute vx = new Attribute("horizontal velocity","m/s");
        public static Attribute dy = new Attribute("vertical distance","m");
        public static Attribute voy = new Attribute("vertical initial velocity","m/s");
        public static Attribute vy = new Attribute("vertical final velocity","m/s");
        public static Attribute t = new Attribute("time","s");
        public static Attribute initVelo = new Attribute("initial diagonal velocity","m/s");
        public static Attribute finalVelo = new Attribute("final diagonal velocity","m/s");
        public static Attribute initAngle = new Attribute("launch angle", "°");
        public static Attribute finalAngle = new Attribute("impact angle", "°");
        public static Attribute maxHeight = new Attribute("maximum height","m");
        public static List<Attribute> answers = new List<Attribute> { dx, vx, dy, voy, vy, t, initVelo, finalVelo, initAngle, finalAngle, maxHeight };
        public static bool end;

        //HOW THE MAGIC HAPPENS
        public static void Calculate()
        {
            void tickEvent(Object source, System.Timers.ElapsedEventArgs e)
            {
                end = true;
            }
            System.Timers.Timer timer = new System.Timers.Timer(6000);
            timer.Enabled = true;
            timer.Elapsed += tickEvent;
            end = false;
            //i.e "while any of the properties are unaccounted for
            while (dx.value == null || vx.value == null || dy.value == null || voy.value == null || vy.value == null || t.value == null || initVelo.value == null || finalVelo.value == null || initAngle.value == null || finalAngle.value == null || maxHeight.value == null)
            {
                if (end == true) break;
                if (voy.value != null && dy.value != null && t.value == null)
                {
                    //ways to find time
                    if (dy.value == 0 && voy.value != 0)
                    {
                        t.value = -voy.value / (-GRAV_CONST / 2);
                    }
                    if (voy.value == 0)
                    {
                        t.value = Math.Sqrt((2 * Convert.ToDouble(dy.value)) / GRAV_CONST);
                    }
                    else
                    {
                        t.value = (-Convert.ToDouble(voy.value) - Math.Sqrt(((Math.Pow(Convert.ToDouble(voy.value), 2) - (4 * (-(GRAV_CONST / 2) * Convert.ToDouble(dy.value))))))) / (-GRAV_CONST);
                    }
                }
                if (vx.value != null && dx.value != null && t.value == null) { t.value = dx.value / vx.value; }
                //finding Vx
                if (t.value != null && dx.value != null && vx.value == null) { vx.value = dx.value / t.value; }
                if (finalVelo.value != null && vy.value != null && vx.value == null) { vx.value = Math.Sqrt(Math.Pow(Convert.ToDouble(finalVelo.value), 2) - Math.Pow(Convert.ToDouble(vy.value), 2)); }
                //finding Vy
                if (finalVelo.value != null && vx.value != null && vy.value == null) { vy.value = Math.Sqrt(Math.Pow(Convert.ToDouble(finalVelo.value), 2) - Math.Pow(Convert.ToDouble(vx.value), 2)); }
                if (voy.value != null && t.value != null && vy.value == null) { vy.value = voy.value - (GRAV_CONST * t.value); }
                //finding Dx
                if (t.value != null && vx.value != null && dx.value == null) { dx.value = vx.value * t.value; }
                //finding Vx or Viy
                if (initAngle.value != null && initVelo.value != null && (vx.value == null || voy.value == null))
                {
                    vx.value =  (Math.Cos((Math.PI / 180) * Convert.ToDouble(initAngle.value))) * initVelo.value;
                    voy.value = (Math.Sin((Math.PI / 180) * Convert.ToDouble(initAngle.value))) * initVelo.value;
                }
                if (maxHeight.value != null && voy.value == null) { voy.value = Math.Sqrt(Convert.ToDouble(maxHeight.value) * (2 * GRAV_CONST)); }
                //finding initial angle and initial velocity using trigonometry
                if (vx.value != null && voy.value != null && (initAngle.value == null || initVelo.value == null))
                {
                    initVelo.value = Math.Sqrt(Math.Pow(Convert.ToDouble(voy.value), 2) + Math.Pow(Convert.ToDouble(vx.value), 2));
                    initAngle.value = DEG_CONV * (Math.Atan((Convert.ToDouble(voy.value) / Convert.ToDouble(vx.value))));
                }
                //finding Dy
                if (t.value != null && voy.value != null && dy.value == null) { dy.value = Math.Abs(Convert.ToDouble(-4.9 * Math.Pow(Convert.ToDouble(t.value), 2) + voy.value * t.value)); }
                //finding max height
                if (voy.value != null && maxHeight.value == null) { maxHeight.value = (Math.Pow(Convert.ToDouble(voy.value), 2)) / (2 * GRAV_CONST); }
                //finding impact angle and velocity using trigonometry
                if (vy.value != null && vx.value != null && (finalVelo.value == null || finalAngle.value == null))
                {
                    finalVelo.value = Math.Sqrt(Math.Pow(Convert.ToDouble(vy.value), 2) + Math.Pow(Convert.ToDouble(vx.value), 2));
                    finalAngle.value = DEG_CONV * Math.Atan((Convert.ToDouble(vy.value) / (Convert.ToDouble(vx.value))));
                }
            }
            timer.Enabled = false;
        }
        //OUTPUT METHOD
        public static void Print()
        {
            if (end == true)
            {
                Console.WriteLine("Values could not be calculated. Did you provide all the necessary information?\n");
            }
            else
            {
                Console.WriteLine("ANSWERS\n");
                foreach (Attribute question in answers)
                {
                    if (question.given == true)
                    {
                        Console.WriteLine(question.name + " (GIVEN): " + Math.Round(Convert.ToDouble(question.value), 2)+question.measurement);
                    }
                }
                Console.WriteLine();
                foreach (Attribute question in answers)
                {
                    if (question.given == false)
                    {
                        Console.WriteLine(question.name + ": " + Math.Round(Convert.ToDouble(question.value), 2)+question.measurement);
                    }
                }
                Console.WriteLine();
            }
        }
        //INPUT METHOD
        public static void Ask(Attribute question)
        {
            string temp;
            question.given = false;
            Console.Write("Enter {0} in ({1}): ", question.name, question.measurement);
                temp = Console.ReadLine();
            if (temp != "")
            {
                    question.value = Convert.ToDouble(temp);
                    question.given = true;
            }
        }
        //WHERE THE MAGIC HAPPENS
        static void Main(string[] args)
        {
            string answer;
            Console.WriteLine("Welcome to Jordan's super sweet projectile motion calculator!\n\nPress any button to get started.");
            Console.ReadKey(true);
            do
            {
                Console.Clear();
                foreach (Attribute question in answers)
                {
                    question.value = null;
                    Ask(question);
                }
                Console.Clear();
                Calculate();
                Print();
                Console.Write("Enter n to exit. Otherwise, press enter to try again. ");
                answer = Console.ReadLine(); ;
            } while (answer != "n");
        }
    }
}
