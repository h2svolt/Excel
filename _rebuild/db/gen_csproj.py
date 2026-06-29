import os, glob

bin_dir = r'F:\Excel-on-Work-Updated\Excel-main\bin'
proj_dir = r'F:\Excel-on-Work-Updated\_rebuild\decompiled_v9'

# All managed DLLs in bin top-level except the app itself
dlls = []
for f in sorted(glob.glob(os.path.join(bin_dir, '*.dll'))):
    name = os.path.basename(f)
    if name.lower() == 'exceltranscript.dll':
        continue
    dlls.append(name[:-4])  # strip .dll

framework_refs = ['System','System.Core','System.Data','System.Data.DataSetExtensions',
    'System.Web','System.Web.Extensions','System.Xml','System.Xml.Linq','System.Drawing',
    'System.Configuration','System.ComponentModel.DataAnnotations','System.Net.Http',
    'System.Runtime.Serialization','System.Transactions','System.Security','Microsoft.CSharp',
    'System.IdentityModel','System.Runtime.Caching']

refs = []
for fr in framework_refs:
    refs.append(f'    <Reference Include="{fr}" />')
for d in dlls:
    refs.append(f'    <Reference Include="{d}">\n      <HintPath>..\\..\\Excel-main\\bin\\{d}.dll</HintPath>\n      <Private>False</Private>\n    </Reference>')

csproj = f'''<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Release</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <OutputType>Library</OutputType>
    <RootNamespace>ExcelTranscript</RootNamespace>
    <AssemblyName>ExcelTranscript</AssemblyName>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <Deterministic>true</Deterministic>
    <LangVersion>7.3</LangVersion>
    <OutputPath>bin\\Release\\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <Optimize>true</Optimize>
    <DebugType>pdbonly</DebugType>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <NoWarn>0108;0114;0169;0219;0414;0618;0649;1591;0282;0464</NoWarn>
  </PropertyGroup>
  <ItemGroup>
{chr(10).join(refs)}
  </ItemGroup>
  <ItemGroup>
    <Compile Include="**\\*.cs" />
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\\Microsoft.CSharp.targets" />
</Project>
'''
open(os.path.join(proj_dir,'build.csproj'),'w',encoding='utf-8').write(csproj)
print(f"Wrote build.csproj with {len(dlls)} bin refs + {len(framework_refs)} framework refs")
