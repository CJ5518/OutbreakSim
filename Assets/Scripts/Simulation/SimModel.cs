using System;

//Class representing a disease model
//Holds information relating to disease states, algorithms, etc.

namespace CJSim {

	//Model type determines how the model is processed, i.e deterministically or stochastically
	public enum ModelType {
		Deterministic,
		DeterministicWithGillespie,
		TauLeaping,
		Gillespie,
		GillespieSpatialSingleThreaded //Could technically use the threads to calculate min times in their blocks, then take min of each thread block. But that would take effort for basically no value
	}

	public class SimModel {
		public float[] parameters;
		public float parameterNoiseModifier; //cjnote not sure how I feel about this being here

		//stoichiometry[0] = (1,2): the 1st reaction goes from 2nd compartment to 3rd compartment
		public Tuple<int,int>[] stoichiometry;

		//Contains arguments to the SimAlgorithms class, defines a set of reactions
		//reactionFunctionDetails[0] = [0,1,2,3]  -  the first reactions, pass these args to SimAlgos
		public int[][] reactionFunctionDetails;
		
		public IMovementModel movementModel;

		#region Properties

		//The tpye of this model
		public ModelType modelType {private set; get;}
		
		//The number of different reactions in the model
		public int reactionCount {private set; get;}

		//The number of compartments in this model
		public int compartmentCount {private set; get;}

		//The number of parameters in this model
		public int parameterCount {private set; get;}

		#endregion
		
		//Interface is WIP, need to be able to load from files in the future

		//The 'count' constructor, used for when you have some code to populate the various necessary fields
		//Makes the basic data structures have the correct size, just unpopulated
		public SimModel(int compartmentCount, int reactionCount, int parameterCount, IMovementModel movement, ModelType modelType) {
			this.reactionCount = reactionCount;
			this.compartmentCount = compartmentCount;
			this.parameterCount = parameterCount;
			this.modelType = modelType;
			this.movementModel = movement;
			this.parameterNoiseModifier = 0.0f;

			stoichiometry = new Tuple<int, int>[reactionCount];
			reactionFunctionDetails = new int[reactionCount][];
			parameters = new float[parameterCount];
		}
		
		//Gets the highest order of reaction for this compartment
		//cjnote should be easy to chache this somewhere, it also only used for tau leaping tho so not much need to bother
		public int getHOR(int compartment) {
			int HOR = 0;
			//Search reactions for things affecting this compartment
			for (int q = 0; q < reactionCount; q++) {
				if (stoichiometry[q].Item1 == compartment || stoichiometry[q].Item2 == compartment) {
					HOR = Math.Max(HOR, SimAlgorithms.getOrderOfReaction(reactionFunctionDetails[q][0]));
				}
			}
			return HOR;
		}


		//Loads a model from a file (in a constructor)
		
		public SimModel(string filename) {
			throw new System.NotImplementedException();
		}

		//Writes a model to a file
		public void writeToFile(string filename) {
			throw new System.NotImplementedException();
		}

		public void loadFromFile(string filename) {
			throw new System.NotImplementedException();
		}

		//Validate the model (makes sure things are initialized and not conflicting)
		//Returns true if everything is alright, false otherwise
		//Doesn't guarantee that a model will actually work properly, but at the very least it should make sure the model won't crash the simulation
		public bool validate() {
			//First verify that the user provided arrays are in fact provided
			//Reaction function details must be provided, how else would we know what parameters go where
			for (int q = 0; q < reactionCount; q++) {
				if (reactionFunctionDetails[q] == null) {
					return false;
				}
				//Also check for stoichiometry, how else would we know which states go to which other states
				if (stoichiometry[q] == null) {
					return false;
				}
			}

			//Tau leaping can't have spatial anything
			if (modelType == ModelType.TauLeaping) {
				for (int q = 0; q < reactionFunctionDetails.Length; q++) {
					//Make sure tau leaping doesn't try to do spatial things,
					//would likely cause errors due to some poor programming
					if (reactionFunctionDetails[q][0] == 2) {
						return false;
					}
				}
			}

			return true;
		}
	}
}
