# f5-irule-tool
iRule management tool for MS Windows


*Building*

`dotnet build -c Release`

*Running*

`dotnet irule-tool.dll`

# Building

Install VSCode on MS Platform
Install .NET Core SDK 2.1.x
Open Terminal

`dotnet restore`
`dotnet publish --self-contained -c Release`

# iRule tool how to use

`irule-tool.exe -h`

By default it will show you list of iRules on your F5

If used with -q will show you other iCR objects, like:

`irule-tool.exe -q net/self`

+ irule rest client, (C) 2017 Krystian Baniak
+ using target: 10.128.1.45, query: /mgmt/tm/net/self
response: tm:net:self:selfcollectionstate -->
  item: tm:net:self:selfstate self_server_1 gen: 145
  item: tm:net:self:selfstate app_float_1 gen: 1
  item: tm:net:self:selfstate ha_1 gen: 1
  item: tm:net:self:selfstate self_app_main_1 gen: 1

The best application is to follow batch processing mode by specifying batch file as the parameter:

`irule-tool.exe -b batch-file.json`

Example batch file is in directory.

Supported batch commands:
SAVE
SYNC
LOAD_RULES

Self explaining isn't it :-)

# Generating Databooks

THis will generate comprehensive Excel file with configuration dump.

`irule-tool.exe -t host -a file.xlsx`
