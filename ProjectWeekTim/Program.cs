using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectWeekTim
{
    static class Globals
    {
        public static int ID;
        public static string Username;
        public static char[] Password;
        public static int Money;
    }
    class Program
    {
        static Dictionary<string, string> symbols = new Dictionary<string, string>
        {
           { "Club", "\u2663" },
           { "Spade", "\u2660" },
           { "Diamond", "\u2666" },
           { "Heart", "\u2665" },
           { "Square", "\u1758"}
        };

        static void Main()
        {
            Globals.ID = -1;
            string filename = "UserData.txt";
            if (!File.Exists(filename))
            {
                using (FileStream fs = File.Create(filename)) { }
            }
            Console.OutputEncoding = Encoding.UTF8;

            switch (MainMenu())
            {

                case ConsoleKey.NumPad1:
                    {
                        CreateNewUser();
                        Play();
                    }
                    break;
                case ConsoleKey.NumPad2:
                    {
                        EditUser();
                    }
                    break;
                case ConsoleKey.NumPad3:
                    {
                        DeleteUser();
                    }
                    break;
                case ConsoleKey.NumPad4:
                    {
                        while (!Login()) ;
                        Play();
                    }
                    break;
            }
        }
        static ConsoleKey MainMenu()
        {
            SetTitle("Main Menu");
            Console.WriteLine("1. Gebruiker toevoegen");
            Console.WriteLine("2. Gebruiker bewerken");
            Console.WriteLine("3. Gebruiker verwijderen");
            Console.WriteLine("4. Inloggen");
            ConsoleKey input = Console.ReadKey().Key;
            Console.Clear();
            return input;
        }
        static void SetTitle(string title)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = default;
            for (int i = 0; i < title.Length; i++)
            {
                Console.Write(" ");
            }
            Console.Write(title);
            for (int i = 0; i < title.Length; i++)
            {
                Console.Write(" ");
            }
            Console.BackgroundColor = ConsoleColor.Gray;
            if (Globals.ID == -1) Console.WriteLine();
            else Console.WriteLine(Globals.Username + "\t$" + Globals.Money + "\t" + DateTime.Now.ToString("HH:mm"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = default;
            Console.WriteLine();
        }

        static void CreateNewUser()
        {
            SetTitle("Nieuwe gebruiker aanmaken");

            Globals.Username = CreateUsername();
            Globals.Password = CreatePassword();
            Globals.Money = 200;
            SaveUserData(Globals.ID, Globals.Username, Globals.Password, Globals.Money, true);
            Console.WriteLine($"Welkom {Globals.Username}! Uw startkapitaal is ${Globals.Money}. Good luck!");
            Console.WriteLine("Druk op een knop om verder te gaan.");
            Console.ReadKey();

        }
        static string CreateUsername()
        {
            string username, usernameCheck;
            string[] userData;
            string extractor;
            string comparison;
            bool exists;
            do
            {
                exists = false;
                Console.WriteLine("Geef een username in.");
                username = Console.ReadLine();
                usernameCheck = Validate(username, true);
                userData = File.ReadAllLines("UserData.txt");
                if (usernameCheck != "")
                {
                    Console.WriteLine(usernameCheck);
                }
                if (!string.IsNullOrWhiteSpace(username) && userData.Length != 0)
                {
                    for (int i = 0; i < userData.Length; i++)
                    {
                        int index = userData[i].IndexOf('#');
                        extractor = userData[i].Substring(0, index).ToLower();
                        comparison = extractor.ToLower();
                        string usernameToLower = username.ToLower();
                        if (comparison == usernameToLower)
                        {
                            exists = true;
                        }
                    }
                }
                if (exists)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Username bestaat al.");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }
            while (exists || usernameCheck != ""); ;
            return username;
        }
        static char[] CreatePassword()
        {
            string password, passwordCheck;
            do
            {
                Console.WriteLine("Geef een wachtwoord in. (8-20 karakters waarvan minstens 1 hoofdletter, 1 kleine letter, 1 cijfer en 1 speciaal teken)");
                password = Console.ReadLine();
                passwordCheck = Validate(password, false);
                Console.WriteLine(passwordCheck);
            }
            while (passwordCheck != "");
            char[] encryptedpw = EncryptPassword(password);
            return encryptedpw;
        }
        static string Validate(string input, bool isUsername)
        {
            string userOrPassword;
            string message = string.Empty;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasMinMaxChars = new Regex(@".{8,20}");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            if (!isUsername)
            {
                userOrPassword = "Wachtwoord";

                if (!hasLowerChar.IsMatch(input))
                {
                    message = $"{userOrPassword} moet minstens 1 kleine letter bevatten.";
                }
                else if (!hasUpperChar.IsMatch(input))
                {
                    message = $"{userOrPassword} moet minstens 1 grote letter bevatten.";
                }
                else if (!hasMinMaxChars.IsMatch(input))
                {
                    message = $"{userOrPassword} moet minstens 8 karakters en maximum 20 karakters groot zijn.";
                }
                else if (!hasNumber.IsMatch(input))
                {
                    message = $"{userOrPassword} moet minstens 1 nummer bevatten.";
                }

                else if (!hasSymbols.IsMatch(input))
                {
                    message = $"{userOrPassword} moet minstens 1 speciaal teken bevatten.";
                }
                else
                {
                    message = "";
                }
            }
            else
            {
                userOrPassword = "Username";
                if (hasSymbols.IsMatch(input))
                {
                    message = $"{userOrPassword} mag geen speciale tekens bevatten.";
                }
            }
            if (input.Contains(" "))
            {
                Console.WriteLine($"{userOrPassword} mag geen spaties bevatten.");
            }
            else if (string.IsNullOrWhiteSpace(input))
            {
                message = $"{userOrPassword} mag niet leeg zijn";
            }
            return message;
        }
        static char[] EncryptPassword(string password)
        {
            char[] encryptedpw = password.ToCharArray();
            for (int i = 0; i < password.Length; i++)
            {
                encryptedpw[i]++;
            }
            return encryptedpw;
        }
        static void EditUser()
        {
            SetTitle("Gebruiker bewerken");

            while (!Login()) ;
            Console.WriteLine("1. Username veranderen");
            Console.WriteLine("2. Wachtwoord veranderen");
            ConsoleKey input = Console.ReadKey().Key;
            switch (input)
            {
                case ConsoleKey.NumPad1:
                    {
                        Console.WriteLine("Geef een nieuwe username in");
                        Globals.Username = CreateUsername();
                    }
                    break;
                case ConsoleKey.NumPad2:
                    {
                        Console.WriteLine("Geef een nieuw wachtwoord in");
                        Globals.Password = CreatePassword();
                    }
                    break;
            }
            SaveUserData(Globals.ID, Globals.Username, Globals.Password, Globals.Money, false);
            Console.WriteLine("Gegevens veranderd.");
            Console.WriteLine("Druk op een knop om door te gaan.");
            Console.ReadKey();
            Main();

        }
        static void SaveUserData(int userID, string username, char[] password, int money, bool newUser)
        {
            string[] users = File.ReadAllLines("UserData.txt");
            string pw = new string(password);
            string newData = string.Empty;
            if (newUser)
            {
                Globals.ID = users.Length;
                using (StreamWriter writer = new StreamWriter("UserData.txt", append: true)) //append source: https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-streamwriter
                {
                    newData = $"{username}#{pw}#{money}";
                    writer.WriteLine(newData);
                }
            }
            else
            {
                newData = $"{username}#{pw}#{money}";
                users[Convert.ToInt32(userID)] = newData;
                File.WriteAllLines("UserData.txt", users);
            }
        }
        static void DeleteUser()
        {
            while (!Login()) ;
            Console.WriteLine("Bent u zeker dat u uw account wilt verwijderen? [Y/N]");
            ConsoleKey input = Console.ReadKey().Key;
            if (input == ConsoleKey.Y)
            {
                List<string> users = File.ReadAllLines("UserData.txt").ToList();
                users.RemoveAt(Globals.ID);
                File.WriteAllLines("UserData.txt", users.ToArray());
                Console.WriteLine("\nAccount verwijderd. U wordt doorverwezen naar het hoofdmenu.");
                Thread.Sleep(2000);
                Main();
            }
            else
            {
                Console.WriteLine("\nBedankt om te blijven. U wordt doorverwezen naar het spelkeuzemenu.");
                Thread.Sleep(2000);
                Play();
            }


        }
        static bool Login()
        {
            Console.WriteLine("Geef uw username in.");
            string username = Console.ReadLine();
            Console.WriteLine("Geef uw wachtwoord in.");
            string password = Console.ReadLine();
            char[] encryptedpw = EncryptPassword(password);
            int money = 0;
            SetTitle("Log in");
            string data, dataName;
            char[] dataPassword = new char[encryptedpw.Length];
            bool comparison = false;
            string[] users = File.ReadAllLines("UserData.txt");
            bool exists = false;
            for (int i = 0; i < users.Length; i++)
            {
                data = users[i];
                int index = data.IndexOf('#');
                dataName = data.Substring(0, index);
                if (dataName.ToLower() == username.ToLower())
                {
                    Globals.ID = i;
                    data = data.Remove(0, index + 1);
                    index = data.IndexOf('#');
                    dataPassword = (data.Substring(0, index)).ToCharArray();
                    data = data.Remove(0, index + 1);
                    money = Convert.ToInt32(data);
                    exists = true;
                }
            }
            if (!exists)
            {
                Console.WriteLine("Username bestaat niet.");
                return false;
            }
            if (encryptedpw.Length >= dataPassword.Length)
            {
                for (int i = 0; i < dataPassword.Length; i++)
                {
                    if (dataPassword[i] == encryptedpw[i])
                    {
                        comparison = true;
                    }
                    else
                    {
                        comparison = false;
                        break;
                    }
                }
            }
            if (comparison)
            {
                Console.WriteLine("Succes! Druk op een knop om verder te gaan.");
                Globals.Username = username;
                Globals.Password = encryptedpw;
                Globals.Money = money;
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.WriteLine("Wachtwoord incorrect. Probeer opnieuw.");
                return false;
            }

        }

        static void Play()
        {
            SetTitle("Spelkeuze");
            Console.WriteLine("Wat wilt u spelen?");
            Console.WriteLine("1.BlackJack ($10)");
            Console.WriteLine("2.Slot Machine ($5)");
            Console.WriteLine("3.Memory ($20)");
            ConsoleKey input;
            do
            {
                input = Console.ReadKey().Key;
                if (input != ConsoleKey.NumPad1 && input != ConsoleKey.NumPad2 && input != ConsoleKey.NumPad3)
                {
                    Console.WriteLine("U heeft geen geldige input gegeven.");
                }
            }
            while (input != ConsoleKey.NumPad1 && input != ConsoleKey.NumPad2 && input != ConsoleKey.NumPad3);
            ChooseGame(input);
        }
        static void ChooseGame(ConsoleKey input)
        {
            switch (input)
            {
                case ConsoleKey.NumPad1:
                    {
                        BlackJack();
                    }
                    break;
                case ConsoleKey.NumPad2:
                    {
                        SlotMachine();
                    }
                    break;
                case ConsoleKey.NumPad3:
                    {
                        Memory();
                    }
                    break;
            }
        }

        static void BlackJack()
        {
            StartGame("BlackJack");
            Pay(10);

            int[] deck = Shuffle(52, 4);
            int counterPlayer = 2;
            int counterDealer;
            int totalPlayer;
            int totalDealer;
            ConsoleKey input = ConsoleKey.A;
            totalPlayer = Deal(deck, 0, counterPlayer);
            do
            {
                if (totalPlayer == 21 && input == ConsoleKey.A)
                {
                    Victory(25);
                    break;
                }
                Console.WriteLine("1. Nog een kaart trekken.");
                Console.WriteLine("2. Stoppen");
                input = Console.ReadKey().Key;
                if (input == ConsoleKey.NumPad1) counterPlayer++;
                SetTitle("BlackJack");
                totalPlayer = Deal(deck, 0, counterPlayer);

            }
            while (input == ConsoleKey.NumPad1 && totalPlayer < 21);

            if (totalPlayer == 21 && input == ConsoleKey.NumPad1)
            {
                Victory(20);
            }
            else if (totalPlayer > 21)
            {
                Console.WriteLine("U hebt verloren!");
            }
            if (input == ConsoleKey.NumPad2)
            {
                counterDealer = 1;
                totalDealer = Deal(deck, counterPlayer, counterDealer);

                if (totalDealer > 21 || totalPlayer > totalDealer)
                {
                    Victory(20);
                }
                else if (totalDealer == totalPlayer)
                {
                    Victory(10);
                }
                else Console.WriteLine("U hebt verloren!");
            }
            EndGame("BlackJack");
        }
        static void SlotMachine()
        {

            StartGame("SlotMachine");
            Pay(5);
            int[] list = Shuffle(63, 9);
            int value = 0;
            string symbol;

            for (int i = 0; i < 9; i++)
            {
                if (i % 3 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("[");
                    symbol = GetSymbol(list[i]);
                    Console.Write(symbol + " ");
                }
                else if (i % 3 == 2)
                {
                    symbol = GetSymbol(list[i]);
                    Console.Write(symbol);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("]");

                }
                else
                {
                    symbol = GetSymbol(list[i]);
                    Console.Write(symbol + " ");
                }
            }
            if (list[0] == list[1] && list[0] == list[2])
            {
                value += GetValue(list[0]);
            }
            if (list[4] == list[0] && list[4] == list[8])
            {
                value += GetValue(list[4]);
            }
            if (list[4] == list[3] && list[4] == list[5])
            {
                value += GetValue(list[4]);
            }
            if (list[4] == list[2] && list[4] == list[6])
            {
                value += GetValue(list[4]);
            }
            if (list[6] == list[7] && list[6] == list[8])
            {
                value += GetValue(list[6]);
            }
            if (value != 0)
            {
                Victory(value);
            }
            else Defeat();

            EndGame("SlotMachine");
        }
        static void Memory()
        {
            StartGame("Memory");
            Pay(20);
            int[] list = Shuffle(10, 2);
            DrawMemory(list);
            Console.WriteLine("Onthoud!");
            int[] known = new int[list.Length];
            Thread.Sleep(5000);

            SetTitle("Memory");
            DrawMemory(known);

            int teller = 0;
            int inputIndex = 0;
            ConsoleKey input = ConsoleKey.A;
            int temp = -1;
            bool winning = false;
            do
            {
                for (int i = 0; i < 2; i++)
                {
                    do
                    {
                        Console.Write($"Keuze {i + 1}: ");
                        input = Console.ReadKey().Key;
                        inputIndex = ConvertInput(input);
                        if (known[inputIndex] != 0)
                        {
                            Console.WriteLine(" Deze optie is al gekozen.");
                        }

                        if (temp == list[inputIndex])
                        {
                            winning = true;
                        }
                        else
                        {
                            temp = list[inputIndex];
                            winning = false;
                        }

                    }
                    while (known[inputIndex] != 0);
                    known[inputIndex] = list[inputIndex];
                    SetTitle("Memory");
                    DrawMemory(known);
                }
                teller += 2;
                if (winning && teller != 10) Console.WriteLine("Goed bezig!");
                else if (teller == 10) Victory(30);
                else Defeat();
            }
            while (winning && teller < 10);
            EndGame("Memory");


        }

        static int[] Shuffle(int total, int columns)
        {
            //source: https://stackoverflow.com/questions/35973361/fill-array-with-random-but-unique-numbers-in-c-sharp
            int[,] list = new int[columns, total / columns];
            int[] shuffledList = new int[total];
            int next;
            Random r = new Random();

            for (int i = 0; i < columns; i++)
            {
                int[] row = new int[total / columns];
                for (int j = 0; j < total / columns; j++)
                {
                    while (true)
                    {
                        next = r.Next(0, (total / columns) + 1);
                        if (!row.Contains(next)) break;
                    }
                    row[j] = next;
                    list[i, j] = row[j];
                }

            }
            int teller = 0;
            for (int i = 0; i < total / columns; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    shuffledList[teller] = list[j, i];
                    teller++;
                }

            }
            return shuffledList;

        }
        private static string GetSymbol(int value)
        {
            switch (value)
            {
                case 0:
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        return "?";
                    }
                case 1:
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        return symbols["Heart"];
                    }
                case 2:
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        return symbols["Club"];
                    }
                case 3:
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        return symbols["Spade"];
                    }
                case 4:
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        return symbols["Diamond"];
                    }
                case 5:
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        return "A";
                    }
                case 6:
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        return "J";
                    }
                case 7:
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        return "7";
                    }
                default: return string.Empty;
            }
        }
        static void DrawMemory(int[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Console.Write(" " + i + "\t");

            }
            Console.WriteLine();
            for (int i = 0; i < list.Length; i++)
            {
                string symbol = GetSymbol(list[i]);
                Console.Write($"[{symbol}] \t");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\n");
        }
        static void StartGame(string gamemode)
        {

            if (Console.ForegroundColor != ConsoleColor.Yellow)
            {
                SetTitle(gamemode);
                Console.WriteLine($"Welkom bij {gamemode}! Druk op een knop om te beginnen.");
                Console.ReadKey();
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            SetTitle(gamemode);

        }
        static void EndGame(string game)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1. Opnieuw spelen");
            Console.WriteLine("2. Ander spel kiezen");
            Console.WriteLine("3. Stoppen");
            ConsoleKey input = Console.ReadKey().Key;
            SetTitle(game);
            switch (input)
            {
                case ConsoleKey.NumPad1:
                    {
                        switch (game)
                        {
                            case "BlackJack":
                                BlackJack();
                                break;
                            case "SlotMachine":
                                SlotMachine();
                                break;
                            case "Memory":
                                Memory();
                                break;
                        }
                        break;
                    }
                case ConsoleKey.NumPad2:
                    {
                        Play();
                        break;
                    }
                case ConsoleKey.NumPad3:
                    {
                        Console.WriteLine($"Bedankt om te spelen, {Globals.Username}! Uw huidige balans is ${Globals.Money}. Tot ziens!");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                    }


            }
            StartGame(game);
        }
        static int Deal(int[] deck, int startPosition, int counter)
        {
            if (startPosition == 0)
            {
                Console.WriteLine("Speler:");
            }

            else
            {
                Console.WriteLine("Dealer:");
            }

            int total = 0;
            int value;
            string card;
            for (int i = startPosition; i < startPosition + counter; i++)
            {
                value = deck[i];
                switch (value)
                {
                    case 1:
                        card = "A";
                        break;
                    case 11:
                        card = "J";
                        break;
                    case 12:
                        card = "Q";
                        break;
                    case 13:
                        card = "K";
                        break;
                    default:
                        card = value.ToString();
                        break;
                }
                if (value > 10)
                {
                    value = 10;
                }
                else if (value == 1)
                {
                    value = 11;
                }
                Console.WriteLine($"Kaart {i + 1 - startPosition}: {card}");
                total += value;

                if (startPosition != 0)
                {
                    counter++;
                    Thread.Sleep(500);
                    if (total >= 17) break;
                }
            }
            int[] drawn = new int[counter];
            int aceCounter = 0;
            for (int j = 0; j < counter; j++)
            {
                drawn[j] = deck[j + startPosition];
                if (drawn[j] == 1)
                {
                    aceCounter++;
                }
            }
            if (aceCounter != 0)
            {
                for (int j = 0; j < aceCounter; j++)
                {
                    if (total > 21)
                    {
                        total -= 10;
                    }
                }
            }

            Console.Write($"Totaal: ");
            if (total <= 21)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(total);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            return total;
        }
        static int GetValue(int number)
        {
            int value = 0;
            switch (number)
            {
                case 1:
                    {
                        value = 3;
                        break;
                    }
                case 2:
                    {
                        value = 5;
                        break;
                    }
                case 3:
                    {
                        value = 7;
                        break;
                    }
                case 4:
                    {
                        value = 10;
                        break;
                    }
                case 5:
                    {
                        value = 20;
                        break;
                    }
                case 6:
                    {
                        value = 50;
                        break;
                    }
                case 7:
                    {
                        value = 100;
                        break;
                    }
            }
            return value;
        }
        static int ConvertInput(ConsoleKey input)
        {
            int converted = 0;
            switch (input)
            {
                case ConsoleKey.NumPad0:
                    {
                        converted = 0;
                        break;
                    }
                case ConsoleKey.NumPad1:
                    {
                        converted = 1;
                        break;
                    }
                case ConsoleKey.NumPad2:
                    {
                        converted = 2;
                        break;
                    }
                case ConsoleKey.NumPad3:
                    {
                        converted = 3;
                        break;
                    }
                case ConsoleKey.NumPad4:
                    {
                        converted = 4;
                        break;
                    }
                case ConsoleKey.NumPad5:
                    {
                        converted = 5;
                        break;
                    }
                case ConsoleKey.NumPad6:
                    {
                        converted = 6;
                        break;
                    }
                case ConsoleKey.NumPad7:
                    {
                        converted = 7;
                        break;
                    }
                case ConsoleKey.NumPad8:
                    {
                        converted = 8;
                        break;
                    }
                case ConsoleKey.NumPad9:
                    {
                        converted = 9;
                        break;
                    }
            }
            return converted;
        }
        static void Victory(int value)
        {
            Console.WriteLine($"U hebt ${value} gewonnen!");
            Globals.Money += value;
            SaveUserData(Globals.ID, Globals.Username, Globals.Password, Globals.Money, false);

        }
        static void Defeat()
        {
            Console.WriteLine("U hebt verloren!");
            SaveUserData(Globals.ID, Globals.Username, Globals.Password, Globals.Money, false);
        }

        static void Pay(int payment)
        {
            if (Globals.Money >= payment)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("-$" + payment);
                Console.ForegroundColor = ConsoleColor.Gray;
                Globals.Money -= payment;
            }
            else Loan();

        }
        static void Loan()
        {
            SetTitle("Niet genoeg geld!");
            Console.WriteLine("U heeft niet genoeg geld om verder te spelen.");
            ConsoleKey input = ConsoleKey.A;
            do
            {
                Console.WriteLine("Wilt u een lening aangaan van $200?[Y/N]");
                input = Console.ReadKey().Key;
                if (input == ConsoleKey.Y)
                {
                    Globals.Money += 200;
                    Console.WriteLine($"\nUw huidige balans is nu ${Globals.Money}.");
                    Console.WriteLine("Druk op een knop om door te gaan naar het spelkeuzemenu");
                    Console.ReadKey();
                    Play();
                }
                else if (input == ConsoleKey.N)
                {
                    Console.WriteLine($"\nTot ziens, {Globals.Username}!");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else Console.WriteLine("Geen geldige input.");
            }
            while (input != ConsoleKey.Y && input != ConsoleKey.N);

        }
    }
}

