using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using SAIL.Framework.Host;
using SAIL.Framework.Host.BaseClasses;

namespace IQ.BUS.Vast.Common.Types
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class AuthenticationToken : ViewModelResultBase, IAuthToken 
    {
        // Token supplied by the Authentication Provider
        private string _accessToken = string.Empty;

        [DataMember(Order = 1)]
        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }

        // Used with OAuth
        private Enums.TokenType _tokenType = Enums.TokenType.Bearer;

        [DataMember(Order = 2)]
        public Enums.TokenType TokenType
        {
            get { return _tokenType; }
            set { _tokenType = value; }
        }

        // Expire every X milliseconds
        private int _expiresInMilliseconds = 600000;    // 10 minutes

        [DataMember(Order = 3)]
        public int ExpiresInMilliseconds
        {
            get { return _expiresInMilliseconds; }
            set { _expiresInMilliseconds = value; }
        }

        // Used to regenerate once expired.
        private string _refreshToken = string.Empty;

        [DataMember(Order = 4)]
        public string RefreshToken
        {
            get { return _refreshToken; }
            set { _refreshToken = value; }
        }

        // Date/Time token issued
        private DateTime _issued = DateTime.Now;

        [DataMember(Order = 5)]
        public DateTime Issued
        {
            get { return _issued; }
            set { _issued = value; }
        }

        // For use with LDAP
        private string _filterAttribute = string.Empty;

        [DataMember(Order = 6)]
        public string FilterAttribute
        {
            get { return _filterAttribute; }
            set { _filterAttribute = value; }
        }

        // For use wiht LDAP
        private string _path = string.Empty;

        [DataMember(Order = 7)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        // For performance it may make since to return groups at the same time as authenticiaton.
        private List<string> _groupNameList = null;

        [DataMember(Order = 8)]
        public List<string> GroupNameList
        {
            get { return _groupNameList; }
            set { _groupNameList = value; }
        }

        private bool _isAuthenticated = false;

        [DataMember(Order = 9)]
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set { _isAuthenticated = value; }
        }

        private string _defaultModuleID = string.Empty;

        [DataMember(Order = 10)]
        public string DefaultModuleID
        {
            get { return _defaultModuleID; }
            set { _defaultModuleID = value; }
        }

        private string _defaultFormID = string.Empty;

        [DataMember(Order = 11)]
        public string DefaultFormID
        {
            get { return _defaultFormID; }
            set { _defaultFormID = value; }
        }

        private string _defaultActionType = string.Empty;

        [DataMember(Order = 12)]
        public string DefaultActionType
        {
            get { return _defaultActionType; }
            set { _defaultActionType = value; }
        }

        private string _firstName = string.Empty;

        [DataMember(Order = 13)]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _lastName = string.Empty;

        [DataMember(Order = 13)]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        private string _sysLoginPk = string.Empty;

        [DataMember(Order = 14)]
        public string SysLoginPk
        {
            get { return _sysLoginPk; }
            set { _sysLoginPk = value; }
        }

        private string _logSetting = string.Empty;

        [DataMember(Order = 15)]
        public string LogSetting
        {
            get { return _logSetting; }
            set { _logSetting = value; }
        }

        bool IAuthToken.Load(IContext context)
        {
            throw new NotImplementedException();
        }

        string IAuthToken.Read(IContext context)
        {
            return string.Empty;
        }

        void IAuthToken.SetClaim(string claimType, string claimValue)
        {
            throw new NotImplementedException();
        }
    }
}
