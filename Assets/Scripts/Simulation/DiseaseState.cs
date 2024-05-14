
//Holds the disease state of a single cell, in other words,
//represents a single bucket in a compartment model

namespace CJSim {
	public struct DiseaseState {
		public readonly int[] state;
		
		//The main constrcutor, requires the state (compartment) count
		public DiseaseState(int stateCount) {
			state = new int[stateCount];

			setToZero();
		}

		//Allow for indexing this struct directly
		public int this[int index] {
			get {return state[index];}
			set {state[index] = value;}
		}

		//Copy constructor, this struct contains a reference type that needs to be explicitly copied
		public DiseaseState(DiseaseState other) {
			state = new int[other.stateCount];
			for (int q = 0; q< other.stateCount; q++) {
				state[q] = other.state[q];
			}
		}

		//Constructor for the lazy man
		public DiseaseState(SimModel model) : this(model.compartmentCount) {}

		//Shorthand for getting the number of states (compartments)
		public int stateCount {
			get {
				return state.Length;
			}
		}

		//Calculates the sum of all the compartments
		public int numberOfPeople {
			get {
				int ret = 0;
				for (int q = 0; q < stateCount; q++) {
					ret += state[q];
				}
				return ret;
			}
		}

		//Sets every value in the state array to zero
		public void setToZero() {
			for (int q = 0; q < stateCount; q++) {
				state[q] = 0;
			}
		}

		public void setTo(DiseaseState other) {
			if (other.stateCount == stateCount) {
				for (int q = 0; q < stateCount; q++) {
					state[q] = other.state[q];
				}
			}
		}
	}
}
