using System;
using System.IO.Ports;

namespace SmsSender
{
    /// <summary>
    /// This class sends a text message from a connected mobile phone using AT protocol.
    /// It is tuned to work on Nokia phone, but 
    /// </summary>
    class SmsSender
    {
        private SerialPort myPort = null;

        public void Open()
        {
            if (myPort != null)
            {
                throw new Exception("Already opened. Call close first.");
            }

            foreach(string s in SerialPort.GetPortNames())
            {
                try
                {
                    myPort = new SerialPort(s, 9600);
                    myPort.NewLine = "\r";
                    myPort.Open();
                    myPort.WriteTimeout = 300;
                    myPort.WriteLine("at"); 
                    myPort.WriteLine("at"); 
                    myPort.WriteLine("at");  ValidateResponse("ok", 300);
                    myPort.WriteLine("AT+CMGF=1"); ValidateResponse("ok", 300);
                    myPort.WriteLine("AT+CSCS=\"HEX\""); ValidateResponse("ok", 300);
                    myPort.WriteLine("AT+CSMP=17,167,0,8"); ValidateResponse("ok", 300);
                    
                    return;
                }
                catch
                {
                    Close();
                }
            }

            throw new Exception("Cannot find modem.");
        }

        private void ValidateResponse(string resp, int timeout)
        {
            myPort.ReadTimeout = timeout;
            //This is the only proper way to read,as there might be more the one line in return
            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < timeout)
            {
                string r = myPort.ReadLine();
                if (r.ToLower().Contains(resp.ToLower()))
                {
                    //but before you exit, take everything you have out of the buffer
                    myPort.ReadTimeout = 10;
                    while (true)
                    {
                        try
                        {
                            myPort.ReadLine();
                        }
                        catch
                        {
                            break;
                        }
                    }
                    return;
                }
            }
            throw new Exception("Modem did not repond properly.");
        }

        public void Close()
        {
            if (myPort != null)
            {
                myPort.Close();
                myPort = null;
            }
        }

        public void SendSms(string number, string msg)
        {
            string msghex = "";
            foreach (char c in msg)
            {
                msghex += string.Format("{0:X4}", (int)c);
            }

            myPort.WriteLine("AT+CMGS=\""+number+"\"");

            //here we will just pause a bit
            System.Threading.Thread.Sleep(200);
            myPort.Write(msghex + ((char)26));
            ValidateResponse("+cmgs:", 10000);
        }
    }
}
