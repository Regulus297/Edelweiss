using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Consts
{
    internal class ObjectDepths : LoennModule
    {
        public override string ModuleName => "consts.object_depths";

        public override Table GenerateTable(Script script)
        {
            Table depths = new(script);

            depths["bgTerrain"] = 10000;
            depths["bgMirrors"] = 9500;
            depths["bgDecals"] = 9000;
            depths["bgParticles"] = 8000;
            depths["SolidsBelow"] = 5000;
            depths["below"] = 2000;
            depths["npcs"] = 1000;
            depths["theoCrystal"] = 100;
            depths["player"] = 0;
            depths["dust"] = -50;
            depths["pickups"] = -100;
            depths["seeker"] = -200;
            depths["particles"] = -8000;
            depths["above"] = -8500;
            depths["solids"] = -9000;
            depths["fgTerrain"] = -10000;
            depths["fgDecals"] = -10500;
            depths["dreamBlocks"] = -11000;
            depths["crystalSpinners"] = -11500;
            depths["playerDreamDashing"] = -12000;
            depths["enemy"] = -12500;
            depths["fakeWalls"] = -13000;
            depths["fgParticles"] = -50000;
            depths["top"] = -1000000;
            depths["formationSequences"] = -2000000;
            depths["triggers"] = -10000000;

            return depths;
        }
    }
}