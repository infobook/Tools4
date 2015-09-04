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
    public enum CommandStateChangeConstants
    {
        UpdateCommands  = -1,
        NavigateForward =  1,
        NavigateBack    =  2
    } 
}

#endif