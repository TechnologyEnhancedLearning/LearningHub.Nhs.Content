# Introduction 
This is the official open source repository for the Learning  Hub Content Server.

The LH Content Server provides access from the ESR system into the content storage of the Learning Hub in order to facilitate remote launching of content. It also provides historic URL launching where an entire URL has been redirected into the LH for rewriting subsequent to a resource migration.


# Getting Started
## Required installs
- [Visual Studio Professional 2022](https://visualstudio.microsoft.com/downloads/) or other suitable An IDE that supports the Microsoft Tech Stack
  - Make sure you have the [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) extension
- SQL Server 2019
- [SQL Server Management Studio 18](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
- [Git](https://git-scm.com/download)
- [Node](https://nodejs.org/en/download/) install specific/required version using NVM - see below.
- [SASS](https://www.sass-lang.com/install) for the command line
    - Specifically, follow the "Install Anywhere (Standalone)" guide. Simply download and extract the files somewhere, and point PATH at the dart-sass folder. This should allow you to use the "sass" command.
    - You don't want to install it via Yarn, as those are JavaScript versions that perform significantly worse.
- [.Net 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Azure storage emulator](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Azure storage explorer](https://azure.microsoft.com/en-gb/features/storage-explorer/#overview)
- [Node version manager (nvm)](https://github.com/coreybutler/nvm-windows/releases) - use this to install and use Node version 16.13.0 and NPM version 8.1.0 to work with this repository.

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)
