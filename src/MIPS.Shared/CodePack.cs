using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MIPS.Shared
{
    // Manage a list of words as machine code (not MIPS assembly)
    // this class responsible for any type or format convertion within the code
    public class MachineCodePack : IEnumerable<Word32b>
    {
        private List<Word32b> _codeList = new List<Word32b>();

        public int Count => _codeList.Count;

        public MachineCodePack() {}

        public MachineCodePack(List<Word32b> code)
        {
            foreach (var word in code)
            {
                _codeList.Add(word);
            }
        }

        public void Add(Word32b code)
        {
            _codeList.Add(code);
        }

        public string ToBinaryString(bool isWithNewLines)
        {
            StringBuilder binary = new StringBuilder();
            foreach (var word in this._codeList)
            {
                binary.Append(word.ToBinaryString());
                if (isWithNewLines)
                    binary.Append("\n");
            }
            return binary.ToString();
        }

        public string ToHexString(bool isWithSpaces)
        {
            StringBuilder binary = new StringBuilder();
            foreach (var word in this._codeList)
            {
                binary.Append(word.ToHexString());
                if (isWithSpaces)
                    binary.Append(" ");
            }
            return binary.ToString();
        }

        public string ToMipsString()
        {
            StringBuilder mips = new StringBuilder();
            foreach (var word in this._codeList)
            {
                mips.Append(word.ToMipsString());
            }
            return mips.ToString();
        }

        public IEnumerator<Word32b> GetEnumerator()
        {
            return _codeList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _codeList.GetEnumerator();
        }
    }
}
