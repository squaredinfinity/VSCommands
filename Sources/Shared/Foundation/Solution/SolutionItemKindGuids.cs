using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.Solution
{
    public class SolutionItemKindGuids
    {
        public const string SharedProject_AsString = "{d954291e-2a0b-460d-934e-dc6b0785db48}";
        public static readonly Guid SharedProject = new Guid(SharedProject_AsString);
        public const string CSharpProject_AsString = "{fae04ec0-301f-11d3-bf4b-00c04f79efbc}";
        public static readonly Guid CSharpProject = new Guid(CSharpProject_AsString);
        public const string FSharpProject_AsString = "{f2a71f9b-5d33-465a-a702-920d77279786}";
        public static readonly Guid FSharpProject = new Guid(FSharpProject_AsString);
        public const string VBProject_AsString = "{f184b08f-c81c-45f6-a57f-5abd9991f28f}";
        public static readonly Guid VBProject = new Guid(VBProject_AsString);
        public const string VSA_AsString = "{13b7a3ee-4614-11d3-9bc7-00c04f79de25}";
        public static readonly Guid VSA = new Guid(VSA_AsString);
        public const string ProjectAsSolutionFolder_AsString = "{66a26720-8fb5-11d2-aa7e-00c04f688dde}";
        public static readonly Guid ProjectAsSolutionFolder = new Guid(ProjectAsSolutionFolder_AsString);
        public const string ProjectItemAsSolutionFolder_AsString = "{66a26722-8fb5-11d2-aa7e-00c04f688dde}";
        public static readonly Guid ProjectItemAsSolutionFolder = new Guid(ProjectItemAsSolutionFolder_AsString);
        public const string VisualCPPProject_AsString = "{8bc9ceb8-8b4a-11d0-8d11-00a0c91bc942}";
        public static readonly Guid VisualCPPProject = new Guid(VisualCPPProject_AsString);
        public const string VisualJSharpProject_AsString = "{e6fdf86b-f3d1-11d4-8576-0002a516ece8}";
        public static readonly Guid VisualJSharpProject = new Guid(VisualJSharpProject_AsString);
        public const string WebProject_AsString = "{e24c65dc-7377-472b-9aba-bc803b73c61a}";
        public static readonly Guid WebProject = new Guid(WebProject_AsString);
        public const string WebApplicationProject_AsString = "{349c5851-65df-11da-9384-00065b846f21}";
        public static readonly Guid WebApplicationProject = new Guid(WebApplicationProject_AsString);
        public const string Project_AsString = "{66a26722-8fb5-11d2-aa7e-00c04f688dde}";
        public static readonly Guid Project = new Guid(Project_AsString);
        public const string Folder_AsString = "{6bb5f8ef-4483-11d3-8bcf-00c04f8ec28c}";
        public static readonly Guid Folder = new Guid(Folder_AsString);
        public const string File_AsString = "{6bb5f8ee-4483-11d3-8bcf-00c04f8ec28c}";
        public static readonly Guid File = new Guid(File_AsString);
        public const string MicsFolder_AsString = "{66a2671d-8fb5-11d2-aa7e-00c04f688dde}";
        public static readonly Guid MicsFolder = new Guid(MicsFolder_AsString);
        public const string SetupProject_AsString = "{54435603-dbb4-11d2-8724-00a0c9a8b90c}";
        public static readonly Guid SetupProject = new Guid(SetupProject_AsString);

        /// <summary>
        /// Note that some projects will return 'unmodeled' (e.g. database project, web deployment projects, unlodaded projects)
        /// </summary>
        public const string UnmodeledProject_AsString = "{67294a52-a4f0-11d2-aa88-00c04f688dde}";
        public static readonly Guid UnmodeledProject = new Guid(UnmodeledProject_AsString);
    }
}
