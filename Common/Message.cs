using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Common
{
    public class Message
    {
        [JsonProperty(PropertyName = "usr")]
        public string User { get; set; }
        [JsonProperty(PropertyName = "msg")]
        public string Text { get; set; }
        public Message()
        {

        }
        public Message(string s)
        {
            var o = JsonConvert.DeserializeObject(s, this.GetType()) as Message;
            this.User = o.User;
            this.Text = o.Text;
        }
        public override string ToString()
        {
            return this.User + ": " + this.Text + System.Environment.NewLine;
        }

        public string GetJSONString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
