using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.XCodeEditor;
using UnityEngine;

public class IOSetting
{
   [PostProcessBuildAttribute(100)]
   public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {
       if (target != BuildTarget.iOS) {
           Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
           return;
       }
       // Create a new project object from build target
       PBXProject project = new PBXProject();
       string configFilePath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
       project.ReadFromFile(configFilePath);
       string targetGuid = project.TargetGuidByName("Unity-iPhone");
       string debug = project.BuildConfigByName(targetGuid, "Debug");
       string release = project.BuildConfigByName(targetGuid, "Release");
       project.AddBuildPropertyForConfig(debug, "CODE_SIGN_RESOURCE_RULES_PATH", "$(SDKROOT)/ResourceRules.plist");
       project.AddBuildPropertyForConfig(release, "CODE_SIGN_RESOURCE_RULES_PATH", "$(SDKROOT)/ResourceRules.plist");

       project.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", true);
       project.AddFrameworkToProject(targetGuid, "Security.framework", true);
       project.AddFrameworkToProject(targetGuid, "libz.tbd", true);
       project.AddFrameworkToProject(targetGuid, "libc++.tbd", true);

       project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

       project.WriteToFile(configFilePath);

       EditSuitIpXCode(pathToBuiltProject);
   }

   public static void EditSuitIpXCode(string path) {
       //插入代码
       //读取UnityAppController.mm文件
       string src = @"_window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];";
       string dst = @"//    _window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];

   CGRect winSize = [UIScreen mainScreen].bounds;
   if (winSize.size.height / winSize.size.width > 2) {
       winSize.size.height -= 150;
       winSize.origin.y = 75;
       ::printf(""-> is iphonex aaa hello world\n"");
   } else {
       ::printf(""-> is not iphonex aaa hello world\n"");
   }
   _window = [[UIWindow alloc] initWithFrame: winSize];

   ";

       string unityAppControllerPath = path + "/Classes/UnityAppController.mm";
       XClassExt UnityAppController = new XClassExt(unityAppControllerPath);
       UnityAppController.Replace(src, dst);
   }
    

}
