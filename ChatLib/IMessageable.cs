using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib
{
    /// <summary>
    /// Interface for clients and servers to have messaging functionality.
    /// </summary>
    public interface IMessageable
    {
        bool WriteMessage(string message);
        void CheckForMessages();
        void Disconnect();
        List<string> Messages { get;  }
    }
}
