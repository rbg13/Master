using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.Database;

namespace TeamMentor.CoreLib
{ 					
    //WebServices related to: Config Methods
    public partial class TM_WebServices 
    {		
        
        [WebMethod(EnableSession = true)]                       public string GetTime() 						{   return "...Via Proxy:" + DateTime.Now.str();                        }  	         
        [WebMethod(EnableSession = true)]                       public string Ping(string message)  			{   return "received ping: {0}".format(message);                        }        
        
        //Xml Database Specific
        [WebMethod(EnableSession = true)] [Admin]	            public string XmlDatabase_GetDatabasePath()		{	UserGroup.Admin.demand(); return tmXmlDatabase.path_XmlDatabase();	                            }
        [WebMethod(EnableSession = true)] [Admin]	            public string XmlDatabase_GetLibraryPath()		{	UserGroup.Admin.demand(); return tmXmlDatabase.Path_XmlLibraries;	                            }		
        [WebMethod(EnableSession = true)] [Admin]	            public string XmlDatabase_GetUserDataPath()		{	UserGroup.Admin.demand(); return tmXmlDatabase.UserData.Path_UserData;	                    }		
        [WebMethod(EnableSession = true)] [Admin]	            public string XmlDatabase_ReloadData()			{	UserGroup.Admin.demand(); guiObjectsCacheOk = false; return  tmXmlDatabase.reloadData();  }
        [WebMethod(EnableSession = true)] [Admin]	            public bool   XmlDatabase_IsUsingFileStorage()	{	UserGroup.Admin.demand(); return tmXmlDatabase.usingFileStorage();                              }        
        
        [WebMethod(EnableSession = true)] [Admin]	            public bool   XmlDatabase_ImportLibrary_fromZipFile(string pathToZipFile, string unzipPassword) { return TM_Xml_Database.Current.xmlDB_Libraries_ImportFromZip(pathToZipFile, unzipPassword); }                                                                                                                                     
        //[WebMethod(EnableSession = true)] [Admin]	            public bool   XmlDatabase_SetUserDataPath(string userDataPath)	{	return tmXmlDatabase.UserData.setUserDataPath(userDataPath); }


        [WebMethod(EnableSession = true)] public List<Guid>     XmlDatabase_GuidanceItems_SearchTitleAndHtml(List<Guid> guidanceItemsIds, string searchText)
                                                                                                                                {
                                                                                                                                    var results = TM_Xml_Database.Current.guidanceItems_SearchTitleAndHtml(guidanceItemsIds,searchText);
                                                                                                                                    this.logUserActivity("User Search", "on {0} item(s) for '{1}' with {2} results".format(guidanceItemsIds.size(), searchText,results.size()));
                                                                                                                                    return results;
                                                                                                                                }																																		
        [WebMethod(EnableSession = true)] public string         XmlDatabase_GetGuidanceItemXml(Guid guidanceItemId)	    {	return  TM_Xml_Database.Current.xmlDB_guidanceItemXml(guidanceItemId); }        
        [WebMethod(EnableSession = true)] public string         XmlDatabase_GetGuidanceItemPath(Guid guidanceItemId)	{	return  TM_Xml_Database.Current.xmlDB_guidanceItemPath(guidanceItemId); }                
                                                                    
        [WebMethod(EnableSession = true)] public string         RBAC_CurrentIdentity_Name()				                {	return new UserRoleBaseSecurity().currentIdentity_Name(); }
        [WebMethod(EnableSession = true)] public bool           RBAC_CurrentIdentity_IsAuthenticated()	                {	return new UserRoleBaseSecurity().currentIdentity_IsAuthenticated(); }
        [WebMethod(EnableSession = true)] public List<string>   RBAC_CurrentPrincipal_Roles()		                    {	return new UserRoleBaseSecurity().currentPrincipal_Roles().toList(); }
        [WebMethod(EnableSession = true)] public bool           RBAC_HasRole(string role)					            {	return RBAC_CurrentPrincipal_Roles().contains(role); }
        [WebMethod(EnableSession = true)] public bool           RBAC_IsAdmin()											{	return RBAC_CurrentPrincipal_Roles().contains("Admin"); }        
        
        [WebMethod(EnableSession = true)] [Admin]	            public bool      RBAC_Demand_Admin()						{	UserGroup.Admin.demand() ; return true; }        
        [WebMethod(EnableSession = true)] [EditArticles]	    public bool      RBAC_Demand_EditArticles()					{	UserGroup.Editor.demand(); return true; }
        [WebMethod(EnableSession = true)] [ReadArticles]	    public bool      RBAC_Demand_ReadArticles()					{	UserGroup.Reader.demand(); return true; }
        [WebMethod(EnableSession = true)] [ManageUsers]	        public bool      RBAC_Demand_ManageUsers()					{	UserGroup.Admin.demand() ; return true; }

//        [WebMethod(EnableSession = true)]		                public Guid		SSO_AuthenticateUser(string ssoToken)            {   return new SingleSignOn().authenticateUserBasedOn_SSOToken(ssoToken); }
//        [WebMethod(EnableSession = true)] [Admin]			    public string	SSO_GetSSOTokenForUser(string userName)          {   return new SingleSignOn().getSSOTokenForUser(userName); }
//        [WebMethod(EnableSession = true)] [Admin]			    public TM_User	SSO_GetUserFromSSOToken(string ssoToken)         {   return new SingleSignOn().getUserFromSSOToken(ssoToken).user(); }                
                                                                                                                        						
        [WebMethod(EnableSession = true)] [Admin]	            public string		TMConfigFileLocation()			     {	return userData.tmConfig_Location();             }		
        [WebMethod(EnableSession = true)] [Admin]	            public TMConfig		TMConfigFile()                       {	return TMConfig.Current;              }																					        
        [WebMethod(EnableSession = true)] [Admin]	            public bool		    SetTMConfigFile(TMConfig tmConfig)   {   return userData.tmConfig_SetCurrent(tmConfig); }                                                                                            
        [WebMethod(EnableSession = true)] [Admin] 	            public Firebase_ClientConfig Get_Firebase_ClientConfig() {   return userData.firebase_ClientConfig();  }
        

        // Install libraries from ZIP
        [WebMethod(EnableSession = true)] [Admin]	            public string		TMServerFileLocation()			{	return tmXmlDatabase.Server.tmServer_Location();  }		
        [WebMethod(EnableSession = true)] [Admin]	            public TM_Server		TMServerFile()
                                                                                    {	
                                                                                        return tmXmlDatabase.Server;  
                                                                                    }
        [WebMethod(EnableSession = true)] [Admin]	            public bool		    SetTMServerFile(TM_Server tmServer)
                                                                                    {
                                                                                         tmXmlDatabase.Server = tmServer;
                                                                                         tmXmlDatabase.UserData.Server = tmServer;
                                                                                         return tmServer.tmServer_Save();                                                                                        
                                                                                    }

        [WebMethod(EnableSession = true)] [Admin]	            public string		Get_Libraries_Zip_Folder()
                                                                                    {
                                                                                        var librariesZipsFolder = TMConfig.Current.TMSetup.LibrariesUploadedFiles;
                                                                                        return TM_Xml_Database.Current.path_XmlDatabase().fullPath().pathCombine(librariesZipsFolder).fullPath();
                                                                                    }		
        [WebMethod(EnableSession = true)] [Admin]	            public List<string> Get_Libraries_Zip_Folder_Files()
                                                                                    {
                                                                                        return Get_Libraries_Zip_Folder().files().fileNames();
                                                                                    }																							
        [WebMethod(EnableSession = true)] [Admin]	            public string		Set_Libraries_Zip_Folder(string folder)
                                                                                    {
                                                                                        if (folder.valid())
                                                                                        {
                                                                                            var tmConfig = TMConfig.Current;
                                                                                            tmConfig.TMSetup.LibrariesUploadedFiles = folder;
                                                                                            //folder.createDir();
                                                                                            if (userData.tmConfig_Save())																																										
                                                                                                return "Path set to '{0}' which currently has {1} files".format(folder.fullPath(), folder.files().size());
                                                                                        }
                                                                                        return null;
                                                                                    }

        [WebMethod(EnableSession = true)] [Admin]	           public Guid			GetUploadToken()
                                                                                    {
                                                                                        var uploadToken = Guid.NewGuid();
                                                                                        FileUpload.UploadTokens.Add(uploadToken);
                                                                                        return uploadToken;
                                                                                    }

        [WebMethod(EnableSession = true)] [Admin]	           public string		GetLogs()                           { return PublicDI.log.LogRedirectionTarget.prop("LogData").str() ; }        
        [WebMethod(EnableSession = true)] [Admin]	           public string		ResetLogs()                         { (PublicDI.log.LogRedirectionTarget.prop("LogData") as StringBuilder).Clear() ; return "done"; }                
        
        [WebMethod(EnableSession = true)] [Admin]	           public string		REPL_ExecuteSnippet(string snippet) { return REPL.executeSnippet(snippet);}
        


        //Virtual Articles
        [WebMethod(EnableSession = true)] [Admin]	            public List<VirtualArticleAction>	VirtualArticle_GetCurrentMappings()        
                                                                                    {
                                                                                        UserGroup.Admin.demand();
                                                                                        return TM_Xml_Database.Current.getVirtualArticles().Values.toList();
                                                                                    }				
        [WebMethod(EnableSession = true)] [Admin]	            public VirtualArticleAction			VirtualArticle_Add_Mapping_VirtualId( Guid id, Guid virtualId)
                                                                                    {
                                                                                        UserGroup.Admin.demand();
                                                                                        return TM_Xml_Database.Current.add_Mapping_VirtualId(id, virtualId);																						
                                                                                    }
        [WebMethod(EnableSession = true)] [Admin]	            public VirtualArticleAction			VirtualArticle_Add_Mapping_Redirect (Guid id, string redirectUri)
                                                                                    {
                                                                                        UserGroup.Admin.demand();
                                                                                        return TM_Xml_Database.Current.add_Mapping_Redirect(id, redirectUri.uri());																						
                                                                                    }
        [WebMethod(EnableSession = true)] [Admin]	            public VirtualArticleAction			VirtualArticle_Add_Mapping_ExternalArticle(Guid id, string tmServer, Guid externalId)
                                                                                    {
                                                                                        UserGroup.Admin.demand();
                                                                                        return TM_Xml_Database.Current.add_Mapping_ExternalArticle(id, tmServer, externalId);																						
                                                                                    }			
        [WebMethod(EnableSession = true)] [Admin]	            public VirtualArticleAction			VirtualArticle_Add_Mapping_ExternalService(Guid id, string service, string data)
                                                                                    {
                                                                                        UserGroup.Admin.demand();
                                                                                        return TM_Xml_Database.Current.add_Mapping_ExternalService(id, service, data);																						
                                                                                    }			
        [WebMethod(EnableSession = true)] [Admin]	            public bool		                    VirtualArticle_Remove_Mapping( Guid id)
                                                                                    {
                                                                                        UserGroup.Admin.demand();
                                                                                        return TM_Xml_Database.Current.remove_Mapping_VirtualId(id);																						
                                                                                    }
        [WebMethod(EnableSession = true)] [ReadArticles]        public string					    VirtualArticle_Get_GuidRedirect(Guid id)
                                                                                    {
                                                                                        UserGroup.Reader.demand();
                                                                                        return TM_Xml_Database.Current.get_GuidRedirect(id);																						
                                                                                    }				
        [WebMethod(EnableSession = true)] [ReadArticles]        public TeamMentor_Article		    VirtualArticle_CreateArticle_from_ExternalServiceData(string service, string serviceData)
                                                                                    {
                                                                                        UserGroup.Reader.demand();
                                                                                        return service.createArticle_from_ExternalServiceData(serviceData);																						
                                                                                    }
        

        //Article Guid Mappings
        [WebMethod(EnableSession = true)]		                public Guid getGuidForMapping(string mapping)
                                                                            {
                                                                                return TM_Xml_Database.Current.xmlBD_resolveMappingToArticleGuid(mapping);
                                                                            }
        [WebMethod(EnableSession = true)]		                public bool IsGuidMappedInThisServer(Guid guid)
                                                                            {
                                                                                if (GetGuidanceItemById(guid).notNull())
                                                                                    return true;
                                                                                if (TM_Xml_Database.Current.get_GuidRedirect(guid).valid())
                                                                                    return true;
                                                                                return false;
                                                                            }
    }	
}