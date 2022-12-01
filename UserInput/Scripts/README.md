# Scripts for user inputs and image generation


#### Problem may encounter

1. The script "QuestionManager.cs" uses Newtonsoft.Json. It may cause a code stripping issue for builds. Solution: Use the Managed Stripping Level option to disable code stripping. However, when the IL2CPP scripting backend is selected, the Disabled option of Managed code stripping is not available. Have to select MONO as backend.
But MONO only support ARMv7, no ARM64.

2. It is very important to override the default AndroidManifest using the new AndroidManifest.xml for Android build.

3. If deployed on iOS, have to use IL2CPP as scripting backend. The QuestionManager.cs has to use a AOT version of Newtonsoft, so switch to NET2.1 as backend. And change the script to not use the <dynamic> type.
