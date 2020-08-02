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


            public double GetRPM()
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
                if (RPM > 0)
                {
                    return RPM;
                }
            return 0;
            }

        public void Send2Database()
        {
            string myconnection;
            myconnection = @"Data Source=SQL2008-VS5\inst5;Initial Catalog=JWB_EET321; User ID=JWB18; Password=EET321";
            SqlConnection cnn = new SqlConnection(myconnection); //sets up connection too SQL data base
            DataClasses1DataContext dc = new DataClasses1DataContext(); // this connects C# to the Database
            JWB_Table LT = new JWB_Table();// create object based on class definition C# obtained from the database fields
            cnn.Open();
            LT.DateTime = System.DateTime.Now;
            LT.Name = ;//16 characters each
            LT.Voltage = 0;
            LT.RPM = 0;
            LT.Note = 0;
            dc.JWB_Tables.InsertOnSubmit(LT);
            dc.SubmitChanges();
            //MessageBox1.Text = ("Connection Open!");
            cnn.Close();
        }
