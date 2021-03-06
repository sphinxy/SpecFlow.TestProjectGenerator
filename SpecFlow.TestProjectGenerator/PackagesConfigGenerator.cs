﻿using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow.TestProjectGenerator.Data;
using TechTalk.SpecFlow.TestProjectGenerator.Extensions;
using TechTalk.SpecFlow.TestProjectGenerator.Factories;
using TechTalk.SpecFlow.TestProjectGenerator.Helpers;

namespace TechTalk.SpecFlow.TestProjectGenerator
{
    public class PackagesConfigGenerator : XmlFileGeneratorBase
    {
        private readonly ProjectFileFactory _projectFileFactory = new ProjectFileFactory();

        public ProjectFile Generate(IEnumerable<NuGetPackage> nuGetPackages, TargetFramework targetFramework)
        {
            using (var ms = new MemoryStream())
            {
                using (var xw = GenerateDefaultXmlWriter(ms))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("packages");

                    string tfm = targetFramework == 0 ? null : targetFramework.ToTargetFrameworkMoniker();

                    foreach (var package in nuGetPackages)
                    {
                        xw.WriteStartElement("package");
                        xw.WriteAttributeString("id", package.Name);
                        xw.WriteAttributeString("version", package.Version);

                        if (package.AllowedVersions.IsNotNullOrWhiteSpace())
                        {
                            xw.WriteAttributeString("allowedVersions", package.AllowedVersions);
                        }

                        if (!(tfm is null))
                        {
                            xw.WriteAttributeString("targetFramework", tfm);
                        }

                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                    xw.Flush();

                    return _projectFileFactory.FromStream(ms, "packages.config", "None");
                }
            }
        }
    }
}
