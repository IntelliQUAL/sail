using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace IQ.BUS.Vast.Helpers
{
    public class WebsiteReader : IWebsiteReader
    {
        private string _instanceStatus = string.Empty;
        private string _instanceGuid = string.Empty;

        public WebsiteReader(string instanceGuid, string instanceStatus)
        {
            _instanceGuid = instanceGuid.Trim();
            _instanceStatus = instanceStatus.Trim().ToLower();

            // Blank out prod
            if (_instanceStatus == IQ.ViewModel.Vast.Enums.InstanceStatus.Prod.ToString().ToLower())
            {
                _instanceStatus = string.Empty;
            }
        }

        string IWebsiteReader.DisplayName
        {
            get { return "InteliQUAL Cloud"; }
        }

        string IWebsiteReader.HomePageUrl
        {
            get { return "http://inteliqual.com"; }
        }

        string IWebsiteReader.LogoUrl
        {
            get { return "http://inteliqual.com/images/logo.png"; }
        }

        string IWebsiteReader.PartnerTopLevelEntityKeyCRM
        {
            get { return "IQCLOUD"; }
        }

        string IWebsiteReader.FtpPassword
        {
            get { return "!VAST2015"; }
        }

        string IWebsiteReader.FtpRemoteDirectory
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("/");

                if (string.IsNullOrWhiteSpace(_instanceStatus) == false)
                {
                    sb.Append(_instanceStatus);
                    sb.Append("/");
                }

                return sb.ToString();
            }
        }

        string IWebsiteReader.FtpServer
        {
            get { return "ftp.automatedofficesystems.com"; }
        }

        string IWebsiteReader.FtpUser
        {
            get { return "vast@automatedofficesystems.com"; }
        }


        string IWebsiteReader.FtpWebsiteBaseUrl
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("http://inteliqual.com/vast/");

                if (string.IsNullOrWhiteSpace(_instanceStatus) == false)
                {
                    sb.Append(_instanceStatus);
                    sb.Append("/");
                }

                return sb.ToString();
            }
        }
    }
}
