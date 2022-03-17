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
			Pipeline p = new Pipeline();
			Queue<InstructionCommand> queue = new Queue<InstructionCommand>();
			foreach (InstructionCommand command in commands)
				queue.Enqueue(command);
			p.StartPipelineWithCommand(queue.Dequeue());
			int maxIter = queue.Count;
			for (int i = 0; i < maxIter; i++)
				p.AddCommandToPipline(queue.Dequeue());
			//string dependencies = DetermineDependencies(commands);
			//string timingNF = DeterimineTiming(commands);
			//string timingF = DeterimineTimingWithForwarding(commands);
			
			//TODO later : dummy output
			return p.__state;
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

