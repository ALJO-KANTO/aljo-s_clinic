using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    public partial class MasterRecycle : Form
    {
        public MasterRecycle(string v)
        {
            InitializeComponent();
        }

        public Action<object, EventArgs> DataRestored { get; internal set; }
    }
}
