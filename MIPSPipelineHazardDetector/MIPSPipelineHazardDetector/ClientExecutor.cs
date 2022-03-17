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
			Pipeline p = new Pipeline(false);
			Queue<InstructionCommand> queue = new Queue<InstructionCommand>();

			//put commands into an execution queue
			foreach (InstructionCommand command in commands)
				queue.Enqueue(command);

			//put the first intruction into the pipeline
			p.StartPipelineWithCommand(queue.Dequeue());

			//run the intructions in the queue through the pipeline
			int maxIter = queue.Count;
			for (int i = 0; i < maxIter; i++)
				p.AddCommandToPipline(queue.Dequeue());

			//Output the state of the pipeline to the user
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

