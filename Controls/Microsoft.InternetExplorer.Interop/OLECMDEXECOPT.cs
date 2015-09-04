#define USE_INTERNAL_SHDOCVW
// # define USE_AURIGMA_SHDOCVW

#if USE_INTERNAL_SHDOCVW

using System;

namespace Microsoft.InternetExplorer.Interop
{
    public enum OLECMDEXECOPT
    {
        OLECMDEXECOPT_DODEFAULT = 0,
        OLECMDEXECOPT_PROMPTUSER = 1,
        OLECMDEXECOPT_DONTPROMPTUSER = 2,
        OLECMDEXECOPT_SHOWHELP = 3
    }
}

#endif