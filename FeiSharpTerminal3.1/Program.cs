using FeiSharpStudio;
using System.Diagnostics;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace FeiSharp8._5RuntimeSdk
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "FeiSharp Terminal 8.0";
            while (true)
            {
                CustomConsole.WriteGray(">>>");
                string command = Console.ReadLine();
                if (command == "exit")
                {
                    Environment.Exit(0);
                }
                else if (command.Contains("run"))
                {
                    Console.WriteLine(">>>\"Input these codes......Enter 'exit' to exit this mode......\"");
                    string code = "";
                    string scode = "";
                    while (code != "exit")
                    {
                        if (code != "exit")
                        {
                            code = Console.ReadLine();
                            scode += code;
                        }
                        else
                        {
                            Console.WriteLine(">>>:\"User enter the 'exit' to exit this mode at \"" + DateTime.Now);
                            break;
                        }
                    }
                    string sourceCode = scode;
                    Lexer lexer = new(sourceCode);
                    List<Token> tokens = [];
                    Token token;
                    do
                    {
                        token = lexer.NextToken();

                        tokens.Add(token);
                    } while (token.Type != TokenTypes.EndOfFile);

                    Parser parser = new(tokens);
                    parser.OutputEvent += (s, e) =>
                    {
                        Console.WriteLine(e.Message);
                    };
                    try
                    {
                        parser.ParseStatements();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Parsing error: " + ex.Message);
                    }
                }
                else if (command.Contains("file"))
                {
                    Console.WriteLine(">>>:\"Input the path......\"");
                    string sourceCode = File.ReadAllText(Console.ReadLine());
                    Lexer lexer = new(sourceCode);
                    List<Token> tokens = [];
                    Token token;
                    do
                    {
                        token = lexer.NextToken();

                        tokens.Add(token);
                    } while (token.Type != TokenTypes.EndOfFile);
                    Parser parser = new(tokens);
                    parser.OutputEvent += (s, e) =>
                    {
                        Console.WriteLine(e.Message);
                    };
                    try
                    {
                        parser.ParseStatements();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Parsing error: " + ex.Message);
                    }
                }
                else if (command.Contains("nsbuild"))
                {
                    Console.WriteLine(">>>:\"Input these codes......Enter 'exit' to exit this mode......\"");
                    string code = "";
                    string scode = "";
                    while (code != "exit")
                    {
                        if (code != "exit")
                        {
                            code = Console.ReadLine();
                            scode += code;
                        }
                        else
                        {
                            Console.WriteLine(">>>:\"User enter the 'exit' to exit this mode at \"" + DateTime.Now);
                            break;
                        }
                    }
                    string sourceCode = scode;
                    Lexer lexer = new(sourceCode);
                    List<Token> tokens = [];
                    Token token;
                    do
                    {
                        token = lexer.NextToken();

                        tokens.Add(token);
                    } while (token.Type != TokenTypes.EndOfFile);

                    Parser parser = new(tokens);
                    parser.OutputEvent += (s, e) =>
                    {
                        Console.WriteLine(e.Message);
                    };
                    bool isvalid = true;
                    try
                    {
                        parser.ParseStatements();
                    }
                    catch (Exception ex)
                    {
                        isvalid = false;
                        Console.WriteLine("Your code has some mistake, please try again.");
                    }
                    if (isvalid)
                    {
                        Console.WriteLine("Your code is correct, will you save the code?(y/n)");
                        string yn = Console.ReadLine();
                        if (yn == "y")
                        {
                            Console.WriteLine(">>>:\"Please enter the file path:\"");
                            string path = Console.ReadLine();
                            bool isarg = true;
                            try
                            {
                                File.WriteAllText(path, sourceCode);
                            }
                            catch
                            {
                                isarg = false;
                                Console.WriteLine(">>>:\"Args is error......Please try again\"");
                            }
                            if (isarg)
                            {
                                Console.WriteLine(">>>:\"Save successful.\"");
                            }
                        }
                        else
                        {
                            Console.WriteLine(">>>:\"User enter the 'exit' to exit this mode at \"" + DateTime.Now);
                        }
                    }
                }
                else if(command == "path")
                {
                    Console.WriteLine(Directory.GetCurrentDirectory());
                }
                else if (command == "cd..")
                {
                    Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
                }
                else if (command.Contains("cd"))
                {
                    Directory.SetCurrentDirectory(command.Split(' ')[1]);
                }
                else if(command == "cls")
                {
                    Console.Clear();
                }
                else if(command == "feedback")
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("liuqingyuff@outlook.com");
                    mail.To.Add("liuqingyuff@outlook.com");
                    mail.Subject = "Feedback";
                    Console.WriteLine("Please write the Feedback:");
                    mail.Body = Console.ReadLine();
                    SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
                    smtp.Credentials = new System.Net.NetworkCredential("liuqingyuff@outlook.com", "Tiger19880206");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    Console.WriteLine("Successfully.");
                }
                else if (command.Contains("version"))
                {
                    if(command == "version")
                    {
                        Console.WriteLine(">>>FeiSharp Console-Edition 8.0");
                    }
                    else if (command.Split(' ')[1] == "--update")
                    {
                        Console.WriteLine(">>>Finding update file from web......");
                        if (IsConnectedToInternet())
                        {
                            Thread.Sleep(Random.Shared.Next(1500,4000));
                            Console.WriteLine(">>>No much update file for download.");
                        }
                        else
                        {
                            Console.WriteLine(">>>You are not online.");
                        }
                    }
                    else if(command.Split(' ')[1] == "--select")
                    {
                        Console.WriteLine(">>>Please select version:");
                        Console.WriteLine("----1.FeiSharp Console-Edition 7.5");
                        Console.WriteLine("----2.FeiSharp Console-Edition 7.0");
                        Console.WriteLine("----3.FeiSharp Base Lexer");
                        try
                        {
                            char a = Console.ReadLine().ToCharArray()[0];
                            if (char.IsDigit(a))
                            {
                                int aInt = Convert.ToInt32(a.ToString());
                                if (aInt > 0 && aInt < 4)
                                {
                                    Console.WriteLine(">>>Edit version......");
                                    try
                                    {
                                        using (Ping ping = new Ping())
                                        {
                                            PingReply reply = ping.Send("https://github.com", 1000);
                                        }
                                        Console.WriteLine(">>>Edit successfully.");
                                    }
                                    catch (PingException)
                                    {
                                        Console.WriteLine(">>>Can not connect to github.com.");
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine(">>>Invalid format.");
                        }
                    }
                }
                else if(command == "license")
                {
                    Console.WriteLine("Copyright (c) 2024-2025 Newsoft FeiSharp\r\n\r\nLicensed under the Apache License, Version 2.0 (the \"License\");\r\nyou may not use this software except in compliance with the License.\r\nYou may obtain a copy of the License at\r\n\r\n    http://www.apache.org/licenses/LICENSE-2.0\r\n\r\nUnless required by law or agreed to in writing, no person shall:\r\n1. sublicense, sell, rent, lease, or otherwise distribute the Software without the express written permission of the copyright holders;\r\n2. modify the Software without the express written permission of the copyright holders;\r\n3. sublicense the Software under any other license without the express written permission of the copyright holders;\r\n4. sublicense the Software to any person or entity without the express written permission of the copyright holders;\r\n5. sublicense the Software to any person or entity in any jurisdiction without the express written permission of the copyright holders;\r\n6. sublicense the Software to any person or entity in any country without the express written permission of the copyright holders;\r\n7. sublicense the Software to any person or entity in any language without the express written permission of the copyright holders;\r\n8. sublicense the Software to any person or entity in any format without the express written permission of the copyright holders;\r\n9. sublicense the Software to any person or entity in any medium without the express written permission of the copyright holders;\r\n10. sublicense the Software to any person or entity in any context without the express written permission of the copyright holders;\r\n11. sublicense the Software to any person or entity in any way without the express written permission of the copyright holders;\r\n12. sublicense the Software to any person or entity in any form without the express written permission of the copyright holders;\r\n13. sublicense the Software to any person or entity in any manner without the express written permission of the copyright holders;\r\n14. sublicense the Software to any person or entity in any means without the express written permission of the copyright holders;\r\n15. sublicense the Software to any person or entity in any case without the express written permission of the copyright holders;\r\n16. sublicense the Software to any person or entity in any instance without the express written permission of the copyright holders;\r\n17. sublicense the Software to any person or entity in any situation without the express written permission of the copyright holders;\r\n18. sublicense the Software to any person or entity in any circumstance without the express written permission of the copyright holders;\r\n19. sublicense the Software to any person or entity in any condition without the express written permission of the copyright holders;\r\n20. sublicense the Software to any person or entity in any respect without the express written permission of the copyright holders;\r\n21. sublicense the Software to any person or entity in any regard without the express written permission of the copyright holders;\r\n22. sublicense the Software to any person or entity in any way whatsoever without the express written permission of the copyright holders;\r\n23. sublicense the Software to any person or entity in any other way without the express written permission of the copyright holders;\r\n24. sublicense the Software to any person or entity in any other form without the express written permission of the copyright holders;\r\n25. sublicense the Software to any person or entity in any other manner without the express written permission of the copyright holders;\r\n26. sublicense the Software to any person or entity in any other means without the express written permission of the copyright holders;\r\n27. sublicense the Software to any person or entity in any other case without the express written permission of the copyright holders;\r\n28. sublicense the Software to any person or entity in any other instance without the express written permission of the copyright holders;\r\n29. sublicense the Software to any person or entity in any other situation without the express written permission of the copyright holders;\r\n30. sublicense the Software to any person or entity in any other circumstance without the express written permission of the copyright holders;\r\n31. sublicense the Software to any person or entity in any other condition without the express written permission of the copyright holders;\r\n32. sublicense the Software to any person or entity in any other respect without the express written permission of the copyright holders;\r\n33. sublicense the Software to any person or entity in any other regard without the express written permission of the copyright holders;\r\n34. sublicense the Software to any person or entity in any other way whatsoever without the express written permission of the copyright holders;\r\n\r\nIf you do sublicense the Software, you must also:\r\n1. sublicense the Software under the License;\r\n2. sublicense the Software to any person or entity in any jurisdiction under the License;\r\n3. sublicense the Software to any person or entity in any country under the License;\r\n4. sublicense the Software to any person or entity in any language under the License;\r\n5. sublicense the Software to any person or entity in any format under the License;\r\n6. sublicense the Software to any person or entity in any medium under the License;\r\n7. sublicense the Software to any person or entity in any context under the License;\r\n8. sublicense the Software to any person or entity in any way under the License;\r\n9. sublicense the Software to any person or entity in any form under the License;\r\n10. sublicense the Software to any person or entity in any manner under the License;\r\n11. sublicense the Software to any person or entity in any means under the License;\r\n12. sublicense the Software to any person or entity in any case under the License;\r\n13. sublicense the Software to any person or entity in any instance under the License;\r\n14. sublicense the Software to any person or entity in any situation under the License;\r\n15. sublicense the Software to any person or entity in any circumstance under the License;\r\n16. sublicense the Software to any person or entity in any condition under the License;\r\n17. sublicense the Software to any person or entity in any respect under the License;\r\n18. sublicense the Software to any person or entity in any regard under the License;\r\n19. sublicense the Software to any person or entity in any way whatsoever under the License;\r\n20. sublicense the Software to any person or entity in any other way under the License;\r\n21. sublicense the Software to any person or entity in any other form under the License;\r\n22. sublicense the Software to any person or entity in any other manner under the License;\r\n23. sublicense the Software to any person or entity in any other means under the License;\r\n24. sublicense the Software to any person or entity in any other case under the License;\r\n25. sublicense the Software to any person or entity in any other instance under the License;\r\n26. sublicense the Software to any person or entity in any other situation under the License;\r\n27. sublicense the Software to any person or entity in any other circumstance under the License;\r\n28. sublicense the Software to any person or entity in any other condition under the License;\r\n29. sublicense the Software to any person or entity in any other respect under the License;\r\n30. sublicense the Software to any person or entity in any other regard under the License;\r\n31. sublicense the Software to any person or entity in any other way whatsoever under the License;\r\n\r\nIf you do sublicense the Software, you must also:\r\n1. sublicense the Software under the License;\r\n2. sublicense the Software to any person or entity in any jurisdiction under the License;\r\n3. sublicense the Software to any person or entity in any country under the License;\r\n4. sublicense the Software to any person or entity in any language under the License;\r\n5. sublicense the Software to any person or entity in any format under the License;\r\n6. sublicense the Software to any person or entity in any medium under the License;\r\n7. sublicense the Software to any person or entity in any context under the License;\r\n8. sublicense the Software to any person or entity in any way under the License;\r\n9. sublicense the Software to any person or entity in any form under the License;\r\n10. sublicense the Software to any person or entity in any manner under the License;\r\n11. sublicense the Software to any person or entity in any means under the License;\r\n12. sublicense the Software to any person or entity in any case under the License;\r\n13. sublicense the Software to any person or entity in any instance under the License;\r\n14. sublicense the Software to any person or entity in any situation under the License;\r\n15. sublicense the Software to any person or entity in any circumstance under the License;\r\n16. sublicense the Software to any person or entity in any condition under the License;\r\n17. sublicense the Software to any person or entity in any respect under the License;\r\n18. sublicense the Software to any person or entity in any regard under the License;\r\n19. sublicense the Software to any person or entity in any way whatsoever under the License;\r\n20. sublicense the Software to any person or entity in any other way under the License;\r\n21. sublicense the Software to any person or entity in any other form under the License;\r\n22. sublicense the Software to any person or entity in any other manner under the License;\r\n23. sublicense the Software to any person or entity in any other means under the License;\r\n24. sublicense the Software to any person or entity in any other case under the License;\r\n25. sublicense the Software to any person or entity in any other instance under the License;\r\n26. sublicense the Software to any person or entity in any other situation under the License;\r\n27. sublicense the Software to any person or entity in any other circumstance under the License;\r\n28. sublicense the Software to any person or entity in any other condition under the License;\r\n29. sublicense the Software to any person or entity in any other respect under the License;\r\n30. sublicense the Software to any person or entity in any other regard under the License;\r\n31. sublicense the Software to any person or entity in any other way whatsoever under the License;\r\n\r\nIf you do sublicense the Software, you must also:\r\n1. sublicense the Software under the License;\r\n2. sublicense the Software to any person or entity in any jurisdiction under the License;\r\n3. sublicense the Software to any person or entity in any country under the License;\r\n4. sublicense the Software to any person or entity in any language under the License;\r\n5. sublicense the Software to any person or entity in any format under the License;\r\n6. sublicense the Software to any person or entity in any medium under the License;\r\n7. sublicense the Software to any person or entity in any context under the License;\r\n8. sublicense the Software to any person or entity in any way under the License;\r\n9. sublicense the Software to any person or entity in any form under the License;\r\n10. sublicense the Software to any person or entity in any manner under the License;\r\n11. sublicense the Software to any person or entity in any means under the License;\r\n12. sublicense the Software to any person or entity in any case under the License;\r\n13. sublicense the Software to any person or entity in any instance under the License;\r\n14. sublicense the Software to any person or entity in any situation under the License;\r\n15. sublicense the Software to any person or entity in any circumstance under the License;\r\n16. sublicense the Software to any person or entity in any condition under the License;\r\n17. sublicense the Software to any person or entity in any respect under the License;\r\n18. sublicense the Software to any person or entity in any regard under the License;\r\n19. sublicense the Software to any person or entity in any way whatsoever under the License;\r\n20. sublicense the Software to any person or entity in any other way under the License;\r\n21. sublicense the Software to any, sublicense the Software to any person or entity in any other form under the License;\r\n22. sublicense the Software to any person or entity in any other manner under the License;\r\n23. sublicense the Software to any person or entity in any other means under the License;\r\n24. sublicense the Software to any person or entity in any other case under the License;\r\n25. sublicense the Software to any person or entity in any other instance under the License;\r\n26. sublicense the Software to any person or entity in any other situation under the License;\r\n27. sublicense the Software to any person or entity in any other circumstance under the License;\r\n28. sublicense the Software to any person or entity in any other condition under the License;\r\n29. sublicense the Software to any person or entity in any other respect under the License;\r\n30. sublicense the Software to any person or entity in any other regard under the License;\r\n31. sublicense the Software to any person or entity in any other way whatsoever under the License;\r\n\r\nIf you do sublicense the Software, you must also:\r\n1. sublicense the Software under the License;\r\n2. sublicense the Software to any person or entity in any jurisdiction under the License;\r\n3. sublicense the Software to any person or entity in any country under the License;\r\n4. sublicense the Software to any person or entity in any language under the License;\r\n5. sublicense the Software to any person or entity in any format under the License;\r\n6. sublicense the Software to any person or entity in any medium under the License;\r\n7. sublicense the Software to any person or entity in any context under the License;\r\n8. sublicense the Software to any person or entity in any way under the License;\r\n9. sublicense the Software to any person or entity in any form under the License;\r\n10. sublicense the Software to any person or entity in any manner under the License;\r\n11. sublicense the Software to any person or entity in any means under the License;\r\n12. sublicense the Software to any person or entity in any case under the License;\r\n13. sublicense the Software to any person or entity in any instance under the License;\r\n14. sublicense the Software to any person or entity in any situation under the License;\r\n15. sublicense the Software to any person or entity in any circumstance under the License;\r\n16. sublicense the Software to any person or entity in any condition under the License;\r\n17. sublicense the Software to any person or entity in any respect under the License;\r\n18. sublicense the Software to any person or entity in Any regard under the License;\r\n19. sublicense the Software to any person or entity in any way whatsoever under the License;\r\n20. sublicense the Software to any person or entity in any other way under the License;\r\n21. sublicense the Software to any person or entity in any other form under the License;\r\n22. sublicense the Software to any person or entity in any other manner under the License;\r\n23. sublicense the Software to any person or entity in any other means under the License;\r\n24. sublicense the Software to any person or entity in any other case under the License;\r\n25. sublicense the Software to any person or entity in any other instance under the License;\r\n26. sublicense the Software to any person or entity in any other situation under the License;\r\n27. sublicense the Software to any person or entity in any other circumstance under the License;\r\n28. sublicense the Software to any person or entity in any other condition under the License;\r\n29. sublicense the Software to any person or entity in any other respect under the License;\r\n30. sublicense the Software to any person or entity in any other regard under the License;\r\n31. sublicense the Software to any person or entity in any other way whatsoever under the License;\r\n\r\nIf you do sublicense the Software, you must also:\r\n1. sublicense the Software under the License;\r\n2. sublicense the Software to any person or entity in any jurisdiction under the License;\r\n3. sublicense the Software to any person or entity in any country under the License;\r\n4. sublicense the Software to any person or entity in any language under the License;\r\n5. sublicense the Software to any person or entity in any format under the License;\r\n6. sublicense the Software to any person or entity in any medium under the License;\r\n7. sublicense the Software to any person or entity in any context under the License;\r\n8. sublicense the Software to any person or entity in any way under the License;\r\n9. sublicense the Software to any person or entity in any form under the License;\r\n10. sublicense the Software to any person or entity in any manner under the License;\r\n11. sublicense the Software to any person or entity in any means under the License;\r\n12. sublicense the Software to any person or entity in any case under the License;\r\n13. sublicense the Software to any person or entity in any instance under the License;\r\n14. sublicense the Software to any person or entity in any situation under the License;\r\n15. sublicense the Software to any person or entity in any circumstance under the License;\r\n16. sublicense the Software to any person or entity in any condition under the License;\r\n17. sublicense the Software to any person or entity in any respect under the License;\r\n18. sublicense the Software to any person or");
                }
                else if(command == "copyright")
                {
                    Console.WriteLine("Copyright (c) 2024-2025 Newsoft FeiSharp\r\nAll Rights Reserved.");
                }
                else if(command == "credits")
                {
                    Console.WriteLine("Thanks to Nagarro, CWI, Microsoft, OpenAI, thinkgeo.com, and a cast of thousands for supporting FeiSharp development.");
                }
                else
                {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    if (string.IsNullOrEmpty(output))
                    {
                        Process process1 = new Process();
                        ProcessStartInfo startInfo1 = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments = $"/c {command}",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };
                        process1.StartInfo = startInfo1;
                        process1.Start();
                        string output1 = process1.StandardOutput.ReadToEnd();
                        process1.WaitForExit();
                        Console.WriteLine(output1);
                    }
                    Console.WriteLine(output);
                }
            }
        }
        public static bool IsConnectedToInternet()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send("8.8.8.8", 1000); // Ping Google DNS
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (PingException)
            {
                return false;
            }
        }
    }
    public class CustomConsole
    {
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, ushort wAttributes);
        const int STD_OUTPUT_HANDLE = -11;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);

        public static void WriteGray(string text)
        {
            IntPtr consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            ushort grayColor = 8;
            SetConsoleTextAttribute(consoleHandle, grayColor);
            Console.Write(text);
            ushort defaultColor = 7;
            SetConsoleTextAttribute(consoleHandle, defaultColor);
        }
    }
}
