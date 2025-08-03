using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Shell
{
    class Shell
    {
        public void ExecuteSingleProcess(string sCommand)
        {
            string inputFile = null; // משתנה לשמירת שם קובץ הקלט
            string outputFile = null; // משתנה לשמירת שם קובץ הפלט

            // זיהוי אם יש גם < וגם >
            if (sCommand.Contains("<") && sCommand.Contains(">"))
            {
                var parts = sCommand.Split(new char[] { '<', '>' });
                sCommand = parts[0].Trim(); // הפקודה עצמה
                inputFile = parts[1].Trim(); // שם קובץ הקלט
                outputFile = parts[2].Trim(); // שם קובץ הפלט
            }
            else if (sCommand.Contains("<")) // אם יש רק <
            {
                var parts = sCommand.Split('<');
                sCommand = parts[0].Trim();
                inputFile = parts[1].Trim();
            }
            else if (sCommand.Contains(">")) // אם יש רק >
            {
                var parts = sCommand.Split('>');
                sCommand = parts[0].Trim();
                outputFile = parts[1].Trim();
            }

            // בדיקה אם התהליך נדרש לרוץ ברקע (עם & בסוף הפקודה)
            bool runInBackground = sCommand.EndsWith("&");
            if (runInBackground)
            {
                sCommand = sCommand.TrimEnd('&').Trim();
            }

            // יצירת אובייקט Process
            Process p = new Process();

            // נשתמש ב-cmd.exe לביצוע הפקודה
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/C {sCommand}"; // הפקודה תועבר ל-cmd.exe
            p.StartInfo.UseShellExecute = false; // לא להשתמש בקונסולה חדשה
            p.StartInfo.RedirectStandardInput = inputFile != null; // הפניה לקובץ קלט אם קיים
            p.StartInfo.RedirectStandardOutput = outputFile != null || !runInBackground; // הפניה לפלט אם קיים או במצב לא ברקע
            p.StartInfo.RedirectStandardError = true; // הפניה לשגיאות
            p.StartInfo.CreateNoWindow = true; // לא להציג חלון נוסף

            try
            {
                p.Start();

                // אם יש קובץ קלט, לקרוא ממנו ולהזין את התוכן לתוך התהליך
                if (inputFile != null)
                {
                    string inputContent = System.IO.File.ReadAllText(inputFile); // קריאת התוכן מקובץ הקלט
                    using (StreamWriter writer = p.StandardInput)
                    {
                        writer.Write(inputContent); // כתיבת התוכן לקלט
                    }
                }

                // אם יש קובץ פלט, לקרוא את הפלט של התהליך ולכתוב אותו לקובץ
                if (outputFile != null)
                {
                    string outputContent = p.StandardOutput.ReadToEnd(); // קריאת הפלט
                    System.IO.File.WriteAllText(outputFile, outputContent); // כתיבת הפלט לקובץ
                }
                else if (!runInBackground) // אם אין קובץ פלט ואין ריצה ברקע
                {
                    Console.WriteLine(p.StandardOutput.ReadToEnd()); // הדפסת הפלט לקונסולה
                }

                // אם התהליך לא רץ ברקע, ממתינים לסיום שלו
                if (!runInBackground)
                {
                    p.WaitForExit();
                }
            }
            catch (Exception e)
            {
                // טיפול בשגיאות
                Console.WriteLine($"Error executing command: {e.Message}");
            }
        }

        public void KillProcess(string sCommand)
        {
            string[] asCommand = sCommand.Split(' ');
            int iPid = 0;
            if (int.TryParse(asCommand[1].Trim(), out iPid))
            {
                Process p = Process.GetProcessById(iPid);
                p.Kill();
                Console.WriteLine($"Process with ID {iPid} terminated.");
            }
            else
            {
                string processName = asCommand[1].Trim();
                Process[] arrayProcesses = Process.GetProcessesByName(processName);

                if (arrayProcesses.Length == 0)
                {
                    Console.WriteLine($"No processes found with name '{processName}'.");
                }
                else
                {
                    foreach (var process in arrayProcesses)
                    {
                        process.Kill();
                        Console.WriteLine($"Process '{processName}' with ID {process.Id} terminated.");
                    }
                }
            }
        }

        public void Execute(string sFullCommand)
        {
            try
            {
                if (sFullCommand == "")
                    return;
                if (sFullCommand.StartsWith("kill"))
                {
                    KillProcess(sFullCommand);
                }
                else if (sFullCommand == "exit")
                {
                    Environment.Exit(0); // יציאה מהתוכנית
                }
                else
                {
                    ExecuteSingleProcess(sFullCommand);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Run()
        {
            int cLines = 0;
            while (true)
            {
                Console.Write(cLines + " >> ");
                string sLine = Console.ReadLine();
                Execute(sLine.Trim());
                cLines++;
            }
        }
    }
}
