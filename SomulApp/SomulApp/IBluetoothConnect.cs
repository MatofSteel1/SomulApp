using System;
using System.Collections.Generic;
using System.Text;

namespace SomulApp
{
    public interface IBluetoothConnect
    {
        bool Connect();
        bool Disconnect();
        void BeginListenForData();
        void WriteData(string data);
    }
}
