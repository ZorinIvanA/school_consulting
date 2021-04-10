using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Miners
{
    class Program
    {
        private const string FILE_NAME = "minings.txt";
        private const int MINES_COUNT = 5;
        private const string _digits = "abcdefghij";
        private const string _digitsClean = "1234567890";
        static void Main(string[] args)
        {
            if (!File.Exists(FILE_NAME))
            {
                Console.WriteLine("Такого файла нет");
                return;
            }

            try
            {
                var incomes = ReadFile(FILE_NAME);

                //Определить самую эффективную шахту холдинга в каждом квартале
                for (int i = 1; i <= 12; i += 3)
                {
                    var mostEffiecientMine = 0;
                    var maxEfficience = 0;
                    for (int j = 0; j < MINES_COUNT; j++)
                    {
                        var efiicency = GetEfficencyForPeriod(j, i, i + 2, incomes);
                        if (efiicency > maxEfficience)
                        {
                            mostEffiecientMine = j;
                            maxEfficience = efiicency;
                        }
                    }
                    Console.WriteLine($"{mostEffiecientMine}, {maxEfficience}");
                }

                //Определить самую эффективную шахту за год
                var mostEffiecientMineYear = 0;
                var maxEfficienceYear = 0;
                for (int j = 0; j < MINES_COUNT; j++)
                {
                    var efiicency = GetEfficencyForPeriod(j, 1, 12, incomes);
                    if (efiicency > maxEfficienceYear)
                    {
                        mostEffiecientMineYear = j;
                        maxEfficienceYear = efiicency;
                    }
                }
                Console.WriteLine($"Годовая эффективность {mostEffiecientMineYear}, {maxEfficienceYear}");

                for (int j = 0; j < MINES_COUNT; j++)
                {
                    var efiicency = GetAverageIncome(j, 1, 12, incomes);
                    Console.WriteLine($"Среднегодовая добыча шахты {j + 1}: {efiicency}");
                }

                //Расчитать средний объём добычи угля по каждой шахте в течение каждого квартала года.
                var averagesByQuartal = new int[4][];
                var currentQ = -1;

                for (int i = 1; i <= 12; i += 3)
                {
                    currentQ++;
                    var averages = GetAverageIncomeForAllMines(i, i + 2, incomes);
                    for (var j = 0; j < averages.Length; j++)
                    {
                        Console.WriteLine($"Шахта {j + 1} добыла {averages[j]}");
                    }

                    averagesByQuartal[currentQ] = averages;
                }
                //Console.WriteLine("Введите имя файла");
                //var outputFileName = Console.ReadLine();
                //string[] linesToRecord = new string[averagesByQuartal.Length];
                //for (int i = 0; i < averagesByQuartal.Length; i++)
                //{
                //    for (int j = 0; j < averagesByQuartal[i].Length; j++)
                //    {
                //        linesToRecord[i] += $"{averagesByQuartal[i][j]};";
                //    }
                //}
                //File.WriteAllLines(outputFileName, linesToRecord);


                ////Расчитать средний объём добычи угля по каждой шахте в течение квартала, указанного пользователем.
                //Console.WriteLine("Введите номер квартала");
                //var quartal = Console.ReadLine();
                //int qInt = int.Parse(quartal);
                //if (qInt < 1 || qInt > 4)
                //{
                //    Console.WriteLine("Ошибка ввода квартала");
                //    return;
                //}

                //switch (qInt)
                //{
                //    case 1:
                //        GetAverageIncomeForAllMines(1, 3, incomes);
                //        break;
                //    case 2:
                //        GetAverageIncomeForAllMines(4, 6, incomes);
                //        break;
                //    case 3:
                //        GetAverageIncomeForAllMines(7, 9, incomes);
                //        break;
                //    case 4:
                //        GetAverageIncomeForAllMines(10, 12, incomes);
                //        break;
                //}

                var answer = string.Empty;
                while (!answer.Equals("y", StringComparison.InvariantCultureIgnoreCase) &&
                       !answer.Equals("n", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Сохранить результаты (Y/N):");
                    answer = Console.ReadLine();

                    var resultsFileName = "results.txt";
                    var strings = new string[averagesByQuartal.Length];
                    var dt = DateTime.Now;

                    for (int i = 0; i < strings.Length; i++)
                    {
                        foreach (var i1 in averagesByQuartal[i])
                        {
                            var s = strings[i];
                            s += i1.ToString();
                            s += ";";
                            for (int j = 0; j < s.Length; j++)
                            {
                                if (_digitsClean.Contains(s[j]))
                                {
                                    var oldChar = s[j];
                                    var newChar = _digits[int.Parse(s[j].ToString())];
                                    s=s.Replace(oldChar, newChar);
                                }
                            }

                            strings[i] = s;
                        }
                    }
                    File.WriteAllLines(resultsFileName, strings);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static int[] GetAverageIncomeForAllMines(int start, int end, int[,] incomes)
        {
            var averageIncomes = new int[MINES_COUNT];
            for (int j = 0; j < MINES_COUNT; j++)
            {
                averageIncomes[j] = GetAverageIncome(j, start, end, incomes);
            }

            return averageIncomes;
        }

        private static int GetEfficencyForPeriod(int mine, int startMonth, int endMonth, int[,] incomes)
        {
            var ef = 0;
            for (int i = startMonth - 1; i < endMonth; i++)
            {
                ef += incomes[i, mine];
            }

            return ef;
        }

        private static int GetAverageIncome(int mine, int startMonth, int endMonth, int[,] incomes)
        {
            var ef = 0;
            for (int i = startMonth - 1; i < endMonth; i++)
            {
                ef += incomes[i, mine];
            }

            return ef / (endMonth - startMonth + 1);
        }
        private static int[,] ReadFile(string fileName)
        {
            var incomesLocal = new int[12, MINES_COUNT];
            var currentRow = 0;
            foreach (var line in File.ReadAllLines(FILE_NAME))
            {
                var splittedString = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var currentColumn = 0;
                for (int i = 1; i < splittedString.Length; i++)
                {
                    incomesLocal[currentRow, currentColumn] = int.Parse(splittedString[i]);
                    currentColumn++;
                }

                currentRow++;
            }

            return incomesLocal;
        }
    }
}
