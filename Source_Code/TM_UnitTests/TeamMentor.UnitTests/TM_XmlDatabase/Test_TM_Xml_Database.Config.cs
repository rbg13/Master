﻿using NUnit.Framework;
using System;
using TeamMentor.CoreLib;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.Database;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_TM_Xml_Database_Config
    {
        public Test_TM_Xml_Database_Config()
        {
            UserGroup.Admin.assert();
        }
        [TearDown] public void teardown()
        {
            TM_Server.WebRoot = AppDomain.CurrentDomain.BaseDirectory;            
        }
        
        [Test] public void userData()
        {
            var tmXmlDatabase = new TM_Xml_Database();
            Assert.IsNull(tmXmlDatabase.UserData);
            var userData = tmXmlDatabase.userData();                //first time should create a new instance of TM_UserData
            Assert.NotNull(userData);
            Assert.AreEqual(userData, tmXmlDatabase.UserData);
            Assert.AreEqual(userData, tmXmlDatabase.userData());    // 2nd time should NOT create a new instance
            
            tmXmlDatabase = null;
            Assert.IsNull(tmXmlDatabase.userData(), null);
        }
        [Test] public void load_UserData()
        {
            var tmXmlDatabase = new TM_Xml_Database().useFileStorage();

            Assert.IsNull(tmXmlDatabase.UserData);
            
            tmXmlDatabase.Server.Users_Create_Default_Admin = false;

            tmXmlDatabase.load_UserData();

            var userData = tmXmlDatabase.userData();

            
            Assert.NotNull (userData);
            Assert.AreEqual(userData, tmXmlDatabase.UserData);
            Assert.NotNull (userData.Path_UserData);            
            Assert.IsEmpty (userData.TMUsers);
            
            tmXmlDatabase.delete_Database();
                        
        }
        [Test] public void load_TM_Config()
        {
            TMConfig.Current = null;
            var tmXmlDatabase = new TM_Xml_Database();                                                     
            
            tmXmlDatabase.userData().tmConfig_Load();

            //if Path_UserData is null, then tmConfig_Location is also null

            Assert.NotNull(tmXmlDatabase.userData());
            Assert.IsNull(tmXmlDatabase.userData().Path_UserData);            
            Assert.IsNull(tmXmlDatabase.userData().tmConfig_Location());
            Assert.NotNull(TMConfig.Current);

        }
        [Test] public void tm_Server_Load__In_Memory()
        {            
            var tmServer        = new TM_Server().setDefaultData(); 
            var tmXmlDatabase   = new TM_Xml_Database(tmServer);            

            Assert.NotNull (tmXmlDatabase.Server);
            Assert.AreEqual(tmXmlDatabase.Server.toXml(), tmServer.toXml()); // check if the value matches a new object of type TM_Server
        }
        [Test] public void tm_Server_Load_UsingFileStorage()
        {            
            
            var baseReadOnlyDir = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"site\wwwroot";        // simulates use of AppData
            TM_Server.WebRoot = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();

            var tmXmlDatabase    = new TM_Xml_Database().useFileStorage();
            

            var tmServerFile     = tmXmlDatabase.Server.tmServer_Location();
            var expectedLocation = tmXmlDatabase.path_XmlDatabase().pathCombine(TMConsts.TM_SERVER_FILENAME);

            Assert.IsNotNull(tmXmlDatabase.path_XmlDatabase());    
            Assert.IsTrue   (TM_Status.Current.TM_Database_Location_Using_AppData);
            Assert.NotNull  (tmXmlDatabase.Server);
            Assert.IsTrue   (tmServerFile.fileExists());
            Assert.AreEqual(tmServerFile, expectedLocation);
                     

/*            Assert.Ignore("TO FIX (Refactor Side Effect");
            var tmServer_withDefaultData = new TM_Server().setDefaultData();
            Assert.AreEqual(tmXmlDatabase.Server.toXml(), tmServer_withDefaultData.toXml());

            //make a change, saved it and ensure it gets loaded ok

            var tmpName1 = 10.randomLetters();
            var tmpName2 = 10.randomLetters();
            tmXmlDatabase.Server.UserData_Configs.first().Name = tmpName1;
            tmXmlDatabase.Server.tmServer_Save();
            tmXmlDatabase.Server.UserData_Configs.first().Name = tmpName2;

            tmXmlDatabase.Server.tmServer_Load();
            Assert.AreEqual   (tmXmlDatabase.Server.UserData_Configs.first().Name, tmpName1);
            Assert.AreNotEqual(tmXmlDatabase.Server.toXml(), tmServer_withDefaultData.toXml());

            //Try loading up an corrupted tmServer (whcih will default to load a default TM_Server
            "aaaa".saveAs(tmServerFile);
            tmXmlDatabase.Server.tmServer_Load();
            Assert.AreNotEqual(tmXmlDatabase.Server.UserData_Configs.first().Name, tmpName1);
            Assert.AreEqual   (tmXmlDatabase.Server.toXml(), tmServer_withDefaultData.toXml());
            */
            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse(baseReadOnlyDir.dirExists());
            tmXmlDatabase.delete_Database();
        }
        [Test] public void tm_Server_Save()
        {                        
            var tmXmlDatabase            = new TM_Xml_Database().useFileStorage();

            //tmXmlDatabase.set_Path_XmlDatabase()
            //             .tmServer_Load();
            Assert.NotNull(tmXmlDatabase.path_XmlDatabase());      

            var tmServerLocation         = tmXmlDatabase.Server.tmServer_Location();
           
            var tmServer_withDefaultData = new TM_Server().setDefaultData();             

                  
            Assert.IsTrue(tmServerLocation.fileExists());    
        
            Assert.Ignore("TO FIX (Refactor Side Effect");

            Assert.AreEqual(tmXmlDatabase.Server.toXml(), tmServer_withDefaultData.toXml());

            var tmpName1 = 10.randomLetters();
            
            tmXmlDatabase.Server.UserData_Configs.first().Name = tmpName1;
            Assert.IsTrue(tmXmlDatabase.Server.tmServer_Save());
            Assert.AreEqual(tmServerLocation.load<TM_Server>().UserData_Configs.first().Name, tmpName1);   //check that it was  saved

            /*
             // this works but makes the test run in 10s (and the test being done is if there is no exception)
                
                var tmpName2 = 10.randomLetters();             
                tmServerLocation.fileInfo()
                        .setAccessControl("Users", FileSystemRights.Write, AccessControlType.Deny);

                tmXmlDatabase.Server.UserData_Configs.first().Name = tmpName2;
            
                Assert.IsFalse(tmXmlDatabase.tmServer_Save());

                Assert.AreEqual(tmServerLocation.load<TM_Server>().UserData_Configs.first().Name, tmpName1);   //check that it was not saved

                Assert.IsTrue(tmServerLocation.delete_File());
                Assert.IsFalse(tmServerLocation.fileExists());
             */
            tmXmlDatabase.delete_Database();
            Assert.IsFalse(tmServerLocation.fileExists());
            Assert.IsFalse(tmXmlDatabase.path_XmlDatabase().dirExists());

            //check when not UsingFileStorage

            //check for nulls            
            TM_Server.Path_XmlDatabase = null;
            Assert.IsFalse(tmXmlDatabase.Server.tmServer_Save());
            Assert.IsFalse(new TM_Xml_Database().Server.tmServer_Save());
        }
        [Test] public void set_Path_UserData()
        {
            var tmXmlDatabase = new TM_Xml_Database().useFileStorage();                        

            var expectedPath  = tmXmlDatabase.path_XmlDatabase().pathCombine(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA);
            
            tmXmlDatabase.set_Path_UserData();
            var userData = tmXmlDatabase.UserData;

            //Assert.NotNull (tmServer);
            Assert.NotNull (userData);
            Assert.AreEqual(userData.Path_UserData, expectedPath);
            Assert.True    (userData.Path_UserData.dirExists());

           
            // try with a different Name value
            var tempName = 10.randomLetters(); 
            tmXmlDatabase.Server.userData_Config().Name = tempName;
            tmXmlDatabase.set_Path_UserData();            
            Assert.IsTrue(tmXmlDatabase.userData().Path_UserData.contains(tempName));
            Assert.IsTrue(tmXmlDatabase.UserData.usingFileStorage());


            tmXmlDatabase.delete_Database();
            Assert.False   (userData.Path_UserData.dirExists());

            //check bad data handling
            tmXmlDatabase.Server.userData_Config().Name = null;
            tmXmlDatabase.set_Path_UserData();                  
            Assert.IsTrue(tmXmlDatabase.userData().Path_UserData.contains(TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA));
            

            tmXmlDatabase.Server.userData_Config().Name = "aaa:bbb"; // will fail to create the UserData folder and force memory mode
            tmXmlDatabase.set_Path_UserData();                  
            Assert.IsNotNull (tmXmlDatabase.UserData);
            Assert.IsNull    (tmXmlDatabase.UserData.Path_UserData);
            Assert.IsFalse   (tmXmlDatabase.UserData.usingFileStorage());

            //test nulls
            tmXmlDatabase.Server = null;                   
            tmXmlDatabase.set_Path_UserData();
            Assert.IsNull(tmXmlDatabase.userData().Path_UserData);

            tmXmlDatabase = null;
            Assert.IsNull(tmXmlDatabase.set_Path_UserData());

            Assert.IsNull(new TM_Xml_Database().set_Path_UserData().UserData.Path_UserData);


        }
        [Test] public void set_Path_XmlDatabase__In_Memory()
        {
            var tmXmlDatabase1 = new TM_Xml_Database();
            
            var tmServer = tmXmlDatabase1.Server;
            Assert.AreEqual(tmServer, tmServer.set_Path_XmlDatabase());
            Assert.AreEqual(tmXmlDatabase1, TM_Xml_Database.Current);
            Assert.IsNull  (tmXmlDatabase1.path_XmlDatabase());
            Assert.IsFalse (tmXmlDatabase1.usingFileStorage());

            var tmXmlDatabase2 = new TM_Xml_Database();
            Assert.AreNotEqual(tmXmlDatabase1, tmXmlDatabase2, "A new tmXmlDatabase1 should had been created");            
            Assert.AreEqual   (tmXmlDatabase2, TM_Xml_Database.Current);
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage()
        {            
            var tmXmlDatabase = new TM_Xml_Database().useFileStorage();                     
            Assert.AreEqual   (tmXmlDatabase.Server, tmXmlDatabase.Server.set_Path_XmlDatabase());
            Assert.AreEqual   (tmXmlDatabase, TM_Xml_Database.Current);
            Assert.IsTrue     (tmXmlDatabase.usingFileStorage());
            Assert.IsNotNull  (tmXmlDatabase.path_XmlDatabase());            

            tmXmlDatabase.delete_Database();
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_Custom_WebRoot()
        {
            Assert.AreEqual(TM_Server.WebRoot, AppDomain.CurrentDomain.BaseDirectory);
            TM_Server.WebRoot = "_tmp_webRoot".tempDir().info();            
            
            TM_Server.WebRoot.delete_Folder();
            Assert.IsFalse(TM_Server.WebRoot.dirExists());

            var tmXmlDatabase = new TM_Xml_Database().useFileStorage();            
            
            tmXmlDatabase.Server.set_Path_XmlDatabase();

            Assert.IsTrue  (tmXmlDatabase.path_XmlDatabase().dirExists(), "db ctor should create a library folder");   

            var usingAppDataFolder = TM_Status.Current.TM_Database_Location_Using_AppData;
            
            "*** DB path: {0}".info(tmXmlDatabase.path_XmlDatabase());
            "*** Lib path: {0}".info(tmXmlDatabase.Path_XmlLibraries);
            "*** Current WebRoot: {0}".debug(TM_Server.WebRoot);
            "*** Current WebRoot exists: {0}".debug(TM_Server.WebRoot.dirExists());
            "*** TM_Status.Current.TM_Database_Location_Using_AppData: {0}".debug(TM_Status.Current.TM_Database_Location_Using_AppData);

            Assert.AreEqual(usingAppDataFolder, TM_Server.WebRoot.dirExists()       , "db ctor should not create a Web Root (if it doesn't exist)");
            Assert.AreEqual(usingAppDataFolder, tmXmlDatabase.path_XmlDatabase().contains ("App_Data"));
            Assert.AreEqual(usingAppDataFolder, tmXmlDatabase.path_XmlDatabase().contains (TM_Server.WebRoot));
            Assert.AreEqual(usingAppDataFolder, tmXmlDatabase.path_XmlDatabase().contains (PublicDI.config.O2TempDir));

            tmXmlDatabase.delete_Database();

            Assert.AreEqual(usingAppDataFolder, TM_Server.WebRoot.dirExists()  , "if not usingAppDataFolder the TM_Server.WebRoot shouldn't exist");
            Assert.IsFalse(tmXmlDatabase.path_XmlDatabase().dirExists()          , "should had been deleted");            
        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_Custom_WebRoot_without_Read_Privs()
        {
            var baseReadOnlyDir   = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"virtual/path";
            TM_Server.WebRoot     = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();

            //Check that ensure we can write to baseReadOnlyDir
            Assert.IsTrue  (baseReadOnlyDir.dirExists());
            Assert.IsTrue  (TM_Server.WebRoot.dirExists());
            Assert.IsTrue  (TM_Server.WebRoot.contains(baseReadOnlyDir));
            Assert.IsTrue  (baseReadOnlyDir.canWriteToPath());
            Assert.AreEqual(TM_Server.WebRoot.parentFolder().parentFolder(), baseReadOnlyDir);

            //Now remote the write privileges for all users (on baseReadOnlyDir) while keeping  TM_Server.WebRoot writeable
            
            baseReadOnlyDir  .directoryInfo().deny_Write_Users();
            TM_Server.WebRoot.directoryInfo().allow_Write_Users();   
            
            Assert.IsFalse(baseReadOnlyDir .canWriteToPath());
            Assert.IsTrue(TM_Server.WebRoot.canWriteToPath());

            //Since baseReadOnlyDir can be written, creating an TM_Xml_Database should now default to the App_Data folder (which is on webRootVirualPath )

            var tmXmlDatabase = new TM_Xml_Database().useFileStorage();
            
            tmXmlDatabase.Server.set_Path_XmlDatabase();

            Assert.IsNotNull(tmXmlDatabase.path_XmlDatabase());
            Assert.IsTrue   (tmXmlDatabase.usingFileStorage());

            Assert.Ignore("TO FIX (Refactor Side Effect");
            Assert.IsTrue   (tmXmlDatabase.path_XmlDatabase().contains("App_Data"));
            Assert.IsTrue   (tmXmlDatabase.path_XmlDatabase().contains(TM_Server.WebRoot));
            Assert.IsTrue   (tmXmlDatabase.path_XmlDatabase().contains(PublicDI.config.O2TempDir));

            //Finally re enable write so that we can delete the folder
            baseReadOnlyDir.directoryInfo().allow_Write_Users();
            Assert.IsTrue(baseReadOnlyDir.canWriteToPath());
            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse  (baseReadOnlyDir.dirExists());

        }
        [Test] public void set_Path_XmlDatabase__UsingFileStorage_On_AppData__without_Read_Privs()
        {
            var baseReadOnlyDir = "_tmp_webRoot".tempDir();
            var webRootVirualPath = @"site\wwwroot";        // simulates use of AppData
            TM_Server.WebRoot = baseReadOnlyDir.pathCombine(webRootVirualPath).createDir();

            TM_Server.WebRoot.directoryInfo().deny_CreateDirectories_Users(); 
             
            var tmXmlDatabase = new TM_Xml_Database().useFileStorage();       // usually a true paramater will set UsingFileStorage to true
            
            tmXmlDatabase.Server.set_Path_XmlDatabase();

            Assert.IsNull (tmXmlDatabase.path_XmlDatabase());      // if we can't write to the AppData folder then this value can't be set automatically            
            Assert.IsFalse(tmXmlDatabase.usingFileStorage());      // and the offline mode (i.e. UsingFileStorage = false) should be activated
            Files.deleteFolder(baseReadOnlyDir, true);
            Assert.IsFalse(baseReadOnlyDir.dirExists());
        }
        [Test] public void set_Path_XmlLibraries()
        {
            var tmXmlDatabase = new TM_Xml_Database().useFileStorage();
            tmXmlDatabase.Server.set_Path_XmlDatabase();

            Assert.NotNull(TMConfig.Current);

            tmXmlDatabase.set_Path_XmlLibraries();

            Assert.NotNull(tmXmlDatabase.path_XmlDatabase());
            Assert.NotNull(tmXmlDatabase.Path_XmlLibraries);
            Assert.IsTrue (tmXmlDatabase.path_XmlDatabase().dirExists());
            Assert.IsTrue (tmXmlDatabase.Path_XmlLibraries.dirExists());

            tmXmlDatabase.delete_Database();
            
        }
        [Test]
        public void set_Default_Values()
        {            
            var tmDatabase = new TM_Xml_Database();

            var events     = tmDatabase.Events;           // this value should not change

            tmDatabase.set_Default_Values();

            Assert.NotNull  (tmDatabase);
            Assert.False    (tmDatabase.usingFileStorage());                        
            
            Assert.IsEmpty  (tmDatabase.Cached_GuidanceItems);
            Assert.IsEmpty  (tmDatabase.GuidanceItems_FileMappings);
            Assert.IsEmpty  (tmDatabase.GuidanceExplorers_XmlFormat);            
            Assert.IsEmpty  (tmDatabase.GuidanceExplorers_Paths);           
            Assert.IsEmpty  (tmDatabase.VirtualArticles);            
            Assert.AreEqual (tmDatabase.Events, events);                    // check that events object has not changed after set_Default_Values    

            Assert.IsNull   (tmDatabase.path_XmlDatabase());
            Assert.IsNull   (tmDatabase.Path_XmlLibraries);
            Assert.IsNull   (tmDatabase.UserData);
            Assert.IsNotNull(tmDatabase.Server);
        }

    }
}