using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerWPF
{
    public class ClientInfo
    {
        public TcpClient Client { get; set; }
        public string Name { get; set; }
    }
}
