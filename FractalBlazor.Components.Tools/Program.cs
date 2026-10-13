using FractalBlazor.Components.Layout.Utilities;
using FractalBlazor.Components.Tools;

Console.WriteLine("Fractal Blazor Tools");

switch (args.FirstOrDefault())
{
    case "atoms":
        WriteAtoms();
        break;
    default:
        ImageToColorArray.ImportStandard();
        break;
}

// Regenerates the atoms section of the Layout stylesheet from FbAtoms.
static void WriteAtoms()
{
    var directory = new DirectoryInfo(Environment.CurrentDirectory);
    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
        directory = directory.Parent;
    if (directory is null)
        throw new DirectoryNotFoundException("Run the tool from inside the repository.");

    var path = Path.Combine(directory.FullName, FbAtomStylesheet.RelativePath);
    File.WriteAllText(path, FbAtomStylesheet.Update(File.ReadAllText(path)));
    Console.WriteLine($"{FbAtoms.Names.Count} atoms written to {path}");
}
