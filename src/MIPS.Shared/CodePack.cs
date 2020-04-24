using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MIPS.Shared
{
    // Manage a list of words as code
    // this class responsible for any type or format convertion within the code
    public class CodePack : IEnumerator
    {
        private List<Word32b> _code = new List<Word32b>();
        private int _position = 0;

        public object Current => _code[_position];

        public CodePack() {}

        public CodePack(List<Word32b> code)
        {
            foreach (var word in code)
            {
                _code.Add(word);
            }
        }

        public string ToBinaryString(bool isWithNewLines)
        {
            StringBuilder binary = new StringBuilder();
            foreach (var word in this._code)
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
            foreach (var word in this._code)
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
            foreach (var word in this._code)
            {
                string wordBinStr = word.ToBinaryString();
                Instruction instruction = new Instruction(wordBinStr);
                mips.Append(instruction.ToMipsString());
            }
            return mips.ToString();
        }

        // TODO: review
        public bool MoveNext()
        {
            if (_position >= _code.Count)
                return false;
            _position++;
            return true;
        }

        public void Reset()
        {
            _position = 0;
        }
    }
}
