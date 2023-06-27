using DataTypeStore;
using SFM;
using System.IO.Compression;
using System.Numerics;
using TASI;

namespace Codename_TALaT_CS
{

    /// <summary>
    /// Basically everything that makes a package
    /// </summary>
    public class Story
    {
        public List<InternalFileEmulation> codeFiles;
        public List<InternalFileEmulation> descriptions;
        public List<SupportedLanguage> languages;

        //Settings
        public string storyName;
        public InternalFileEmulation storyStart;
        public string storyDefaultLangVal;
        public bool storyUseLauncherFunctions;
        public string storyVer;
        public string updatesVer;
        public string updatesPackage;
        public bool storyActive = true;
        public RSAKeygenLib.KeyPair storyCreatorKey;
        public string updatesRandomStoryID;

        enum Properties
        {
            storyUseLauncherFunctions,
            storyHasUpdating,
            updatesNeedSignature,
            hasNoSignature,
            installThirdPartyLibs,
            useDefaultLanguageConfig,
            useCustomName,
            useCustomDescription
        }
        //Settings End

        public Story() 
        {
            codeFiles = new();
            descriptions = new();
            languages = new();
        }
        public void RunStory()
        {
            Global.InitInternalNamespaces();
            Global.internalFiles = codeFiles;

            LoadFile.RunCode(LoadFile.ByPath(storyStart.path));
        }
        public async void UpdateStory()
        {
            HttpClient downloader = new HttpClient();
            Log.Shared.LogE($"Downloading ver for story {storyName}");
            string response = await downloader.GetStringAsync(updatesVer);
            if (response == storyVer)
            {
                Log.Shared.LogE($"No update found for story {storyName}");
                return;
            }
            Log.Shared.LogE($"Update found for story {storyName}");
            Log.Shared.LogE($"Clearing story {storyName}");
            ClearStory();
            Log.Shared.LogE($"Downloading package from story {storyName}");

            var zipFile = await downloader.GetByteArrayAsync(updatesPackage);

            string tempFile = Path.GetTempFileName();

            File.WriteAllBytes(tempFile, zipFile);

            Log.Shared.LogE($"Importing story {storyName}");
            try
            {
                ImportPackage(tempFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update was faulty. The previous version cannot be restored. Removing assets");
            }




        }

        public void ClearStory()
        {
            codeFiles.ForEach(x => x.Content = null);
            codeFiles = new();
            descriptions.ForEach(x => x.Content = null);
            descriptions = new();
            languages = new();
        }

        /// <summary>
        /// Imports a story using a story package.
        /// </summary>
        /// <param name="packagePath"></param>
        /// 


        public Story(string packagePath)
        {

            ImportPackage(packagePath);


        }

        private void ImportPackage(string packagePath)
        {

            packagePath = packagePath.RemoveWorking('\"');
            //Tempdir setup
            Log.Shared.LogL("Setting up temp dir");
            string tempDir = Path.Combine(Path.GetTempPath(), $"TALaT-StoryImport-{Random.Shared.Next()}");
            Directory.CreateDirectory(tempDir);
            try
            {
                ZipFile.ExtractToDirectory(packagePath, tempDir, true);
                Log.Shared.LogL("Checking signature: Importing key");
                

                Region signatureRegion = Read.TopLevelRegion(File.ReadAllText(Path.Combine(tempDir, "Story\\Signature")).Split(';'))[0];
                storyCreatorKey = new(signatureRegion.FindSubregionWithName("KeyPair"));

                Log.Shared.LogL("Checking signature: Hashing");
                BigInteger hash = BigInteger.Parse( HashFolderUtils.HashFolder(new string[] { Path.Combine(tempDir, "Story\\Code"), Path.Combine(tempDir, "Story\\Meta") }), System.Globalization.NumberStyles.HexNumber);
                
                Log.Shared.LogL("Checking signature: Validating");
                Log.Shared.LogE("Decrypted hash: " + storyCreatorKey.CryptUsingKeypair(BigInteger.Parse(signatureRegion.FindDirectValue("signedHash").value)));

                if (storyCreatorKey.CryptUsingKeypair( BigInteger.Parse(signatureRegion.FindDirectValue("signedHash").value)) == hash) 
                {
                    Log.Shared.LogE("Signature valid.");

                }

                Log.Shared.LogL("Importing code files");
                codeFiles = LoadAllFilesFromBaseDir(Path.Combine(tempDir, "Story\\Code"), "\\");
                Log.Shared.LogL("Importing description files");
                descriptions = LoadAllFilesFromBaseDir(Path.Combine(tempDir, "Story\\Meta\\Description"), "", true);
                //Importing files done

                Log.Shared.LogL("Loading supported languages");
                languages = new();
                File.ReadAllText(Path.Combine(tempDir, "Story\\Meta\\SupportedLanguages"))
                    .RemoveWorking('\r')
                    .RemoveWorking('\n')
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .ForEach(x => languages.Add(new(x)));
                Log.Shared.LogL("Loading launcher metadata");

                foreach (string line in File.ReadAllText(Path.Combine(tempDir, "Story\\Meta\\Launcher")).RemoveWorking('\r').RemoveWorking('\n').Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    string command = "";

                    foreach (char c in line)
                    {
                        if (c == '=')
                        {
                            break;
                        }
                        else
                        {
                            command += c;
                        }
                    }

                    string value = line.Substring(command.Length + 1);

                    switch (command.ToLower())
                    {
                        case "storyname":
                            storyName = value;
                            break;
                        case "storystart":
                            InternalFileEmulation? storyStart = codeFiles.FirstOrDefault(x => x.path.ToLower() == value.ToLower(), null);
                            if (storyStart == null)
                                throw new Exception("Launcher_Metadata.Not_Found_Start_Path: The file, that was set as the story start, doesn't exist in the Code folder.");
                            this.storyStart = storyStart;
                            break;
                        case "storydefaultlangval":
                            if (!languages.Any(x => x.internalReference == value))
                                throw new Exception("Launcher_Metadata.Not_Found_Internal_Language: The language, that was explained as storyDefaultLangVar isn't said to be a supported language.");
                            storyDefaultLangVal = value;
                            break;
                        case "storyuselauncherfunctions":
                            storyUseLauncherFunctions = bool.Parse(value);
                            break;
                        case "updatesver":
                            updatesVer = value;
                            break;
                        case "updatespackage":
                            updatesPackage = value;
                            break;
                        case "updatesrandomstoryid":
                            updatesRandomStoryID = $"{value}";
                            break;
                        case "storyver":
                            storyVer = value;
                            break;
                        default:
                            throw new Exception("Launcher_Metadata.Unknown_Metadata_Variable");

                    }
                }
                Log.Shared.LogL("Finishing import");
                storyStart.path = codeFiles.First(x => x.Hash == storyStart.Hash).path;


                Log.Shared.LogE("Story successfully imported!");
            }
            catch (Exception ex)
            {
                DeleteStory();
                throw;
            }
            finally
            {
                Log.Shared.LogL("Cleaning up");
                Directory.Delete(tempDir, true);
            }
        }

        public void DeleteStory()
        {

            codeFiles?.ForEach(x => x.Content = null);
            descriptions?.ForEach(x => x.Content = null);
            if (storyStart != null)
                storyStart.Content = null;
            storyActive = false;
        }
        /// <summary>
        /// Self-calling function, to load all files in a directory into the save-system and then loads all the directorys by calling itself.
        /// </summary>
        /// <param name="directory">The directory you want to load</param>
        /// <param name="basePathName">The base to the "path" variable you want it to be referred to (It will be expanded using the filenames and subdirectorys)</param>
        /// <returns></returns>
        private static List<InternalFileEmulation> LoadAllFilesFromBaseDir(string directory, string basePathName, bool noSubDirs = false)
        {
            List<InternalFileEmulation> result = new();
            foreach (string file in Directory.GetFiles(directory))
            {
                result.Add(new(basePathName + Path.GetFileName(file), File.ReadAllText(file)));
            }
            foreach (string dir in Directory.GetDirectories(directory))
            {
                if (noSubDirs) throw new Exception("LoadFiles.No_Subdirs_Allowed: Incorrect format");
                result.AddRange(LoadAllFilesFromBaseDir(dir, basePathName + "\\" + dir));
            }
            return result;
        }
        public Region SaveStory
        {
            get
            {
                Region result = new("Story");
                result.SubRegions.Add(InternalFileEmulation.SaveInternalFiles(codeFiles, "codeFiles"));
                result.SubRegions.Add(InternalFileEmulation.SaveInternalFiles(descriptions, "descriptions"));
                languages.ForEach(x => result.SubRegions.Add(x.SaveLanguage));

                result.SubRegions.Add(storyCreatorKey.Save);

                //Save settings
                result.directValues.Add(new("storyName", storyName, false));
                result.directValues.Add(new("storyStart", storyStart.Hash, false));
                result.directValues.Add(new("storyDefaultLangVal", storyDefaultLangVal, false));
                result.directValues.Add(new("storyUseLauncherFunctions", storyUseLauncherFunctions.ToString(), false));
                result.directValues.Add(new("storyVer", storyVer, false));
                result.directValues.Add(new("updatesVer", updatesVer, false));
                result.directValues.Add(new("updatesPackage", updatesPackage, false));
                result.directValues.Add(new("updatesRandomStoryID", updatesRandomStoryID, false));

                //Save settings end

                return result;
            }
        }

        public Story(Region region)
        {
            codeFiles = InternalFileEmulation.LoadInternalFiles(region.FindSubregionWithName("codeFiles"));
            descriptions = InternalFileEmulation.LoadInternalFiles(region.FindSubregionWithName("descriptions"));
            languages = new();
            region.FindSubregionWithNameArray("sLang").ToList().ForEach(x => languages.Add(new(x)));

            storyCreatorKey = new(region.FindSubregionWithName("KeyPair"));

            //Settings
            storyName = region.FindDirectValue("storyName").value;
            storyStart = InternalFileEmulation.CreateWithHash("", region.FindDirectValue("storyStart").value);
            storyDefaultLangVal = region.FindDirectValue("storyDefaultLangVal").value;
            storyUseLauncherFunctions = Convert.ToBoolean(region.FindDirectValue("storyUseLauncherFunctions").value);
            storyVer = region.FindDirectValue("storyVer").value;
            updatesVer = region.FindDirectValue("updatesVer").value;
            updatesPackage = region.FindDirectValue("updatesPackage").value;
            updatesRandomStoryID = region.FindDirectValue("updatesRandomStoryID").value;
            storyStart.path = codeFiles.First(x => x.Hash == storyStart.Hash).path;



        }



    }

    public class SupportedLanguage
    {


        public List<string> launcherLanguage;
        public string storyLanguage;
        public string internalReference;

        public SupportedLanguage(string parse)
        {
            string[] parts = parse.Trim(' ').Split('=');

            if (parts.Length != 2) throw new Exception("SupportedLanguage.Equals_Formating: There's something wrong wíth the formating of the \"SupportedLanguages\" file.");

            launcherLanguage = parts[0].Split(',').ToList();
            string[] storySide = parts[1].Split(':');

            if (storySide.Length != 2) throw new Exception("SupportedLanguage.Colon_Formating: There's something wrong wíth the formating of the \"SupportedLanguages\" file.");

            storyLanguage = storySide[0];
            internalReference = storySide[1];
        }

        public SupportedLanguage(List<string> launcherLanguage, string storyLanguage, string internalReference)
        {
            this.launcherLanguage = launcherLanguage ?? throw new ArgumentNullException(nameof(launcherLanguage));
            this.storyLanguage = storyLanguage ?? throw new ArgumentNullException(nameof(storyLanguage));
            this.internalReference = internalReference ?? throw new ArgumentNullException(nameof(internalReference));
        }
        public SupportedLanguage(Region region)
        {
            launcherLanguage = new();
            region.FindDirectValueArray("ll").ToList().ForEach(x => launcherLanguage.Add(x.value));
            storyLanguage = region.FindDirectValue("sl").value;
            internalReference = region.FindDirectValue("ir").value;
        }

        public Region SaveLanguage
        {
            get
            {
                Region result = new("sLang");
                launcherLanguage.ForEach(x => result.directValues.Add(new("ll", x, false)));
                result.directValues.Add(new("sl", storyLanguage, false));
                result.directValues.Add(new("ir", internalReference, false));
                return result;
            }
        }
    }
}
