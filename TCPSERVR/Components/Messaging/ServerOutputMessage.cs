// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Newtonsoft.Json;
using Tbasic.Errors;

namespace TCPSERVR.Messaging
{
    /// <summary>
    /// A message that is sent from the server
    /// </summary>
    public class ServerOutputMessage
    {
        /// <summary>
        /// The id of the message this is a response to
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public uint ID { get; set; }

        /// <summary>
        /// Gets or sets the status code for this response
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the status message for this response
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string StatusMessage { get; set; }

        /// <summary>
        /// The response data that will be sent
        /// </summary>
        [JsonProperty(PropertyName = "response", TypeNameHandling = TypeNameHandling.All)]
        public object Response { get; set; }
        
        internal ServerOutputMessage()
        {
        }

        public ServerOutputMessage(int status, object response)
            : this(status, FunctionException.GetGenericMessage(status), response)
        {
        }

        public ServerOutputMessage(FunctionException ex)
            : this(ex.Status, ex.Message, null)
        {
        }

        public ServerOutputMessage(int status, string statusMessage, object response)
        {
            Status = status;
            StatusMessage = statusMessage;
            Response = response;
        }
    }
}