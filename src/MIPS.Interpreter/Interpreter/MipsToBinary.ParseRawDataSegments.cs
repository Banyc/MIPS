using System.Xml.Linq;
using System.Net.Sockets;
using System;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MIPS.Shared;
using MIPS.Shared.Models;

namespace MIPS.Interpreter.Interpreter
{
    // main entry for the interpreter
    public partial class MipsToBinary
    {
        private const string _dataBegin = "data begin";
        private const string _dataEnd = "data end";

        private ProgramInfo ParseRawDataSegments(ProgramInfo programInfo, string input)
        {
            int startIndex = 0;
            do
            {
                startIndex = ParseFirstRawDataSegment(programInfo, input, startIndex);
            } while (startIndex >= 0);

            return programInfo;
        }

        private int ParseFirstRawDataSegment(ProgramInfo programInfo, string input, int startIndex)
        {
            // find start and end
            int dataBeginTag = input.IndexOf(_dataBegin, startIndex);
            int dataEndTag = input.IndexOf(_dataEnd, startIndex);

            if (dataBeginTag < 0 || dataEndTag < 0)
            {
                return -1;
            }

            // remove the first # from each line
            string segmentRaw = input.Substring(dataBeginTag + _dataBegin.Length, dataEndTag - (dataBeginTag + _dataBegin.Length));
            string segment = RemoveHeadCommentMarks(segmentRaw).Trim();

            // get start address
            string[] elements = segment.Split();

            RawDataSegment rawDataSegment = new RawDataSegment();
            rawDataSegment.StartAddress = uint.Parse(elements[0]);

            // get radix
            CodingSystem codingSystem = CodingSystem.Binary;
            if (elements[1] == "2")
            {
                codingSystem = CodingSystem.Binary;
            }
            else if (elements[1] == "16")
            {
                codingSystem = CodingSystem.Hex;
            }

            // get raw data
            int i;
            for (i = 2; i < elements.Length; i++)
            {
                switch (codingSystem)
                {
                    case CodingSystem.Binary:
                        rawDataSegment.Data.Add(
                            Convert.ToByte(elements[i], 2)
                        );
                        break;
                    case CodingSystem.Hex:
                        rawDataSegment.Data.Add(
                            Convert.ToByte(elements[i], 16)
                        );
                        break;
                    default:
                        break;
                }
            }
            programInfo.RawDataSegments.Add(rawDataSegment);
            return dataEndTag;
        }

        private string RemoveHeadCommentMarks(string lines)
        {
            string[] lineArray = lines.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int i;
            StringBuilder stringBuilder = new StringBuilder();
            for (i = 0; i < lineArray.Length; i++)
            {
                lineArray[i] = RemoveOneHeadCommentMark(lineArray[i]);
                stringBuilder.Append(lineArray[i]);
            }
            return stringBuilder.ToString();
        }

        private string RemoveOneHeadCommentMark(string line)
        {
            line = line.TrimStart();
            if (line.Length > 0 && line.Substring(0, 1) == "#")
            {
                line = line.Substring(1);
            }
            return line.TrimStart();
        }
    }
}
