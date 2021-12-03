using System;
using System.Data;
using System.Runtime.Loader;
using VkBotFramework;
using VkBotFramework.Models;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Model.Keyboard;
using System.Data.SQLite;
using System.IO;
using System.Reflection.Emit;
using VkNet.Enums.SafetyEnums;

namespace ServerAgregator
{
    public class Program
    {
        private static SQLiteConnection _connection;
        private static SQLiteDataReader _reader;
        private static SQLiteCommand _command;

        private static string accessToken = "82245a591340d96dd1c0ae108441550b88698bace68001d0ed91f57545efd144ce10c8e2a53d872a0cadf";

        private static string groupUrl = "https://vk.com/ucheckproject";
        private static VkBot _bot;

        private static string name;
        private static string link;
        private static string status = "start";


        static public void dbCreate()
        {
            if (!File.Exists(@"Database/statusDb.db"))
            {
                SQLiteConnection.CreateFile(@"Database/statusDb.db");
            }
        }


        static public bool dbConnect(string fileName)
        {
            try
            {
                _connection = new SQLiteConnection("Data Source=" + fileName);
                _connection.Open();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}");
                return false;
            }
        }
        


        public static void Main(string[] args)
        {
            dbCreate();
            if (dbConnect(@"Database/statusDb.db"))
            {
                _command = new SQLiteCommand(_connection)
                {
                    CommandText =
                        "CREATE TABLE IF NOT EXISTS PersonalData (id INTEGER , name TEXT, link TEXT, status TEXT, message TEXT, sendMsg TEXT);"
                };
                _command.ExecuteNonQuery();

                _connection.Close();
            }

            _bot = new VkBot(accessToken, groupUrl);
            //_bot = new VkBot(accessToken, groupUrl);
            Console.WriteLine("Bot started.\n");
            _bot.OnMessageReceived += BotOnOnMessageReceived;
            _bot.Start();
        }


        private static void BotOnOnMessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            var chatId = e.Message.PeerId;
            status = "";
            _command = new SQLiteCommand();
            dbConnect(@"Database/statusDb.db");

            {
                _command = new SQLiteCommand(_connection)
                {
                    CommandText =
                        $"SELECT id FROM [PersonalData] WHERE id={e.Message.PeerId};"
                };

                Console.WriteLine(_command.CommandText);
                _command.ExecuteNonQuery();
                _reader = _command.ExecuteReader();
                var value = _reader.Read();
                _reader.Close();
                if (value == false)
                {
                    _command.CommandText =
                        $"INSERT INTO [PersonalData] VALUES ('{e.Message.PeerId}','','', 'start','','');";
                    _command.ExecuteNonQuery();
                }

                _command.CommandText = $"SELECT * FROM [PersonalData] WHERE id={e.Message.PeerId};";
                _command.ExecuteNonQuery();
                _reader = _command.ExecuteReader();
                _reader.Read();
                status = _reader.GetValue(3).ToString();
                _reader.Close();
            }


            if (status == "start")
            {
                KeyboardBuilder keyStart = new KeyboardBuilder();
                keyStart.AddButton("!Запустить", "", KeyboardButtonColor.Positive);
                keyStart.AddButton("!Заполнить", "", KeyboardButtonColor.Negative);
                keyStart.AddButton("!Данные", "", KeyboardButtonColor.Primary);
                MessageKeyboard keyboardStart = keyStart.Build();
                Console.WriteLine($"{e.Message.Text}\n");
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message =
                        "Выбери, что хотел бы сделать:\n1) Запустить бота\n2) Заполнить данные\n3) Показать имеющиеся данные",
                    PeerId = chatId,
                    RandomId = Environment.TickCount,
                    Keyboard = keyboardStart
                });

                _command.CommandText = $"UPDATE PersonalData SET status = 'refill' WHERE id = {e.Message.PeerId};";
                _command.ExecuteNonQuery();
            }
            else if (e.Message.Text == "!Заполнить" && (status == "refill"))
            {
                KeyboardBuilder keyfill = new KeyboardBuilder();
                keyfill.AddButton("!Стоп", "", KeyboardButtonColor.Negative);
                MessageKeyboard keyboardFill = keyfill.Build();
                Console.WriteLine($"{e.Message.Text}\n");
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message =
                        "Напиши имя для подключения к конференции(то, что ты укажешь здесь, будет выбрано в качестве имени на конференции).",
                    PeerId = chatId,
                    RandomId = Environment.TickCount,
                    Keyboard = keyboardFill
                });

                _command.CommandText = $"UPDATE PersonalData SET status = 'name' WHERE id = {e.Message.PeerId};";
                _command.ExecuteNonQuery();
            }
            else if (status == "name" && e.Message.Text != "!Стоп" && e.Message.Text != "!Данные")
            {
                KeyboardBuilder keyfill = new KeyboardBuilder();
                keyfill.AddButton("!Стоп", "", KeyboardButtonColor.Negative);
                MessageKeyboard keyboardFill = keyfill.Build();
                Console.WriteLine($"{e.Message.Text}\n");
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message = "Имя есть, а теперь введи ссылку на конференцию Zoom.",
                    PeerId = chatId,
                    RandomId = Environment.TickCount,
                    Keyboard = keyboardFill
                });
                _command.CommandText = $"UPDATE PersonalData SET status = 'link' WHERE id = {e.Message.PeerId};";
                _command.ExecuteNonQuery();
                _command.CommandText =
                    $"UPDATE PersonalData SET name = '{e.Message.Text}' WHERE id = {e.Message.PeerId};";
                _command.ExecuteNonQuery();
            }
            else if (status == "link" && e.Message.Text != "!Стоп" && e.Message.Text != "!Данные")
            {
                KeyboardBuilder keyLink = new KeyboardBuilder();
                keyLink.AddButton("!Стоп", "", KeyboardButtonColor.Negative);
                MessageKeyboard keyboardlink = keyLink.Build();
                if (e.Message.Text != "")
                {
                    if (e.Message.Text.Contains("zoom.us") || e.Message.Text.Contains("Zoom.us"))
                    {
                        Console.WriteLine($"{e.Message.Text}\n");
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message =
                                "Ссылка есть, а теперь введи сообщение, которое будет написано в чат по команде !Отправить сообщение.",
                            PeerId = chatId,
                            RandomId = Environment.TickCount,
                            Keyboard = keyboardlink
                        });


                        _command.CommandText =
                            $"UPDATE PersonalData SET status = 'message' WHERE id = {e.Message.PeerId};";
                        _command.ExecuteNonQuery();
                        _command.CommandText =
                            $"UPDATE PersonalData SET link = '{e.Message.Text}' WHERE id = {e.Message.PeerId};";
                        _command.ExecuteNonQuery();
                    }
                    else
                    {
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message = "Ссылка не принадлежит Zoom\nПопробуй снова либо напиши !Стоп",
                            PeerId = chatId,
                            RandomId = Environment.TickCount,
                            Keyboard = keyboardlink
                        });
                    }
                }


                else if (e.Message.Attachments.Count != 0 && e.Message.Attachments[0].Type == typeof(Link))
                {
                    Attachment link1 = new Attachment();


                    link1 = e.Message.Attachments[0];
                    link = link1.Instance.ToString();

                    if (link.Contains("zoom.us") || link.Contains("Zoom.us"))
                    {
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message =
                                $"Ссылка есть, а теперь введи сообщение, которое будет написано в чат по команде !Отправить сообщение.",
                            PeerId = chatId,
                            RandomId = Environment.TickCount,
                            Keyboard = keyboardlink
                        });
                        _command.CommandText =
                            $"UPDATE PersonalData SET status = 'message' WHERE id = {e.Message.PeerId};";
                        _command.ExecuteNonQuery();
                        _command.CommandText =
                            $"UPDATE PersonalData SET link = '{link}' WHERE id = {e.Message.PeerId};";
                        _command.ExecuteNonQuery();
                    }
                    else
                    {
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message = "Ссылка не принадлежит Zoom\nПопробуй снова либо напиши !Стоп",
                            PeerId = chatId,
                            RandomId = Environment.TickCount,
                            Keyboard = keyboardlink
                        });
                    }
                }
            }
            else if (status == "message" && e.Message.Text != "!Стоп" && e.Message.Text != "!Данные")
            {
                KeyboardBuilder keyMessage = new KeyboardBuilder();
                keyMessage.AddButton("Дальше", "", KeyboardButtonColor.Positive);
                MessageKeyboard keyboardMessage = keyMessage.Build();
                Console.WriteLine($"{e.Message.Text}\n");
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message = "Сообщение введено",
                    PeerId = chatId,
                    RandomId = Environment.TickCount,
                    Keyboard = keyboardMessage
                });
                _command.CommandText = $"UPDATE PersonalData SET status = 'start' WHERE id = {e.Message.PeerId};";
                _command.ExecuteNonQuery();
                _command.CommandText =
                    $"UPDATE PersonalData SET message = '{e.Message.Text}' WHERE id = {e.Message.PeerId};";
                _command.ExecuteNonQuery();


                KeyboardBuilder keyFinish = new KeyboardBuilder();
                keyFinish.AddButton("Дальше", "", KeyboardButtonColor.Positive);
            }
            else if (e.Message.Text == "!Стоп")
            {
                KeyboardBuilder keyStop = new KeyboardBuilder();
                keyStop.AddButton("!Запустить", "", KeyboardButtonColor.Positive);
                keyStop.AddButton("!Заполнить", "", KeyboardButtonColor.Negative);
                keyStop.AddButton("!Данные", "", KeyboardButtonColor.Primary);
                MessageKeyboard keyboardStop = keyStop.Build();
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message =
                        "Выбери, что хотел бы сделать:\n1) Запустить бота\n2) Заполнить данные\n3) Показать имеющиеся данные",
                    PeerId = chatId,
                    RandomId = Environment.TickCount,
                    Keyboard = keyboardStop
                });
                _command.CommandText = $"UPDATE PersonalData SET status = 'start' WHERE id = {e.Message.PeerId};";
                _command.ExecuteNonQuery();
            }
            else if (e.Message.Text == "!Запустить")
            {
                _command.CommandText = $"SELECT * FROM [PersonalData] WHERE id={e.Message.PeerId};";
                _command.ExecuteNonQuery();
                _reader = _command.ExecuteReader();
                _reader.Read();

                KeyboardBuilder keySend = new KeyboardBuilder();
                keySend.AddButton("Дальше", "", KeyboardButtonColor.Positive);

                MessageKeyboard keyboardSend = keySend.Build();


                if (_reader.GetValue(1) != "" && _reader.GetValue(2) != "" && _reader.GetValue(3) != "" &&
                    _reader.GetValue(4) != "")
                {
                    _reader.Close();
                    _bot.Api.Messages.Send(new MessagesSendParams()
                    {
                        Message =
                            "*Сейчас бот запустил бота зума(да, звучит бредово, но пока зумовская часть не реализована, будет так)*",
                        PeerId = chatId,
                        RandomId = Environment.TickCount,
                        Keyboard = keyboardSend
                    });
                    _command.CommandText = $"UPDATE PersonalData SET status = 'start' WHERE id = {e.Message.PeerId};";
                    _command.ExecuteNonQuery();
                }
                else
                {
                    _reader.Close();
                    _bot.Api.Messages.Send(new MessagesSendParams()
                    {
                        Message =
                            "Невозможно запустить бота так как не имеются все данные",
                        PeerId = chatId,
                        RandomId = Environment.TickCount,
                        Keyboard = keyboardSend
                    });
                    _command.CommandText = $"UPDATE PersonalData SET status = 'start' WHERE id = {e.Message.PeerId};";
                    _command.ExecuteNonQuery();
                }
            }
            else if (e.Message.Text == "!Данные")
            {
                KeyboardBuilder keyData = new KeyboardBuilder();
                keyData.AddButton("!Запустить", "", KeyboardButtonColor.Positive);
                keyData.AddButton("!Заполнить", "", KeyboardButtonColor.Negative);
                keyData.AddButton("!Данные", "", KeyboardButtonColor.Primary);
                MessageKeyboard keyboardData = keyData.Build();
                _command.CommandText = $"SELECT * FROM [PersonalData] WHERE id={e.Message.PeerId};";
                _command.ExecuteNonQuery();
                _reader = _command.ExecuteReader();
                _reader.Read();
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message =
                        $"Вот твои данные:\nid: {_reader.GetValue(0)}\nname: {_reader.GetValue(1)}\nlink: {_reader.GetValue(2)}\nstatus: {_reader.GetValue(3)}\nmessage: {_reader.GetValue(4)}\nСледующая команда будет продолжать то, что было до !Данные",
                    PeerId = chatId,
                    RandomId = Environment.TickCount,
                    Keyboard = keyboardData
                });
                _reader.Close();
            }
            else
            {
                KeyboardBuilder keyElse = new KeyboardBuilder();
                keyElse.AddButton("!Запустить", "", KeyboardButtonColor.Positive);
                keyElse.AddButton("!Заполнить", "", KeyboardButtonColor.Negative);
                keyElse.AddButton("!Данные", "", KeyboardButtonColor.Primary);
                MessageKeyboard keyboardElse = keyElse.Build();
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message =
                        "Выбери, что хотел бы сделать:\n1) Запустить бота\n2) Заполнить данные\n3) Показать имеющиеся данные",
                    PeerId = chatId,
                    RandomId = Environment.TickCount,
                    Keyboard = keyboardElse
                });
            }
        }
    }
}