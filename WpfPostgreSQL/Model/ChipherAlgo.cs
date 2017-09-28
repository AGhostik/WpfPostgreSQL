using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPostgreSQL.Model
{
    public enum ChipherAlgo
    {
        NoCrypt,
        Blowfish,
        AES_128,
        AES_192,
        AES_256,
        ThripleDes,
        Cast5
    }
}
