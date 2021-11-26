using System;
using System.Runtime.Loader;
using VkBotFramework;
using VkBotFramework.Models;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using System.Data.SQLite;
using System.IO;

namespace ServerAgregator
{
    public class Program
    {

        private static SQLiteConnection _connection;
        private static SQLiteDataReader _reader;
        private static SQLiteCommand _command;
        private static string AccessToken = "82245a591340d96dd1c0ae108441550b88698bace68001d0ed91f57545efd144ce10c8e2a53d872a0cadf";
        private static string GroupUrl = "https://vk.com/thezoomerproject";
        private static VkBot _bot;
        
        private static string name;
        private static string link;
        private static string status = "start";





        static public void dbCreate()
        {
            if (!File.Exists(@"DataBase\statusDb.db"))
            {
                SQLiteConnection.CreateFile(@"DataBase\statusDb.db");
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
            if(dbConnect(@"Database\statusDb.db")){
                _command = new SQLiteCommand(_connection)
                {
                    CommandText =
                        "CREATE TABLE IF NOT EXISTS PersonalData (id INTEGER , name TEXT, link TEXT, status TEXT, message TEXT);"
                };
                _command.ExecuteNonQuery();
                
                _connection.Close();
            }

            _bot = new VkBot(AccessToken, GroupUrl);
            Console.WriteLine("Bot started.\n");
            _bot.OnMessageReceived += BotOnOnMessageReceived;
            _bot.Start();
            Console.ReadLine();
        }

        private static void BotOnOnMessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            var chatId = e.Message.PeerId;
            _command = new SQLiteCommand();
            dbConnect(@"Database\statusDb.db");
            if (e.Message.Text == "!start" && (status == "start"))
            {
                Console.WriteLine($"{e.Message.Text}\n");
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message = "Дарова, пидор. Введи своё имя.",
                    PeerId = chatId,
                    RandomId = Environment.TickCount

                });
                    status = "name";
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
                            $"INSERT INTO [PersonalData] VALUES ('{e.Message.PeerId}','','', '','');";
                        _command.ExecuteNonQuery();
                    }

                    _command.CommandText = $"UPDATE PersonalData SET status = '{status}' WHERE id = {e.Message.PeerId};";
                    _command.ExecuteNonQuery();






            }else
            if (status == "name" && e.Message.Text!="!stop")
            {
                Console.WriteLine($"{e.Message.Text}\n");
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message = "Имя есть, введи ссылку мудак",
                    PeerId = chatId,
                    RandomId = Environment.TickCount
                });
                name = e.Message.Text;
                status = "link";
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
            } else if (status == "link")
            {
                if ((e.Message.Text != "") && (e.Message.Text != "!stop"))
                {
                    if (e.Message.Text.Contains("zoom.us"))
                    {
                        Console.WriteLine($"{e.Message.Text}\n");
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message = "С ссылкой разобрались, а теперь иди нахуй",
                            PeerId = chatId,
                            RandomId = Environment.TickCount
                        });
                        link = e.Message.Text;

                        status = "start";
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message = $"Короче вот твои мудацкие данные: {name} {link}",
                            PeerId = chatId,
                            RandomId = Environment.TickCount
                        });
                    }
                    else
                    {
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message = "Ну ты короче клоун ссылка то нихуя не зумовская\nПопробуй снова еблан либо напиши !stop",
                            PeerId = chatId,
                            RandomId = Environment.TickCount
                        }); 
                    }
                }
                
                
                else if( e.Message.Attachments.Count !=0  &&  e.Message.Attachments[0].Type == typeof(Link))
                {
                    
                    Attachment link1 = new Attachment();
                    
                    
                    
                    
                    link1 = e.Message.Attachments[0];
                    link = link1.Instance.ToString();

                    if (link.Contains("zoom.us"))
                    {
                        status = "start";
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message = $"Короче вот твои мудацкие данные: {name} {link}",
                            PeerId = chatId,
                            RandomId = Environment.TickCount
                        });
                       
                    }
                    else
                    {
                        _bot.Api.Messages.Send(new MessagesSendParams()
                        {
                            Message = "Ну ты короче клоун ссылка то нихуя не зумовская\nПопробуй снова еблан либо напиши !stop",
                            PeerId = chatId,
                            RandomId = Environment.TickCount
                        }); 
                    }
                }
                else if (e.Message.Text == "!stop")
                {
                    _bot.Api.Messages.Send(new MessagesSendParams()
                    {
                        Message = "Чтобы начать напиши !start",
                        PeerId = chatId,
                        RandomId = Environment.TickCount
                    });
                    status = "start";
                }
            }
            else if(e.Message.Text == "!help")
            {
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message = "Для того, чтобы остановить нажмите !stop",
                    PeerId = chatId,
                    RandomId = Environment.TickCount
                });
            }
            else
            {
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message = "Чтобы начать напиши !start.",
                    PeerId = chatId,
                    RandomId = Environment.TickCount
                });
            }
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }
    }
}