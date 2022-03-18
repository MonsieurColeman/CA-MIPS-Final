using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
	public enum HazardType { none, data, control, structural } //structural resources

	public class HazardDepicter
    {


		public List<HazardObject> HazardDetector(Queue<InstructionCommand> _commands_, bool unifiedMemory)
		{
			List<HazardObject> checkedCommands = new List<HazardObject>();
			Queue<HazardObject> objOfHazard = new Queue<HazardObject>(InstructionCommandToHazardObj(_commands_.ToList()));

			//Setup for the switch statement
			HazardObject obj = new HazardObject();
			HazardObject olderObj = new HazardObject();
			int iterations;
			int numOfObjects = objOfHazard.Count;

			for (int j = 0; j < numOfObjects; j++)
            {
				//Iterate over previous commands to detect hazards
				switch (checkedCommands.Count)
				{
					case 0:
						//not much to do
						checkedCommands.Add(objOfHazard.Dequeue());
						break;
					case 1:
						obj = objOfHazard.Dequeue();
						HazardObject oldOBJ = checkedCommands[checkedCommands.Count - 1];
						GetHazard(ref obj, ref oldOBJ, unifiedMemory, false);
						checkedCommands.Add(obj);
						break;
					case 2:
						obj = objOfHazard.Dequeue();
						olderObj = new HazardObject();
						iterations = checkedCommands.Count;
						for (int i = 0; i < iterations; i++)
						{
							olderObj = checkedCommands[checkedCommands.Count - 1 - i];
							GetHazard(ref obj, ref olderObj, unifiedMemory, false);
						}
						checkedCommands.Add(obj);
						break;
					default:
						obj = objOfHazard.Dequeue();
						olderObj = new HazardObject();
						iterations = 2;
						for (int i = 0; i < iterations; i++)
						{
							olderObj = checkedCommands[checkedCommands.Count - 1 - i];
							GetHazard(ref obj, ref olderObj, unifiedMemory, true);
						}
						checkedCommands.Add(obj);
						break;
				}
			}

			return checkedCommands;
		}

		public bool HazardChecker(InstructionCommand newCommand, InstructionCommand command)
        {
			if (command.rs_ == newCommand.rt_ || command.rs_ == newCommand.rd_)
				return true;
			return false;
		}

		public void GetHazard(ref HazardObject newCommand, ref HazardObject command
			, bool unifiedMemory, bool moreThan3Instructions)
		{
			if(command.__rs == newCommand.__rd)
            {
				newCommand.rd__Hazard = HazardType.data;
				command.rs__Hazard = HazardType.data;
				command.hazards = true;
				newCommand.hazards = true;
            }
			if (command.__rs == newCommand.__rt && newCommand._inst.ToString() != "sw")
			{
				newCommand.rt__Hazard = HazardType.data;
				command.rs__Hazard = HazardType.data;
				command.hazards = true;
				newCommand.hazards = true;
			}
			
			if (command.__rs == newCommand.__rs && newCommand._inst.ToString() == "sw")
			{
				newCommand.rs__Hazard = HazardType.data;
				command.rs__Hazard = HazardType.data;
				command.hazards = true;
				newCommand.hazards = true;
			}
			
			if (unifiedMemory && moreThan3Instructions)
            {
				newCommand.inst__hazard = HazardType.structural;
				command.inst__hazard = HazardType.structural;
				command.hazards = true;
				newCommand.hazards = true;
			}
		}

		public List<HazardObject> InstructionCommandToHazardObj(List<InstructionCommand> commands)
        {
			List<HazardObject> returnList = new List<HazardObject>();
			int iterations = commands.Count;
			for (int i = 0; i < iterations; i++)
            {
				HazardObject hazardObject = new HazardObject();
				hazardObject._inst = commands[i].inst_;
				hazardObject.instructionType = commands[i].type_;
				hazardObject.__rs = commands[i].rs_;
				hazardObject.__rt = commands[i].rt_;
				hazardObject.__rd = commands[i].rd_;
				hazardObject.hazards = false;
				hazardObject.rs__Hazard = HazardType.none;
				hazardObject.rt__Hazard = HazardType.none;
				hazardObject.rd__Hazard = HazardType.none;
				hazardObject.inst__hazard = HazardType.none;
				hazardObject.__immediate = commands[i].immediate_;
				hazardObject.__wordAddress = commands[i].wordAddress_;
				hazardObject.__rTypeImmediateFlag = commands[i].rTypeImmediateFlag_;
				returnList.Add(hazardObject);
			}
			return returnList;
        }
	}

	public class HazardObject
    {
		public bool hazards;
		public Instruction _inst;
		public InstructionType instructionType;
		public HazardType inst__hazard;
		public HazardType rs__Hazard;
		public HazardType rt__Hazard;
		public HazardType rd__Hazard;
		public Register __rs;
		public Register __rt;
		public Register __rd;
		public int __immediate;
		public int __wordAddress;
		public bool __rTypeImmediateFlag;
	}
}
