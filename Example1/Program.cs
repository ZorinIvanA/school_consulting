using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;

namespace Example1
{
    class Program
    {
        private static string serial;
        private static Colors[] module;

        static void Main(string[] args)
        {
            string command = string.Empty;
            while (command != "exit")
            {
                Console.WriteLine("Enter the next command");
                command = Console.ReadLine();
                switch (command)
                {
                    case "?":
                        {
                            PrintCommandList();
                        }
                        break;
                    case "exit":
                        {
                            Console.WriteLine("Exiting program");
                        }
                        break;
                    case "start":
                        {
                            StartDefuse();
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Command not exists");
                            PrintCommandList();
                        }
                        break;
                }
            }
        }

        private static void PrintCommandList()
        {
            Console.WriteLine("-----------Commands list ---------------");
            Console.WriteLine("? - outputs commands list");
            Console.WriteLine("start - start defusing");
            Console.WriteLine("exit - exit software");
        }

        private static void StartDefuse()
        {
            serial = GenerateSerial();
            Console.WriteLine($"The bomb has been planted");
            Console.WriteLine($"Defusing bomb with serial {serial}");
            module = GetModule();
            Console.WriteLine("Wires in module are");
            for (int i = 0; i < module.Length; i++)
            {
                Console.Write(module[i]);
                Console.Write("; ");
            }

            Console.WriteLine("Choose wire number to cut");
            var number = Console.ReadLine();
            var numberAsInt = int.Parse(number);

            var defuseResult = CalcDefuseResult(module, numberAsInt);

            if (defuseResult)
                Console.WriteLine("The bomb is defused");
            else
                Console.WriteLine("BOOOOOM");
        }

        private static bool CalcDefuseResult(Colors[] module, int numberAsInt)
        {
            if (module.Length == 3)
            {
                if (!module.Contains(Colors.Black) && numberAsInt == 0)
                    return true;
                var yellows = 0;
                var lastYellow = -1;
                for (int i = 0; i < module.Length; i++)
                {
                    if (module[i] == Colors.Yellow)
                        yellows++;
                    lastYellow = i;
                }

                if (yellows > 2)
                    return lastYellow == numberAsInt;

                if (module.Last() == Colors.Blue || module[module.Length - 1] == Colors.White)
                    return numberAsInt == module.Length - 1;

                return numberAsInt == 1;
            }

            return false;
        }

        private static Colors[] GetModule()
        {
            Random rnd = new Random();
            module = new Colors[rnd.Next(3, 3)];
            for (int i = 0; i < module.Length; ++i)
            {
                module[i] = (Colors)rnd.Next(1, 5);
            }

            return module;
        }

        private static string GenerateSerial()
        {
            return "TEST1234";
        }
    }

    public enum Colors
    {
        Red = 1,
        Blue = 2,
        White = 3,
        Yellow = 4,
        Black = 5
    }
}
