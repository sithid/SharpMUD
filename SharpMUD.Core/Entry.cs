using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMUD.Core {
    internal static class Entry {
        static void Main(string[] args) {

            MudServer server = new MudServer(2100);
            server.Start();
        }
    }
}
