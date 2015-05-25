using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;

namespace SysMo_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string hName = System.Environment.MachineName;
            string dName = System.Environment.UserDomainName;
            int upTime = System.Environment.TickCount;
            int pFileSize = System.Environment.SystemPageSize;
            int procCount = System.Environment.ProcessorCount;
            DriveInfo[] drives = DriveInfo.GetDrives();
            string infoDrives = "";
            foreach (DriveInfo drive in drives)
            {
                
                if (drive.IsReady)
                {
                    //Console.WriteLine(drive.Name + drive.TotalFreeSpace + drive.TotalSize + "\n");
                    infoDrives = infoDrives + drive.Name + ", Total: " + (drive.TotalFreeSpace / (1024 * 1024 * 1024)) + "GB, Free: " + (drive.TotalSize / (1024 * 1024 * 1024)) + "GB\n";
                }
            }
        

            comp.Content = "Hostname: " + hName + ", Domain: " + dName + "\nUptime: " + (upTime/(3600*1000)) + " Stunden\nAnzahl CPUs: " + procCount + "\nRam Total: " + "\nPagefilesize: " + pFileSize + "\nDisks: " + infoDrives;


        }

        private void onAnmelden(object sender, RoutedEventArgs e)
        {
            string ipAddr = ip.Text;
            string username = un.Text;
            string pw = pass.Text;
            SqlConnection myConnection = new SqlConnection("user id=sysmo;" +
                           "password=omsys!321;server="+ipAddr+";" +
                           "Trusted_Connection=yes;" +
                           "database=SysMo; " +
                           "connection timeout=30");

            try
            {
                myConnection.Open();
                statusT.Text = "Successfully connected to SysMo";
                statusT.Background = Brushes.Green;
            }
            catch (Exception f)
            {
                info.Content =  "Connection failed: " + f.ToString();
            }

            try
            {
                SqlDataReader myReader = null;
                string sqlS = "select Usr_ID, Vorname, Nachname, EMail from Usr where Username ='" + username + "' and PW ='" + pw + "'";
                SqlCommand myCommand = new SqlCommand(sqlS, myConnection);
                myReader = myCommand.ExecuteReader();
                int i = 0;
                while (myReader.Read())
                {
                    ++i;
                    info.Content = myReader["Usr_ID"].ToString() + ", " + myReader["Vorname"].ToString() + ", " + myReader["Nachname"].ToString();
                }
                if (i == 0)
                {
                    statusPW.Text = "Zugangsdaten falsch: ";
                    statusPW.Background = Brushes.Tomato;
                    //info.Content = i;
                }
                else
                {
                    statusPW.Text = "Zugangsdaten OK";
                    statusPW.Background = Brushes.Green;

                    myConnection.Close();
                    myConnection.Open();
                    SqlDataReader readReg = null;
                    SqlCommand isRegistered = new SqlCommand("select count(Client_ID) from Client where Hostname = '" + System.Environment.MachineName + "'", myConnection);
                    readReg = isRegistered.ExecuteReader();
                    int z = 0;
                    while (readReg.Read())
                    {
                        z = Convert.ToInt32(readReg[0].ToString());
                    }
                    if (z == 1)
                    {
                        statusC.Text = "Registriert!!";
                    }
                    else
                    {
                        info.Content = "Client " + System.Environment.MachineName + "nicht registriert - registriere";
                        myConnection.Close();
                        myConnection.Open();
                        //SqlDataReader rReg = null;
                        SqlCommand Reg = new SqlCommand("insert into Client(Hostname, Dept_ID, Type_ID, SerNr, Comment) values('" + System.Environment.MachineName + "', 6, 1, 'PC003435643', 'Was soll ich dazu sagen')", myConnection);
                        info.Content = Reg.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception f)
            {
                info.Content = "Select failed: " + f.ToString();
            }
            try
            {
                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }
 
        }
    }
}
