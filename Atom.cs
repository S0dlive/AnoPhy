using Godot;
using System;
using System.Collections.Generic;

public partial class Atom : Node3D
{
	[Export] public PackedScene ProtonScene;
	[Export] public PackedScene NeutronScene;
	[Export] public PackedScene ElectronScene;

	[Export] public int NbProtons = 2;
	[Export] public int NbNeutrons = 2;
	[Export] public int NbElectrons = 2;
	
	private List<Node3D> electrons = new();

	public override void _Ready()
	{
		GenerateNucleus();
		GenerateElectrons();
	}
	
	public double GetAtomicMass()
	{
		double protonMass = 1.0073;
		double neutronMass = 1.0087;
		double electronMass = 0.00055;

		return NbProtons * protonMass
			   + NbNeutrons * neutronMass
			   + NbElectrons * electronMass;
	}
	
	public int GetValence()
	{
		int z = NbProtons;

		if (z == 1) return 1; 
		if (z == 2) return 0; 

		int electrons = NbElectrons;
		int valenceElectrons;

		if (electrons <= 2)
			valenceElectrons = electrons;
		else
			valenceElectrons = electrons % 8;

		int needed = (valenceElectrons < 4) 
			? (4 - valenceElectrons) 
			: (8 - valenceElectrons);

		return needed;
	}
	
	public bool CanBondWith(Atom other)
	{
		return this.GetValence() > 0 
		       && other.GetValence() > 0
		       && (this.GetValence() + other.GetValence()) <= 8;
	}
	
	public void TryBondWith(Atom other)
	{
		if (CanBondWith(other))
		{
			GD.Print($"Nouvelle liaison chimique entre {NbProtons} et {other.NbProtons}"); 
			var molecule = new Molecule();
			molecule.AddAtom(this, Vector3.Zero);
			molecule.AddAtom(other, new Vector3(2, 0, 0));

			GetParent().AddChild(molecule);

			QueueFree();
			other.QueueFree();
		}
		else
		{
			GD.Print("Pas de liaison possible.");
		}
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("mouss_press"))
		{
			PrintProperties();
		}
	}
	
	public int GetCharge()
	{
		return NbProtons - NbElectrons;
	}
	
	public void PrintProperties()
	{
		int A = GetMassNumber();
		double mass = GetAtomicMass();
		int charge = GetCharge();

		GD.Print($"------ Atome sélectionné ------");
		GD.Print($"Protons (Z) : {NbProtons}");
		GD.Print($"Neutrons (N) : {NbNeutrons}");
		GD.Print($"Nombre de masse (A) : {A}");
		GD.Print($"Electrons : {NbElectrons}");
		GD.Print($"Charge : {charge} (e)");
		GD.Print($"Masse atomique ≈ {mass:F4} u");
		GD.Print($"Stable ? {(IsStable() ? "Oui" : "Non")}");
		GD.Print($"--------------------------------");
	}
	
	
	public int GetMassNumber()
	{
		return NbProtons + NbNeutrons;
	}

	private void GenerateNucleus()
	{
		Random rnd = new Random();

		for (int i = 0; i < NbProtons; i++)
		{
			var proton = ProtonScene.Instantiate<Node3D>();
			proton.Position = RandomInsideSphere(0.5f, rnd);
			AddChild(proton);
		}

		for (int i = 0; i < NbNeutrons; i++)
		{
			var neutron = NeutronScene.Instantiate<Node3D>();
			neutron.Position = RandomInsideSphere(0.5f, rnd);
			AddChild(neutron);
		}
	}

	public bool IsStable()
	{
		int Z = NbProtons;
		int N = NbNeutrons;

		if (Z == 0) return false;

		float ratio = (float)N / Z;

		if (Z <= 20)
		{
			return ratio >= 0.9f && ratio <= 1.15f;
		}
		else if (Z <= 40)
		{
			return ratio >= 1.1f && ratio <= 1.3f;
		}
		else if (Z <= 83)
		{
			return ratio >= 1.25f && ratio <= 1.6f;
		}
		else
		{
			return false;
		}
	}
	
	public void Decay()
	{
		if (IsStable())
		{
			GD.Print("Atome stable → pas de désintégration.");
			return;
		}

		Random rnd = new Random();
		int type = rnd.Next(0, 3);

		if (type == 0 && NbProtons >= 2 && NbNeutrons >= 2)
		{
			NbProtons -= 2;
			NbNeutrons -= 2;
			NbElectrons -= 2;
			GD.Print("Émission alpha (α) : perte de 2 protons et 2 neutrons !");
		}
		else if (type == 1 && NbNeutrons > 0)
		{
			NbNeutrons -= 1;
			NbProtons += 1;
			NbElectrons += 1;
			GD.Print("Émission beta- (β⁻) : neutron → proton.");
		}
		else if (type == 2 && NbProtons > 0)
		{
			NbProtons -= 1;
			NbNeutrons += 1;
			NbElectrons -= 1;
			GD.Print("Émission beta+ (β⁺) : proton → neutron.");
		}
		else
		{
			GD.Print("Aucune désintégration possible.");
		}
	}
	
	private void GenerateElectrons()
	{
		for (int i = 0; i < NbElectrons; i++)
		{
			var electron = ElectronScene.Instantiate<Node3D>();
			AddChild(electron);
			electrons.Add(electron);
			
			electron.SetMeta("angle", GD.RandRange(0, Mathf.Pi * 2));
			electron.SetMeta("radius", 3.0 + i * 1.0);
			electron.SetMeta("speed", 1.0 + i * 0.5);
		}
	}

	public override void _Process(double delta)
	{
		foreach (var electron in electrons)
		{
			float angle = (float)electron.GetMeta("angle");
			float radius = (float)electron.GetMeta("radius");
			float speed = (float)electron.GetMeta("speed");

			angle += speed * (float)delta;
			electron.SetMeta("angle", angle);

			float x = radius * Mathf.Cos(angle);
			float z = radius * Mathf.Sin(angle);
			electron.Position = new Vector3(x, 0, z);
		}
	}

	private Vector3 RandomInsideSphere(float radius, Random rnd)
	{
		float u = (float)rnd.NextDouble();
		float v = (float)rnd.NextDouble();
		float theta = u * 2.0f * Mathf.Pi;
		float phi = Mathf.Acos(2.0f * v - 1.0f);
		float r = radius * Mathf.Pow((float)rnd.NextDouble(), 1.0f / 3.0f);

		float sinPhi = Mathf.Sin(phi);
		float x = r * sinPhi * Mathf.Cos(theta);
		float y = r * sinPhi * Mathf.Sin(theta);
		float z = r * Mathf.Cos(phi);

		return new Vector3(x, y, z);
	}
}
