using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
	public class ClientExec
	{
		MainWindow mainWindow;

		public ClientExec(MainWindow main)
		{
			mainWindow = main;
		}

		public string RunApplication(List<InstructionCommand> commands)
        {
			Pipeline p = new Pipeline(false);
			Queue<InstructionCommand> queue = new Queue<InstructionCommand>();
			List<List<string>> strings = new List<List<string>>();

			//put commands into an execution queue
			foreach (InstructionCommand command in commands)
				queue.Enqueue(command);

			//put the first intruction into the pipeline
			p.StartPipelineWithCommand(queue.Dequeue());

			//run the intructions in the queue through the pipeline
			int maxIter = queue.Count;
			for (int i = 0; i < maxIter; i++)
				p.AddCommandToPipline(queue.Dequeue());
			p.EndPipeline();

			List<Pipeline.PipelineObj> s = Pipeline.__output;
			for (int i = 0; i < s.Count; i++)
            {
				strings.Add(s[i].str);
            }

			mainWindow.ShowDiagramForFirstFourInstructions(strings);

			//Output the state of the pipeline to the user
			return p.__state;
        }
	}
}

