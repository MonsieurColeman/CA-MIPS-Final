using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColemanPeerToPeer.MVVM.Model
{
    class MessageModel
    {
        public string Username { get; set; }
        public string UsernameColor { get; set; }
        public string ImageSourcer { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public bool isFromMe { get; set; }
        public bool? FirstMessage { get; set; }
    }
}
