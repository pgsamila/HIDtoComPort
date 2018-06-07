// USB HID Scale Reader by Matt Galanto using HidLibrary.dll by Mike O'Brien
//  http://www.bumderland.com/dev


// Reffer from : http://www.bumderland.com/dev/USBHIDScale.html
// Com port writing implementation by pgsamila

using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HidLibrary;

namespace PostageScale
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Serial Port data
        /// </summary>
        SerialPort serialPort;
        bool isSerialPortOpen = false;
        bool isReading = false;
        String vsText = "none";

        /// <summary>
        /// Reading Thread
        /// </summary>
        Thread hidReaderThread;

        /// <summary>
        /// HID device data
        /// </summary>
        HidDevice[] mahdDevices;
		HidDevice mhdDevice;

        /// <summary>
        /// Form1 constroctor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
        }

		/// <summary>
        /// If no HID device is selected and try to open HID device
        /// </summary>
        /// <returns></returns>
		private bool FailNoDevice()
		{
			if(mhdDevice == null)
			{
				MessageBox.Show("No Device Selected", "Error", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return true;
			}

			return false;
		}

        /// <summary>
        /// Com port not selected, but try to open
        /// </summary>
        /// <returns></returns>
		private bool FailNoComPort()
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("No Com port Selected", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Failed to close
        /// </summary>
        /// <returns></returns>
		private bool FailDeviceClosed()
		{
			if(!mhdDevice.IsOpen)
			{
				MessageBox.Show("Device is not open.", "Error", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return true;
			}

			return false;
		}

        /// <summary>
        /// Load form 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
			mahdDevices = HidDevices.Enumerate().ToArray();
			mhdDevice = null;

            for(int i = 0; i < mahdDevices.Length; i++)
            {
				cmbDevices.Items.Add(mahdDevices[i].Description);
            }
        }

        /// <summary>
        /// HID device selecting com port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void cmbDevices_SelectedIndexChanged(object sender, EventArgs e)
		{
			mhdDevice = mahdDevices[cmbDevices.SelectedIndex];

			if(mhdDevice.IsOpen)
				btnOpen.Text = "Close";
			else
				btnOpen.Text = "Open";
		}

        /// <summary>
        /// HID device open/ close Button
        /// Open & close HID device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnOpen_Click(object sender, EventArgs e)
		{
			if(FailNoDevice())
				return;

			if(mhdDevice.IsOpen)
			{
				mhdDevice.CloseDevice();
				btnOpen.Text = "Open";
			}
			else
			{
				mhdDevice.OpenDevice();
				btnOpen.Text = "Close";
			}
		}

        /// <summary>
        /// Com port Open/ Close buttn
        /// open & close com port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comButton_Click(object sender, EventArgs e)
        {
            if (FailNoComPort())
                return;

            if (comboBox1.SelectedItem != null & !isSerialPortOpen)
            {
                string comPort = comboBox1.GetItemText(comboBox1.SelectedItem);
                serialPort = new SerialPort(comPort);
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    serialPort.BaudRate = 115200;
                    serialPort.Parity = Parity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;
                    comButton.Text = "Close";
                    isSerialPortOpen = true;
                    btnRead.Enabled = true;
                }
            }
            else if (isSerialPortOpen)
            {
                if (hidReaderThread != null)
                {
                    hidReaderThread.Abort();
                    isReading = false;
                }
                btnRead.Enabled = false;
                btnRead.Text = "Start Reading";
                serialPort.Close();
                comButton.Text = "Open";
            }
        }

        /// <summary>
        /// Read/Stop read button click event action
        /// hitRead thread will start and keep reading
        /// if it is "Stop Reading", then the thread will kill and stop reading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRead_Click_1(object sender, EventArgs e)
        {
            /**
             * Knows ISSUE -> Stop the thread & close the port at exit
             */
            if (!isReading)
            {
                hidReaderThread = new Thread(hidReader);
                hidReaderThread.Start();
                isReading = true;
                btnRead.Text = "Stop Reading";
            }
            else
            {
                hidReaderThread.Abort();
                isReading = false;
                btnRead.Text = "Start Reading";
            }
        }

        /// <summary>
        /// Reading HID device and write on Com port
        /// </summary>
        private void hidReader()
        {
            while (true)
            {
                if (FailNoDevice() || FailDeviceClosed())
                    return;

                HidDeviceData hddData = mhdDevice.Read();
                vsText = BitConverter.ToString(hddData.Data);
                serialPort.WriteLine(vsText);
           
                Thread.Sleep(1);
            }
        }
    }
}
