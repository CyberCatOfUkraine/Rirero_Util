using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FR_Util
{
        [XmlRoot(ElementName = "Settings")]
        public class ServiceSettingsForRRO
        {
        [XmlElement("ServiceSettings")]  // Вказуємо, що це елемент ServiceSettings
        public ServiceSettingsWithDataForPRRO ServiceSettings { get; set; }
        }

    public class ServiceSettingsWithDataForPRRO
    {
        [XmlElement(ElementName = "IpAddress")]
        public string IpAddress { get; set; }

        [XmlElement(ElementName = "IpPort")]
        public string IpPort { get; set; }

        [XmlElement(ElementName = "ComPort")]
        public string ComPort { get; set; }

        [XmlElement(ElementName = "ComBods")]
        public string ComBods { get; set; }
    }

    [XmlRoot(ElementName = "Settings")]
        public class ServiceSettingsForPRRO
        {

        [XmlElement("ServiceSettings")]  // Вказуємо, що це елемент ServiceSettings
        public ServiceSettingsWithDataForRRO ServiceSettings { get; set; }

    }

    public class ServiceSettingsWithDataForRRO
    {
        [XmlElement(ElementName = "IpAddress")]
        public string IpAddress { get; set; }

        [XmlElement(ElementName = "IpPort")]
        public string IpPort { get; set; }

        [XmlElement(ElementName = "CashierSignature")]
        public string CashierSignature { get; set; }

        [XmlElement(ElementName = "FiscalNumber")]
        public string FiscalNumber { get; set; }
    }

}