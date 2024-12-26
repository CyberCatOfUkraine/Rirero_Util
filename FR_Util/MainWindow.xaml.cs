using System.Windows;
using System.Windows.Shapes;
using System.Management;
using System.IO;
using Path = System.IO.Path;
using System.Xml.Serialization;

namespace FR_Util
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                SeedFRInfoTable();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void SeedFRInfoTable()
        {
            var fr_services = new List<FiscalRegistar>();
            
            //Для тесту
            //fr_services.Add(new FiscalRegistar() { FR_Type= FR_Type.PRRO.To_String_UA(),FR_IP="192.168.20.101",FR_FN="40012332", FR_Port= "2009",FR_Signature="this prosto text", FR_Path="Fr"});

            // Створення пошукового запиту для всіх служб
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");

            // Виконання запиту
            foreach (ManagementObject service in searcher.Get())
            {
                //Перевіряємо чи це одна з наших служб
                if ((service["Name"]as string).Substring(0,3)=="HMS")
                {
                    var fr = new FiscalRegistar();

                    #region Отримуємо назву служби
                    fr.FR_Name = service["Name"].ToString();
                    #endregion

                    #region Отримуємо тип служби
                    if (fr.FR_Name.Contains("UAPRRO"))
                    {
                        fr.FR_Type=FR_Type.PRRO.To_String_UA();
                        fr.eFr_Type = FR_Type.PRRO;
                    }
                    else 
                    if (fr.FR_Name.Contains("FR"))
                    {
                        fr.FR_Type = FR_Type.RRO.To_String_UA();
                        fr.eFr_Type = FR_Type.RRO;
                    }
                    else
                    {
                        fr.FR_Type = FR_Type.Unknown.ToString();
                        fr.eFr_Type = FR_Type.Unknown;
                    }
                    #endregion


                    #region Отримуємо шлях до служби

                    fr.FR_Path = service["PathName"].ToString().Substring(0, service["PathName"].ToString().LastIndexOf("\\")).Replace("\"", "");

                    #endregion
                    try
                    {


                    //Далі читаємо дані з конфігу
                    var cfg =File.ReadAllText(Path.Combine(fr.FR_Path, "ServiceSettings.xml"));

                    #region Парсим для ПРРО

                    if (fr.eFr_Type==FR_Type.PRRO)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ServiceSettingsForPRRO));

                        // Створення StringReader для обробки XML тексту
                        using (StringReader reader = new StringReader(cfg))
                        {
                            // Десеріалізація XML в об'єкт типу Book
                            var prro_config = (ServiceSettingsForPRRO)serializer.Deserialize(reader);
                            fr.FR_IP = prro_config.ServiceSettings.IpAddress;
                            fr.FR_Port = prro_config.ServiceSettings.IpPort;
                            fr.FR_FN = prro_config.ServiceSettings.FiscalNumber;
                            fr.FR_Signature = prro_config.ServiceSettings.CashierSignature;
                        }
                    }
                    #endregion



                    #region Парсим для РРО

                    if (fr.eFr_Type == FR_Type.RRO)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ServiceSettingsForRRO));

                        // Створення StringReader для обробки XML тексту
                        using (StringReader reader = new StringReader(cfg))
                        {
                            // Десеріалізація XML в об'єкт типу Book
                            var rro_config = (ServiceSettingsForRRO)serializer.Deserialize(reader);

                            fr.FR_IP = rro_config.ServiceSettings.IpAddress;
                            fr.FR_Port = rro_config.ServiceSettings.IpPort;
                            fr.FR_COMPort=rro_config.ServiceSettings.ComPort;
                            fr.FR_ComBods = rro_config.ServiceSettings.ComBods;
                        }
                    }
                        #endregion

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    fr_services.Add(fr);

                }
            }

            dgFiscalRegistars.ItemsSource = fr_services;
        }
    }
    public enum FR_Type
    {
        PRRO, RRO,Unknown
    }
    public static class FR_Extension
    {
        public static string To_String_UA(this FR_Type type)
        {
            return type == FR_Type.PRRO ? "ПРРО" : "РРО";
        }
    }
    public class FiscalRegistar()
    {
        public string FR_Type { get; set; }
        public string FR_Name { get; set; }
        public string FR_IP { get; set; }
        public string FR_Port { get; set; }
        public string FR_COMPort { get; set; }
        public string FR_ComBods { get; set; }
        public string FR_FN { get; set; }
        public string FR_Signature { get; set; }
        public string FR_Path { get; set; }

        public FR_Type eFr_Type { get; set; }
    }
}