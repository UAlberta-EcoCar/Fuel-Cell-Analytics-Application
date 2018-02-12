using System;
using System.Collections.Generic;
using System.Threading;
using System.IO.Ports;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FCA.Core
{
    public class CommunicationsControl
    {
        #region Fields
        // COM Port
        private SerialPort com_port = null;

        // Stores strings in
        private List<char> com_in_buffer = null;
        private Stack<char> com_in_stack = null;

        // Event for Input
        public event EventHandler InputStringRecieved;
        #endregion

        #region Constructors

        public CommunicationsControl()
        {
            this.com_port = new SerialPort();
            this.com_port.DataReceived += SerialInEventHandler;
            this.com_in_buffer = new List<char>();
            this.com_in_stack = new Stack<char>();
        }

        public CommunicationsControl(string com_port)
        {
            this.com_port = new SerialPort(com_port);
            this.com_port.DataReceived += SerialInEventHandler;
            this.com_in_buffer = new List<char>();
            this.com_in_stack = new Stack<char>();
        }

        public CommunicationsControl(string com_port, int baud)
        {
            this.com_port = new SerialPort(com_port, baud);
            this.com_port.DataReceived += SerialInEventHandler;
            this.com_in_buffer = new List<char>();
            this.com_in_stack = new Stack<char>();
        }

        #endregion

        #region Methods
        #region Protected
        /// <summary>
        /// The event throw wrapper for InputStringRecieved event
        ///     - Throws the event
        /// </summary>
        /// <param name="e"></param>

        protected virtual void OnInputStringRecieved(InputStringRecievedEventArgs e)
        {
            if (InputStringRecieved != null)
            {
                InputStringRecieved(this, e);
            }
        }

        #endregion
        #region Public


        /// <summary>
        /// Connects to the provided port
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <returns> True if connection was successful </returns>
        public bool SetPort(string port, int baud=1000000)
        {
            if (this.com_port.IsOpen && this.com_port.PortName == port)
            {
                return true;
            }

            if (this.com_port.IsOpen)
            {
                this.com_port.Close();
            }
            this.com_port.PortName = port;
            this.com_port.BaudRate = baud;
            try
            {
                this.com_port.Open();
            }
            catch (UnauthorizedAccessException e)
            {
                return false;
            }

            this.com_port.DataReceived += this.SerialInEventHandler;

            return true;
        }

        public bool SendCommand(string command)
        {
            if (command == null) return false;

            // Check if command is a valid command format
            
            // Write command
            this.com_port.Write(command);
            return true;
        }

        /// <summary>
        /// Eventhandler for SerialInEvent thrown by the serial port
        ///     - Throws InputStringReceivedEvent after a full JSON string has been received
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SerialInEventHandler(object sender, 
                                            SerialDataReceivedEventArgs e)
        {
            SerialPort com = (SerialPort)sender;
            int byte_avalible = com.BytesToRead;
            char[] buffer = new char[byte_avalible];
            com.Read(buffer, 0, byte_avalible);
            foreach(char i in buffer)
            {
                
                //if (i.Equals('\n'))
                //{
                //    string in_string = new string(this.com_in_buffer.ToArray());
                //    try
                //    {
                //        OnInputStringRecieved(new InputStringRecievedEventArgs(JObject.Parse(in_string)));
                //    }
                //    catch (JsonReaderException ex)
                //    {
                //        // Do Nothing - log somewhere eventually
                //    }
                    
                //    this.com_in_buffer = new List<char>();
               // }

                if (i.Equals('{'))
                {
                    this.com_in_stack.Push('{');
                }
                else if (i.Equals('}'))
                {
                    this.com_in_stack.Pop();
                    this.com_in_buffer.Add(i);
                    if (this.com_in_stack.Count == 0)
                    {
                        string in_string = new string(this.com_in_buffer.ToArray());
                        try
                        {
                            OnInputStringRecieved(new InputStringRecievedEventArgs(JObject.Parse(in_string)));
                        }
                        catch (JsonReaderException ex)
                        {
                            // Do Nothing - log somewhere eventually
                        }

                        this.com_in_buffer = new List<char>();
                    }
                }

                if (this.com_in_stack.Count != 0)
                {
                    this.com_in_buffer.Add(i);
                }
            } 
        }
        #endregion
        #endregion
    }
}
