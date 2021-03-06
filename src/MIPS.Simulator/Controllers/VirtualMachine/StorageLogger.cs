using System.Collections.Generic;
using MIPS.Shared;
using MIPS.Shared.Models;

namespace MIPS.Simulator.Controllers.VirtualMachine
{
    public class StorageLogger<T>
    {
        private readonly Dictionary<T, (Word32b oldValue, Word32b newValue)> _log
             = new Dictionary<T, (Word32b oldValue, Word32b newValue)>();

        public bool IsEmpty { get => _log.Count == 0; }
        public void Clear()
        {
            _log.Clear();
        }
        public void LogChange(T address, Word32b oldValue, Word32b newValue)
        {
            if (_log.ContainsKey(address))
            {
                _log[address] = (_log[address].oldValue, newValue);
            }
            else
            {
                _log[address] = (oldValue, newValue);
            }
        }
        public Dictionary<T, (Word32b oldValue, Word32b newValue)> GetLog()
        {
            return _log;
        }
    }
}
