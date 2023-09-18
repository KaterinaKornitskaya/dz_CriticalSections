using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dz_CriticalSections
{
    /*
     Создайте приложение, использующее критические секции. Создайте
    несколько потоков: первый поток генерирует и сохраняет в файл
    некоторое кол-во пар чисел. Второй поток ожидает завершение 
    генерации, после чего подсчитывает сумму каждой пары, результат 
    записывается в файл. Третий поток также ожидает завершение генерации,
    после чего подсчитывает произведение кажой пары, результат записывает
    в файл.
     */

    // пользовательский класс MyFile
    class MyFile : IDisposable // наследует интерфейс IDisposable для правильной работы с файлами
    {
        public FileStream File { get; set; }
        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }
        public string FileName { get; set; }

        public MyFile(string fileName)
        {
            FileName = fileName;
        }
        public void Dispose()
        {
            File.Close();
        }
        
        
        // метод для генерации пар чисел и записи в файл
        public void NumberGenerator()
        {
            lock (this)  // критическая секция для работы с общим ресурсом
            {
                Random rand = new Random();
                File = new FileStream(FileName, FileMode.OpenOrCreate);  // создаем объект FileStream
                Writer = new StreamWriter(File);  // создаем объект StreamWriter
                try
                {
                    for(int i = 0; i < 10; i++)  // в цикле с 10 итерациями
                    {
                        // записываем в файл два рандомный числа через пробел
                        Writer.WriteLine(rand.Next(0,10) + " " + rand.Next(0, 10));
                    }
                    Console.WriteLine("Файл с парами чисел записан успешно!");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Writer.Close();
            }
        }

        // метод для генерации сумм пар чисел и записи в файл
        public void Sum()
        {
            lock(this)  // критическая секция для работы с общим ресурсом
            {
                File = new FileStream(FileName, FileMode.OpenOrCreate);  // создаем объект FileStream
                Reader = new StreamReader(File);  // создаем объект StreamReader
                string name = "Add.txt";  // название файла для записи
                Writer = new StreamWriter(name, false);  // создаем объект StreamWriter с новым файлом
                
                string line;
                try
                {
                    // считываем каждую строку файла
                    while ((line = Reader.ReadLine()!) != null)
                    {
                        // число 1 - берем подстроку с помощью метода Substring,
                        // где начало подстроки - это 0ой индекс line, а длина подстроки - первое вхождение пробела
                        int n1 = int.Parse(line.Substring(0, line.IndexOf(" ")));
                        // число 2 - подстрока с помощью Substring, начало - пробел, и до конца строки
                        int n2 = int.Parse(line.Substring(line.IndexOf(" "), line.Length-1));
                        int sum = n1+n2;
                        Writer.WriteLine($"{n1} + {n2} = {sum}");
                    }
                    Console.WriteLine("Файл c суммами пар чисел записан успешно!");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Reader.Close();
                Writer.Close();
            }
        }

        // метод для генерации произведений пар чисел и записи в файл, аналогично методу Sum()
        public void Mult()
        {
            lock (this)
            {
                File = new FileStream(FileName, FileMode.OpenOrCreate);
                Reader = new StreamReader(File);
                string name = "Mult.txt";
                Writer = new StreamWriter(name, false);
                string line;
                try
                {
                    while ((line = Reader.ReadLine()!) != null)
                    {
                        int n1 = int.Parse(line.Substring(0, line.IndexOf(" ")));
                        int n2 = int.Parse(line.Substring(line.IndexOf(" "), line.Length-1));
                        int mult = n1*n2;
                        Writer.WriteLine($"{n1} * {n2} = {mult}");
                    }
                    Console.WriteLine("Файл c произведениями пар чисел записан успешно!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Reader.Close();
                Writer.Close();
            }
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            // создаем экземпляр класса myFile
            MyFile myFile = new MyFile("Generated Numbers.txt");
            
            // создаем экземпляры класса Task, который инкапсулирует поток,
            // и инициализируем аски соответствующими методами
            Task t1 = new Task(myFile.NumberGenerator);
            Task t2 = new Task(myFile.Sum);
            Task t3 = new Task(myFile.Mult);
            
            // запускаем потоки
            t1.Start();
            t2.Start();
            t3.Start();

            // ожидание, пока все дочерние потоки (t1, t2, t3) не завершат свою
            // работу
            Task.WaitAll(t1, t2, t3);

            // после завершения потоков t1, t2, t3 выполняется работа основного потока
            Console.WriteLine("Конец работы программы!");
        }
    }
}