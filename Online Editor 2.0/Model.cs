using Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Editor
{
    class Model
    {

        private ClientMain client;

        public Model ()
        {
            //this.client = new ClientMain("localhost", 34192);
        }

        public void SendMessage ()
        {
            this.client.SendMessage("button clicked");
        }

    }
}
