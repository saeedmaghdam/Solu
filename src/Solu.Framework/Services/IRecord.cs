using System;

namespace Solu.Framework.Services
{
    public interface IRecord : IIdentify
    {
        short RecordStatus
        {
            get;
            set;
        }


        DateTime RecordInsertDate
        {
            get;
            set;
        }


        DateTime RecordLastEditDate
        {
            get;
            set;
        }
    }
}
