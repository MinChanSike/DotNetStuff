﻿using System;
using More.Net.Nfs3Procedure;

namespace More.Net
{
#if !WindowsCE
    [NpcInterface]
#endif
    public interface INfs3ServerNiceInterface
    {
        String[] RootShareNames();
        ShareObject[] ShareObjects();
        FileSystemStatusReply FSStatusByName(String directory);
        FSInfoReply FSInfoByName(String directory);
        ReadDirPlusReply ReadDirPlus(String directoryName, UInt64 cookie, UInt32 maxDirectoryInfoBytes);
    }
}