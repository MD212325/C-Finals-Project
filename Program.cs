using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Application
{
    // =================================================================
    // CLASSES for da program busseng
    // =================================================================

    class MenuItem
    {
        public string Title { get; private set; }
        public Action Run { get; private set; }

        public MenuItem(string title, Action run)
        {
            Title = title;
            Run = run;
        }
    }

    class Student
    {
        public string Name { get; set; }
        public int Grade { get; set; }
        public string Remarks { get { return Grade >= 75 ? "PASSED" : "FAILED"; } }

        public Student(string name, int grade)
        {
            Name = name;
            Grade = grade;
        }
    }

    class Question
    {
        public string Text { get; private set; }
        public string[] Choices { get; private set; }
        public int Answer { get; private set; }      // 0-based index of the right choice

        public Question(string text, string[] choices, int answer)
        {
            Text = text;
            Choices = choices;
            Answer = answer;
        }
    }

    enum Hand { Rock, Paper, Scissors }
    static class UI
    {
        public const int Width = 62;                       // width of every box
        const ConsoleColor Bg = ConsoleColor.Black;        // change for another background

        // ---------- setup ----------
        public static void Init()
        {
            try { Console.OutputEncoding = Encoding.UTF8; } catch { }
            try { Console.Title = "C# Finals Output"; } catch { }
            Console.BackgroundColor = Bg;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
        }

        public static void Reset()
        {
            Console.BackgroundColor = Bg;
            Console.ForegroundColor = ConsoleColor.White;
        }

        // ---------- colored output ----------
        public static void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Reset();
        }

        public static void PrintLine(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Reset();
        }

        public static void Error(string message)   { PrintLine("  [!] " + message, ConsoleColor.Red); }
        public static void Success(string message) { PrintLine("  [OK] " + message, ConsoleColor.Green); }

        public static void Typewriter(string text, ConsoleColor color, int delay = 20)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
            Reset();
        }

        public static void LoadingBar(string label)
        {
            const int total = 30;
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i <= total; i++)
            {
                Console.Write("\r  " + label + " [" + new string('█', i) + new string('░', total - i)
                              + "] " + (i * 100 / total) + "%  ");
                Thread.Sleep(40);
            }
            Console.WriteLine();
            Reset();
        }

        // ---------- sounds (Console.Beep only works on Windows) ----------
        //            Could use LinuxBeep but it requires installation. ¯\_(ツ)_/¯
        public static void Sound(int frequency, int milliseconds)
        {
            try { Console.Beep(frequency, milliseconds); } catch (Exception) { }
        }
        public static void SoundClick()   { Sound(800, 40); }
        public static void SoundSuccess() { Sound(659, 110); Sound(784, 110); Sound(1047, 220); }
        public static void SoundError()   { Sound(220, 220); Sound(165, 320); }

        // ---------- graphics ----------
        static string Bar(char c) { return new string(c, Width - 2); }

        static string Center(string text, int width)
        {
            if (text.Length >= width) return text.Substring(0, width);
            int left = (width - text.Length) / 2;
            return new string(' ', left) + text + new string(' ', width - text.Length - left);
        }

        public static void Banner()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔" + Bar('═') + "╗");
            Console.WriteLine("║" + Center("G R O U P   N G   M G A   P O G I", Width - 2) + "║");
            Console.WriteLine("║" + Center("~ Console Application ~", Width - 2) + "║");
            Console.WriteLine("╚" + Bar('═') + "╝");
            Reset();
        }
        public static void Box(string title, ConsoleColor color, params string[] body)
        {
            int inner = Width - 2;
            Console.ForegroundColor = color;
            Console.WriteLine("╔" + Bar('═') + "╗");
            Console.WriteLine("║" + Center(title, inner) + "║");
            if (body.Length > 0)
            {
                Console.WriteLine("╠" + Bar('═') + "╣");
                foreach (string line in body)
                {
                    string text = "  " + line;
                    if (text.Length > inner) text = text.Substring(0, inner);
                    Console.WriteLine("║" + text.PadRight(inner) + "║");
                }
            }
            Console.WriteLine("╚" + Bar('═') + "╝");
            Reset();
        }

        /// A numbah menu
        public static void MenuBox(string title, ConsoleColor color, IList<string> items)
        {
            int inner = Width - 2;
            Console.ForegroundColor = color;
            Console.WriteLine("╔" + Bar('═') + "╗");
            Console.WriteLine("║" + Center(title, inner) + "║");
            Console.WriteLine("╠" + Bar('═') + "╣");
            Console.WriteLine("║" + new string(' ', inner) + "║");

            for (int i = 0; i < items.Count; i++)
            {
                string number = (i + 1).ToString();
                int used = 4 + number.Length + 2 + items[i].Length;
                int pad = Math.Max(0, inner - used);
                bool isLast = (i == items.Count - 1);

                Console.ForegroundColor = color;
                Console.Write("║   [");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(number);
                Console.ForegroundColor = color;
                Console.Write("] ");
                Console.ForegroundColor = isLast ? ConsoleColor.Red : ConsoleColor.White;
                Console.Write(items[i]);
                Console.ForegroundColor = color;
                Console.WriteLine(new string(' ', pad) + "║");
            }

            Console.WriteLine("║" + new string(' ', inner) + "║");
            Console.WriteLine("╚" + Bar('═') + "╝");
            Reset();
        }

        public static void PrintCentered(string text, ConsoleColor color)
        {
            PrintLine(Center(text, Width), color);
        }

        // ---------- input ----------
        public static string Prompt(string text)
        {
            Print(text, ConsoleColor.Yellow);
            return (Console.ReadLine() ?? "").Trim();
        }

        public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                string s = Prompt(prompt);
                int value;
                if (int.TryParse(s, out value) && value >= min && value <= max) return value;

                if (min == int.MinValue && max == int.MaxValue)
                    Error("Invalid input. Please enter a whole number.");
                else
                    Error("Invalid input. Please enter a whole number from " + min + " to " + max + ".");
            }
        }

        public static double ReadDouble(string prompt, double min = double.MinValue, double max = double.MaxValue)
        {
            while (true)
            {
                string s = Prompt(prompt);
                double value;
                if (double.TryParse(s, out value) && value >= min && value <= max) return value;

                if (min == double.MinValue && max == double.MaxValue)
                    Error("Invalid input. Please enter a number.");
                else
                    Error("Invalid input. Please enter a number from " + min + " to " + max + ".");
            }
        }

        /// Reads a password and shows * instead of the characters.
        public static string ReadPassword(string prompt)
        {
            Print(prompt, ConsoleColor.Yellow);
            var sb = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }

        public static void InvalidSelection()
        {
            Error("Invalid Selection. Please choose from the menu.");
            SoundError();
        }

        /// Keeps asking until the user types a number from 1 to max.
        public static int AskChoice(int max)
        {
            while (true)
            {
                Console.WriteLine();
                string s = Prompt("  Enter your choice: ");
                int value;
                if (int.TryParse(s, out value) && value >= 1 && value <= max)
                {
                    SoundClick();
                    return value;
                }
                InvalidSelection();
            }
        }

        /// "Try Again or Quit" prompt. true = Try Again, false = Quit.
        public static bool AskTryAgain()
        {
            Console.WriteLine();
            PrintLine("  " + new string('─', Width - 4), ConsoleColor.DarkGray);
            while (true)
            {
                Print("  [T] Try Again    [Q] Quit  >  ", ConsoleColor.Cyan);
                string c = (Console.ReadLine() ?? "").Trim().ToUpper();
                if (c == "T") { SoundClick(); return true; }
                if (c == "Q") { SoundClick(); return false; }
                InvalidSelection();
            }
        }

        public static void Pause(string message = "  Press any key to continue...")
        {
            Print(message, ConsoleColor.Gray);
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }

    // =================================================================
    //  BEYSIK FEATURES
    // =================================================================
    static class BasicFeatures
    {
        // ---- 1. Simple calculator ----
        public static void Calculator()
        {
            double a = UI.ReadDouble("  Enter first number    : ");

            string op;
            while (true)
            {
                op = UI.Prompt("  Operator ( + - * / % ): ");
                if (op.Length == 1 && "+-*/%".IndexOf(op) >= 0) break;
                UI.Error("Please enter only one of these: + - * / %");
            }

            double b = UI.ReadDouble("  Enter second number   : ");

            double result = 0;
            bool valid = true;
            switch (op)
            {
                case "+": result = a + b; break;
                case "-": result = a - b; break;
                case "*": result = a * b; break;
                case "/":
                    if (b == 0) valid = false; else result = a / b;
                    break;
                case "%":
                    if (b == 0) valid = false; else result = a % b;
                    break;
            }

            Console.WriteLine();
            if (valid)
            {
                UI.PrintLine("  " + a + " " + op + " " + b + " = " + Math.Round(result, 4), ConsoleColor.Green);
                UI.SoundSuccess();
            }
            else
            {
                UI.Error("Cannot divide by zero!");
                UI.SoundError();
            }
        }

        // ---- 2. Even or Odd ----
        public static void EvenOrOdd()
        {
            int n = UI.ReadInt("  Enter an integer: ");

            string parity = (n % 2 == 0) ? "EVEN" : "ODD";
            string sign = n > 0 ? "POSITIVE" : (n < 0 ? "NEGATIVE" : "ZERO");

            Console.WriteLine();
            UI.PrintLine("  " + n + " is an " + parity + " number.", ConsoleColor.Green);
            UI.PrintLine("  Sign: " + sign, ConsoleColor.Cyan);
        }

        // ---- 3. Multiplication table ----
        public static void MultiplicationTable()
        {
            int n = UI.ReadInt("  Enter a number (1 - 100): ", 1, 100);
            Console.WriteLine();
            UI.PrintLine("  ┌" + new string('─', 20) + "┐", ConsoleColor.Yellow);
            for (int i = 1; i <= 12; i++)
            {
                ConsoleColor c = (i % 2 == 0) ? ConsoleColor.Cyan : ConsoleColor.White;
                UI.PrintLine("  │ " + string.Format("{0,3} x {1,2} = {2,6}", n, i, n * i) + "  │", c);
            }
            UI.PrintLine("  └" + new string('─', 20) + "┘", ConsoleColor.Yellow);
        }

        // ---- 4. Grade evaluator ----
        public static void GradeEvaluator()
        {
            double score = UI.ReadDouble("  Enter your score (0 - 100): ", 0, 100);

            string letter, remark;
            ConsoleColor color;

            if (score >= 90)      { letter = "A"; remark = "Excellent!";        color = ConsoleColor.Green; }
            else if (score >= 80) { letter = "B"; remark = "Very Good!";        color = ConsoleColor.Cyan; }
            else if (score >= 70) { letter = "C"; remark = "Good.";             color = ConsoleColor.Yellow; }
            else if (score >= 60) { letter = "D"; remark = "Needs improvement."; color = ConsoleColor.DarkYellow; }
            else                  { letter = "F"; remark = "Failed. Study more!"; color = ConsoleColor.Red; }

            Console.WriteLine();
            UI.Box("GRADE REPORT", color,
                   "Score  : " + score,
                   "Letter : " + letter,
                   "Remark : " + remark);
            if (letter == "F") UI.SoundError(); else UI.SoundSuccess();
        }
    }

    // =================================================================
    //  INTERMIDYET FEATURES
    // =================================================================
    static class IntermediateFeatures
    {
        // ---- 1. Array statistics ----
        public static void ArrayStatistics()
        {
            int n = UI.ReadInt("  How many numbers (2 - 10)? ", 2, 10);
            int[] nums = new int[n];
            for (int i = 0; i < n; i++)
                nums[i] = UI.ReadInt("  Number #" + (i + 1) + ": ");

            int sum = 0, min = nums[0], max = nums[0];
            foreach (int x in nums)
            {
                sum += x;
                if (x < min) min = x;
                if (x > max) max = x;
            }
            double average = (double)sum / n;

            int[] ascending = (int[])nums.Clone();
            Array.Sort(ascending);
            int[] descending = (int[])ascending.Clone();
            Array.Reverse(descending);

            Console.WriteLine();
            UI.Box("ARRAY REPORT", ConsoleColor.Cyan,
                   "Original   : " + string.Join(", ", nums),
                   "Ascending  : " + string.Join(", ", ascending),
                   "Descending : " + string.Join(", ", descending),
                   "Sum        : " + sum,
                   "Average    : " + Math.Round(average, 2),
                   "Smallest   : " + min,
                   "Largest    : " + max);
        }

        // ---- 2. String tools ----
        public static void StringTools()
        {
            string text = UI.Prompt("  Enter a word or sentence: ");
            if (string.IsNullOrWhiteSpace(text))
            {
                UI.Error("You did not type anything.");
                return;
            }

            char[] chars = text.ToCharArray();
            Array.Reverse(chars);
            string reversed = new string(chars);

            int vowels = text.Count(c => "aeiouAEIOU".IndexOf(c) >= 0);
            int words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;

            string cleaned = new string(text.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
            bool palindrome = cleaned.Length > 0 && cleaned == new string(cleaned.Reverse().ToArray());

            Console.WriteLine();
            UI.Box("STRING REPORT", ConsoleColor.Cyan,
                   "Length     : " + text.Length,
                   "Words      : " + words,
                   "Vowels     : " + vowels,
                   "UPPERCASE  : " + text.ToUpper(),
                   "lowercase  : " + text.ToLower(),
                   "Reversed   : " + reversed,
                   "Palindrome : " + (palindrome ? "YES" : "NO"));
        }

        // ---- 3. Recursion: factorial and Fibonacci ----
        public static void RecursionDemo()
        {
            int n = UI.ReadInt("  Enter a number (1 - 20): ", 1, 20);

            // build "5 x 4 x 3 x 2 x 1"
            var parts = new List<string>();
            for (int i = n; i >= 1; i--) parts.Add(i.ToString());

            Console.WriteLine();
            UI.PrintLine("  Factorial  : " + n + "! = " + string.Join(" x ", parts) + " = " + Factorial(n),
                         ConsoleColor.Green);

            var fib = new List<long>();
            for (int i = 0; i < n; i++) fib.Add(Fibonacci(i));
            UI.PrintLine("  Fibonacci  : " + string.Join(", ", fib), ConsoleColor.Cyan);
        }

        static long Factorial(int n)
        {
            if (n <= 1) return 1;
            return n * Factorial(n - 1);
        }

        static long Fibonacci(int n)
        {
            if (n <= 1) return n;
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }

        // ---- 4. Student records ----
        public static void StudentRecords()
        {
            int count = UI.ReadInt("  How many students (1 - 8)? ", 1, 8);
            var students = new List<Student>();

            for (int i = 1; i <= count; i++)
            {
                Console.WriteLine();
                string name = UI.Prompt("  Student #" + i + " name  : ");
                if (name == "") name = "Student " + i;
                int grade = UI.ReadInt("  Student #" + i + " grade (0-100): ", 0, 100);
                students.Add(new Student(name, grade));
            }

            var ranked = students.OrderByDescending(s => s.Grade).ToList();

            Console.WriteLine();
            UI.PrintLine("  " + string.Format("{0,-6}{1,-21}{2,-8}{3}", "Rank", "Name", "Grade", "Remarks"),
                         ConsoleColor.Yellow);
            UI.PrintLine("  " + new string('─', 46), ConsoleColor.Gray);

            int rank = 1;
            foreach (Student s in ranked)
            {
                string name = s.Name.Length > 19 ? s.Name.Substring(0, 19) : s.Name;
                UI.Print("  " + string.Format("{0,-6}{1,-21}{2,-8}", rank++, name, s.Grade));
                UI.PrintLine(s.Remarks, s.Grade >= 75 ? ConsoleColor.Green : ConsoleColor.Red);
            }

            UI.PrintLine("  " + new string('─', 46), ConsoleColor.Gray);
            UI.PrintLine("  Class average : " + Math.Round(students.Average(s => s.Grade), 2), ConsoleColor.Cyan);
            UI.PrintLine("  Top student   : " + ranked[0].Name + " (" + ranked[0].Grade + ")", ConsoleColor.Green);
            UI.PrintLine("  Passed        : " + students.Count(s => s.Grade >= 75) + " of " + students.Count,
                         ConsoleColor.Cyan);
        }
    }

    // =================================================================
    //  EXTRA FEATURES
    // =================================================================
    static class ExtraFeatures
    {
        static readonly Random Rng = new Random();

        // ---- 1. Number guessing game ----
        public static void GuessTheNumber()
        {
            const int maxAttempts = 10;
            int secret = Rng.Next(1, 101);
            bool won = false;

            UI.PrintLine("  I'm thinking of a number from 1 to 100.", ConsoleColor.White);
            UI.PrintLine("  You have " + maxAttempts + " attempts. Good luck!\n", ConsoleColor.White);

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                int guess = UI.ReadInt("  Attempt " + attempt + "/" + maxAttempts + " - your guess: ", 1, 100);

                if (guess == secret)
                {
                    won = true;
                    break;
                }

                if (guess < secret) UI.PrintLine("  Too LOW!  Go higher.", ConsoleColor.Yellow);
                else                UI.PrintLine("  Too HIGH! Go lower.", ConsoleColor.Yellow);
                UI.Sound(500, 80);
            }

            Console.WriteLine();
            if (won)
            {
                UI.Box("YOU WIN!", ConsoleColor.Green, "Correct! The number was " + secret + ".");
                UI.SoundSuccess();
            }
            else
            {
                UI.Box("GAME OVER", ConsoleColor.Red, "Out of attempts. The number was " + secret + ".");
                UI.SoundError();
            }
        }

        // ---- 2. Trivia quiz ----
        static readonly List<Question> TriviaBank = new List<Question>
        {
            new Question("Who created the C# programming language?",
                new[] { "James Gosling", "Anders Hejlsberg", "Dennis Ritchie", "Guido van Rossum" }, 1),
            new Question("Which keyword declares a value that can never change?",
                new[] { "static", "readonly var", "const", "final" }, 2),
            new Question("What does Console.ReadLine() return?",
                new[] { "int", "string", "char", "void" }, 1),
            new Question("Which loop always runs at least once?",
                new[] { "for", "while", "foreach", "do-while" }, 3),
            new Question("Which symbol starts a single-line comment in C#?",
                new[] { "//", "#", "--", "/*" }, 0),
            new Question("What is the capital city of the Philippines?",
                new[] { "Cebu City", "Davao City", "Manila", "Baguio" }, 2),
            new Question("Which data type stores only true or false?",
                new[] { "int", "bool", "string", "double" }, 1),
            new Question("Which method is the entry point of a C# program?",
                new[] { "Start()", "Run()", "Main()", "Init()" }, 2)
        };

        public static void TriviaQuiz()
        {
            const int total = 5;
            var questions = TriviaBank.OrderBy(q => Rng.Next()).Take(total).ToList();
            int score = 0;

            for (int i = 0; i < questions.Count; i++)
            {
                Question q = questions[i];
                UI.PrintLine("  Q" + (i + 1) + ". " + q.Text, ConsoleColor.Cyan);
                for (int c = 0; c < q.Choices.Length; c++)
                    UI.PrintLine("      " + (c + 1) + ") " + q.Choices[c]);

                int answer = UI.ReadInt("  Your answer (1-4): ", 1, 4) - 1;
                if (answer == q.Answer)
                {
                    score++;
                    UI.Success("Correct!");
                    UI.Sound(900, 100);
                }
                else
                {
                    UI.Error("Wrong! The answer is: " + q.Choices[q.Answer]);
                    UI.Sound(250, 200);
                }
                Console.WriteLine();
            }

            string rating = score == total ? "Perfect! You're a genius!"
                          : score >= 3     ? "Good job!"
                                           : "Keep studying!";
            UI.Box("QUIZ RESULT", ConsoleColor.Yellow,
                   "Score  : " + score + " / " + total,
                   "Rating : " + rating);
            if (score >= 3) UI.SoundSuccess(); else UI.SoundError();
        }

        // ---- 3. Rock - Paper - Scissors ----
        public static void RockPaperScissors()
        {
            int you = 0, cpu = 0, round = 1;
            UI.PrintLine("  First to win 2 rounds wins the match!\n", ConsoleColor.White);

            while (you < 2 && cpu < 2)
            {
                UI.PrintLine("  ── Round " + round + " ── (You " + you + " : " + cpu + " CPU)", ConsoleColor.Magenta);
                UI.PrintLine("  [1] Rock   [2] Paper   [3] Scissors");

                Hand player = (Hand)(UI.ReadInt("  Your move: ", 1, 3) - 1);
                Hand computer = (Hand)Rng.Next(3);

                UI.PrintLine("  You    : " + player, ConsoleColor.Cyan);
                UI.PrintLine("  Cpu    : " + computer, ConsoleColor.Yellow);

                int diff = ((int)player - (int)computer + 3) % 3;   // 0 = tie, 1 = you win, 2 = cpu wins
                if (diff == 0)
                {
                    UI.PrintLine("  It's a TIE! Replay the round.\n", ConsoleColor.White);
                    UI.Sound(600, 100);
                    continue;
                }
                if (diff == 1)
                {
                    you++;
                    UI.PrintLine("  You win this round!\n", ConsoleColor.Green);
                    UI.Sound(900, 100);
                }
                else
                {
                    cpu++;
                    UI.PrintLine("  Computer wins this round!\n", ConsoleColor.Red);
                    UI.Sound(250, 150);
                }
                round++;
            }

            if (you > cpu)
            {
                UI.Box("YOU WON THE MATCH!", ConsoleColor.Green, "Final score  You " + you + " : " + cpu + " CPU");
                UI.SoundSuccess();
            }
            else
            {
                UI.Box("COMPUTER WON THE MATCH", ConsoleColor.Red, "Final score  You " + you + " : " + cpu + " CPU");
                UI.SoundError();
            }
        }

        // ---- 4. Hangman ----
        static readonly string[] HangmanWords =
        {
            "ESMERINGHOY", "BUSSENG", "LOLA NA NAKA DOLPHIN SHORTS",
            "LIBOG", "BADAP", "WALLY BAYOLA"
        };

        public static void Hangman()
        {
            const int maxWrong = 6;
            string word = HangmanWords[Rng.Next(HangmanWords.Length)];
            var guessed = new List<char>();
            int wrong = 0;
            bool won = false;

            while (wrong < maxWrong && !won)
            {
                Console.Clear();
                if (word.Any(char.IsWhiteSpace))
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        if (char.IsWhiteSpace(word[i]))
                        {
                            guessed.Add(' ');  // Automatically guess spaces
                        }
                    }
                }
                UI.Box("HANGMAN", ConsoleColor.Yellow, "Guess the letters of the word before the man is hanged!");
                DrawGallows(wrong);

                string display = string.Join(" ", word.Select(c => guessed.Contains(c) ? c : '_'));
                UI.PrintLine("    Word  : " + display, ConsoleColor.Cyan);
                switch (word) //HINTS
                {
                    case "ESMERINGHOY": UI.PrintLine("    Hint  : Bembangan ba", ConsoleColor.Gray); break;
                    case "BUSSENG": UI.PrintLine("    Hint  : Wala pa bang kanin?", ConsoleColor.Gray); break;
                    case "LOLA NA NAKA DOLPHIN SHORTS": UI.PrintLine("    Hint  : Masarap", ConsoleColor.Gray); break;
                    case "LIBOG": UI.PrintLine("    Hint  : Lagi mong nararamdaman", ConsoleColor.Gray); break;
                    case "BADAP": UI.PrintLine("    Hint  : Malakas na kaba", ConsoleColor.Gray); break;
                    case "WALLY BAYOLA": UI.PrintLine("    Hint  : Kalbo", ConsoleColor.Gray); break;
                }
                UI.PrintLine("    Wrong : " + wrong + "/" + maxWrong + "   Guessed: " + string.Join(" ", guessed.Where(c => !char.IsWhiteSpace(c))),
                             ConsoleColor.Gray);                                                    // I don't get this line pero gumagana kaya wag na tong hawakan :)

                if (display.IndexOf('_') < 0) { won = true; break; }

                Console.WriteLine();
                string input = UI.Prompt("  Guess a letter: ").ToUpper();
                if (input.Length != 1 || !char.IsLetter(input[0]))
                {
                    UI.Error("Please type a single letter.");
                    Thread.Sleep(800);
                    continue;
                }

                char letter = input[0];
                if (guessed.Contains(letter))
                {
                    UI.Error("You already guessed '" + letter + "'.");
                    Thread.Sleep(800);
                    continue;
                }

                guessed.Add(letter);
                if (word.IndexOf(letter) < 0)
                {
                    wrong++;
                    UI.Sound(250, 150);
                }
                else
                {
                    UI.Sound(900, 80);
                }
            }

            Console.WriteLine();
            if (won)
            {
                UI.Box("YOU SAVED HIM!", ConsoleColor.Green, "The word was " + word + ".");
                UI.SoundSuccess();
            }
            else
            {
                DrawGallows(wrong);
                UI.Box("GAME OVER", ConsoleColor.Red, "The word was " + word + ".");
                UI.SoundError();
            }
        }

        static void DrawGallows(int wrong)
        {
            string head = wrong >= 1 ? "O" : " ";
            string arms = (wrong >= 3 ? "/" : " ") + (wrong >= 2 ? "|" : " ") + (wrong >= 4 ? "\\" : " ");
            string legs = (wrong >= 5 ? "/" : " ") + " " + (wrong >= 6 ? "\\" : " ");

            UI.PrintLine("      +---+", ConsoleColor.Yellow);
            UI.PrintLine("      |   |", ConsoleColor.Yellow);
            UI.PrintLine("      " + head + "   |", ConsoleColor.Yellow);
            UI.PrintLine("     " + arms + "  |", ConsoleColor.Yellow);
            UI.PrintLine("     " + legs + "  |", ConsoleColor.Yellow);
            UI.PrintLine("          |", ConsoleColor.Yellow);
            UI.PrintLine("    =========", ConsoleColor.Yellow);
        }
    }

    // =================================================================
    //  PROGRAM
    // =================================================================
    class Program
    {
        static readonly bool QuitReturnsToSubMenu = false;

        static void Main()
        {
            UI.Init();

            if (!Login()) return;      // 3 wrong attempts -> program ends

            MainMenu();
            Goodbye();
        }

        // ---------------------------------------------------------
        //  LOGIN lowkey chopped implementation but we ball
        // ---------------------------------------------------------
        static bool Login()
        {
            const string correctUser = "user";  // Could use hashing algorithm with Argon2id
            const string correctPass = "pass";  // Kaso apaka sakit sa ulo i implement ;-;
            const int maxAttempts = 3;

            string[] lockArt =
            {
                "     ╭───────╮     ",
                "     │ ╭───╮ │     ",
                "     │ │   │ │     ",
                "  ╔══╧═╧═══╧═╧══╗  ",
                "  ║             ║  ",
                "  ║      ●      ║  ",
                "  ║      ┃      ║  ",
                "  ╚═════════════╝  "
            };

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                Console.Clear();
                UI.Banner();
                foreach (string line in lockArt)
                    UI.PrintCentered(line, ConsoleColor.Yellow);
                UI.Box("USER LOGIN", ConsoleColor.Cyan,
                       "Please enter your credentials.",
                       "Attempt " + attempt + " of " + maxAttempts);
                Console.WriteLine();

                string user = UI.Prompt("  Username : ");
                string pass = UI.ReadPassword("  Password : ");

                if (user == correctUser && pass == correctPass)
                {
                    Console.WriteLine();
                    UI.SoundSuccess();
                    UI.Typewriter("  Access granted. Welcome, " + user + "!", ConsoleColor.Green);
                    UI.LoadingBar("Loading system");
                    Thread.Sleep(400);
                    return true;
                }

                Console.WriteLine();
                UI.SoundError();
                int left = maxAttempts - attempt;
                if (left > 0)
                {
                    UI.Error("Incorrect username or password. Attempts left: " + left);
                    UI.Pause();
                }
            }

            Console.WriteLine();
            UI.Box("ACCESS DENIED", ConsoleColor.Red, "Too many failed attempts.", "The system is now locked.");
            UI.Pause("  Press any key to close...");
            return false;
        }

        // ---------------------------------------------------------
        //  MEYN MENYU
        // ---------------------------------------------------------
        static void MainMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                UI.Banner();
                UI.MenuBox("M A I N   M E N U", ConsoleColor.Cyan, new[]
                {
                    "Basic          - C# fundamentals",
                    "Intermediate   - arrays, strings, classes",
                    "Extra          - games & entertainment",
                    "EXIT"
                });

                switch (UI.AskChoice(4))
                {
                    case 1: RunSubMenu("B A S I C   M E N U", ConsoleColor.Green, BasicItems()); break;
                    case 2: RunSubMenu("I N T E R M E D I A T E   M E N U", ConsoleColor.Yellow, IntermediateItems()); break;
                    case 3: RunSubMenu("E X T R A   M E N U", ConsoleColor.Magenta, ExtraItems()); break;
                    case 4: running = false; break;
                }
            }
        }

        static List<MenuItem> BasicItems()
        {
            return new List<MenuItem>
            {
                new MenuItem("Simple Calculator",      BasicFeatures.Calculator),
                new MenuItem("Even or Odd Checker",    BasicFeatures.EvenOrOdd),
                new MenuItem("Multiplication Table",   BasicFeatures.MultiplicationTable),
                new MenuItem("Grade Evaluator",        BasicFeatures.GradeEvaluator)
            };
        }

        static List<MenuItem> IntermediateItems()
        {
            return new List<MenuItem>
            {
                new MenuItem("Array Statistics",             IntermediateFeatures.ArrayStatistics),
                new MenuItem("String Tools",                 IntermediateFeatures.StringTools),
                new MenuItem("Factorial & Fibonacci",        IntermediateFeatures.RecursionDemo),
                new MenuItem("Student Records (Classes)",    IntermediateFeatures.StudentRecords)
            };
        }

        static List<MenuItem> ExtraItems()
        {
            return new List<MenuItem>
            {
                new MenuItem("Guess the Number",       ExtraFeatures.GuessTheNumber),
                new MenuItem("Trivia Quiz",            ExtraFeatures.TriviaQuiz),
                new MenuItem("Rock Paper Scissors",    ExtraFeatures.RockPaperScissors),
                new MenuItem("Hangman",                ExtraFeatures.Hangman)
            };
        }

        // ---------------------------------------------------------
        //  SUB MENU all basic inter and extra features are run from here :3
        // ---------------------------------------------------------
        static void RunSubMenu(string title, ConsoleColor color, List<MenuItem> items)
        {
            while (true)
            {
                Console.Clear();
                UI.Banner();

                List<string> labels = items.Select(i => i.Title).ToList();
                labels.Add("Back to Main Menu");
                UI.MenuBox(title, color, labels);

                int choice = UI.AskChoice(labels.Count);
                if (choice == labels.Count) return;           // Back to Main Menu

                MenuItem selected = items[choice - 1];

                do
                {
                    Console.Clear();
                    UI.Box(selected.Title.ToUpper(), color);
                    Console.WriteLine();
                    selected.Run();                           // run the chosen application
                } while (UI.AskTryAgain());

                if (!QuitReturnsToSubMenu) return;
            }
        }

        // ---------------------------------------------------------
        //  GOODBYE SCREEN
        // ---------------------------------------------------------
        static void Goodbye()
        {
            Console.Clear();
            UI.Banner();
            UI.Box("THANK YOU PO BUSSENG!", ConsoleColor.Magenta,
                   "Program terminated successfully.",
                   "Goodbye and see you again!");
            // DonutAnimation.Run(); currently not working will reimplement when fixed !!!HIGH PRIORITY FIX!!!
            UI.Sound(1047, 150);
            UI.Sound(784, 150);
            UI.Sound(659, 150);
            UI.Sound(523, 300);
            Console.WriteLine();
            UI.Pause("  Press any key to close...");

        }
    }

    // -----------------------------------------------------------------
    //  DONUT ANIMATION di ko gets tangina di parin gumagana !!!HIGH PRIORITY FIX!!!
    // -----------------------------------------------------------------

    // static class DonutAnimation
    // {
    //     const int Cols = 60;     // screen width in characters
    //     const int Rows = 22;     // screen height in characters

    //     public static void Run()
    //     {
    //         const double R1 = 1.0;     
    //         const double R2 = 2.0;    
    //         const double K2 = 5.0; 
    //         const double K1x = 30.0;
    //         const double K1y = 15.0;
    //         string shades = ".,-~:;=!*#$@";

    //         double A = 0, B = 0;

    //         // Throw away any key presses still waiting, so the donut doesn't stop immediately
    //         while (Console.KeyAvailable) Console.ReadKey(true);

    //         Console.Clear();
    //         try { Console.CursorVisible = false; } catch { }   

    //         Console.SetCursorPosition(0, Rows);
    //         UI.PrintLine("  Thank you!", ConsoleColor.Cyan);
    //         UI.Print("  Press any key to close...", ConsoleColor.Gray);

    //         char[] output = new char[Cols * Rows];          
    //         double[] zbuffer = new double[Cols * Rows];

    //         while (!Console.KeyAvailable)                 
    //         {
    //             for (int i = 0; i < output.Length; i++)
    //             {
    //                 output[i] = ' ';
    //                 zbuffer[i] = 0;
    //             }

    //             double cosA = Math.Cos(A), sinA = Math.Sin(A);
    //             double cosB = Math.Cos(B), sinB = Math.Sin(B);

    //             for (double theta = 0; theta < 2 * Math.PI; theta += 0.07)
    //             {
    //                 double cosT = Math.Cos(theta), sinT = Math.Sin(theta);

    //                 for (double phi = 0; phi < 2 * Math.PI; phi += 0.02)
    //                 {
    //                     double cosP = Math.Cos(phi), sinP = Math.Sin(phi);

    //                     double circleX = R2 + R1 * cosT;
    //                     double circleY = R1 * sinT;

    //                     double x = circleX * (cosB * cosP + sinA * sinB * sinP) - circleY * cosA * sinB;
    //                     double y = circleX * (sinB * cosP - sinA * cosB * sinP) + circleY * cosA * cosB;
    //                     double z = K2 + cosA * circleX * sinP + circleY * sinA;
    //                     double ooz = 1 / z; 

    //                     int xp = (int)(Cols / 2 + K1x * ooz * x);
    //                     int yp = (int)(Rows / 2 - K1y * ooz * y); 

    //                     double L = cosP * cosT * sinB - cosA * cosT * sinP - sinA * sinT
    //                              + cosB * (cosA * sinT - cosT * sinA * sinP);

    //                     if (xp >= 0 && xp < Cols && yp >= 0 && yp < Rows)
    //                     {
    //                         int index = xp + Cols * yp;  
    //                         if (ooz > zbuffer[index])
    //                         {
    //                             zbuffer[index] = ooz;
    //                             int shade = L > 0 ? (int)(L * 8) : 0;  
    //                             output[index] = shades[Math.Min(shade, shades.Length - 1)];
    //                         }
    //                     }
    //                 }
    //             }

    //             var frame = new StringBuilder();
    //             for (int row = 0; row < Rows; row++)
    //             {
    //                 frame.Append(output, row * Cols, Cols);
    //                 if (row < Rows - 1) frame.Append(Environment.NewLine);
    //             }
    //             Console.SetCursorPosition(0, 0); 
    //             UI.Print(frame.ToString(), ConsoleColor.Yellow);

    //             A += 0.07;                 
    //             B += 0.03;
    //             Thread.Sleep(30);         
    //         }

    //         Console.ReadKey(true);        
    //         try { Console.CursorVisible = true; } catch { }
    //         Console.Clear();
    //     }
    // }
}