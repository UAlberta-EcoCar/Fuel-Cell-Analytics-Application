using System;
using Newtonsoft.Json.Linq;

namespace FCA.Core
{
    public class InputStringRecievedEventArgs: EventArgs
    {
        private readonly JObject obj = null;

        public InputStringRecievedEventArgs(JObject obj)
        {
            this.obj = obj;
        }

        public JObject JObject
        {
            get{ return this.obj; }
        }
    }
}
