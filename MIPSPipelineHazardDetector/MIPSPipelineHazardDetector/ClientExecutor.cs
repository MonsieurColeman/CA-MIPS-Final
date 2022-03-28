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
		
		public void RunApplication(List<InstructionCommand> commands)
		{
			//Determine the hazards within the list of instructions
			List <HazardObject> unifiedHazards = new HazardDepicter().HazardDetector(new Queue<InstructionCommand>(commands), false);
			List<HazardObject> HazardsGoodMem = new HazardDepicter().HazardDetector(new Queue<InstructionCommand>(commands), true);
			mainWindow.ManifestHazards(unifiedHazards);

			//Run and display the results of the no forwarding pipeline
			Queue<InstructionCommand> queue = new Queue<InstructionCommand>(commands);
			Pipeline NoForwardingPipeline = RunPipeline(false, queue);
			List<List<string>> diagramStrings_NF = GetDiagramFromPipeline(NoForwardingPipeline);
			mainWindow.ShowNonForwardingDiagram(diagramStrings_NF);
			mainWindow.DisplayNoForwardingPipeline(NoForwardingPipeline.__state);

			//Run and display the results of the forwarding pipeline
			queue = new Queue<InstructionCommand>(commands);
			Pipeline ForwardingPipeline = RunPipeline(true, queue);
			List<List<string>> diagramStrings_F = GetDiagramFromPipeline(ForwardingPipeline);
			mainWindow.ShowForwardingDiagram(diagramStrings_F);
			mainWindow.DisplayForwardingPipeline(ForwardingPipeline.__state);

			//export
			mainWindow.outputForwarding = diagramStrings_F;
			mainWindow.outputNonforwarding = diagramStrings_NF;
		}
		
		private Pipeline RunPipeline(bool forwarding, Queue<InstructionCommand> queue)
        {
			//Create Pipeline
			Pipeline p = new Pipeline(forwarding);

			//put the first intruction into the pipeline
			p.StartPipelineWithCommand(queue.Dequeue());

			//run the intructions in the queue through the pipeline
			int queueSize = queue.Count;
			for (int i = 0; i < queueSize; i++)
				p.AddCommandToPipline(queue.Dequeue());

			//End the pipeline
			p.EndPipeline();

			//returns the pipeline
			return p;
		}

		private List<List<string>> GetDiagramFromPipeline(Pipeline p)
        {
			List<List<string>> strings = new List<List<string>>();
			List<Pipeline.PipelineObj> s = p.__output;
			for (int i = 0; i < s.Count; i++)
			{
				strings.Add(s[i].str);
			}
			return strings;
		}
	}
}


