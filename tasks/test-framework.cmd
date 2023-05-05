set CoreLibPath=..\..\..\.application\net-framework\core-bin
set RelativePkgFolderPath=..\..\..\.application\net-framework\packages
set CoreTargetFramework=net472
dotnet test ..\.solution\CreatioPackages.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=".\..\TestResults\coverage.opencover.xml"

