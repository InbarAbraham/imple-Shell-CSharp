using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMonitor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // הדפסת שורת כותרת
            Console.WriteLine("Id \t Memory \t Name");

            // קבלת כל התהליכים הרצים במערכת
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    // קבלת פרטי התהליך
                    int processId = process.Id;
                    string processName = process.ProcessName;
                    long memorySize = process.PrivateMemorySize64;

                    // המרת כמות הזיכרון לפורמט קריא
                    string formattedMemory;
                    if (memorySize < 1024)
                    {
                        formattedMemory = $"{memorySize} Bytes";
                    }
                    else if (memorySize < 1024 * 1024)
                    {
                        formattedMemory = $"{memorySize / 1024} KB";
                    }
                    else
                    {
                        formattedMemory = $"{memorySize / (1024 * 1024)} MB";
                    }

                    // הדפסת פרטי התהליך
                    Console.WriteLine($"{processId} \t {formattedMemory} \t {processName}");
                }
                catch (Exception)
                {
                    // אם קורה Exception, התוכנית ממשיכה לרוץ
                    Console.WriteLine($"Could not access process {process.Id}");
                }
            }
        }
    }
}