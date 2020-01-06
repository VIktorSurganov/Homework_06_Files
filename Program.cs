using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Text;
namespace ObjectLessons
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ВНИМАНИЕ, ПОТОК БУДЕТ ОСУЩЕСТВЛЯТЬ ЗАПИСЬ В ПАПКУ С ПРОЕКТОМ");
            Console.ResetColor();
            Console.Write("Введите количество N для расчета: ");
            int n; bool choice = false;
            n = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Нажмите \"0\" только для подсчета количества груп М или \n" +
                                    "\t\"1\" для записи файла с результатом расчетов");

            switch (Console.ReadKey().KeyChar)
            {
                case ('0'):
                    choice = false;
                    break;
                case ('1'):
                    choice = true;
                    break;
            }

            DateTime date = new DateTime();
            date = DateTime.Now;
            Console.WriteLine();
            myStrangeMethod(n, choice);

            TimeSpan timeSpan = DateTime.Now.Subtract(date);
            Console.WriteLine($"Рассчет завершен за {timeSpan.Seconds} сек. {timeSpan.Milliseconds} миллисек.");
            if (choice == true)
            { 
            Console.WriteLine("Вы хотите заархивировать результаты вычислений?(y/n)");
            switch (Console.ReadKey().KeyChar)
            {
                case ('y'):
                    ArchiveMyStrangeMethod();
                    Console.WriteLine("\nРассчеты заархивированы!");
                    break;

                case ('н'):
                    ArchiveMyStrangeMethod();
                    Console.WriteLine("\nРассчеты заархивированы!");
                    break;
                }
            }       

            Console.ReadLine();
        }
        /// <summary>
        /// Метод для архивации вычислений
        /// </summary>
        public static void ArchiveMyStrangeMethod()
        {
            using (FileStream fs = new FileStream(@"myStrangeMethod.csv", FileMode.OpenOrCreate))
            {
                using (FileStream fsComp = File.Create(@"myStrangeMethod.zip"))
                {
                    using (GZipStream zipStream = new GZipStream(fsComp, CompressionMode.Compress))
                    {
                        fs.CopyTo(zipStream);
                    }
                }
            }
        }
        /// <summary>
        /// Метод разбиения последовательности чисел на группы неделимых друг на друга без остатка
        /// </summary>
        /// <param name="numbs">Количество чисел</param>
        /// <param name="Write">true для создания файла вывода, false для только подсчета групп</param>
        public static void myStrangeMethod(int numbs, bool Write=false)
        {
            //1.Из принятого numbs образую массив
            int[] numbsArr = new int[numbs];
            for (int i = 0; i < numbs; i++)
            {
                numbsArr[i] = i + 1;
            }
            //2.Объявляю необходимые переменные.
            int lines = 0;          //переменная для посдсчета групп
            int pow = 0;            //для степени двойки
            const int osnov = 2;

            //Судя по примеру, при n = 50, получилось 6 групп, каждая из которых начиналась 
            //с числа 2 возведенного в степень от 0 до 5 соответсвенно
            //я решил ради эксперимента применить данный алгоритм для образования групп в своей программе
            //т.к. очевидно, что не может быть в одном ряду 16, 32, 64 и т.д.

            while (Math.Pow(osnov,pow)<numbs)
            {
                lines++;
                pow++;
            }
            Console.WriteLine($"Колличество групп при N = {numbs} - {lines.ToString()}");
            
            //Если пользователь выбрал false, то метод на этом моменте завершится 
            if (Write)
            {
                Console.WriteLine("Ожидайте...");
                //Создаю зубчатый массив, т.к. группы решений больше всего напоминают их
                int[][] numbsRawArr = new int[lines][];
                //Заполняю все подмассивы первыми членами, что равны 2 в степени от 0 до номера последней группы
            for (int i = 0; i < lines; i++)
            {
                int firstElement = (int)Math.Pow(osnov, i);
                if (firstElement <= numbs)
                {
                    numbsRawArr[i] = new int[1] { firstElement };
                }
                else
                {
                    numbsRawArr[i] = new int[0]; break;
                }
            }
            //Здесь основная часть рассчетов. Начинаю с 2, т.к. 0 нет, а 1 уже является первым элементом первого подмассива
            int countForNumbs = 2;            
                do
                {
                    //Для каждой строки (всей группы подмассивов)
                    for (int i = 0; i < numbsRawArr.Length; i++)
                    {
                        //Для каждого члена каждого подмассива
                        for (int j = 0; j < numbsRawArr[i].Length; j++)
                        {
                            //Если данное проверяеоме число больше текущего обрабатываемого элемента и если для всех элементов имеется деление с остатком
                            if (numbsRawArr[i][j] < countForNumbs && numbsRawArr[i].All(n => countForNumbs % n != 0))
                            {
                                //Создаю переменную, содержащую новую длину текущего подмассива (+1)
                                int newSize = numbsRawArr[i].Length + 1;
                                //Расширяю текущий массив до этой длины 
                                Array.Resize(ref numbsRawArr[i], newSize);
                                //Вношу текущее проверяемое число на эту позицию
                                numbsRawArr[i][newSize - 1] = countForNumbs;
                                //Выбираею следующее для проверки
                                countForNumbs++;
                                //Перехожу в первый подмассив для осуществления полного цикла проверки для следующего числа
                                i = 0;
                                break;
                            }
                        }
                        //Проверка на выход за пределы изначально заданного N, если перебор закончился не на последней строке
                        if (countForNumbs == numbs + 1) break;
                    }
                    countForNumbs++;
                    
                } while (countForNumbs <= numbs);
                //Здесь при помощи StreamWriter, после обработки всех элементов, заполняется файл .csv
                //для удобного просмотра результатов в Excel либо просто в текстовом редакторе

                using (StreamWriter sw = new StreamWriter(@"myStrangeMethod.csv", false, Encoding.Unicode))
                {
                    for (int i = 0; i < numbsRawArr.Length; i++)
                    {
                        for (int g = 0; g < numbsRawArr[i].Length; g++)
                        {
                            sw.Write($"{numbsRawArr[i][g]} \t");
                        }
                        sw.WriteLine();
                    }
                }
            }            
        }       
    }
}
