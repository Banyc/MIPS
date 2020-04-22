using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace MIPS_interpreter.Interpreter
{
    public class MipsAsmVisitorToBinary : MipsAsmBaseVisitor<object>
    {
        Dictionary<string, object> memory = new Dictionary<string, object>();

        public override object VisitIden([NotNull] MipsAsmParser.IdenContext context)
        {
            // don't Visit(context.iden())
            // context.iden().GetText() instead
            throw new System.NotImplementedException();
        }

        public override object VisitInstr([NotNull] MipsAsmParser.InstrContext context)
        {
            object visitRet = null;
            visitRet = Visit(context.children[0]);
            Instruction ret = (Instruction)visitRet;
            return ret;
        }

        public override object VisitInstr_i([NotNull] MipsAsmParser.Instr_iContext context)
        {
            Instruction ret = null;
            foreach (var child in context.children)
            {
                ret = (Instruction)Visit(child);
            }
            ret.Type = FormatType.Immediate;
            return ret;
        }

        public override object VisitInstr_j([NotNull] MipsAsmParser.Instr_jContext context)
        {
            Instruction ret = null;
            foreach (var child in context.children)
            {
                ret = (Instruction)Visit(child);
            }
            ret.Type = FormatType.Jump;
            return ret;
        }

        public override object VisitInstr_r([NotNull] MipsAsmParser.Instr_rContext context)
        {
            Instruction ret = null;
            foreach (var child in context.children)
            {
                ret = (Instruction)Visit(child);
            }
            ret.Type = FormatType.Register;
            return ret;
        }

        public override object VisitInstr_pseudo([NotNull] MipsAsmParser.Instr_pseudoContext context)
        {
            Instruction ret = (Instruction)Visit(context.children[0]);
            // Format type has been set in advanced
            return ret;
        }

        public override object VisitLabel([NotNull] MipsAsmParser.LabelContext context)
        {
            Label label = new Label();
            label.Identity = context.iden().GetText();
            return label;
        }

        public override object VisitOp_add([NotNull] MipsAsmParser.Op_addContext context)
        {
            Instruction obj = new Instruction();
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Rd = (RegisterType)Visit(context.rd);
            obj.Funct = Funct.add;
            return obj;
        }

        public override object VisitOp_addi([NotNull] MipsAsmParser.Op_addiContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.addi;
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Immediate = (int)Visit(context.signed_imm());
            return obj;
        }

        public override object VisitOp_addiu([NotNull] MipsAsmParser.Op_addiuContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.addiu;
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Immediate = (int)Visit(context.usigned_imm());
            return obj;
        }

        public override object VisitOp_addu([NotNull] MipsAsmParser.Op_adduContext context)
        {
            Instruction obj = new Instruction();
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Rd = (RegisterType)Visit(context.rd);
            obj.Funct = Funct.addu;
            return obj;
        }

        public override object VisitOp_and([NotNull] MipsAsmParser.Op_andContext context)
        {
            Instruction obj = new Instruction();
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Rd = (RegisterType)Visit(context.rd);
            obj.Funct = Funct.and;
            return obj;
        }

        public override object VisitOp_andi([NotNull] MipsAsmParser.Op_andiContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.addi;
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Immediate = (int)Visit(context.usigned_imm());
            return obj;
        }

        public override object VisitOp_beq([NotNull] MipsAsmParser.Op_beqContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.beq;
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.TargetLabelIdentity = context.iden().GetText();
            return obj;
        }

        public override object VisitOp_bgtz([NotNull] MipsAsmParser.Op_bgtzContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_bne([NotNull] MipsAsmParser.Op_bneContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.bne;
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.TargetLabelIdentity = context.iden().GetText();
            return obj;
        }

        public override object VisitOp_halt([NotNull] MipsAsmParser.Op_haltContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_j([NotNull] MipsAsmParser.Op_jContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.j;
            obj.TargetLabelIdentity = context.target.GetText(); ;
            return obj;
        }

        public override object VisitOp_jal([NotNull] MipsAsmParser.Op_jalContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.jal;
            obj.TargetLabelIdentity = context.target.GetText(); ;
            return obj;
        }

        public override object VisitOp_jr([NotNull] MipsAsmParser.Op_jrContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_lb([NotNull] MipsAsmParser.Op_lbContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_lbu([NotNull] MipsAsmParser.Op_lbuContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_lh([NotNull] MipsAsmParser.Op_lhContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_lhu([NotNull] MipsAsmParser.Op_lhuContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_lui([NotNull] MipsAsmParser.Op_luiContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_lw([NotNull] MipsAsmParser.Op_lwContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.lw;
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Immediate = (int)Visit(context.signed_imm());
            return obj;
        }

        public override object VisitOp_nand([NotNull] MipsAsmParser.Op_nandContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_nor([NotNull] MipsAsmParser.Op_norContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_nori([NotNull] MipsAsmParser.Op_noriContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_or([NotNull] MipsAsmParser.Op_orContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_ori([NotNull] MipsAsmParser.Op_oriContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_sb([NotNull] MipsAsmParser.Op_sbContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_sh([NotNull] MipsAsmParser.Op_shContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_sll([NotNull] MipsAsmParser.Op_sllContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_slt([NotNull] MipsAsmParser.Op_sltContext context)
        {
            Instruction obj = new Instruction();
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Rd = (RegisterType)Visit(context.rd);
            obj.Funct = Funct.slt;
            return obj;
        }

        public override object VisitOp_sltu([NotNull] MipsAsmParser.Op_sltuContext context)
        {
            Instruction obj = new Instruction();
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Rd = (RegisterType)Visit(context.rd);
            obj.Funct = Funct.sltu;
            return obj;
        }

        public override object VisitOp_slti([NotNull] MipsAsmParser.Op_sltiContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_sra([NotNull] MipsAsmParser.Op_sraContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_srl([NotNull] MipsAsmParser.Op_srlContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitOp_sub([NotNull] MipsAsmParser.Op_subContext context)
        {
            Instruction obj = new Instruction();
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Rd = (RegisterType)Visit(context.rd);
            obj.Funct = Funct.sub;
            return obj;
        }

        public override object VisitOp_sw([NotNull] MipsAsmParser.Op_swContext context)
        {
            Instruction obj = new Instruction();
            obj.Opcode = Opcode.sw;
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Immediate = (int)Visit(context.signed_imm());
            return obj;
        }

        public override object VisitOp_xor([NotNull] MipsAsmParser.Op_xorContext context)
        {
            Instruction obj = new Instruction();
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Rd = (RegisterType)Visit(context.rd);
            obj.Funct = Funct.xor;
            return obj;
        }

        public override object VisitProg([NotNull] MipsAsmParser.ProgContext context)
        {
            ProgramInfo prog = new ProgramInfo();
            int i = 0;
            for (i = 0; i < context.ChildCount; i++)
            {
                Statement statement = (Statement)Visit(context.stat(i));
                if (statement == null)
                    continue;
                prog.Statements.Add(statement);
                // to check if there is other instrutions attached to this instruction
                // pseudo instruction could be split to several intructions
                Instruction instruction = statement.Instruction;
                while (instruction.NextInstruction != null)
                {
                    instruction = instruction.NextInstruction;
                    Statement subStatement = new Statement();
                    subStatement.Instruction = instruction;
                    prog.Statements.Add(subStatement);
                }
            }
            return prog;
        }

        public override object VisitReg([NotNull] MipsAsmParser.RegContext context)
        {
            RegisterType reg = RegisterType.zero;
            string ret = context.children[0].GetText();
            switch (ret)
            {
                case "$zero":
                    reg = RegisterType.zero;
                    break;
                case "$sp":
                    reg = RegisterType.sp;
                    break;
                case "$fp":
                    reg = RegisterType.fp;
                    break;
                case "$ra":
                    reg = RegisterType.ra;
                    break;
                default:
                    reg = (RegisterType)Enum.Parse(typeof(RegisterType), ret.Substring(1));
                    break;
            }
            return reg;
        }

        public override object VisitSigned_imm([NotNull] MipsAsmParser.Signed_immContext context)
        {
            uint unsignedImm = (uint)Visit(context.usigned_imm());
            int signedImm = (int)unsignedImm;
            if (context.ChildCount == 2)
            {
                signedImm = -signedImm;
            }
            return signedImm;
        }

        public override object VisitStat([NotNull] MipsAsmParser.StatContext context)
        {
            // boxing instruction
            Statement obj = new Statement();
            var instrNode = context.instr();
            if (instrNode == null)  // if it is a white space or a broken text
                return null;  // skip it
            Instruction instr = (Instruction)Visit(context.instr());
            obj.Instruction = instr;
            // boxing Label
            var labelNode = context.label();
            if (labelNode != null)
            {
                obj.Label = (Label)Visit(context.label());
            }
            return obj;
        }

        public override object VisitTerminal(ITerminalNode node)
        {
            // don't Visit(context.TERMINAL())
            // context.TERMINAL().GetText() instead
            throw new System.NotImplementedException();
        }

        public override object VisitUsigned_imm([NotNull] MipsAsmParser.Usigned_immContext context)
        {
            string numStr = context.children[0].GetText();
            var item = context.children[0];
            uint num = 0;
            // work around - better get terminal type from context
            if (numStr.Length >= 3 && (numStr[1] == 'x' || numStr[1] == 'X'))
                num = Convert.ToUInt32(numStr.Substring(2), 16);
            else
                num = Convert.ToUInt32(numStr);
            return num;
        }

        public override object VisitOp_move([NotNull] MipsAsmParser.Op_moveContext context)
        {
            Instruction obj = new Instruction();
            obj.Type = FormatType.Immediate;
            obj.Opcode = Opcode.addi;
            obj.Rs = (RegisterType)Visit(context.rs);
            obj.Rt = (RegisterType)Visit(context.rt);
            obj.Immediate = 0;
            return obj;
        }

        public override object VisitOp_bgt([NotNull] MipsAsmParser.Op_bgtContext context)
        {
            // bgt rs, rt, label
            // => slt at, rt, rs
            Instruction slt = new Instruction();
            slt.Type = FormatType.Register;
            slt.Opcode = Opcode.RType;
            slt.Rs = (RegisterType)Visit(context.rt);
            slt.Rt = (RegisterType)Visit(context.rs);
            slt.Rd = RegisterType.at;
            slt.Funct = Funct.slt;
            // + bne at, zero, label
            Instruction bne = new Instruction();
            bne.Type = FormatType.Immediate;
            bne.Opcode = Opcode.bne;
            bne.Rs = RegisterType.at;
            bne.Rt = RegisterType.zero;
            bne.TargetLabelIdentity = context.target.GetText();

            slt.NextInstruction = bne;
            return slt;
        }
    }
}
