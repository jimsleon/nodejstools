﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.NodejsTools.Npm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NpmTests
{

    [TestClass]
    public class ModuleHierarchyTests : AbstractFilesystemPackageJsonTests
    {

        protected const string PkgSingleDependency = @"{
    ""name"": ""TestPkg"",
    ""version"": ""0.1.0"",
    ""dependencies"": {
        ""sax"": "">=0.1.0 <0.2.0""
    }
}";

        private string CreateRootPackage(string json)
        {
            var dir = TempFileManager.GetNewTempDirectory();
            var path = Path.Combine(dir.FullName, "package.json");
            CreatePackageJson(path, json);
            return dir.FullName;
        }

        [TestMethod]
        public void TestReadRootPackageNoDependencies()
        {
            var rootDir = CreateRootPackage(PkgSimple);
            var pkg = RootPackageFactory.Create(rootDir);
            Assert.IsNotNull(pkg, "Root package should not be null.");
            Assert.AreEqual(rootDir, pkg.Path, "Package path mismatch.");
            var json = pkg.PackageJson;
            Assert.IsNotNull(json, "package.json should not be null.");
            Assert.AreEqual(json.Name, pkg.Name, "Package name mismatch.");
            Assert.AreEqual(json.Version, pkg.Version, "Package version mismatch.");
            var modules = pkg.Modules;
            Assert.IsNotNull(modules, "Modules should not be null.");
            Assert.AreEqual(0, modules.Count, "Module count mismatch.");
        }

        [TestMethod]
        public void TestReadRootPackageOneDependency()
        {
            var rootDir = CreateRootPackage(PkgSingleDependency);

            var info = new ProcessStartInfo("npm", "install") {WorkingDirectory = rootDir};
            var p = new Process {StartInfo = info};
            p.Start();
            p.WaitForExit();

            var pkg = RootPackageFactory.Create(rootDir);

            var json = pkg.PackageJson;
            var dependencies = json.AllDependencies;
            Assert.AreEqual(1, dependencies.Count, "Dependency count mismatch.");

            IDependency dep = dependencies["sax"];
            Assert.IsNotNull(dep, "sax dependency should not be null.");
            Assert.AreEqual(">=0.1.0 <0.2.0", dep.VersionRangeText, "Version range mismatch.");

            var modules = pkg.Modules;
            Assert.AreEqual(1, modules.Count, "Module count mismatch");

            IPackage module = modules[0];
            Assert.IsNotNull(module, "Module should not be null when retrieved by index.");
            module = modules["sax"];
            Assert.IsNotNull(module, "Module should not be null when retrieved by name.");

            Assert.AreEqual(modules[0], modules["sax"], "Modules should be same whether retrieved by name or index.");

            Assert.AreEqual("sax", module.Name, "Module name mismatch.");

            //  All of these should be indicated, in some way, in the Visual Studio treeview.

            Assert.IsNotNull(module.PackageJson, "Module package.json should not be null.");

            Assert.IsTrue(module.IsDependencyInParentPackageJson, "Should be listed as a dependency in parent package.json.");
            Assert.IsFalse(module.IsMissing, "Should not be marked as missing.");
            Assert.IsFalse(module.IsDevDependency, "Should not be marked as dev dependency.");
            Assert.IsFalse(module.IsOptionalDependency, "Should not be marked as optional dependency.");
            Assert.IsFalse(module.IsBundledDependency, "Should not be marked as bundled dependency.");

            //  Redundant?
            Assert.IsTrue(module.HasPackageJson, "Module should have its own package.json");
        }

        [TestMethod]
        public void TestReadRootPackageMissingDependency()
        {
            var rootDir = CreateRootPackage(PkgSingleDependency);

            var pkg = RootPackageFactory.Create(rootDir);

            var json = pkg.PackageJson;
            var dependencies = json.AllDependencies;
            Assert.AreEqual(1, dependencies.Count, "Dependency count mismatch.");

            IDependency dep = dependencies["sax"];
            Assert.IsNotNull(dep, "sax dependency should not be null.");
            Assert.AreEqual(">=0.1.0 <0.2.0", dep.VersionRangeText, "Version range mismatch.");

            var modules = pkg.Modules;
            Assert.AreEqual(1, modules.Count, "Module count mismatch");

            IPackage module = modules[0];
            Assert.IsNotNull(module, "Module should not be null when retrieved by index.");
            module = modules["sax"];
            Assert.IsNotNull(module, "Module should not be null when retrieved by name.");

            Assert.AreEqual(modules[0], modules["sax"], "Modules should be same whether retrieved by name or index.");

            Assert.AreEqual("sax", module.Name, "Module name mismatch.");

            //  All of these should be indicated, in some way, in the Visual Studio treeview.

            Assert.IsNull(module.PackageJson, "Module package.json should be null for missing dependency.");

            Assert.IsTrue(module.IsDependencyInParentPackageJson, "Should be listed as a dependency in parent package.json.");
            Assert.IsTrue(module.IsMissing, "Should be marked as missing.");
            Assert.IsFalse(module.IsDevDependency, "Should not be marked as dev dependency.");
            Assert.IsFalse(module.IsOptionalDependency, "Should not be marked as optional dependency.");
            Assert.IsFalse(module.IsBundledDependency, "Should not be marked as bundled dependency.");

            //  Redundant?
            Assert.IsFalse(module.HasPackageJson, "Missing module should not have its own package.json");
        }

    }
}
