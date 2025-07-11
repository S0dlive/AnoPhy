using Godot;
using System;

public partial class Main : Node3D
{
	
	[Export] public PackedScene AtomScene;
	public override void _Ready()
	{
		GD.Print("Simulateur d'atome lancé ");

		Random rnd = new Random();

		for (int i = 0; i < 3; i++)
		{
			var atom = AtomScene.Instantiate<Node3D>();

			
			int nbProtons = rnd.Next(1, 11);
			int nbNeutrons = nbProtons;          
			int nbElectrons = nbProtons;          
			// Chercher le script Atom attaché
			if (atom is Atom atomScript)
			{
				atomScript.NbProtons = nbProtons;
				atomScript.NbNeutrons = nbNeutrons;
				atomScript.NbElectrons = nbElectrons;
			}

			// Disposer les atomes dans l'espace pour qu'ils ne se superposent pas
			atom.Position = new Vector3(i * 10, 0, 0);

			AddChild(atom);

			GD.Print($"Atom #{i+1} → Protons: {nbProtons}");
		}
	}
	
	
}
