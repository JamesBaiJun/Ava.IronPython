using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ava.IronPython.Models
{
    public partial class BreakPoint : ObservableObject
    {
        public int Row { get; set; }

        [ObservableProperty]
        bool enable;

        [ObservableProperty]
        bool isHit;

        public void SwitchPoint()
        {
            Enable = !Enable;
        }
    }
}
