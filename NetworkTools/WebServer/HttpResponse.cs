﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Marler.NetworkTools
{
    public class HttpResponse
    {
        public Int32 status;
        public String version;

        public Dictionary<String, String> Headers;

        public MemoryStream bodyStream;
        //public Byte[] BodyData;

        public HttpResponse()
        {
            this.bodyStream = new MemoryStream();
        }

        public override String ToString()
        {
            return String.Format("HttpResponse status='{0}' version='{1}' Headers='{2}' BodySize='{3}']",
                status, version, (Headers == null) ? "<null>" : GenericUtilities.GetString(Headers), bodyStream.Length);
        }
    }
}
