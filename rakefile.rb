require 'albacore'
require 'nuget_helper'

$dir = File.dirname(__FILE__)
$sln = File.join($dir, "Pollock.sln")

desc "Install missing NuGet packages."
task :restore do
  sh("paket restore")
end

desc "build"
build :build => [:restore] do |msb|
  msb.prop :configuration, :Debug
  msb.prop :platform, "Any CPU"
  msb.target = :Rebuild
  msb.be_quiet
  msb.nologo
  msb.sln = $sln
end

task :default => ['build']

$test_files = Dir.glob(File.join($dir, "*Tests", "**", "bin", "Debug", "*Tests.dll"))


desc "test using nunit console"
test_runner :test => [:build] do |nunit|
  nunit.exe = NugetHelper.nunit_path
  nunit.files = $test_files 
end

desc "test using nunit console"
test_runner :mono_test => [:build] do |nunit|
  nunit.exe = NugetHelper.nunit_path
  nunit.files = $test_files
  nunit.parameters.add("-exclude=not_mono")
end

desc "create the nuget packages"
task :pack => [:build, :pollock_pack, :pollock_json_pack]

task :pollock_pack => [:build] do |nuget|
  cd File.join($dir, "Pollock") do
    NugetHelper::exec "pack Pollock.csproj"
  end
end

task :pollock_json_pack => [:build] do |nuget|
  cd File.join($dir, "Pollock.Newtonsoft.Json") do
    NugetHelper::exec "pack Pollock.Newtonsoft.Json.csproj -IncludeReferencedProjects"
  end
end

task :pollock_push do |nuget|
  cd File.join($dir, "Pollock") do
    latest = NugetHelper.last_version(Dir.glob("Pollock.*.nupkg"))
    NugetHelper::exec("push #{latest}")
  end
end

task :pollock_json_push do |nuget|
  cd File.join($dir, "Pollock.Newtonsoft.Json") do
    latest = NugetHelper.last_version(Dir.glob("Pollock.Newtonsoft.Json.*.nupkg"))
    NugetHelper::exec("push #{latest}")
  end
end