﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using More.Net.Nfs3Procedure;

namespace More.Net
{
    public interface IPermissions
    {
        ModeFlags GetPermissions(ShareObject shareObject);
    }

    public class ConstantPermissions : IPermissions
    {
        public readonly ModeFlags defaultDirectoryPermissions;
        public readonly ModeFlags defaultFilePermissions;

        public ConstantPermissions(Nfs3Procedure.ModeFlags defaultDirectoryPermissions,
            ModeFlags defaultFilePermissions)
        {
            this.defaultDirectoryPermissions = defaultDirectoryPermissions;
            this.defaultFilePermissions = defaultFilePermissions;
        }
        public ModeFlags GetPermissions(ShareObject shareObject)
        {
            return (shareObject.fileType == Nfs3Procedure.FileType.Directory) ? defaultDirectoryPermissions :
                defaultFilePermissions;
        }
    }

    /*
    public class WindowsPermissions : IPermissions
    {
        readonly AccessControlSections controlSections;
        public WindowsPermissions()
        {
            this.controlSections = AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner;
        }
        public ModeFlags GetPermissions(FileInfo fileInfo)
        {
            FileSecurity fileSecurity = fileInfo.GetAccessControl(controlSections);

        }
    }
    */
}
