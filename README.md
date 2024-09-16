# Sir-Morphalot
Proof of Concept for demonstrating polymorph and metamorph malware capabilities in C#

Because polymorphism and metamorphism are a very exciting and interesting topic in the context of malware, I would like to introduce my PoC Sir Morphalot, which serves as a demonstration of advanced malware techniques. The goal of the program is to bypass signature-based and behavior-based detection methods using polymorphic and partially metamorphic techniques, as it also continuously change its behavior.

## How it works:

### Encrypted Overlay: 
The finished compiled executable file contains an overlay that includes the current C# source code of the binary in encrypted form. This overlay is decrypted every time the program is executed.

### Self-modification of the code:
The decrypted source code is analyzed and dynamically modified. This includes:

- Label Replacement: 
Unique comments in the code are randomly replaced with predefined "trash" functions that serve no real functionality but vary the code.
- Function Replacement: 
Specific legitimate functions are randomly replaced with alternative implementations that achieve the same result in different ways. Each of these functions has multiple variants, creating a multitude of possible combinations and thus different variants of the program.
- Update of the encryption key:
The encryption key embedded in the code is updated with a randomly generated key. This key update is a crucial part of the code manipulation and ensures that the next version of the program can correctly decrypt its overlay.

### Recompilation:
After modification and key update, the next version of the program is created by recompiling the modified source code with the Windows-provided C# compiler csc.exe (the main reason why I chose C#).

### Encryption and overlay attachment:
The modified source code is then encrypted with the new key and attached as an overlay to the new executable file, so that the new version also has a correctly encrypted overlay to run properly later.

### Self-deletion:
After successful creation and modification of the new version of the program, the old version deletes itself using cmd.exe.

## Summarized
So Sir Morphalot combines both polymorphic and metamorphic properties. The continuous recompilation and encryption of the source code with an updated key ensure that each version of the program is unique. And the dynamic changes to the source code for each version affect its behavior in the next execution. This results in a wide variety of different program variants.

By combining this techniques, the program achieves a high level of resistance to both static and dynamic AV analysis, as both the binary structure and the behavior of the program vary with each execution.

## How to built:
- Download the project and open it with Visual Studio (VS).
- Build the executable (Ctrl + B) in debug mode.
- Run the runAppender.bat in the Debug folder, where your built EXE is located, to encrypt the C# code of the project and attach it to the built program.

Now the program is ready to go and you can monitor the code modifications e.g. in dnSpy.
