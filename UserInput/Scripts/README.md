# Scripts for user inputs and image generation


#### Problem may encounter
1. The script "QuestionManager.cs" uses Newtonsoft.Json. It may cause a code stripping issue. Solution: Use the new Managed Stripping Level option to diable code stripping. However, when the IL2CPP scripting backend is selected, the Disabled option of Managed code stripping is not available. Have to select MONO as backend.
But MONO only support ARMv7, no ARM64.
