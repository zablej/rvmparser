using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.Api
{
    public interface INext<T>
    {
        T next { get; set; }
    }
}
