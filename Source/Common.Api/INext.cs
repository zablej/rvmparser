using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api
{
    public interface INext<T>
    {
        T next { get; set; }
    }
}
