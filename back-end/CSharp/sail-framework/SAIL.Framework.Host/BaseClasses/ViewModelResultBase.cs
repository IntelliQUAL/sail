using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using SAIL.Framework.Host.Types;

namespace SAIL.Framework.Host.BaseClasses
{
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class ViewModelResultBase : IViewModelResult
    {
        private Result _result = new Result();

        [DataMember(EmitDefaultValue = false)]
        public Result Result
        {
            get { return _result; }
            set { _result = value; }
        }

        bool IViewModelResult.Success
        {
            get
            {
                return _result.Success;
            }
            set
            {
                _result.Success = value;
            }
        }

        string IViewModelResult.ErrorCode
        {
            get
            {
                return _result.ErrorCode;
            }
            set
            {
                _result.ErrorCode = value;
            }
        }

        string IViewModelResult.ErrorText
        {
            get
            {
                return _result.ErrorText;
            }
            set
            {
                _result.ErrorText = value;
            }
        }

        string IViewModelResult.UserMessage
        {
            get
            {
                return _result.UserMessage;
            }
            set
            {
                _result.UserMessage = value;
            }
        }
    }
}
