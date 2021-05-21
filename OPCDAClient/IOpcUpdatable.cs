using System;
using System.Collections.Generic;
using System.Text;

namespace Intma.OPCDAClient
{
    public interface IOpcUpdatable
    {
        void OPCUpdate(int quality, object value, string timeStamp);
    }
}
