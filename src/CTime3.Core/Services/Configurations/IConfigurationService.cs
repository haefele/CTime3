using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CTime3.Core.Services.CTime;

namespace CTime3.Core.Services.Configurations
{
    public interface IConfigurationService
    {
        Configuration Config { get; }

        Task Modify(Func<Configuration, Configuration> changeAction);
    }
}
