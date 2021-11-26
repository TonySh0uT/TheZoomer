using System;
using System.Runtime.Loader;
using VkBotFramework;
using VkBotFramework.Models;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace ServerAgregator
{
    public class Program
    {
        private static string AccessToken = "82245a591340d96dd1c0ae108441550b88698bace68001d0ed91f57545efd144ce10c8e2a53d872a0cadf";
        private static string GroupUrl = "https://vk.com/thezoomerproject";
        private static VkBot _bot;
        
        private static string name;
        private static string link;
        private static string status = "start";
        
        
        
        public static void Main(string[] args)
        {
            _bot = new VkBot(AccessToken, GroupUrl);
            _bot.OnMessageReceived += BotOnOnMessageReceived;
            _bot.Start();
            Console.ReadLine();
        }

        private static void BotOnOnMessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            var chatId = e.Message.PeerId;



            
            

            if (e.Message.Text == "!start" && (status == "start"))
            {
                _bot.Api.Messages.Send(new MessagesSendParams()
                {
                    Message = "Дарова, пидор. Введи своё имя.",
                    PeerId = chatId,
                    RandomId = Environment.TickCount

                });
                    status = "name";
            }else
            if (status == "name")
            {
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