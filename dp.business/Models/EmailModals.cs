using System;
using System.Collections.Generic;
using System.Text;

namespace dp.business.Models
{
    public class EmailAddresses
    {

        public string SenderEmail { get; set; }
        public string AdminEmail { get; set; }
        public string SupportEmail { get; set; }
    }

    public class EmailConfig
    {

        public EmailAddresses EmailAddresses { get; set; }
        public string SendGridKey { get; set; }
    }
}
