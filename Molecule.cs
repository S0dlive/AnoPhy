using Godot;
using System;

using Godot;
using System;
using System.Collections.Generic;

public partial class Molecule : Node3D
{
	private List<Atom> atoms = new List<Atom>();

	public void AddAtom(Atom atom, Vector3 position)
	{
		atoms.Add(atom);
		AddChild(atom);
		atom.Position = position;
	}

	public double GetMolecularMass()
	{
		double total = 0;
		foreach (var atom in atoms)
		{
			total += atom.GetAtomicMass();
		}
		return total;
	}

	public string GetFormula()
	{
		Dictionary<string, int> atomCounts = new();

		foreach (var atom in atoms)
		{
			string symbol = GetElementSymbol(atom.NbProtons);
			if (symbol == null) continue;

			if (!atomCounts.ContainsKey(symbol))
				atomCounts[symbol] = 0;
			atomCounts[symbol]++;
		}

		string formula = "";
		foreach (var pair in atomCounts)
		{
			formula += pair.Key;
			if (pair.Value > 1)
				formula += pair.Value.ToString();
		}

		return formula;
	}

	private string GetElementSymbol(int protons)
	{
		return protons switch
		{
			1 => "H",
			2 => "He",
			6 => "C",
			7 => "N",
			8 => "O",
			17 => "Cl",
			_ => null,
		};
	}

	public void PrintProperties()
	{
		GD.Print($"----- Molécule -----");
		GD.Print($"Formule : {GetFormula()}");
		GD.Print($"Masse molaire ≈ {GetMolecularMass():F4} u");
		GD.Print("--------------------");
	}
}
