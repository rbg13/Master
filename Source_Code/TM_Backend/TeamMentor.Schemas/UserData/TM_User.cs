﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class ValidationRegex
    {        
        public const string Email                = @"^[\w-+\.]{1,}\@([\w-]{1,}\.){1,}[a-zA-Z]{2,4}$";
        public const string PasswordComplexity   = @"((?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{8,256})";
    }
    // This is the TMUser VIEW Object (only to be used for data transfer)
    [DataContract]
    public class TM_User
    {   
        [DataMember][StringLength(30)]              public string	Company		        { get; set; }
        [DataMember][StringLength(30)]              public string	Country		        { get; set; }
        [DataMember][StringLength(30)]              public string	FirstName	        { get; set; }
        [DataMember][StringLength(30)]              public string	LastName	        { get; set; }
        [DataMember][StringLength(30)]              public string	State		        { get; set; }
        [DataMember][StringLength(255)]             public string	Title		        { get; set; }
        [DataMember][Required]	                    public int	    UserId		        { get; set; }
        [DataMember][Required][StringLength(30)]    public string	UserName	        { get; set; }
        
        [DataMember][Required][StringLength(50)]  
        [RegularExpression(ValidationRegex.Email)]	public string	Email		        { get; set; }        
        [DataMember]                                public Int64	CreatedDate	        { get; set; }
        [DataMember]                                public string   CSRF_Token          { get; set; }         
        [DataMember]                                public DateTime ExpirationDate      { get; set; } 
        [DataMember]                                public bool     PasswordExpired     { get; set; } 
        [DataMember]                                public bool     AccountNeverExpires { get; set; }
        [DataMember]                                public bool     UserEnabled         { get; set; } 
        [DataMember]                                public int	    GroupID	            { get; set; }
        [DataMember]                                public List<UserTag>   UserTags	    { get; set; }
    }


    public static class TMUser_ExtensionMethod
    {
        public static List<TM_User> users(this List<TMUser> tmUsers)
        {
            return (from tmUser in tmUsers select tmUser.user()).toList();
        }
        public static TM_User user(this TMUser tmUser)
        {
            if (tmUser.isNull())
                return null;
            var user = new TM_User
            {
                
                Company             = tmUser.Company,
                Email               = tmUser.EMail,
                FirstName           = tmUser.FirstName,
                LastName            = tmUser.LastName,
                Title               = tmUser.Title,
                Country             = tmUser.Country,
                State               = tmUser.State,
                UserId              = tmUser.UserID,
                UserName            = tmUser.UserName,
                CSRF_Token          = tmUser.SecretData.CSRF_Token,                                
                GroupID             = tmUser.GroupID,
                UserTags            = tmUser.UserTags
            };
            try
            {
                
                user.ExpirationDate     = tmUser.AccountStatus.ExpirationDate;
                user.PasswordExpired    = tmUser.AccountStatus.PasswordExpired;
                user.CreatedDate        = (tmUser.Stats.CreationDate) != default(DateTime)
                                            ? tmUser.Stats.CreationDate.ToFileTimeUtc()
                                            : 0;
                user.UserEnabled         = tmUser.AccountStatus.UserEnabled;
                user.AccountNeverExpires = tmUser.AccountStatus.AccountNeverExpires;
            }
            catch (Exception ex)
            {
                ex.log();
                "[TMUser] user(), failed to convert user {0} with id {1}".format(tmUser.UserName, tmUser.UserID);                
            }
            return user;

        }
        public static NewUser newUser(this TM_User user)
        {
            return new NewUser
                {
                    Company     = user.Company,                    
                    Email       = user.Email,
                    Firstname   = user.FirstName,
                    Lastname    = user.LastName,
                    Title       = user.Title,                    
                    Username    = user.UserName,                    
                    Country     = user.Country,
                    State       = user.State,
                    UserTags    = user.UserTags
                };  
        }
    }
}