using System;
using Solu.Framework.Services;

namespace Solu.Service
{
    public class Record : Idendity, IRecord
    {
        public short RecordStatus
        {
            get;
            set;
        }

        public DateTime RecordInsertDate
        {
            get;
            set;
        }

        public DateTime RecordLastEditDate
        {
            get;
            set;
        }
    }
}
