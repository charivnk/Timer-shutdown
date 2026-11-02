using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;


namespace Shutdown;
class Program
{
    public static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth)); 
        Console.SetCursorPosition(0, currentLineCursor);
    }
    static async Task Main(string[] args)
    { 
        string filePath = "datetime.txt";

        DateTime startTime = DateTime.Now;
        DateTime endTime = DateTime.Now;

        CultureInfo provider = CultureInfo.InvariantCulture;

        int hours = 0;
        int minutes = 0;

        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            
            // Parse the string back into a DateTime object
            DateTime extractedTime = DateTime.ParseExact(fileContent,"o",provider);
            
            //Console.WriteLine(fileContent);
            if (extractedTime>startTime)
            {
                Console.WriteLine("Timer continued...");
                endTime = extractedTime;
                Console.WriteLine($"Endtime: {endTime:HH:mm:ss}");
            }
            else
            {

                bool correct;
                int answer;
                
                bool finishAddingTime = false;
                
                while(!finishAddingTime)
                {
                    answer = 0;

                    Console.WriteLine("Select what to do:");
                    Console.WriteLine("1) 1 minutes");
                    Console.WriteLine("2) 10 minutes");
                    Console.WriteLine("3) 30 minutes");
                    Console.WriteLine("4) 1 hour");
                    Console.WriteLine("0) Start timer!");
                    
                    
                    correct = int.TryParse( Console.ReadLine(), out answer); 
                    if (correct)
                    {
                        if (answer >= 0 && answer <= 4)
                        {
                            if (answer == 1)
                            {
                                minutes += 1;
                            }
                            if (answer == 2)
                            {
                                minutes += 10;
                            }
                            else if (answer == 3)
                            {
                                minutes += 30;
                            }
                            else if (answer == 4)
                            {
                                hours += 1;
                            }
                            else if (answer == 0)
                            {
                                finishAddingTime=true;
                            }
                        }
                    } 
                }
                Console.WriteLine("Timer started...");
                endTime = startTime.AddHours(hours).AddMinutes(minutes); 
                Console.WriteLine($"Endtime: {endTime:HH:mm:ss}");  
            }
        }
        string isoString = endTime.ToString("o");
        File.WriteAllText(filePath, isoString);
        // 1. Initialize the timer with a 1-second interval
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        using CancellationTokenSource cts = new CancellationTokenSource();

            // 2. Loop asynchronously every time the period elapses
        while (await timer.WaitForNextTickAsync(cts.Token))
        {
            startTime = DateTime.Now;
            //record time every second
            Console.WriteLine($"CurrenTime: {DateTime.Now:HH:mm:ss}");
            Thread.Sleep(1000);
            Console.SetCursorPosition(0, Console.CursorTop-1);
            ClearCurrentConsoleLine();
            if (startTime>=endTime)
            {
            cts.Cancel();
            Console.WriteLine("Exiting the program...");
            Environment.Exit(0);
            }
        }
    }
}        
 


