using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using MIPS.Shared.Helpers;

namespace MIPS.Shared.Models
{
    // this class only serves for AST (abstract syntax tree)
    // the class does not take responsibility for disassembled MIPS codes
    public class ProgramInfo
    {
        public List<Statement> Statements = new List<Statement>();

        public List<RawDataSegment> RawDataSegments = new List<RawDataSegment>();

        public MachineCodePack ToMachineCode()
        {
            List<Word32b> code = new List<Word32b>();
            // write text segment
            foreach (var stat in this.Statements)
            {
                string binary = stat.Instruction.ToBinaryString();
                Word32b word = new Word32b(binary, CodingSystem.Binary);
                code.Add(word);
            }
            // overwrite with raw data segments
            code = Decorate(code);

            MachineCodePack codePack = new MachineCodePack(code);

            return codePack;
        }

        private List<Word32b> Decorate(List<Word32b> code)
        {
            foreach (RawDataSegment rawDataSegment in this.RawDataSegments)
            {
                var sizeToEndOfRawDataSegment = (rawDataSegment.StartAddress + rawDataSegment.Data.Count);
                var wordSizeCeiling = sizeToEndOfRawDataSegment / 4
                    + (sizeToEndOfRawDataSegment % 4 == 0 ? 0 : 1);
                while (
                    wordSizeCeiling + 1 > code.Count)
                {
                    code.Add(new Word32b("00000000", CodingSystem.Hex));
                }

                int i;
                for (i = 0; i < rawDataSegment.Data.Count; i++)
                {
                    uint currentAddress = rawDataSegment.StartAddress + (uint)i;
                    int index;
                    int offset;
                    (index, offset) = Pagination.GetTwoLevelIndex((int)currentAddress, 4);
                    Word32b word = code[index];
                    // change endian
                    word.Value[4 - offset - 1] = rawDataSegment.Data[i];
                }
            }

            return code;
        }
    }
}
