using Godot;
using System;

public partial class Main : Node3D
{
    [Export] public PackedScene AtomScene;
    [Export] public PackedScene MoleculeScene;

    public override void _Ready()
    {
        GD.Print("Simulateur d'atome lancé");

        CreateCrystalOfMethamphetamine();


        var water = CreateWater();
        water.Position = new Vector3(0, 0, 0);
        AddChild(water);
        water.PrintProperties();

        var co2 = CreateCO2();
        co2.Position = new Vector3(-30, 0, 0);
        AddChild(co2);
        co2.PrintProperties();
    }

    private void CreateCrystalOfMethamphetamine()
    {
        int sizeX = 3;
        int sizeY = 4;
        int sizeZ = 3;
        float spacing = 20.0f;


        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    var meth = CreateMethamphetamine();
                    meth.Position = new Vector3(
                        x * spacing,
                        y * spacing,
                        z * spacing
                    );

                    // Optionnel : légère rotation pour un effet plus réaliste
                    meth.Rotation = new Vector3(
                        GD.Randf() * Mathf.Tau,
                        GD.Randf() * Mathf.Tau,
                        GD.Randf() * Mathf.Tau
                    );

                    AddChild(meth);
                }
            }
        }

        GD.Print($"Cristal géant de méthamphétamine généré ({sizeX * sizeY * sizeZ} molécules) !");
    }

    private Molecule CreateWater()
    {
        var molecule = MoleculeScene.Instantiate<Molecule>();

        var oxygen = CreateAtom(8, 8, 8);
        molecule.AddAtom(oxygen, Vector3.Zero);

        var hydrogen1 = CreateAtom(1, 1, 1);
        molecule.AddAtom(hydrogen1, new Vector3(2, 0, 0));

        var hydrogen2 = CreateAtom(1, 1, 1);
        molecule.AddAtom(hydrogen2, new Vector3(-2, 0, 0));

        return molecule;
    }

    private Molecule CreateMethamphetamine()
    {
        var molecule = MoleculeScene.Instantiate<Molecule>();

        // 10 Carbons
        for (int i = 0; i < 10; i++)
        {
            var carbon = CreateAtom(6, 6, 6);
            molecule.AddAtom(carbon, new Vector3(i * 1.5f, 0, 0));
        }

        // 15 Hydrogens
        for (int i = 0; i < 15; i++)
        {
            var hydrogen = CreateAtom(1, 1, 1);
            molecule.AddAtom(hydrogen, new Vector3(i * 1.0f, 2, 0));
        }

        var nitrogen = CreateAtom(7, 7, 7);
        molecule.AddAtom(nitrogen, new Vector3(5, 0, 2));

        return molecule;
    }

    private Molecule CreateCO2()
    {
        var molecule = MoleculeScene.Instantiate<Molecule>();

        var carbon = CreateAtom(6, 6, 6);
        molecule.AddAtom(carbon, Vector3.Zero);

        var oxygen1 = CreateAtom(8, 8, 8);
        molecule.AddAtom(oxygen1, new Vector3(2.5f, 0, 0));

        var oxygen2 = CreateAtom(8, 8, 8);
        molecule.AddAtom(oxygen2, new Vector3(-2.5f, 0, 0));

        return molecule;
    }

    private Atom CreateAtom(int protons, int neutrons, int electrons)
    {
        var atom = AtomScene.Instantiate<Atom>();
        atom.NbProtons = protons;
        atom.NbNeutrons = neutrons;
        atom.NbElectrons = electrons;
        return atom;
    }
}
