using UnityEditor;
using UnityEditor.Build.Profile;

public class BuildScript
{
    public static void BuildMazeGeneratorWithWebProfile()
    {
       var bp=AssetDatabase.LoadAssetAtPath<BuildProfile>("Assets/Settings/Build Profiles/MazeGenerator-Web.asset");
       BuildProfile.SetActiveBuildProfile(bp);
       BuildPlayerWithProfileOptions options=new ();
       options.buildProfile=bp;
       options.locationPathName="Builds/MazeGenerator/Web/Out.exe";
       options.options=BuildOptions.None;
       // Lancer le build
        BuildPipeline.BuildPlayer(options);
    }
}
