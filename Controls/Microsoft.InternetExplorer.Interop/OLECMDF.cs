#define USE_INTERNAL_SHDOCVW
// # define USE_AURIGMA_SHDOCVW
 
#if USE_INTERNAL_SHDOCVW

using System;

namespace Microsoft.InternetExplorer.Interop
{
    [Flags]
    public enum OLECMDF
    {
        OLECMDF_SUPPORTED = 1,
        OLECMDF_ENABLED = 2,
        OLECMDF_LATCHED = 4,
        OLECMDF_NINCHED = 8,
        OLECMDF_INVISIBLE = 16,
        OLECMDF_DEFHIDEONCTXTMENU = 32
    }
 
}

#endif