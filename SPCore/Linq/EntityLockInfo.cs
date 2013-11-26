using System;
using Microsoft.SharePoint;

namespace SPCore.Linq
{
    public sealed class EntityLockInfo
    {
        public SPFile.SPLockType LockType { get; set; }
        public string LockId { get; set; }
        public DateTime LockExpires { get; set; }
        public DateTime LockedDate { get; set; }
        public SPUser LockedByUser { get; set; }

        public EntityLockInfo(SPFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            LockType = file.LockType;
            LockId = file.LockId;
            LockExpires = file.LockExpires;
            LockedByUser = file.LockedByUser;
            LockedDate = file.LockedDate;
        }
    }
}
