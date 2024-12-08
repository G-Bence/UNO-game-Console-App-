using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Security.Policy;
using System.Threading;

namespace CLASS_241115
{
    internal class Program
    {

        static string[] cards_color = { "Red", "Blue", "Green", "Yellow" };
        static string[] cards_nums = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        static string[] special_cards = { "Pick a color card", "Draw 2 card", "Draw 4 card", "Skip" };

        static Random rnd = new Random();

        static bool win;
        static bool Skip;
        
        static string new_game;


        static void Main(string[] args)
        {
            do {
                int starter_value = 7;

                win = false;

                string[,] your_cards = new string[112, 2]; //108 is the deck size but I add 4 extra pleaces to avoid error if the deck would fill up
                string[,] opp_cards = new string[112, 2];
                string[,] deck = new string[108, 2];

                Console.WriteLine("[________________UNO________________]");


                //Making your deck:
                your_cards = Draw_Cards(your_cards, starter_value);
                your_cards = Sort_2D(your_cards);


                //Making your opponent's deck:
                opp_cards = Draw_Cards(opp_cards, starter_value);
                opp_cards = Sort_2D(opp_cards);



                int index = 0;
                int round = 0;
                string winner = "";

                do
                {
                    string[] opp_chosen = new string[2];
                    string[] your_chosen = new string[2];
                    int rand_chose = 0;

                    if ((index + 2) >= deck.GetLength(0))
                    {
                        int deck_ammount = 0;
                        for (int i = 0; i < deck.GetLength(0); i++)
                        {
                            if (deck[i, 0] != null && deck[i, 0].Length > 0)
                            {
                                deck_ammount++;
                            }
                        }
                        string last_color = deck[deck_ammount, 0];
                        string last_num = deck[deck_ammount, 1];

                        Array.Clear(deck, 0, index);

                        deck[0, 0] = last_color;
                        deck[0, 1] = last_num;
                        index = 1;
                    }

                    if (Is_it_full(opp_cards))
                    {
                        winner = "You";
                        break;
                    }

                    if (Is_it_full(your_cards))
                    {
                        winner = "Your opponent";
                        break;
                    }

                    Console.WriteLine($"\n\n------------{round,3} ROUND! ------------");


                    //Choosing your card:
                    if (!Skip)
                    {
                        int your_amount = 0;

                        int answer = 0;

                        string[] your_color = new string[your_cards.GetLength(0)];
                        string[] your_numbs = new string[your_cards.GetLength(0)];

                        bool is_special = false;

                        for (int i = 0; i < your_cards.GetLength(0); i++)
                        {
                            if (your_cards[i, 0] != null && your_cards[i, 0].Length > 0)
                            {
                                if (special_cards.Contains(your_color[i]))
                                {
                                    is_special = true;
                                }
                                your_color[i] = your_cards[i, 0];
                                your_numbs[i] = your_cards[i, 1];
                                your_amount++;
                            }

                        }

                        if (your_amount == 0)
                        {
                            winner = "You";
                            break;
                        }

                        else if ((your_color.Contains(deck[index, 0])) || (your_numbs.Contains(deck[index, 1])) || is_special)
                        {
                            Console.WriteLine("___________________________________");
                            Console.WriteLine("Your current cards: ");

                            for (int i = 0; i < your_cards.GetLength(0); i++)
                            {
                                if (your_cards[i, 0] != null && your_cards[i, 0].Length > 0)
                                {
                                    Console.WriteLine($"\t-{i + 1}: {your_cards[i, 0]} {your_cards[i, 1]}");
                                }
                            }
                            Console.WriteLine("\nPlease write the number of chosen card: ");

                            do
                            {
                                try
                                {
                                    answer = Convert.ToInt32(Console.ReadLine());
                                    if (answer > your_amount || answer <= 0)
                                    {
                                        throw new IndexOutOfRangeException();
                                    }

                                    if (answer.GetType() != typeof(int))
                                    {
                                        throw new FormatException();
                                    }

                                }


                                catch (Exception)
                                {
                                    bool Validity = false;
                                    do
                                    {
                                        try
                                        {
                                            Console.WriteLine($"\nInvalid value!\nPlease write the number of chosen card (between 1 and {your_amount}): ");
                                            answer = Convert.ToInt32(Console.ReadLine());


                                            if (answer > your_amount || answer <= 0)
                                            {
                                                throw new IndexOutOfRangeException();

                                            }
                                            else if (answer.GetType() != typeof(int))
                                            {
                                                throw new FormatException();
                                            }
                                            else
                                            {
                                                Validity = true;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }

                                    } while (!Validity);
                                }

                                answer--;

                                your_chosen[0] = your_cards[answer, 0];
                                your_chosen[1] = your_cards[answer, 1];

                                if ((your_chosen[0] != deck[index, 0] && your_chosen[1] != deck[index, 1] && !special_cards.Contains(your_chosen[0])) && index != 0)
                                {
                                    Console.WriteLine("\nInvalid Card!\nPlease choose a card which has color or number equals to the previous card in the deck!");
                                }
                            } while ((your_chosen[0] != deck[index, 0] && your_chosen[1] != deck[index, 1] && !special_cards.Contains(your_chosen[0])) && index != 0);

                            if (round != 0)
                            {
                                index++;
                            }

                            switch (your_chosen[0])
                            {
                                case "Pick a color card":
                                    deck = Pick_color(deck, index, true);
                                    break;

                                case "Draw 2 card":
                                    opp_cards = Draw_Cards(opp_cards, 2);
                                    deck[index, 0] = your_cards[answer, 0];
                                    deck[index, 1] = your_cards[answer, 1];
                                    break;

                                case "Draw 4 card":
                                    opp_cards = Draw_Cards(opp_cards, 4);
                                    deck[index, 0] = your_cards[answer, 0];
                                    deck[index, 1] = your_cards[answer, 1];
                                    break;

                                case "Skip":
                                    Skip = true;
                                    deck[index, 0] = your_cards[answer, 0];
                                    deck[index, 1] = your_cards[answer, 1];
                                    break;

                                default:
                                    deck[index, 0] = your_cards[answer, 0];
                                    deck[index, 1] = your_cards[answer, 1];
                                    break;
                            }

                            your_cards = Remover_2D(your_cards, your_chosen);

                            Console.WriteLine($"\nYour chosen card: \n{your_chosen[0]} {your_chosen[1]}");
                            Console.WriteLine("___________________________________");
                        }

                        else
                        {
                            your_cards = Draw_Cards(your_cards, 2);
                            your_cards = Sort_2D(your_cards);

                            Console.WriteLine("\nSorry!\nYou hadn't an appropriate card, so you had to draw 2!\n");
                            Console.WriteLine("___________________________________");
                            Console.WriteLine("Your current cards: ");

                            for (int i = 0; i < your_cards.GetLength(0); i++)
                            {
                                if (your_cards[i, 0] != null && your_cards[i, 0].Length > 0)
                                {
                                    Console.WriteLine($"\t-{i + 1}: {your_cards[i, 0]} {your_cards[i, 1]}");
                                    your_amount++;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("You skip this round, sorry!");
                        Skip = false;
                    }


                    Console.WriteLine("\n\n");


                    //Choosing opponent's card:
                    if (!Skip)
                    {

                        int opp_amount = 0;

                        string[] opp_color = new string[opp_cards.GetLength(0)];
                        string[] opp_numbs = new string[opp_cards.GetLength(0)];

                        bool is_special = false;

                        for (int i = 0; i < opp_cards.GetLength(0); i++)
                        {
                            if (opp_cards[i, 0] != null && opp_cards[i, 0].Length > 0)
                            {
                                if (special_cards.Contains(opp_color[i]))
                                {
                                    is_special = true;
                                }
                                opp_color[i] = opp_cards[i, 0];
                                opp_numbs[i] = opp_cards[i, 1];
                                opp_amount++;
                            }

                        }

                        if (opp_amount == 0)
                        {
                            winner = "Your opponent!";
                            break;
                        }

                        if ((opp_color.Contains(deck[index, 0])) || (opp_numbs.Contains(deck[index, 1])) || is_special)
                        {
                            do
                            {
                                try
                                {
                                    rand_chose = rnd.Next(rnd.Next(0, opp_amount));
                                }

                                catch (IndexOutOfRangeException)
                                {
                                    Console.WriteLine("ERROR: Your opponent is out of the range");
                                    do
                                    {
                                        rand_chose = rnd.Next(rnd.Next(0, opp_amount));
                                    } while (opp_cards[rand_chose, 0] != null);
                                }

                                opp_chosen[0] = opp_cards[rand_chose, 0];
                                opp_chosen[1] = opp_cards[rand_chose, 1];

                            } while ((opp_chosen[0] != deck[index, 0] && opp_chosen[1] != deck[index, 1]) && !special_cards.Contains(your_chosen[0]));

                            index++;

                            switch (opp_chosen[0])
                            {
                                case "Pick a color card":
                                    deck = Pick_color(deck, index, true);
                                    break;

                                case "Draw 2 card":
                                    your_cards = Draw_Cards(your_cards, 2);
                                    deck[index, 0] = opp_cards[rand_chose, 0];
                                    deck[index, 1] = opp_cards[rand_chose, 1];
                                    break;

                                case "Draw 4 card":
                                    your_cards = Draw_Cards(your_cards, 4);
                                    deck[index, 0] = opp_cards[rand_chose, 0];
                                    deck[index, 1] = opp_cards[rand_chose, 1];
                                    break;

                                case "Skip":
                                    Skip = true;
                                    deck[index, 0] = opp_cards[rand_chose, 0];
                                    deck[index, 1] = opp_cards[rand_chose, 1];
                                    break;

                                default:
                                    deck[index, 0] = opp_cards[rand_chose, 0];
                                    deck[index, 1] = opp_cards[rand_chose, 1];
                                    break;
                            }

                            opp_cards = Remover_2D(opp_cards, opp_chosen);

                            Console.WriteLine($"Your opponent chose: {opp_chosen[0]} {opp_chosen[1]}");
                            Console.WriteLine("___________________________________");
                        }

                        else
                        {
                            opp_cards = Draw_Cards(opp_cards, 2);
                            opp_cards = Sort_2D(opp_cards);
                            Console.WriteLine("Your opponent hadn't an appropriate card, so it had to draw 2!");
                            Console.WriteLine("___________________________________");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Your opponents skips this round!");
                        Skip = false;
                    }

                    Console.WriteLine($"\n\nLast card on the deck: {deck[index, 0]}, {deck[index, 1]}");

                    round++;

                } while (!win);

                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("\n###################################\n");


                if (winner == "You")
                {
                    Console.WriteLine("Congratuation!\nYou won the game!");

                    ProcessStartInfo NotRickRoll = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/k curl ascii.live/rick",
                        UseShellExecute = true,
                        CreateNoWindow = false
                    };
                    Process process = Process.Start(NotRickRoll);

                }
                else
                {
                    Console.WriteLine("Your opponent won the game, but don't be discouraged!");
                }
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("\nWould you like to start a new game?");
                new_game = Console.ReadLine().ToUpper();
                

            } while (new_game.Contains("Y") || new_game.Contains("I"));


            Console.WriteLine("\nThank you for playing!\nI hope you enjoyed =)");

            Console.ReadLine();
        }



        static string[,] Sort_2D(string[,] array)
        {

            List<string> Card_list = new List<string>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                string element = "";
                if (array[i, 0] != null && array[i, 0].Length > 0)
                {
                    element += $"{array[i, 0]}|{array[i, 1]}";
                    Card_list.Add(element);
                }

            }

            Card_list.Sort();

            Array.Clear(array, 0, array.GetLength(0));

            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (i < Card_list.Count)
                {
                    string[] element_array = Card_list[i].Split('|');
                    array[i, 0] = element_array[0];
                    array[i, 1] = element_array[1];
                }
            }
            return array;
        }


        static string[,] Remover_2D(string[,] array, string[] removable)
        {
            int founded = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (array[i, 0] == removable[0] && array[i, 1] == removable[1] && founded == 0)
                {
                    array[i, 1] = null;
                    array[i, 0] = null;
                    founded++;
                }
            }

            array = Sort_2D(array);

            return array;
        }


        static string[,] Draw_Cards(string[,] array, int draw_num)
        {
            int fulls = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if ((array[i, 0] != null && array[i, 0].Length > 0) && array[i, 1] != null && array[i, 1].Length > 0)
                {
                    fulls++;
                }
            }

            for (int i = fulls; i < (fulls + draw_num); i++)
            {
                try
                {
                    if (rnd.Next(0, 27) == 0)
                    {
                        array[i, 0] = special_cards[rnd.Next(0, special_cards.Length)];
                        array[i, 1] = "";
                    }
                    else
                    {
                        array[i, 0] = cards_color[rnd.Next(0, cards_color.Length)];
                        array[i, 1] = Convert.ToString(cards_nums[rnd.Next(0, cards_nums.Length)]);
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("\nERROR!\nYOUR ARRAY HAS FILLED!");
                    win = true;
                    break;

                }
            }
            return array;
        }


        static string[,] Pick_color(string[,] deck, int index, bool is_human)
        {
            string answer = "";
            if (is_human)
            {
                try
                {
                    Console.WriteLine("Please enter the color of card: ");
                    answer = Console.ReadLine().ToLower();
                    if (!cards_color.Contains(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(answer)))
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    bool Validity = false;
                    do
                    {
                        try
                        {
                            Console.WriteLine($"\nInvalid value!\nPlease write the color of card that you want to put your opponent!");
                            answer = Console.ReadLine().ToLower();


                            if (!cards_color.Contains(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(answer)))
                            {
                                throw new IndexOutOfRangeException();
                            }
                            else if (answer.GetType() != typeof(string))
                            {
                                throw new FormatException();
                            }
                            else
                            {
                                Validity = true;
                            }
                        }
                        catch (Exception)
                        {
                        }

                    } while (!Validity);
                }
                finally
                {
                    deck[index, 0] = answer;
                    deck[index, 1] = "";
                }
            }
            else
            {
                answer = cards_color[rnd.Next(0, cards_color.Length)];
                deck[index, 0] = answer;
                deck[index, 1] = "";
            }
            return deck;
        }


        static bool Is_it_full(string[,] deck)
        {
            bool full = false;
            int counter = 0;
            for(int i = 0; i < deck.GetLength(0); i++)
            {
                if (deck[i, 0] != null && deck[i, 0].Length > 0)
                {
                    counter++;
                }
            }
            if (counter >= (deck.GetLength(0) - 4))
            {
                full = true;
            }
            return full;
        }
    }
}
