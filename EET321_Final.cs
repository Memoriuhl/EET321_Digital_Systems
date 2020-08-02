using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using AnalogDiscoveryClasses;


namespace PynqPythonEthernet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void sendCommandButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SendTextBox.Text == "")
                    System.Threading.Thread.Sleep(500); //delay
                else
                    chart1.Series["Actual_RPM"].BorderWidth = 5; //adjusts graph line width
                sendtext.Text = SendTextBox.Text; //sends data to the other label
                SendTextBox.Text = ""; //clears input data
                timer1.Start(); //starts the timer below
            }
            catch
            {
                MessageBox.Show("Connect failed try again");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void SendTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void outDataTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //the following code establishes a connection to the python server and sends the SP data 
            //to the server and recieves the pwm output % and rpm value. This is executed every 500mS
            int rpm;
            int pwm;

            string IpAdd = Convert.ToString("192.168.0.101");

            var client = new TcpClient(IpAdd, 50507);  //50507 is working port

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(sendtext.Text); //stores text box data
            NetworkStream stream = client.GetStream(); //connects
            stream.Write(data, 0, data.Length); //writes data

            NetworkStream stream2 = client.GetStream(); //connects
            data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes = stream2.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes); //recieves data
            bytes = stream2.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            if (responseData == "")
            {
                System.Threading.Thread.Sleep(500); //delay
            }

            else
            {
                string[] splitRes = new string[2];
                splitRes = responseData.Split(','); //splits message at the ',' and puts in array
                rpm = Convert.ToInt32(splitRes[1]); //splits the message up and gets correct information for rpm
                pwm = Convert.ToInt32(splitRes[0]); // splits the message up and gets correct information for pwm
                RPM_Data.Text = splitRes[1]; //shows data
                PWM_Data.Text = splitRes[0]; //shows data
                chart1.Series["Actual_RPM"].Points.AddY((Convert.ToInt32(RPM_Data.Text))); //updates the chart
                int setrpm = Convert.ToInt32(sendtext.Text);
                int a = rpm - setrpm;
                float PerDif = ((((float)a) / (float)setrpm) * 100);
                PercDiff.Text = Convert.ToString(Convert.ToInt32(PerDif));
                System.Threading.Thread.Sleep(500); //delay
            }


        }

        private void sendtext_TextChanged(object sender, EventArgs e)
        {

        }

        private void sendtext_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        double outVoltage, inVoltage;
        string[] VoltageOut = new string[159];
        string[] VoltageIn = new string[159];
        double voltage = 0;
        string vstring;
        string stringcmd = ":APPL CH1,";
        string cmdstring;
        private void test1Start_Click(object sender, EventArgs e)
        {
            /* TODO:
             * Ascending Voltages +0.5 from 6V to 24V
             * Descending Voltages -0.5 from 24V to 6V
             * Measure frequency, calculate and plot RPM as the program runs
             */

            voltage = 5;

            NationalInstruments.NI4882.Device DMMin = new NationalInstruments.NI4882.Device(0, 1);//DMM GPIB Initialization (Voltage input)
            Byte[] response = new Byte[500000];
            var client = new TcpClient("192.168.0.100", 5555);

            for (int x = 0; x < 76; x++)
            {
                if (voltage >= 4 && x < 38)
                {
                    voltage = voltage + 0.5;
                    vstring = Convert.ToString(voltage);
                    cmdstring = stringcmd + vstring + ",0\n";
                    label1.Text = cmdstring;
                    //command for the rigol scope to set the voltage and current of channel 1
                    Byte[] cmd = System.Text.Encoding.ASCII.GetBytes(cmdstring);
                    NetworkStream stream = client.GetStream();
                    stream.Write(cmd, 0, cmd.Length);

                    /*
                    DMMin.Write("MEAS:VOLT:DC?");//measuring voltage across wire
                    System.Threading.Thread.Sleep(250);//sleeps for 1 second
                    VoltageIn[x] = DMMin.ReadString();//reads the voltage from the volt command
                    System.Threading.Thread.Sleep(250);//sleeps for 12 seconds
                    inVoltage = Convert.ToDouble(VoltageIn[x]);

                    string myconnection;
                    myconnection = @"Data Source=SQL2008-VS5\inst5;Initial Catalog=EET321_Lab6_S19; User ID=EET321; Password=EET321_s19_!";
                    SqlConnection cnn = new SqlConnection(myconnection); //sets up connection too SQL data base
                    DataClasses2DataContext dc = new DataClasses2DataContext(); // this connects C# to the Database
                    Lab6_Table_1 LT = new Lab6_Table_1();// create object based on class definition C# obtained from the database fields
                    cnn.Open();
                    LT.DateTime = System.DateTime.Now;
                    LT.GroupID = initials.Text + "ck" + buffer.Text + "U";
                    LT.InputVoltage = inVoltage;
                    LT.Step = x;
                    LT.VoltageOutput = outVoltage;
                    dc.Lab6_Table_1s.InsertOnSubmit(LT);
                    dc.SubmitChanges();
                    //MessageBox1.Text = ("Connection Open!");
                    cnn.Close();
                    */
                }

                if (voltage <= 24 && x >= 38)
                {
                    voltage = voltage - 0.5;
                    vstring = Convert.ToString(voltage);
                    cmdstring = stringcmd + vstring + ",0\n";
                    label1.Text = cmdstring;
                    //command for the rigol scope to set the voltage and current of channel 1
                    Byte[] cmd = System.Text.Encoding.ASCII.GetBytes(cmdstring);
                    NetworkStream stream = client.GetStream();
                    stream.Write(cmd, 0, cmd.Length);
                    /*
                    DMMin.Write("MEAS:VOLT:DC?");//measuring voltage across wire
                    System.Threading.Thread.Sleep(250);//sleeps for 1 second
                    VoltageIn[x] = DMMin.ReadString();//reads the voltage from the volt command
                    System.Threading.Thread.Sleep(250);//sleeps for 12 seconds
                    inVoltage = Convert.ToDouble(VoltageIn[x]);

                    string myconnection;
                    myconnection = @"Data Source=SQL2008-VS5\inst5;Initial Catalog=EET321_Lab6_S19; User ID=EET321; Password=EET321_s19_!";
                    SqlConnection cnn = new SqlConnection(myconnection); //sets up connection too SQL data base
                    DataClasses2DataContext dc = new DataClasses2DataContext(); // this connects C# to the Database
                    Lab6_Table_1 LT = new Lab6_Table_1();// create object based on class definition C# obtained from the database fields
                    cnn.Open();
                    LT.DateTime = System.DateTime.Now;
                    LT.GroupID = ;
                    LT.InputVoltage = inVoltage;
                    LT.Step = x;
                    LT.VoltageOutput = outVoltage;
                    dc.Lab6_Table_1s.InsertOnSubmit(LT);
                    dc.SubmitChanges();
                    //MessageBox1.Text = ("Connection Open!");
                    cnn.Close();
                    */


                }
                System.Threading.Thread.Sleep(1000);//sleeps for 12 seconds
            }
        }
        Boolean status;
        int enumfilter, pnDevice, idxDevice, devid, devver, pfIsUsed, phdwf, hdwf;
        string UserName, DeviceName, SerialNumber;
        byte[] szUserName = new byte[20];
        byte[] szDeviceName = new byte[20];
        Byte[] szSerialNumber = new Byte[20];
        double hzFrequency;
        double vAmplitude = 1.5;
        double vOffset = -1.5;

        double SampleRate, VoltsRMS, SumOfSquares;
        double VoltsRange = 8;
        double freq = 0, DistanceBetweenCrosses = 0;
        const int Samples = 1000;
        int LengthOfTime, NumOfCrossings;
        double[] Data = new double[Samples];
        double[] Data1 = new double[Samples];
        byte DwfState;
        double RPM = 0;
        private void test_Click(object sender, EventArgs e)
        {
            //The following code collects information about the Analog Discovery connected such as device ID, Seriak Number, etc
            status = AnalogDiscoveryAPI.FDwfEnum(enumfilter, ref pnDevice);//pnDevice = 1 if attached
            status = AnalogDiscoveryAPI.FDwfEnumDeviceType(idxDevice, ref devid, ref devver);
            status = AnalogDiscoveryAPI.FDwfEnumDeviceIsOpened(idxDevice, ref pfIsUsed);
            status = AnalogDiscoveryAPI.FDwfEnumUserName(idxDevice, szUserName);
            UserName = System.Text.Encoding.ASCII.GetString(szUserName);
            status = AnalogDiscoveryAPI.FDwfEnumDeviceName(idxDevice, szDeviceName);
            DeviceName = System.Text.Encoding.ASCII.GetString(szDeviceName);
            status = AnalogDiscoveryAPI.FDwfEnumSN(idxDevice, szSerialNumber);
            SerialNumber = System.Text.Encoding.ASCII.GetString(szSerialNumber);

            //The following code opens the Analog Discovery and enables the Channel 1 waveform output
            status = AnalogDiscoveryAPI.FDwfDeviceOpen(idxDevice, ref phdwf); //This makes the DeviceID as a pointer phdwf
            status = AnalogDiscoveryAPI.FDwfAnalogOutNodeEnableSet(phdwf, -1, 0, 1);

            //This for loop runs three times to account for all three states of operation in procedure 1
            for (int i = 0; i < 3; i++)
            {
                //clear variables
                freq = 0;
                DistanceBetweenCrosses = 0;
                NumOfCrossings = 0;
                SumOfSquares = 0;


                //the following code will configure the data collection with the determined sample rate and number of samples
                status = AnalogDiscoveryAPI.FDwfAnalogInFrequencySet(phdwf, 12500);
                status = AnalogDiscoveryAPI.FDwfAnalogInBufferSizeSet(phdwf, 250);
                status = AnalogDiscoveryAPI.FDwfAnalogInChannelEnableSet(phdwf, 0, true);
                status = AnalogDiscoveryAPI.FDwfAnalogInChannelRangeSet(phdwf, 0, 5);

                System.Threading.Thread.Sleep(1000);

                status = AnalogDiscoveryAPI.FDwfAnalogInConfigure(phdwf, true, true);

                //the following loop will collect data until all samples are accounted for
                while (true)
                {
                    AnalogDiscoveryAPI.FDwfAnalogInStatus(phdwf, true, ref DwfState);
                    if (DwfState == AnalogDiscoveryAPI.DwfStateDone)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                }

                //presents data in an array of the number of samples determined
                status = AnalogDiscoveryAPI.FDwfAnalogInStatusData(phdwf, 0, ref Data1[0], Samples);

                //the following code will compare positive and negative numbers to determine zero crossings of the sine wave
                for (int j = 0; j < Samples - 1; j++)
                {
                    if (Data1[j] > 0 && Data1[j + 1] < 0)
                    {
                        NumOfCrossings = NumOfCrossings + 1;
                    }
                    if (Data1[j] < 0 && Data1[j + 1] > 0)
                    {
                        NumOfCrossings = NumOfCrossings + 1;
                    }
                }

                //once zero crossings are and rms value is accounted for, the sample data is cleared
                for (int l = 0; l < Samples; l++)
                {
                    Data1[l] = 0;
                    Data[l] = 0;
                }

                /*Note: The calculated frequency will be more accurate with frequencies divisible by 10 due to the sample rate and number of samples. 
                  As it stands, this calculation is set up to collect an average of ten cycles worth of data with about 100 samples per cycle.
                  This means that our number of samples is at 1000, and our sample frequency is 100 times greater than our set frequency.*/

                //the number of samples divided by the number of zero crossings will give the amount of samples in between crossings. 
                DistanceBetweenCrosses = Samples / (double)NumOfCrossings;

                //the inverse of the period of the sample rate multiplied by the distance between zero crossings will give the frequecy of one HALF waveform.
                //this frequency is divided in HALF to account for an entire sine wave, thus giving the actual frequency of the waveform.

                freq = 1 / (1 / SampleRate * (double)DistanceBetweenCrosses) / 2;

                RPM = freq * (1 / 8) * 60;
                //Calculates the RPM based on given frequency
                if (RPM != RPM)
                {
                    RPM = 0;
                }
                //Avoids Sending a nan value to pwm
            }
        } 

        private void test2Start_Click(object sender, EventArgs e)
        {
            voltage = 6;
            NationalInstruments.NI4882.Device DMMin = new NationalInstruments.NI4882.Device(0, 1);//DMM GPIB Initialization (Voltage input)
            Byte[] response = new Byte[500000];
            var client = new TcpClient("192.168.0.101", 5555);

            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                    voltage = 6.5;
                if (i == 2)
                    voltage = 15;
                if (i == 3)
                    voltage = 23.5;

                vstring = Convert.ToString(voltage);
                cmdstring = stringcmd + vstring + ",0\n";
                label1.Text = cmdstring;
                //command for the rigol scope to set the voltage and current of channel 1
                Byte[] cmd = System.Text.Encoding.ASCII.GetBytes(cmdstring);
                NetworkStream stream = client.GetStream();
                stream.Write(cmd, 0, cmd.Length);

                System.Threading.Thread.Sleep(600000);//sleeps for 1 second

                DMMin.Write("MEAS:VOLT:DC?");//measuring voltage across wire
                System.Threading.Thread.Sleep(250);//sleeps for 1 second
                VoltageIn[i] = DMMin.ReadString();//reads the voltage from the volt command
                System.Threading.Thread.Sleep(250);//sleeps for 12 seconds
                inVoltage = Convert.ToDouble(VoltageIn[i]);

            }

        }
    }
}
