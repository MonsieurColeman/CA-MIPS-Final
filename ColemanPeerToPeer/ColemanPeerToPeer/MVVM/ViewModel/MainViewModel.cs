using ColemanPeerToPeer.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColemanPeerToPeer.MVVM.ViewModel
{
    class MainViewModel
    {
        public  ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<MessageModel>();
            Users = new ObservableCollection<UserModel>();

            Messages.Add(new MessageModel
            {
                Username = "Jeff",
                UsernameColor = "#40ff9a",
                ImageSourcer = "https://picsum.photos/200/300",
                Message = "Want to Handout",
                Time = DateTime.Now,
                isFromMe = false,
                FirstMessage = true
            });

            for (int i = 0; i < 3; i++)
            {
                Messages.Add(new MessageModel
                {
                    Username = "Jeff",
                    UsernameColor = "#40ff9a",
                    ImageSourcer = "https://picsum.photos/200/300",
                    Message = "Hey!",
                    Time = DateTime.Now,
                    isFromMe = false,
                    FirstMessage = false
                });
            }
            for (int i = 0; i < 4; i++)
            {
                Messages.Add(new MessageModel
                {
                    Username = "NotJeff",
                    UsernameColor = "#40ff9a",
                    ImageSourcer = "https://picsum.photos/200/300",
                    Message = "Hey!",
                    Time = DateTime.Now,
                    isFromMe = true,
                });
            }

            Messages.Add(new MessageModel
            {
                Username = "NotJeff",
                UsernameColor = "#40ff9a",
                ImageSourcer = "https://picsum.photos/200/300",
                Message = "Last",
                Time = DateTime.Now,
                isFromMe = true,
            });

            for (int i = 0; i < 30; i++)
            {
                Users.Add(new UserModel
                {
                    Username = $"Nigel {i}",
                    ImageSource = "https://picsum.photos/200/300",
                    Messages = Messages
                });
            }



        }
    }
}
