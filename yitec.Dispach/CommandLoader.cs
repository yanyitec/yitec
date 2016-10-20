using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public class CommandLoader : ICommandLoader
    {
        public CommandLoader(ICommandBuilder commandBuilder) {
            this.CommandBuilder = commandBuilder;
        }
        public ICommandFactory Load()
        {
            throw new NotImplementedException();
        }

        public ICommandBuilder CommandBuilder { get; private set; }
    }
}
