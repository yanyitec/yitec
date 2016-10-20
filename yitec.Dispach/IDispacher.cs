using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    /// <summary>
    /// 分派器
    /// </summary>
    public interface IDispacher
    {
        Task<object> DispachAsync(object rawContext);
    }
}
