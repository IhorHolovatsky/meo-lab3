using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var altQty = GetQuantity("Введіть число альтернатив: ");
            var expQty = GetQuantity("Введіть число експертів: ");
            var system = GetQuantity("Введіть номер системи: ");

            // Write alternatives to data
            var altNames = GetAnswerNames("Введіть назви альтернатив", altQty);

            // Write headers
            //  data[0][data[0].length - 1] = "SUM";
            //  data[data.length - 1][0] = "NORMALIZED";

            // Write experts to data
            var expNames = GetExpertNames("Введіть імена експертів", expQty);

            // Write points of each expert
            var tableModel = SetPoints(system, altNames, expNames);

            // Calculate and write sum for each expert
            Console.WriteLine("Обраховуємо результати");
            var expSums = CalculateAndSetSums(tableModel, expNames, altNames);

            Console.WriteLine("Нормовані оцінки");
            CalculateAndSetNorms(expSums, tableModel, expNames, altNames);

            Console.ReadLine();
        }

        private static void CalculateAndSetNorms(List<ExpertSum> expSums, List<ExpertAnswerModel> tableModel, List<ExpertModel> expNames, List<AnswerModel> altNames)
        {
            var headersList = new List<string> { "Альтернатива", "Нормалізована оцінка" };

            PrintRow(headersList.ToArray());

            PrintLine();

            foreach (var a in altNames)
            {
                var currentRow = new List<string> {a.Name};

                var currentPoints = tableModel.Where(x => x.AnswerId == a.Id).ToList();

                double sum = 0;
                foreach (var p in currentPoints)
                {
                    var pA = Convert.ToDouble(p.ExpertAnswer);
                    var sumEx = Convert.ToDouble(expSums.First(x => x.ExpertId == p.ExpertId).Sum);
                    sum += pA / sumEx;
                }

                var norm = Convert.ToString(Math.Round(sum / expNames.Count, 2));

                currentRow.Add(norm);

                PrintRow(currentRow.ToArray());

                PrintLine();
            }

        }

        private static List<ExpertSum> CalculateAndSetSums(List<ExpertAnswerModel> tableModel, List<ExpertModel> expNames, List<AnswerModel> altNames)
        {

            var result = new List<ExpertSum>();
            var headersList = new List<string>();
            headersList.Add("Експерт");
            foreach (var item in altNames)
            {
                headersList.Add(item.Name);
            }
            headersList.Add("Сума");

            PrintRow(headersList.ToArray());

            PrintLine();

            foreach (var item in expNames)
            {
                var currentRow = new List<string>();
                currentRow.Add(item.Name);
                var expertAnswers = tableModel
                    .Where(x => x.ExpertId == item.Id)
                    .OrderBy(x => x.AnswerId)
                    .Select(x => x.ExpertAnswer);
                currentRow.AddRange(expertAnswers);
                int sum = 0;
                foreach (var p in expertAnswers)
                {
                    sum += Convert.ToInt32(p);
                }
                currentRow.Add(sum.ToString());

                PrintRow(currentRow.ToArray());

                PrintLine();

                result.Add(new ExpertSum() { ExpertId = item.Id, Sum = sum });
            }

            return result;
        }
        private static int GetQuantity(String message)
        {
            try
            {
                Console.WriteLine(message);
                return Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private static List<ExpertModel> GetExpertNames(String message, int length)
        {
            Console.WriteLine(message);
            List<ExpertModel> result = new List<ExpertModel>();
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine($"{i.ToString()}:");
                result.Add(new ExpertModel() { Id = i, Name = Console.ReadLine() });
            }
            return result;
        }
        private static List<AnswerModel> GetAnswerNames(String message, int length)
        {
            Console.WriteLine(message);
            List<AnswerModel> result = new List<AnswerModel>();
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine($"{i.ToString()}:");
                result.Add(new AnswerModel() { Id = i, Name = Console.ReadLine() });
            }
            return result;
        }

        private static List<ExpertAnswerModel> SetPoints(int pointSystem, List<AnswerModel> answerModels, List<ExpertModel> expertModels)
        {
            var result = new List<ExpertAnswerModel>();
            foreach (var expert in expertModels)
            {
                Console.WriteLine($"Експерт {expert.Name} - введіть оцінки (значення оцінки має бути нище {pointSystem})");
                foreach (var answer in answerModels)
                {
                    try
                    {
                        Console.WriteLine($"Альтернатива {answer.Name}");
                        var point = Console.ReadLine();
                        if (Convert.ToInt32(point) > pointSystem)
                        {
                            point = pointSystem.ToString();
                        }
                        if (Convert.ToInt32(point) < 0)
                        {
                            point = "0";
                        }
                        result.Add(new ExpertAnswerModel()
                        { AnswerId = answer.Id, ExpertAnswer = point, ExpertId = expert.Id });
                    }
                    catch (Exception)
                    {
                        result.Add(new ExpertAnswerModel()
                        { AnswerId = answer.Id, ExpertAnswer = "0", ExpertId = expert.Id });
                    }
                }
            }
            return result;
        }


        static int tableWidth = 77;

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
    }
}
