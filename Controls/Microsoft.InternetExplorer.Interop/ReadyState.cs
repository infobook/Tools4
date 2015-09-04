#define USE_INTERNAL_SHDOCVW
// # define USE_AURIGMA_SHDOCVW

#if USE_INTERNAL_SHDOCVW

using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.InternetExplorer.Interop
{
    public enum ReadyState
    {
        Unitialized     = 0,
        Loading         = 1,
        Loaded          = 2,
        Interactive     = 3,
        Complete        = 4
    }
}

#endif