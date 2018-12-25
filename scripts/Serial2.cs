using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.Events;

// System.IO.Ports requires a working Serial Port. On Mac, you will need to purcase the Uniduino plug-in on the Unity Store
// This adds a folder + a file into your local folder at ~/lib/libMonoPosixHelper.dylib
// This file will activate your serial port for C# / .NET
// The functions are the same as the standard C# SerialPort library
// cf. http://msdn.microsoft.com/en-us/library/system.io.ports.serialport(v=vs.110).aspx

namespace UniSerial{
    public class Serial2 : MonoBehaviour
    {
        public enum Baudrate
        {
            B_300 = 300,
            B_1200 = 1200,
            B_2400 = 2400,
            B_4800 = 4800,
            B_9600 = 9600,
            B_19200 = 19200,
            B_38400 = 38400,
            B_57600 = 57600,
            B_74880 = 74880,
            B_115200 = 115200,
            B_230400 = 230400,
            B_250000 = 250000
        }

        [System.Serializable] public class MBStringEvent : UnityEvent<string> { }
        [System.Serializable] public class MBStringArrayEvent : UnityEvent<string[]> { }
        public MBStringEvent receiveDataEvent;
        public MBStringEvent receiveLineEvent;

        public string PortName = "/dev/cu.usbserial-A601E9SK";
        [SerializeField] Baudrate baudrate = Baudrate.B_115200;

        /// <summary>
        /// Maximum number of lines to remember. Get them with GetLines() or GetLastLine()
        /// </summary>
        public int RememberLines = 0;

        //string serialOut = "";
        private List<string> linesIn = new List<string>();

        /// <summary>
        /// Gets the received bytes count.
        /// </summary>
        /// <value>The received bytes count.</value>
        public int ReceivedBytesCount { get { return BufferIn.Length; } }

        /// <summary>
        /// Gets the received bytes.
        /// </summary>
        /// <value>The received bytes.</value>
        public string ReceivedBytes { get { return BufferIn; } }

        /// <summary>
        /// Clears the received bytes. 
        /// Warning: This prevents line detection and notification. 
        /// To be used when no \n is expected to avoid keeping unnecessary big amount of data in memory
        /// You should normally not call this function if \n are expected.
        /// </summary>
        public void ClearReceivedBytes()
        {
            BufferIn = "";
        }

        /// <summary>
        /// Gets the lines count.
        /// </summary>
        /// <value>The lines count.</value>
        public int linesCount { get { return linesIn.Count; } }

        #region Private vars

        // buffer data as they arrive, until a new line is received
        private string BufferIn = "";

        // flag to detect whether coroutine is still running to workaround coroutine being stopped after saving scripts while running in Unity
        private int nCoroutineRunning = 0;

        private List<string> dataBuf;
        private List<string> lineBuf;
        private List<string[]> valuesBuf;

        #endregion

        #region Static vars

        // Only one serial port shared among all instances and living after all instances have been destroyed
        private static SerialPort s_serial;

        // 
        private static List<Serial2> s_instances = new List<Serial2>();

        #endregion

        static Serial2 _instance;
        void Awake()
        {
            _instance = this;
        }

        void Start()
        {
            // print ("Serial Start ");
            dataBuf = new List<string>();
            lineBuf = new List<string>();
            valuesBuf = new List<string[]>();
        }

        void OnValidate()
        {
            if (RememberLines < 0)
                RememberLines = 0;
        }

        void OnEnable()
        {
            //        print("Serial OnEnable");
            //        if (s_serial != null)
            //            print ("serial IsOpen: " + s_serial.IsOpen);
            //        else
            //            print ("no serial: ");

            s_instances.Add(this);

            checkOpen((int)baudrate);

        }

        void OnDisable()
        {
            //print("Serial OnDisable");
            s_instances.Remove(this);
        }

        public void OnApplicationQuit()
        {

            if (s_serial != null)
            {
                if (s_serial.IsOpen)
                {
                    print("closing serial port");
                    s_serial.Close();
                }

                s_serial = null;
            }

        }

        void Update()
        {
            //print ("Serial Update");

            if (s_serial != null && s_serial.IsOpen)
            {
                if (nCoroutineRunning == 0)
                {
                    // Each instance has its own coroutine but only one will be active a 
                    StartCoroutine(ReadSerialLoop());
                }
                else
                {
                    if (nCoroutineRunning > 1)
                        print(nCoroutineRunning + " coroutines in " + name);

                    nCoroutineRunning = 0;
                }
            }
            if(dataBuf.Count>0){
                foreach(string data in dataBuf){
//                    SendMessage("OnSerialData", data);
                    receiveDataEvent.Invoke(data);
                }
                dataBuf.Clear();
            }
            if(lineBuf.Count>0){
                foreach(string line in lineBuf){
//                    SendMessage("OnSerialLine", line);
                    receiveLineEvent.Invoke(line);
                }
                lineBuf.Clear();
            }
            if(valuesBuf.Count>0){
                foreach(string[] vals in valuesBuf){
                    SendMessage("OnSerialValues", vals);
                }
                valuesBuf.Clear();
            }
        }

        public IEnumerator ReadSerialLoop()
        {

            while (true)
            {

                if (!enabled)
                {
                    //print ("behaviour not enabled, stopping coroutine");
                    yield break;
                }

                //print("ReadSerialLoop ");
                nCoroutineRunning++;

                try
                {
                    while (s_serial.BytesToRead > 0)
                    {  // BytesToRead crashes on Windows -> use ReadLine in a Thread

                        string serialIn = s_serial.ReadExisting();

                        // Dispatch new data to each instance
                        foreach (Serial2 inst in s_instances)
                        {
                            inst.receivedData(serialIn);
                        }

                    }

                }
                catch (System.Exception e)
                {
                    print("System.Exception in serial.ReadLine: " + e.ToString());
                }

                yield return null;
            }

        }

        /// return all received lines and clear them
        /// Useful if you need to process all the received lines, even if there are several since last call
        public List<string> GetLines(bool keepLines = false)
        {

            List<string> lines = new List<string>(linesIn);

            if (!keepLines)
                linesIn.Clear();

            return lines;
        }

        /// return only the last received line and clear them all
        /// Useful when you need only the last received values and can ignore older ones
        public string GetLastLine(bool keepLines = false)
        {

            string line = "";
            if (linesIn.Count > 0)
                line = linesIn[linesIn.Count - 1];

            if (!keepLines)
                linesIn.Clear();

            return line;
        }

        public void Write(string message)
        {
            if (checkOpen())
                s_serial.Write(message);
        }

        public void WriteLn(string message = "")
        {
            if (s_serial != null && s_serial.IsOpen)
                s_serial.Write(message+"\n");

        }


        /// <summary>
        /// Verify if the serial port is opened and opens it if necessary
        /// </summary>
        /// <returns><c>true</c>, if port is opened, <c>false</c> otherwise.</returns>
        /// <param name="portSpeed">Port speed.</param>
        public bool checkOpen(int portSpeed = 230400)
        {

            if (s_serial == null)
            {

                string portName = PortName;

                if (portName == "")
                {
                    print("Error: Couldn't find serial port.");
                    return false;
                }
                else
                {
                    //print ("Opening serial port: " + portName);
                }

                s_serial = new SerialPort(portName, portSpeed);

                s_serial.Open();
                //print ("default ReadTimeout: " + serial.ReadTimeout);
                //serial.ReadTimeout = 10;

                // cler input buffer from previous garbage
                s_serial.DiscardInBuffer();


            }

            return s_serial.IsOpen;
        }

        // Data has been received, do what this instance has to do with it
        protected void receivedData(string data)
        {

            if (receiveDataEvent.GetPersistentEventCount() > 0)
            {
                dataBuf.Add(data);
                SendMessage("OnSerialData", data);
//                receiveDataEvent.Invoke(data);
            }

            // Detect lines
            if (receiveLineEvent.GetPersistentEventCount() > 0)
            {

                // prepend pending buffer to received data and split by line
                string[] lines = (BufferIn + data).Split('\n');

                // If last line is not empty, it means the line is not complete (new line did not arrive yet), 
                // We keep it in buffer for next data.
                int nLines = lines.Length;
                BufferIn = lines[nLines - 1];

                // Loop until the penultimate line (don't use the last one: either it is empty or it has already been saved for later)
                for (int iLine = 0; iLine < nLines - 1; iLine++)
                {
                    string line = lines[iLine];
                    //print(line);

                    // Buffer line
                    if (RememberLines > 0)
                    {
                        linesIn.Add(line);

                        // trim lines buffer
                        int overflow = linesIn.Count - RememberLines;
                        if (overflow > 0)
                        {
                            print("Serial removing " + overflow + " lines from lines buffer. Either consume lines before they are lost or set RememberLines to 0.");
                            linesIn.RemoveRange(0, overflow);
                        }
                    }

                    // notify new line
                    if (receiveLineEvent.GetPersistentEventCount()>0)
                    {
                        lineBuf.Add(line);
//                        SendMessage("OnSerialLine", line);
//                        receiveLineEvent.Invoke(line);
                    }

                }
            }
        }

        string GetPortName()
        {

            string[] portNames;

            switch (Application.platform)
            {

                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                //      case RuntimePlatform.OSXDashboardPlayer:
                case RuntimePlatform.LinuxPlayer:

                    portNames = System.IO.Ports.SerialPort.GetPortNames();

                    if (portNames.Length == 0)
                    {
                        portNames = System.IO.Directory.GetFiles("/dev/");
                    }

                    foreach (string portName in portNames)
                    {
                        if (portName.StartsWith("/dev/tty.usb") || portName.StartsWith("/dev/ttyUSB"))
                            return portName;
                    }
                    return "";

                default: // Windows

                    portNames = System.IO.Ports.SerialPort.GetPortNames();

                    if (portNames.Length > 0)
                        return portNames[0];
                    else
                        return "COM3";

            }

        }

        #region Singleton
        public static Serial2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject(typeof(Serial2).ToString());
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<Serial2>();
                }
                return _instance;
            }
        }
        #endregion
    }
}
