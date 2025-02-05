using UnityEditor;
using UnityEditor.Build.Profile;


public class BuildScript
{
    public static void BuildMazeGeneratorWithWebProfile()
    {
       var bp=AssetDatabase.LoadAssetAtPath<BuildProfile>("Assets/Settings/Build Profiles/MazeGenerator-Win.asset");
       BuildProfile.SetActiveBuildProfile(bp);
       BuildPlayerWithProfileOptions options=new ();
       options.buildProfile=bp;
       options.locationPathName="Builds/MazeGenerator/Win/Out.exe";
       options.options=BuildOptions.None;
       // Lancer le build
        BuildPipeline.BuildPlayer(options);
    }
}
