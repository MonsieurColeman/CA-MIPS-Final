using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
	public class ClientExec
	{


		public ClientExec()
		{
		}

		public string RunApplication(List<InstructionCommand> commands)
        {
			string dependencies = DetermineDependencies(commands);
			string timingNF = DeterimineTiming(commands);
			string timingF = DeterimineTimingWithForwarding(commands);

			//TODO later : dummy output
			return commands.ToString();
        }

		private string DetermineDependencies(List<InstructionCommand> commands)
		{
			
			return "";
        }

		private string DeterimineTiming(List<InstructionCommand> commands)
		{

			return "";
		}

		private string DeterimineTimingWithForwarding(List<InstructionCommand> commands)
		{

			return "";
		}

		private string Foo()
        {
			
			return "";
        }
	}
}

