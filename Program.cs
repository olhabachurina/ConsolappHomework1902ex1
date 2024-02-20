using System.Diagnostics;
using System.Net.Sockets;

namespace ConsolappHomework1902ex1;

class Program
{

    private static readonly object locker = new object();
    private static bool isCompleted = false;
    static void Main()
    {
        string filePath = @"Data\text.txt";
        Thread thread1 = new Thread(() => ProcessFile(filePath));
        Thread thread2 = new Thread(() => ModifyFile(filePath));
        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
    }
    private static void ProcessFile(string filePath)
    {
        try
        {
            string content = File.ReadAllText(filePath);
            int sentenceCount = content.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;

            Console.WriteLine($"Количество предложений: {sentenceCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке файла: {ex.Message}");
        }
        finally
        {
            lock (locker)
            {
                isCompleted = true;
            }
        }
    }
    private static void ModifyFile(string filePath)
    {
        bool completed = false;
        while (!completed)
        {
            lock (locker)
            {
                completed = isCompleted;
            }
            if (!completed)
            {
                Console.WriteLine("Ожидание завершения первого потока...");
                Thread.Sleep(50);
            }
        }

        try
        {
            Console.WriteLine("Начало модификации файла...");
            string content = File.ReadAllText(filePath);
            if (content.Contains('!'))
            {
                content = content.Replace('!', '#');
                File.WriteAllText(filePath, content);
                Console.WriteLine("Модификация файла завершена.");
            }
            else
            {
                Console.WriteLine("В файле не найдены восклицательные знаки для замены.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при модификации файла: {ex.Message}");
        }
    }
}
