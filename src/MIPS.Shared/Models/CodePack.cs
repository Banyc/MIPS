using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MIPS.Shared.Models
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
            AddCode(code);
        }

        private void AddCode(List<Word32b> code)
        {
            foreach (var word in code)
            {
                _codeList.Add(word);
            }
        }

        public MachineCodePack(string fullMachingCode, CodingSystem coding)
        {
            List<Word32b> code = new List<Word32b>();
            fullMachingCode = fullMachingCode.Replace(" ", "");
            fullMachingCode = fullMachingCode.Replace("\n", "");

            int cursor = 0;
            while (cursor < fullMachingCode.Length)
            {
                // get sub string
                string subString = "";
                switch (coding)
                {
                    case CodingSystem.Binary:
                        subString = fullMachingCode.Substring(cursor, 32);
                        break;
                    case CodingSystem.Hex:
                        subString = fullMachingCode.Substring(cursor, 8);
                        break;
                }
                // pack up word
                Word32b word = new Word32b(subString, coding);
                code.Add(word);
                // find next word
                switch (coding)
                {
                    case CodingSystem.Binary:
                        cursor += 32;
                        break;
                    case CodingSystem.Hex:
                        cursor += 8;
                        break;
                }
            }
            AddCode(code);
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
            StringBuilder hex = new StringBuilder();
            foreach (var word in this._codeList)
            {
                hex.Append(word.ToHexString());
                if (isWithSpaces)
                    hex.Append(" ");
            }
            return hex.ToString();
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
