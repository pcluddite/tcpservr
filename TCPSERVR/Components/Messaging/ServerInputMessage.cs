// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Newtonsoft.Json;
using System;
using Tbasic.Parsing;
using Tbasic.Runtime;

namespace TCPSERVR.Messaging
{
    /// <summary>
    /// A message that is sent to the server
    /// </summary>
    public class ServerInputMessage
    {
        /// <summary>
        /// Gets or sets an ID for this message
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public uint ID { get; set; }

        /// <summary>
        /// Gets or sets the this message
        /// </summary>
        [JsonProperty(PropertyName = "msg")]
        public string Message { get; set; }

        /// <summary>
        /// Gets the time the object was created (this is not serialized)
        /// </summary>
        [JsonIgnore]
        public DateTime CreationTime { get; private set; }

        private string name = null;

        /// <summary>
        /// Gets the name of the message (first argument)
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get {
                if (name == null) { // lazy initialization
                    Line line = new Line(0, Message);
                    return name = line.Name;
                }
                else {
                    return name;
                }
            }
        }

        internal ServerInputMessage()
        {
        }

        /// <summary>
        /// Initialize a new TMessage object
        /// <param name="msg">the message this object handles as a string</param>
        /// </summary>
        public ServerInputMessage(string msg)
            : this(0, msg)
        {
        }
        
        /// <summary>
        /// Initialize a new TMessage object
        /// </summary>
        /// <param name="id">the identification number to assign the message</param>
        /// <param name="msg">the message this object handles as a string</param>
        public ServerInputMessage(uint id, string msg)
        {
            ID = id;
            Message = msg;
            CreationTime = DateTime.Now;
        }

        public StackData ToStackData(TRuntime runtime)
        {
            return new StackData(runtime, Message);
        }
    }
}