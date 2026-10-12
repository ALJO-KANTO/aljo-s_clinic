using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.global_classes
{
    public class clsEventArgs : EventArgs
    {
        public int ID { get;}
        public int PersonID { get; }

        public clsEventArgs(int ID, int PersonID)
        {
            this.ID = ID;
            this.PersonID = PersonID;
        }
    }
}
