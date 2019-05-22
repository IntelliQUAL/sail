using System;
namespace SAIL.Framework.Host
{
    public interface IWebsiteReader
    {
        string DisplayName { get; }
        string LogoUrl { get; }
        string HomePageUrl { get; }
        string PartnerTopLevelEntityKeyCRM { get; }
        string FtpServer { get; }
        string FtpUser { get; }
        string FtpPassword { get; }
        string FtpRemoteDirectory { get; }
        string FtpWebsiteBaseUrl { get; }
    }
}
