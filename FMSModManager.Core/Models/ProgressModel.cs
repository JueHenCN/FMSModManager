using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Models
{
    public class ProgressModel : IProgress<float>
    {
        float lastvalue = 0;

        public Action<float> ProgressChanged { get; set; }

        public void Report(float value)
        {
            if (lastvalue >= value) return;
            lastvalue = value;
            ProgressChanged?.Invoke(value);
        }
    }
}
