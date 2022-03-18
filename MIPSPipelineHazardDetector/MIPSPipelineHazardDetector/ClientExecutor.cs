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

/*
 todo: fix the ui
	   add to ui
		validate pipeline stuff
		functionality: instruction checking for instructions a few ways back
 */




/*
public void RunApplication(List<InstructionCommand> commands)
{
	Pipeline p = new Pipeline(false);

	Queue<InstructionCommand> queue = new Queue<InstructionCommand>();
	List<List<string>> strings = new List<List<string>>();

	//put commands into an execution queue
	foreach (InstructionCommand command in commands)
		queue.Enqueue(command);

	//put the first intruction into the pipeline
	p.StartPipelineWithCommand(queue.Dequeue());
	//pp.StartPipelineWithCommand(queue.Dequeue());

	//run the intructions in the queue through the pipeline
	int maxIter = queue.Count;
	for (int i = 0; i < maxIter; i++)
		p.AddCommandToPipline(queue.Dequeue());
	p.EndPipeline();

	//List<Pipeline.PipelineObj> s = Pipeline.__output;
	List<Pipeline.PipelineObj> s = p.__output;
	for (int i = 0; i < s.Count; i++)
	{
		strings.Add(s[i].str);
	}

	mainWindow.ShowNonForwardingDiagram(strings);

	//Output the state of the pipeline to the user
	//mainWindow.DisplayForwardingPipeline();
	mainWindow.DisplayNoForwardingPipeline(p.__state);

	//return p.__state;
	//
	//
	//
	//

	p = new Pipeline(true);
	//Pipeline pp = new Pipeline(true);
	queue = new Queue<InstructionCommand>();
	strings = new List<List<string>>();

	//put commands into an execution queue
	foreach (InstructionCommand command in commands)
		queue.Enqueue(command);

	//put the first intruction into the pipeline
	p.StartPipelineWithCommand(queue.Dequeue());
	//pp.StartPipelineWithCommand(queue.Dequeue());

	//run the intructions in the queue through the pipeline
	maxIter = queue.Count;
	for (int i = 0; i < maxIter; i++)
		p.AddCommandToPipline(queue.Dequeue());
	p.EndPipeline();

	s = Pipeline.__output;
	for (int i = 0; i < s.Count; i++)
	{
		strings.Add(s[i].str);
	}

	mainWindow.ShowForwardingDiagram(strings);

	//Output the state of the pipeline to the user
	//mainWindow.DisplayForwardingPipeline();
	mainWindow.DisplayForwardingPipeline(p.__state);
}
*/